using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventPlanner.Models
{
    public class Avaliacao
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [StringLength(1000)]
        public string Comentario { get; set; }

        [DataType(DataType.Date)]
        public DateOnly DataComentario { get; set; }
        public int EventoID { get; set; }
        public int? UsuarioPremiumID { get; set; }
        public int? UsuarioID { get; set; }

    }
}