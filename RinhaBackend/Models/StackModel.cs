using System.Text.Json.Serialization;

namespace RinhaBackend.Models
{
    public class StackModel
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public Guid PessoaId { get; set; }

        [JsonIgnore]
        public PessoasModel Pessoa { get; set; }
    }
}
