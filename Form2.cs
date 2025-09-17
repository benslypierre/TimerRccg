using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace TimerRccg
{

    public partial  class Form2 : Form
    {
        public static int min = 0;
        public static int sec = 0;
        //Creating an array to get both title and time for each program.
        public static List<string> titleList = new List<string>();
        public static List<int> timeList = new List<int>();
        public static Form2 displayTime;
        public static int i = 0;
        public static string Mtitle;
        public static bool isVisble = true;
        public static Form1 main;
        public static Form3 programList = new Form3();

        public static string[] title
        {
            get { return titleList.ToArray(); }
            set { titleList = new List<string>(value); }
        }
        public static int[] time
        {
            get { return timeList.ToArray(); }
            set { timeList = new List<int>(value); }
        }

        private static Form2 instance; // Singleton instance

        // Public property to access the instance(ChatGpt).
        public static Form2 Instance
        {
            get
            {
                if (instance == null || instance.IsDisposed)
                {
                    instance = new Form2();
                }
                return instance;
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            if (timer1 != null)
                timer1.Dispose();
            instance = null;
        }


        public Form2()
        {
            InitializeComponent();
            // Set dark blue background
            this.BackColor = Color.FromArgb(24, 32, 72); // Deep blue
            this.Font = new Font("Segoe UI", 14F, FontStyle.Regular);
            this.FormBorderStyle = FormBorderStyle.None;
            // Add logo to top center
            PictureBox logo = new PictureBox();
            logo.Image = Image.FromFile("bethel-logo.png");
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
            main = Form1.main;
            programList= Form3.programList;
            min = 0;
            sec= 0;
            displayTime = this;
            this.WindowState = FormWindowState.Maximized;
            TimeLabel.Visible = true;
             Console.WriteLine("I am working timeSchedual2");
           



        }

        // Singleton method to get or recreate the instance
        public static Form2 GetInstance()
        {
            if (displayTime == null || displayTime.IsDisposed)
            {
                displayTime = new Form2();
            }
            return displayTime;
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
            idTimer.Text = min.ToString("D2") + ":" + sec.ToString("D2");
            
 
            
        }
        public  void titleUpdate()
        {
            idTitle.Text = Mtitle;
            Console.WriteLine("I am working titleUpdate");
        }
        


        public void timeSchedual()
        {
            if (i < titleList.Count && i < timeList.Count)
            {
                main.updateMiniText();
                idTitle.Text = titleList[i];
                min = timeList[i];
                sec = 0;
                update();
                // Only increment i if moving to the next item elsewhere
            }
            else
            {
                idTitle.Text = "";
                min = 0;
                sec = 0;
                update();
            }
        }
        public void updateProgram()
        {
            programList.getValues();

        }
        //This is a looping program that has a delay of 1000ms( 1 sec) and increments the time.

        private void timer1_Tick(object sender, EventArgs e)
        {
           
            DateTime currentTime = DateTime.Now;
            TimeLabel.Text = currentTime.ToString("hh:mm:ss tt");
            
            // this displays the time.
            if (Form1.timeUpdate == true)
            {

                Form1.main.getTime();
            }
            else
            {
                idTimer.Visible = true;
            }
            //This code ensure that the count down is done properly.
            if (sec == 0)
            {
                if (min > 0)
                {
                    min--;
                    sec = 60;

                }
                else
                {
                    idTimer.Text = "Time Up";
                    
                }
            }
            else
            {
                sec--;
                idTimer.Text = min.ToString("D2") + ":" + sec.ToString("D2");
            }



            // this changes the color of the timer when it the time is less than 5 mins
            if (sec == 0 && min == 0)
                idTimer.Text = "Time Up";
            if (min < 5 )
            {
                idTimer.ForeColor = Color.Red;
 
            }
            else 
            {
                idTimer.ForeColor = Color.White;
            }

            //This checkes to see if the minutes is less than five and start blinking.
            if(min < 5)
            {
                isVisble = !isVisble;
                idTimer.Visible = isVisble;
                  
            }


            

            Form1.main.updateMiniText();
           

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
            if (idTitle.Text == null)
                idTitle.Text = "Welcome";
            return idTitle.Text;
        }
        
        //Getting the Timer from form2.
        public string getTimer()
        {
            if (idTimer.Text == null)
                idTimer.Text = "Set time";
            return idTimer.Text; 
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
