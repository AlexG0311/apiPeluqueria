﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Model
{
    [Table("detallefactura")]
    public class DetalleFactura
    {

        [Key]
        public int idDetalle { get; set; } // Asume que este campo es único y no autoincremental

        public int Cantidad { get; set; }
        public decimal Precio { get; set; } // Cambiado a double para que coincida con tu modelo de base de datos

        // Relación con Factura
        [ForeignKey("Factura")]
        public int Factura_idFactura { get; set; }
        public Factura Factura { get; set; }

        // Relación con Producto
        [ForeignKey("Producto")]
        public int Producto_idProducto { get; set; }
        public Producto Producto { get; set; }
    }
}
