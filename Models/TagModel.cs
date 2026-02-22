using System;
using System.ComponentModel.DataAnnotations;

namespace StudySpeech.Models
{
public class TagModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // Many-to-many relation
    public List<NoteTagModel> NoteTags { get; set; } = new();
} }