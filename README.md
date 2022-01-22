# ErrLogIO.AspNetCore [![NuGet](https://img.shields.io/nuget/v/ErrLogIO.AspNetCore.svg)](https://www.nuget.org/packages/ErrLogIO.AspNetCore)

ASP.NET Core library for [ErrLog.IO](https://errlog.io/)

## Installation

```
> dotnet add package ErrLogIO.AspNetCore
```

```
PM> Install-Package ErrLogIO.AspNetCore
```

## Usage

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
            options.KeysToExclude = new [] {"password"};
            options.HideAllRequestValues = true;
        });
    }
}
```

```csharp

public class Startup
{
    public void Configure(IApplicationBuilder app)
    {
        app.UseErrLogIO();
    }
}
```
