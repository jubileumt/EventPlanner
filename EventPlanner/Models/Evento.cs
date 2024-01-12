using EventPlanner.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace EventPlanner.Models
{
    [Table("Evento")]
    public class Evento
    {
        [Required(ErrorMessage = "O campo ID é obrigatório.")]
        [Display(Name = "Codigo")]
        [Column("ID")]
        public int ID { get; set; }

        [Required(ErrorMessage = "O campo Nome é obrigatório.")]
        [StringLength(100, ErrorMessage = "O Nome do Evento não pode exceder 100 caracteres.")]
        [Display(Name = "Nome do Evento")]
        [Column("NomeEvento")]
        public string NomeEvento { get; set; }

        [Required(ErrorMessage = "O campo Descrição é obrigatório.")]
        [StringLength(500, ErrorMessage = "A Descrição do evento não pode exceder 500 caracteres.")]
        [Display(Name = "Descrição do evento")]
        [Column("Descricao")]
        public string Descricao { get; set; }

        [Required(ErrorMessage = "O campo Data é obrigatório.")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
        [Display(Name = "Data inicial do evento")]
        [Column("DataInicio")]
        public DateTime DataInicio { get; set; }

        [Required(ErrorMessage = "O campo Data é obrigatório.")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
        [Display(Name = "Data final do evento")]
        [Column("DataFinal")]
        public DateTime DataFinal { get; set; }

        [Display(Name = "Data de criação do evento")]
        [DataType(DataType.DateTime)]
        [Column("DataCriacao")]
        public DateTime DataCriacao { get; set; }

        [NotMapped]
        [Display(Name = "Data de criação do evento")]
        public string DataCriacaoFormatadaBr
        {
            get
            {
                return DataCriacao.ToString("dd/MM/yyyy HH:mm:ss");
            }
        }

        [NotMapped]
        public long TempoAteEventoTicks { get; set; }

        [Display(Name = "Tempo ate inicio do evento")]
        [Column("TempoAteEvento")]
        public TimeSpan TempoAteEvento
        {
            get { return TimeSpan.FromTicks(TempoAteEventoTicks); }
            set { TempoAteEventoTicks = value.Ticks; }
        }

        [Display(Name = "Tempo ate inicio do evento")]
        [NotMapped]
        public string TempoAteEventoFormatado
        {
            get
            {
                return string.Format("{0} dias, {1} horas, {2} minutos",
                    TempoAteEvento.Days, TempoAteEvento.Hours, TempoAteEvento.Minutes);
            }
        }

        [NotMapped]
        public long DuracaoTicks { get; set; }

        [Display(Name = "Duração do evento")]
        [Column("Duracao")]
        public TimeSpan Duracao
        {
            get { return TimeSpan.FromTicks(DuracaoTicks); }
            set { DuracaoTicks = value.Ticks; }
        }

        [Display(Name = "Duração do evento")]
        [NotMapped]
        public string DuracaoFormatada
        {
            get
            {
                return string.Format("{0} dias, {1} horas, {2} minutos",
                    Duracao.Days, Duracao.Hours, Duracao.Minutes);
            }
        }

        [Display(Name = "Metros quadrados do evento")]
        [Column("QuantMetrosQuadrados")]
        public int? QuantMetrosQuadrados { get; set; }

        [Required(ErrorMessage = "Quantidade maxima de pessoas deve ser informada!")]
        [Display(Name = "Quantidade maxima de pesssoas no evento")]
        [Column("QuantMaxPessoas")]
        public int? QuantMaxPessoas { get; set; }

        [Display(Name = "Quantidade de crianças no evento")]
        [Column("QuantCriancas")]
        public int? QuantCriancas { get; set; }

        [Display(Name = "Quantidade de refrigerante")]
        [Column("QuantRefri")]
        public int? QuantRefri { get; set; }

        [Display(Name = "Quantidade de alcool")]
        [Column("QuantAlcool")]
        public int? QuantAlcool { get; set; }

        [Display(Name = "Quantidade de carne")]
        [Column("QuantCarne")]
        public int? QuantCarne { get; set; }

        [Display(Name = "Quantidade de doces")]
        [Column("QuantDoces")]
        public int? QuantDoces { get; set; }

        [Display(Name = "Quantidade de salgados")]
        [Column("QuantSalgados")]
        public int? QuantSalgados { get; set; }

        [Display(Name = "Quantidade de cadeiras no evento")]
        [Column("QuantCadeiras")]
        public int? QuantCadeiras { get; set; }

        [Required(ErrorMessage = "O campo CEP é obrigatório.")]
        [StringLength(9, ErrorMessage = "O CEP deve ter exatamente 9 caracteres.", MinimumLength = 9)]
        [RegularExpression(@"^\d{5}-\d{3}$", ErrorMessage = "O CEP deve estar no formato XXXXX-XXX.")]
        [Display(Name = "Cep do evento")]
        [Column("CEP")]
        public string CEP { get; set; }

        [Display(Name = "Estado do evento")]
        [Column("Estado")]
        public string? Estado { get; set; }

        [Display(Name = "Cidade do evento")]
        [Column("Cidade")]
        public string? Cidade { get; set; }

        [Display(Name = "Bairro do evento")]
        [Column("Bairro")]
        public string? Bairro { get; set; }

        [Display(Name = "Foto do evento")]
        [Column("FotoDoEvento")]
        public byte[]? FotoDoEvento { get; set; }

        [Display(Name = "Media de doçes por pessoas")]
        [NotMapped]
        public double? MediaDocesPorPessoa { get; set; }

        [Display(Name = "Media de salgados por pessoas")]
        [NotMapped]
        public double? MediaSalgadosPorPessoa { get; set; }

        [Display(Name = "Pessoas por metro quadrado")]
        [NotMapped]
        public double? MediaMetrosPorPessoa { get; set; }

        [Display(Name = "Media de cadeiras por pessoas")]
        [NotMapped]
        public double? MediaCadeiraPorPessoa { get; set; }

        [Display(Name = "Media de refrigerante por pessoas")]
        [NotMapped]
        public double? MediaRefriPorPessoa { get; set; }


        [Display(Name = "Media de alcool por pessoa")]
        [NotMapped]
        public double? MediaAlcoolPorPessoa { get; set; }


        [Display(Name = "Percentual de crianças")]
        [NotMapped]
        public double? PercentualDeCrianças { get; set; }

        [Display(Name = "Percentual de adultos")]
        [NotMapped]
        public double? PercentualDeAdultos { get; set; }

        [Display(Name = "Quantidade de copos")]
        [NotMapped]
        public double? QuantidadeDeCopos { get; set; }

        [Display(Name = "Quantidade de pratos")]
        [NotMapped]
        public double? QuantidadeDePratos { get; set; }

        [Display(Name = "Quantidade de talheres")]
        [NotMapped]
        public int? QuantidadeTalheres { get; set; }

        [Display(Name = "Quantidade de guardanapos")]
        [NotMapped]
        public double? QuantidadeQuardanapos { get; set; }

        [Column("Identificador")]
        public string? Identificador { get; set; }

        [Display(Name = "Tipo do evento")]
        [Column("TipoEvento")]
        public string? TipoEvento { get; set; }

        [NotMapped]
        public string? Buscador { get; set; }

        [NotMapped]
        public List<Avaliacao>? Avaliacoes { get; set; }

        [Display(Name = "Quem registrou o evento do evento")]
        [Column("Organizador")]
        public string? Organizador { get; set; }
        public int? UsuarioID { get; set; }
        public int? UsuarioPremiumID { get; set; }
    }
}