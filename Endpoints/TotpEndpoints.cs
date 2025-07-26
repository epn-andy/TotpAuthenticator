
using Microsoft.Extensions.Caching.Memory;
using OtpNet;
using QRCoder;
using TotpAuthenticator.Models;

namespace TotpAuthenticator.Endpoints;

public static class TotpEndpoints
{
    public static void MapTotpEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/generate-qr", (IMemoryCache cache, TotpGenerationRequest request) =>
        {
            if (string.IsNullOrWhiteSpace(request.UserId))
            {
                return Results.BadRequest(new { error = "User ID cannot be empty" });
            }

            if (cache.TryGetValue(request.UserId, out _))
            {
                return Results.BadRequest(new { error = "User ID already registered" });
            }

            var secretKey = KeyGeneration.GenerateRandomKey(20);
            var secretKeyBase32 = Base32Encoding.ToString(secretKey);
            var userId = request.UserId;

            cache.Set(userId, secretKey, TimeSpan.FromMinutes(10));

            var issuer = "MyApp";
            var accountIdentity = userId;
            var provisioningUrl = $"otpauth://totp/{issuer}:{accountIdentity}?secret={secretKeyBase32}&issuer={issuer}";

            var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(provisioningUrl, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new PngByteQRCode(qrCodeData);
            var qrCodeImage = qrCode.GetGraphic(20);

            return Results.File(qrCodeImage, "image/png");
        });

        app.MapPost("/verify-totp", (IMemoryCache cache, TotpVerificationRequest request) =>
        {
            if (string.IsNullOrWhiteSpace(request.UserId))
            {
                return Results.BadRequest(new { error = "User ID cannot be empty" });
            }
            
            if (string.IsNullOrWhiteSpace(request.TotpCode))
            {
                return Results.BadRequest(new { error = "TOTP code cannot be empty" });
            }

            if (!cache.TryGetValue(request.UserId, out byte[]? secretKey))
            {
                return Results.BadRequest(new { error = "Invalid user ID" });
            }

            var totp = new Totp(secretKey);
            var isValid = totp.VerifyTotp(request.TotpCode, out _);

            return Results.Ok(new { isValid });
        });
    }
}

