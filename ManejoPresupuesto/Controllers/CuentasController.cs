using AutoMapper;
using ManejoPresupuesto.Models;
using ManejoPresupuesto.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualBasic;
using System.Reflection;

namespace ManejoPresupuesto.Controllers
{
    public class CuentasController : Controller
    {
        private readonly IRepositorioTiposCuenta repositorioTiposCuenta;
        private readonly IservicioUsuarios servicioUsuarios;
        private readonly IRepositorioCuentas repositorioCuentas;
        private readonly IMapper mapper;

        public CuentasController(IRepositorioTiposCuenta repositorioTiposCuenta,
            IservicioUsuarios servicioUsuarios,IRepositorioCuentas repositorioCuentas,
            IMapper mapper)
        {
            this.repositorioTiposCuenta = repositorioTiposCuenta;
            this.servicioUsuarios = servicioUsuarios;
            this.repositorioCuentas = repositorioCuentas;
            this.mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var usuarioid = servicioUsuarios.obtenerusuarioid();
            var cuentascontipocuenta = await repositorioCuentas.Buscar(usuarioid);
            var modelo = cuentascontipocuenta
                .GroupBy(x => x.TipoCuenta)
                .Select(grupo => new IndiceCuentaVista
                {
                    TipoCuenta = grupo.Key,
                    Cuentas = grupo.AsEnumerable()
                }).ToList();
            return View(modelo);
        }
        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            var usuarioId = servicioUsuarios.obtenerusuarioid();
           
            var modelo = new CreacionCuentaVista();
            modelo.TiposCuentas = await ObtenerTiposCuentas(usuarioId);
            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> Crear(CreacionCuentaVista cuenta)
        {
            var usuarioid = servicioUsuarios.obtenerusuarioid();
            var tipocuenta = await repositorioTiposCuenta.ObtenerPorId(cuenta.TipoCuentaId,usuarioid);
            if(tipocuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            if(!ModelState.IsValid)
            {
                cuenta.TiposCuentas = await ObtenerTiposCuentas(usuarioid);
                return View(cuenta);
            }
            await repositorioCuentas.Crear(cuenta);
            return RedirectToAction("Index");
        }

        private async Task<IEnumerable<SelectListItem>> ObtenerTiposCuentas(int usuarioId)
        {
            var tiposcuentas = await repositorioTiposCuenta.Obtner(usuarioId);
            return tiposcuentas.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
        }

        public async Task<IActionResult> Editar(int id)
        {
            var usuarioid = servicioUsuarios.obtenerusuarioid();
            var cuenta = await repositorioCuentas.ObtenerPorId(id, usuarioid);
            if(cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            var modelo = mapper.Map<CreacionCuentaVista>(cuenta);
            //new CreacionCuentaVista()
            //{
            //    Id = cuenta.Id,
            //    Nombre = cuenta.Nombre,
            //    TipoCuentaId = cuenta.TipoCuentaId,
            //    Descripcion = cuenta.Descripcion,
            //    Balance = cuenta.Balance
            //};
            modelo.TiposCuentas = await ObtenerTiposCuentas(usuarioid);
            return View(modelo);

        }
        [HttpPost]
        public async Task<IActionResult> Editar(CreacionCuentaVista cuentaeditar)
        {
            var usuarioid = servicioUsuarios.obtenerusuarioid();
            var cuenta = await repositorioCuentas.ObtenerPorId(cuentaeditar.Id, usuarioid);
            if(cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            var tipocuenta = await repositorioTiposCuenta.ObtenerPorId(cuentaeditar.TipoCuentaId, usuarioid);
            if(tipocuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            await repositorioCuentas.Actualizar(cuentaeditar);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Borrar(int id)
        {
            var usuarioid = servicioUsuarios.obtenerusuarioid();
            var cuenta = await repositorioCuentas.ObtenerPorId(id,usuarioid);
            if(cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            return View(cuenta);
        }
        [HttpPost]
        public async Task<IActionResult> BorrarCuenta(int id)
        {
            var usuariosid = servicioUsuarios.obtenerusuarioid();
            var cuenta = await repositorioCuentas.ObtenerPorId(id,usuariosid);
            if(cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            await repositorioCuentas.Borrar(id);
            return RedirectToAction("Index");
        }
    }
}
