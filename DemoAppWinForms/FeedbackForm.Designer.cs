namespace DemoAppWinForms
{
    partial class FeedbackForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.tbxContent = new System.Windows.Forms.TextBox();
            this.chbxEmail = new System.Windows.Forms.CheckBox();
            this.tbxEmail = new System.Windows.Forms.TextBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(389, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "We appreciate your feedback. Tell us what do you like or what we can do better.";
            // 
            // tbxContent
            // 
            this.tbxContent.Location = new System.Drawing.Point(12, 34);
            this.tbxContent.Multiline = true;
            this.tbxContent.Name = "tbxContent";
            this.tbxContent.Size = new System.Drawing.Size(439, 134);
            this.tbxContent.TabIndex = 1;
            // 
            // chbxEmail
            // 
            this.chbxEmail.AutoSize = true;
            this.chbxEmail.Location = new System.Drawing.Point(12, 176);
            this.chbxEmail.Name = "chbxEmail";
            this.chbxEmail.Size = new System.Drawing.Size(143, 17);
            this.chbxEmail.TabIndex = 2;
            this.chbxEmail.Text = "Include an email address";
            this.chbxEmail.UseVisualStyleBackColor = true;
            this.chbxEmail.CheckedChanged += new System.EventHandler(this.Email_CheckedChanged);
            // 
            // tbxEmail
            // 
            this.tbxEmail.Location = new System.Drawing.Point(175, 174);
            this.tbxEmail.Name = "tbxEmail";
            this.tbxEmail.Size = new System.Drawing.Size(276, 20);
            this.tbxEmail.TabIndex = 3;
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(338, 203);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(113, 33);
            this.btnSend.TabIndex = 4;
            this.btnSend.Text = "Send Feedback";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.Send_Click);
            // 
            // FeedbackForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(463, 248);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.tbxEmail);
            this.Controls.Add(this.chbxEmail);
            this.Controls.Add(this.tbxContent);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FeedbackForm";
            this.Text = "Feedback Form";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbxContent;
        private System.Windows.Forms.CheckBox chbxEmail;
        private System.Windows.Forms.TextBox tbxEmail;
        private System.Windows.Forms.Button btnSend;
    }
}