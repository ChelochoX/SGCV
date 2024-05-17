using sgcv_backend.Core.Domain.Entities;
using sgcv_backend.Core.Domain.Request;
using sgcv_backend.Core.Domain.Response;

namespace sgcv_backend.Core.Application.Services.Interfaces;

public interface IProductoRepository
{
    Task<Datos<int>> InsertarDatosProducto(ProductoDatosRequest request);

    Task<Datos<IEnumerable<ClienteDatosPersonalesResponse>>> ObtenerDatosdelProducto(ClienteDatosPersonalesObtenerRequest request);
}
