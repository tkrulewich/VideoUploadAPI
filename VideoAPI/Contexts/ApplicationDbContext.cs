using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TkrulVideoUpload.Models.Entities;

public class ApplicationDbContext : IdentityDbContext
{

    public DbSet<Video> Videos { get; set; }
    public DbSet<VideoAccessLog> VideoAccessLogs { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
        Videos = Set<Video>();
        VideoAccessLogs = Set<VideoAccessLog>();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Video>()
            .HasMany(v => v.AccessLogs)
            .WithOne()
            .HasForeignKey(v => v.VideoId);

        builder.Entity<VideoAccessLog>()
            .HasOne(log => log.Video)
            .WithMany(video => video.AccessLogs)
            .HasForeignKey(log => log.VideoId);

        builder.Entity<VideoAccessLog>()
            .HasOne(log => log.User)
            .WithMany()
            .HasForeignKey(log => log.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}