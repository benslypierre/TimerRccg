using System;
using System.Windows.Forms;
using System.Drawing;

namespace TimerRccg
{
    public partial class Form4 : Form
    {
        private readonly IScreenService _screenService;
        private Form formOnSelectedScreen;
        public Screen[] screens; 

        public Form4(IScreenService screenService)
        {
            _screenService = screenService ?? throw new ArgumentNullException(nameof(screenService));
            InitializeComponent();
            label1.Text = "All Monitors contected\n";
            screens = Screen.AllScreens;
            
            // Apply theme
            Theme.Apply(this);
            Theme.ApplyToAllControls(this);
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            //Listing all the screens avaliable on the computer system.
            Screen[] screen = Screen.AllScreens;
            int i = 0;

            foreach (var displays in WindowsDisplayAPI.Display.GetDisplays())
            {
                label1.Text += displays.DeviceName +" --> (" + screen[i].DeviceName+") \n";
                comboBox1.Items.Add(displays.DeviceName + " --> (" + screen[i].DeviceName);
                i++;
            }
        }

        //This part of the code present the time on which screen is selected
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                screens = getScreens();
                int screenIndex = comboBox1.SelectedIndex;
                
                if (screenIndex == -1)
                {
                    screenIndex = screens.Length > 1 ? 1 : 0;
                }
                
                _screenService.SetSelectedScreenIndex(screenIndex);
                
                // Debug output
                System.Diagnostics.Debug.WriteLine($"Selected screen index: {screenIndex}");
                System.Diagnostics.Debug.WriteLine($"Available screens: {screens.Length}");
                if (screenIndex < screens.Length)
                {
                    System.Diagnostics.Debug.WriteLine($"Target screen: {screens[screenIndex].DeviceName}");
                    System.Diagnostics.Debug.WriteLine($"Target screen bounds: {screens[screenIndex].WorkingArea}");
                }
                
                // Get the current Form2 instance and force repositioning
                formOnSelectedScreen = Form2.Instance;
                
                // Use both the screen service and direct Form2 method for redundancy
                _screenService.ShowOnSelectedScreen(formOnSelectedScreen, true);
                
                // Also call Form2's direct method as backup
                if (formOnSelectedScreen is Form2 form2)
                {
                    form2.ForceRepositionToScreen(screenIndex);
                }
                
                this.Close();
            }
            catch (ObjectDisposedException a)
            {
                Console.Write(a.Message);
            }
        }
        //Getting combo box
        public ComboBox getcomboBox()
        {
            return comboBox1;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Store the selected screen index
            if (comboBox1.SelectedIndex >= 0)
            {
                _screenService.SetSelectedScreenIndex(comboBox1.SelectedIndex);
            }
        }
        public Screen[] getScreens()
        {
            return Screen.AllScreens;
        }
    }
}
