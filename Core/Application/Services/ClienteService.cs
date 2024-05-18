using AutoMapper;
using sgcv_backend.Core.Application.Services.Interfaces;
using sgcv_backend.Core.Application.Services.Interfaces.IRepository;
using sgcv_backend.Core.Domain.Entities;
using sgcv_backend.Core.Domain.Request;
using sgcv_backend.Core.Domain.Response;

namespace sgcv_backend.Core.Application.Services;

public class ClienteService : IClienteService
{
    private readonly IClienteRepository _repository;
    private readonly IMapper _mapper;

    public ClienteService(IClienteRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }
     
    public async Task<Datos<int>> InsertarDatosPersonalesCliente(ClienteDatosPersonalesInsertarRequest request)
    {
        return await _repository.InsertarDatosPersonalesCliente(request);
    }

    public async Task<Datos<IEnumerable<ClienteDatosPersonalesResponse>>> ObtenerDatosdeCliente(ClienteDatosPersonalesObtenerRequest request)
    {
        return await _repository.ObtenerDatosdeCliente(request);
    }

    public async Task<int> ActualizarDatosParticularesdelCliente(ClienteDatosPersonalesActualizarRequest request)
    {
        return await _repository.ActualizarDatosParticularesdelCliente(request);
    }


}
