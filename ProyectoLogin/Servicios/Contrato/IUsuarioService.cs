using Microsoft.EntityFrameworkCore;
using ProyectoLogin.Models;

namespace ProyectoLogin.Servicios.Contrato
{
    public interface IUsuarioService
    {
        Task<Usuario> SaveUsuario(Usuario usuario);
        Task<Usuario> GetUsuario(string correo, string clave);

    }
}
