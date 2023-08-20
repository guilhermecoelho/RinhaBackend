using System.Text.Json.Serialization;

namespace RinhaBackendV2.Models
{
    public class StackModel
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public Guid PessoaId { get; set; }

        [JsonIgnore]
        public PessoaModel Pessoa { get; set; }
    }
}
