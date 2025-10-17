using System;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;

namespace TimerRccg
{
    public partial class Form3 : Form
    {
        private readonly IScheduleService _scheduleService;
        private readonly ITimerService _timerService;
        private readonly IScreenService _screenService;
        private string _currentTitle = "";
        private int _currentTime = 0;

        public Form3(IScheduleService scheduleService, ITimerService timerService, IScreenService screenService)
        {
            _scheduleService = scheduleService ?? throw new ArgumentNullException(nameof(scheduleService));
            _timerService = timerService ?? throw new ArgumentNullException(nameof(timerService));
            _screenService = screenService ?? throw new ArgumentNullException(nameof(screenService));
            
            InitializeComponent();
            
            // Apply theme
            Theme.Apply(this);
            Theme.ApplyToAllControls(this);
        }

        //Getting the title and time form user and appending to array
        private void idAdd_Click(object sender, EventArgs e)
        {
            string title = idTitle.Text;
            if (!int.TryParse(idTimeRange.Text, out int minValue) || minValue < 0)
            {
                idTitle.Text = "";
                idTimeRange.Text = "";
                MessageBox.Show("Please enter a valid number of minutes (0 or greater)", "Invalid Input", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                idTimeRange.Focus();
                return;
            }
            
            if (string.IsNullOrWhiteSpace(title))
            {
                MessageBox.Show("Please enter a title", "Missing Title", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                idTitle.Focus();
                return;
            }
            
            try
            {
                _scheduleService.AddItem(title, minValue);
                idTitle.Text = "";
                idTimeRange.Text = "";
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        //This button is used to signify that we are done with the list.
        private void idDone_Click(object sender, EventArgs e)
        {
            Hide();
        }

        public void ShowItemForEdit(string title, int time)
        {
            _currentTitle = title;
            _currentTime = time;
            idTitle.Text = title;
            idTimeRange.Text = time.ToString();
        }

        public void showIndexArray()
        {
            idTitle.Text = _currentTitle;
            idTimeRange.Text = _currentTime.ToString();
        }

        public void getValues()
        {
        }

        private void idTimeRange_KeyPress(object sender, KeyPressEventArgs e)
        {
            //This restrict the user from putting letters into the computer.
            if (!char.IsNumber(e.KeyChar) && (e.KeyChar != (char)Keys.Back))
                e.Handled = true;
        }

        //This functions is to update the list if there is any changes the user make.
        private void idUpdate_Click(object sender, EventArgs e)
        {
            string title = idTitle.Text;
            if (!int.TryParse(idTimeRange.Text, out int minValue) || minValue < 0)
            {
                idTitle.Text = "";
                idTimeRange.Text = "";
                MessageBox.Show("Please enter a valid number of minutes (0 or greater)", "Invalid Input", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                idTimeRange.Focus();
                return;
            }
            
            if (string.IsNullOrWhiteSpace(title))
            {
                MessageBox.Show("Please enter a title", "Missing Title", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                idTitle.Focus();
                return;
            }
            
            try
            {
                // Find the item to update by matching title and time
                for (int i = 0; i < _scheduleService.ScheduleItems.Count; i++)
                {
                    var item = _scheduleService.ScheduleItems[i];
                    if (item.Title == _currentTitle && item.TimeInMinutes == _currentTime)
                    {
                        _scheduleService.EditItem(i, title, minValue);
                        break;
                    }
                }
                
                _currentTitle = title;
                _currentTime = minValue;
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
