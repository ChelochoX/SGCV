using sgcv_backend.Core.Domain.Entities;
using sgcv_backend.Core.Domain.Request;
using sgcv_backend.Core.Domain.Response;

namespace sgcv_backend.Core.Application.Services.Interfaces.IRepository;

public interface IProductoRepository
{
    Task<Datos<int>> InsertarDatosProducto(ProductoDatosRequest request);
    Task<Datos<IEnumerable<ProductoConPrecioResponse>>> ObtenerDatosdelProducto(ProductoObtenerDatosRequest request);
    Task<IEnumerable<CategoriasDatosResponse>> ObtenerDatosCategoria();
    Task<IEnumerable<UnidadMedidaResponse>> ObtenerDatosUnidadMedida();
    Task<Datos<int>> InsertarPreciosProducto(PrecioProductoRequest request);
    Task<int> ActualizarDatosdelProducto(ProductoDatosActualizarRequest request);
    Task<int> ActualizarDatosdelPrecioProducto(PrecioProductoActualizarRequest request);
}