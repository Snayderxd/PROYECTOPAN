namespace ProyectoLogin.Models
{
    public class ProductosPan
    {
        public int Id { get; set; } // Identificador del producto
        public string? Nombre { get; set; } // Nombre del producto
        public decimal Precio { get; set; } // Precio del producto
        public int Cantidad { get; set; } // Cantidad disponible del producto
    }
}
