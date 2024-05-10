
namespace VerificationsProvider.Services
{
    public interface IVerificationCleanerService
    {
        Task RemoveExpiredRecordsAsync();
    }
}