using System;
using System.ComponentModel.DataAnnotations;
// Represents the model for a tag, containing properties for ID, name, user ID, and many-to-many relationships with notes.
namespace StudySpeech.Models
{
public class TagModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public string UserId { get; set; } = string.Empty;

    // Many-to-many relation
    public List<NoteTagModel> NoteTags { get; set; } = new();
} }