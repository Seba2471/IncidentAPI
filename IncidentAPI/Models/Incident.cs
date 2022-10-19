using AngleSharp.Dom;
using System.Text.RegularExpressions;

namespace IncidentAPI.Models
{
    public class Incident
    {

        public string Date { get; set; } = "";
        public string Time { get; set; } = "";
        public string Ip { get; set; } = "";
        public string Port { get; set; } = "";


        public Incident(IElement element)
        {
            var time = element.QuerySelector("time");

            if (time is not null)
            {
                var dateTimeString = time.GetAttribute("datetime");
                if (dateTimeString is not null)
                {
                    var datetime = DateTime.Parse(dateTimeString);

                    datetime.AddHours(4);

                    Time = datetime.ToString("HH:mm");
                    Date = datetime.ToString("yyyyMMdd");
                }
            }

            var remoteConnection = element.QuerySelector("pre");

            if (remoteConnection is not null)
            {
                Regex rxRemote = new(@"(Remote connection: (\[)\d*.\d*.\d*.\d*:\d*])");
                MatchCollection mxRemote = rxRemote.Matches(remoteConnection.InnerHtml);


                Regex ipRx = new(@"([0-9]{1,3}\.){3}[0-9]{1,3}");
                if (mxRemote.Count == 1)
                {
                    MatchCollection mxIp = ipRx.Matches(mxRemote[0].Value);
                    if (mxIp.Count == 1)
                    {
                        Ip = mxIp[0].Value;
                    }

                    Regex portRx = new(@":[0-9][0-9]*");

                    MatchCollection mxPort = portRx.Matches(mxRemote[0].Value);
                    if (mxPort.Count == 1)
                    {
                        Port = mxPort[0].Value[1..mxPort[0].Length];
                    }
                }

            }
        }


        public static bool IsComplete(Incident node)
        {
            if (node.Ip == "" || node.Port == "")
            {
                return false;
            }
            return true;
        }

        public static bool IsInLast14Days(Incident node)
        {
            var now = DateTime.UtcNow;

            DateOnly nodeDate = DateOnly.ParseExact(node.Date, "yyyyMMdd");

            DateOnly nodeDateWithAddedDays = nodeDate.AddDays(14);

            if (nodeDateWithAddedDays >= DateOnly.FromDateTime(now))
            {
                return true;
            }

            return false;
        }

    }
}
