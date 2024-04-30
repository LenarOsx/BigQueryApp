using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.Configuration
{
    public class FtpConfiguration
    {
        public string Name { get; set; } = "";
        public string Host { get; set; } = "";
        public string PublicHost { get; set; } = "";
        public int Port { get; set; } = 0;
        public string User { get; set; } = "";
        public string Password { get; set; } = "";
        public string Fingerprint { get; set; } = "";
        public int TryOpenSession { get; set; } = 5;
    }
}
