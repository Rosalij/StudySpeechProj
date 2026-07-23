using System;
using System.ComponentModel.DataAnnotations;
// Represents the model for a note-tag relationship, containing properties for note ID, tag ID, and navigation properties for the associated note and tag.
namespace StudySpeech.Models {
public class NoteTagModel
{
    public int NoteId { get; set; }
    public NoteModel Note { get; set; } = null!;

    public int TagId { get; set; }
    public TagModel Tag { get; set; } = null!;
} }