using System.Net;

namespace mypaperwork.Models;

public class GenericResponseData
{
    public bool Success { get; set; }
    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;
    public object Data { get; set; }
    public object Error { get; set; }
    public string Message { get; set; }
    public int? TotalRecords { get; set; }
    public int? TotalFilteredRecords { get; set; }
    public int? PageNumber { get; set; }
    public int? PageSize { get; set; }
}
