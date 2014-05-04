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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Timers;
using ProgReporter.Helpers;
using Timer = System.Timers.Timer;

namespace ProgReporter
{
    public class ProgStats : IProgStats
    {
        private const string ServiceUrl = @"http://progreporter.com/stats.php";

        private readonly CriptoService cryptoService;
        private readonly int[] featureClicks;
        private readonly IoHelper ioHelper;
        private readonly Thread threadGeoPlugin;
        private readonly Thread threadSendStats;
        private readonly Timer timer;
        private readonly string userId = string.Empty;
        private readonly WebService webService;
        private DateTime appStartTime;
        private string applicationId;
        private string countryCode;
        private bool isStarted;
        private bool isStoping;
        private int startDelay;
        private Thread threadLive;

        public ProgStats()
        {
            webService = new WebService();
            cryptoService = new CriptoService();
            ioHelper = new IoHelper();
            featureClicks = new int[10];
            userId = GetUserId();
            countryCode = RegionInfo.CurrentRegion.Name;
            threadGeoPlugin = new Thread(ReadGeoPlugin);
            threadSendStats = new Thread(SendProgStatsFromPreviousRuns);
            timer = new Timer {AutoReset = true, Interval = 12*60*60*1000};
            timer.Elapsed += Timer_Elapsed;

            AppLicenseType = LicenseType.Unknown;
            AppLicenseId = userId;
            SendUsageStatistics = true;
        }

        public LicenseType AppLicenseType { private get; set; }

        public string AppLicenseId { private get; set; }

        public bool SendUsageStatistics { private get; set; }

        public void AppStart(string appId)
        {
            AppStart(appId, 0);
        }

        public void AppStart(string appId, int delay)
        {
            startDelay = delay > 0 ? delay : 0;
            applicationId = appId;
            appStartTime = DateTime.UtcNow;
            isStarted = true;

            threadGeoPlugin.Start();
            timer.Start();
        }

        public void AppStop()
        {
            isStoping = true;
            if (!isStarted) return;

            if (timer.Enabled)
                timer.Stop();

            if (threadSendStats != null)
                threadSendStats.Abort();

            if (threadLive != null)
                threadLive.Abort();

            if (threadGeoPlugin != null)
                threadGeoPlugin.Abort();

            string parameters = ComposeParameters();
            string crypto = cryptoService.Encrypt(parameters);
            SaveProgStats(crypto);
        }

        public void FeatureClick(int index)
        {
            if (index < 0 || index >= featureClicks.Length) return;
            featureClicks[index]++;
        }

        private string GetUserId()
        {
            string user = string.Empty;

            IEnumerable<NetworkInterface> netInterfaces = ioHelper.GetAllNetworkInterfaces();
            if (netInterfaces != null)
                foreach (NetworkInterface nic in netInterfaces)
                {
                    if (nic.OperationalStatus != OperationalStatus.Up) continue;
                    user = nic.GetPhysicalAddress().ToString();
                    break;
                }

            if (string.IsNullOrEmpty(user))
            {
                string machineName = Environment.MachineName;
                string userName = Environment.UserName;
                string osVersion = Environment.OSVersion.ToString();
                string processor = Environment.ProcessorCount.ToString(CultureInfo.InvariantCulture);
                user = cryptoService.Encrypt(machineName + userName + osVersion + processor);
            }

            user = Truncate(user, 30);
            return user;
        }

        private string GetFeatureClicks()
        {
            var sb = new StringBuilder();
            for (int i = 0; i < featureClicks.Length; i++)
                sb.Append("&feature" + i + "_click=" + featureClicks[i]);
            return sb.ToString();
        }

        private int GetRuntime()
        {
            var workTime = (int) (DateTime.UtcNow - appStartTime).TotalSeconds;
            if (workTime < 0)
                workTime = 5; // Work time is minimum 5 seconds
            if (workTime > 2*7*24*60*60)
                workTime = 2*7*24*60*60; // and maximum 2 weeks
            return workTime;
        }

