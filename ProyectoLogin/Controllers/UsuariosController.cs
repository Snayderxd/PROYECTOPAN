using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ProyectoLogin.Models;
using System.Linq;

namespace ProyectoLogin.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly DbpruebaContext _context;
        private readonly PasswordHasher<Usuario> _passwordHasher;

        public UsuariosController(DbpruebaContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<Usuario>();
        }

        // Acción para listar todos los usuarios
        public IActionResult Index()
        {
            var usuarios = _context.Usuario.ToList();
            return View(usuarios);
        }

        // Acción para mostrar el formulario de creación de usuario
        public IActionResult Create()
        {
            return View();
        }

        // Acción para manejar la creación de un nuevo usuario
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(usuario.Clave))
                {
                    usuario.Clave = _passwordHasher.HashPassword(usuario, usuario.Clave);
                }
                _context.Usuario.Add(usuario);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }

        // Acción para mostrar el formulario de edición de usuario
        public IActionResult Edit(int id)
        {
            var usuario = _context.Usuario.Find(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }

        // Acción para manejar la edición de un usuario existente
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                var usuarioExistente = _context.Usuario.Find(usuario.IdUsuario);
                if (usuarioExistente != null)
                {
                    if (!string.IsNullOrEmpty(usuario.Clave))
                    {
                        usuarioExistente.Clave = _passwordHasher.HashPassword(usuarioExistente, usuario.Clave);
                    }

                    usuarioExistente.NombreUsuario = usuario.NombreUsuario;
                    usuarioExistente.Correo = usuario.Correo;
                    usuarioExistente.EstaSuspendido = usuario.EstaSuspendido;

                    _context.Usuario.Update(usuarioExistente);
                    _context.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
                return NotFound(); // Usuario no encontrado
            }
            return View(usuario);
        }

        // Acción para mostrar el formulario de suspensión de usuario
        public IActionResult Suspend(int id)
        {
            var usuario = _context.Usuario.Find(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }

        // Acción para manejar la suspensión de un usuario
        [HttpPost, ActionName("Suspend")]
        [ValidateAntiForgeryToken]
        public IActionResult SuspendConfirmed(int id)
        {
            var usuario = _context.Usuario.Find(id);
            if (usuario != null)
            {
                usuario.EstaSuspendido = true;
                _context.Usuario.Update(usuario);
                _context.SaveChanges();
                return RedirectToAction(nameof(SuspendConfirmed));
            }
            return RedirectToAction(nameof(Index));
        }

        // Acción para mostrar la vista de confirmación de suspensión
        public IActionResult SuspendConfirmed()
        {
            ViewData["Message"] = "Suspensión exitosa";
            return View();
        }

        // Acción para mostrar el formulario de reactivación de usuario
        public IActionResult Reactivate(int id)
        {
            var usuario = _context.Usuario.Find(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }

        // Acción para manejar la reactivación de un usuario
        [HttpPost, ActionName("Reactivate")]
        [ValidateAntiForgeryToken]
        public IActionResult ReactivateConfirmed(int id)
        {
            var usuario = _context.Usuario.Find(id);
            if (usuario != null)
            {
                usuario.EstaSuspendido = false;
                _context.Usuario.Update(usuario);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
