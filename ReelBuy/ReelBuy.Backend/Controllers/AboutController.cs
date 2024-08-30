using Microsoft.AspNetCore.Mvc;

namespace ReelBuy.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AboutController : ControllerBase
{
    public AboutController()
    {
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok("ReelBuy");
    }
}