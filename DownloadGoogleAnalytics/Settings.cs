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
                Log.Write("Probleem met het lezen van de setting " + key);
            }
            return result;
        }

        public static void set(string _key, string _data)
        {
            try
            {
                //lees de settings in
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                //vraag af of de setting er is, zo niet, dan wordt deze aangemaakt
                if (settings[_key] == null)
                {
                    settings.Add(_key, _data);
                }
                else
                {
                    settings[_key].Value = _data;
                }
                //sla de wijzigingen op
                configFile.Save(ConfigurationSaveMode.Modified);
                //ververs de settings
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
                //schrijf resultaat naar console en log
                Console.WriteLine(_key + " succesvol toegevoegd");
                Log.Write(_key + " succesvol gewijzigd");
            }
            catch (ConfigurationErrorsException cex)
            {
                Log.Write("Er ging iets fout met het wijzigen van de instelling " + cex);
            }
        }

        public static void setWithQuestion(string _key)
        {
            string _data;
            if (_key == "clientsecretjson")
            {
                Console.WriteLine("Geef de locatie op van de client_secret.json:");
                string _location = Console.ReadLine();
                using (StreamReader sr = new StreamReader(_location))
                {
                    _data = sr.ReadToEnd();
                }
            }
            else
            {
                Console.WriteLine("Plak hieronder de instelling voor " + _key);
                _data = Console.ReadLine();
            }
            set(_key, _data);
        }

        public static void MissingSetting(string type)
        {
            try
            {
                Console.WriteLine("De setting voor " + type + " is niet aanwezig");
                Settings.setWithQuestion(type);
            }
            catch
            {
                Log.Write("Er ging iets fout met opslaan van de " + type);
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
                Log.Write("Er ging iets fout met opslaan van de " + type + "/r/n" + ex);
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
