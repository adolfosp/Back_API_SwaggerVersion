using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MimicAPI.V1.Models
{
    public class Palavra
    {

        public int Id { get; set; }
        [Required]
        public string Nome { get; set; }
        [Required]
        public int Pontuacao { get; set; }

        public bool Ativo { get; set; }

        public DateTime DataCriacao { get; set; }

        public DateTime? Atualizado { get; set; }

    }
}
