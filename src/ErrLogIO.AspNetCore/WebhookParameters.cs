using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ErrLogIO.AspNetCore;

public class WebhookParameters
{
    public WebhookParameters(string apiKey, string message)
    {
        ApiKey = apiKey;
        Message = message;
    }

    [JsonPropertyName("apikey")]
    public string ApiKey { get; init; }

    [JsonPropertyName("message")]
    public string Message { get; init; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("applicationname")]
    public string? ApplicatioNname { get; set; }

    [JsonPropertyName("querystring")]
    public Dictionary<string, string>? QueryString { get; set; }

    [JsonPropertyName("trace")]
    public string? Trace { get; set; }

    [JsonPropertyName("page")]
    public string? Page { get; set; }

    [JsonPropertyName("method")]
    public string? Method { get; set; }

    [JsonPropertyName("lineno")]
    public int? LineNumber { get; set; }

    [JsonPropertyName("colno")]
    public int? ColumnNumber { get; set; }

    [JsonPropertyName("filename")]
    public string? FileName { get; set; }

    [JsonPropertyName("useragent")]
    public string? UserAgent { get; set; }

    [JsonPropertyName("servername")]
    public string? ServerName { get; set; }

    [JsonPropertyName("ipaddress")]
    public string? IpAddress { get; set; }

    [JsonPropertyName("custom")]
    public object? CustomData { get; set; }

    [JsonPropertyName("language")]
    public int? Language { get; set; }

    [JsonPropertyName("session_data")]
    public Dictionary<string, string>? SessionData { get; set; }

    [JsonPropertyName("assemblyversion")]
    public string? AssemblyVersion { get; set; }

    [JsonPropertyName("application_data")]
    public Dictionary<string, string>? ApplicationData { get; set; }

    [JsonPropertyName("request_header")]
    public Dictionary<string, string>? HeadersData { get; set; }

    [JsonPropertyName("request_formdata")]
    public Dictionary<string, string>? FormData { get; set; }

    [JsonPropertyName("request_cookies")]
    public Dictionary<string, string>? CookiesData { get; set; }

    [JsonPropertyName("environment")]
    public object? EnvironmentData { get; set; }
}
