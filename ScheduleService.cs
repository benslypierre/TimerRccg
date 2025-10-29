using System;
using System.ComponentModel;
using System.IO;
using Newtonsoft.Json;

namespace TimerRccg
{
    public class ScheduleService : IScheduleService
    {
        private BindingList<ScheduleItem> _scheduleItems;
        private int _currentIndex = 0;
        private static readonly string SCHEDULE_FILE_PATH = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TimerRccg", "schedule.json");

        public ScheduleService()
        {
            _scheduleItems = new BindingList<ScheduleItem>();
            LoadSchedule();
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

        public void SaveSchedule()
        {
            try
            {
                var directory = Path.GetDirectoryName(SCHEDULE_FILE_PATH);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var list = new System.Collections.Generic.List<ScheduleItem>(_scheduleItems);
                var json = JsonConvert.SerializeObject(list, Formatting.Indented);
                File.WriteAllText(SCHEDULE_FILE_PATH, json);
            }
            catch (IOException)
            {
                // Intentionally ignore I/O errors in service layer
            }
            catch (UnauthorizedAccessException)
            {
                // Intentionally ignore permission errors
            }
            catch (JsonException)
            {
                // Intentionally ignore serialization errors
            }
        }

        public void LoadSchedule()
        {
            try
            {
                if (!File.Exists(SCHEDULE_FILE_PATH))
                {
                    return;
                }

                var json = File.ReadAllText(SCHEDULE_FILE_PATH);
                var list = JsonConvert.DeserializeObject<System.Collections.Generic.List<ScheduleItem>>(json) ?? new System.Collections.Generic.List<ScheduleItem>();

                _scheduleItems.Clear();
                foreach (var item in list)
                {
                    if (item != null)
                        _scheduleItems.Add(item);
                }

                OnScheduleChanged();
            }
            catch (IOException)
            {
                // Ignore corrupt or unreadable files
            }
            catch (JsonException)
            {
                // Ignore deserialization errors and start fresh
            }
            catch (Exception)
            {
                // Swallow any unexpected exceptions to avoid crashing on startup
            }
        }
    }
}
