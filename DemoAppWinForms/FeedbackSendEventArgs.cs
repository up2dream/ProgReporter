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

namespace DemoAppWinForms
{
    public class FeedbackSendEventArgs : EventArgs
    {
        public FeedbackSendEventArgs(string content, string email)
        {
            Content = content;
            Email = email;
        }

        public string Content { get; set; }
        public string Email { get; set; }
    }
}