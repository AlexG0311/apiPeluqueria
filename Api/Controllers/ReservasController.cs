using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Api.Context;
using Api.Model;
using Api.Dtos;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReservasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Reservas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reserva>>> GetReserva()
        {
            return await _context.Reserva.ToListAsync();
        }

        [HttpGet("Empleado/{idEmpleado}/Reservas")]
        public async Task<IActionResult> GetReservasByEmpleado(int idEmpleado)
        {
            var reservas = await _context.Reserva
                .Where(r => r.Empleado_idEmpleado == idEmpleado)
                .Select(r => new
                {
                    r.idReserva,
                    r.Fecha,
                    r.Hora,
                    ClienteNombre = r.Cliente.Usuario.Nombre, // Suponiendo que hay una relación con la entidad Cliente
                    ClienteApellido = r.Cliente.Usuario.Apellidos,
                    r.Estado_idEstado, // ID del estado
                    EstadoDescripcion = r.Estado.Descripcion // Descripción del estado (si está relacionada)
                })
                .ToListAsync();

            if (reservas == null || !reservas.Any())
            {
                return NotFound(new { mensaje = "No se encontraron reservas para este empleado." });
            }

            return Ok(reservas);
        }



        // PUT: api/Reservas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReserva(int id, Reserva reserva)
        {
            if (id != reserva.idReserva)
            {
                return BadRequest();
            }

            _context.Entry(reserva).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReservaExists(id))
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

        // POST: api/Reservas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> CrearReserva([FromBody] ReservaDTO crearReservaDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Crea una nueva instancia del modelo Reserva usando los datos del DTO
            var nuevaReserva = new Reserva
            {
                Fecha = crearReservaDto.Fecha,
                Hora = crearReservaDto.Hora,
                Cliente_idCliente = crearReservaDto.Cliente_idCliente,
                Servicio_idServicio = crearReservaDto.Servicio_idServicio,
                Empleado_idEmpleado = crearReservaDto.Empleado_idEmpleado,
                Estado_idEstado = crearReservaDto.Estado_idEstado
            };

            // Guarda la nueva reserva en la base de datos
            _context.Reserva.Add(nuevaReserva);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(ObtenerReservaPorId), new { id = nuevaReserva.idReserva }, nuevaReserva);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerReservaPorId(int id)
        {
            var reserva = await _context.Reserva
                .Include(r => r.Empleado)
                .Include(r => r.Servicio)
                .FirstOrDefaultAsync(r => r.idReserva == id);

            if (reserva == null)
            {
                return NotFound();
            }

            return Ok(reserva);
        }



        // DELETE: api/Reservas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReserva(int id)
        {
            var reserva = await _context.Reserva.FindAsync(id);
            if (reserva == null)
            {
                return NotFound();
            }

            _context.Reserva.Remove(reserva);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ReservaExists(int id)
        {
            return _context.Reserva.Any(e => e.idReserva == id);
        }

        [HttpGet("EmpleadosDisponibles")]
        public async Task<IActionResult> GetEmpleadosDisponibles(DateTime fecha, TimeSpan hora, int idServicio)
        {
            // Obtener el servicio y verificar si existe
            var servicio = await _context.Servicio.FindAsync(idServicio);
            if (servicio == null)
            {
                return NotFound(new { mensaje = "El servicio no existe" });
            }

           
            var empleados = await _context.empleado
                .Where(e => !e.Reservas.Any(r =>
                    r.Fecha == fecha &&
                    hora >= r.Hora &&
                    hora < r.Hora + servicio.Duracion)) // Usamos servicio.Duracion
                .Select(e => new EmpleadoDTO
                {
                    idEmpleado = e.idEmpleado,
                    Nombre = e.Usuario.Nombre,
                    Apellidos = e.Usuario.Apellidos
                })
                .ToListAsync();

            return Ok(empleados);
        }

    }
}
