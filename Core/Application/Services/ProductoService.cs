using AutoMapper;
using sgcv_backend.Core.Application.Services.Interfaces;
using sgcv_backend.Core.Application.Services.Interfaces.IRepository;
using sgcv_backend.Core.Domain.Entities;
using sgcv_backend.Core.Domain.Request;
using sgcv_backend.Core.Domain.Response;

namespace sgcv_backend.Core.Application.Services;

public class ProductoService : IProductoService
{
    private readonly IProductoRepository _repository;
    private readonly IMapper _mapper;

    public ProductoService (IProductoRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Datos<int>> InsertarDatosProducto(ProductoDatosRequest request)
    {
        return await _repository.InsertarDatosProducto(request);
    }

    public async Task<Datos<int>> InsertarPreciosProducto(PrecioProductoRequest request)
    {
        return await _repository.InsertarPreciosProducto(request);
    }

    public async Task<IEnumerable<CategoriasDatosResponse>> ObtenerDatosCategoria()
    {
        return await _repository.ObtenerDatosCategoria();
    }
    public async Task<IEnumerable<UnidadMedidaResponse>> ObtenerDatosUnidadMedida()
    {
       return await _repository.ObtenerDatosUnidadMedida();
    }
    public async Task<Datos<IEnumerable<ClienteDatosPersonalesResponse>>> ObtenerDatosdelProducto(ClienteDatosPersonalesObtenerRequest request)
    {
        return await _repository.ObtenerDatosdelProducto(request);
    }


  
}
