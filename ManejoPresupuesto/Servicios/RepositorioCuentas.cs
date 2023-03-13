using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{
    public interface IRepositorioCuentas
    {
        Task Actualizar(CreacionCuentaVista cuenta);
        Task Borrar(int id);
        Task<IEnumerable<Cuenta>> Buscar(int usuarioId);
        Task Crear(Cuenta cuenta);
        Task<Cuenta> ObtenerPorId(int id, int usuarioId);
    }
    public class RepositorioCuentas : IRepositorioCuentas
    {
        private readonly string connectionString;

        public RepositorioCuentas(IConfiguration configuration) 
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear(Cuenta cuenta)
        {
            using var conexion = new SqlConnection(connectionString);
            var id = await conexion.QuerySingleAsync<int>("INSERT INTO Cuentas " +
                "(Nombre,TipoCuentaId,Balance,Descripcion)\r\n" +
                "VALUES (@Nombre,@TipoCuentaId,@Balance,@Descripcion);\r\n" +
                "SELECT SCOPE_IDENTITY();",cuenta);
            cuenta.Id = id;
        }

        public async Task<IEnumerable<Cuenta>> Buscar(int usuarioId)
        {
            using var conexion = new SqlConnection(connectionString);
            return await conexion.QueryAsync<Cuenta>(@"
                   SELECT c.Id,c.Nombre,c.Balance,tc.Nombre AS TipoCuenta FROM Cuentas c
                   JOIN TipoCuenta tc ON tc.Id=c.TipoCuentaId
                   WHERE tc.UsuarioId=@UsuarioId
                   ORDER BY tc.Orden", new { usuarioId });
        }
        public async Task<Cuenta> ObtenerPorId(int id, int usuarioId)
        {
            using var conexion = new SqlConnection(connectionString);
            return await conexion.QueryFirstOrDefaultAsync<Cuenta>(@"
                                  SELECT c.Id,c.Nombre,c.Balance,c.Descripcion,TipoCuentaId
                                  FROM Cuentas c
                                  JOIN TipoCuenta tc ON tc.Id=c.TipoCuentaId
                                  WHERE tc.UsuarioId=@UsuarioId AND c.Id=@Id", new { id, usuarioId });

        }

        public async Task Actualizar(CreacionCuentaVista cuenta)
        {
            using var conexion = new SqlConnection(connectionString);
            await conexion.ExecuteAsync(@"
                           UPDATE Cuentas 
                           SET Nombre=@Nombre,Balance=@Balance,Descripcion=@Descripcion,
                           TipoCuentaId=@TipoCuentaId
                           WHERE Id=@Id", cuenta);
        }

        public async Task Borrar(int id)
        {
            using var conexion = new SqlConnection(connectionString);
            await conexion.ExecuteAsync(@"DELETE FROM Cuentas WHERE Id=@Id", new { id });

        }
    }
}
