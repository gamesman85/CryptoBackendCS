using CryptoBackend.Models;
using CryptoBackend.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add Swagger/OpenAPI support if you want it
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAll");

// Root route
app.MapGet("/", () => new { Message = "Crypto Explorer Backend", Status = "active" });

// Health check
app.MapGet("/health", () => new { Status = "ok" });

// Crypto endpoint
app.MapPost("/api/crypto", (CryptoRequest request) =>
{
    try
    {
        if (string.IsNullOrEmpty(request.Text))
        {
            return Results.BadRequest(new CryptoResponse { Error = "Text is required" });
        }
        
        string result;
        
        switch (request.Algorithm)
        {
            case "aes-256-cbc":
                if (string.IsNullOrEmpty(request.Key))
                {
                    return Results.BadRequest(new CryptoResponse { Error = "Key is required for AES encryption/decryption" });
                }
                
                result = request.Operation == "encrypt" 
                    ? CryptoService.AesEncrypt(request.Text, request.Key)
                    : CryptoService.AesDecrypt(request.Text, request.Key);
                break;
                
            case "sha256":
                result = CryptoService.HashSHA256(request.Text);
                break;
                
            case "md5":
                result = CryptoService.HashMD5(request.Text);
                break;
                
            case "base64":
                result = request.Operation == "encrypt" 
                    ? CryptoService.EncodeBase64(request.Text)
                    : CryptoService.DecodeBase64(request.Text);
                break;
                
            default:
                return Results.BadRequest(new CryptoResponse { Error = "Unsupported algorithm" });
        }
        
        return Results.Ok(new CryptoResponse { Result = result });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new CryptoResponse { Error = ex.Message });
    }
});

app.Run();