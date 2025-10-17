using System;
using System.Windows.Forms;

namespace TimerRccg
{
    public class ScreenService : IScreenService
    {
        private int _selectedScreenIndex = 1; // Default to second screen

        public int GetSelectedScreenIndex()
        {
            var screens = Screen.AllScreens;
            if (_selectedScreenIndex >= screens.Length)
            {
                _selectedScreenIndex = screens.Length > 1 ? 1 : 0;
            }
            return _selectedScreenIndex;
        }

        public void SetSelectedScreenIndex(int index)
        {
            var screens = Screen.AllScreens;
            if (index >= 0 && index < screens.Length)
            {
                _selectedScreenIndex = index;
            }
        }

        public void ShowOnSelectedScreen(Form form, bool ensureMaximized = true)
        {
            if (form == null || form.IsDisposed) return;

            var screens = GetAvailableScreens();
            var selectedScreen = GetSelectedScreen();
            
            if (selectedScreen != null)
            {
                form.StartPosition = FormStartPosition.Manual;
                form.Location = selectedScreen.WorkingArea.Location;
                
                if (ensureMaximized)
                {
                    form.WindowState = FormWindowState.Maximized;
                }
                
                form.Show();
                form.BringToFront();
            }
        }

        public Screen[] GetAvailableScreens()
        {
            return Screen.AllScreens;
        }

        public Screen GetSelectedScreen()
        {
            var screens = GetAvailableScreens();
            int index = GetSelectedScreenIndex();
            
            if (index >= 0 && index < screens.Length)
            {
                return screens[index];
            }
            
            // Fallback to first available screen
            return screens.Length > 0 ? screens[0] : null;
        }
    }
}
