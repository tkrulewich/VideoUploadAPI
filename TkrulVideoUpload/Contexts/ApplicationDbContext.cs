using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TkrulVideoUpload.Models.Entities;

public class ApplicationDbContext : IdentityDbContext
{

    public virtual DbSet<Video> Videos { get; set; }
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Additional DbSet properties for your entities can be
}