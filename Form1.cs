using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq; // Added for .OfType()
using System.ComponentModel;

namespace TimerRccg
{
    public partial class Form1 : Form
    {
        private readonly IScheduleService _scheduleService;
        private readonly ITimerService _timerService;
        private readonly IScreenService _screenService;
		private WebControlServer _webServer;
        private Timer _overtimeTimer;
        private int _overtimeSeconds;
        private bool _isScheduleRunning = false;
        private int _cachedRemainingScheduleSeconds = 0;
        
        private Form3 scheduler;
        private Form2 displayTime;
        private ContextMenuStrip listBoxContextMenu;
        private ToolStripMenuItem deleteMenuItem;
        private ToolStripMenuItem editMenuItem;
        private ToolStripMenuItem moveUpMenuItem;
        private ToolStripMenuItem moveDownMenuItem;

        // inisailizing all needed Objects.
        public Form1(IScheduleService scheduleService, ITimerService timerService, IScreenService screenService)
        {
            _scheduleService = scheduleService ?? throw new ArgumentNullException(nameof(scheduleService));
            _timerService = timerService ?? throw new ArgumentNullException(nameof(timerService));
            _screenService = screenService ?? throw new ArgumentNullException(nameof(screenService));
            
            InitializeComponent();
            
            // Set the services for Form2 singleton and get the instance
            Form2.SetServices(_timerService, _scheduleService, _screenService);
            displayTime = Form2.Instance;
            scheduler = new Form3(_scheduleService, _timerService, _screenService);
            
            // Navigation buttons should be visible when there are schedule items
            idPrevious.Hide();
            idNext.Hide();
            
            // Extra time controls should be hidden initially
            idExtraTime.Hide();
            idExtraMins.Hide();
            idAdd.Hide();
            idSub.Hide();
            
            // Bind ListBox to schedule service
            idListBox.DataSource = _scheduleService.ScheduleItems;
            idListBox.DisplayMember = "ToString";
            
            // Subscribe to schedule changes
            _scheduleService.ScheduleChanged += OnScheduleChanged;
            
            // Subscribe to timer events
            _timerService.TimerTick += OnTimerTick;
            _timerService.TimerCompleted += OnTimerCompleted;

            // Context menu for deleting and editing events
            listBoxContextMenu = new ContextMenuStrip();
            deleteMenuItem = new ToolStripMenuItem("Delete");
            deleteMenuItem.Click += DeleteMenuItem_Click;
            listBoxContextMenu.Items.Add(deleteMenuItem);
            editMenuItem = new ToolStripMenuItem("Edit");
            editMenuItem.Click += EditMenuItem_Click;
            listBoxContextMenu.Items.Add(editMenuItem);
            moveUpMenuItem = new ToolStripMenuItem("Move Up");
            moveUpMenuItem.Click += MoveUpMenuItem_Click;
            listBoxContextMenu.Items.Add(moveUpMenuItem);
            moveDownMenuItem = new ToolStripMenuItem("Move Down");
            moveDownMenuItem.Click += MoveDownMenuItem_Click;
            listBoxContextMenu.Items.Add(moveDownMenuItem);
            idListBox.ContextMenuStrip = listBoxContextMenu;
            idListBox.MouseDown += IdListBox_MouseDown;


            // Get the singleton instance
            displayTime = Form2.Instance;

            // Initialize display with timer service
            Title1.Text = _timerService.Title;
            Timer2.Text = _timerService.IsCompleted ? "Time Up" : $"{_timerService.Minutes:D2}:{_timerService.Seconds:D2}";

            // Apply theme to main display elements
            Title1.Font = Theme.TitleFont;
            Timer2.Font = Theme.TimerFont;
            Title1.ForeColor = Theme.TextColor;
            Timer2.ForeColor = Theme.TextColor;
            panel1.BackColor = Theme.PrimaryBackground;

            // Always launch on the 2nd available screen if more than one screen
            Screen[] screens = Screen.AllScreens;
            if (screens.Length > 1)
            {
                this.StartPosition = FormStartPosition.Manual;
                this.Location = screens[1].WorkingArea.Location;
            }
            // Apply theme to form and controls
            Theme.Apply(this);
            Theme.ApplyToAllControls(this);
            Theme.Apply(menuStrip1);
            Theme.Apply(idListBox);
            
            // Ensure overtime label maintains yellow color after theme application
            overtimeLabel.ForeColor = Color.Yellow;
            
            // Set up tooltips
            ToolTip toolTip = new ToolTip();
            toolTip.SetToolTip(idSetTime, "Set the timer for the current event");
            toolTip.SetToolTip(idStop, "Stop the timer");
            toolTip.SetToolTip(idSchedule, "Open the schedule editor");
            toolTip.SetToolTip(idStart, "Start the selected program");
            toolTip.SetToolTip(idNext, "Go to the next program");
            toolTip.SetToolTip(idPrevious, "Go to the previous program");
            toolTip.SetToolTip(idListBox, "List of scheduled programs");
            toolTip.SetToolTip(idgetMin, "Enter minutes for the timer");
            toolTip.SetToolTip(idTitle, "Enter the title for the timer");
            toolTip.SetToolTip(idExtraTime, "Enter extra minutes to add or subtract");
            toolTip.SetToolTip(idAdd, "Add extra minutes");
            toolTip.SetToolTip(idSub, "Subtract extra minutes");

            // Ensure layout and anchors are correct for responsive UI
            ConfigureRightSideLayout();

            // Set logical Tab order
            SetTabOrder(this);

            // Center Title1 and Timer2 in the main panel
            CenterMainDisplay();
			this.FormClosing += Form1_FormClosing;

			// Start web control server
			try
			{
				_webServer = new WebControlServer(_timerService, _scheduleService, this);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Failed to initialize web server: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

            // Initialize overtime timer
            _overtimeTimer = new Timer();
            _overtimeTimer.Interval = 1000; // 1 second
            _overtimeTimer.Tick += OvertimeTimer_Tick;
        }

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			// Ensure handle exists before starting web server
			var handle = this.Handle;
			try
			{
				_webServer?.Start();
			}
			catch (Exception ex)
			{
				MessageBox.Show("Failed to start web server: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

        private void ConfigureRightSideLayout()
        {
            // Anchor main display panel to resize with the window
            if (panel1 != null)
                panel1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

            // Rebuild Set Time group (NOTE: groupBox2 is Set Time in designer)
            if (groupBox2 != null && LabelT != null && idTitle != null && label1 != null && idgetMin != null && idSetTime != null && idStop != null)
            {
                groupBox2.SuspendLayout();
                // Clear any previous layout artifacts while keeping controls
                var setTimeControls = new Control[] { LabelT, idTitle, label1, idgetMin, idSetTime, idStop };
                foreach (var c in setTimeControls) groupBox2.Controls.Remove(c);

                var setTable = new TableLayoutPanel();
                setTable.ColumnCount = 2;
                setTable.RowCount = 0;
                setTable.Dock = DockStyle.Fill;
                setTable.Padding = new Padding(8, 8, 8, 8);
                setTable.AutoSize = true;
                setTable.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                setTable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90));
                setTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

                void AddRow(Label label, Control input)
                {
                    setTable.RowCount++;
                    setTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
                    label.TextAlign = ContentAlignment.MiddleRight;
                    label.Dock = DockStyle.Fill;
                    input.Dock = DockStyle.Fill;
                    setTable.Controls.Add(label, 0, setTable.RowCount - 1);
                    setTable.Controls.Add(input, 1, setTable.RowCount - 1);
                }

                AddRow(LabelT, idTitle);
                AddRow(label1, idgetMin);

                var buttonsPanel = new FlowLayoutPanel();
                buttonsPanel.AutoSize = true;
                buttonsPanel.FlowDirection = FlowDirection.LeftToRight;
                buttonsPanel.WrapContents = false;
                idSetTime.Width = 72;
                idStop.Width = 72;
                buttonsPanel.Controls.Add(idSetTime);
                buttonsPanel.Controls.Add(idStop);
                var centeredButtons = CreateCenteredRow(buttonsPanel);
                setTable.RowCount++;
                setTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
                setTable.Controls.Add(centeredButtons, 0, setTable.RowCount - 1);
                setTable.SetColumnSpan(centeredButtons, 2);

                groupBox2.Controls.Add(setTable);
                groupBox2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                groupBox2.ResumeLayout();
            }

            // Rebuild Change Time group neatly (NOTE: groupBox1 is Change Time in designer)
            if (groupBox1 != null && idExtraMins != null && idExtraTime != null && idAdd != null && idSub != null)
            {
                groupBox1.SuspendLayout();
                var changeControls = new Control[] { idExtraMins, idExtraTime, idAdd, idSub };
                foreach (var c in changeControls) groupBox1.Controls.Remove(c);

                var changeTable = new TableLayoutPanel();
                changeTable.ColumnCount = 2;
                changeTable.RowCount = 0;
                changeTable.Dock = DockStyle.Fill;
                changeTable.Padding = new Padding(8, 8, 8, 8);
                changeTable.AutoSize = true;
                changeTable.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                changeTable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90));
                changeTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

                // Row for label + input
                changeTable.RowCount++;
                changeTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
                // Label alignment (idExtraMins is a Label)
                if (idExtraMins is Label changeLbl)
                {
                    changeLbl.TextAlign = ContentAlignment.MiddleRight;
                    changeLbl.Dock = DockStyle.Fill;
                }
                idExtraTime.Dock = DockStyle.Fill;
                changeTable.Controls.Add(idExtraMins, 0, changeTable.RowCount - 1);
                changeTable.Controls.Add(idExtraTime, 1, changeTable.RowCount - 1);

                // Row for buttons
                var changeBtns = new FlowLayoutPanel();
                changeBtns.AutoSize = true;
                changeBtns.FlowDirection = FlowDirection.LeftToRight;
                changeBtns.WrapContents = false;
                idAdd.Width = 72;
                idSub.Width = 72;
                changeBtns.Controls.Add(idAdd);
                changeBtns.Controls.Add(idSub);
                var centeredChangeBtns = CreateCenteredRow(changeBtns);
                changeTable.RowCount++;
                changeTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
                changeTable.Controls.Add(centeredChangeBtns, 0, changeTable.RowCount - 1);
                changeTable.SetColumnSpan(centeredChangeBtns, 2);

                groupBox1.Controls.Add(changeTable);
                groupBox1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
                groupBox1.ResumeLayout();
            }

