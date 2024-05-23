using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using sgcv_backend.Core.Application.Services.Interfaces;
using sgcv_backend.Core.Domain.Entities;
using sgcv_backend.Core.Domain.Request;
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
}
