namespace RinhaBackendV2.Models.Requests
{
    public class TransacoesRequest
    {
        public int Valor { get; set; }
        public char Tipo { get; set; }
        public string Descricao { get; set; }
    }
}
