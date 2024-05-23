using AutoMapper;
using sgcv_backend.Core.Application.Services.Interfaces;
using sgcv_backend.Core.Application.Services.Interfaces.IRepository;
using sgcv_backend.Core.Domain.Entities;
using sgcv_backend.Core.Domain.Request;

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
}
