using ManejoPresupuesto.Models;
using ManejoPresupuesto.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace ManejoPresupuesto.Controllers
{
    public class CategoriasController : Controller
    {
        private readonly IRepositorioCategorias repositorioCategorias;
        private readonly IservicioUsuarios servicioUsuarios;

        public CategoriasController(IRepositorioCategorias repositorioCategorias,
            IservicioUsuarios servicioUsuarios) 
        {
            this.repositorioCategorias = repositorioCategorias;
            this.servicioUsuarios = servicioUsuarios;
        }

        [HttpGet]
        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Categoria categoria)
        {
            if (!ModelState.IsValid)
            {
                return View(categoria);
            }
            var usuarioid = servicioUsuarios.obtenerusuarioid();
            categoria.UsuarioId = usuarioid;
            await repositorioCategorias.Crear(categoria);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Index()
        {
            var usuarioid = servicioUsuarios.obtenerusuarioid();
            var categorias = await repositorioCategorias.Obtener(usuarioid);
            return View(categorias);
        }

        public async Task<IActionResult> Editar(int id)
        {
            var usuarioid = servicioUsuarios.obtenerusuarioid();
            var categoria = await repositorioCategorias.ObtenerporId(id, usuarioid);
            if(categoria is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            return View(categoria);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(Categoria categoriaeditar)
        {
            if (!ModelState.IsValid)
            {
                return View(categoriaeditar);
            }
            var usuarioid = servicioUsuarios.obtenerusuarioid();
            var categoria = await repositorioCategorias.ObtenerporId(categoriaeditar.Id, usuarioid);
            if(categoria is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            categoriaeditar.UsuarioId= usuarioid;
            await repositorioCategorias.Actualizar(categoriaeditar);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Borrar(int id)
        {
            var usuarioid = servicioUsuarios.obtenerusuarioid();
            var categoria = await repositorioCategorias.ObtenerporId(id, usuarioid);
            if(categoria is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            return View(categoria);
        }

        [HttpPost]
        public async Task<IActionResult> BorrarCategoria(int id)
        {
            var usuarioid = servicioUsuarios.obtenerusuarioid();
            var categoria = await repositorioCategorias.ObtenerporId(id, usuarioid);
            if(categoria is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            await repositorioCategorias.Borrar(id);
            return RedirectToAction("Index");
        }
    }
}
