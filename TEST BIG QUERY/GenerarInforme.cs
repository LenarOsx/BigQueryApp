using TEST_BIG_QUERY.Controllers;
using TEST_BIG_QUERY.Models;

namespace TEST_BIG_QUERY
{
    public class GenerarInforme
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly BQService _bqService;
        public GenerarInforme(BQService bqService, ILogger<WeatherForecastController> logger) {
            _bqService = bqService;
            _logger = logger;
        }
       public bool Generar() {
            try
            {
                var datos = _bqService.Execute<NaturgyIninEntity>("Naturgy", "Naturgy_Inin_Historico",0);
                return true;
            }
            catch (Exception ex)
            {
                return false;

            }
        }
    }
}