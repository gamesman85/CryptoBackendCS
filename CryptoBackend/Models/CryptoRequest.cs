namespace CryptoBackend.Models;

public class CryptoRequest
{
    public string Text { get; set; } = string.Empty;
    public string Algorithm { get; set; } = string.Empty;
    public string Operation { get; set; } = string.Empty;
    public string? Key { get; set; }
}

public class CryptoResponse
{
    public string? Result { get; set; }
    public string? Error { get; set; }
}