            // Anchor the right side controls so they resize with the window height and stick to the right
            if (idSchedule != null)
                idSchedule.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            if (idListBox != null)
            {
                idListBox.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Left;
                // Ensure listbox aligns width with the set time group above it
                if (groupBox2 != null)
                    idListBox.Width = groupBox2.Width;
            }

			// Anchor Program starter group so it stays at bottom and doesn't overlap
			if (Program != null)
				Program.Anchor = AnchorStyles.Bottom;

			// Place Estimated Time group to the right of Program and keep at bottom
			if (estimatedTimeGroup != null && Program != null)
			{
				estimatedTimeGroup.Anchor = AnchorStyles.Bottom;
				int gap = 50; // Increased gap from 20 to 50 pixels
				int left = Program.Right + gap;
				int top = Program.Top + (Program.Height - estimatedTimeGroup.Height) / 2;
				estimatedTimeGroup.Location = new Point(left, top);
			}

            // Keep right column widths aligned during resize
            this.Resize -= OnFormResizedSyncRight;
            this.Resize += OnFormResizedSyncRight;
        }

        private Control CreateCenteredRow(Control content)
        {
            var container = new TableLayoutPanel();
            container.ColumnCount = 3;
            container.RowCount = 1;
            container.Dock = DockStyle.Fill;
            container.AutoSize = true;
            container.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            container.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            container.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            container.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            content.Anchor = AnchorStyles.None;
            container.Controls.Add(new Panel() { Dock = DockStyle.Fill }, 0, 0);
            container.Controls.Add(content, 1, 0);
            container.Controls.Add(new Panel() { Dock = DockStyle.Fill }, 2, 0);
            return container;
        }

