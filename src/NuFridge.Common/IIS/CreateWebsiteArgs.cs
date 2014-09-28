using System;

namespace NuFridge.Common.Manager
{
    public class CreateWebsiteArgs : EventArgs
    {
        public CreateWebsiteArgs(string physicalPath)
        {
            this.PhysicalPath = physicalPath;
            this.Protocol = HttpProtocol.Http;
            this.PortNumber = 80;
            this.HostName = "*";
        }

        public HttpProtocol Protocol { get; set; }
        public int PortNumber { get; set; }
        public string HostName { get; set; }
        public string PhysicalPath { get; set; }

        public string GetBindingInformationString()
        {
            var host = string.IsNullOrEmpty(this.HostName) ? "*" : this.HostName;
            return string.Format("{0}:{1}:", host, this.PortNumber);
        }
    }
}