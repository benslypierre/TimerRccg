using System;
using System.ComponentModel;

namespace TimerRccg
{
    public interface IScheduleService
    {
        BindingList<ScheduleItem> ScheduleItems { get; }
        int CurrentIndex { get; set; }
        event EventHandler<ScheduleChangedEventArgs> ScheduleChanged;

        void AddItem(string title, int timeInMinutes);
        void EditItem(int index, string title, int timeInMinutes);
        void DeleteItem(int index);
        void MoveItem(int index, int direction);
        ScheduleItem GetCurrentItem();
        bool HasItems();
    }

    public class ScheduleChangedEventArgs : EventArgs
    {
        public int ActiveIndex { get; }
        public ScheduleChangedEventArgs(int activeIndex)
        {
            ActiveIndex = activeIndex;
        }
    }
}
