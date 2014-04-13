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
        private const string ServiceUrl = @"http://progreporter.com/service";
        private const string LiveUrl = @"http://progreporter.com/service/live";

        private readonly CriptoService cryptoService;
        private readonly int[] featureClicks;
        private readonly IoHelper ioHelper;
        private readonly Timer timer;
        private readonly string userId = string.Empty;
        private readonly WebService webService;
        private DateTime appStartTime;
        private string applicationId;
        private string countryCode;
        private bool isStarted;
        private Thread threadGeoPlugin;
        private Thread threadLive;
        private Thread threadWebClient;

        /// <summary>
        ///     Public constructor.
        /// </summary>
        public ProgStats()
        {
            webService = new WebService();
            cryptoService = new CriptoService();
            ioHelper = new IoHelper();
            featureClicks = new int[10];
            userId = GetUserId();

            AppLicenseType = LicenseType.Unknown;
            countryCode = RegionInfo.CurrentRegion.Name;

            timer = new Timer {AutoReset = true, Interval = 60*1000};
            timer.Elapsed += Timer_Elapsed;
        }

        /// <summary>
        ///     Sets the license type of your application.
        ///     Trial, Expired, Valid, NotValid, Unknown
        /// </summary>
        public LicenseType AppLicenseType { private get; set; }

        /// <summary>
        ///     Id string for your application license. Up to 40 chars.
        ///     Used only with Valid license type.
        /// </summary>
        public string AppLicenseId { private get; set; }

        /// <summary>
        ///     Sets if ProgReporter sends usage statistics like feature clicks, runtime..
        /// </summary>
        public bool SendUsageStatistics { private get; set; }

        /// <summary>
        ///     Tells ProgStats that your application is started.
        /// </summary>
        public void AppStart(string appId)
        {
            applicationId = appId;
            appStartTime = DateTime.UtcNow;
            isStarted = true;

            threadWebClient = new Thread(SendProgStatsFromPreviousRuns);
            threadWebClient.Start();

            threadGeoPlugin = new Thread(ReadGeoPlugin);
            threadGeoPlugin.Start();

            timer.Start();
        }

        /// <summary>
        ///     Tells ProgStats that your application is going to be stopped.
        /// </summary>
        public void AppStop()
        {
            if (!isStarted) return;

            if (timer.Enabled)
                timer.Stop();

            if (threadWebClient != null)
                threadWebClient.Abort();

            if (threadLive != null)
                threadLive.Abort();

            if (threadGeoPlugin != null)
                threadGeoPlugin.Abort();

            string parameters = ComposeParameters();
            string crypto = cryptoService.Encrypt(parameters);
            SaveProgStats(crypto);
        }

        /// <summary>
        ///     Saves a record that a feature with a specific index was used.
        /// </summary>
        /// <param name="index">Index of the feature.</param>
        public void FeatureClick(int index)
        {
            if (index < 0 || index >= featureClicks.Length) return;
            featureClicks[index]++;
        }

        private string GetUserId()
        {
            string mac = string.Empty;
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus != OperationalStatus.Up) continue;
                mac = nic.GetPhysicalAddress().ToString();
                break;
            }
            return mac;
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
            if (workTime < 0) workTime = 5;
            if (workTime > 2*24*60*60)
                workTime = 2*24*60*60;

            return workTime;
        }

        private string GetReportTime()
        {
            return DateTime.UtcNow.ToString("yyyy-MM-dd");
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            threadLive = new Thread(SendLiveStats);
            threadLive.Start();
        }

        private void SendLiveStats()
        {
            string param =
                "app_id=" + applicationId +
                "&user_id=" + userId +
                "&country_code=" + countryCode;

            try
            {
                webService.SendPostRequest(LiveUrl, param);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private string ComposeParameters()
        {
            string parameters =
                "app_id=" + Truncate(applicationId, 40) + // String max 40 chars
                "&user_id=" + Truncate(userId, 40) + // String max 40 chars
                "&country_code=" + Truncate(countryCode, 2) + // String 2 chars: BG, US..
                "&report_time=" + GetReportTime() + // String "yyyy-MM-dd"
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
                string[] files = Directory.GetFiles(filePath, "*.prr");
                foreach (string file in files)
                {
                    string fileName = Path.GetFileNameWithoutExtension(file);
                    if (string.IsNullOrEmpty(fileName)) continue;
                    if (!fileName.StartsWith(Truncate(applicationId, 10))) continue;

                    string parameters = LoadProgStats(file);

                    if (string.IsNullOrEmpty(parameters))
                        ioHelper.DeleteFile(file);

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

        private void ReadGeoPlugin()
        {
            try
            {
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
        }

        private string SendStats(string url, string parameters)
        {
            try
            {
                return webService.SendPostRequest(url, parameters);
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
                if (!Directory.Exists(dirPath))
                    Directory.CreateDirectory(dirPath);
                string fileName = string.Format("{0}_{1:yyyyMMddHHmmss}.prr",
                    Truncate(applicationId, 10), DateTime.Now);
                string filePath = Path.Combine(dirPath, fileName);
                using (var fs = new FileStream(filePath, FileMode.Create))
                using (TextWriter writer = new StreamWriter(fs))
                    writer.Write(report);
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
                string cripto = File.ReadAllText(file);
                return cryptoService.Decrypt(cripto);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return null;
        }

        private string Truncate(string value, int maxLength)
        {
            if (!string.IsNullOrEmpty(value) && value.Length > maxLength)
                return value.Substring(0, maxLength);
            return value;
        }
    }
}