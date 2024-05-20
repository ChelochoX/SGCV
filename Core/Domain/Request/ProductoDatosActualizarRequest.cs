using FluentValidation;
using sgcv_backend.Core.Domain.Request;

namespace sgcv_backend.Core.Domain.Request
{
    public class ProductoDatosActualizarRequest
    {        
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int CodigoCategoria { get; set; }
        public int CodigoUnidadMedida { get; set; }
        public int CodigoProducto { get; set; }
    }
}
