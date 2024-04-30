 using Microsoft.Extensions.Configuration;


namespace Core.Models.Configuration
{
    public class AppConfiguration
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);
        private readonly IConfiguration _configuration;

        public APIConfiguration API { get; private set; } = new APIConfiguration();
        public DatabaseConfiguration Database { get; private set; } = new DatabaseConfiguration();
        public SmtpConfiguration Notification { get; private set; } = new();
        public List<FtpConfiguration> FtpConfigs { get; private set; } = new();
        public SmtpConfiguration SmtpConfig { get; private set; } = new();
        public AppConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;

            LoadConfig();
        }

        private void LoadConfig()
        {
            try
            {
                #region API

                #region Info
                var _apiVersion = Environment.GetEnvironmentVariable("API_VERSION");
                if (string.IsNullOrWhiteSpace(_apiVersion))
                    API.Version = _configuration.GetValue<string>("API:Version");
                else
                    API.Version = _apiVersion;

                var _apiTitle = Environment.GetEnvironmentVariable("API_TITLE");
                if (string.IsNullOrWhiteSpace(_apiTitle))
                    API.Title = _configuration.GetValue<string>("API:Title");
                else
                    API.Title = _apiTitle;

                var _apiDescription = Environment.GetEnvironmentVariable("API_DESCRIPTION");
                if (string.IsNullOrWhiteSpace(_apiDescription))
                    API.Description = _configuration.GetValue<string>("API:Description");
                else
                    API.Description = _apiDescription;

                var _apiTermsOfService = Environment.GetEnvironmentVariable("API_TERMS_OF_SERVICE");
                if (string.IsNullOrWhiteSpace(_apiTermsOfService))
                    API.TermsOfService = _configuration.GetValue<string>("API:TermsOfService");
                else
                    API.TermsOfService = _apiTermsOfService;

                #endregion

                #region Contact
                var _apiContactName = Environment.GetEnvironmentVariable("API_CONTACT_NAME");
                if (string.IsNullOrWhiteSpace(_apiContactName))
                    API.Contact.Name = _configuration.GetValue<string>("API:Contact:Name");
                else
                    API.Contact.Name = _apiContactName;

                var _apiContactEmail = Environment.GetEnvironmentVariable("API_CONTACT_EMAIL");
                if (string.IsNullOrWhiteSpace(_apiContactEmail))
                    API.Contact.Email = _configuration.GetValue<string>("API:Contact:Email");
                else
                    API.Contact.Email = _apiContactEmail;

                var _apiContactUrl = Environment.GetEnvironmentVariable("API_CONTACT_URL");
                if (string.IsNullOrWhiteSpace(_apiContactUrl))
                    API.Contact.Url = _configuration.GetValue<string>("API:Contact:Url");
                else
                    API.Contact.Url = _apiContactUrl;
                #endregion

                #region License
                var _apiLicenseName = Environment.GetEnvironmentVariable("API_LICENSE_NAME");
                if (string.IsNullOrWhiteSpace(_apiLicenseName))
                    API.License.Name = _configuration.GetValue<string>("API:License:Name");
                else
                    API.License.Name = _apiLicenseName;

                var _apiLicenseUrl = Environment.GetEnvironmentVariable("API_LICENSE_URL");
                if (string.IsNullOrWhiteSpace(_apiLicenseUrl))
                    API.License.Url = _configuration.GetValue<string>("API:License:Url");
                else
                    API.License.Url = _apiLicenseUrl;
                #endregion

                #region Authentication

                var _apiAuthType = Environment.GetEnvironmentVariable("API_AUTHENTICATION_TYPE");
                if (string.IsNullOrWhiteSpace(_apiAuthType))
                    API.Authentication.Type = Convert.ToInt32(_configuration.GetValue<string>("API:Authentication:Type"));
                else
                    API.Authentication.Type = Convert.ToInt32(_apiAuthType);

                var _apiAuthBasicUsername = Environment.GetEnvironmentVariable("API_AUTHENTICATION_BASIC_USERNAME");
                if (string.IsNullOrWhiteSpace(_apiAuthBasicUsername))
                    API.Authentication.Basic.Username = _configuration.GetValue<string>("API:Authentication:Basic:Username");
                else
                    API.Authentication.Basic.Username = _apiAuthBasicUsername;

                var _apiAuthBasicPassword = Environment.GetEnvironmentVariable("API_AUTHENTICATION_BASIC_PASSWORD");
                if (string.IsNullOrWhiteSpace(_apiAuthBasicPassword))
                    API.Authentication.Basic.Password = _configuration.GetValue<string>("API:Authentication:Basic:Password");
                else
                    API.Authentication.Basic.Password = _apiAuthBasicPassword;

                var _apiAuthDescription = Environment.GetEnvironmentVariable("API_AUTHENTICATION_DESCRIPTION");
                if (string.IsNullOrWhiteSpace(_apiAuthDescription))
                    API.Authentication.Description = _configuration.GetValue<string>("API:Authentication:Description");
                else
                    API.Authentication.Description = _apiAuthDescription;

                #endregion

                #endregion

                #region Database
                var _connectionString = Environment.GetEnvironmentVariable("DB_CONNECTIONSTRING");
                if (string.IsNullOrWhiteSpace(_connectionString))
                    Database.ConnectionString = _configuration.GetConnectionString("App");
                else
                    Database.ConnectionString = _connectionString;

                var _dbQuerySplittingBehavior = Environment.GetEnvironmentVariable("DB_QUERY_SPLITTING_BEHAVIOR");
                if (string.IsNullOrWhiteSpace(_dbQuerySplittingBehavior))
                    Database.QuerySplittingBehavior = Convert.ToInt32(_configuration.GetValue<string>("Database:QuerySplittingBehavior"));
                else
                    Database.QuerySplittingBehavior = Convert.ToInt32(_dbQuerySplittingBehavior);

                var _dbTimeout = Environment.GetEnvironmentVariable("DB_TIMEOUT");
                if (string.IsNullOrWhiteSpace(_dbTimeout))
                    Database.Timeout = Convert.ToInt32(_configuration.GetValue<string>("Database:Timeout"));
                else
                    Database.Timeout = Convert.ToInt32(_dbTimeout);

                var _dbSensitiveDataLogging = Environment.GetEnvironmentVariable("DB_SENSITIVE_DATA_LOGGING");
                if (string.IsNullOrWhiteSpace(_dbSensitiveDataLogging))
                    Database.SensitiveDataLogging = Convert.ToBoolean(_configuration.GetValue<string>("Database:SensitiveDataLogging"));
                else
                    Database.SensitiveDataLogging = Convert.ToBoolean(_dbSensitiveDataLogging);
                #endregion

                
                #region Notification
                    Notification = _configuration.GetSection("Notification").Get<SmtpConfiguration>() ?? new();
                #endregion

                #region SmtpConfig
                SmtpConfig = _configuration.GetSection("SmtpConfig").Get<SmtpConfiguration>() ?? new();
                #endregion

                #region FtpConfiguration
                FtpConfigs = _configuration.GetSection("FtpsGlobalConfig").Get<List<FtpConfiguration>>() ?? new();
                #endregion
            }
            catch (Exception ex)
            {
                Log.Error("LoadConfig error");
                Log.Error(ex);
            }
        }
    }
}
