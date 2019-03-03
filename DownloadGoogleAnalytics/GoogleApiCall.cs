using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Discovery;
using Google.Apis.Services;
using Google.Apis;
using Google.Apis.Oauth2.v2;
using Google.Apis.Oauth2.v2.Data;
using Google.Apis.Auth.OAuth2;
using System.IO;
using System.Threading;
using Google.Apis.Util.Store;
using Google.Apis.AnalyticsReporting.v4;
using Google.Apis.AnalyticsReporting.v4.Data;
using ServiceStack.Text;
using System.Collections;
using System.Configuration;
using LINQtoCSV;

namespace DownloadGoogleAnalytics
{
    
    public class DateTimeEnumerator : System.Collections.IEnumerable
    {
        private DateTime begin;
        private DateTime end;

        public DateTimeEnumerator(DateTime begin, DateTime end)
        {
            this.begin = begin;
            this.end = end;
        }

        public System.Collections.IEnumerator GetEnumerator()
        {
            for (DateTime date = begin; date < end; date = date.AddDays(1))
            {
                yield return date;
            }
        }
    }

    class GoogleApiCall
    {
        public void GetAnalyticsData()
        {
            try
            {
                DateTime _startDate;
                // vangt een lege lastget datum af
                if (Settings.read("lastget") == "")
                {
                    _startDate = DateTime.Parse("2018-01-01");
                }
                else
                {
                    _startDate = DateTime.Parse(Settings.read("lastget"));
                }
                DateTime _endDate = DateTime.Today.AddDays(-1);
                Log.Write("Getting data from " + _startDate.ToString("yyyy-MM-dd") + " to " + _endDate.ToString("yyyy-MM-dd"));
                DateTimeEnumerator dates = new DateTimeEnumerator(_startDate, _endDate);
                foreach (DateTime date in dates)
                {
                    GetAnalyticsDataWithDate(date);
                }
                Log.Write("Setting lastget to last date: " + DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd"));
                Settings.set("lastget", DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd"));
                Log.Write("Ready :)");
            }
            catch(Exception ex)
            {
                Log.Write(ex.ToString());
            }
        }

        public void GetAnalyticsDataWithDate(DateTime date)
        {
            try
            {
                Log.Write("Ophalen Google Analytics data for date " + date.ToString("yyyy-MM-dd"));
                var credential = GetCredential().Result;
                using (var svc = new AnalyticsReportingService(
                    new BaseClientService.Initializer
                    {
                        HttpClientInitializer = credential,
                        ApplicationName = "GetAnalyticsData"
                    }))
                {
                    var reportRequest = CreateReportRequest(date);
                    var getReportsRequest = new GetReportsRequest
                    {
                        ReportRequests = new List<ReportRequest> { reportRequest }
                    };
                    Log.Write("Starting request");
                    var batchRequest = svc.Reports.BatchGet(getReportsRequest);
                    var response = batchRequest.Execute();
                    Log.Write("Processing data");
                    var Results = new Results();
                    Results.ResultList = new List<Result>();
                    foreach(var x in response.Reports.First().Data.Rows)
                    {
                        Result _result = new Result();
                        _result.dDate = x.Dimensions[0];
                        _result.dsourceMedium = x.Dimensions[1];
                        _result.ddayOfWeek = x.Dimensions[2];
                        _result.dexitPagePath = x.Dimensions[4];
                        _result.dcustomVarValue2 = x.Dimensions[3];
                        _result.mSessions = x.Metrics.First().Values[0];
                        _result.msessionDuration = x.Metrics.First().Values[1];
                        _result.mpageViewsPerSession = x.Metrics.First().Values[2];
                        _result.msessionsPerUser = x.Metrics.First().Values[3];
                        Results.ResultList.Add(_result);
                    }
                    SaveCSV(Results);
                }
            }
            catch (NullReferenceException nex)
            {
                Log.Write("No data fetched\r\n" + nex);
            }
            catch (Exception ex)
            {
                Log.Write(ex.ToString());
            }
        }

        private void SaveCSV(Results _results)
        {
            try
            {
                string _newfilename = Path.GetFullPath(Settings.read("locationcsv")) + DateTime.Now.ToString("yyyy-MM-ddThhmmss") + ".log";
                if (File.Exists(Settings.read("locationcsv")))
                {
                    Log.Write("CSV already exists, importing existing csv " + Settings.read("locationcsv"));
                    CsvFileDescription inputfileDescription = new CsvFileDescription
                    {
                        SeparatorChar = ',',
                        FirstLineHasColumnNames = true
                    };
                    CsvContext cc = new CsvContext();
                    IEnumerable<Result> _oldcsv = cc.Read<Result>(Settings.read("locationcsv"), inputfileDescription);
                    Log.Write("Existing csv imported, adding old data to new data");
                    foreach (Result _result in _oldcsv)
                    {
                        _results.ResultList.Add(_result);
                    }
                    Log.Write("Rename old csv to " + _newfilename);
                    File.Copy(Settings.read("locationcsv"), _newfilename);
                }


                string csv = CsvSerializer.SerializeToCsv(_results);
                Log.Write("Writing CSV");
                System.IO.File.WriteAllText(Settings.read("locationcsv"), csv);
                Log.Write("Deleting temporary csv from location " + _newfilename);
                if (System.IO.File.Exists(_newfilename)) { System.IO.File.Delete(_newfilename); }
            }
            catch (Exception ex)
            {
                Log.Write("Something went wrong with saving the csv: " + ex);
            }
        }

        private ReportRequest CreateReportRequest(DateTime date)
        {
            
            var appSettings = ConfigurationManager.AppSettings;
            var dateRange = new DateRange { StartDate = date.ToString("yyyy-MM-dd"), EndDate = date.ToString("yyyy-MM-dd") };
            var mSessions = new Metric { Expression = "ga:sessions", Alias = "Sessions" };
            var msessionDuration = new Metric { Expression = "ga:sessionDuration", Alias = "sessionDuration" };
            var mpageviewsPerSession = new Metric { Expression = "ga:pageviewsPerSession", Alias = "pageviewsPerSession" };
            var msessionsPerUser = new Metric { Expression = "ga:sessionsPerUser", Alias = "sessionsPerUser" };
            var dDate = new Dimension { Name = "ga:date" };
            var dsourceMedium = new Dimension { Name = "ga:sourceMedium" };
            var ddayOfWeek = new Dimension { Name = "ga:dayOfWeek" };
            var dexitPagePath = new Dimension { Name = "ga:exitPagePath" };
            var dcustomVarValue2 = new Dimension { Name = "ga:customVarValue2" };
            var reportRequest = new ReportRequest
            {
                DateRanges = new List<DateRange> { dateRange },
                Dimensions = new List<Dimension> { dDate, dsourceMedium, ddayOfWeek, dcustomVarValue2, dexitPagePath },
                Metrics = new List<Metric> { mSessions, msessionDuration, mpageviewsPerSession, msessionsPerUser },
                //ViewID find here: https://ga-dev-tools.appspot.com/account-explorer/
                ViewId = Settings.read("viewid"),
                PageSize = 10000
            };
            return reportRequest;
        }

        static async Task<UserCredential> GetCredential()
        {
            //using (var stream = new FileStream("client_secret.json",
            //FileMode.Open, FileAccess.Read))
            byte[] byteArray = Encoding.UTF8.GetBytes(Settings.read("clientsecretjson"));
            using (var stream = new MemoryStream(byteArray))
            {
                string loginEmailAddress = Settings.read("email");
                return await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    new[] { AnalyticsReportingService.Scope.Analytics },
                    loginEmailAddress, CancellationToken.None,
                    new FileDataStore("GoogleAnalyticsApiConsole"));
            }
        }
    }
}
