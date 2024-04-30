using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.Configuration
{
    public class ReportConfiguration
    {
        public int Order { get; set; }
        public bool Enabled { get; set; }
        public bool CountData { get; set; } = false;
        public string Name { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string TemplatePath { get; set; } = string.Empty;
        public string SavingPath { get; set; } = string.Empty;
        public ProcedureConfiguration MailProcedureConfig { get; set; } = new ProcedureConfiguration();
        public ReportMailConfiguration MailSmtpConfig { get; set; } = new ReportMailConfiguration();
        public List<ReportFtpConfiguration> FtpConfigs { get; set; } = new List<ReportFtpConfiguration>();
        public List<ReportExcelConfiguration> ExcelConfigs { get; set; } = new List<ReportExcelConfiguration>();
    }
    public class ProcedureConfiguration
    {
        public string Name { get; set; } = string.Empty;
        public string Procedure { get; set; } = string.Empty;
        public bool Enabled { get; set; }
        public Dictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();
    }
    public class ReportFtpConfiguration
    {
        public int Order { get; set; }
        public string FtpConnectionName { get; set; } = string.Empty;
        public string FtpPath { get; set; } = string.Empty;
        public string LocalPath { get; set; } = string.Empty;
        public string ContainsString { get; set; } = string.Empty;
        public bool IsUpload { get; set; }
        public bool DeleteFilesAfter { get; set; }
        public bool Enabled { get; set; }
        public bool CreateDirectory { get; set; }
    }
    public class ReportExcelConfiguration
    {
        public int Order { get; set; }
        public string WorksheetName { get; set; } = string.Empty;
        public string ClassToMap { get; set; } = string.Empty;
        public int WorksheetNumber { get; set; }
        public int StartRowNumber { get; set; }
        public int StartColumnNumber { get; set; }
        public bool IsInsert { get; set; }
        public List<ReportExcelWorksheetSectionConfiguration> WorksheetSections { get; set; } = new List<ReportExcelWorksheetSectionConfiguration>();
        public ProcedureConfiguration ProcedureConfig { get; set; } = new ProcedureConfiguration();
        public ReportExcelConfiguration AddAccumulated { get; set; }
    }

    public class ReportExcelWorksheetSectionConfiguration
    {
        public int StartColumnNumber { get; set; }
        public int DataStartIndex { get; set; }
        public int DataEndIndex { get; set; }
    }

    public class ReportMailConfiguration
    {
        public bool Enabled { get; set; }
        public string ContainsString { get; set; } = "yyyyMMdd";
        public string Subject { get; set; } = "Informes";
        public string Message { get; set; } = string.Empty;
        public List<MailConfiguration> MailsTo { get; set; } = new List<MailConfiguration>();
        public MailConfiguration MailFrom { get; set; } = new MailConfiguration() { MailAddress = "no-reply@abaigroup.com" };
    }
    public class MailConfiguration
    {
        public string MailAddress { get; set; } = string.Empty;
        public bool Enabled { get; set; }
    }
}
