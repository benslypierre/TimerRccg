using System;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;

namespace TimerRccg
{
    public partial class Form3 : Form
    {
        public static Form3 programList;
        public static string title = "";
        public static int min = 0;
        private static int i = 0;
        public static Form1 main;
        public static Form2 displayTime;
        public static List<string> titleList = Form2.titleList;
        public static List<int> timeList = Form2.timeList;

        public Form3()
        {
            InitializeComponent();
            programList = this;
            displayTime = Form2.displayTime;
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
            // Remove logo from Form3 (logo only on Form2)
        }

        //Getting the title and time form user and appending to array
        private void idAdd_Click(object sender, EventArgs e)
        {
            title = idTitle.Text;
            int minValue;
            if (!int.TryParse(idTimeRange.Text, out minValue))
            {
                idTitle.Text = "";
                idTimeRange.Text = "";
                MessageBox.Show("Please fill in the minute box");
                return;
            }
            Form1.Title = idTitle.Text;
            // Add to lists
            Form2.titleList.Add(title);
            Form2.timeList.Add(minValue);
            if (Form1.main != null)
                Form1.main.UpdateListBox();
            idTitle.Text = "";
            idTimeRange.Text = "";
            i++;
            if (i == 10) i = 0;
        }

        //This button is used to signify that we are done with the list.
        private void idDone_Click(object sender, EventArgs e)
        {
            Hide();
            if (Form1.main != null)
                Form1.main.showStart();
            getValues();
        }

        public void showIndexArray()
        {
            idTitle.Text = title.ToString();
            idTimeRange.Text = min.ToString();
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
            title = idTitle.Text;
            int minValue;
            if (!int.TryParse(idTimeRange.Text, out minValue))
            {
                idTitle.Text = "";
                idTimeRange.Text = "";
                MessageBox.Show("Please fill in the minute box");
                return;
            }
            try
            {
                int selectedIndex = Form1.main.idListBox.SelectedIndex;
                if (selectedIndex >= 0 && selectedIndex < Form2.titleList.Count && Form1.main != null && Form1.main.idListBox.Items.Count > selectedIndex)
                {
                    // Update the data lists
                    Form2.titleList[selectedIndex] = title;
                    Form2.timeList[selectedIndex] = minValue;
                    // Update the ListBox item
                    Form1.main.idListBox.Items[selectedIndex] = title + " - Time :- " + minValue + " mins";
                }
                else
                {
                    MessageBox.Show("Please select a valid event to update.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating event: " + ex.Message);
            }
        }
    }
}
