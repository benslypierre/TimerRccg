using System;
using System.ComponentModel;

namespace TimerRccg
{
    public class ScheduleService : IScheduleService
    {
        private BindingList<ScheduleItem> _scheduleItems;
        private int _currentIndex = 0;

        public ScheduleService()
        {
            _scheduleItems = new BindingList<ScheduleItem>();
        }

        public BindingList<ScheduleItem> ScheduleItems => _scheduleItems;

        public int CurrentIndex
        {
            get => _currentIndex;
            set
            {
                if (value >= 0 && value < _scheduleItems.Count)
                {
                    _currentIndex = value;
                }
                else if (value < 0)
                {
                    _currentIndex = 0;
                }
                else if (value >= _scheduleItems.Count && _scheduleItems.Count > 0)
                {
                    _currentIndex = _scheduleItems.Count - 1;
                }
            }
        }

        public event EventHandler<ScheduleChangedEventArgs> ScheduleChanged;

        public void AddItem(string title, int timeInMinutes)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty", nameof(title));
            
            if (timeInMinutes < 0)
                throw new ArgumentException("Time must be non-negative", nameof(timeInMinutes));

            var item = new ScheduleItem(title, timeInMinutes);
            _scheduleItems.Add(item);
            OnScheduleChanged();
        }

        public void EditItem(int index, string title, int timeInMinutes)
        {
            if (index < 0 || index >= _scheduleItems.Count)
                throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range");

            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty", nameof(title));
            
            if (timeInMinutes < 0)
                throw new ArgumentException("Time must be non-negative", nameof(timeInMinutes));

            _scheduleItems[index].Title = title;
            _scheduleItems[index].TimeInMinutes = timeInMinutes;
            OnScheduleChanged();
        }

        public void DeleteItem(int index)
        {
            if (index < 0 || index >= _scheduleItems.Count)
                throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range");

            _scheduleItems.RemoveAt(index);
            
            // Adjust current index if necessary
            if (_currentIndex >= _scheduleItems.Count && _scheduleItems.Count > 0)
            {
                _currentIndex = _scheduleItems.Count - 1;
            }
            else if (_scheduleItems.Count == 0)
            {
                _currentIndex = 0;
            }
            
            OnScheduleChanged();
        }

        public void MoveItem(int index, int direction)
        {
            if (index < 0 || index >= _scheduleItems.Count)
                throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range");

            int newIndex = index + direction;
            if (newIndex < 0 || newIndex >= _scheduleItems.Count)
                return; // Cannot move outside bounds

            var item = _scheduleItems[index];
            _scheduleItems.RemoveAt(index);
            _scheduleItems.Insert(newIndex, item);

            // Adjust current index if it was affected
            if (_currentIndex == index)
            {
                _currentIndex = newIndex;
            }
            else if (_currentIndex > index && _currentIndex <= newIndex)
            {
                _currentIndex--;
            }
            else if (_currentIndex < index && _currentIndex >= newIndex)
            {
                _currentIndex++;
            }

            OnScheduleChanged();
        }

        public ScheduleItem GetCurrentItem()
        {
            if (_scheduleItems.Count == 0 || _currentIndex < 0 || _currentIndex >= _scheduleItems.Count)
                return null;

            return _scheduleItems[_currentIndex];
        }

        public bool HasItems()
        {
            return _scheduleItems.Count > 0;
        }

        protected virtual void OnScheduleChanged()
        {
            ScheduleChanged?.Invoke(this, new ScheduleChangedEventArgs(_currentIndex));
        }
    }
}
