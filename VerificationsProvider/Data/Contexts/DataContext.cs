using Microsoft.EntityFrameworkCore;
using VerificationsProvider.Data.Entities;

namespace VerificationsProvider.Data.Contexts;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    public DbSet<VerificationRequestEntity> VerificationRequests { get; set; }
}
