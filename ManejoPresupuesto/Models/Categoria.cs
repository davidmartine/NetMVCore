using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuesto.Models
{
    public class Categoria
    {
        public int Id { get; set; }

        [Required(ErrorMessage ="El campo {0} es Obligatorio")]
        [StringLength(maximumLength:50, ErrorMessage ="El {1} no puede ser mayor a 50 caracteres")]
        public string Nombre { get; set; }

        [Display(Name ="Tipo Operación")]
        public TipoOperacion TipoOperacionId { get; set; }
        public int UsuarioId { get; set; }
    }
}
