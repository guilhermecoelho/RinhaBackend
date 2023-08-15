using System.ComponentModel.DataAnnotations;

namespace RinhaBackend.Models
{
    public class PessoaModel
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nome { get; set;}
        [Required]
        [StringLength(32)]
        public string Apelido { get; set;}
        [Required]
        public DateTime Nascimento { get; set; }

        public ICollection<StackModel> Stacks { get; set;} = new List<StackModel>();
    }
}
