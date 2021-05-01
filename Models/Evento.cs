using Agenda.Desktop.Areas.Identity.Data;
using Agenda.Desktop.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace Agenda.Desktop.Models
{
    public class Evento
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "É necessário preencher o campo {0}.", AllowEmptyStrings = false)]
        [StringLength(70, MinimumLength = 4, ErrorMessage = "O campo {0} precisa conter de {2} a {1} caracteres.")]
        public string Nome { get; set; }

        [Display(Name = "Descrição")]
        [StringLength(500, ErrorMessage = "O campo {0} precisa conter no máximo {1} caracteres.")]
        public string Descricao { get; set; }

        [Required(ErrorMessage = "É necessário preencher o campo {0}.")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Início")]
        [DisplayFormat(DataFormatString = "mm/dd/yyyy")]
        [DateTimeCompare]
        public DateTime DataInicial { get; set; }

        [Required(ErrorMessage = "É necessário preencher o campo {0}.")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Fim")]
        [DisplayFormat(DataFormatString = "mm/dd/yyyy")]
        [DateTimeCompareInterval("DataInicial")]
        public DateTime DataFinal { get; set; }

        [Required(ErrorMessage = "É necessário preencher o campo {0}.")]
        [StringLength(40, MinimumLength = 4, ErrorMessage = "O campo {0} precisa conter de {2} a {1} caracteres.")]
        public string Local { get; set; }

        [Range(0, 10000, ErrorMessage = "A quantidade de {0} deve ser ao menos {1} e no máximo {2}.")]
        public int Participantes { get; set; }

        [Required(ErrorMessage = "É necessário preencher o campo {0}.")]
        [CustomTipoValidation("DataInicial", "DataFinal")]
        public string Tipo { get; set; }

        [Display(Name = "Lista de Participantes")]
        public IEnumerable<Participante> ListaDeParticipantes { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime DataCriado { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime DataAlterado { get; set; }

        public string AppIdentityUserId { get; set; }
    }

    public class CustomTipoValidation : ValidationAttribute, IClientValidatable
    {
        private readonly string inical;
        private readonly string final;

        public CustomTipoValidation(string inical, string final)
        {
            this.inical = inical;
            this.final = final;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            throw new NotImplementedException();
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            Type _t = validationContext.ObjectInstance.GetType();

            var startDate = (DateTime)_t.GetProperty(inical).
                GetValue(validationContext.ObjectInstance);
            var finalDate = (DateTime)_t.GetProperty(final).
                GetValue(validationContext.ObjectInstance);

            var context = (ApplicationDbContext)validationContext
                .GetService(typeof(ApplicationDbContext));

            string tipo = (string)value;
            if (tipo != "Exclusivo") return ValidationResult.Success;

            var exlusivos = context.Eventos.Where(e => e.Tipo == "Exclusivo").AsEnumerable();
            if (!(exlusivos.Count() > 0)) return ValidationResult.Success;
            Evento conflito = new Evento();
            foreach (var exclusivo in exlusivos)
            {
                if (finalDate < exclusivo.DataInicial) return ValidationResult.Success;
                if (exclusivo.DataFinal < startDate) return ValidationResult.Success;
                conflito = exclusivo;
            }
            return new ValidationResult(string.Format
                ("Em conflito com evento exclusivo existente: {0} ({1} - {2})", conflito.Nome, conflito.DataInicial, conflito.DataFinal));
        }
    }

    public class DateTimeCompareAttribute : ValidationAttribute, IClientValidatable
    {
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            throw new NotImplementedException();
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime inputDate = (DateTime)value;
            if (inputDate != null && DateTime.Now <= inputDate)
            {
                return ValidationResult.Success;
            }
            return new ValidationResult("A data inicial do evento não é válida");
        }
    }

    public class DateTimeCompareIntervalAttribute : ValidationAttribute, IClientValidatable
    {
        private string start;

        public DateTimeCompareIntervalAttribute(string start)
        {
            this.start = start;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            throw new NotImplementedException();
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (validationContext.ObjectInstance != null)
            {
                Type _t = validationContext.ObjectInstance.GetType();
                var startDate = (DateTime)_t.GetProperty(start).
                    GetValue(validationContext.ObjectInstance);
                DateTime inputDate = (DateTime)value;
                if (inputDate != null &&
                    startDate < inputDate &&
                    DateTime.Now < inputDate)
                {
                    return ValidationResult.Success;
                }
            }
            return new ValidationResult("A data final do evento não é válida");
        }
    }
}