using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManejoPresupuesto.Models
{
    public class CreacionCuentaVista : Cuenta
    {
        public IEnumerable<SelectListItem> TiposCuentas { get; set; }

    }
}
