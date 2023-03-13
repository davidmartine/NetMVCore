using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{
    public interface IRepositorioTransacciones
    {
        Task Actualizar(Transaccion transaccion, decimal montoAnterior, int cuentaAnterior);
        Task Borrar(int id);
        Task Crear(Transaccion transaccion);
        Task<IEnumerable<Transaccion>> ObtenerPorCuentaId(ObtenerTransaccionesPorCuenta modelo);
        Task<Transaccion> ObtenerPorId(int id, int usuarioid);
    }
    public class RepositorioTransacciones : IRepositorioTransacciones
    {
        private readonly string connectionString;
        public RepositorioTransacciones(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");

        }
        public async Task Crear(Transaccion transaccion)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>("Transaccion_Insertar",
                new
                {
                    transaccion.UsuarioId,
                    transaccion.FechaTransaccion,
                    transaccion.Monto,
                    transaccion.Nota,
                    transaccion.CuentaId,
                    transaccion.CategoriaId,
                },commandType: System.Data.CommandType.StoredProcedure);
            transaccion.Id = id;
        }

        public async Task Actualizar(Transaccion transaccion, decimal montoAnterior,
            int CuentaAnteriorId)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("Transacciones_Actualizar",
                new
                {
                    transaccion.Id,
                    transaccion.FechaTransaccion,
                    transaccion.Monto,
                    transaccion.CategoriaId,
                    transaccion.CuentaId,
                    transaccion.Nota,
                    montoAnterior,
                    CuentaAnteriorId
                }, commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<Transaccion> ObtenerPorId(int id,int usuarioid)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Transaccion>(@"
                                    SELECT T.*,c.TipoOperacionId
                                    FROM Transacciones AS T
                                    INNER JOIN Categorias AS C 
                                    ON T.CategoriaId=C.Id
                                    WHERE T.Id = @Id AND T.UsuarioId=@UsuarioId",
                                    new {id,usuarioid});
        }

        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("sp_Transacciones_Borrar",
                new { id }, commandType: System.Data.CommandType.StoredProcedure);

        }

        public async Task<IEnumerable<Transaccion>> ObtenerPorCuentaId(ObtenerTransaccionesPorCuenta modelo)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Transaccion>(@"
                SELECT T.Id,T.Monto,T.FechaTransaccion,C.Nombre AS Categoria,
                CU.Nombre AS Cuenta,C.TipoOperacionId
                FROM Transacciones T
                INNER JOIN Categorias C
                ON C.Id=T.CategoriaId
                INNER JOIN Cuentas CU
                ON CU.Id=T.CuentaId
                WHERE T.CuentaId=@CuentaId AND T.UsuarioId=@UsuarioId
                AND FechaTransaccion BETWEEN @FechaInicio AND @FechaFin", modelo);

        }
    }
}