        private string GetReportDate()
        {
            return DateTime.UtcNow.ToString("yyyy-MM-dd");
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (isStoping) return;
            threadLive = new Thread(SendLiveStats);
            threadLive.Start();
        }

        private void SendLiveStats()
        {
            string param =
                "app_id=" + applicationId +
                "&app_run=0" +
                "&user_id=" + userId +
                "&country_code=" + countryCode;

            SendStats(ServiceUrl, param);
        }

        private string ComposeParameters()
        {
            string parameters =
                "app_id=" + Truncate(applicationId, 40) + // String max 40 chars
                "&app_run=1" +
                "&user_id=" + Truncate(userId, 40) + // String max 40 chars
                "&country_code=" + Truncate(countryCode, 2) + // String 2 chars: BG, US..
                "&report_time=" + GetReportDate() + // String "yyyy-MM-dd"
                "&license_id=" + Truncate(AppLicenseId, 32) +
                "&license_type=" + AppLicenseType + // Trial, Expired, Valid, NotValid, Unknown
                "&stats_on=" + (SendUsageStatistics ? 1 : 0); // Integer: 0 or 1

            if (SendUsageStatistics)
                parameters +=
                    "&app_runtime=" + GetRuntime() + // Integer
                    GetFeatureClicks();

            return parameters;
        }

        private void SendProgStatsFromPreviousRuns()
        {
            try
            {
                string filePath = Path.Combine(ioHelper.AppDataFolder(), "ProgReporter");
                string[] files = ioHelper.GetFiles(filePath, "*.prr");
                if (files == null || files.Length == 0)
                {
                    SendInitialStats();
                    return;
                }

                foreach (string file in files)
                {
                    string fileName = Path.GetFileNameWithoutExtension(file);
                    if (string.IsNullOrEmpty(fileName)) continue;
                    if (!fileName.StartsWith(Truncate(applicationId, 10))) continue;

                    string parameters = LoadProgStats(file);

                    if (string.IsNullOrEmpty(parameters))
                    {
                        ioHelper.DeleteFile(file);
                        return;
                    }

                    string respond = SendStats(ServiceUrl, parameters).ToLower();

                    if (respond == "ok")
                        ioHelper.DeleteFile(file);

                    Thread.Sleep(1000);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void SendInitialStats()
        {
            try
            {
                string parameters = ComposeParameters();
                SendStats(ServiceUrl, parameters);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void ReadGeoPlugin()
        {
            try
            {
                Thread.Sleep(startDelay*1000);
                const string geoUrl = @"http://www.geoplugin.net/php.gp";
                string input = webService.GetWebData(geoUrl);
                var deserializer = new PhpDeserialization();
                var ht = (Hashtable) deserializer.Deserialize(input);
                string country = ht["geoplugin_countryCode"].ToString();

                if (country.Length == 2)
                    countryCode = country;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                threadSendStats.Start();
            }
        }

        private string SendStats(string url, string parameters)
        {
            try
            {
                return webService.SendPostRequest(url, parameters);
                //return webService.GetWebData(url + "?" + parameters);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return string.Empty;
            }
        }

        private void SaveProgStats(string report)
        {
            try
            {
                string dirPath = Path.Combine(ioHelper.AppDataFolder(), "ProgReporter");
                if (!ioHelper.DirectoryExists(dirPath))
                    ioHelper.CreateDirectory(dirPath);
                string fileName = string.Format("{0}_{1:yyyyMMddHHmmss}.prr",
                    Truncate(applicationId, 10), DateTime.Now);
                string filePath = Path.Combine(dirPath, fileName);
                ioHelper.WriteTextFile(filePath, report);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private string LoadProgStats(string file)
        {
            try
            {
                string crypto = ioHelper.ReadTextFile(file);
                if (string.IsNullOrEmpty(crypto))
                    return null;
                return cryptoService.Decrypt(crypto);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        private string Truncate(string value, int maxLength)
        {
            if (!string.IsNullOrEmpty(value) && value.Length > maxLength)
                return value.Substring(0, maxLength);
            return value;
        }
    }
}