        private void OnFormResizedSyncRight(object sender, EventArgs e)
        {
            if (groupBox2 != null && idListBox != null)
            {
                idListBox.Left = groupBox2.Left;
                idListBox.Width = groupBox2.Width;
            }

			// Keep Estimated Time group positioned to the right of Program
			if (estimatedTimeGroup != null && Program != null)
			{
				int gap = 50; // Increased gap from 20 to 50 pixels
				estimatedTimeGroup.Left = Program.Right + gap;
				estimatedTimeGroup.Top = Program.Top + (Program.Height - estimatedTimeGroup.Height) / 2;
			}

            // Re-center main display elements including overtime label
            CenterMainDisplay();
        }


        private void SetTabOrder(Control parent)
        {
            int tabIndex = 0;
            void SetTabOrderRecursive(Control ctrl)
            {
                ctrl.TabIndex = tabIndex++;
                if (ctrl.HasChildren)
                {
                    foreach (Control child in ctrl.Controls)
                    {
                        SetTabOrderRecursive(child);
                    }
                }
            }
            SetTabOrderRecursive(parent);
        }

        private void CenterMainDisplay()
        {
            if (panel1 != null && Title1 != null && Timer2 != null)
            {
                // Center the display elements in the panel
                int x, y;
                x = (panel1.Width - Timer2.Width) / 2;
                y = (panel1.Height - Timer2.Height) / 2;
                Timer2.Location = new Point(x, y);

                // Position title above timer
                x = (panel1.Width - Title1.Width) / 2;
                y = y - Title1.Height - 20; // 20px gap
                Title1.Location = new Point(x, y);

                // Position overtime label below timer, centered horizontally
                if (overtimeLabel != null)
                {
                    x = (panel1.Width - overtimeLabel.Width) / 2;
                    y = Timer2.Bottom + 20; // 20px gap below timer
                    overtimeLabel.Location = new Point(x, y);
                }

                // Set proper font sizes
                Timer2.Font = Theme.LargeTimerFont;
                Title1.Font = Theme.TitleFont;
            }
        }

