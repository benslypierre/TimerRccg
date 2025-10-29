using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace TimerRccg
{
	public class WebControlServer
	{
		private readonly HttpListener _listener;
		private readonly ITimerService _timerService;
		private readonly IScheduleService _scheduleService;
		private readonly Control _uiControl;
		private volatile bool _isRunning;
		private const string BASE_URL = "http://localhost:8080/";

		public WebControlServer(ITimerService timerService, IScheduleService scheduleService, Control uiControl)
		{
			_timerService = timerService ?? throw new ArgumentNullException(nameof(timerService));
			_scheduleService = scheduleService ?? throw new ArgumentNullException(nameof(scheduleService));
			_uiControl = uiControl ?? throw new ArgumentNullException(nameof(uiControl));
			_listener = new HttpListener();
			_listener.Prefixes.Add(BASE_URL);
			// Also listen on all interfaces for LAN access
			_listener.Prefixes.Add("http://+:8080/");
		}

		public void Start()
		{
			if (_isRunning) return;
			try
			{
				// Ensure URL ACL and firewall are configured (no-op if not elevated)
				EnsureUrlAclAndFirewall();
				_listener.Start();
				_isRunning = true;
				_listener.BeginGetContext(ProcessRequest, null);
			}
			catch (Exception ex)
			{
				_isRunning = false;
				try { _listener.Stop(); _listener.Close(); } catch { }
				MessageBox.Show("Failed to start WebControlServer on port 8080. " + ex.Message, "Web Server Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		public void Stop()
		{
			if (!_isRunning) return;
			_isRunning = false;
			try
			{
				_listener.Stop();
				_listener.Close();
			}
			catch { }
		}

		private void ProcessRequest(IAsyncResult ar)
		{
			HttpListenerContext context = null;
			try
			{
				context = _listener.EndGetContext(ar);
				var request = context.Request;
				var response = context.Response;

				string path = request.Url.AbsolutePath.TrimEnd('/');
				if (path == string.Empty) path = "/";

				switch (path)
				{
					case "/":
						ServeHtmlPage(response);
						break;
					case "/status" when request.HttpMethod.Equals("GET", StringComparison.OrdinalIgnoreCase):
						HandleStatusRequest(response);
						break;
					case "/next" when request.HttpMethod.Equals("POST", StringComparison.OrdinalIgnoreCase):
						HandleNextRequest(response);
						break;
					case "/previous" when request.HttpMethod.Equals("POST", StringComparison.OrdinalIgnoreCase):
						HandlePreviousRequest(response);
						break;
					default:
						response.StatusCode = 404;
						WriteResponse(response, "Not Found", "text/plain");
						break;
				}

				if (_isRunning)
				{
					_listener.BeginGetContext(ProcessRequest, null);
				}
			}
			catch
			{
				// Swallow to keep server alive; optionally log
			}
		}

		private static bool IsAdmin()
		{
			try
			{
				var id = System.Security.Principal.WindowsIdentity.GetCurrent();
				var p = new System.Security.Principal.WindowsPrincipal(id);
				return p.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
			}
			catch { return false; }
		}

		private static void TryRun(string file, string args)
		{
			try
			{
				var psi = new System.Diagnostics.ProcessStartInfo
				{
					FileName = file,
					Arguments = args,
					CreateNoWindow = true,
					UseShellExecute = false,
					RedirectStandardOutput = true,
					RedirectStandardError = true
				};
				using (var p = System.Diagnostics.Process.Start(psi))
				{
					p.WaitForExit(4000);
				}
			}
			catch { }
		}

		private static void EnsureUrlAclAndFirewall()
		{
			if (!IsAdmin()) return;
			// URL ACL for all hosts on 8080
			TryRun("netsh", "http add urlacl url=http://+:8080/ user=Everyone");
			// Firewall rule for this executable
			try
			{
				var exe = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
				TryRun("netsh", "advfirewall firewall add rule name=\"TimerRccg Remote\" dir=in action=allow program=\"" + exe + "\" enable=yes profile=any");
			}
			catch { }
		}

		private void ServeHtmlPage(HttpListenerResponse response)
		{
			response.StatusCode = 200;
			var html = @"<!DOCTYPE html>
<html lang=""en"">
<head>
<meta charset=""utf-8"" />
<meta name=""viewport"" content=""width=device-width, initial-scale=1"" />
<title>Timer Remote Control</title>
<style>
  :root { --bg:#0f172a; --fg:#e2e8f0; --muted:#94a3b8; --accent:#22c55e; --danger:#ef4444; }
  body { margin:0; font-family:Segoe UI,Roboto,Helvetica,Arial,sans-serif; background:var(--bg); color:var(--fg); }
  .container { max-width:900px; margin:0 auto; padding:24px; }
  .card { background:#111827; border:1px solid #1f2937; border-radius:12px; padding:20px; }
  .header { display:flex; align-items:center; justify-content:space-between; gap:12px; }
  .title { font-size:20px; font-weight:600; color:var(--muted); }
  .time { font-size:48px; font-weight:800; letter-spacing:1px; }
  .grid { display:grid; grid-template-columns:1fr 1fr; gap:16px; margin-top:16px; }
  @media (max-width: 700px) { .grid { grid-template-columns:1fr; } }
  .queue { max-height:320px; overflow:auto; }
  ul { list-style:none; padding:0; margin:0; }
  li { padding:10px 12px; border-bottom:1px solid #1f2937; display:flex; justify-content:space-between; gap:12px; }
  .controls { display:flex; gap:12px; margin-top:12px; }
  button { cursor:pointer; border:0; border-radius:8px; padding:12px 16px; font-size:16px; font-weight:600; }
  .btn { background:#1f2937; color:var(--fg); }
  .btn:hover { background:#374151; }
  .btn-next { background:var(--accent); color:#052e16; }
  .btn-prev { background:#3b82f6; color:#00122a; }
  .error { color:var(--danger); margin-top:8px; min-height:24px; }
</style>
</head>
<body>
  <div class=""container"">
    <div class=""card"">
      <div class=""header"">
        <div>
          <div class=""title"">Current Item</div>
          <div id=""currentTitle"" style=""font-size:24px; font-weight:700;"">&nbsp;</div>
        </div>
        <div id=""time"" class=""time"">--:--</div>
      </div>
      <div class=""controls"">
        <button id=""prev"" class=""btn btn-prev"">Previous</button>
        <button id=""next"" class=""btn btn-next"">Next</button>
      </div>
      <div id=""error"" class=""error""></div>
      <div class=""grid"">
        <div class=""card"">
          <div class=""title"">Queue</div>
          <div class=""queue"">
            <ul id=""queue""></ul>
          </div>
        </div>
        <div class=""card"">
          <div class=""title"">Status</div>
          <div id=""status""></div>
        </div>
      </div>
    </div>
  </div>
<script>
async function fetchStatus() {
  try {
    const r = await fetch('/status');
    if (!r.ok) throw new Error('Failed to fetch status');
    const s = await r.json();
    document.getElementById('currentTitle').textContent = s.currentItem ? s.currentItem.title : '';
    const mm = String(s.timer.minutes).padStart(2,'0');
    const ss = String(s.timer.seconds).padStart(2,'0');
    document.getElementById('time').textContent = s.timer.isCompleted ? 'Time Up' : `${mm}:${ss}`;
    const q = document.getElementById('queue');
    q.innerHTML = '';
    (s.queue || []).forEach((item, idx) => {
      const li = document.createElement('li');
      const left = document.createElement('div'); left.textContent = item.title;
      const right = document.createElement('div'); right.textContent = `${item.timeInMinutes} min`;
      if (idx === s.currentIndex) { li.style.background = '#0b1220'; }
      li.appendChild(left); li.appendChild(right); q.appendChild(li);
    });
    document.getElementById('status').textContent = `Running: ${s.timer.isRunning}`;
    document.getElementById('error').textContent = '';
  } catch (e) {
    document.getElementById('error').textContent = e.message;
  }
}

async function post(path) {
  try {
    const r = await fetch(path, { method: 'POST' });
    const j = await r.json();
    if (!j.success) throw new Error(j.message || 'Action failed');
    await fetchStatus();
  } catch (e) {
    document.getElementById('error').textContent = e.message;
  }
}

document.getElementById('next').addEventListener('click', () => post('/next'));
document.getElementById('prev').addEventListener('click', () => post('/previous'));

fetchStatus();
setInterval(fetchStatus, 2000);
</script>
</body>
</html>";
			WriteResponse(response, html, "text/html");
		}

		private void HandleStatusRequest(HttpListenerResponse response)
		{
			string json = "{}";
			try
			{
				_uiControl.Invoke(new Action(() =>
				{
					var current = _scheduleService.GetCurrentItem();
					var payload = new
					{
						currentItem = current == null ? null : new { title = current.Title, timeInMinutes = current.TimeInMinutes },
						queue = _scheduleService.ScheduleItems.Select(x => new { title = x.Title, timeInMinutes = x.TimeInMinutes }).ToArray(),
						timer = new { minutes = _timerService.Minutes, seconds = _timerService.Seconds, title = _timerService.Title, isRunning = _timerService.IsRunning, isCompleted = _timerService.IsCompleted },
						currentIndex = _scheduleService.CurrentIndex
					};
					json = JsonConvert.SerializeObject(payload);
				}));
				response.StatusCode = 200;
			}
			catch (Exception ex)
			{
				response.StatusCode = 500;
				json = JsonConvert.SerializeObject(new { error = ex.Message });
			}
			WriteResponse(response, json, "application/json");
		}

		private void HandleNextRequest(HttpListenerResponse response)
		{
			string json;
			try
			{
				_uiControl.Invoke(new Action(() =>
				{
					if (_scheduleService.CurrentIndex < _scheduleService.ScheduleItems.Count - 1)
					{
						_scheduleService.CurrentIndex++;
						var currentItem = _scheduleService.GetCurrentItem();
						if (currentItem != null)
						{
							_timerService.Minutes = currentItem.TimeInMinutes;
							_timerService.Seconds = 0;
							_timerService.Title = currentItem.Title;
							_timerService.Start();
							Form2.Instance.titleUpdate();
							(_uiControl as Form1)?.UpdateMiniText();
						}
					}
					else
					{
						throw new InvalidOperationException("This is the last program left.");
					}
				}));
				response.StatusCode = 200;
				json = JsonConvert.SerializeObject(new { success = true });
			}
			catch (Exception ex)
			{
				response.StatusCode = ex is InvalidOperationException ? 409 : 500;
				json = JsonConvert.SerializeObject(new { success = false, message = ex.Message });
			}
			WriteResponse(response, json, "application/json");
		}

		private void HandlePreviousRequest(HttpListenerResponse response)
		{
			string json;
			try
			{
				_uiControl.Invoke(new Action(() =>
				{
					if (_scheduleService.CurrentIndex > 0)
					{
						_scheduleService.CurrentIndex--;
						var currentItem = _scheduleService.GetCurrentItem();
						if (currentItem != null)
						{
							_timerService.Minutes = currentItem.TimeInMinutes;
							_timerService.Seconds = 0;
							_timerService.Title = currentItem.Title;
							_timerService.Start();
							Form2.Instance.titleUpdate();
							(_uiControl as Form1)?.UpdateMiniText();
						}
					}
					else
					{
						throw new InvalidOperationException("There is no Previous program before this.");
					}
				}));
				response.StatusCode = 200;
				json = JsonConvert.SerializeObject(new { success = true });
			}
			catch (Exception ex)
			{
				response.StatusCode = ex is InvalidOperationException ? 409 : 500;
				json = JsonConvert.SerializeObject(new { success = false, message = ex.Message });
			}
			WriteResponse(response, json, "application/json");
		}

		private void WriteResponse(HttpListenerResponse response, string content, string contentType)
		{
			try
			{
				var bytes = Encoding.UTF8.GetBytes(content ?? string.Empty);
				response.ContentType = contentType;
				response.ContentEncoding = Encoding.UTF8;
				response.ContentLength64 = bytes.Length;
				using (var output = response.OutputStream)
				{
					output.Write(bytes, 0, bytes.Length);
				}
			}
			catch { }
		}
	}
}


