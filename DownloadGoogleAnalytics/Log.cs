using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DownloadGoogleAnalytics
{
    class Log
    {
        public static void Write(string text)
        {
            try
            {
                string _locationLog = Path.Combine(Settings.read("logfolder"), "GoogleAnalyticsDownload.log");
                string _today = DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss");
                string _output = _today + " - " + text;

                if (!File.Exists(_locationLog))
                {
                    using (StreamWriter sw = File.CreateText(_locationLog))
                    {
                        sw.WriteLine(_output);
                    }
                }
                else
                {
                    using (StreamWriter sw = File.AppendText(_locationLog))
                    {
                        sw.WriteLine(_output);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.ReadLine();
            }
        }
    }
}
