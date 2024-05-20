using FluentValidation;
using sgcv_backend.Core.Domain.Request;

namespace sgcv_backend.Core.Domain.Request
{
    public class ProductoObtenerDatosRequest
    {
        public string? CodigoProducto { get; set; }
        public string? NombreProducto { get; set; }
        public string? DescripcionProducto { get; set; }
        public string? NombreCategoria { get; set; }

        public string? TerminoBusqueda { get; set; }
        public int Pagina { get; set; }
        public int CantidadRegistros { get; set; }
    }
}
