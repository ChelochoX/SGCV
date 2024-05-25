using AutoMapper;
using sgcv_backend.Core.Application.Services.Interfaces;
using sgcv_backend.Core.Application.Services.Interfaces.IRepository;
using sgcv_backend.Core.Domain.Entities;
using sgcv_backend.Core.Domain.Request;
using sgcv_backend.Core.Domain.Response;

namespace sgcv_backend.Core.Application.Services;

public class ProveedorService : IProveedorService
{
    private readonly IProveedorRepository _repository;
    private readonly IMapper _mapper;

    public ProveedorService(IMapper mapper, IProveedorRepository repository)
    {
        _mapper = mapper;
        _repository = repository;
    }

    public async Task<Datos<int>> InsertarDatosProveedor(ProveedorDatosRequest request)
    {
        return await _repository.InsertarDatosProveedor(request);   
    }

    public async Task<Datos<IEnumerable<ProveedorDatosResponse>>> ObtenerDatosdelProveedor(ProveedorObtenerDatosRequest request)
    {
        return await _repository.ObtenerDatosdelProveedor(request);
    }

    public async Task<int> ActualizarDatosProveedor(ProveedorDatosActualizarRequest request)
    {
        return await _repository.ActualizarDatosProveedor(request);
    }
}
