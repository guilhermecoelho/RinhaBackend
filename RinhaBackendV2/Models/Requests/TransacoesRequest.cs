using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RinhaBackendV2.Models.Requests
{
    public class TransacoesRequest
    {
        [Required]
        public int Valor { get; set; }
        [Required]
        [Range(1,1)]
        public string Tipo { get; set; }
        [Required]
        [Range(1, 10)]
        public string Descricao { get; set; }
        [NotMapped]
        public int ClienteId { get; set; }
    }
}
