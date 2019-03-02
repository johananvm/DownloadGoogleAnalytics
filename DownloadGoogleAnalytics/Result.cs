using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloadGoogleAnalytics
{
    public class Result
    {
        public string dDate { get; set; }
        public string dsourceMedium { get; set; }
        public string ddayOfWeek { get; set; }
        public string dexitPagePath { get; set; }
        public string dcustomVarValue2 { get; set; }
        public string mSessions { get; set; }
        public string msessionDuration { get; set; }
        public string mpageViewsPerSession { get; set; }
        public string msessionsPerUser { get; set;}
    }

    public class Results : IEnumerable<Result>
    {
        public List<Result> ResultList { get; set; }

        public IEnumerator<Result> GetEnumerator()
        {
            return ResultList.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }

}
