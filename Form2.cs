using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace TimerRccg
{

    public partial  class Form2 : Form
    {
        private readonly ITimerService _timerService;
        private readonly IScheduleService _scheduleService;
        private readonly IScreenService _screenService;
        private bool _isBlinking = false;
        private Color _originalTimerColor = Color.White;

        private static Form2 instance; // Singleton instance

        // Public property to access the instance(ChatGpt).
        public static Form2 Instance
        {
            get
            {
                if (instance == null || instance.IsDisposed)
                {
                    // Create with default services - these should be injected properly
                    // This is a fallback - ideally services should be injected
                    instance = new Form2(new TimerService(), new ScheduleService(), new ScreenService());
                }
                return instance;
            }
        }

        // Method to set the services for the singleton instance
        public static void SetServices(ITimerService timerService, IScheduleService scheduleService, IScreenService screenService)
        {
            if (instance != null && !instance.IsDisposed)
            {
                // Dispose the old instance
                instance.Dispose();
            }
            // Create new instance with provided services
            instance = new Form2(timerService, scheduleService, screenService);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            if (timer1 != null)
                timer1.Dispose();
            instance = null;
        }


        public Form2(ITimerService timerService, IScheduleService scheduleService, IScreenService screenService)
        {
            _timerService = timerService ?? throw new ArgumentNullException(nameof(timerService));
            _scheduleService = scheduleService ?? throw new ArgumentNullException(nameof(scheduleService));
            _screenService = screenService ?? throw new ArgumentNullException(nameof(screenService));
            
            InitializeComponent();
            
            // Apply theme
            Theme.Apply(this);
            this.FormBorderStyle = FormBorderStyle.None;
            // Add logo to top center
            PictureBox logo = new PictureBox();
            try
            {
                if (System.IO.File.Exists("bethel-logo.png"))
                {
                    logo.Image = Image.FromFile("bethel-logo.png");
                }
                else
                {
                    // Create a placeholder or skip logo if file doesn't exist
                    logo.BackColor = Color.Transparent;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not load logo: {ex.Message}");
                logo.BackColor = Color.Transparent;
            }
            logo.SizeMode = PictureBoxSizeMode.Zoom;
            logo.Size = new Size(120, 120);
            logo.Location = new Point((this.Width - logo.Width) / 2, 20);
            logo.Anchor = AnchorStyles.Top;
            this.Controls.Add(logo);
            logo.BringToFront();
            // Style timer and title
            idTimer.Font = new Font("Segoe UI", 80F, FontStyle.Bold);
            idTimer.ForeColor = Color.White;
            idTimer.BackColor = Color.Transparent;
            idTitle.Font = new Font("Segoe UI", 36F, FontStyle.Bold);
            idTitle.ForeColor = Color.WhiteSmoke;
            idTitle.BackColor = Color.Transparent;
            // Center timer and title
            idTitle.TextAlign = ContentAlignment.MiddleCenter;
            idTimer.TextAlign = ContentAlignment.MiddleCenter;
            // Add shadow effect to timer (simulate by drawing twice in Paint event)
            idTimer.Paint += (s, e) => {
                e.Graphics.DrawString(idTimer.Text, idTimer.Font, new SolidBrush(Color.FromArgb(80, 0, 0, 0)), 4, 4);
                e.Graphics.DrawString(idTimer.Text, idTimer.Font, Brushes.White, 0, 0);
            };
            // Set TimeLabel style
            TimeLabel.Font = new Font("Segoe UI", 18F, FontStyle.Regular);
            TimeLabel.ForeColor = Color.WhiteSmoke;
            TimeLabel.BackColor = Color.Transparent;
            this.WindowState = FormWindowState.Normal; // Changed from Maximized to Normal
            TimeLabel.Visible = true;
            
            // Subscribe to timer events
            _timerService.TimerTick += OnTimerTick;
            _timerService.TimerCompleted += OnTimerCompleted;
           



        }

        // Singleton method to get or recreate the instance
        public static Form2 GetInstance()
        {
            return Instance;
        }

        public void ShowOnScreen(int screenIndex, bool maximize = true)
        {
            var screens = Screen.AllScreens;
            if (screenIndex >= 0 && screenIndex < screens.Length)
            {
                this.StartPosition = FormStartPosition.Manual;
                this.WindowState = FormWindowState.Normal;
                this.Bounds = screens[screenIndex].WorkingArea;
                
                if (maximize)
                {
                    this.WindowState = FormWindowState.Maximized;
                }
                
                this.Show();
                this.BringToFront();
            }
        }

        public void ForceRepositionToScreen(int screenIndex)
        {
            var screens = Screen.AllScreens;
            if (screenIndex >= 0 && screenIndex < screens.Length)
            {
                this.Hide();
                this.StartPosition = FormStartPosition.Manual;
                this.WindowState = FormWindowState.Normal;
                this.Bounds = screens[screenIndex].WorkingArea;
                this.WindowState = FormWindowState.Maximized;
                this.Show();
                this.BringToFront();
                this.Activate();
            }
        }


        // Sample method to show the form
        public static void ShowForm()
        {
            Form2 form = GetInstance();
            form.Show();
        }



        private void idTimer_Click(object sender, EventArgs e) 
        {
            

        }

        //This method update form 2 by stoping and starting the timer.
        public void update()
        {
            timer1.Stop();
            timer1.Start();
            idTimer.Text = _timerService.IsCompleted ? "Time Up" : $"{_timerService.Minutes:D2}:{_timerService.Seconds:D2}";
        }
        
        public void titleUpdate()
        {
            idTitle.Text = _timerService.Title;
            Console.WriteLine("I am working titleUpdate");
        }

        public void timeSchedual()
        {
            var currentItem = _scheduleService.GetCurrentItem();
            if (currentItem != null)
            {
                _timerService.Minutes = currentItem.TimeInMinutes;
                _timerService.Seconds = 0;
                _timerService.Title = currentItem.Title;
                _timerService.Start();
                idTitle.Text = currentItem.Title;
                update();
            }
            else
            {
                idTitle.Text = "";
                _timerService.StopAndReset();
                update();
            }
        }
        
        public void updateProgram()
        {
            // This method can be removed or updated as needed
        }
        //This is a looping program that has a delay of 1000ms( 1 sec) and increments the time.

        private void timer1_Tick(object sender, EventArgs e)
        {
            DateTime currentTime = DateTime.Now;
            TimeLabel.Text = currentTime.ToString("hh:mm:ss tt");
            
            // Update timer display from service
            idTimer.Text = _timerService.IsCompleted ? "Time Up" : $"{_timerService.Minutes:D2}:{_timerService.Seconds:D2}";
            
            // Change color when time is low
            if (_timerService.Minutes < 5)
            {
                idTimer.ForeColor = Color.Red;
                
                // Visual emphasis instead of blinking
                if (_timerService.Minutes < 1) // Blink only in last minute
                {
                    _isBlinking = !_isBlinking;
                    idTimer.ForeColor = _isBlinking ? Color.Yellow : Color.Red;
                }
            }
            else
            {
                idTimer.ForeColor = _originalTimerColor;
                _isBlinking = false;
            }
            
            // Always keep timer visible
            idTimer.Visible = true;
        }

        private void OnTimerTick(object sender, TimerTickEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => OnTimerTick(sender, e)));
                return;
            }
            
            idTimer.Text = e.DisplayText;
            idTimer.ForeColor = e.IsLowTime ? Color.Red : _originalTimerColor;
            
            // Visual emphasis for low time instead of blinking
            if (e.IsLowTime && e.Minutes < 1)
            {
                _isBlinking = !_isBlinking;
                idTimer.ForeColor = _isBlinking ? Color.Yellow : Color.Red;
            }
        }

        private void OnTimerCompleted(object sender, TimerCompletedEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => OnTimerCompleted(sender, e)));
                return;
            }
            
            idTimer.Text = "Time Up";
            idTimer.ForeColor = Color.Red;
            idTimer.Visible = true;
            _isBlinking = false;
        }

        //This method activates when the Form is loaded.
        private void Form2_Load(object sender, EventArgs e)
        {
            int x, y;
            x = (this.Width - idTimer.Width) / 2;
            y = (this.Height - idTimer.Height) / 2;

            idTimer.Location = new Point(x, y);

            
        }


        // this method rearrange the Title text location.
        private void idTitle_Click(object sender, EventArgs e)
        {
            int x, y;
            x = (this.Width - idTimer.Width) / 2;
            y = (this.Height - idTimer.Height) / 2;

            idTitle.Location = new Point(x, y / 2);
            idTimer.Location = new Point(x, y);
            
        }

        private void Form2_Resize(object sender, EventArgs e)
        {
            int x, y;
            x = (this.Width - idTimer.Width) / 2;
            y = (this.Height - idTimer.Height) / 2;

            idTitle.Location= new Point(x/2 - (int)(x/3), (int)(y/5));
            idTimer.Location = new Point(x -(x/3), y);
            float newFontSizeTitle = Math.Max(1, y / 3); // Ensure font size is at least 1
            float newFontSizeTimer = Math.Max(1, y / 2); // Ensure font size is at least 1
            idTitle.Font = new Font(idTitle.Font.FontFamily, newFontSizeTitle);
            idTimer.Font = new Font(idTitle.Font.FontFamily, newFontSizeTimer);

            // Position TimeLabel at the bottom-right corner
            int labelX = this.ClientSize.Width - TimeLabel.Width - 20; // 20px margin from right
            int labelY = this.ClientSize.Height - TimeLabel.Height - 20; // 20px margin from bottom

            TimeLabel.Location = new Point(labelX, labelY);

           
        }

        //Getting the Timer title from form2.
        public string getTitle()
        {
            return _timerService.Title ?? "Welcome";
        }
        
        //Getting the Timer from form2.
        public string getTimer()
        {
            return _timerService.IsCompleted ? "Time Up" : $"{_timerService.Minutes:D2}:{_timerService.Seconds:D2}";
        }

        // All this code is to get all the attribute for the font and size of the text.

        public Color getForeColorTitle()
        {
            return idTitle.ForeColor;
        }
        public Color getForeColorTimer() 
        { 
            return idTimer.ForeColor;
        }
        public int getTitleSizeHeight()
        {
            return idTitle.Height;
        }
        public int getTitleSizeWidth()
        {
            return idTitle.Width;
        }
        public int getTimerSizeHeight() 
        {
            return idTimer.Height;
        }
        public int getTimerWidth()
        { 
            return idTimer.Width; 
        }
        public Font timerFontSyle()
        {
            return idTimer.Font;
        }
        public Font titleFontSyle()
        {
            return idTitle.Font;
        }
        public Color getTimerColor() 
        {
            return idTimer.ForeColor;
        }
        public void StopAndReset()
        {
            timer1.Stop();
            _timerService.StopAndReset();
            idTimer.Text = "Time Up";
            idTimer.ForeColor = _originalTimerColor;
            idTimer.Visible = true;
            _isBlinking = false;
        }

        public void Mtimer(string text)
        {
            idTimer.Text = text;
        }

        private void TimeLabel_Click(object sender, EventArgs e)
        {
            // Position TimeLabel at the bottom-right corner
            int labelX = this.ClientSize.Width - TimeLabel.Width - 20; // 20px margin from right
            int labelY = this.ClientSize.Height - TimeLabel.Height - 20; // 20px margin from bottom

            TimeLabel.Location = new Point(labelX, labelY);
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            e.Cancel = true;  // Cancel the default close behavior
            this.Hide();       // Hide instead of closing
        }
    }
}
