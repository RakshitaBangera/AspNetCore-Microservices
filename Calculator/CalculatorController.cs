using Microsoft.AspNetCore.Mvc;

namespace CalculatorController;

[ApiController]
[Route("api")]
public class CalculatorController : ControllerBase
{
    [HttpGet("add")]
    public IActionResult Add(int a, int b)
    {
        return Ok(a + b);
    }
    [HttpGet("sub")]
    public IActionResult Sub(int a, int b)
    {
        return Ok(a - b);
    }
}