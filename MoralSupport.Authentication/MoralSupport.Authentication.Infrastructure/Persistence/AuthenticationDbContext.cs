using Microsoft.EntityFrameworkCore;
using MoralSupport.Authentication.Domain.Entities;

namespace MoralSupport.Authentication.Infrastructure.Persistence
{
    public class AuthenticationDbContext : DbContext
    {
        public AuthenticationDbContext(DbContextOptions<AuthenticationDbContext> options)
            : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<SsoSession> SsoSessions => Set<SsoSession>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.HasIndex(x => x.Email).IsUnique();
                entity.Property(x => x.Email).IsRequired().HasMaxLength(150);
                entity.Property(x => x.Name).IsRequired().HasMaxLength(100);
                entity.Property(x => x.Provider).IsRequired();
                entity.Property(x => x.ProviderId).IsRequired();
            });

            modelBuilder.Entity<SsoSession>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).HasMaxLength(64);
                e.HasIndex(x => x.UserId);
            });
        }
    }
}
