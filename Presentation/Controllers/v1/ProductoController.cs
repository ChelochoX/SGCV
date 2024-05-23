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
public class ProductoController : ControllerBase
{

    private readonly IProductoService _service;

    public ProductoController(IProductoService service)
    {
        _service = service;
    }
       

    [HttpPost("InsertarDatosNombreProducto")]
    [SwaggerOperation(
       Summary = "Permite insertar datos del producto",
       Description = "Permitir insertar todos los datos correspondiente al producto")]
    public async Task<IActionResult> InsertarDatosdelProducto(
    [FromBody][Description("Datos que corresponden a los datos del producto")]
       ProductoDatosRequest request)
    {  
        var validationResult = new ProductoDatosRequestValidator().Validate(request);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var idProducto = await _service.InsertarDatosProducto(request);

        if (idProducto != null && idProducto.Items != -1)
        {
            return Ok(new ApiResponse<Datos<int>>
            {
                Success = true,
                Data = idProducto,
                StatusCode = (int)HttpStatusCode.Created
            });
        }
        else if (idProducto != null && idProducto.Items == -1)
        {
            return BadRequest(new ApiResponse<string>
            {
                Success = false,
                Message = "El producto ya existe. No se pudo insertar datos del producto.",
                StatusCode = (int)HttpStatusCode.BadRequest
            });
        }
        else
        {
            return BadRequest(new ApiResponse<string>
            {
                Success = false,
                Message = "No se pudo insertar datos del producto",
                StatusCode = (int)HttpStatusCode.BadRequest
            });
        }
    }



    [HttpPost("InsertarDatosPrecioProducto")]
    [SwaggerOperation(
        Summary = "Permite insertar datos del precio producto",
        Description = "Permitir insertar todos los datos correspondiente al percio del producto")]
    public async Task<IActionResult> InsertarPreciodelProducto(
    [FromBody][Description("Datos que corresponden al preciop del producto")]
       PrecioProductoRequest request)
    {
        var validationResult = new PrecioProductoRequestValidator().Validate(request);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var idProducto = await _service.InsertarPreciosProducto(request);

        if (idProducto != null && idProducto.Items != -1)
        {
            return Ok(new ApiResponse<Datos<int>>
            {
                Success = true,
                Data = idProducto,
                StatusCode = (int)HttpStatusCode.Created
            });
        }
        else if (idProducto != null && idProducto.Items == -1)
        {
            return BadRequest(new ApiResponse<string>
            {
                Success = false,
                Message = "El producto ya existe. No se pudo insertar datos del producto.",
                StatusCode = (int)HttpStatusCode.BadRequest
            });
        }
        else
        {
            return BadRequest(new ApiResponse<string>
            {
                Success = false,
                Message = "No se pudo insertar datos del producto",
                StatusCode = (int)HttpStatusCode.BadRequest
            });
        }
    }


    [HttpGet("ListarDatosProducto")]
    [SwaggerOperation(
     Summary = "Permite obtener datos del producto",
     Description = "Permitir obtener todos los datos correspondiente al producto")]
    public async Task<IActionResult> ListarDatosdelProducto(
        [FromQuery][Description("Valor que indica el periodo de la solicitud")] string? codigoProducto,
        [FromQuery][Description("Valor que indica el centro de responsabilidad")] string? nombreProducto,
        [FromQuery][Description("Valor que indica la descripcion del objeto de gasto")] string? descripcionProducto,
        [FromQuery][Description("Valor que indica el periodo de la solicitud")] string? nombreCategoria,
        [FromQuery][Description("Valor que indica el centro de responsabilidad")] string? terminoBusqueda,
        [FromQuery][Description("Valor que indica la descripcion del objeto de gasto")] int pagina,
        [FromQuery][Description("Valor que indica el centro de responsabilidad")] int cantidadRegistros)
    {
        var request = new ProductoObtenerDatosRequest
        {
            CodigoProducto = codigoProducto,
            NombreProducto = nombreProducto,
            DescripcionProducto = descripcionProducto,
            NombreCategoria = nombreCategoria,
            TerminoBusqueda = terminoBusqueda,
            Pagina = pagina,
            CantidadRegistros = cantidadRegistros
        };

        //var validationResult = new ClienteDatosPersonalesObtenerRequestValidator().Validate(request);

        //if (!validationResult.IsValid)
        //{
        //    throw new ValidationException(validationResult.Errors);
        //}

        var resultado = await _service.ObtenerDatosdelProducto(request);

        if (resultado == null)
        {
            throw new KeyNotFoundException();
        }

        return Ok(new ApiResponse<Datos<IEnumerable<ProductoConPrecioResponse>>>
        {
            Success = true,
            Data = resultado,
            StatusCode = (int)HttpStatusCode.OK
        });
    }



    [HttpPut("ActualizarDatosProducto")]
    [SwaggerOperation(
        Summary = "Permite actualizar datos del producto",
        Description = "Permitir actualizar todos los datos del producto")]
    public async Task<IActionResult> ActualizarDatosProducto(
    [FromQuery][Description("Valor que indica el codigo del producto")] string codigo,
    [FromQuery][Description("Valor que indica el nombre del producto")] string nombre,
    [FromQuery][Description("Valor que indica la descripcion del producto")] string descripcion,
    [FromQuery][Description("Valor que indica el codigo de la categoria")] int codigoCategoria,
    [FromQuery][Description("Valor que indica el codigo de la unidad de medida")] int codigoUnidadMedida,
    [FromQuery][Description("Valor que indica el codigo del producto")] int codigoProducto)
    {
        var request = new ProductoDatosActualizarRequest
        {
            Codigo = codigo,
            Nombre = nombre,
            Descripcion = descripcion,
            CodigoCategoria = codigoCategoria,
            CodigoUnidadMedida = codigoUnidadMedida,
            CodigoProducto = codigoProducto
        };

        //var validationResult = new ClienteDatosPersonalesActualizarRequestValidator().Validate(request);

        //if (!validationResult.IsValid)
        //{
        //    throw new ValidationException(validationResult.Errors);
        //}

        var resultado = await _service.ActualizarDatosdelProducto(request);

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


    [HttpPut("ActualizarPrecioProducto")]
    [SwaggerOperation(
        Summary = "Permite actualizar los precios del producto",
        Description = "Permitir actualizar los datos del precio del producto")]
    public async Task<IActionResult> ActualizarPrecioProducto(
     [FromBody][Description("Datos que corresponden al precio del producto a actualizar")]
       PrecioProductoActualizarRequest request)
    {
        var validationResult = new PrecioProductoActualizarRequestValidator().Validate(request);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var resultado = await _service.ActualizarDatosdelPrecio(request);

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
