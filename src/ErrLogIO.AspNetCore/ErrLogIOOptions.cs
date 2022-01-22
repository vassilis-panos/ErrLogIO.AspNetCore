namespace ErrLogIO.AspNetCore;

public class ErrLogIOOptions
{
    public ErrLogIOOptions()
    {
        KeysToExclude = new [] { 
            "password", 
            "pwd", 
            "token", 
            "key", 
            "viewstate", 
            "cookie", 
            "aspnet", 
            "validation"
        };
    }

    public string? ApiKey { get; set; }
    public string? AppName { get; set; }
    public Language? Language { get; set; }
    public string[]? KeysToExclude { get; set; }
    public bool HideAllRequestValues { get; set; }
}
