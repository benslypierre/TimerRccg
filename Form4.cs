using System;
using System.Windows.Forms;
using System.Drawing;

namespace TimerRccg
{
    public partial class Form4 : Form
    {
        public static Form4 screenMenu;
        public static Form4 mainForm = null;
        public static int? SelectedScreenIndex = null; // null means no selection
        Form formOnSelectedScreen = Form2.GetInstance();
        private bool LocationCheck = false;
        public Screen[] screens;
        private Screen selectedScreen; 

        public Form4()
        {
            InitializeComponent();
            label1.Text = "All Monitors contected\n";
            screenMenu = this;
            screens = Screen.AllScreens;
            // Theme
            this.BackColor = Color.FromArgb(24, 32, 72);
            this.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            foreach (Control c in this.Controls)
            {
                if (c is Button btn)
                {
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.BackColor = Color.FromArgb(24, 32, 72);
                    btn.ForeColor = Color.White;
                    btn.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                    btn.FlatAppearance.BorderSize = 1;
                    btn.FlatAppearance.BorderColor = Color.White;
                    btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(44, 54, 112);
                }
                if (c is Label lbl)
                {
                    lbl.ForeColor = Color.White;
                }
            }
            // Remove logo from Form4 (logo only on Form2)
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
            mainForm = screenMenu;
            try
            {
                screens = getScreens();
                if (comboBox1.SelectedIndex == -1)
                {
                    if (screens.Length > 1)
                        selectedScreen = screens[1];
                    else
                        selectedScreen = screens[0];
                }
                else
                {
                    selectedScreen = screens[comboBox1.SelectedIndex];
                }
                formOnSelectedScreen = Form2.GetInstance();
                formOnSelectedScreen.WindowState = FormWindowState.Normal; // Ensure not maximized
                formOnSelectedScreen.StartPosition = FormStartPosition.Manual;
                formOnSelectedScreen.Location = selectedScreen.WorkingArea.Location;
                if (formOnSelectedScreen.IsDisposed)
                {
                    formOnSelectedScreen = new Form2();
                    formOnSelectedScreen.StartPosition = FormStartPosition.Manual;
                    formOnSelectedScreen.Location = selectedScreen.WorkingArea.Location;
                }
                if (!formOnSelectedScreen.Visible)
                {
                    formOnSelectedScreen.Show();
                    formOnSelectedScreen.WindowState = FormWindowState.Maximized;
                }
                else
                {
                    // Hide and show to force move if already visible
                    formOnSelectedScreen.Hide();
                    formOnSelectedScreen.Show();
                    formOnSelectedScreen.BringToFront();
                    formOnSelectedScreen.Refresh();
                    formOnSelectedScreen.WindowState = FormWindowState.Maximized;
                }
                this.Close();
            }
            catch (ObjectDisposedException a)
            {
                if (formOnSelectedScreen == null || formOnSelectedScreen.IsDisposed)
                {
                    formOnSelectedScreen = new Form2();
                    formOnSelectedScreen.StartPosition = FormStartPosition.Manual;
                    formOnSelectedScreen.Location = selectedScreen.WorkingArea.Location;
                    formOnSelectedScreen.Show();
                }
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
            SelectedScreenIndex = comboBox1.SelectedIndex;
        }
        public Screen[] getScreens()
        {
            return Screen.AllScreens;
        }
    }
}
