﻿using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Security.Authentication;
using System.Text;
using Microsoft.AspNetCore.Http.Extensions;
using Newtonsoft.Json;
using Serilog;
using StockKAutoManagement_API.Models;

namespace StockKAutoManagement_API.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await ErrorLogging(context, ex);
        }
    }

    private async Task ErrorLogging(HttpContext httpContext, Exception ex)
    {
        HttpStatusCode statusCode = GetErrorCode(ex);
        httpContext.Response.Clear();
        httpContext.Response.ContentType = "application/json";
        httpContext.Response.StatusCode = (int)statusCode;
        var responseData = new GenericResponseData();
        responseData.Success = false;
        responseData.StatusCode = statusCode;
        responseData.Error = ex;
        var message = new StringBuilder();
        var requestId = httpContext.Request.Headers.Keys.Contains("RequestId") ?
            httpContext?.Request.Headers["RequestId"].ToString() :
            httpContext?.TraceIdentifier;

        var clientIP = httpContext.Request.Headers.Keys.Contains("ClientIP") ?
            httpContext?.Request.Headers["ClientIP"].ToString() :
            httpContext?.Connection?.RemoteIpAddress?.MapToIPv4().ToString();
        message.Append($"{clientIP}\t");
        message.Append($"{requestId}\t");
        message.Append($"{httpContext.Request.Method}\t{UriHelper.GetDisplayUrl(httpContext.Request)}\t");
        message.Append($"{httpContext.GetEndpoint()?.DisplayName}\t");

        if (!httpContext.Request.Method.Equals("GET", StringComparison.OrdinalIgnoreCase))
        {
            using (var stream = new MemoryStream())
            {
                // make sure that body is read from the beginning
                httpContext.Request.Body.Seek(0, SeekOrigin.Begin);
                await httpContext.Request.Body.CopyToAsync(stream);
                string requestBody = Encoding.UTF8.GetString(stream.ToArray());
                message.Append($"{Environment.NewLine}\tRequest Body: {requestBody}{Environment.NewLine}");
                // this is required, otherwise model binding will return null
                httpContext.Request.Body.Seek(0, SeekOrigin.Begin);
            }
        }
        message.Append($"\t{ex}");
        Log.Error(message.ToString());
        
        responseData.Message = ex.Message;
        
        await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(responseData), Encoding.UTF8);
        return;
    }

    private HttpStatusCode GetErrorCode(Exception exception)
    {
        switch (exception)
        {
            case ValidationException _:
                return HttpStatusCode.BadRequest;
            case AuthenticationException _:
                return HttpStatusCode.Forbidden;
            case NotImplementedException _:
                return HttpStatusCode.NotImplemented;
            //case NullReferenceException _:
            //    return HttpStatusCode.NotFound;
            default:
                return HttpStatusCode.InternalServerError;
        }
    }
}

