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
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;

namespace ProgReporter.Helpers
{
    internal class IoHelper
    {
        internal string AppDataFolder()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        }

        internal void DeleteFile(string file)
        {
            try
            {
                File.Delete(file);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public bool DirectoryExists(string dirPath)
        {
            try
            {
                return Directory.Exists(dirPath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public void CreateDirectory(string dirPath)
        {
            try
            {
               Directory.CreateDirectory(dirPath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void WriteTextFile(string path, string contents)
        {
            try
            {
                File.WriteAllText(path, contents);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public string ReadTextFile(string path)
        {
            try
            {
                return File.ReadAllText(path);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return string.Empty;
            }
        }

        public IEnumerable<NetworkInterface> GetAllNetworkInterfaces()
        {
            try
            {
                return NetworkInterface.GetAllNetworkInterfaces();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public IEnumerable<string> GetFiles(string path, string searchPattern)
        {
            try
            {
                return Directory.GetFiles(path, searchPattern);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
    }
}