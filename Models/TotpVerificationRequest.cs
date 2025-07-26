namespace TotpAuthenticator.Models;

public record TotpVerificationRequest(string UserId, string TotpCode);
