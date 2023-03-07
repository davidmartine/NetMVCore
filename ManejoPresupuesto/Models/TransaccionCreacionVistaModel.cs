using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuesto.Models
{
    public class TransaccionCreacionVistaModel : Transaccion
    {
        public IEnumerable<SelectListItem> Cuentas { get; set; }

        public IEnumerable<SelectListItem> Categorias { get; set; }

    }
}
