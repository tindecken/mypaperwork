using Microsoft.AspNetCore.Mvc;
using mypaperwork.Middlewares;
using mypaperwork.Models;
using mypaperwork.Services.Document;
using mypaperwork.Services.Logging;
using mypaperwork.Services.Testing;

namespace mypaperwork.Controllers.Testing;

[ApiController]
[Route("[controller]")]
public class Document : TransformResponse
{
    private readonly DocumentServices _documentServices;
    public Document(DocumentServices documentServices)
    {
        _documentServices = documentServices;

    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> UploadFiles()
    {
        var formCollection = await Request.ReadFormAsync();
        var files = formCollection.Files;
        var paperWorkGUID = formCollection["paperWorkGUID"];
        if (string.IsNullOrEmpty(paperWorkGUID))
        {
            return BadRequest("PaperworkGUID is empty!");
        }

        if (files.Any(f => f.Length == 0))
        {
            return BadRequest();
        }
        var response = await _documentServices.Upload(paperWorkGUID, files);
        return Transform(response);
    }
}

