
namespace Core.Models.Configuration
{
    public class SmtpConfiguration
    {
        public bool Enabled { get; set; } = false;
        public string User { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; } = 0;
        public bool Ssl { get; set; } = false;
    }
}
