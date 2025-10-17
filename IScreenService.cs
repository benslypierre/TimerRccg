using System;
using System.Windows.Forms;

namespace TimerRccg
{
    public interface IScreenService
    {
        int GetSelectedScreenIndex();
        void SetSelectedScreenIndex(int index);
        void ShowOnSelectedScreen(Form form, bool ensureMaximized = true);
        Screen[] GetAvailableScreens();
        Screen GetSelectedScreen();
    }
}
