using Microsoft.AspNetCore.Mvc;
using System.Net;
using mypaperwork.Models;

namespace mypaperwork.Middlewares;

public class TransformResponse: ControllerBase
{
    [ApiExplorerSettings(IgnoreApi=true)]
    public IActionResult Transform(GenericResponseData data)
    {
        return TransformData(data.StatusCode, data);
    }
        
    private IActionResult TransformData(HttpStatusCode statusCode, object data)
    {
        switch (statusCode)
        {
            case HttpStatusCode.OK:
                return Ok(data);
            case HttpStatusCode.NotFound:
                return NotFound(data);
            case HttpStatusCode.BadRequest:
                return BadRequest(data);
            case HttpStatusCode.NoContent:
                return NoContent();
            case HttpStatusCode.InternalServerError:
                return StatusCode(500, data);
            default:
                return StatusCode((int)statusCode, data);
        }
    }
}
    
