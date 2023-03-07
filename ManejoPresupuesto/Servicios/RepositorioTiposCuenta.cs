using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{
    public interface IRepositorioTiposCuenta
    {
        Task Actualizar(TipoCuenta tipocuenta);
        Task Borrar(int id);
        Task Crear(TipoCuenta tipocuenta);
        Task<bool> Existe(string nombre, int usuarioid);
        Task<TipoCuenta> ObtenerPorId(int id, int usuarioid);
        Task<IEnumerable<TipoCuenta>> Obtner(int usuarioid);
        Task Ordenar(IEnumerable<TipoCuenta> tipocuentaordenados);
    }
    public class RepositorioTiposCuenta : IRepositorioTiposCuenta
    {
        private readonly string connectionstring;
        public RepositorioTiposCuenta(IConfiguration configuration) 
        {
            connectionstring = configuration.GetConnectionString("DefaultConnection");

        }

        public async Task Crear(TipoCuenta tipocuenta)
        {
            using var connection = new SqlConnection(connectionstring);
            var id = await connection.QuerySingleAsync<int>("sp_TiposCuentasInsertar",
                                                            new {usuarioid=tipocuenta.UsuarioId,
                                                            nombre=tipocuenta.Nombre},
                                                            commandType: System.Data.CommandType.StoredProcedure);
            tipocuenta.Id = id;
        }

        public async Task<bool> Existe(string nombre,int usuarioid)
        {
            using var connection = new SqlConnection(connectionstring);
            var existe = await connection.QueryFirstOrDefaultAsync<int>(@$"
                                SELECT 1 
                                FROM TipoCuenta 
                                WHERE Nombre=@Nombre AND UsuarioId=@UsuarioId", new { nombre, usuarioid });
            return existe == 1;
        }

        public async Task<IEnumerable<TipoCuenta>> Obtner(int usuarioid)
        {
            using var connection = new SqlConnection(connectionstring);
            return await connection.QueryAsync<TipoCuenta>(@$"SELECT Id,Nombre,Orden 
                                                          FROM TipoCuenta
                                                          WHERE UsuarioId=@UsuarioId
                                                          ORDER BY Orden",
                                                          new { usuarioid });
        }

        public async Task Actualizar(TipoCuenta tipocuenta)
        {
            using var connection = new SqlConnection(connectionstring);
            await connection.ExecuteAsync($@"UPDATE TipoCuenta
                                         SET Nombre=@Nombre
                                         WHERE Id=@Id",tipocuenta);
        }
        public async Task<TipoCuenta> ObtenerPorId(int id,int usuarioid)
        {
            using var connection = new SqlConnection(connectionstring);
            return await connection.QueryFirstOrDefaultAsync<TipoCuenta>($@"
                                     SELECT id,Nombre,Orden 
                                     FROM TipoCuenta
                                     WHERE Id=@Id AND UsuarioId=@UsuarioId", new { id, usuarioid });
        }
        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(connectionstring);
            await connection.ExecuteAsync(@$"DELETE FROM TipoCuenta WHERE Id=@Id", new { id });
        }
        public async Task Ordenar(IEnumerable<TipoCuenta> tipocuentaordenados)
        {
            var query = "UPDATE TipoCuenta SET Orden=@Orden WHERE Id=@Id";
            using var connection = new SqlConnection(connectionstring);
            await connection.ExecuteAsync(query, tipocuentaordenados);
        }
    }
}
