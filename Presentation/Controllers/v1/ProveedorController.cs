using Microsoft.AspNetCore.Mvc;
using sgcv_backend.Core.Application.Services.Interfaces;

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


}
