using Data.Types;
using Microsoft.EntityFrameworkCore;

namespace Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Post> Posts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureUsersTable(modelBuilder);
        ConfigurePostsTable(modelBuilder);
        base.OnModelCreating(modelBuilder);
    }

    private static void ConfigureUsersTable(ModelBuilder modelBuilder)
    {
        var builder = modelBuilder.Entity<User>();

        builder.HasIndex(x => x.Username)
            .IsUnique();

        builder.HasIndex(x => x.ReferenceId)
            .IsUnique();

        builder.HasMany(x => x.Posts)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction);
    }

    private static void ConfigurePostsTable(ModelBuilder modelBuilder)
    {
        var builder = modelBuilder.Entity<Post>();

        builder.HasIndex(x => x.ReferenceId)
            .IsUnique();

        builder.Property(x => x.Title)
            .HasMaxLength(100);
    }

}