        private void OnScheduleChanged(object sender, ScheduleChangedEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => OnScheduleChanged(sender, e)));
                return;
            }
            
            // Handle schedule mutations while running to keep CurrentIndex consistent
            if (_scheduleService.CurrentIndex >= _scheduleService.ScheduleItems.Count)
            {
                _scheduleService.CurrentIndex = Math.Max(0, _scheduleService.ScheduleItems.Count - 1);
            }
            
            // Recompute cached remaining schedule seconds when schedule changes
            RecomputeCachedRemainingScheduleSeconds();
            
            // Show start button if there are items
            if (_scheduleService.HasItems())
            {
                idStart.Show();
            }
            
            // Update ListBox selection if needed
            if (e.ActiveIndex >= 0 && e.ActiveIndex < idListBox.Items.Count)
            {
                idListBox.SelectedIndex = e.ActiveIndex;
            }
            UpdateEstimatedTime();
        }

        private void OnTimerTick(object sender, TimerTickEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => OnTimerTick(sender, e)));
                return;
            }
            
            Timer2.Text = e.DisplayText;
            Timer2.ForeColor = e.IsLowTime ? Color.Red : Theme.TextColor;
            UpdateEstimatedTime();
        }

        private void OnTimerCompleted(object sender, TimerCompletedEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => OnTimerCompleted(sender, e)));
                return;
            }
            
            Timer2.Text = "Time Up";
            Timer2.ForeColor = Color.Red;
            
            // Start overtime tracking
            _overtimeSeconds = 0;
            _overtimeTimer.Start();
            overtimeLabel.Visible = true;
            overtimeLabel.Text = "Overtime: 00:00";
            
            UpdateEstimatedTime();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        //This button objects updates the form two by setting a simple timer
        private void idSetTime_Click(object sender, EventArgs e)
        {
            idExtraTime.Show();
            idExtraMins.Show();
            idAdd.Show();
            idSub.Show();

            // Hide overtime display when manually setting timer
            StopAndHideOvertime();

            // Validate input
            if (!int.TryParse(idgetMin.Text, out int minutes) || minutes < 0)
            {
                idgetMin.Text = "";
                idTitle.Text = "";
                MessageBox.Show("Please enter a valid number of minutes (0 or greater)", "Invalid Input", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                idgetMin.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(idTitle.Text))
            {
                MessageBox.Show("Please enter a title for the timer", "Missing Title", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                idTitle.Focus();
                return;
            }

            // Set timer values
            _timerService.Minutes = minutes;
            _timerService.Seconds = 0;
            _timerService.Title = idTitle.Text;
            _timerService.Start();

            // Clear schedule running flag for manual timer mode
            _isScheduleRunning = false;

            // Update Form2 title display
            displayTime.titleUpdate();

            idgetMin.Text = "";
            idTitle.Text = "";

            // Show timer on selected screen
            _screenService.ShowOnSelectedScreen(displayTime, true);
            UpdateMiniText();
        }

        //This button stops the secoud form from displaying.
        private void idStop_Click(object sender, EventArgs e)
        {
            _timerService.StopAndReset();
            displayTime.Visible = false;
            
            // Stop overtime timer, reset counter, and hide overtime label
            StopAndHideOvertime();
            
            // Clear schedule running flag
            _isScheduleRunning = false;
            
            UpdateMiniText();
            if (estimatedTimeGroup != null)
                estimatedTimeGroup.Visible = false;

            // Hide Change Time controls to restore initial UI state
            idExtraTime.Hide();
            idExtraMins.Hide();
            idAdd.Hide();
            idSub.Hide();
        }

        //This button activates the Third form.
        private void idSchedule_Click(object sender, EventArgs e)
        {
            if (scheduler == null || scheduler.IsDisposed)
            {
                scheduler = new Form3(_scheduleService, _timerService, _screenService);
            }
            scheduler.Show();
        }

        private void idStart_Click(object sender, EventArgs e)
        {
            idStart.Hide();
            
            if (idListBox.SelectedIndex >= 0 && idListBox.SelectedIndex < _scheduleService.ScheduleItems.Count)
                _scheduleService.CurrentIndex = idListBox.SelectedIndex;
            else
                _scheduleService.CurrentIndex = 0; // Default to first if nothing selected

            var currentItem = _scheduleService.GetCurrentItem();
            if (currentItem != null)
            {
                _timerService.Minutes = currentItem.TimeInMinutes;
                _timerService.Seconds = 0;
                _timerService.Title = currentItem.Title;
                _timerService.Start();
                
                // Set schedule running flag
                _isScheduleRunning = true;
                
                // Hide overtime display when starting schedule
                StopAndHideOvertime();
                
                // Update Form2 title display
                displayTime.titleUpdate();
            }

            idNext.Show();
            idPrevious.Show();

            // Show and maximize the timer form on the correct screen
            _screenService.ShowOnSelectedScreen(displayTime, true);
            
            // Show Change Time controls
            idExtraTime.Show();
            idExtraMins.Show();
            idAdd.Show();
            idSub.Show();
            
            // Recompute cached remaining schedule seconds when starting schedule
            RecomputeCachedRemainingScheduleSeconds();
            
            UpdateMiniText();
            UpdateEstimatedTime();
        }
        public void showStart()
        {
            //Show the start button when there are schedule items.
            if (_scheduleService.HasItems())
            {
                idStart.Show();
            }
        }

        private void idNext_Click(object sender, EventArgs e)
        {
            // Move to the next program in the array
            if (_scheduleService.CurrentIndex < _scheduleService.ScheduleItems.Count - 1)
            {
                _scheduleService.CurrentIndex++;
                var currentItem = _scheduleService.GetCurrentItem();
                if (currentItem != null)
                {
                    _timerService.Minutes = currentItem.TimeInMinutes;
                    _timerService.Seconds = 0;
                    _timerService.Title = currentItem.Title;
                    _timerService.Start();
                    
                    // Hide overtime display when moving to next item
                    StopAndHideOvertime();
                    
                    // Update Form2 title display
                    displayTime.titleUpdate();
                }
                
                // Recompute cached remaining schedule seconds when index changes
                RecomputeCachedRemainingScheduleSeconds();
            }
            else
            {
                MessageBox.Show("This is the last program left.");
            }
            UpdateEstimatedTime();
        }

        private void idPrevious_Click(object sender, EventArgs e)
        {
            // Go to the previous program in the array
            if (_scheduleService.CurrentIndex > 0)
            {
                _scheduleService.CurrentIndex--;
                var currentItem = _scheduleService.GetCurrentItem();
                if (currentItem != null)
                {
                    _timerService.Minutes = currentItem.TimeInMinutes;
                    _timerService.Seconds = 0;
                    _timerService.Title = currentItem.Title;
                    _timerService.Start();
                    
                    // Hide overtime display when moving to previous item
                    StopAndHideOvertime();
                    
                    // Update Form2 title display
                    displayTime.titleUpdate();
                }
                
                // Recompute cached remaining schedule seconds when index changes
                RecomputeCachedRemainingScheduleSeconds();
                
                UpdateMiniText();
                UpdateEstimatedTime();
            }
            else
            {
                MessageBox.Show("There is no Previous program before this.");
            }
        }

        private void idgetMin_KeyPress(object sender, KeyPressEventArgs e)
        {
            //This restrict the user from putting letters into the computer.

            if (!char.IsNumber(e.KeyChar) && (e.KeyChar != (char)Keys.Back))
                e.Handled = true;
        }

        private void idAdd_Click(object sender, EventArgs e)
        {
            //Adding to the timer.
            if (!int.TryParse(idExtraTime.Text, out int minutes) || minutes <= 0)
            {
                idExtraTime.Text = "";
                MessageBox.Show("Please enter a valid number of minutes to add", "Invalid Input", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                idExtraTime.Focus();
                return;
            }
            
            _timerService.AddTime(minutes);
            idExtraTime.Text = "";
            UpdateMiniText();
        }

        private void idSub_Click(object sender, EventArgs e)
        {
            //Subtracting in the timer.
            if (!int.TryParse(idExtraTime.Text, out int minutes) || minutes <= 0)
            {
                idExtraTime.Text = "";
                MessageBox.Show("Please enter a valid number of minutes to subtract", "Invalid Input", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                idExtraTime.Focus();
                return;
            }
            
            if (_timerService.Minutes >= minutes)
            {
                _timerService.SubtractTime(minutes);
            }
            else
            {
                MessageBox.Show("Unable to subtract - not enough time remaining", "Invalid Operation", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            
            idExtraTime.Text = "";
            UpdateMiniText();
        }

        private void idExtraTime_KeyPress(object sender, KeyPressEventArgs e)
        {
            //This restrict the user from putting letters into the TextBox .

            if (!char.IsNumber(e.KeyChar) && (e.KeyChar != (char)Keys.Back))
                e.Handled = true;
        }
        //This code edit any Title and time on the List box.
        public void idListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = idListBox.SelectedIndex;
            if (index == -1) return;
            if (index >= 0 && index < _scheduleService.ScheduleItems.Count)
            {
                var item = _scheduleService.ScheduleItems[index];
                if (scheduler != null)
                {
                    scheduler.ShowItemForEdit(item.Title, item.TimeInMinutes);
                }
            }
        }

        private void isDoubleClickedItem(object sender, EventArgs e)
        {
            if (idListBox.SelectedIndex >= 0 && idListBox.SelectedIndex < _scheduleService.ScheduleItems.Count)
            {
                _scheduleService.CurrentIndex = idListBox.SelectedIndex;
                var currentItem = _scheduleService.GetCurrentItem();
                if (currentItem != null)
                {
                    _timerService.Minutes = currentItem.TimeInMinutes;
                    _timerService.Seconds = 0;
                    _timerService.Title = currentItem.Title;
                    _timerService.Start();
                    
                    // Set schedule running flag
                    _isScheduleRunning = true;
                    
                    // Hide overtime display when double-clicking schedule item
                    StopAndHideOvertime();

                    // Update Form2 title display to keep behavior consistent
                    displayTime.titleUpdate();

                    // Show navigation controls and optionally hide Start like idStart_Click
                    idNext.Show();
                    idPrevious.Show();
                    idStart.Hide();
                    
                    // Show and maximize the timer form on the correct screen
                    _screenService.ShowOnSelectedScreen(displayTime, true);
                    
                    // Show Change Time controls
                    idExtraTime.Show();
                    idExtraMins.Show();
                    idAdd.Show();
                    idSub.Show();
                }
                
                // Recompute cached remaining schedule seconds when starting via double-click
                RecomputeCachedRemainingScheduleSeconds();
                
                UpdateMiniText();
                UpdateEstimatedTime();
            }
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void screensToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form4 to = new Form4(_screenService);
            to.Show();
        }

        // Method to refresh screen positioning for Form2
        public void RefreshScreenPositioning()
        {
            if (displayTime != null && !displayTime.IsDisposed)
            {
                int selectedScreenIndex = _screenService.GetSelectedScreenIndex();
                _screenService.ShowOnSelectedScreen(displayTime, true);
                
                // Also use Form2's direct method
                displayTime.ForceRepositionToScreen(selectedScreenIndex);
            }
        }

        private void idExtraTime_TextChanged(object sender, EventArgs e)
        {

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        //This Funciton Changes the Location of the texts on the panel when Title is change.
        private void Title1_Click(object sender, EventArgs e)
        {
            Title1.Text = Form2.Instance.getTitle();
            int x, y;
            x = (panel1.Width - Title1.Width) / 2;
            y = (panel1.Height - Title1.Height) / 2;

            Title1.Location = new Point(x, y / 2);
            Timer2.Location = new Point(x, y);
        }
        //This fuction updates the mini timer on form 1 by getting it from form 2
        private void Timer2_Click(object sender, EventArgs e)
        {
            Timer2.Text = _timerService.IsCompleted ? "Time Up" : $"{_timerService.Minutes:D2}:{_timerService.Seconds:D2}";
        }

        //This function update the timer on form 1.
        public void UpdateMiniText()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(UpdateMiniText));
                return;
            }

            Title1.Text = _timerService.Title;
            Timer2.Text = _timerService.IsCompleted ? "Time Up" : $"{_timerService.Minutes:D2}:{_timerService.Seconds:D2}";
            Timer2.ForeColor = _timerService.Minutes < 5 ? Color.Red : Theme.TextColor;
            Timer2.Visible = true; // Always visible
        }

        private void RecomputeCachedRemainingScheduleSeconds()
        {
            _cachedRemainingScheduleSeconds = 0;
            
            // Add remaining items' time in seconds
            for (int i = _scheduleService.CurrentIndex + 1; i < _scheduleService.ScheduleItems.Count; i++)
            {
                _cachedRemainingScheduleSeconds += _scheduleService.ScheduleItems[i].TimeInMinutes * 60;
            }
        }

        private void UpdateEstimatedTime()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(UpdateEstimatedTime));
                return;
            }

            if (!_scheduleService.HasItems() || !_timerService.IsRunning)
            {
                if (estimatedTimeGroup != null)
                    estimatedTimeGroup.Visible = false;
                return;
            }

            // Guard against invalid CurrentIndex to prevent out-of-range iteration
            if (_scheduleService.CurrentIndex < 0 || _scheduleService.CurrentIndex >= _scheduleService.ScheduleItems.Count)
            {
                if (estimatedTimeGroup != null)
                    estimatedTimeGroup.Visible = false;
                return;
            }

            // Gate estimated panel to schedule mode only - hide during manual timer
            if (!_isScheduleRunning)
            {
                if (estimatedTimeGroup != null)
                    estimatedTimeGroup.Visible = false;
                return;
            }

            // Use cached remaining schedule seconds for performance
            int totalSeconds = _timerService.Minutes * 60 + _timerService.Seconds + _cachedRemainingScheduleSeconds;

            // Convert totalSeconds to hours and minutes
            int hours = totalSeconds / 3600;
            int minutes = (totalSeconds % 3600) / 60;
            string displayText = $"{hours:D2}:{minutes:D2}";

            if (estimatedTimeLabel != null)
                estimatedTimeLabel.Text = displayText;
            if (estimatedTimeGroup != null)
                estimatedTimeGroup.Visible = true;
        }

        private void OvertimeTimer_Tick(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => OvertimeTimer_Tick(sender, e)));
                return;
            }

            _overtimeSeconds++;
            int minutes = _overtimeSeconds / 60;
            int seconds = _overtimeSeconds % 60;
            string displayText = $"Overtime: {minutes:D2}:{seconds:D2}";
            overtimeLabel.Text = displayText;
            overtimeLabel.Visible = true;
        }

        private void StopAndHideOvertime()
        {
            _overtimeTimer?.Stop();
            _overtimeSeconds = 0;
            if (overtimeLabel != null) overtimeLabel.Visible = false;
        }

        private void IdListBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int index = idListBox.IndexFromPoint(e.Location);
                if (index != ListBox.NoMatches)
                {
                    idListBox.SelectedIndex = index;
                }
            }
        }

        private void DeleteMenuItem_Click(object sender, EventArgs e)
        {
            int index = idListBox.SelectedIndex;
            if (index >= 0 && index < _scheduleService.ScheduleItems.Count)
            {
                _scheduleService.DeleteItem(index);
            }
        }

        private void EditMenuItem_Click(object sender, EventArgs e)
        {
            int index = idListBox.SelectedIndex;
            if (index >= 0 && index < _scheduleService.ScheduleItems.Count)
            {
                var item = _scheduleService.ScheduleItems[index];
                using (var editDialog = new EditScheduleDialog(item.Title, item.TimeInMinutes))
                {
                    if (editDialog.ShowDialog() == DialogResult.OK)
                    {
                        _scheduleService.EditItem(index, editDialog.UpdatedTitle, editDialog.UpdatedTime);
                    }
                }
            }
        }

        private void MoveUpMenuItem_Click(object sender, EventArgs e)
        {
            int index = idListBox.SelectedIndex;
            if (index > 0)
            {
                _scheduleService.MoveItem(index, -1);
                idListBox.SelectedIndex = index - 1;
            }
        }

        private void MoveDownMenuItem_Click(object sender, EventArgs e)
        {
            int index = idListBox.SelectedIndex;
            if (index >= 0 && index < idListBox.Items.Count - 1)
            {
                _scheduleService.MoveItem(index, 1);
                idListBox.SelectedIndex = index + 1;
            }
        }

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                // Stop and dispose overtime timer
                _overtimeTimer?.Stop();
                _overtimeTimer?.Dispose();
                
				_webServer?.Stop();
                if (_scheduleService.HasItems())
                {
                    _scheduleService.SaveSchedule();
                }
            }
            catch
            {
                // Do not block closing; optionally log
            }
        }
    }
}
