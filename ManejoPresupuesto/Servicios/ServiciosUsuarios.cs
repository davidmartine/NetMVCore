namespace ManejoPresupuesto.Servicios
{
    public interface IservicioUsuarios
    {
        int obtenerusuarioid();
    }
    public class ServiciosUsuarios : IservicioUsuarios
    {
        public int obtenerusuarioid()
        { 
            return 1; 
        }
    }
}
