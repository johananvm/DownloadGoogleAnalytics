using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;
using System.Security.Cryptography;
using System.Configuration;

namespace DownloadGoogleAnalytics
{
    class EncryptDecrypt
    {
        public static void Encrypt()
        {
            string appName = "DownloadGoogleAnalytics.exe";
            Configuration config = ConfigurationManager.OpenExeConfiguration(appName);
            var section = (AppSettingsSection)config.GetSection("appSettings");
            if (!section.SectionInformation.IsProtected)
            {
                section.SectionInformation.ProtectSection("DataProtectionConfigurationProvider");
            }
            config.Save();
        }

        public static void Decrypt()
        {
            string appName = "DownloadGoogleAnalytics.exe";
            Configuration config = ConfigurationManager.OpenExeConfiguration(appName);
            var section = (AppSettingsSection)config.GetSection("appSettings");
            if (section.SectionInformation.IsProtected)
            {
                section.SectionInformation.UnprotectSection();
            }
            config.Save();
        }

    }
}
