//==============================================================
// ProgReporter
// Copyright © Miroslav Popov. All rights reserved.
//==============================================================
// THIS CODE IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE.
//==============================================================

using System;
using System.Windows.Forms;

namespace DemoAppWinForms
{
    public partial class FeedbackForm : Form
    {
        public FeedbackForm()
        {
            InitializeComponent();

            tbxEmail.Enabled = false;
            chbxEmail.Checked = false;
        }

        private void Email_CheckedChanged(object sender, EventArgs e)
        {
            tbxEmail.Enabled = chbxEmail.Checked;
        }

        private void Send_Click(object sender, EventArgs e)
        {
            string content = tbxContent.Text;
            string email = chbxEmail.Checked ? tbxEmail.Text : string.Empty;

            if (string.IsNullOrEmpty(tbxContent.Text)) return;

            OnFeedbackSend(new FeedbackSendEventArgs(content, email));

            Close();
        }

        public event EventHandler<FeedbackSendEventArgs> FeedbackSend;

        protected virtual void OnFeedbackSend(FeedbackSendEventArgs e)
        {
            EventHandler<FeedbackSendEventArgs> handler = FeedbackSend;
            if (handler != null) handler(this, e);
        }
    }
}