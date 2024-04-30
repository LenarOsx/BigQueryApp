
using static Test_Aplicacion_Consola_C_.Net_Framework.Data.ExtensionMethod;

namespace Core.Dtos
{
    public class CargaLeadsDto
    {

        //[JsonPropertyName("CONTACTID")]
        [DbReaderMap("CONTACTID")]
        public string ContactId { get; set; } = string.Empty;
        [DbReaderMap("Nombre")]

        public string Nombre { get; set; } = string.Empty;
        [DbReaderMap("FIJO1")]
        public string Fijo1 { get; set; } = string.Empty;
        [DbReaderMap("FIJO2")]
        public string Fijo2 { get; set; } = string.Empty;
        [DbReaderMap("FIJO3")]
        public string Fijo3 { get; set; } = string.Empty;
        [DbReaderMap("FIJO4")]
        public string Fijo4 { get; set; } = string.Empty;
        [DbReaderMap("FIJO5")]
        public string Fijo5 { get; set; } = string.Empty;
        [DbReaderMap("FIJO6")]
        public string Fijo6 { get; set; } = string.Empty;
        [DbReaderMap("MOVIL1")]
        public string Movil1 { get; set; } = string.Empty;
        [DbReaderMap("MOVIL2")]
        public string Movil2 { get; set; } = string.Empty;
        [DbReaderMap("MOVIL3")]
        public string Movil3 { get; set; } = string.Empty;
        [DbReaderMap("MOVIL4")]
        public string Movil4 { get; set; } = string.Empty;
        [DbReaderMap("MOVIL5")]
        public string Movil5 { get; set; } = string.Empty;
        [DbReaderMap("MOVIL6")]
        public string Movil6 { get; set; } = string.Empty;
        [DbReaderMap("CampaignId")]
        public string CampaignId { get; set; } = string.Empty;
        [DbReaderMap("OutboundProcessId")]
        public string OutboundProcessId { get; set; } = string.Empty;
        [DbReaderMap("Lote")]
        public string Lote { get; set; } = string.Empty;

    }
}
