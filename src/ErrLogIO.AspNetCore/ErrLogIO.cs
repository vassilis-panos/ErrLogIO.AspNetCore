using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ErrLogIO.AspNetCore;

public class ErrLogIO
{
    private readonly HttpClient _httpClient;
    private readonly ErrLogIOOptions _options;
    private readonly ILogger<ErrLogIO> _logger;
    private readonly string[] _keysToExclude;
    private readonly bool _hideAllRequestValues;

    public ErrLogIO(
        HttpClient httpClient, 
        IOptions<ErrLogIOOptions> options, 
        ILogger<ErrLogIO> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;

        _keysToExclude = 
            _options.KeysToExclude ?? Array.Empty<string>();

        _hideAllRequestValues = _options.HideAllRequestValues;
    }

    public async Task LogAsync(
        Exception exception, 
        HttpContext? httpContext = default,
        string? pageName = default, 
        object? customData = default,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(_options.ApiKey))
                throw new ArgumentException("Api key is null or empty");

            var stackTrace = new StackTrace(exception, true);
            var appName =
                _options.AppName ?? AppDomain.CurrentDomain.FriendlyName.ToString();
            var assemblyVersion =
                GetType()?.Assembly?.GetName()?.Version?.ToString();
            var language = (int)(_options.Language ?? Language.CSharp);

            var requestBody = new WebhookParameters(
                _options.ApiKey, exception.Message)
            {
                Type = exception.GetType().ToString(),
                ApplicatioNname = _options.AppName,
                QueryString = GetQueryString(httpContext),
                Trace = exception.StackTrace,
                Page = pageName,
                Method = stackTrace?.GetFrame(0)?.GetMethod()?.Name,
                LineNumber = stackTrace?.GetFrame(0)?.GetFileLineNumber(),
                ColumnNumber = stackTrace?.GetFrame(0)?.GetFileColumnNumber(),
                FileName = stackTrace?.GetFrame(0)?.GetFileName(),
                UserAgent = GetUserAgent(httpContext),
                ServerName = Environment.MachineName,
                IpAddress = GetIpAddress(httpContext),
                CustomData = customData,
                Language = language,
                SessionData = GetSessionData(httpContext),
                AssemblyVersion = assemblyVersion,
                HeadersData = GetHeadersData(httpContext),
                FormData = GetFormData(httpContext),
                CookiesData = GetCookiesData(httpContext),
                EnvironmentData = GetEnvironmentData()
            };

            await _httpClient.PostAsJsonAsync(
                "api/v1/log", requestBody, cancellationToken);

            _logger.LogInformation("Logging completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError("Logging failed with error: {0}", ex.Message);
        }
    }

    private static string? GetUserAgent(HttpContext? httpContext)
    {
        if (httpContext is null)
            return null;

        httpContext.Request.Headers
            .TryGetValue("User-Agent", out var userAgent);

        return userAgent.FirstOrDefault();
    }

    private static string? GetIpAddress(HttpContext? httpContext)
    {
        return httpContext?.Connection.RemoteIpAddress?.ToString();
    }

    private Dictionary<string, string>? GetQueryString(HttpContext? httpContext)
    {
        if (httpContext is null)
            return null;

        try
        {
            var query = httpContext.Request.Query;
            var queryData = new Dictionary<string, string>();

            foreach (var key in query.Keys.Where(key
                => !_keysToExclude.Contains(key,
                StringComparer.InvariantCultureIgnoreCase)))
            {
                queryData.Add(key, _hideAllRequestValues ?
                    "[Hidden]" : query[key].ToString());
            }

            return queryData;
        }
        catch { return null; }
    }

    private Dictionary<string, string>? GetHeadersData(HttpContext? httpContext)
    {
        if (httpContext is null)
            return null;

        try
        {
            var headers = httpContext.Request.Headers;
            var headersData = new Dictionary<string, string>();

            foreach (var key in headers.Keys.Where(key
                => !_keysToExclude.Contains(key,
                StringComparer.InvariantCultureIgnoreCase)))
            {
                headersData.Add(key, _hideAllRequestValues ?
                    "[Hidden]" : headers[key].ToString());
            }

            return headersData;
        }
        catch { return null; }
    }

    private Dictionary<string, string>? GetSessionData(HttpContext? httpContext)
    {
        if (httpContext is null)
            return null;

        try
        {
            var session = httpContext.Session;
            var sessionData = new Dictionary<string, string>();

            foreach (var key in session.Keys.Where(key
                => !_keysToExclude.Contains(key,
                StringComparer.InvariantCultureIgnoreCase)))
            {
                if (session.TryGetValue(key, out var value))
                {
                    sessionData.Add(key, _hideAllRequestValues ?
                        "[Hidden]" : Encoding.UTF8.GetString(value));
                }   
            }

            return sessionData;
        }
        catch { return null; }
    }

    private Dictionary<string, string>? GetCookiesData(HttpContext? httpContext)
    {
        try
        {
            if (httpContext is null)
                return null;

            var cookies = httpContext.Request.Cookies;
            var cookiesData = new Dictionary<string, string>();

            foreach (var key in cookies.Keys.Where(key
                => !_keysToExclude.Contains(key,
                StringComparer.InvariantCultureIgnoreCase)))
            {
                cookiesData.Add(key, _hideAllRequestValues ?
                    "[Hidden]" : cookies[key].ToString());
            }

            return cookiesData;
        }
        catch { return null; }
    }

    private Dictionary<string, string>? GetFormData(HttpContext? httpContext)
    {
        if (httpContext is null)
            return null;

        try
        {
            var form = httpContext.Request.Form;
            var formData = new Dictionary<string, string>();

            foreach (var key in form.Keys.Where(key
                => !_keysToExclude.Contains(key,
                StringComparer.InvariantCultureIgnoreCase)))
            {
                formData.Add(key, _hideAllRequestValues ?
                    "[Hidden]" : form[key].ToString());
            }

            return formData;
        }
        catch { return null; }
    }

    private static object GetEnvironmentData()
    {
        return new
        {
            WorkingMemory = Process.GetCurrentProcess().WorkingSet64,
            NonpagedSystemMemorySize = Process.GetCurrentProcess().NonpagedSystemMemorySize64,
            UserProcessorTime = Process.GetCurrentProcess().UserProcessorTime.ToString("c"),
            PrivilegedProcessorTime = Process.GetCurrentProcess().PrivilegedProcessorTime.ToString("c"),
            TotalProcessorTime = Process.GetCurrentProcess().TotalProcessorTime.ToString("c"),
            ThreadsCount = Process.GetCurrentProcess().Threads.Count
        };
    }
}
