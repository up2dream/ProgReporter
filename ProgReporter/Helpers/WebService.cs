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
using System.Net;
using System.Text;

namespace ProgReporter.Helpers
{
    internal class WebService
    {
        internal WebService()
        {
            ServicePointManager.Expect100Continue = false;
        }

        internal string SendPostRequest(string url, string parameters)
        {
            try
            {
                var client = new WebClient();
                client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                byte[] paramBytes = Encoding.UTF8.GetBytes(parameters);
                byte[] respondBytes = client.UploadData(url, "POST", paramBytes);
                return Encoding.UTF8.GetString(respondBytes);
            }
            catch (WebException e)
            {
                Console.WriteLine(e.Message);
                return string.Empty;
            }
        }

        internal string GetWebData(string url)
        {
            try
            {
                var client = new WebClient();
                return client.DownloadString(url);
            }
            catch (WebException e)
            {
                Console.WriteLine(e.Message);
                return string.Empty;
            }
        }
    }
}