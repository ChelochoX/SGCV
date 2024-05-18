using FluentValidation;
using sgcv_backend.Core.Domain.Request;

namespace sgcv_backend.Core.Domain.Request
{
    public class PrecioProductoRequest
    {       
        public int Lista { get; set; }
        public decimal Compra { get; set; }
        public decimal Venta { get; set; }
        public decimal Iva10 { get; set; }
        public decimal Iva5 { get; set; }
        public decimal Exenta { get; set; }
        public bool Estado { get; set; }
        public string Observacion { get; set; }
        public int CodigoProducto { get; set; }
    }
}
