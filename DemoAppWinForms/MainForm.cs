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
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using ProgReporter;

namespace DemoAppWinForms
{
    public partial class MainForm : Form
    {
        private string filePath = string.Empty;
        private ProgStats stats;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            stats = new ProgStats();

            // If your application uses licensing, you can set the license type of your app.
            // Available license types are: Free, Trial, Expired, Valid, NotValid, Unknown
            stats.AppLicenseType = LicenseType.Free;

            // Sets application version.
            stats.AppVersion = Application.ProductVersion;

            // Begin proceding stats
            // Sets the particular application Id and optionally a starting delay in seconds.
            stats.AppStart("cbc15a23946e34067c0085b2087ac33bf221a7d5", 3);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Tells ProgReporter that you are going to stop your application
            stats.AppStop();
        }

        private void ItmOpen_Click(object sender, EventArgs e)
        {
            // Count "Open" usages as a feature with index 0
            stats.FeatureClick(0);

            filePath = GetOpenPath();
            if (filePath == string.Empty)
                return;

            using (var stream = new FileStream(filePath, FileMode.Open))
            using (TextReader reader = new StreamReader(stream))
                textBox.Text = reader.ReadToEnd();
        }

        private void ItmSave_Click(object sender, EventArgs e)
        {
            // Count "Save" usages as a feature with index 1
            stats.FeatureClick(1);

            if (filePath == string.Empty)
            {
                filePath = GetSaveAsPath();
                if (filePath == string.Empty)
                    return;
            }

            SaveTextToFile();
        }

        private void ItmSaveAs_Click(object sender, EventArgs e)
        {
            // Count "Save As" usages as a feature with index 2
            stats.FeatureClick(2);

            filePath = GetSaveAsPath();
            if (filePath == string.Empty)
                return;

            SaveTextToFile();
        }

        private void OnlineHelp_Click(object sender, EventArgs e)
        {
            // Count "Help" usages as a feature with index 3
            stats.FeatureClick(3);

            try
            {
                Process.Start(@"http://progreporter.com/docs");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        private void SaveTextToFile()
        {
            using (var stream = new FileStream(filePath, FileMode.Create))
            using (TextWriter writer = new StreamWriter(stream))
                writer.Write(textBox.Text);
        }

        private string GetSaveAsPath()
        {
            var saveFileDialog = new SaveFileDialog
            {
                InitialDirectory = filePath,
                FileName = Path.GetFileName(filePath),
                AddExtension = true,
                Title = "Save as...",
                Filter = "Text file (*.txt)|*.txt"
            };

            return saveFileDialog.ShowDialog() == DialogResult.OK
                ? saveFileDialog.FileName
                : string.Empty;
        }

        private string GetOpenPath()
        {
            var saveFileDialog = new OpenFileDialog
            {
                InitialDirectory = filePath,
                FileName = Path.GetFileName(filePath),
                AddExtension = true,
                Title = "Open file...",
                Filter = "Text file (*.txt)|*.txt"
            };

            return saveFileDialog.ShowDialog() == DialogResult.OK
                ? saveFileDialog.FileName
                : string.Empty;
        }
    }
}