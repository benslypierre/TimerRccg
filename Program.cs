using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TimerRccg
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            // Create services
            var scheduleService = new ScheduleService();
            var timerService = new TimerService();
            var screenService = new ScreenService();
            
            // Create and wire forms with services
            var form1 = new Form1(scheduleService, timerService, screenService);
            
            Application.Run(form1);
        }
    }
}
