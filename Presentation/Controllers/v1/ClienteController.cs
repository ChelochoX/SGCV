using Microsoft.AspNetCore.Mvc;

namespace sgcv_backend.Presentation.Controllers.v1;

[ApiController]
[Route("api/v1/[controller]")]
public class ClienteController : ControllerBase
{
    public IActionResult Index()
    {
        return View();
    }
}
