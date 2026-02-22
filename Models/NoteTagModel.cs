using System;
using System.ComponentModel.DataAnnotations;

namespace StudySpeech.Models {
public class NoteTagModel
{
    public int NoteId { get; set; }
    public NoteModel Note { get; set; } = null!;

    public int TagId { get; set; }
    public TagModel Tag { get; set; } = null!;
} }