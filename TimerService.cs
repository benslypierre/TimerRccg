using System;
using System.Windows.Forms;

namespace TimerRccg
{
    public class TimerService : ITimerService
    {
        private Timer _timer;
        private int _minutes = 0;
        private int _seconds = 0;
        private string _title = "";
        private bool _isRunning = false;
        private bool _isCompleted = false;

        public TimerService()
        {
            _timer = new Timer();
            _timer.Interval = 1000; // 1 second
            _timer.Tick += Timer_Tick;
        }

        public int Minutes
        {
            get => _minutes;
            set => _minutes = Math.Max(0, value);
        }

        public int Seconds
        {
            get => _seconds;
            set => _seconds = Math.Max(0, Math.Min(59, value));
        }

        public string Title
        {
            get => _title;
            set => _title = value ?? "";
        }

        public bool IsRunning => _isRunning;
        public bool IsCompleted => _isCompleted;

        public event EventHandler<TimerTickEventArgs> TimerTick;
        public event EventHandler<TimerCompletedEventArgs> TimerCompleted;

        public void Start()
        {
            if (!_isRunning)
            {
                _isRunning = true;
                _isCompleted = false;
                _timer.Start();
            }
        }

        public void Stop()
        {
            if (_isRunning)
            {
                _isRunning = false;
                _timer.Stop();
            }
        }

        public void StopAndReset()
        {
            Stop();
            _minutes = 0;
            _seconds = 0;
            _isCompleted = false;
            SetCurrentTime();
        }

        public void Update()
        {
            if (_isRunning)
            {
                Stop();
                Start();
            }
        }

        public void AddTime(int minutes)
        {
            if (minutes > 0)
            {
                _minutes += minutes;
                Update();
            }
        }

        public void SubtractTime(int minutes)
        {
            if (minutes > 0)
            {
                if (_minutes >= minutes)
                {
                    _minutes -= minutes;
                    Update();
                }
                else
                {
                    _minutes = 0;
                    _seconds = 0;
                    Update();
                }
            }
        }

        public void SetCurrentTime()
        {
            DateTime currentTime = DateTime.Now;
            _title = currentTime.ToString("hh:mm:ss tt");
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (_seconds == 0)
            {
                if (_minutes > 0)
                {
                    _minutes--;
                    _seconds = 59;
                }
                else
                {
                    // Timer completed
                    _isCompleted = true;
                    _isRunning = false;
                    _timer.Stop();
                    OnTimerCompleted();
                    return;
                }
            }
            else
            {
                _seconds--;
            }

            string displayText = _isCompleted ? "Time Up" : $"{_minutes:D2}:{_seconds:D2}";
            bool isLowTime = _minutes < 5;
            
            OnTimerTick(new TimerTickEventArgs(_minutes, _seconds, displayText, isLowTime));
        }

        protected virtual void OnTimerTick(TimerTickEventArgs e)
        {
            TimerTick?.Invoke(this, e);
        }

        protected virtual void OnTimerCompleted()
        {
            TimerCompleted?.Invoke(this, new TimerCompletedEventArgs());
        }
    }
}
