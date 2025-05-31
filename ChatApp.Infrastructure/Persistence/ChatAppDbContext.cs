using ChatApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Infrastructure.Persistence;

public class ChatAppDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Group> Groups => Set<Group>();
    public DbSet<Message> Messages => Set<Message>();

    public ChatAppDbContext(DbContextOptions<ChatAppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
            entity.Property(e => e.PasswordHash).IsRequired();

            entity
                .HasMany(e => e.Contacts)
                .WithMany()
                .UsingEntity(j => j.ToTable("UserContacts"));

            entity
                .HasMany(e => e.Groups)
                .WithMany(e => e.Members)
                .UsingEntity(j => j.ToTable("UserGroups"));
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);

            entity
                .HasOne(e => e.Creator)
                .WithMany()
                .HasForeignKey(e => e.CreatorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Content).IsRequired();
            entity.Property(e => e.SentAt).IsRequired();

            entity
                .HasOne(e => e.Sender)
                .WithMany(e => e.SentMessages)
                .HasForeignKey(e => e.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasOne(e => e.RecipientUser)
                .WithMany(e => e.ReceivedMessages)
                .HasForeignKey(e => e.RecipientUserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasOne(e => e.RecipientGroup)
                .WithMany(e => e.Messages)
                .HasForeignKey(e => e.RecipientGroupId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}