namespace RinhaBackendV2.Models.Responses
{
    public class ExtratoResponse
    {
        public Saldo Saldo { get; set; }
        public IEnumerable<Transacao> Transacao { get; set; }
    }

    public class Saldo
    {
        public int Total { get; set;}
        public DateTime DataExtrato { get; set;}
        public int Limite { get; set;}
    }
}
