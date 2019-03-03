using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;

namespace DownloadGoogleAnalytics
{
    class Settings
    {
        public string[] AllSettings = { "clientsecretjson", "viewid", "email", "locationcsv" , "logfolder" };
        public static string read(string key)
        {
            string result = "";
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                result = appSettings[key] ?? "Not Found";
            }
            catch (ConfigurationErrorsException)
            {
                Log.Write("Problem with reading setting " + key);
            }
            return result;
        }

        public static void set(string _key, string _data)
        {
            try
            {
                //read settings
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                //determine if setting exists or should be added
                if (settings[_key] == null)
                {
                    settings.Add(_key, _data);
                }
                else
                {
                    settings[_key].Value = _data;
                }
                //save changes
                configFile.Save(ConfigurationSaveMode.Modified);
                //refresh settings
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
                //write results to console and log
                Console.WriteLine(_key + " successfull added");
                Log.Write(_key + " successfully edited");
            }
            catch (ConfigurationErrorsException cex)
            {
                Log.Write("Somethihng went wrong editing setting " + cex);
            }
        }

        public static void setWithQuestion(string _key)
        {
            string _data;
            if (_key == "clientsecretjson")
            {
                Console.WriteLine("Enter the location of the client_secret.json file:");
                string _location = Console.ReadLine();
                using (StreamReader sr = new StreamReader(_location))
                {
                    _data = sr.ReadToEnd();
                }
            }
            else
            {
                Console.WriteLine("Paste de settings for " + _key);
                _data = Console.ReadLine();
            }
            set(_key, _data);
        }

        public static void MissingSetting(string type)
        {
            try
            {
                Console.WriteLine("The setting for " + type + " does not exist");
                Settings.setWithQuestion(type);
            }
            catch
            {
                Log.Write("Something went wrong with saving " + type);
            }
        }
        public static void SetSetting(string type)
        {
            try
            {
                Settings.setWithQuestion(type);
            }
            catch (Exception ex)
            {
                Log.Write("Something went wrong with saving " + type + "/r/n" + ex);
            }
        }
        public static void CheckSettings()
        {
            Settings _settings = new Settings();
            foreach (string _setting in _settings.AllSettings)
            {
                if (Settings.read(_setting) == "")
                {
                    MissingSetting(_setting);
                }
            }
        }
    }
}
