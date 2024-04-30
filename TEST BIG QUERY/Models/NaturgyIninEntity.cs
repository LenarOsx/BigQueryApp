namespace TEST_BIG_QUERY.Models
{
    public class NaturgyIninEntity
    {
        public string Hora { get; set; } = string.Empty;
        public float Entrantes { get; set; }
        public float TMO { get; set; }
        public float Atendidas { get; set; }
        public float Abandonos { get; set; }
        public int LlamadasDesconectadas { get; set; }
        public int DesbordamientoIndirecto { get; set; }
        public float PorcentajeContestadas { get; set; }
        public float PorcentajeAbandonadas { get; set; }
        public int MediaAgentes { get; set; }
        public float NivelAtencion18Seg { get; set; }
        public float Atendidas_0_18 { get; set; }
        public DateTime StartDate { get; set; }
    }
}
