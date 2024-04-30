using OfficeOpenXml;


namespace Infraestructure.Services
{
    public class EPPlusService
    {
        private readonly ILogger<EPPlusService> _logger;
        public EPPlusService(ILogger<EPPlusService> logger)
        {
            _logger = logger;
        }

        public int ContarFilas(string excelFileName, int headersCount = 0)
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                ExcelPackage excelPackage = new ExcelPackage(excelFileName);
                ExcelWorksheet printingWorkSheet = excelPackage.Workbook.Worksheets[0];
                var count = printingWorkSheet.Dimension.End.Row;
                count -= headersCount;
                _logger.LogInformation($"El archivo  [{excelFileName}] tiene un total de [{count}] registros");
                return count;

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrió un error al intentar contal los registros del archivo [{excelFileName}], excepción: " + ex);
                return 0;
            }
        }
    }
}
