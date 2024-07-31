using EventPlanner.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventPlanner.Models
{
    [Table("UsuarioPremium")]
    public class UsuarioPremium
    {
        [Required(ErrorMessage = "O campo Código é obrigatório.")]
        [Display(Name = "Codigo")]
        [Column("ID")]
        public int ID { get; set; }

        [Required(ErrorMessage = "O campo Nome é obrigatório.")]
        [MaxLength(70, ErrorMessage = "Nome pode ter no maximo 70 caracteres")]
        [Display(Name = "Nome")]
        [Column("Nome")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O campo Email é obrigatório.")]
        [EmailAddress(ErrorMessage = "Endereço de email inválido.")]
        [Column("Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "O campo CPF é obrigatório.")]
        [Display(Name = "CPF")]
        [Column("CPF")]
        public string CPF { get; set; }

        [Required(ErrorMessage = "O campo Telefone é obrigatório.")]
        [Display(Name = "Telefone")]
        [Column("Telefone")]
        public string Telefone { get; set; }

        [Required(ErrorMessage = "O campo Idade é obrigatório.")]
        [Display(Name = "Idade")]
        [Column("Idade")]
        public int Idade { get; set; }

        [Required(ErrorMessage = "O campo Senha é obrigatório.")]
        //[MinLength(8, ErrorMessage = "Senha precisa ter no minimo 8 carateres")]
        [Display(Name = "Senha")]
        [Column("Senha")]
        [DataType(DataType.Password)]
        public string Senha { get; set; }

        [Required(ErrorMessage = "O campo CEP é obrigatório.")]
        [Display(Name = "CEP")]
        [Column("CEP")]
        public string CEP { get; set; }

        [Required(ErrorMessage = "O campo Bairro é obrigatório.")]
        [Display(Name = "Bairro")]
        [Column("Bairro")]
        public string Bairro { get; set; }

        [Required(ErrorMessage = "O campo Cidade é obrigatório.")]
        [Display(Name = "Cidade")]
        [Column("Cidade")]
        public string Cidade { get; set; }

        [Required(ErrorMessage = "O campo Estado é obrigatório.")]
        [Display(Name = "Estado")]
        [Column("Estado")]
        public string Estado { get; set; }

        [Required(ErrorMessage = "O campo Número do Cartão é obrigatório.")]
        [CreditCard(ErrorMessage = "Cartão de crédito inválido.")]
        [Display(Name = "Número do Cartão")]
        [Column("NumeroCartao")]
        public string NumeroCartao { get; set; }

        [Required(ErrorMessage = "O campo Titular do cartão é obrigatório.")]
        [Display(Name = "Titular do cartao")]
        [Column("TitularCartao")]
        public string TitularCartao { get; set; }

        [Required(ErrorMessage = "O campo Data de Validade é obrigatório.")]
        [Display(Name = "Data de Validade")]
        [Column("DataValidade")]
        public string DataValidade { get; set; }

        [Required(ErrorMessage = "O campo Código de Segurança é obrigatório.")]
        [Display(Name = "Código de Segurança")]
        [Column("CodigoSeguranca")]
        [MaxLength(3, ErrorMessage = "O Código de Segurança precisa ter 3 caracteres.")]
        [DataType(DataType.Password)]
        public string CodigoSeguranca { get; set; }
        public int? eventoID { get; set; }
        public virtual Evento? Evento { get; set; }

        public int? usuarioID { get; set; }
        public virtual Usuario? Usuario { get; set; }

        public int? AvaliacaoID { get; set; }
        public virtual Avaliacao? Avaliacao { get; set; }

        [Required(ErrorMessage = "O campo Tipo é obrigatório.")]
        [Display(Name = "Tipo")]
        [Column("Tipo")]
        [DefaultValue(2)]
        public int Tipo { get; set; }
    }
}