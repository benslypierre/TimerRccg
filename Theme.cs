using System;
using System.Drawing;
using System.Windows.Forms;

namespace TimerRccg
{
    public static class Theme
    {
        // Color scheme
        public static readonly Color PrimaryBackground = Color.FromArgb(24, 32, 72); // Deep blue
        public static readonly Color SecondaryBackground = Color.FromArgb(34, 44, 92);
        public static readonly Color TertiaryBackground = Color.FromArgb(44, 54, 112);
        public static readonly Color GroupBoxBackground = Color.FromArgb(37, 49, 90); // Fixed: proper dark blue instead of pink
        public static readonly Color ButtonBackground = Color.FromArgb(44, 54, 112);
        public static readonly Color ButtonHoverBackground = Color.FromArgb(80, 100, 180);
        public static readonly Color TextColor = Color.White;
        public static readonly Color SecondaryTextColor = Color.WhiteSmoke;

        // Fonts
        public static readonly Font DefaultFont = new Font("Segoe UI", 10F, FontStyle.Regular);
        public static readonly Font BoldFont = new Font("Segoe UI", 10F, FontStyle.Bold);
        public static readonly Font GroupBoxFont = new Font("Segoe UI", 12F, FontStyle.Bold);
        public static readonly Font LargeFont = new Font("Segoe UI", 14F, FontStyle.Bold);
        public static readonly Font TitleFont = new Font("Segoe UI", 18F, FontStyle.Bold);
        public static readonly Font TimerFont = new Font("Segoe UI", 32F, FontStyle.Bold);
        public static readonly Font LargeTimerFont = new Font("Segoe UI", 48F, FontStyle.Bold);

        public static void Apply(Form form)
        {
            form.BackColor = PrimaryBackground;
            form.Font = DefaultFont;
        }

        public static void Apply(Button button, bool includeHover = true)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.BackColor = ButtonBackground;
            button.ForeColor = TextColor;
            button.Font = BoldFont;
            button.FlatAppearance.BorderSize = 1;
            button.FlatAppearance.BorderColor = TextColor;
            
            if (includeHover)
            {
                button.FlatAppearance.MouseOverBackColor = ButtonHoverBackground;
            }
        }

        public static void Apply(Label label, bool isSecondary = false)
        {
            label.ForeColor = isSecondary ? SecondaryTextColor : TextColor;
            label.Font = DefaultFont;
        }

        public static void Apply(GroupBox groupBox)
        {
            groupBox.BackColor = GroupBoxBackground;
            groupBox.ForeColor = TextColor;
            groupBox.Font = GroupBoxFont;
            groupBox.Padding = new Padding(12);
        }

        public static void Apply(Panel panel)
        {
            panel.BackColor = SecondaryBackground;
            panel.Padding = new Padding(12);
        }

        public static void Apply(TextBox textBox)
        {
            textBox.Font = DefaultFont;
            textBox.Size = new Size(160, 28);
        }

        public static void Apply(ListBox listBox)
        {
            listBox.BackColor = SecondaryBackground;
            listBox.ForeColor = TextColor;
            listBox.Font = DefaultFont;
            listBox.DrawMode = DrawMode.OwnerDrawFixed;
            listBox.ItemHeight = 28;
            listBox.DrawItem += ListBox_DrawItem;
        }

        public static void Apply(MenuStrip menuStrip)
        {
            menuStrip.BackColor = Color.FromArgb(32, 24, 72); // Dark purple
            menuStrip.ForeColor = TextColor;
            menuStrip.Font = BoldFont;
            
            foreach (ToolStripMenuItem item in menuStrip.Items)
            {
                item.ForeColor = TextColor;
                item.BackColor = Color.FromArgb(32, 24, 72);
                item.Font = BoldFont;
            }
        }

        private static void ListBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;
            
            var listBox = sender as ListBox;
            if (listBox == null) return;
            
            bool selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
            Color backgroundColor = selected ? 
                Color.FromArgb(64, 74, 132) : 
                (e.Index % 2 == 0 ? SecondaryBackground : TertiaryBackground);
                
            e.Graphics.FillRectangle(new SolidBrush(backgroundColor), e.Bounds);
            e.Graphics.DrawString(listBox.Items[e.Index].ToString(), listBox.Font, Brushes.White, 
                e.Bounds.Left + 4, e.Bounds.Top + 4);
        }

        public static void ApplyToAllControls(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                ApplyControl(control);
                if (control.HasChildren)
                {
                    ApplyToAllControls(control);
                }
            }
        }

        private static void ApplyControl(Control control)
        {
            switch (control)
            {
                case Button btn:
                    Apply(btn);
                    break;
                case Label lbl when !(lbl.Name == "Title1" || lbl.Name == "Timer2"):
                    Apply(lbl, true);
                    break;
                case GroupBox gb:
                    Apply(gb);
                    break;
                case Panel p:
                    Apply(p);
                    break;
                case TextBox tb:
                    Apply(tb);
                    break;
                case ListBox lb:
                    Apply(lb);
                    break;
                case MenuStrip ms:
                    Apply(ms);
                    break;
            }
        }

        public static void CenterInPanel(Control control, Panel panel)
        {
            if (control == null || panel == null) return;
            
            control.Location = new Point(
                (panel.Width - control.Width) / 2,
                (panel.Height - control.Height) / 2
            );
        }

        public static void CenterInPanel(Control[] controls, Panel panel, int spacing = 24)
        {
            if (controls == null || panel == null || controls.Length == 0) return;
            
            int totalHeight = 0;
            foreach (var control in controls)
            {
                totalHeight += control.Height;
            }
            totalHeight += (controls.Length - 1) * spacing;
            
            int startY = (panel.Height - totalHeight) / 2;
            int currentY = startY;
            
            foreach (var control in controls)
            {
                control.Location = new Point((panel.Width - control.Width) / 2, currentY);
                currentY += control.Height + spacing;
            }
        }
    }
}
