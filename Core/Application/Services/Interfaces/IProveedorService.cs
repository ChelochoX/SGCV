using sgcv_backend.Core.Domain.Entities;
using sgcv_backend.Core.Domain.Request;

namespace sgcv_backend.Core.Application.Services.Interfaces;

public interface IProveedorService
{
    Task<Datos<int>> InsertarDatosProveedor(ProveedorDatosRequest request);
}
