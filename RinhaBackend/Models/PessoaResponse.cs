using System.ComponentModel.DataAnnotations;

namespace RinhaBackend.Models
{
    public class PessoaResponse
    {
        public Guid Id { get; set; }

        public string Nome { get; set; }
        public string Apelido { get; set; }
        public string Nascimento { get; set; }

        public ICollection<string> Stacks { get; set; } = new List<string>();
    }
}
