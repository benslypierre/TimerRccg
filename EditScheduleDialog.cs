using System;
using System.Windows.Forms;
using System.Drawing;


namespace TimerRccg
{
    public class EditScheduleDialog : Form
    {
        private TextBox titleTextBox;
        private NumericUpDown timeNumericUpDown;
        private Button okButton;
        private Button cancelButton;

        public string UpdatedTitle => titleTextBox.Text;
        public int UpdatedTime => (int)timeNumericUpDown.Value;

        public EditScheduleDialog(string currentTitle, int currentTime)
        {
            this.Text = "Edit Schedule Item";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Width = 300;
            this.Height = 180;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;
            this.AcceptButton = okButton;
            this.CancelButton = cancelButton;
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
            // Remove logo from EditScheduleDialog (logo only on Form2)

            Label titleLabel = new Label { Text = "Title:", Left = 10, Top = 20, Width = 50 };
            titleTextBox = new TextBox { Left = 70, Top = 18, Width = 200, Text = currentTitle };

            Label timeLabel = new Label { Text = "Time (min):", Left = 10, Top = 55, Width = 70 };
            timeNumericUpDown = new NumericUpDown { Left = 90, Top = 53, Width = 60, Minimum = 1, Maximum = 1000, Value = currentTime };

            okButton = new Button { Text = "OK", Left = 70, Width = 80, Top = 100, DialogResult = DialogResult.OK };
            cancelButton = new Button { Text = "Cancel", Left = 160, Width = 80, Top = 100, DialogResult = DialogResult.Cancel };

            this.Controls.Add(titleLabel);
            this.Controls.Add(titleTextBox);
            this.Controls.Add(timeLabel);
            this.Controls.Add(timeNumericUpDown);
            this.Controls.Add(okButton);
            this.Controls.Add(cancelButton);
        }
    }
} 