namespace TimerRccg
{
    partial class Form2
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form2));
            this.idTimer = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.idTitle = new System.Windows.Forms.Label();
            this.TimeLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // idTimer
            // 
            this.idTimer.AutoSize = true;
            this.idTimer.Font = new System.Drawing.Font("Microsoft YaHei", 72F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.idTimer.Location = new System.Drawing.Point(238, 260);
            this.idTimer.Name = "idTimer";
            this.idTimer.Size = new System.Drawing.Size(310, 128);
            this.idTimer.TabIndex = 0;
            this.idTimer.Text = "Label";
            this.idTimer.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.idTimer.Click += new System.EventHandler(this.idTimer_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // idTitle
            // 
            this.idTitle.AutoSize = true;
            this.idTitle.Font = new System.Drawing.Font("Lucida Bright", 21.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.idTitle.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.idTitle.Location = new System.Drawing.Point(278, 165);
            this.idTitle.Name = "idTitle";
            this.idTitle.Size = new System.Drawing.Size(81, 34);
            this.idTitle.TabIndex = 1;
            this.idTitle.Text = "Title";
            this.idTitle.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.idTitle.Click += new System.EventHandler(this.idTitle_Click);
            // 
            // TimeLabel
            // 
            this.TimeLabel.AutoSize = true;
            this.TimeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 72F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TimeLabel.Location = new System.Drawing.Point(906, 401);
            this.TimeLabel.Name = "TimeLabel";
            this.TimeLabel.Size = new System.Drawing.Size(72, 108);
            this.TimeLabel.TabIndex = 2;
            this.TimeLabel.Text = ".";
            this.TimeLabel.Click += new System.EventHandler(this.TimeLabel_Click);
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.ClientSize = new System.Drawing.Size(1010, 518);
            this.Controls.Add(this.TimeLabel);
            this.Controls.Add(this.idTitle);
            this.Controls.Add(this.idTimer);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form2";
            this.Text = "Rccg Bethel Timer";
            this.Load += new System.EventHandler(this.Form2_Load);
            this.Resize += new System.EventHandler(this.Form2_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label idTimer;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label idTitle;
        private System.Windows.Forms.Label TimeLabel;
    }
}