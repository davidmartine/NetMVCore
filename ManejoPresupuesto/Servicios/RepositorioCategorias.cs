using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{
    public interface IRepositorioCategorias
    {
        Task Actualizar(Categoria categoria);
        Task Borrar(int id);
        Task Crear(Categoria categoria);
        Task<IEnumerable<Categoria>> Obtener(int usuarioid);
        Task<IEnumerable<Categoria>> Obtener(int usuarioId, TipoOperacion tipoOperacion);
        Task<Categoria> ObtenerporId(int id, int usuarioid);
    }
    public class RepositorioCategorias :IRepositorioCategorias
    {
        private readonly string connectionSting;
        public RepositorioCategorias(IConfiguration configuration)
        {
            connectionSting = configuration.GetConnectionString("DefaultConnection");

        }
        public async Task Crear(Categoria categoria)
        {
            using var conexion = new SqlConnection(connectionSting);
            var id = await conexion.QuerySingleAsync<int>(@"
                                    INSERT INTO Categorias(Nombre,TipoOperacionId,UsuarioId)
                                    VALUES(@Nombre,@TipoOperacionId,@UsuarioId);
                                    SELECT SCOPE_IDENTITY();", categoria);
            categoria.Id = id;
        }
        public async Task<IEnumerable<Categoria>> Obtener(int usuarioid)
        {
            using var conexion = new SqlConnection(connectionSting);
            return await conexion.QueryAsync<Categoria>(@"
                                  SELECT * FROM Categorias 
                                  WHERE UsuarioId=@UsuarioId",new { usuarioid });

        }

        public async Task<IEnumerable<Categoria>> Obtener(int usuarioId,TipoOperacion tipoOperacionId)
        {
            using var conexion = new SqlConnection(connectionSting);
            return await conexion.QueryAsync<Categoria>(@"
                                   SELECT * FROM Categorias
                                   WHERE UsuarioId=@UsuarioId AND TipoOperacionId=@TipoOperacionId",
                                   new { usuarioId, tipoOperacionId });
        }
        public async Task<Categoria> ObtenerporId(int id,int usuarioid)
        {
            using var conexion = new SqlConnection(connectionSting);
            return await conexion.QueryFirstOrDefaultAsync<Categoria>(@"
                                  SELECT * 
                                  FROM Categorias 
                                  WHERE Id=@Id AND UsuarioId=@UsuarioId", new {id,usuarioid});


        }
        public async Task Actualizar(Categoria categoria)
        {
            using var conexion = new SqlConnection(connectionSting);
            await conexion.ExecuteAsync(@"
                           UPDATE Categorias 
                           SET Nombre=@Nombre,TipoOperacionId=@TipoOperacionId
                           WHERE Id=@Id", categoria);
        }

        public async Task Borrar(int id)
        {
            using var conexion = new SqlConnection(connectionSting);
            await conexion.ExecuteAsync(@"DELETE FROM Categorias WHERE Id=@Id", new { id });
        }
    }
}
