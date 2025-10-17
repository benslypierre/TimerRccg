using System;

namespace TimerRccg
{
    public interface ITimerService
    {
        int Minutes { get; set; }
        int Seconds { get; set; }
        string Title { get; set; }
        bool IsRunning { get; }
        bool IsCompleted { get; }
        
        event EventHandler<TimerTickEventArgs> TimerTick;
        event EventHandler<TimerCompletedEventArgs> TimerCompleted;

        void Start();
        void Stop();
        void StopAndReset();
        void Update();
        void AddTime(int minutes);
        void SubtractTime(int minutes);
        void SetCurrentTime();
    }

    public class TimerTickEventArgs : EventArgs
    {
        public int Minutes { get; }
        public int Seconds { get; }
        public string DisplayText { get; }
        public bool IsLowTime { get; }

        public TimerTickEventArgs(int minutes, int seconds, string displayText, bool isLowTime)
        {
            Minutes = minutes;
            Seconds = seconds;
            DisplayText = displayText;
            IsLowTime = isLowTime;
        }
    }

    public class TimerCompletedEventArgs : EventArgs
    {
        public TimerCompletedEventArgs()
        {
        }
    }
}
