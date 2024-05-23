using sgcv_backend.Core.Domain.Entities;
using sgcv_backend.Core.Domain.Request;

namespace sgcv_backend.Core.Application.Services.Interfaces.IRepository
{
    public interface IProveedorRepository
    {
        Task<Datos<int>> InsertarDatosProveedor(ProveedorDatosRequest request);
    }
}
