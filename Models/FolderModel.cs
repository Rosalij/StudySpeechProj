using System;
using System.ComponentModel.DataAnnotations;
// Represents the model for a folder, containing properties for ID, name, user ID, and a list of associated notes.
namespace StudySpeech.Models
{
public class FolderModel
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public string UserId { get; set; } = string.Empty;

    public List<NoteModel> Notes { get; set; } = new();
} }
