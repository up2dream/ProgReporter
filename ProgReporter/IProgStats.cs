//==============================================================
// ProgReporter
// Copyright � Miroslav Popov. All rights reserved.
//==============================================================
// THIS CODE IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE.
//==============================================================

namespace ProgReporter
{
    public interface IProgStats
    {
        /// <summary>
        ///     Sets the license type of your application.
        ///     Trial, Expired, Valid, NotValid, Unknown
        /// </summary>
        LicenseType AppLicenseType { set; }

        /// <summary>
        ///     Id string for your application license. Up to 40 chars.
        ///     Used only with Valid license type.
        /// </summary>
        string AppLicenseId { set; }

        /// <summary>
        ///     Sets if ProgReporter sends usage statistics like feature clicks, runtime..
        /// </summary>
        bool SendUsageStatistics { set; }

        /// <summary>
        ///     Tells ProgStats that your application is started.
        /// </summary>
        /// <param name="appId">An unique ID fro your app you receive from ProgReporter.com</param>
        void AppStart(string appId);

        /// <summary>
        ///     Tells ProgStats that your application is started.
        /// </summary>
        /// <param name="appId">An unique ID fro your app you receive from ProgReporter.com</param>
        /// <param name="delay">Delay in seconds</param>
        void AppStart(string appId, int delay);

        /// <summary>
        ///     Tells ProgStats that your application is going to be stopped.
        /// </summary>
        void AppStop();

        /// <summary>
        ///     Saves a record that a feature with a specific index was used.
        /// </summary>
        /// <param name="index">Index of the feature.</param>
        void FeatureClick(int index);
    }
}