using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace ErrLogIO.AspNetCore;

public class ErrLogIOMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ErrLogIOService _errLogIO;

    public ErrLogIOMiddleware(RequestDelegate next, ErrLogIOService errLogIO)
    {
        _next = next;
        _errLogIO = errLogIO;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            await _errLogIO.LogAsync(ex, httpContext);
            throw;
        }
    }
}
