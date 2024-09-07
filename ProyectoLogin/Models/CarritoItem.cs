namespace ProyectoLogin.Models
{
    public class CarritoItem
    {
        public int CarritoId { get; set; }
        public Carrito Carrito { get; set; }
        public int ProductoId { get; set; }
        public ProductosPan Producto { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }

        public decimal Total => Cantidad * Precio;
    }
}
