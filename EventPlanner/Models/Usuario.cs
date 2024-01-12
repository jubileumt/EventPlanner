using EventPlanner.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventPlanner.Models
{
    [Table("Usuario")]

    public class Usuario

    {
        [Required]
        [Display(Name = "Codigo")]
        [Column("ID")]
        public int ID { get; set; }

        [Required(ErrorMessage = "Nome Precisa ser informado")]
        [MinLength(10, ErrorMessage = "Nome precisa ter no minimo 10 caracteres")]
        [MaxLength(70, ErrorMessage = "Nome pode ter no maximo 70 caracteres")]
        [Display(Name = "Nome")]
        [Column("Nome")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Email precisa ser informado")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Endereço de email inválido.")]
        [Column("Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Cpf precisa ser informado")]
        [MaxLength(11)]
        [Display(Name = "CPF")]
        [Column("CPF")]
        public string CPF { get; set; }

        [Required(ErrorMessage = "Telefone precisa ser informado")]
        [MaxLength(11, ErrorMessage = "Telefone precisa ter o DDD e o numero")]
        [Display(Name = "Telefone")]
        [Column("Telefone")]
        public string Telefone { get; set; }

        [Required(ErrorMessage = "Idade precisa ser informado")]
        [Display(Name = "Idade")]
        [Column("Idade")]
        public int Idade { get; set; }

        [Required(ErrorMessage = "Senha precisa ser informado")]
        [MinLength(8, ErrorMessage = "Senha precisa ter no minimo 8 carateres")]
        [Display(Name = "Senha")]
        [Column("Senha")]
        [DataType(DataType.Password)]
        public string Senha { get; set; }

        [ForeignKey("Evento")]
        public int? eventoID { get; set; }
        public virtual Evento? Evento { get; set; }

        [Display(Name = "Tipo")]
        [Column("Tipo")]
        [DefaultValue(1)]
        public int Tipo { get; set; }


    }
}