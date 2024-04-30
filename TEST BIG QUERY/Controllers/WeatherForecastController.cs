using Google.Cloud.BigQuery.V2;
using Microsoft.AspNetCore.Mvc;

namespace TEST_BIG_QUERY.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly GenerarInforme _generarInforme;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, GenerarInforme generarInforme)
        {
            _logger = logger;
            _generarInforme = generarInforme;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public bool Get()
        {
            try
            {
                _generarInforme.Generar();
                var client = BigQueryClient.Create("operation-datalake");
                //operation-datalake.Naturgy.Naturgy_Inin_Historico
                //var table = client.GetTable("bigquery-public-data", "samples", "shakespeare");
                //var sql = $"SELECT corpus AS title, COUNT(word) AS unique_words FROM {table} GROUP BY title ORDER BY unique_words DESC LIMIT 10";
                var table = client.GetTable("operation-datalake", "Naturgy", "Naturgy_Inin_Historico");
                var sql = $"SELECT Hora,Entrantes,TMO,Atendidas,Abandonos,0 AS LlamadasDesconectadas," +
                    $"0 AS DesbordamientoIndirecto,IFNULL(ROUND(SAFE_DIVIDE(Atendidas,entrantes),2),0) AS PorcentajeContestadas," +
                    $"IFNULL(ROUND(SAFE_DIVIDE(Abandonos,Entrantes),2),0) AS PorcentajeAbandonadas,0 AS MediaAgentes," +
                    $"IFNULL(ROUND(SAFE_DIVIDE(Atendidas_0_18,Atendidas),2),0) AS NivelAtencion18Seg,Atendidas_0_18" +
                    $" FROM {table} LIMIT 100";

                var results = client.ExecuteQuery(sql, parameters: null);

                foreach (var row in results)
                {
                    var rows = row["Servicio"].GetType();
                    Console.WriteLine($"{row["Servicio"].GetType().Name}");
                    Console.WriteLine($"{row["Servicio"]}: {row["Idioma"]}");
                }
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }
    }
}