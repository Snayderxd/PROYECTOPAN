using System.Collections.Generic;

namespace ProyectoLogin.Models
{
    public class Carrito
    {
        public int CarritoId { get; set; }
        public string? UsuarioId { get; set; } // Ajusta el tipo según lo que necesites
        public ICollection<CarritoItem> Items { get; set; } = new List<CarritoItem>();
    }
}
