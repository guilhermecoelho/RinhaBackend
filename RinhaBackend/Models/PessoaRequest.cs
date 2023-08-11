using FluentValidation;
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

    public class PessoaRequestValidation : AbstractValidator<PessoaRequest>
    {
        public PessoaRequestValidation()
        {
            RuleFor(x => x.Nome).NotNull().MaximumLength(100);
            RuleFor(x => x.Apelido).NotNull().MaximumLength(32);
            RuleFor(x => x.Nascimento).NotNull();
        }
    }
}
