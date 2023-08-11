using System.ComponentModel.DataAnnotations;

namespace RinhaBackend.Models
{
    public class PessoaRequest
    {
        [Required]
        public string Nome { get; set; }
        [Required]
        public string Apelido { get; set; }
        public string Nascimento { get; set; }

        public ICollection<string> Stacks { get; set; } = new List<string>();
    }
}
