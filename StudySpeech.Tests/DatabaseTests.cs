using System.ComponentModel.DataAnnotations;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using StudySpeech.Data;
using StudySpeech.Models;

namespace StudySpeech.Tests;

// Tests run against a real SQLite database that lives in memory,
// so they check the actual EF Core mapping without touching app.db.
public class DatabaseTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<ApplicationDbContext> _options;

    public DatabaseTests()
    {
        // An in-memory SQLite database exists as long as its connection is open.
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        _options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(_connection)
            .Options;

        using var db = new ApplicationDbContext(_options);
        db.Database.EnsureCreated();
    }

    public void Dispose() => _connection.Dispose();

    private ApplicationDbContext NewContext() => new(_options);

    [Fact]
    public void SeedData_CreatesThreeSampleNotes()
    {
        using var db = NewContext();

        SeedData.EnsureSampleNotes(db);

        Assert.Equal(3, db.Notes.Count(n => n.IsSample));
    }

    [Fact]
    public void SeedData_RunTwice_DoesNotCreateDuplicates()
    {
        using var db = NewContext();

        SeedData.EnsureSampleNotes(db);
        SeedData.EnsureSampleNotes(db);

        Assert.Equal(3, db.Notes.Count());
        Assert.Equal(6, db.Tags.Count());
    }

    [Fact]
    public void SeedData_LinksEachSampleNoteToTwoTags()
    {
        using var db = NewContext();

        SeedData.EnsureSampleNotes(db);

        var tagsPerNote = db.NoteTags
            .GroupBy(nt => nt.NoteId)
            .Select(g => g.Count())
            .ToList();

        Assert.Equal(3, tagsPerNote.Count);
        Assert.All(tagsPerNote, count => Assert.Equal(2, count));
    }

    [Fact]
    public void DeletingFolder_KeepsTheNote_AndClearsItsFolder()
    {
        int noteId;

        using (var db = NewContext())
        {
            var note = new NoteModel
            {
                Title = "Cell biology",
                Content = "Mitosis has four phases.",
                UserId = "user-1",
                UserName = "tester",
                Folder = new FolderModel { Name = "Biology", UserId = "user-1" }
            };
            db.Notes.Add(note);
            db.SaveChanges();
            noteId = note.Id;
        }

        using (var db = NewContext())
        {
            db.Folders.Remove(db.Folders.Single());
            db.SaveChanges();
        }

        using (var db = NewContext())
        {
            var note = db.Notes.Single(n => n.Id == noteId);
            Assert.Null(note.FolderId);
        }
    }

    [Fact]
    public void Note_WithoutTitleAndContent_FailsValidation()
    {
        var note = new NoteModel();
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(note, new ValidationContext(note), results, true);

        Assert.False(isValid);
        Assert.Equal(2, results.Count);
    }
}
