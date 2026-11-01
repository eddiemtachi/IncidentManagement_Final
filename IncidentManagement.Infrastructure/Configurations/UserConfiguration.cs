using IncidentManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IncidentManagement.Infrastructure.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");
            builder.HasKey(u => u.UserId);
            builder.Property(u => u.Username).IsRequired().HasMaxLength(100);
            builder.Property(u => u.Email).IsRequired().HasMaxLength(200);
            builder.Property(u => u.PasswordHash).IsRequired();
            // PasswordHash is the ASP.NET Identity encoded hash. The legacy PasswordSalt
            // column is not used by PasswordHasher and is intentionally not mapped.
            builder.Ignore(u => u.PasswordSalt);

            builder.HasOne(u => u.Property).WithMany().HasForeignKey(u => u.PropertyId);
            builder.HasOne(u => u.Role).WithMany().HasForeignKey(u => u.RoleId);
        }
    }
}
