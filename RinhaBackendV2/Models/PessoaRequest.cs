using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace RinhaBackendV2.Models
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
            RuleFor(x => x.Nascimento).Must(IsValidDate).WithMessage("Data invalida");
        }

        private bool IsValidDate(string date)
        {
            DateTime dateConverted;
            if (!DateTime.TryParse(date, out dateConverted))
                return false;
            return true;
        }
    }
}
