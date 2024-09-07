using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProyectoLogin.Models;
using ProyectoLogin.Servicios.Contrato;
using System.Threading.Tasks;

namespace ProyectoLogin.Servicios.Implementacion
{
    public class UsuarioService : IUsuarioService
    {
        private readonly DbpruebaContext _dbContext;
        private readonly PasswordHasher<Usuario> _passwordHasher;

        public UsuarioService(DbpruebaContext dbContext)
        {
            _dbContext = dbContext;
            _passwordHasher = new PasswordHasher<Usuario>();
        }

        public async Task<Usuario> GetUsuario(string correo, string clave)
        {
            // Buscar al usuario por correo
            Usuario usuario = await _dbContext.Usuario.FirstOrDefaultAsync(u => u.Correo == correo);

            if (usuario != null)
            {
                // Verificar la contraseña proporcionada
                var result = _passwordHasher.VerifyHashedPassword(usuario, usuario.Clave, clave);

                if (result == PasswordVerificationResult.Success)
                {
                    return usuario; // Contraseña correcta
                }
            }

            return null; // Usuario no encontrado o contraseña incorrecta
        }

        public async Task<Usuario> SaveUsuario(Usuario modelo)
        {
            // Encriptar la clave antes de guardar
            modelo.Clave = _passwordHasher.HashPassword(modelo, modelo.Clave);

            _dbContext.Usuario.Add(modelo);
            await _dbContext.SaveChangesAsync();
            return modelo;
        }
    }
}
