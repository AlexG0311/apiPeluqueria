using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Api.Context;
using Api.Model;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacturasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FacturasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Facturas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Factura>>> Getfacturas()
        {
            return await _context.facturas.ToListAsync();
        }

        // GET: api/Facturas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Factura>> GetFactura(int id)
        {
            var factura = await _context.facturas.FindAsync(id);

            if (factura == null)
            {
                return NotFound();
            }

            return factura;
        }

        // PUT: api/Facturas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFactura(int id, Factura factura)
        {
            if (id != factura.IdFactura)
            {
                return BadRequest();
            }

            _context.Entry(factura).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FacturaExists(id))
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

        // POST: api/Facturas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // POST: api/Facturas
        [HttpPost]
        public async Task<ActionResult<FacturaResponse>> PostFactura(FacturaDTO facturaDto)
        {
            // Validar la entrada
            if (facturaDto == null || facturaDto.Detalles == null || !facturaDto.Detalles.Any())
            {
                return BadRequest("La factura no contiene detalles.");
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Crear la factura
                    var factura = new Factura
                    {
                        FechaFactura = facturaDto.FechaFactura,
                        Total = facturaDto.Total
                    };

                    _context.facturas.Add(factura);
                    await _context.SaveChangesAsync(); // Guarda para generar el ID de la factura

                    // Procesar cada detalle
                    foreach (var detalleDto in facturaDto.Detalles)
                    {
                        var producto = await _context.producto.FindAsync(detalleDto.ProductoId);

                        if (producto == null)
                        {
                            return BadRequest($"El producto con ID {detalleDto.ProductoId} no existe.");
                        }

                        if (producto.Stok < detalleDto.Cantidad)
                        {
                            return BadRequest($"El producto {producto.Nombre} no tiene suficiente stock.");
                        }

                        // Crear el detalle de la factura
                        var detalle = new DetalleFactura
                        {
                            idDetalle = factura.IdFactura,
                            Producto_idProducto = detalleDto.ProductoId,
                            Cantidad = detalleDto.Cantidad,
                            Precio = detalleDto.Precio
                        };

                        // Actualizar el stock del producto
                        producto.Stok -= detalleDto.Cantidad;

                        _context.detallefactura.Add(detalle);
                    }

                    // Guardar todos los cambios
                    await _context.SaveChangesAsync();

                    // Confirmar la transacción
                    await transaction.CommitAsync();

                    // Crear la respuesta
                    var facturaResponse = new FacturaResponse
                    {
                        IdFactura = factura.IdFactura,
                        FechaFactura = factura.FechaFactura,
                        Total = factura.Total,
                        Detalles = facturaDto.Detalles.Select(detalle => new DetalleFacturaResponse
                        {
                            ProductoId = detalle.ProductoId,
                            NombreProducto = _context.producto.FirstOrDefault(p => p.idProducto == detalle.ProductoId)?.Nombre,
                            Cantidad = detalle.Cantidad,
                            Precio = detalle.Precio
                        }).ToList()
                    };

                    return Ok(facturaResponse);
                }
                catch (Exception ex)
                {
                    // En caso de error, revertir la transacción
                    await transaction.RollbackAsync();
                    return StatusCode(500, $"Error al procesar la factura: {ex.Message}");
                }
            }
        }


        // DELETE: api/Facturas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFactura(int id)
        {
            var factura = await _context.facturas.FindAsync(id);
            if (factura == null)
            {
                return NotFound();
            }

            _context.facturas.Remove(factura);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FacturaExists(int id)
        {
            return _context.facturas.Any(e => e.IdFactura == id);
        }
    }
}
