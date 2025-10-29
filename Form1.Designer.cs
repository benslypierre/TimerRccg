namespace TimerRccg
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.screensToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.idSetTime = new System.Windows.Forms.Button();
            this.idgetMin = new System.Windows.Forms.TextBox();
            this.idStop = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.idSchedule = new System.Windows.Forms.Button();
            this.idStart = new System.Windows.Forms.Button();
            this.idNext = new System.Windows.Forms.Button();
            this.idPrevious = new System.Windows.Forms.Button();
            this.LabelT = new System.Windows.Forms.Label();
            this.idTitle = new System.Windows.Forms.TextBox();
            this.idExtraMins = new System.Windows.Forms.Label();
            this.idExtraTime = new System.Windows.Forms.TextBox();
            this.idAdd = new System.Windows.Forms.Button();
            this.idSub = new System.Windows.Forms.Button();
            this.idListBox = new System.Windows.Forms.ListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.Program = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.Timer2 = new System.Windows.Forms.Label();
            this.Title1 = new System.Windows.Forms.Label();
            this.overtimeLabel = new System.Windows.Forms.Label();
            this.estimatedTimeGroup = new System.Windows.Forms.GroupBox();
            this.estimatedTimeLabel = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.Program.SuspendLayout();
            this.panel1.SuspendLayout();
            this.estimatedTimeGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.Highlight;
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolStripMenuItem1});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(1220, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.screensToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(56, 20);
            this.fileToolStripMenuItem.Text = "Setting";
            this.fileToolStripMenuItem.Click += new System.EventHandler(this.fileToolStripMenuItem_Click);
            // 
            // screensToolStripMenuItem
            // 
            this.screensToolStripMenuItem.Name = "screensToolStripMenuItem";
            this.screensToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            this.screensToolStripMenuItem.Text = "Screens";
            this.screensToolStripMenuItem.Click += new System.EventHandler(this.screensToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(43, 20);
            this.toolStripMenuItem1.Text = "New";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // idSetTime
            // 
            this.idSetTime.Location = new System.Drawing.Point(0, 207);
            this.idSetTime.Name = "idSetTime";
            this.idSetTime.Size = new System.Drawing.Size(75, 23);
            this.idSetTime.TabIndex = 2;
            this.idSetTime.Text = "Set ";
            this.idSetTime.UseVisualStyleBackColor = true;
            this.idSetTime.Click += new System.EventHandler(this.idSetTime_Click);
            // 
            // idgetMin
            // 
            this.idgetMin.Location = new System.Drawing.Point(207, 145);
            this.idgetMin.Name = "idgetMin";
            this.idgetMin.Size = new System.Drawing.Size(100, 20);
            this.idgetMin.TabIndex = 3;
            this.idgetMin.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.idgetMin_KeyPress);
            // 
            // idStop
            // 
            this.idStop.Location = new System.Drawing.Point(182, 207);
            this.idStop.Name = "idStop";
            this.idStop.Size = new System.Drawing.Size(75, 23);
            this.idStop.TabIndex = 4;
            this.idStop.Text = "Stop";
            this.idStop.UseVisualStyleBackColor = true;
            this.idStop.Click += new System.EventHandler(this.idStop_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(33, 145);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 20);
            this.label1.TabIndex = 5;
            this.label1.Text = "Minutes";
            // 
            // idSchedule
            // 
            this.idSchedule.Location = new System.Drawing.Point(1015, 270);
            this.idSchedule.Name = "idSchedule";
            this.idSchedule.Size = new System.Drawing.Size(183, 23);
            this.idSchedule.TabIndex = 6;
            this.idSchedule.Text = "Set Time Schedule";
            this.idSchedule.UseVisualStyleBackColor = true;
            this.idSchedule.Click += new System.EventHandler(this.idSchedule_Click);
            // 
            // idStart
            // 
            this.idStart.Location = new System.Drawing.Point(65, 30);
            this.idStart.Name = "idStart";
            this.idStart.Size = new System.Drawing.Size(75, 23);
            this.idStart.TabIndex = 7;
            this.idStart.Text = "Start";
            this.idStart.UseVisualStyleBackColor = true;
            this.idStart.Click += new System.EventHandler(this.idStart_Click);
            // 
            // idNext
            // 
            this.idNext.Location = new System.Drawing.Point(106, 67);
            this.idNext.Name = "idNext";
            this.idNext.Size = new System.Drawing.Size(75, 23);
            this.idNext.TabIndex = 8;
            this.idNext.Text = "Next";
            this.idNext.UseVisualStyleBackColor = true;
            this.idNext.Click += new System.EventHandler(this.idNext_Click);
            // 
            // idPrevious
            // 
            this.idPrevious.Location = new System.Drawing.Point(6, 67);
            this.idPrevious.Name = "idPrevious";
            this.idPrevious.Size = new System.Drawing.Size(75, 23);
            this.idPrevious.TabIndex = 9;
            this.idPrevious.Text = "Previous";
            this.idPrevious.UseVisualStyleBackColor = true;
            this.idPrevious.Click += new System.EventHandler(this.idPrevious_Click);
            // 
            // LabelT
            // 
            this.LabelT.AutoSize = true;
            this.LabelT.Font = new System.Drawing.Font("Palatino Linotype", 48F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelT.Location = new System.Drawing.Point(6, 26);
            this.LabelT.Name = "LabelT";
            this.LabelT.Size = new System.Drawing.Size(171, 85);
            this.LabelT.TabIndex = 10;
            this.LabelT.Text = "Title";
            // 
            // idTitle
            // 
            this.idTitle.Location = new System.Drawing.Point(207, 53);
            this.idTitle.Name = "idTitle";
            this.idTitle.Size = new System.Drawing.Size(100, 20);
            this.idTitle.TabIndex = 11;
            // 
            // idExtraMins
            // 
            this.idExtraMins.AutoSize = true;
            this.idExtraMins.Location = new System.Drawing.Point(0, 27);
            this.idExtraMins.Name = "idExtraMins";
            this.idExtraMins.Size = new System.Drawing.Size(56, 13);
            this.idExtraMins.TabIndex = 13;
            this.idExtraMins.Text = "Extra Mins";
            // 
            // idExtraTime
            // 
            this.idExtraTime.Location = new System.Drawing.Point(62, 24);
            this.idExtraTime.Name = "idExtraTime";
            this.idExtraTime.Size = new System.Drawing.Size(100, 20);
            this.idExtraTime.TabIndex = 14;
            this.idExtraTime.TextChanged += new System.EventHandler(this.idExtraTime_TextChanged);
            this.idExtraTime.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.idExtraTime_KeyPress);
            // 
            // idAdd
            // 
            this.idAdd.Location = new System.Drawing.Point(0, 74);
            this.idAdd.Name = "idAdd";
            this.idAdd.Size = new System.Drawing.Size(75, 23);
            this.idAdd.TabIndex = 15;
            this.idAdd.Text = "Add";
            this.idAdd.UseVisualStyleBackColor = true;
            this.idAdd.Click += new System.EventHandler(this.idAdd_Click);
            // 
            // idSub
            // 
            this.idSub.Location = new System.Drawing.Point(87, 74);
            this.idSub.Name = "idSub";
            this.idSub.Size = new System.Drawing.Size(75, 23);
            this.idSub.TabIndex = 16;
            this.idSub.Text = "Sub";
            this.idSub.UseVisualStyleBackColor = true;
            this.idSub.Click += new System.EventHandler(this.idSub_Click);
            // 
            // idListBox
            // 
            this.idListBox.FormattingEnabled = true;
            this.idListBox.Location = new System.Drawing.Point(941, 299);
            this.idListBox.Name = "idListBox";
            this.idListBox.Size = new System.Drawing.Size(257, 420);
            this.idListBox.TabIndex = 17;
            this.idListBox.SelectedIndexChanged += new System.EventHandler(this.idListBox_SelectedIndexChanged);
            this.idListBox.DoubleClick += new System.EventHandler(this.isDoubleClickedItem);
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.groupBox1.Controls.Add(this.idExtraTime);
            this.groupBox1.Controls.Add(this.idExtraMins);
            this.groupBox1.Controls.Add(this.idSub);
            this.groupBox1.Controls.Add(this.idAdd);
            this.groupBox1.Location = new System.Drawing.Point(12, 583);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(266, 136);
            this.groupBox1.TabIndex = 18;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Change Time";
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.groupBox2.Controls.Add(this.idSetTime);
            this.groupBox2.Controls.Add(this.idStop);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.LabelT);
            this.groupBox2.Controls.Add(this.idTitle);
            this.groupBox2.Controls.Add(this.idgetMin);
            this.groupBox2.Location = new System.Drawing.Point(899, 28);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(321, 236);
            this.groupBox2.TabIndex = 19;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Set Time";
            this.groupBox2.Enter += new System.EventHandler(this.groupBox2_Enter);
            // 
            // Program
            // 
            this.Program.Controls.Add(this.idPrevious);
            this.Program.Controls.Add(this.idNext);
            this.Program.Controls.Add(this.idStart);
            this.Program.Location = new System.Drawing.Point(429, 580);
            this.Program.Name = "Program";
            this.Program.Size = new System.Drawing.Size(200, 122);
            this.Program.TabIndex = 20;
            this.Program.TabStop = false;
            this.Program.Text = "Program Stater";
            // 
            // estimatedTimeGroup
            // 
            this.estimatedTimeGroup.Controls.Add(this.estimatedTimeLabel);
            this.estimatedTimeGroup.Location = new System.Drawing.Point(650, 580);
            this.estimatedTimeGroup.Name = "estimatedTimeGroup";
            this.estimatedTimeGroup.Size = new System.Drawing.Size(200, 80);
            this.estimatedTimeGroup.TabIndex = 22;
            this.estimatedTimeGroup.TabStop = false;
            this.estimatedTimeGroup.Text = "Estimated Time";
            this.estimatedTimeGroup.Visible = false;
            // 
            // estimatedTimeLabel
            // 
            this.estimatedTimeLabel.AutoSize = false;
            this.estimatedTimeLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.estimatedTimeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold);
            this.estimatedTimeLabel.Location = new System.Drawing.Point(3, 16);
            this.estimatedTimeLabel.Name = "estimatedTimeLabel";
            this.estimatedTimeLabel.Size = new System.Drawing.Size(194, 61);
            this.estimatedTimeLabel.TabIndex = 0;
            this.estimatedTimeLabel.Text = "00:00";
            this.estimatedTimeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.Timer2);
            this.panel1.Controls.Add(this.Title1);
            this.panel1.Controls.Add(this.overtimeLabel);
            this.panel1.Location = new System.Drawing.Point(12, 54);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(868, 493);
            this.panel1.TabIndex = 21;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // Timer2
            // 
            this.Timer2.AutoSize = true;
            this.Timer2.Location = new System.Drawing.Point(277, 254);
            this.Timer2.Name = "Timer2";
            this.Timer2.Size = new System.Drawing.Size(39, 13);
            this.Timer2.TabIndex = 1;
            this.Timer2.Text = "Timer2";
            this.Timer2.Click += new System.EventHandler(this.Timer2_Click);
            // 
            // Title1
            // 
            this.Title1.AutoSize = true;
            this.Title1.Location = new System.Drawing.Point(334, 143);
            this.Title1.Name = "Title1";
            this.Title1.Size = new System.Drawing.Size(27, 13);
            this.Title1.TabIndex = 0;
            this.Title1.Text = "Title";
            this.Title1.Click += new System.EventHandler(this.Title1_Click);
            // 
            // overtimeLabel
            // 
            this.overtimeLabel.AutoSize = true;
            this.overtimeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold);
            this.overtimeLabel.ForeColor = System.Drawing.Color.Yellow;
            this.overtimeLabel.Location = new System.Drawing.Point(280, 290);
            this.overtimeLabel.Name = "overtimeLabel";
            this.overtimeLabel.Size = new System.Drawing.Size(200, 31);
            this.overtimeLabel.TabIndex = 2;
            this.overtimeLabel.Text = "Overtime: 00:00";
            this.overtimeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.overtimeLabel.Visible = false;
            // 
            // Form1
            // 
            this.AcceptButton = this.idSetTime;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(1220, 729);
            this.Controls.Add(this.estimatedTimeGroup);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.Program);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.idListBox);
            this.Controls.Add(this.idSchedule);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Rccg Bethel Timer";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.Program.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.estimatedTimeGroup.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.Button idSetTime;
        private System.Windows.Forms.TextBox idgetMin;
        private System.Windows.Forms.Button idStop;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button idSchedule;
        private System.Windows.Forms.Button idStart;
        private System.Windows.Forms.Button idNext;
        private System.Windows.Forms.Button idPrevious;
        private System.Windows.Forms.Label LabelT;
        private System.Windows.Forms.TextBox idTitle;
        private System.Windows.Forms.Label idExtraMins;
        private System.Windows.Forms.TextBox idExtraTime;
        private System.Windows.Forms.Button idAdd;
        private System.Windows.Forms.Button idSub;
        internal System.Windows.Forms.ListBox idListBox;
        private System.Windows.Forms.ToolStripMenuItem screensToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox Program;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label Title1;
        private System.Windows.Forms.Label Timer2;
        private System.Windows.Forms.Label overtimeLabel;
        private System.Windows.Forms.GroupBox estimatedTimeGroup;
        private System.Windows.Forms.Label estimatedTimeLabel;
    }
}

