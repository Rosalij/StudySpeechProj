using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// Represents the model for a note, containing properties for ID, title, content, creation date, user information, sample status, folder association, and many-to-many relationships with tags.
namespace StudySpeech.Models {
public class NoteModel{
    public int Id { get; set; }

    [Required]
    public string? Title { get; set; }

    [Required]
    public string? Content { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;

    // Marks a note as a public showcase note displayed on the home page for signed-out visitors.
    public bool IsSample { get; set; } = false;

    public int? FolderId { get; set; }
    public FolderModel? Folder { get; set; }

    // Many-to-many relation
    public List<NoteTagModel> NoteTags { get; set; } = new();

    // Tag ids selected in the Create/Edit form; not persisted directly, used to build NoteTags.
    [NotMapped]
    public List<int> SelectedTagIds { get; set; } = new();
}
}