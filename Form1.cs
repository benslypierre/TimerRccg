using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq; // Added for .OfType()

namespace TimerRccg
{
    public partial class Form1 : Form
    {
        //Creating two form class in other to aid the first form in fuctionality 

        public static Form3 scheduler;
        public static Form1 main; //Creating one instante of form1
        public static string Title = "";
        private static int min = 0;
        public static int index = 0;
        public static int updates = 0;
        public static List<string> titleList = new List<string>();
        public static List<int> timeList = new List<int>();
        public static bool timeUpdate = false;
        public static Form2 displayTime;
        private ContextMenuStrip listBoxContextMenu;
        private ToolStripMenuItem deleteMenuItem;
        private ToolStripMenuItem editMenuItem;
        private ToolStripMenuItem moveUpMenuItem;
        private ToolStripMenuItem moveDownMenuItem;

        // inisailizing all needed Objects.
        public Form1()
        {
            InitializeComponent();
            main = this;
            displayTime = Form2.Instance;
            scheduler = Form3.programList ?? new Form3();
            idStart.Hide();
            idPrevious.Hide();
            idNext.Hide();
            idExtraTime.Hide();
            idExtraMins.Hide();
            idAdd.Hide();
            idSub.Hide();
            // Sync lists with Form2
            titleList = new List<string>(Form2.title);
            timeList = new List<int>(Form2.time);

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

            int x, y;
            x = (panel1.Width - Timer2.Width) / 2;
            y = (panel1.Height - Timer2.Height) / 2;
            Timer2.Location = new Point(x, y);

            // This create an intance because the instance is null.
            if (displayTime == null)
            {
                displayTime = new Form2();
            }

            Title1.Text = Form2.Instance.getTitle();
            Timer2.Text = Form2.Instance.getTimer();

            Title1.Font = Form2.Instance.titleFontSyle();
            Timer2.Font = Form2.Instance.timerFontSyle();

            Title1.ForeColor = Form2.Instance.getForeColorTitle();
            Timer2.ForeColor = Form2.Instance.getForeColorTimer();

            Title1.Height = Form2.Instance.getTitleSizeHeight();
            Title1.Width = Form2.Instance.getTitleSizeWidth();

            Timer2.Height = Form2.Instance.getTimerSizeHeight();
            Timer2.Width = Form2.Instance.getTimerWidth();

            panel1.BackColor = displayTime.BackColor;

            // Always launch on the 2nd available screen if more than one screen
            Screen[] screens = Screen.AllScreens;
            if (screens.Length > 1)
            {
                this.StartPosition = FormStartPosition.Manual;
                this.Location = screens[1].WorkingArea.Location;
            }
            // Set dark blue background
            this.BackColor = Color.FromArgb(24, 32, 72); // Deep blue
            this.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            // Set form icon/logo
            // Keep the .ico for the window icon if desired, otherwise comment out or update as needed
            // this.Icon = new Icon("WhatsApp-Image-2022-11-03-at-14.13.20.ico");
            // Remove old logo placement
            // Add logo to top center
            // Remove logo from Form1 (logo only on Form2)
            // Style buttons
            // Style all buttons, including those inside GroupBoxes and Panels
            // Only declare toolTip and StyleButtons once at the top of the constructor
            ToolTip toolTip = new ToolTip();
            void StyleButtons(Control parent)
            {
                foreach (Control c in parent.Controls)
                {
                    if (c is Button btn)
                    {
                        btn.FlatStyle = FlatStyle.Flat;
                        btn.BackColor = Color.FromArgb(44, 54, 112);
                        btn.ForeColor = Color.White;
                        btn.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                        btn.FlatAppearance.BorderSize = 1;
                        btn.FlatAppearance.BorderColor = Color.White;
                        // 8% lighter hover shade
                        btn.FlatAppearance.MouseOverBackColor = ControlPaint.Light(btn.BackColor, 0.08f);
                        toolTip.SetToolTip(btn, btn.Text);
                    }
                    if (c.HasChildren)
                        StyleButtons(c);
                }
            }
            StyleButtons(this);
            // Style menu strip
            menuStrip1.BackColor = Color.FromArgb(32, 24, 72); // dark purple
            menuStrip1.ForeColor = Color.White;
            menuStrip1.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            foreach (ToolStripMenuItem item in menuStrip1.Items)
            {
                item.ForeColor = Color.White;
                item.BackColor = Color.FromArgb(32, 24, 72);
                item.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            }
            // Style ListBox
            idListBox.BackColor = Color.FromArgb(34, 44, 92);
            idListBox.ForeColor = Color.White;
            idListBox.Font = new Font("Segoe UI", 10F);
            idListBox.DrawMode = DrawMode.OwnerDrawFixed;
            idListBox.ItemHeight = 28;
            idListBox.DrawItem += (s, e) => {
                if (e.Index < 0) return;
                bool selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
                e.Graphics.FillRectangle(new SolidBrush(selected ? Color.FromArgb(64, 74, 132) : (e.Index % 2 == 0 ? Color.FromArgb(34, 44, 92) : Color.FromArgb(44, 54, 112))), e.Bounds);
                e.Graphics.DrawString(idListBox.Items[e.Index].ToString(), idListBox.Font, Brushes.White, e.Bounds.Left + 4, e.Bounds.Top + 4);
            };
            // Style GroupBoxes and Panels
            foreach (Control c in this.Controls)
            {
                if (c is GroupBox gb)
                {
                    gb.BackColor = Color.FromArgb(34, 44, 92);
                    gb.ForeColor = Color.White;
                    gb.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                }
                if (c is Panel p)
                {
                    p.BackColor = Color.FromArgb(34, 44, 92);
                }
            }
            // Style labels
            foreach (Control c in this.Controls)
            {
                if (c is Label lbl && lbl != Title1 && lbl != Timer2)
                {
                    lbl.ForeColor = Color.WhiteSmoke;
                    lbl.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
                }
            }
            Title1.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            Title1.ForeColor = Color.White;
            Timer2.Font = new Font("Segoe UI", 32F, FontStyle.Bold);
            Timer2.ForeColor = Color.White;

            // --- Modern Layout and Styling ---
            this.BackColor = ColorTranslator.FromHtml("#182048");
            this.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            // Only declare toolTip and StyleButtons once at the top of the constructor
            // Helper for margins/padding
            void SetMargins(Control c, int top = 4, int bottom = 4, int left = 8, int right = 8)
            {
                c.Margin = new Padding(left, top, right, bottom);
            }

            // Style all groupboxes/panels
            foreach (Control c in this.Controls)
            {
                if (c is GroupBox gb)
                {
                    gb.BackColor = ColorTranslator.FromHtml("#25315A");
                    gb.Padding = new Padding(12);
                    gb.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
                    SetMargins(gb, 16, 16, 16, 16);
                }
                if (c is Panel p)
                {
                    p.BackColor = ColorTranslator.FromHtml("#25315A");
                    p.Padding = new Padding(12);
                    SetMargins(p, 16, 16, 16, 16);
                }
            }

            // Style all labels and inputs
            foreach (Control c in this.Controls)
            {
                if (c is Label lbl)
                {
                    lbl.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
                    lbl.Location = new Point(16, lbl.Location.Y);
                    SetMargins(lbl);
                }
                if (c is TextBox tb)
                {
                    tb.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
                    tb.Size = new Size(160, 28);
                    tb.Location = new Point(120, tb.Location.Y);
                    SetMargins(tb);
                }
                if (c is Button btn)
                {
                    btn.Size = new Size(88, 30);
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.BackColor = Color.FromArgb(44, 54, 112);
                    btn.ForeColor = Color.White;
                    btn.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                    btn.FlatAppearance.BorderSize = 1;
                    btn.FlatAppearance.BorderColor = Color.White;
                    btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(80, 100, 180); // distinct hover
                    SetMargins(btn);
                }
            }

            // Style ListBox
            idListBox.BackColor = Color.FromArgb(34, 44, 92);
            idListBox.ForeColor = Color.White;
            idListBox.Font = new Font("Segoe UI", 10F);
            idListBox.DrawMode = DrawMode.OwnerDrawFixed;
            idListBox.ItemHeight = 32;
            idListBox.Width = 320;
            idListBox.DrawItem += (s, e) => {
                if (e.Index < 0) return;
                bool selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
                e.Graphics.FillRectangle(new SolidBrush(selected ? Color.FromArgb(64, 74, 132) : (e.Index % 2 == 0 ? Color.FromArgb(34, 44, 92) : Color.FromArgb(44, 54, 112))), e.Bounds);
                e.Graphics.DrawString(idListBox.Items[e.Index].ToString(), idListBox.Font, Brushes.White, e.Bounds.Left + 4, e.Bounds.Top + 6);
            };

            // Style headers
            Title1.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            Title1.ForeColor = Color.White;
            SetMargins(Title1, 8, 8, 8, 8);

            // Style timer
            Timer2.Font = new Font("Segoe UI", 32F, FontStyle.Bold);
            Timer2.ForeColor = Color.White;
            SetMargins(Timer2, 8, 8, 8, 8);

            // Center and space Prev/Next
            idPrevious.Size = new Size(88, 30);
            idNext.Size = new Size(88, 30);
            idPrevious.Location = new Point(Program.Width / 2 - idPrevious.Width - 8, idPrevious.Location.Y);
            idNext.Location = new Point(Program.Width / 2 + 8, idNext.Location.Y);
            SetMargins(idPrevious, 4, 4, 8, 8);
            SetMargins(idNext, 4, 4, 8, 8);

            // Insert 1px separators (Panels) or 16px gutters between major sections
            // (Assume major sections are already separated by GroupBoxes/Panels)

            // Set logical Tab order
            int tabIndex = 0;
            void SetTabOrder(Control parent)
            {
                foreach (Control c in parent.Controls)
                {
                    c.TabIndex = tabIndex++;
                    if (c.HasChildren) SetTabOrder(c);
                }
            }
            SetTabOrder(this);

            // Add tooltips for each control
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

            // --- Enhanced GroupBox Layout and Styling ---
            // Only declare toolTip and StyleButtons once
            // Style all buttons (including hover shade)
            Color groupBoxBg = ColorTranslator.FromHtml("#25315A");
            Color separatorColor = ColorTranslator.FromHtml("#1B2247");
            Font groupHeaderFont = new Font("Segoe UI", 12F, FontStyle.Bold);
            Font labelFont = new Font("Segoe UI", 10F, FontStyle.Regular);
            Font inputFont = new Font("Segoe UI", 10F, FontStyle.Regular);

            foreach (Control c in this.Controls)
            {
                if (c is GroupBox gb)
                {
                    gb.BackColor = groupBoxBg;
                    gb.ForeColor = Color.White;
                    gb.Font = groupHeaderFont;
                    gb.Padding = new Padding(8, 20, 8, 8); // extra top padding

                    // Create TableLayoutPanel for 2-column layout
                    TableLayoutPanel table = new TableLayoutPanel();
                    table.ColumnCount = 2;
                    table.RowCount = 0;
                    table.Dock = DockStyle.Fill;
                    table.Padding = new Padding(8);
                    table.AutoSize = true;
                    table.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                    table.BackColor = groupBoxBg;
                    table.CellBorderStyle = TableLayoutPanelCellBorderStyle.None;
                    table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
                    table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180));

                    // Move all label/input pairs into the table
                    List<Control> toRemove = new List<Control>();
                    for (int i = 0; i < gb.Controls.Count; i++)
                    {
                        if (gb.Controls[i] is Label lbl)
                        {
                            lbl.Font = labelFont;
                            lbl.ForeColor = Color.White;
                            lbl.TextAlign = ContentAlignment.MiddleRight;
                            lbl.Dock = DockStyle.Fill;
                            toRemove.Add(lbl);
                            // Find the next input control
                            Control input = null;
                            for (int j = i + 1; j < gb.Controls.Count; j++)
                            {
                                if (gb.Controls[j] is TextBox || gb.Controls[j] is ComboBox || gb.Controls[j] is NumericUpDown)
                                {
                                    input = gb.Controls[j];
                                    break;
                                }
                            }
                            if (input != null)
                            {
                                input.Font = inputFont;
                                input.Size = new Size(160, 28);
                                input.Dock = DockStyle.Left;
                                toRemove.Add(input);
                                table.RowCount++;
                                table.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));
                                table.Controls.Add(lbl, 0, table.RowCount - 1);
                                table.Controls.Add(input, 1, table.RowCount - 1);
                                // Add tooltip if not already present
                                if (string.IsNullOrEmpty(toolTip.GetToolTip(input)))
                                    toolTip.SetToolTip(input, lbl.Text);
                            }
                        }
                    }
                    // Remove old controls
                    foreach (var ctrl in toRemove)
                        gb.Controls.Remove(ctrl);
                    gb.Controls.Add(table);

