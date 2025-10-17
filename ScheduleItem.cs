using System;

namespace TimerRccg
{
    public class ScheduleItem
    {
        public string Title { get; set; } = "";
        public int TimeInMinutes { get; set; } = 0;

        public ScheduleItem() { }

        public ScheduleItem(string title, int timeInMinutes)
        {
            Title = title;
            TimeInMinutes = timeInMinutes;
        }

        public override string ToString()
        {
            return $"{Title} - Time :- {TimeInMinutes} mins";
        }
    }
}
