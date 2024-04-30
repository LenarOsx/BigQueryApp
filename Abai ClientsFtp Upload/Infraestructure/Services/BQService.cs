using Google.Apis.Auth.OAuth2;
using Google.Cloud.BigQuery.V2;
using log4net;

namespace Infraestructure.Services
{
    public class BQService
    {

        private readonly ILogger<BQService> _logger;
        public BQService(ILogger<BQService> logger)
        {
            _logger = logger;
        }
        public List<T> GetData<T>(string projectId, string query, out string error)
        {

            error = string.Empty;
            List<T> result = new List<T>();
            string rootPath = AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
            FileInfo credenciales = new FileInfo($"{rootPath}application_default_credentials.json");
            if (!credenciales.Exists)
            {
                error = "El achivo Json que contiene las credenciales no existe en el directorio principal de la aplicación";
                return result;
            }

            var credentials = GoogleCredential.FromFile(credenciales.FullName).CreateScoped(new List<string>() {
                "https://www.googleapis.com/auth/bigquery",
                "https://www.googleapis.com/auth/drive" });

            var client = BigQueryClient.Create(projectId, credentials);

            try
            {
                BigQueryResults results = client.ExecuteQuery(query, parameters: null);
                List<string> fields = new List<string>();

                foreach (var col in results.Schema.Fields)
                {
                    fields.Add(col.Name);
                }

                Dictionary<string, object> rowoDict;

                foreach (var row in results)
                {
                    rowoDict = new Dictionary<string, object>();
                    foreach (var col in fields)
                    {
                        rowoDict.Add(col, row[col]);
                    }
                    string json = Newtonsoft.Json.JsonConvert.SerializeObject(rowoDict);
                    T o = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
                    result.Add(o);
                }
            }
            catch (Exception ex)
            {
                error = ex.ToString();
                _logger.LogError(error);
                Console.WriteLine(error);
            }
            finally
            {
                client.Dispose();
                _logger.LogError($"Se obtuvieron [{result.Count}] registros");
                Console.WriteLine("Done.");
            }

            return result;

        }
    }
}
