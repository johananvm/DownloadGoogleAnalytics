using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Reflection;

namespace DownloadGoogleAnalytics
{
    class Program
    {
        public static bool stopProcessor = false;
        public static bool changeSettings = false;
 
        static void Main(string[] args)
        {
            try
            {
                //EncryptDecrypt.Decrypt();
                Log.Write("Check for missing settings");
                Settings.CheckSettings();
                Log.Write("Starten met analytics data ophalen");
                new GoogleApiCall().GetAnalyticsData();

            }
            catch (Exception ex)
            {
                Log.Write("Unknown exception: " + ex);
            }
        }
    }
}
