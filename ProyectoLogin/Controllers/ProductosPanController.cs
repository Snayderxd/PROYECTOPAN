using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoLogin.Models;

namespace ProyectoLogin.Controllers
{
    public class ProductosPanController : Controller
    {
        private readonly DbpruebaContext db;

        public ProductosPanController(DbpruebaContext context)
        {
            db = context;
        }

        // GET: ProductosPan
        public async Task<IActionResult> Index(string searchString)
        {
            var productos = from p in db.ProductosPan
                            select p;

            if (!string.IsNullOrEmpty(searchString))
            {
                productos = productos.Where(p => p.Nombre.Contains(searchString));
            }

            // Verifica si la solicitud es una petición AJAX
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ProductosPartial", await productos.ToListAsync());
            }

            return View(await productos.ToListAsync());
        }

        // GET: ProductosPan/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ProductosPan/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nombre,Precio,Cantidad")] ProductosPan producto)
        {
            if (ModelState.IsValid)
            {
                db.ProductosPan.Add(producto);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(producto);
        }

        // GET: ProductosPan/Edit/1
        public async Task<IActionResult> Edit(int id)
        {
            var producto = await db.ProductosPan.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }
            return View(producto);
        }

        // POST: ProductosPan/Edit/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Precio,Cantidad")] ProductosPan producto)
        {
            if (id != producto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    db.Update(producto);
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductoExists(producto.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(producto);
        }

        // GET: ProductosPan/Delete/1
        public async Task<IActionResult> Delete(int id)
        {
            var producto = await db.ProductosPan
                .FirstOrDefaultAsync(m => m.Id == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // POST: ProductosPan/Delete/1
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var producto = await db.ProductosPan.FindAsync(id);
            if (producto != null)
            {
                db.ProductosPan.Remove(producto);
                await db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(DeleteSuccess));
        }

        // GET: ProductosPan/DeleteSuccess
        public IActionResult DeleteSuccess()
        {
            ViewData["Title"] = "Eliminación Exitosa";
            return View();
        }

        // POST: ProductosPan/AgregarAlCarrito
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgregarAlCarrito(int id, int cantidad)
        {
            if (cantidad <= 0)
            {
                return BadRequest("La cantidad debe ser mayor a cero.");
            }

            // Buscar el producto en la base de datos
            var producto = await db.ProductosPan.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }

            // Obtener el ID del usuario autenticado (esto debería ajustarse a tu lógica de autenticación)
            var usuarioId = "1"; // Aquí deberías obtener el ID del usuario autenticado

            // Buscar el carrito del usuario
            var carrito = await db.Carrito
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);

            if (carrito == null)
            {
                // Crear un nuevo carrito si no existe
                carrito = new Carrito
                {
                    UsuarioId = usuarioId
                };
                db.Carrito.Add(carrito);
                await db.SaveChangesAsync();
            }

            // Verificar si el producto ya está en el carrito
            var carritoItem = carrito.Items
                .FirstOrDefault(ci => ci.ProductoId == id);

            if (carritoItem != null)
            {
                // Actualizar la cantidad si el producto ya está en el carrito
                carritoItem.Cantidad += cantidad;
            }
            else
            {
                // Agregar nuevo producto al carrito
                carritoItem = new CarritoItem
                {
                    CarritoId = carrito.CarritoId,
                    ProductoId = producto.Id,
                    Cantidad = cantidad,
                    Precio = producto.Precio
                };
                carrito.Items.Add(carritoItem);
            }

            // Actualizar la cantidad del producto en la base de datos
            producto.Cantidad -= cantidad;
            db.Update(producto);
            db.Update(carrito);
            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // POST: ProductosPan/EliminarDelCarrito
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarDelCarrito(int productoId, int carritoId)
        {
            var carrito = await db.Carrito
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.CarritoId == carritoId);

            if (carrito == null)
            {
                return NotFound();
            }

            var itemToRemove = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
            if (itemToRemove != null)
            {
                carrito.Items.Remove(itemToRemove);

                // Recuperar el producto y actualizar la cantidad disponible
                var producto = await db.ProductosPan.FindAsync(productoId);
                if (producto != null)
                {
                    producto.Cantidad += itemToRemove.Cantidad;
                    db.Update(producto);
                }

                db.Update(carrito);
                await db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(VerCarrito));
        }

        // GET: ProductosPan/VerCarrito
        public async Task<IActionResult> VerCarrito()
        {
            // Obtener el ID del usuario autenticado (esto debería ajustarse a tu lógica de autenticación)
            var usuarioId = "1"; // Aquí deberías obtener el ID del usuario autenticado

            var carrito = await db.Carrito
                .Include(c => c.Items)
                .ThenInclude(ci => ci.Producto)
                .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);

            if (carrito == null)
            {
                return View(new Carrito());
            }

            return View(carrito);
        }

        // GET: ProductosPan/RealizarPago
        public async Task<IActionResult> RealizarPago()
        {
            var usuarioId = "1"; // Aquí deberías obtener el ID del usuario autenticado

            var carrito = await db.Carrito
                .Include(c => c.Items)
                .ThenInclude(ci => ci.Producto)
                .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);

            if (carrito == null || !carrito.Items.Any())
            {
                return RedirectToAction(nameof(VerCarrito));
            }

            return View(carrito);
        }

        // POST: ProductosPan/ConfirmacionPago
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmacionPago(string metodoPago, string numeroTarjeta = null, string fechaExpiracion = null, string cvv = null)
        {
            var usuarioId = "1"; // Aquí deberías obtener el ID del usuario autenticado

            var carrito = await db.Carrito
                .Include(c => c.Items)
                .ThenInclude(ci => ci.Producto)
                .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);

            if (carrito == null || !carrito.Items.Any())
            {
                return RedirectToAction(nameof(VerCarrito));
            }

            bool pagoExitoso = false;

            if (metodoPago == "tarjeta")
            {
                // Procesar pago con tarjeta
                pagoExitoso = ProcesarPagoConTarjeta(numeroTarjeta, fechaExpiracion, cvv);
            }
            else if (metodoPago == "paypal")
            {
                // Redirigir a PayPal para completar el pago
                return Redirect("https://www.paypal.com/cgi-bin/webscr?cmd=_xclick&business=your_paypal_email&amount=total_amount&currency_code=C$");
            }
            else if (metodoPago == "transferencia")
            {
                // Lógica para transferencia bancaria
                pagoExitoso = true; // Simulación de éxito
            }

            if (!pagoExitoso)
            {
                ViewBag.Mensaje = "El pago ha fallado. Por favor, inténtelo de nuevo.";
                return View("ConfirmacionPago");
            }

            // Limpiar el carrito después del pago
            db.Carrito.Remove(carrito);
            await db.SaveChangesAsync();

            ViewBag.Mensaje = "El pago se ha realizado con éxito. ¡Gracias por tu compra!";
            return View("ConfirmacionPago");
        }

        private bool ProcesarPagoConTarjeta(string numeroTarjeta, string fechaExpiracion, string cvv)
        {
            // Lógica para procesar el pago con tarjeta
            return true; // Simulación de éxito
        }

        private bool ProductoExists(int id)
        {
            return db.ProductosPan.Any(e => e.Id == id);
        }
    }
}
