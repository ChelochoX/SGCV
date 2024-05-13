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
public class ClienteController : ControllerBase
{

    private readonly IClienteService _service;

    public ClienteController(IClienteService service)
    {
        _service = service;
    }


    #region Cliente

    [HttpPost("InsertarDatosPersonalesCliente")]
    [SwaggerOperation(
       Summary = "Permite insertar datos personales correspondiente al cliente",
       Description = "Permitir insertar todos los datos correspondiente al dato personal del cliente")]
    public async Task<IActionResult> InsertarDatosPersonalesdelCliente(
     [FromBody][Description("Datos que corresponden a los datos personales del cliente")]
       ClienteDatosPersonalesInsertarRequest request)
    {  
        var validationResult = new ClienteDatosPersonalesRequestValidator().Validate(request);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var datosInsertados = await _service.InsertarSolicitudBienesCircunscripcion(request);

        if (datosInsertados != null && datosInsertados.Items != -1)
        {
            return Ok(new ApiResponse<Datos<int>>
            {
                Success = true,
                Data = datosInsertados,
                StatusCode = (int)HttpStatusCode.Created
            });
        }
        else if (datosInsertados != null && datosInsertados.Items == -1)
        {
            return BadRequest(new ApiResponse<string>
            {
                Success = false,
                Message = "El cliente ya existe. No se pudo insertar datos particulares del cliente.",
                StatusCode = (int)HttpStatusCode.BadRequest
            });
        }
        else
        {
            return BadRequest(new ApiResponse<string>
            {
                Success = false,
                Message = "No se pudo insertar datos particulares del cliente",
                StatusCode = (int)HttpStatusCode.BadRequest
            });
        }
    }

    [HttpGet("ListarDatosPersonalesCliente")]
    [SwaggerOperation(
     Summary = "Permite obtener datos personales correspondiente al cliente",
     Description = "Permitir obtener todos los datos correspondiente al dato personal del cliente")]
    public async Task<IActionResult> ListarDatosPersonalesdelCliente(
        [FromQuery][Description("Valor que indica el periodo de la solicitud")] string? cedula,
        [FromQuery][Description("Valor que indica el centro de responsabilidad")] string? ruc,
        [FromQuery][Description("Valor que indica la descripcion del objeto de gasto")] string? nombres,
        [FromQuery][Description("Valor que indica el periodo de la solicitud")] string? apellidos,
        [FromQuery][Description("Valor que indica el centro de responsabilidad")] string? telefonoMovil,
        [FromQuery][Description("Valor que indica la descripcion del objeto de gasto")] string? telefonoLineaBaja,
        [FromQuery][Description("Valor que indica el centro de responsabilidad")] string? direccionParticular,
        [FromQuery][Description("Valor que indica la descripcion del objeto de gasto")] int? numeroCasa,
        [FromQuery][Description("Valor que identifica el objeto a buscar en el filtro generico")] string? terminoDeBusqueda,

        [FromQuery][Description("Valor que identifica el periodo de la configuracion presupuestaria")] int parametroCodigoCliente,
        [FromQuery][Description("Valor que indica el inicio desde donde se obtendra el registro")] int pagina,
        [FromQuery][Description("Valor que indica la cantidad de registros a recuperar")] int cantidadRegistros)
    {
        var request = new ClienteDatosPersonalesObtenerRequest
        {
            Cedula = cedula,
            Ruc = ruc,
            Nombres = nombres,
            Apellidos = apellidos,
            TelefonoMovil = telefonoMovil,
            TelefonoLineaBaja = telefonoLineaBaja,
            DireccionParticular = direccionParticular,
            NumeroCasa = numeroCasa,
            TerminoBusqueda = terminoDeBusqueda, 
            ParametroCodigoCliente = parametroCodigoCliente,    
            Pagina = pagina,
            CantidadRegistros = cantidadRegistros
        };

        var validationResult = new ClienteDatosPersonalesObtenerRequestValidator().Validate(request);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var resultado = await _service.ObtenerDatosdeCliente(request);

        if (resultado == null)
        {
            throw new KeyNotFoundException();
        }

        return Ok(new ApiResponse<Datos<IEnumerable<ClienteDatosPersonalesResponse>>>
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
    public async Task<IActionResult> ActualizarDatosPersonalesdelCliente(
    [FromQuery][Description("Valor que indica el periodo de la solicitud")] string? cedula,
    [FromQuery][Description("Valor que indica el centro de responsabilidad")] string? ruc,
    [FromQuery][Description("Valor que indica la descripcion del objeto de gasto")] string? nombres,
    [FromQuery][Description("Valor que indica el periodo de la solicitud")] string? apellidos,
    [FromQuery][Description("Valor que indica el centro de responsabilidad")] string? telefonoMovil,
    [FromQuery][Description("Valor que indica la descripcion del objeto de gasto")] string? telefonoLineaBaja,
    [FromQuery][Description("Valor que indica el centro de responsabilidad")] string? direccionParticular,
    [FromQuery][Description("Valor que indica la descripcion del objeto de gasto")] int? numeroCasa,

    [FromQuery][Description("Valor que identifica el periodo de la configuracion presupuestaria")] int parametroCodigoCliente)
    {
        var request = new ClienteDatosPersonalesActualizarRequest
        {
            Cedula = cedula,
            Ruc = ruc,
            Nombres = nombres,
            Apellidos = apellidos,
            TelefonoMovil = telefonoMovil,
            TelefonoLineaBaja = telefonoLineaBaja,
            DireccionParticular = direccionParticular,
            NumeroCasa = numeroCasa,
            ParametroCodigoCliente = parametroCodigoCliente
        };

        var validationResult = new ClienteDatosPersonalesActualizarRequestValidator().Validate(request);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var resultado = await _service.ActualizarDatosParticularesdelCliente(request);

        if (resultado > 0 && resultado != -1)
        {
            return Ok(new ApiResponse<int>
            {
                Success = true,
                Data = resultado,
                StatusCode = (int)HttpStatusCode.NoContent
            });
        }
        else if (resultado > 0 && resultado == -1)
        {
            return BadRequest(new ApiResponse<string>
            {
                Success = false,
                Message = $"El cliente ya existe con número de Cédula {cedula}. No se puede actualizar el numero de Cédula",
                StatusCode = (int)HttpStatusCode.BadRequest
            });
        }
        else
        {          
            throw new KeyNotFoundException();
        }
           
    }      

    #endregion

}
