namespace TimerRccg
{
    partial class Form3
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form3));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.idTitle = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.idTimeRange = new System.Windows.Forms.TextBox();
            this.idAdd = new System.Windows.Forms.Button();
            this.idDone = new System.Windows.Forms.Button();
            this.idUpdate = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.label1.Location = new System.Drawing.Point(149, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "List of Programs";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(149, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Program Title";
            // 
            // idTitle
            // 
            this.idTitle.Location = new System.Drawing.Point(131, 89);
            this.idTitle.Name = "idTitle";
            this.idTitle.Size = new System.Drawing.Size(100, 20);
            this.idTitle.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(128, 126);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(107, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Program Time Range";
            // 
            // idTimeRange
            // 
            this.idTimeRange.Location = new System.Drawing.Point(131, 153);
            this.idTimeRange.Name = "idTimeRange";
            this.idTimeRange.Size = new System.Drawing.Size(100, 20);
            this.idTimeRange.TabIndex = 4;
            this.idTimeRange.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.idTimeRange_KeyPress);
            // 
            // idAdd
            // 
            this.idAdd.Location = new System.Drawing.Point(235, 206);
            this.idAdd.Name = "idAdd";
            this.idAdd.Size = new System.Drawing.Size(69, 23);
            this.idAdd.TabIndex = 5;
            this.idAdd.Text = "Add";
            this.idAdd.UseVisualStyleBackColor = true;
            this.idAdd.Click += new System.EventHandler(this.idAdd_Click);
            // 
            // idDone
            // 
            this.idDone.Location = new System.Drawing.Point(297, 345);
            this.idDone.Name = "idDone";
            this.idDone.Size = new System.Drawing.Size(75, 23);
            this.idDone.TabIndex = 6;
            this.idDone.Text = "Done";
            this.idDone.UseVisualStyleBackColor = true;
            this.idDone.Click += new System.EventHandler(this.idDone_Click);
            // 
            // idUpdate
            // 
            this.idUpdate.Location = new System.Drawing.Point(75, 206);
            this.idUpdate.Name = "idUpdate";
            this.idUpdate.Size = new System.Drawing.Size(75, 23);
            this.idUpdate.TabIndex = 7;
            this.idUpdate.Text = "Update";
            this.idUpdate.UseVisualStyleBackColor = true;
            this.idUpdate.Click += new System.EventHandler(this.idUpdate_Click);
            // 
            // Form3
            // 
            this.AcceptButton = this.idAdd;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(496, 620);
            this.Controls.Add(this.idUpdate);
            this.Controls.Add(this.idDone);
            this.Controls.Add(this.idAdd);
            this.Controls.Add(this.idTimeRange);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.idTitle);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form3";
            this.Text = "Rccg Bethel Time Scheduler";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox idTitle;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox idTimeRange;
        private System.Windows.Forms.Button idAdd;
        private System.Windows.Forms.Button idDone;
        private System.Windows.Forms.Button idUpdate;
    }
}