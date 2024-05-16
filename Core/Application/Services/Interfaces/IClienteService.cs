using sgcv_backend.Core.Domain.Entities;
using sgcv_backend.Core.Domain.Request;
using sgcv_backend.Core.Domain.Response;

namespace sgcv_backend.Core.Application.Services.Interfaces;

public interface IClienteService
{

    #region Clientes
    Task<Datos<int>> InsertarDatosPersonalesCliente(ClienteDatosPersonalesInsertarRequest request);
    Task<Datos<IEnumerable<ClienteDatosPersonalesResponse>>> ObtenerDatosdeCliente(ClienteDatosPersonalesObtenerRequest request);
    Task<int> ActualizarDatosParticularesdelCliente(ClienteDatosPersonalesActualizarRequest request);

    #endregion
}
