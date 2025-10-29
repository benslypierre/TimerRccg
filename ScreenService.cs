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
                // Force the form to normal state first
                form.WindowState = FormWindowState.Normal;
                form.StartPosition = FormStartPosition.Manual;
                
                // Hide the form first to ensure proper repositioning
                form.Hide();
                
                // Set the form to the selected screen's working area
                form.Bounds = selectedScreen.WorkingArea;
                
                // Show the form first
                form.Show();
                
                if (ensureMaximized)
                {
                    // Set maximized after showing
                    form.WindowState = FormWindowState.Maximized;
                }
                
                // Bring to front and activate
                form.BringToFront();
                form.Activate();
                form.TopMost = true;
                form.TopMost = false; // This ensures it stays on top but allows interaction
                
                // Force refresh to ensure proper display
                form.Refresh();
                
                // Debug output to verify screen positioning
                System.Diagnostics.Debug.WriteLine($"Form positioned on screen: {selectedScreen.DeviceName}");
                System.Diagnostics.Debug.WriteLine($"Form bounds: {form.Bounds}");
                System.Diagnostics.Debug.WriteLine($"Screen bounds: {selectedScreen.WorkingArea}");
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
