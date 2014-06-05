using System.Net;

namespace NuFridge.Common.Manager
{
    public class BindingInfo
    {
        public string BindingInformation { get; set; }
        public string Host { get; set; }
        public string Protocol { get; set; }
        public bool IsIpPortHostBinding { get; set; }
        public IPEndPoint EndPoint { get; set; }
        public string GetFriendlyHostName()
        {
            if (string.IsNullOrEmpty(Host) || Host == "*")
            {
                return System.Environment.MachineName;
            }
            return Host;
        }

        public string Url {
            get { return string.Format("{0}://{1}:{2}", Protocol, GetFriendlyHostName(), EndPoint.Port); }
        }

    }
}