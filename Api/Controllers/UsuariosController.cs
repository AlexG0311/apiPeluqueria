﻿using Api.Context;
using Api.Dtos;
using Api.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsuariosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Usuarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> Getusuario()
        {
            return await _context.usuario.ToListAsync();
        }

        // GET: api/Usuarios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioEditDTO>> GetUsuarioById(int id)
        {
            var usuario = await _context.usuario
                .Where(u => u.idUsuario == id)
                .Select(u => new UsuarioEditDTO
                {
                    idUsuario = u.idUsuario,
                    Nombre = u.Nombre,
                    Apellidos = u.Apellidos,
                    Correo = u.Correo,
                    Contrasena = u.Contrasena,
                    Telefono = u.Telefono
                })
                .FirstOrDefaultAsync();

            if (usuario == null)
            {
                return NotFound();
            }

            return Ok(usuario);
        }


        // ---------------


        // --------------
        [HttpPost]
        public async Task<ActionResult<Usuario>> PostUsuario(UsuarioDTO usuarioDTO)
        {
            // Crear un nuevo usuario con los datos del DTO
            var usuario = new Usuario
            {
                Nombre = usuarioDTO.Nombre,
                Apellidos = usuarioDTO.Apellidos,
                Correo = usuarioDTO.Correo,
                Contrasena = usuarioDTO.Contrasena,
                Telefono = usuarioDTO.Telefono
            };

            // Agregar el usuario a la base de datos
            _context.usuario.Add(usuario);
            await _context.SaveChangesAsync();

            // Crear el cliente asociado al usuario recién creado
            var cliente = new Cliente
            {
                Usuario_idUsuario = usuario.idUsuario
            };
            _context.cliente.Add(cliente);

            // Asignar el rol de "Cliente" en la tabla de unión Rolusuario
            var rolCliente = await _context.rol.FirstOrDefaultAsync(r => r.rolname == "Cliente");
            if (rolCliente != null)
            {
                var rolUsuario = new Rolusuario
                {
                    Rol_idRol = rolCliente.idRol,
                    usuario_idUsuario = usuario.idUsuario
                };
                _context.rolusuario.Add(rolUsuario);
            }

            // Guardar todos los cambios
            await _context.SaveChangesAsync();

            // Retornar la información del usuario registrado
            return CreatedAtAction("GetUsuario", new { id = usuario.idUsuario }, usuario);
        }


        // PUT: api/Usuarios/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUsuario(int id, UsuarioEditDTO usuarioEditDTO)
        {
            if (id != usuarioEditDTO.idUsuario)
            {
                return BadRequest("El ID del usuario no coincide.");
            }

            var usuario = await _context.usuario.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            usuario.Nombre = usuarioEditDTO.Nombre;
            usuario.Apellidos = usuarioEditDTO.Apellidos;
            usuario.Correo = usuarioEditDTO.Correo;
            usuario.Contrasena = usuarioEditDTO.Contrasena;
            usuario.Telefono = usuarioEditDTO.Telefono;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.usuario.Any(e => e.idUsuario == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // En el controlador UsuariosController
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            // Validar el objeto recibido
            if (loginDto == null || string.IsNullOrEmpty(loginDto.Correo) || string.IsNullOrEmpty(loginDto.Contrasena))
            {
                return BadRequest(new { mensaje = "Correo y contraseña son obligatorios" });
            }

            // Buscar el usuario por correo y contraseña
            var usuario = await _context.usuario
                .FirstOrDefaultAsync(u => u.Correo == loginDto.Correo && u.Contrasena == loginDto.Contrasena);

            // Si el usuario no existe, devolver un error
            if (usuario == null)
            {
                return NotFound(new { mensaje = "Usuario no encontrado" });
            }

            // Verificar si es cliente
            var cliente = await _context.cliente
                .FirstOrDefaultAsync(c => c.Usuario_idUsuario == usuario.idUsuario);

            // Verificar si es empleado
            var empleado = await _context.empleado
                .FirstOrDefaultAsync(e => e.Usuario_idUsuario == usuario.idUsuario);

            // Determinar el tipo de usuario y devolver la información correspondiente
            if (empleado != null)
            {
                return Ok(new
                {
                    TipoUsuario = "Empleado",
                    IdUsuario = usuario.idUsuario,
                    IdCliente = (int?)null, // No aplica
                    IdEmpleado = empleado.idEmpleado,
                    Nombre = usuario.Nombre,
                    Apellidos = usuario.Apellidos,
                    Telefono = usuario.Telefono
                });
            }
            else if (cliente != null)
            {
                return Ok(new
                {
                    TipoUsuario = "Cliente",
                    IdUsuario = usuario.idUsuario,
                    IdCliente = cliente.idCliente,
                    IdEmpleado = (int?)null, // No aplica
                    Nombre = usuario.Nombre,
                    Apellidos = usuario.Apellidos,
                    Telefono = usuario.Telefono
                });
            }
            else
            {
                return BadRequest(new { mensaje = "Tipo de usuario desconocido" });
            }
        }



        // POST: api/Usuarios/AddEmpleado
        [HttpPost("AddEmpleado")]
        public async Task<ActionResult<Usuario>> PostEmpleado(EmpleadoDTO empleadoDTO)
        {
            // Crear un nuevo usuario con los datos proporcionados en el DTO
            var usuario = new Usuario
            {
                Nombre = empleadoDTO.Nombre,
                Apellidos = empleadoDTO.Apellidos,
                Correo = empleadoDTO.Correo,
                Contrasena = empleadoDTO.Contrasena,
                Telefono = empleadoDTO.Telefono
            };

            // Agregar el usuario a la base de datos
            _context.usuario.Add(usuario);
            await _context.SaveChangesAsync();

            // Crear el empleado asociado al usuario recién creado
            var empleado = new Empleado
            {
                Usuario_idUsuario = usuario.idUsuario
            };
            _context.empleado.Add(empleado);

            // Asignar el rol de "Empleado" en la tabla de unión Rolusuario
            var rolEmpleado = await _context.rol.FirstOrDefaultAsync(r => r.rolname == "Empleado");
            if (rolEmpleado != null)
            {
                var rolUsuario = new Rolusuario
                {
                    Rol_idRol = rolEmpleado.idRol,
                    usuario_idUsuario = usuario.idUsuario
                };
                _context.rolusuario.Add(rolUsuario);
            }

            // Guardar todos los cambios
            await _context.SaveChangesAsync();

            // Retornar la información del usuario registrado
            return CreatedAtAction("GetUsuario", new { id = usuario.idUsuario }, usuario);
        }











        // DELETE: api/Usuarios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var usuario = await _context.usuario.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            _context.usuario.Remove(usuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UsuarioExists(int id)
        {
            return _context.usuario.Any(e => e.idUsuario == id);
        }
    }
}
