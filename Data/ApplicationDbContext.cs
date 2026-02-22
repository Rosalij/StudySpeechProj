using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StudySpeech.Models;

namespace StudySpeech.Data;
public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<NoteModel> Notes { get; set; }
    public DbSet<TagModel> Tags { get; set; }
    public DbSet<NoteTagModel> NoteTags { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Composite key
        modelBuilder.Entity<NoteTagModel>()
            .HasKey(nt => new { nt.NoteId, nt.TagId });

        // Relations
        modelBuilder.Entity<NoteTagModel>()
            .HasOne(nt => nt.Note)
            .WithMany(n => n.NoteTags)
            .HasForeignKey(nt => nt.NoteId);

        modelBuilder.Entity<NoteTagModel>()
            .HasOne(nt => nt.Tag)
            .WithMany()
            .HasForeignKey(nt => nt.TagId);
    }
    
}
