# ErrLogIO.AspNetCore [![NuGet](https://img.shields.io/nuget/v/ErrLogIO.AspNetCore.svg)](https://www.nuget.org/packages/ErrLogIO.AspNetCore)

ASP.NET Core library for [ErrLog.IO](https://errlog.io/)

## Installation

.NET CLI
```
dotnet add package ErrLogIO.AspNetCore
```

Package Manager
```
Install-Package ErrLogIO.AspNetCore
```

## Configuration

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddErrLogIO("API-KEY");
    }
}
```

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddErrLogIO(options =>
        {
            options.ApiKey = "API-KEY";
            options.AppName = "MyApp";
            options.Language = Language.CSharp;
            options.KeysToExclude = new [] {"password"};
            options.HideAllRequestValues = true;
        });
    }
}
```

### appsettings.json

```json
{
  "ErrLogIO": {
    "ApiKey": "API_KEY"
  }
}
```

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.Configure<ErrLogIOOptions>(
            Configuration.GetSection("ErrLogIO"));
        services.AddErrLogIO();
    }
}
```

## Logging exceptions

To log every uncaught exception to ErrLog.IO, call `UseErrLogIO` in the Configure method. Please make sure to call the `UseErrLogIO` after installation of other pieces of middleware handling exceptions and auth, but **before** any calls to `UseEndpoints`, `UseMvc` etc.

```csharp
public class Startup
{
    public void Configure(IApplicationBuilder app)
    {
        app.UseProblemDetails();
        app.UseErrLogIO();
        
        ...
        ...
        ...
        
        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
```

## Dependency injection

```csharp
public class StorageController : ControllerBase
{
    private readonly IStorageService _storageService;
    private readonly ErrLogIOService _errLogIO;

    public StorageController(IStorageService storageService, ErrLogIOService errLogIO)
    {
        _storageService = storageService;
        _errLogIO = errLogIO;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadFileAsync(CancellationToken ct)
    {
        try
        {
            var stream = Request.BodyReader.AsStream();
            await _storageService.UploadFileAsync(stream, ct);

            return Ok();
        }
        catch (Exception ex)
        {
            await _errLogIO.LogExceptionAsync(
                ex, Request.HttpContext, cancellationToken: ct);

            return BadRequest(ex.Message);
        }
    }
}
```