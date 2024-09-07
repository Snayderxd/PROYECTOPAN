using System.Collections.Generic;

namespace ProyectoLogin.Models
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string? NombreUsuario { get; set; }
        public string? Correo { get; set; }
        public string? Clave { get; set; }
        public bool EstaSuspendido { get; set; }
    }
}
