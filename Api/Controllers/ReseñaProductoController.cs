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
    public class ReseñaProductoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReseñaProductoController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/ReseñaProducto
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReseñaProducto>>> Getreseñaproducto()
        {
            return await _context.reseñaproducto.ToListAsync();
        }

        // GET: api/ReseñaProducto/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ReseñaProducto>> GetReseñaProducto(int id)
        {
            var reseñaProducto = await _context.reseñaproducto.FindAsync(id);

            if (reseñaProducto == null)
            {
                return NotFound();
            }

            return reseñaProducto;
        }

        // PUT: api/ReseñaProducto/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReseñaProducto(int id, ReseñaProducto reseñaProducto)
        {
            if (id != reseñaProducto.idReseñaProducto)
            {
                return BadRequest();
            }

            _context.Entry(reseñaProducto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReseñaProductoExists(id))
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

        // POST: api/ReseñaProducto
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ReseñaProducto>> PostReseñaProducto(ReseñaProductoDTO reseñaProductoDTO)
        {
            // Verificar que el DTO recibido no sea nulo y contenga datos válidos
            if (reseñaProductoDTO == null || reseñaProductoDTO.Estrellas < 1 || reseñaProductoDTO.Estrellas > 5)
            {
                return BadRequest(new { mensaje = "La información de la reseña no es válida." });
            }

            // Crear una nueva instancia de ReseñaProducto a partir del DTO
            var reseña = new ReseñaProducto
            {
                Estrellas = reseñaProductoDTO.Estrellas,
                Fecha = DateTime.Now,
                Comentario = reseñaProductoDTO.Comentario,
                cliente_idCliente = reseñaProductoDTO.Cliente_idCliente,
                producto_idProducto = reseñaProductoDTO.Producto_idProducto
            };

            // Agregar la nueva reseña a la base de datos
            _context.reseñaproducto.Add(reseña);

            try
            {
                // Guardar los cambios en la base de datos
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Retornar error en caso de una excepción de base de datos
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    mensaje = "Ocurrió un error al guardar la reseña.",
                    error = ex.InnerException?.Message ?? ex.Message
                });
            }

            // Retornar la reseña recién creada con su ID generado
            return CreatedAtAction(nameof(GetReseñaProducto), new { id = reseña.idReseñaProducto }, reseña);

        }

        // DELETE: api/ReseñaProducto/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReseñaProducto(int id)
        {
            var reseñaProducto = await _context.reseñaproducto.FindAsync(id);
            if (reseñaProducto == null)
            {
                return NotFound();
            }

            _context.reseñaproducto.Remove(reseñaProducto);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ReseñaProductoExists(int id)
        {
            return _context.reseñaproducto.Any(e => e.idReseñaProducto == id);
        }
    }
}
