using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
					case "/bethel-logo.png" when request.HttpMethod.Equals("GET", StringComparison.OrdinalIgnoreCase):
						ServeLogoPng(response);
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
					case "/timer/add" when request.HttpMethod.Equals("POST", StringComparison.OrdinalIgnoreCase):
						HandleTimerAddRequest(request, response);
						break;
					case "/timer/subtract" when request.HttpMethod.Equals("POST", StringComparison.OrdinalIgnoreCase):
						HandleTimerSubtractRequest(request, response);
						break;
					case "/queue/add" when request.HttpMethod.Equals("POST", StringComparison.OrdinalIgnoreCase):
						HandleQueueAddRequest(request, response);
						break;
					case "/queue/remove" when request.HttpMethod.Equals("POST", StringComparison.OrdinalIgnoreCase):
						HandleQueueRemoveRequest(request, response);
						break;
					case "/queue/move" when request.HttpMethod.Equals("POST", StringComparison.OrdinalIgnoreCase):
						HandleQueueMoveRequest(request, response);
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

		private static void ServeLogoPng(HttpListenerResponse response)
		{
			try
			{
				var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bethel-logo.png");
				if (!File.Exists(path))
				{
					response.StatusCode = 404;
					WriteStaticResponse(response, "Not Found", "text/plain");
					return;
				}
				var bytes = File.ReadAllBytes(path);
				response.StatusCode = 200;
				response.ContentType = "image/png";
				response.ContentLength64 = bytes.Length;
				using (var output = response.OutputStream)
				{
					output.Write(bytes, 0, bytes.Length);
				}
			}
			catch
			{
				response.StatusCode = 500;
				try { WriteStaticResponse(response, "Error", "text/plain"); } catch { }
			}
		}

		private static void WriteStaticResponse(HttpListenerResponse response, string content, string contentType)
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

		private void ServeHtmlPage(HttpListenerResponse response)
		{
			response.StatusCode = 200;
			var html = @"<!DOCTYPE html>
<html lang=""en"">
<head>
<meta charset=""utf-8"" />
<meta name=""viewport"" content=""width=device-width, initial-scale=1"" />
<title>RCCG Bethel Timer · Remote</title>
<style>
  :root { --bg:#0f172a; --fg:#e2e8f0; --muted:#94a3b8; --accent:#22c55e; --danger:#ef4444; }
  *, *::before, *::after { box-sizing:border-box; }
  body { margin:0; font-family:Segoe UI,Roboto,Helvetica,Arial,sans-serif; background:var(--bg); color:var(--fg); overflow-x:hidden; }
  .container { max-width:900px; margin:0 auto; padding:24px; width:100%; min-width:0; }
  .brand-strip { display:flex; align-items:center; gap:10px; margin-bottom:14px; padding-bottom:12px; border-bottom:1px solid #1f2937; min-width:0; }
  .brand-strip img { width:28px; height:28px; object-fit:contain; border-radius:6px; flex-shrink:0; display:block; }
  .brand-strip .brand-name { font-size:14px; font-weight:600; color:var(--fg); letter-spacing:0.02em; white-space:nowrap; overflow:hidden; text-overflow:ellipsis; }
  .card { background:#111827; border:1px solid #1f2937; border-radius:12px; padding:20px; min-width:0; max-width:100%; }
  .header { display:flex; align-items:center; justify-content:space-between; gap:12px; min-width:0; }
  .header > div:first-child { min-width:0; flex:1; overflow:hidden; }
  .title { font-size:20px; font-weight:600; color:var(--muted); }
  .time { font-size:48px; font-weight:800; letter-spacing:1px; flex-shrink:0; }
  .queue-section { margin-top:16px; width:100%; min-width:0; max-width:100%; }
  .queue { max-height:400px; overflow:auto; overflow-x:hidden; width:100%; min-width:0; }
  ul { list-style:none; padding:0; margin:0; }
  li { padding:10px 12px; border-bottom:1px solid #1f2937; display:flex; justify-content:flex-start; align-items:center; gap:8px; min-width:0; }
  .controls { display:flex; gap:12px; margin-top:12px; }
  button { cursor:pointer; border:0; border-radius:8px; padding:12px 16px; font-size:16px; font-weight:600; }
  .btn { background:#1f2937; color:var(--fg); }
  .btn:hover { background:#374151; }
  .btn-next { background:var(--accent); color:#052e16; }
  .btn-prev { background:#3b82f6; color:#00122a; }
  .btn-danger { background:#7f1d1d; color:#fecaca; }
  .btn-danger:hover { background:#991b1b; }
  .btn-small { padding:8px 10px; font-size:14px; }
  .error { color:var(--danger); margin-top:8px; min-height:24px; }
  .toolbar { display:flex; flex-wrap:wrap; align-items:center; gap:8px; margin-top:12px; padding:12px; background:#0b1220; border-radius:8px; border:1px solid #1f2937; }
  .toolbar label { color:var(--muted); font-size:14px; }
  .toolbar input[type=""number""], .toolbar input[type=""text""] { background:#111827; border:1px solid #374151; color:var(--fg); border-radius:6px; padding:8px 10px; font-size:15px; width:72px; }
  .toolbar input[type=""text""] { flex:1; min-width:140px; }
  .queue li { flex-wrap:nowrap; }
  .queue .qi-title { flex:1 1 0; min-width:0; overflow:hidden; text-overflow:ellipsis; white-space:nowrap; }
  .queue .qi-time { flex:0 0 auto; color:var(--muted); width:4.5rem; text-align:right; font-variant-numeric:tabular-nums; }
  .queue-actions { display:flex; gap:4px; align-items:center; flex:0 0 auto; }
  .queue .btn-queue-rm { flex:0 0 auto; }
  .queue .btn-icon { padding:6px 8px; min-width:2rem; font-size:15px; line-height:1.1; }
  button:disabled { opacity:0.35; cursor:not-allowed; }
</style>
</head>
<body>
  <div class=""container"">
    <div class=""brand-strip"">
      <img src=""/bethel-logo.png"" width=""28"" height=""28"" alt="""" />
      <span class=""brand-name"">RCCG Bethel Timer</span>
    </div>
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
      <div class=""toolbar"">
        <label for=""adjMin"">Adjust current timer (min)</label>
        <input id=""adjMin"" type=""number"" min=""1"" value=""1"" />
        <button type=""button"" id=""adjAdd"" class=""btn btn-next btn-small"">Add</button>
        <button type=""button"" id=""adjSub"" class=""btn btn-prev btn-small"">Subtract</button>
        <button type=""button"" class=""btn btn-small"" data-add=""1"">+1</button>
        <button type=""button"" class=""btn btn-small"" data-add=""5"">+5</button>
        <button type=""button"" class=""btn btn-small"" data-sub=""1"">−1</button>
        <button type=""button"" class=""btn btn-small"" data-sub=""5"">−5</button>
      </div>
      <div class=""toolbar"">
        <label>Add to queue</label>
        <input id=""newTitle"" type=""text"" placeholder=""Title"" autocomplete=""off"" />
        <input id=""newMins"" type=""number"" min=""0"" value=""5"" />
        <span style=""color:var(--muted)"">min</span>
        <button type=""button"" id=""addQueue"" class=""btn btn-next btn-small"">Add item</button>
      </div>
      <div id=""error"" class=""error""></div>
      <div class=""card queue-section"">
        <div class=""title"">Queue</div>
        <div class=""queue"">
          <ul id=""queue""></ul>
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
    const list = s.queue || [];
    const lastIdx = list.length - 1;
    list.forEach((item) => {
      const li = document.createElement('li');
      const left = document.createElement('div'); left.className = 'qi-title'; left.textContent = item.title; left.title = item.title;
      const mid = document.createElement('div'); mid.className = 'qi-time'; mid.textContent = item.timeInMinutes + ' min';
      const actions = document.createElement('div'); actions.className = 'queue-actions';
      const up = document.createElement('button'); up.type = 'button'; up.className = 'btn btn-small btn-icon'; up.textContent = '\u2191'; up.title = 'Move up';
      const down = document.createElement('button'); down.type = 'button'; down.className = 'btn btn-small btn-icon'; down.textContent = '\u2193'; down.title = 'Move down';
      if (item.index === 0) up.disabled = true;
      if (item.index === lastIdx) down.disabled = true;
      up.addEventListener('click', (ev) => { ev.preventDefault(); postJson('/queue/move', JSON.stringify({ index: item.index, direction: -1 })); });
      down.addEventListener('click', (ev) => { ev.preventDefault(); postJson('/queue/move', JSON.stringify({ index: item.index, direction: 1 })); });
      actions.appendChild(up); actions.appendChild(down);
      const rm = document.createElement('button'); rm.type = 'button'; rm.className = 'btn btn-danger btn-small btn-queue-rm'; rm.textContent = 'Remove';
      rm.addEventListener('click', (ev) => { ev.preventDefault(); postJson('/queue/remove', JSON.stringify({ index: item.index })); });
      if (item.index === s.currentIndex) { li.style.background = '#0b1220'; }
      li.appendChild(left); li.appendChild(mid); li.appendChild(actions); li.appendChild(rm); q.appendChild(li);
    });
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

async function postJson(url, jsonBody) {
  try {
    const r = await fetch(url, { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: jsonBody });
    const j = await r.json();
    if (!j.success) throw new Error(j.message || 'Action failed');
    await fetchStatus();
  } catch (e) {
    document.getElementById('error').textContent = e.message;
  }
}

function readAdjMinutes() {
  const v = parseInt(document.getElementById('adjMin').value, 10);
  return Number.isFinite(v) && v > 0 ? v : 1;
}

document.getElementById('next').addEventListener('click', () => post('/next'));
document.getElementById('prev').addEventListener('click', () => post('/previous'));
document.getElementById('adjAdd').addEventListener('click', () => post('/timer/add?minutes=' + encodeURIComponent(String(readAdjMinutes()))));
document.getElementById('adjSub').addEventListener('click', () => post('/timer/subtract?minutes=' + encodeURIComponent(String(readAdjMinutes()))));
document.querySelectorAll('[data-add]').forEach((b) => b.addEventListener('click', () => post('/timer/add?minutes=' + encodeURIComponent(b.getAttribute('data-add')))));
document.querySelectorAll('[data-sub]').forEach((b) => b.addEventListener('click', () => post('/timer/subtract?minutes=' + encodeURIComponent(b.getAttribute('data-sub')))));
document.getElementById('addQueue').addEventListener('click', () => {
  const title = document.getElementById('newTitle').value.trim();
  const minutes = parseInt(document.getElementById('newMins').value, 10);
  if (!title) { document.getElementById('error').textContent = 'Enter a title for the new queue item.'; return; }
  if (!Number.isFinite(minutes) || minutes < 0) { document.getElementById('error').textContent = 'Enter a valid number of minutes (0 or more).'; return; }
  postJson('/queue/add', JSON.stringify({ title: title, minutes: minutes }));
  document.getElementById('newTitle').value = '';
});

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
						queue = _scheduleService.ScheduleItems.Select((x, i) => new { index = i, title = x.Title, timeInMinutes = x.TimeInMinutes }).ToArray(),
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

		private static int ParsePositiveMinutesFromRequest(HttpListenerRequest request, string body)
		{
			var q = request.QueryString["minutes"];
			if (!string.IsNullOrEmpty(q) && int.TryParse(q, out int fromQuery) && fromQuery > 0)
				return fromQuery;
			if (!string.IsNullOrWhiteSpace(body))
			{
				var o = JObject.Parse(body);
				var m = o["minutes"];
				if (m != null && m.Type != JTokenType.Null)
				{
					var v = m.ToObject<int>();
					if (v > 0) return v;
				}
			}
			throw new ArgumentException("Provide a positive integer \"minutes\" in the query string or JSON body.");
		}

		private void HandleTimerAddRequest(HttpListenerRequest request, HttpListenerResponse response)
		{
			string json;
			try
			{
				string body;
				using (var reader = new StreamReader(request.InputStream, request.ContentEncoding ?? Encoding.UTF8))
					body = reader.ReadToEnd();

				var minutes = ParsePositiveMinutesFromRequest(request, body);

				_uiControl.Invoke(new Action(() =>
				{
					_timerService.AddTime(minutes);
					var cur = _scheduleService.GetCurrentItem();
					if (cur != null)
					{
						var idx = _scheduleService.CurrentIndex;
						_scheduleService.EditItem(idx, cur.Title, cur.TimeInMinutes + minutes);
					}
					_scheduleService.SaveSchedule();
					Form2.Instance.titleUpdate();
					(_uiControl as Form1)?.UpdateMiniText();
				}));
				response.StatusCode = 200;
				json = JsonConvert.SerializeObject(new { success = true });
			}
			catch (Exception ex)
			{
				response.StatusCode = ex is ArgumentException ? 400 : 500;
				json = JsonConvert.SerializeObject(new { success = false, message = ex.Message });
			}
			WriteResponse(response, json, "application/json");
		}

		private void HandleTimerSubtractRequest(HttpListenerRequest request, HttpListenerResponse response)
		{
			string json;
			try
			{
				string body;
				using (var reader = new StreamReader(request.InputStream, request.ContentEncoding ?? Encoding.UTF8))
					body = reader.ReadToEnd();

				var minutes = ParsePositiveMinutesFromRequest(request, body);

				_uiControl.Invoke(new Action(() =>
				{
					if (_timerService.Minutes < minutes)
						throw new InvalidOperationException("Unable to subtract — not enough time remaining on the timer.");

					_timerService.SubtractTime(minutes);
					var cur = _scheduleService.GetCurrentItem();
					if (cur != null)
					{
						var idx = _scheduleService.CurrentIndex;
						var newMins = Math.Max(0, cur.TimeInMinutes - minutes);
						_scheduleService.EditItem(idx, cur.Title, newMins);
					}
					_scheduleService.SaveSchedule();
					Form2.Instance.titleUpdate();
					(_uiControl as Form1)?.UpdateMiniText();
				}));
				response.StatusCode = 200;
				json = JsonConvert.SerializeObject(new { success = true });
			}
			catch (Exception ex)
			{
				response.StatusCode = ex is InvalidOperationException ? 409 : ex is ArgumentException ? 400 : 500;
				json = JsonConvert.SerializeObject(new { success = false, message = ex.Message });
			}
			WriteResponse(response, json, "application/json");
		}

		private void HandleQueueAddRequest(HttpListenerRequest request, HttpListenerResponse response)
		{
			string json;
			try
			{
				string body;
				using (var reader = new StreamReader(request.InputStream, request.ContentEncoding ?? Encoding.UTF8))
					body = reader.ReadToEnd();
				if (string.IsNullOrWhiteSpace(body))
					throw new ArgumentException("JSON body required with \"title\" and \"minutes\".");

				var o = JObject.Parse(body);
				var title = o["title"]?.Value<string>();
				var minutesToken = o["minutes"];
				if (minutesToken == null || minutesToken.Type == JTokenType.Null)
					throw new ArgumentException("\"minutes\" must be a non-negative integer.");
				var minutes = minutesToken.ToObject<int>();
				if (string.IsNullOrWhiteSpace(title))
					throw new ArgumentException("\"title\" is required.");
				if (minutes < 0)
					throw new ArgumentException("\"minutes\" must be non-negative.");

				_uiControl.Invoke(new Action(() =>
				{
					_scheduleService.AddItem(title.Trim(), minutes);
					_scheduleService.SaveSchedule();
					Form2.Instance.titleUpdate();
					(_uiControl as Form1)?.UpdateMiniText();
				}));
				response.StatusCode = 200;
				json = JsonConvert.SerializeObject(new { success = true });
			}
			catch (Exception ex)
			{
				response.StatusCode = ex is ArgumentException ? 400 : 500;
				json = JsonConvert.SerializeObject(new { success = false, message = ex.Message });
			}
			WriteResponse(response, json, "application/json");
		}

		private void HandleQueueRemoveRequest(HttpListenerRequest request, HttpListenerResponse response)
		{
			string json;
			try
			{
				string body;
				using (var reader = new StreamReader(request.InputStream, request.ContentEncoding ?? Encoding.UTF8))
					body = reader.ReadToEnd();

				int index;
				var q = request.QueryString["index"];
				if (!string.IsNullOrEmpty(q) && int.TryParse(q, out int fromQuery))
					index = fromQuery;
				else if (!string.IsNullOrWhiteSpace(body))
				{
					var o = JObject.Parse(body);
					var ix = o["index"];
					if (ix == null || ix.Type != JTokenType.Integer)
						throw new ArgumentException("Provide \"index\" in the query string or JSON body.");
					index = ix.Value<int>();
				}
				else
					throw new ArgumentException("Provide \"index\" in the query string or JSON body.");

				_uiControl.Invoke(new Action(() =>
				{
					if (index < 0 || index >= _scheduleService.ScheduleItems.Count)
						throw new InvalidOperationException("Index is out of range.");
					_scheduleService.DeleteItem(index);
					_scheduleService.SaveSchedule();
					Form2.Instance.titleUpdate();
					(_uiControl as Form1)?.UpdateMiniText();
				}));
				response.StatusCode = 200;
				json = JsonConvert.SerializeObject(new { success = true });
			}
			catch (Exception ex)
			{
				response.StatusCode = ex is InvalidOperationException ? 409 : ex is ArgumentException ? 400 : 500;
				json = JsonConvert.SerializeObject(new { success = false, message = ex.Message });
			}
			WriteResponse(response, json, "application/json");
		}

		private void HandleQueueMoveRequest(HttpListenerRequest request, HttpListenerResponse response)
		{
			string json;
			try
			{
				string body;
				using (var reader = new StreamReader(request.InputStream, request.ContentEncoding ?? Encoding.UTF8))
					body = reader.ReadToEnd();

				int index;
				int direction;
				var qi = request.QueryString["index"];
				var qd = request.QueryString["direction"];
				if (!string.IsNullOrEmpty(qi) && int.TryParse(qi, out int idxQ) &&
				    !string.IsNullOrEmpty(qd) && int.TryParse(qd, out int dirQ) && (dirQ == -1 || dirQ == 1))
				{
					index = idxQ;
					direction = dirQ;
				}
				else if (!string.IsNullOrWhiteSpace(body))
				{
					var o = JObject.Parse(body);
					var ix = o["index"];
					var dir = o["direction"];
					if (ix == null || ix.Type == JTokenType.Null || dir == null || dir.Type == JTokenType.Null)
						throw new ArgumentException("JSON body must include \"index\" and \"direction\" (-1 = up, 1 = down).");
					index = ix.ToObject<int>();
					direction = dir.ToObject<int>();
					if (direction != -1 && direction != 1)
						throw new ArgumentException("\"direction\" must be -1 (move up) or 1 (move down).");
				}
				else
					throw new ArgumentException("Provide \"index\" and \"direction\" in the query string or JSON body.");

				_uiControl.Invoke(new Action(() =>
				{
					if (index < 0 || index >= _scheduleService.ScheduleItems.Count)
						throw new InvalidOperationException("Index is out of range.");
					_scheduleService.MoveItem(index, direction);
					_scheduleService.SaveSchedule();
					Form2.Instance.titleUpdate();
					(_uiControl as Form1)?.UpdateMiniText();
				}));
				response.StatusCode = 200;
				json = JsonConvert.SerializeObject(new { success = true });
			}
			catch (Exception ex)
			{
				response.StatusCode = ex is InvalidOperationException ? 409 : ex is ArgumentException ? 400 : 500;
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


