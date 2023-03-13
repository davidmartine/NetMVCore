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
        private readonly IRepositorioTransacciones transacciones;

        public CuentasController(IRepositorioTiposCuenta repositorioTiposCuenta,
            IservicioUsuarios servicioUsuarios,IRepositorioCuentas repositorioCuentas,
            IMapper mapper,IRepositorioTransacciones transacciones)
        {
            this.repositorioTiposCuenta = repositorioTiposCuenta;
            this.servicioUsuarios = servicioUsuarios;
            this.repositorioCuentas = repositorioCuentas;
            this.mapper = mapper;
            this.transacciones = transacciones;
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

        //ACCIONES PARA REPORTES
        public async Task<IActionResult> Detalle(int id,int mes,int anio)
        {
            var usuarioId = servicioUsuarios.obtenerusuarioid();
            var cuenta = await repositorioCuentas.ObtenerPorId(id,usuarioId);
            if(cuenta is null)
            {
                return RedirectToAction("NoEncontrado","Home");
            }
            DateTime fechaInicio;
            DateTime fechaFin;

            if(mes <=0 || mes >12 || anio <= 1900)
            {
                var hoy = DateTime.Today;
                fechaInicio = new DateTime(hoy.Year, hoy.Month, 1);

            }
            else
            {
                fechaInicio = new DateTime(anio,mes,1);
            }
            fechaFin =fechaInicio.AddMonths(1).AddDays(-1);
            var obtenerTransaccionesPorCuenta = new ObtenerTransaccionesPorCuenta()
            {
                CuentaId = id,
                UsuarioId = usuarioId,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin
            };

            var transaccione = await transacciones.ObtenerPorCuentaId(obtenerTransaccionesPorCuenta);

            var modelo = new ReporteTransaccionesDetallada();
            ViewBag.Cuenta = cuenta.Nombre;
            var transaccionesporfecha = transaccione.OrderByDescending(x => x.FechaTransaccion)
                .GroupBy(x => x.FechaTransaccion)
                .Select(grupo => new ReporteTransaccionesDetallada.TransaccionesPorFecha()
                {
                    FechaTransaccion = grupo.Key,
                    Transacciones = grupo.AsEnumerable()
                });
            modelo.TransaccionesAgrupadas= transaccionesporfecha;
            modelo.FechaInicio= fechaInicio;
            modelo.FechaFin= fechaFin;

            ViewBag.mesAnterior = fechaInicio.AddMonths(-1).Month;
            ViewBag.anioAnterior = fechaInicio.AddMonths(-1).Year;
            ViewBag.mesPosterior = fechaInicio.AddMonths(1).Month;
            ViewBag.anioPosterior = fechaInicio.AddMonths(1).Year;
            ViewBag.urlRetorno = HttpContext.Request.Path + HttpContext.Request.QueryString;
            return View(modelo);
        }
    }
}
