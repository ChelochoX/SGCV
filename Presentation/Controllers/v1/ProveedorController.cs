using Domain.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using sgcv_backend.Core.Application.Services.Interfaces;
using sgcv_backend.Core.Domain.Entities;
using sgcv_backend.Core.Domain.Request;
using sgcv_backend.Core.Domain.Response;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel;
using System.Net;

namespace sgcv_backend.Presentation.Controllers.v1;


[ApiController]
[Route("api/v1/[controller]")]
public class ProveedorController : Controller
{

    private readonly IProveedorService _service;

    public ProveedorController(IProveedorService service)
    {
        _service = service;
    }

    [HttpPost("InsertarDatosNombreProveedor")]
    [SwaggerOperation(
    Summary = "Permite insertar datos del proveedor",
    Description = "Permitir insertar todos los datos correspondiente al proveedor")]
    public async Task<IActionResult> InsertarDatosdelProveedor(
         [FromBody][Description("Datos que corresponden a los datos del proveedor")]
               ProveedorDatosRequest request)
    {
        
        var idProveedor = await _service.InsertarDatosProveedor(request);

        if (idProveedor != null && idProveedor.Items != -1)
        {
            return Ok(new ApiResponse<Datos<int>>
            {
                Success = true,
                Data = idProveedor,
                StatusCode = (int)HttpStatusCode.Created
            });
        }
        else if (idProveedor != null && idProveedor.Items == -1)
        {
            return BadRequest(new ApiResponse<string>
            {
                Success = false,
                Message = "El Proveedor ya existe. No se pudo insertar datos del proveedor.",
                StatusCode = (int)HttpStatusCode.BadRequest
            });
        }
        else
        {
            return BadRequest(new ApiResponse<string>
            {
                Success = false,
                Message = "No se pudo insertar datos del proveedor",
                StatusCode = (int)HttpStatusCode.BadRequest
            });
        }
    }



    [HttpGet("ListarDatosPersonalesCliente")]
    [SwaggerOperation(
     Summary = "Permite obtener datos del proveedor",
     Description = "Permitir obtener todos los datos correspondiente al proveedor")]
    public async Task<IActionResult> ListarDatosdelProveedor(      
        [FromQuery][Description("Valor que indica el valor del ruc")] string? ruc,
        [FromQuery][Description("Valor que indica la razon social del proveedor")] string? razonsocial,
        [FromQuery][Description("Valor que indica el centro de responsabilidad")] string? direccion,
        [FromQuery][Description("Valor que indica el telefono")] string? telefono,
        [FromQuery][Description("Valor que identifica el objeto a buscar en el filtro generico")] string? terminoDeBusqueda,
    
        [FromQuery][Description("Valor que indica el inicio desde donde se obtendra el registro")] int pagina,
        [FromQuery][Description("Valor que indica la cantidad de registros a recuperar")] int cantidadRegistros)
    {
        var request = new ProveedorObtenerDatosRequest
        {            
            Ruc = ruc,
            RazonSocial = razonsocial,       
            Direccion = direccion,
            Telefono = telefono,
            TerminoBusqueda = terminoDeBusqueda,              
            Pagina = pagina,
            CantidadRegistros = cantidadRegistros
        };       

        var resultado = await _service.ObtenerDatosdelProveedor(request);

        if (resultado == null)
        {
            throw new KeyNotFoundException();
        }

        return Ok(new ApiResponse<Datos<IEnumerable<ProveedorDatosResponse>>>
        {
            Success = true,
            Data = resultado,
            StatusCode = (int)HttpStatusCode.OK
        });
    }


    [HttpPut("ActualizarDatosPersonalesCliente")]
    [SwaggerOperation(
      Summary = "Permite actualizar datos personales correspondiente al cliente",
      Description = "Permitir actualizar todos los datos correspondiente al dato personal del cliente")]
    public async Task<IActionResult> ActualizarDatosProveedor(
      [FromQuery][Description("Valor que indica el ruc")] string? ruc,
      [FromQuery][Description("Valor que indica la razon social")] string? razonSocial,
      [FromQuery][Description("Valor que indica la direccion")] string? direccion,
      [FromQuery][Description("Valor que indica el telefono")] string? telefono,
      [FromQuery][Description("Valor que indica el codigo proveedor")] int codigoProveedor)
    {
        var request = new ProveedorDatosActualizarRequest
        {          
            Ruc = ruc,
            RazonSocial = razonSocial,
            Direccion = direccion,
            Telefono = telefono,
            CodigoProveedor = codigoProveedor
        };

        var validationResult = new ProveedorActualizarRequestValidator().Validate(request);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var resultado = await _service.ActualizarDatosProveedor(request);

        if (resultado > 0)
        {
            return Ok(new ApiResponse<int>
            {
                Success = true,
                Data = resultado,
                StatusCode = (int)HttpStatusCode.NoContent
            });
        }       
        else
        {
            throw new KeyNotFoundException();
        }

    }

}
