using sgcv_backend.Core.Domain.Entities;
using sgcv_backend.Core.Domain.Request;
using sgcv_backend.Core.Domain.Response;

namespace sgcv_backend.Core.Application.Services.Interfaces.IRepository
{
    public interface IProveedorRepository
    {
        Task<Datos<int>> InsertarDatosProveedor(ProveedorDatosRequest request);
        Task<Datos<IEnumerable<ProveedorDatosResponse>>> ObtenerDatosdelProveedor(ProveedorObtenerDatosRequest request);
        Task<int> ActualizarDatosProveedor(ProveedorDatosActualizarRequest request);
    }
}
