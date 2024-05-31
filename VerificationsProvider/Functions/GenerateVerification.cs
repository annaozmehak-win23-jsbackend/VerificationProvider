using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using VerificationsProvider.Services;

namespace VerificationsProvider.Functions;

public class GenerateVerification(ILogger<GenerateVerification> logger, IVerificationService verificationService)
{
    private readonly ILogger<GenerateVerification> _logger = logger;
    private readonly IVerificationService _verificationService = verificationService;


    [Function("GenerateVerification")]
    [ServiceBusOutput("email_request", Connection = "ServiceBusConnection")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req, [ServiceBusTrigger("verification_request", Connection = "ServiceBusConnection")] ServiceBusReceivedMessage message, ServiceBusMessageActions messageActions)
    {
        try
        {
            var verificationRequest = _verificationService.UnpackVerificationRequest(message);
            if (verificationRequest != null)
            {
                var code = _verificationService.GenerateCode();
                if (!string.IsNullOrEmpty(code))
                {
                    var result = await _verificationService.SaveVerificationRequest(verificationRequest, code);
                    if (result)
                    {
                        var emailRequest = _verificationService.GenerateEmailRequest(verificationRequest, code);
                        if (emailRequest != null)
                        {
                            var payload = _verificationService.GenerateServiceBusEmailRequest(emailRequest);
                            if (!string.IsNullOrEmpty(payload))
                            {
                                await messageActions.CompleteMessageAsync(message);
                                return new OkObjectResult(payload);
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error : GenerateVerification.Run() :: {ex.Message}");
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

        return new StatusCodeResult(StatusCodes.Status400BadRequest);
    }
}
