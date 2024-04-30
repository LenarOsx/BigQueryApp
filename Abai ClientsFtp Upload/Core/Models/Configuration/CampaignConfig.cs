

namespace Core.Models.Configuration
{
    public class CampaignConfig
    {
        public string CampaignName { get; set; } = string.Empty;
        public string CargaLeadsServerUrl { get; set; } = string.Empty;
        public string ConnectionString { get; set; } = string.Empty;
        public string StoreProcedure { get; set; } = string.Empty;
        public List<Mail> MailsTo { get; set; } = new();
        public Mail MailFrom { get; set; } = new() { MailAddress = "no-reply@abaigroup.com" };
        public bool DeleteLeadAfterLoad { get; set; }
        public string Vcc { get; set; } = string.Empty;
        public ContactConfig ContactConfig { get; set; } = new();
        public PhoneConfig PhoneConfig { get; set; } = new();
    }

    public class ContactConfig
    {
        public string IsVip { get; set; } = string.Empty;
        public string IsClient { get; set; } = string.Empty;
        public string ContactPriority { get; set; } = string.Empty;
        public string TimeZoneId { get; set; } = string.Empty;
        public string AgentId { get; set; } = string.Empty;
    }
    public class PhoneConfig
    {
        public string CountryId { get; set; } = string.Empty;
        public string Zip { get; set; } = string.Empty;
        public string Extension { get; set; } = string.Empty;
        public string ImportName { get; set; } = string.Empty;
    }

    public class Mail
    {
        public string MailAddress { get; set; }=string.Empty;
        public bool Enabled { get; set; }
    }
}