                    // Insert 1px separator panel between logical sections if needed
                    // (Assume logical sections are separated by GroupBoxes or can be added manually)
                }
            }
            StyleButtons(this);

            // --- Enhanced Set Time GroupBox Layout ---
            // Find the Set Time group box (by name or header text)
            GroupBox setTimeGroup = null;
            foreach (Control c in this.Controls)
            {
                if (c is GroupBox gb && (gb.Text.Trim().ToLower().Contains("set time") || gb.Name.ToLower().Contains("settime")))
                {
                    setTimeGroup = gb;
                    break;
                }
            }
            if (setTimeGroup != null)
            {
                // Remove all controls from the group box
                var controls = setTimeGroup.Controls.OfType<Control>().ToList();
                foreach (var ctrl in controls)
                    setTimeGroup.Controls.Remove(ctrl);

                // Create a new TableLayoutPanel
                TableLayoutPanel table = new TableLayoutPanel();
                table.ColumnCount = 2;
                table.RowCount = 0;
                table.Dock = DockStyle.Fill;
                table.Padding = new Padding(8, 16, 8, 16);
                table.AutoSize = true;
                table.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                table.BackColor = setTimeGroup.BackColor;
                table.CellBorderStyle = TableLayoutPanelCellBorderStyle.None;
                table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
                table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180));

                // Add label/input pairs with consistent spacing and font
                void AddRow(string labelText, Control input, string tooltipText)
                {
                    Label lbl = new Label();
                    lbl.Text = labelText;
                    lbl.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
                    lbl.ForeColor = Color.White;
                    lbl.TextAlign = ContentAlignment.MiddleRight;
                    lbl.Dock = DockStyle.Fill;
                    input.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
                    input.Size = new Size(160, 28);
                    input.Dock = DockStyle.Left;
                    table.RowCount++;
                    table.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));
                    table.Controls.Add(lbl, 0, table.RowCount - 1);
                    table.Controls.Add(input, 1, table.RowCount - 1);
                    toolTip.SetToolTip(input, tooltipText);
                }
                // Add Title row
                AddRow("Title", idTitle, "Enter the title for the timer");
                // Add Minutes row
                AddRow("Minutes", idgetMin, "Enter minutes for the timer");

                // Add buttons row (centered horizontally, spanning both columns)
                FlowLayoutPanel buttonPanel = new FlowLayoutPanel();
                buttonPanel.FlowDirection = FlowDirection.LeftToRight;
                buttonPanel.Dock = DockStyle.Fill;
                buttonPanel.AutoSize = true;
                buttonPanel.WrapContents = false;
                buttonPanel.Padding = new Padding(0, 8, 0, 0);
                buttonPanel.Anchor = AnchorStyles.None;
                buttonPanel.Controls.Add(idSetTime);
                buttonPanel.Controls.Add(idStop);
                idSetTime.Margin = new Padding(8, 0, 8, 0);
                idStop.Margin = new Padding(8, 0, 8, 0);
                table.RowCount++;
                table.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
                table.Controls.Add(buttonPanel, 0, table.RowCount - 1);
                table.SetColumnSpan(buttonPanel, 2);

                setTimeGroup.Controls.Add(table);

                // Align ListBox width and X position to match Set Time group box
                idListBox.Width = setTimeGroup.Width;
                idListBox.Left = setTimeGroup.Left;
                // Anchor both controls to Top, Left, and Right so they resize with the window
                idListBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                setTimeGroup.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            }
            // --- Enhanced Change Time GroupBox Layout ---
            GroupBox changeTimeGroup = null;
            foreach (Control c in this.Controls)
            {
                if (c is GroupBox gb && (gb.Text.Trim().ToLower().Contains("change time") || gb.Name.ToLower().Contains("changetime")))
                {
                    changeTimeGroup = gb;
                    break;
                }
            }
            if (changeTimeGroup != null)
            {
                // Remove all controls from the group box
                var controls = changeTimeGroup.Controls.OfType<Control>().ToList();
                foreach (var ctrl in controls)
                    changeTimeGroup.Controls.Remove(ctrl);

                // Create a new TableLayoutPanel
                TableLayoutPanel table = new TableLayoutPanel();
                table.ColumnCount = 2;
                table.RowCount = 0;
                table.Dock = DockStyle.Fill;
                table.Padding = new Padding(8, 16, 8, 16);
                table.AutoSize = true;
                table.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                table.BackColor = changeTimeGroup.BackColor;
                table.CellBorderStyle = TableLayoutPanelCellBorderStyle.None;
                table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
                table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));

                // Add label/input pairs with consistent spacing and font
                void AddRow2(string labelText, Control input, string tooltipText)
                {
                    Label lbl = new Label();
                    lbl.Text = labelText;
                    lbl.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
                    lbl.ForeColor = Color.White;
                    lbl.TextAlign = ContentAlignment.MiddleRight;
                    lbl.Dock = DockStyle.Fill;
                    input.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
                    input.MinimumSize = new Size(160, 28);
                    input.Dock = DockStyle.Fill;
                    input.Anchor = AnchorStyles.Left | AnchorStyles.Right;
                    table.RowCount++;
                    table.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));
                    table.Controls.Add(lbl, 0, table.RowCount - 1);
                    table.Controls.Add(input, 1, table.RowCount - 1);
                    toolTip.SetToolTip(input, tooltipText);
                }
                // Add Extra Mins row
                AddRow2("Extra Mins", idExtraTime, "Enter extra minutes to add or subtract");

                // Add buttons row (centered horizontally, spanning both columns)
                FlowLayoutPanel buttonPanel = new FlowLayoutPanel();
                buttonPanel.FlowDirection = FlowDirection.LeftToRight;
                buttonPanel.Dock = DockStyle.Fill;
                buttonPanel.AutoSize = true;
                buttonPanel.WrapContents = false;
                buttonPanel.Padding = new Padding(0, 8, 0, 0);
                buttonPanel.Anchor = AnchorStyles.None;
                idAdd.Width = 88;
                idSub.Width = 88;
                idAdd.Anchor = AnchorStyles.None;
                idSub.Anchor = AnchorStyles.None;
                buttonPanel.Controls.Add(idAdd);
                buttonPanel.Controls.Add(idSub);
                idAdd.Margin = new Padding(8, 0, 8, 0);
                idSub.Margin = new Padding(8, 0, 8, 0);
                table.RowCount++;
                table.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
                table.Controls.Add(buttonPanel, 0, table.RowCount - 1);
                table.SetColumnSpan(buttonPanel, 2);

                changeTimeGroup.Controls.Add(table);
            }
            // --- Center Title1 and Timer2 in the main panel and increase Timer2 font size ---
            if (panel1 != null && Title1 != null && Timer2 != null)
            {
                // Increase Timer2 font size
                Timer2.Font = new Font("Segoe UI", 48F, FontStyle.Bold);
                // Center both controls in the panel
                int panelWidth = panel1.Width;
                int panelHeight = panel1.Height;
                // Measure text size for vertical centering
                using (Graphics g = panel1.CreateGraphics())
                {
                    SizeF titleSize = g.MeasureString(Title1.Text, Title1.Font);
                    SizeF timerSize = g.MeasureString(Timer2.Text, Timer2.Font);
                    int totalHeight = (int)titleSize.Height + 24 + (int)timerSize.Height; // 24px gap
                    int startY = (panelHeight - totalHeight) / 2;
                    Title1.Location = new Point((panelWidth - (int)titleSize.Width) / 2, startY);
                    Timer2.Location = new Point((panelWidth - (int)timerSize.Width) / 2, startY + (int)titleSize.Height + 24);
                }
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        //This button objects updates the form two by setting a simple timer
        private void idSetTime_Click(object sender, EventArgs e)
        {
            timeUpdate = false;
            idExtraTime.Show();
            idExtraMins.Show();
            idAdd.Show();
            idSub.Show();

            try
            {
                min = Convert.ToInt32(idgetMin.Text);
            }
            catch (Exception a)
            {
                idgetMin.Text = "";
                idTitle.Text = "";
                MessageBox.Show("Please fill in the minute box");
                Console.WriteLine(a.Message);

                return;
            }

            Form2.min = min;
            Form2.sec = 0;
            Form2.Instance.Visible = true;
            Form2.Instance.update();
            Form2.Mtitle = idTitle.Text;
            Form2.Instance.titleUpdate();

            idgetMin.Text = "";

            // Always show the timer (Form2) on the user-selected screen if available, otherwise 2nd screen
            Screen[] screens = Screen.AllScreens;
            int screenIndex = 1; // default to 2nd screen
            if (Form4.SelectedScreenIndex.HasValue && Form4.SelectedScreenIndex.Value >= 0 && Form4.SelectedScreenIndex.Value < screens.Length)
            {
                screenIndex = Form4.SelectedScreenIndex.Value;
            }
            else if (screens.Length == 1)
            {
                screenIndex = 0; // only one screen
            }
            displayTime.StartPosition = FormStartPosition.Manual;
            displayTime.Location = screens[screenIndex].WorkingArea.Location;
            displayTime.Show();
            displayTime.BringToFront();
            updateMiniText();
        }

        //This button stops the secoud form from displaying.
        private void idStop_Click(object sender, EventArgs e)
        {
            timeUpdate = true;

            displayTime.Visible = false;
            Form2.min = 0;
            Form2.sec = 0;
            getTime();
            Form2.Instance.titleUpdate();

            Form2.Instance.update();
            updateMiniText();
        }

        //This button activates the Third form.
        private void idSchedule_Click(object sender, EventArgs e)
        {
            if (scheduler == null || scheduler.IsDisposed)
            {
                scheduler = new Form3();
                Form3.programList = scheduler;
            }
            scheduler.Show();
        }

        private void idStart_Click(object sender, EventArgs e)
        {
            idStart.Hide();
            if (idListBox.SelectedIndex >= 0 && idListBox.SelectedIndex < Form2.title.Length)
                Form2.i = idListBox.SelectedIndex;
            else
                Form2.i = 0; // Default to first if nothing selected

            Form2.Instance.timeSchedual();
            idNext.Show();
            idPrevious.Show();

            // Show and maximize the timer form on the correct screen
            Screen[] screens = Screen.AllScreens;
            int screenIndex = 1; // default to 2nd screen
            if (Form4.SelectedScreenIndex.HasValue && Form4.SelectedScreenIndex.Value >= 0 && Form4.SelectedScreenIndex.Value < screens.Length)
            {
                screenIndex = Form4.SelectedScreenIndex.Value;
            }
            else if (screens.Length == 1)
            {
                screenIndex = 0; // only one screen
            }
            Form2.Instance.StartPosition = FormStartPosition.Manual;
            Form2.Instance.Location = screens[screenIndex].WorkingArea.Location;
            Form2.Instance.WindowState = FormWindowState.Maximized;
            Form2.Instance.Show();
            Form2.Instance.BringToFront();
            updateMiniText();
        }
        public void showStart()
        {
            //Show the start button.
            idStart.Show();
        }

        private void idNext_Click(object sender, EventArgs e)
        {
            // Move to the next program in the array
            if (Form2.i < Form2.title.Length - 1)
            {
                Form2.i++;
                Form2.Instance.timeSchedual();
            }
            else
            {
                MessageBox.Show("This is the last program left.");
            }
        }

        private void idPrevious_Click(object sender, EventArgs e)
        {
            // Go to the previous program in the array
            if (Form2.i > 0)
            {
                Form2.i--;
                Form2.Instance.timeSchedual();
                updateMiniText();
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
            int min;
            try
            {
                min = Convert.ToInt32(idExtraTime.Text);
            }
            catch
            {
                idExtraTime.Text = "";

                MessageBox.Show("Please fill in the Extra Time box");
                return;
            }
            Form2.min += min;
            Form2.Instance.update();

            idExtraTime.Text = "";
            updateMiniText();
        }

        private void idSub_Click(object sender, EventArgs e)
        {
            //Subtracting in the timer.
            int min;
            try
            {
                min = Convert.ToInt32(idExtraTime.Text);
            }
            catch
            {
                idExtraTime.Text = "";
                MessageBox.Show("Please fill in the Extra Time box");
                return;
            }
            if (Form2.min != 0 && Form2.min > min)
            {
                Form2.min -= min;
            }

            else if (Form2.min == 0 || Form2.min <= 0 || Form2.min < min)
                MessageBox.Show("Unable to subtract");
            Form2.Instance.update();
            idExtraTime.Text = "";
            updateMiniText();
        }

        private void idExtraTime_KeyPress(object sender, KeyPressEventArgs e)
        {
            //This restrict the user from putting letters into the TextBox .

            if (!char.IsNumber(e.KeyChar) && (e.KeyChar != (char)Keys.Back))
                e.Handled = true;
        }
        //This code update the title and shows it on the List box
        public void UpdateListBox()
        {
            if (updates == 0)
            {
                if (Form2.time.Length > idListBox.Items.Count)
                    idListBox.Items.Add(Title + " - Time :- " + Form2.time[idListBox.Items.Count] + " mins");
            }
            else
            {
                int updatedIndex = index;
                if (index >= 0 && index < idListBox.Items.Count && updatedIndex < Form2.title.Length && updatedIndex < Form2.time.Length)
                {
                    idListBox.Items.RemoveAt(index);
                    idListBox.Items.Insert(updatedIndex, (Form2.title[updatedIndex] + " - Time :- " + Form2.time[updatedIndex] + " mins"));
                    index = updatedIndex;
                }
                updates = 0;
            }
        }

        //This code edit any Title and time on the List box.
        public void idListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            index = idListBox.SelectedIndex;
            if (index == -1) return;
            if (index < Form2.title.Length && index < Form2.time.Length)
            {
                Form3.title = Form2.title[index];
                Form3.min = Form2.time[index];
                if (Form3.programList != null)
                    Form3.programList.showIndexArray();
            }
        }

        private void isDoubleClickedItem(object sender, EventArgs e)
        {
            if (idListBox.SelectedIndex >= 0 && idListBox.SelectedIndex < Form2.title.Length)
            {
                Form2.i = idListBox.SelectedIndex;
                Form2.Instance.timeSchedual();
                updateMiniText();
            }
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void screensToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form4 to = new Form4();
            to.Show();
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
            Timer2.Text = Form2.Instance.getTimer();
        }

        //This function update the timer on form 1.
        public void updateMiniText()
        {
            if (Form2.Instance == null)
            {
                Console.WriteLine("Form2.Instance is null, skipping update.");
                return;
            }

            Title1.Text = Form2.Instance.getTitle();
            Timer2.Text = Form2.Instance.getTimer();
            Timer2.ForeColor = Form2.Instance.getForeColorTimer();
            Timer2.Visible = Form2.Instance.Visible;
        }
        public void getTime()
        {
            Form2.Mtitle = "";
            DateTime currentTime = DateTime.Now;
            Form2.Mtitle = currentTime.ToString("hh:mm:ss tt");
            Form2.Instance.titleUpdate();
            Form2.Instance.Mtimer("");
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
            if (index >= 0 && index < Form2.titleList.Count)
            {
                // Remove from data lists
                Form2.titleList.RemoveAt(index);
                Form2.timeList.RemoveAt(index);
                // Remove from ListBox
                idListBox.Items.RemoveAt(index);
            }
        }

        private void EditMenuItem_Click(object sender, EventArgs e)
        {
            int index = idListBox.SelectedIndex;
            if (index >= 0 && index < Form2.titleList.Count)
            {
                string currentTitle = Form2.titleList[index];
                int currentTime = Form2.timeList[index];
                using (var editDialog = new EditScheduleDialog(currentTitle, currentTime))
                {
                    if (editDialog.ShowDialog() == DialogResult.OK)
                    {
                        // Update the lists
                        Form2.titleList[index] = editDialog.UpdatedTitle;
                        Form2.timeList[index] = editDialog.UpdatedTime;
                        // Update ListBox display
                        idListBox.Items[index] = $"{editDialog.UpdatedTitle} - Time :- {editDialog.UpdatedTime} mins";
                    }
                }
            }
        }

        private void MoveUpMenuItem_Click(object sender, EventArgs e)
        {
            int index = idListBox.SelectedIndex;
            if (index > 0)
            {
                // Swap in data lists
                var tempTitle = Form2.titleList[index - 1];
                var tempTime = Form2.timeList[index - 1];
                Form2.titleList[index - 1] = Form2.titleList[index];
                Form2.timeList[index - 1] = Form2.timeList[index];
                Form2.titleList[index] = tempTitle;
                Form2.timeList[index] = tempTime;

                // Swap in ListBox
                var tempItem = idListBox.Items[index - 1];
                idListBox.Items[index - 1] = idListBox.Items[index];
                idListBox.Items[index] = tempItem;

                idListBox.SelectedIndex = index - 1;
            }
        }

        private void MoveDownMenuItem_Click(object sender, EventArgs e)
        {
            int index = idListBox.SelectedIndex;
            if (index >= 0 && index < idListBox.Items.Count - 1)
            {
                // Swap in data lists
                var tempTitle = Form2.titleList[index + 1];
                var tempTime = Form2.timeList[index + 1];
                Form2.titleList[index + 1] = Form2.titleList[index];
                Form2.timeList[index + 1] = Form2.timeList[index];
                Form2.titleList[index] = tempTitle;
                Form2.timeList[index] = tempTime;

                // Swap in ListBox
                var tempItem = idListBox.Items[index + 1];
                idListBox.Items[index + 1] = idListBox.Items[index];
                idListBox.Items[index] = tempItem;

                idListBox.SelectedIndex = index + 1;
            }
        }
    }
}
