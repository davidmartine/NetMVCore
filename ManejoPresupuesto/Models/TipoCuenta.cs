using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuesto.Models
{
    public class TipoCuenta : IValidatableObject
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo {0} es Requerido.")]
        [StringLength(maximumLength: 50,MinimumLength =3,ErrorMessage ="La Longitud del campo {0} debe estar entre {2} y {1}")]
        [Remote(action: "VerificarExisteTipoCuenta",controller: "TiposCuentas")]
        public string Nombre { get; set; }
        public int UsuarioId { get; set; }
        public int Orden { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if(Nombre !=null && Nombre.Length > 0)
            {
                var primeraLetra = Nombre[0].ToString();

                if(primeraLetra != primeraLetra.ToUpper())
                {
                    yield return new ValidationResult("La primera Letra debe de ser Mayuscula",
                                                      new[] {nameof(Nombre)});
                }
            }
        }
    }
}
