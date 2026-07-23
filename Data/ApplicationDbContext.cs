using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StudySpeech.Models;
// Represents the database context for the StudySpeech application, including tables for notes, tags, note-tag relationships, and folders.
namespace StudySpeech.Data;
public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    // Initializes a new instance of the ApplicationDbContext class with the specified options.
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

// Gets or sets the DbSet for notes, tags, note-tag relationships, and folders in the database.
    public DbSet<NoteModel> Notes { get; set; }
    public DbSet<TagModel> Tags { get; set; }
    public DbSet<NoteTagModel> NoteTags { get; set; }
    public DbSet<FolderModel> Folders { get; set; }

// Configures the model relationships and composite keys for the database context.
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

        modelBuilder.Entity<NoteModel>()
            .HasOne(n => n.Folder)
            .WithMany(f => f.Notes)
            .HasForeignKey(n => n.FolderId)
            .OnDelete(DeleteBehavior.SetNull);
    }
    
}
