using Dapper;
using ManejoPresupuesto.Models;
using ManejoPresupuesto.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Controllers
{
    public class TiposCuentasController : Controller
    {
        private readonly IRepositorioTiposCuenta repositorio_Tiposcuenta;
        private readonly IservicioUsuarios servicio_Usuarios;

        public TiposCuentasController(IRepositorioTiposCuenta repositorio_tiposcuenta,IservicioUsuarios servicio_usuarios) 
        {
            this.repositorio_Tiposcuenta= repositorio_tiposcuenta;
            this.servicio_Usuarios = servicio_usuarios;
            
        }

        public async Task<IActionResult> Index()
        {
            var usuarioid = servicio_Usuarios.obtenerusuarioid();
            var tipos_cuentas = await repositorio_Tiposcuenta.Obtner(usuarioid);
            return View(tipos_cuentas);
        }
        public IActionResult Crear()
        {
            
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Crear(TipoCuenta tipo_cuenta)
        {
            if (!ModelState.IsValid)
            {
                return View(tipo_cuenta);
            }
            tipo_cuenta.UsuarioId = servicio_Usuarios.obtenerusuarioid();
            var yaexistetipocuenta = await repositorio_Tiposcuenta.Existe(tipo_cuenta.Nombre, tipo_cuenta.UsuarioId);
            if (yaexistetipocuenta)
            {
                ModelState.AddModelError(nameof(tipo_cuenta.Nombre), $"El nombre {tipo_cuenta.Nombre} ya existe");
                return View(tipo_cuenta);
            }
            await repositorio_Tiposcuenta.Crear(tipo_cuenta);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> Editar(int id)
        {
            var usuarioid = servicio_Usuarios.obtenerusuarioid();
            var tipo_cuenta = await repositorio_Tiposcuenta.ObtenerPorId(id, usuarioid);
            if(tipo_cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            return View(tipo_cuenta);
        }
        [HttpPost]
        public async Task<ActionResult> Editar(TipoCuenta tipo_cuenta)
        {
            var usuarioid = servicio_Usuarios.obtenerusuarioid();
            var tipocuentaExiste = await repositorio_Tiposcuenta.ObtenerPorId(tipo_cuenta.Id, usuarioid);
            if(tipocuentaExiste is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            await repositorio_Tiposcuenta.Actualizar(tipo_cuenta);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Borrar(int id)
        {
            var usuarioId = servicio_Usuarios.obtenerusuarioid();
            var tipocuenta= await repositorio_Tiposcuenta.ObtenerPorId(id, usuarioId);
            if(tipocuenta is null)
            {
                return RedirectToAction("NoEncontrado","Home");
            }
            return View(tipocuenta);
        }
        [HttpPost]
        public async Task<IActionResult> BorrarTipoCuenta(int id)
        {
            var usuarioid = servicio_Usuarios.obtenerusuarioid();
            var tipocuenta = await repositorio_Tiposcuenta.ObtenerPorId(id, usuarioid);
            if(tipocuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            await repositorio_Tiposcuenta.Borrar(id);
            return RedirectToAction("Index");

        }

        [HttpGet]
        public async Task<IActionResult> VerificarExisteTipoCuenta(string nombre)
        {
            var usuarioid = servicio_Usuarios.obtenerusuarioid();
            var yaexistetipocuenta = await repositorio_Tiposcuenta.Existe(nombre, usuarioid);
            if (yaexistetipocuenta)
            {
                return Json($"El nombre {nombre} ya existe");
            }
            return Json(true);
        }

        [HttpPost]
        public async Task<IActionResult> Ordenar([FromBody] int[] ids)
        {
            var usuarioid = servicio_Usuarios.obtenerusuarioid();
            var tiposcuentas = await repositorio_Tiposcuenta.Obtner(usuarioid);
            var idstiposcuentas = tiposcuentas.Select(x => x.Id);
            var idstiposcuentasnopertenecenalusuario = ids.Except(idstiposcuentas).ToList();
            if(idstiposcuentasnopertenecenalusuario.Count > 0)
            {
                return Forbid();
            }
            var tiposcuentasordenados= ids.Select((valor,indice)=> new 
                                       TipoCuenta() { Id=valor,Orden= indice +1}).AsEnumerable();
            await repositorio_Tiposcuenta.Ordenar(tiposcuentasordenados);
            return Ok();

        }
    }
}
