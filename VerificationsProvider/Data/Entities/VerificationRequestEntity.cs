using System.ComponentModel.DataAnnotations;

namespace VerificationsProvider.Data.Entities;

public class VerificationRequestEntity
{
    [Key]
    public string Email { get; set; } = null!;
    public string Code { get; set; } = null!;
    public DateTime ExpiryDate { get; set; } = DateTime.Now.AddMinutes(5);
}
