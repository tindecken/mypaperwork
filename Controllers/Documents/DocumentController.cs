using Microsoft.AspNetCore.Mvc;
using mypaperwork.Middlewares;
using mypaperwork.Models;
using mypaperwork.Models.Filter;
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

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> UploadFiles()
    {
        var formCollection = await Request.ReadFormAsync();
        var files = formCollection.Files;
        var paperWorkId = formCollection["paperWorkId"];
        if (string.IsNullOrEmpty(paperWorkId))
        {
            return BadRequest("PaperworkId is empty!");
        }

        if (files.Any(f => f.Length == 0))
        {
            return BadRequest();
        }
        var response = await _documentServices.Upload(paperWorkId, files);
        return Transform(response);
    }
    [Authorize]
    [HttpGet("{documentId:length(26)}")]
    public async Task<IActionResult> GetDocumentsById(string documentId)
    {
        var response = await _documentServices.GetDocumentById(documentId);
        return Transform(response);
    }
}

