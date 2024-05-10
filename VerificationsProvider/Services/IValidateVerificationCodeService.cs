using Microsoft.AspNetCore.Http;
using VerificationsProvider.Models;

namespace VerificationsProvider.Services
{
    public interface IValidateVerificationCodeService
    {
        Task<ValidateRequest> UnpackValidateRequestAsync(HttpRequest req);
        Task<bool> ValidateCodeAsync(ValidateRequest validateRequest);
    }
}