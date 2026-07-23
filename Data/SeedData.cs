using StudySpeech.Models;

namespace StudySpeech.Data;

// Provides methods to seed the database with sample notes and tags for demonstration purposes.
public static class SeedData
{
    public static void EnsureSampleNotes(ApplicationDbContext db)
    {
        if (db.Notes.Any(n => n.IsSample))
        {
            return;
        }
// Defines an array of sample notes with their titles, content, and associated tags.
        var samples = new[]
        {
            new
            {
                Title = "Mitosis Steps",
                Content = "Prophase, metaphase, anaphase, telophase. Chromosomes condense, align at the metaphase plate, then separate to opposite poles before the cell divides in two.",
                Tags = new[] { "biology", "exam" }
            },
            new
            {
                Title = "Big O Cheat Sheet",
                Content = "O(1) constant, O(log n) binary search, O(n) linear scan, O(n log n) sorting, O(n^2) nested loops. Always check the worst case before optimizing.",
                Tags = new[] { "algorithms", "cs" }
            },
            new
            {
                Title = "French Verb Conjugation",
                Content = "-er verbs: je parle, tu parles, il/elle parle, nous parlons, vous parlez, ils/elles parlent. Practice out loud, then listen back with AI speech.",
                Tags = new[] { "french", "language" }
            }
        };

        foreach (var sample in samples)
        {
            var note = new NoteModel
            {
                Title = sample.Title,
                Content = sample.Content,
                CreatedAt = DateTime.Now,
                UserId = string.Empty,
                UserName = "StudySpeech",
                IsSample = true
            };

            db.Notes.Add(note);
            db.SaveChanges();
// For each tag associated with the sample note, check if it exists in the database. If not, create a new tag and associate it with the note.
            foreach (var tagName in sample.Tags)
            {
                var tag = db.Tags.FirstOrDefault(t => t.Name == tagName && t.UserId == string.Empty);
                if (tag == null)
                {
                    tag = new TagModel { Name = tagName, UserId = string.Empty };
                    db.Tags.Add(tag);
                    db.SaveChanges();
                }

                db.NoteTags.Add(new NoteTagModel { NoteId = note.Id, TagId = tag.Id });
            }
        }

        db.SaveChanges();
    }
}
