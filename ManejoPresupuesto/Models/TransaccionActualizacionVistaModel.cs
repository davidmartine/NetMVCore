namespace ManejoPresupuesto.Models
{
    public class TransaccionActualizacionVistaModel : TransaccionCreacionVistaModel
    {
        public int CuentaAnteriorId { get; set; }
        public Decimal MontoAnterior { get; set; }
    }
}
