using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudySpeech.Data;
using StudySpeech.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace StudySpeech.Controllers
{
    // Only logged in users can use this controller (checked on every action).
    [Authorize]
    public class NoteController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
private readonly AzureSpeechService _speechService;

public NoteController(ApplicationDbContext context,
                      UserManager<IdentityUser> userManager,
                      AzureSpeechService speechService)
{
    _context = context;
    _userManager = userManager;
    _speechService = speechService;
}


// Reads a note out loud using Azure Text To Speech. Only works for your own notes.
[HttpGet]
public async Task<IActionResult> Speak(int id)
{
    var userId = _userManager.GetUserId(User);

    // Make sure the note belongs to the logged in user, not just any note id.
    var note = await _context.Notes
        .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

    if (note == null)
        return NotFound();

    var audioBytes = await _speechService.TextToSpeechAsync(note.Content);

    // Send the audio straight back as an mp3 file the browser can play.
    return File(audioBytes, "audio/mpeg");
}

//  lets signed-out visitors hear the sample notes on the home page.
[HttpGet]
[AllowAnonymous]
public async Task<IActionResult> SpeakSample(int id)
{
    // No login check, only sample notes (IsSample = true) can be played this way.
    var note = await _context.Notes
        .FirstOrDefaultAsync(n => n.Id == id && n.IsSample);

    if (note == null)
        return NotFound();

    var audioBytes = await _speechService.TextToSpeechAsync(note.Content);

    return File(audioBytes, "audio/mpeg");
}

        // GET: Note
        // Main notes page. Handles sorting, searching and filtering by tag/folder all in one query.
        public async Task<IActionResult> Index(string sortOrder, string searchString, int? tagId, int? folderId)
        {
            var userId = _userManager.GetUserId(User);

            // Start with only the current user's notes, include tags + folder so the view can show them.
            var notesQuery = _context.Notes
                .Include(n => n.NoteTags)
                    .ThenInclude(nt => nt.Tag)
                .Include(n => n.Folder)
                .Where(n => n.UserId == userId);

            // Search on title/content.
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                notesQuery = notesQuery.Where(n =>
                    (n.Title != null && n.Title.Contains(searchString)) ||
                    (n.Content != null && n.Content.Contains(searchString)));
            }

            // Only keep notes that have the selected tag.
            if (tagId.HasValue)
            {
                notesQuery = notesQuery.Where(n => n.NoteTags.Any(nt => nt.TagId == tagId));
            }

            // Only keep notes in the selected folder.
            if (folderId.HasValue)
            {
                notesQuery = notesQuery.Where(n => n.FolderId == folderId);
            }

            // Default sort is newest first, unless "oldest" is picked.
            notesQuery = sortOrder == "oldest"
                ? notesQuery.OrderBy(n => n.CreatedAt)
                : notesQuery.OrderByDescending(n => n.CreatedAt);

            // Pass the current filters back to the view so links/inputs can stay selected.
            ViewBag.CurrentSort = sortOrder == "oldest" ? "oldest" : "newest";
            ViewBag.CurrentSearch = searchString;
            ViewBag.CurrentTag = tagId;
            ViewBag.CurrentFolder = folderId;
            ViewBag.Tags = await _context.Tags.Where(t => t.UserId == userId).OrderBy(t => t.Name).ToListAsync();
            ViewBag.Folders = await _context.Folders.Where(f => f.UserId == userId).OrderBy(f => f.Name).ToListAsync();

            var notes = await notesQuery.ToListAsync();
            return View(notes);
        }

        // GET: Note/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            var note = await _context.Notes
                .Include(n => n.NoteTags)
                    .ThenInclude(nt => nt.Tag)
                .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

            if (note == null) return NotFound();

            return View(note);
        }

// GET: Note/Create
 // shows the empty form. Tags/Folders lists are for the dropdowns.
public IActionResult Create()
{
    var userId = _userManager.GetUserId(User);
    ViewBag.Tags = new MultiSelectList(_context.Tags.Where(t => t.UserId == userId).ToList(), "Id", "Name");
    ViewBag.Folders = new SelectList(_context.Folders.Where(f => f.UserId == userId).ToList(), "Id", "Name");
    return View();
}

// POST: Note/Create
// Creates a new note, links the selected tags and saves it to the database.
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create([Bind("Title,Content,SelectedTagIds,FolderId")] NoteModel noteModel)
{
    var userId = _userManager.GetUserId(User);
// Make sure the folder (if any) actually belongs to the logged in user, otherwise just ignore it.
    if (noteModel.FolderId.HasValue && !await _context.Folders.AnyAsync(f => f.Id == noteModel.FolderId && f.UserId == userId))
    {
        noteModel.FolderId = null;
    }

    if (ModelState.IsValid)
    {
      // Set the user id and name on the note
        noteModel.UserId = userId;
        noteModel.UserName = User.Identity?.Name ?? "Unknown";
        noteModel.CreatedAt = DateTime.Now;

        _context.Notes.Add(noteModel);
        await _context.SaveChangesAsync(); // need this first so noteModel.Id is set

        // Link selected tags
        foreach (var tagId in noteModel.SelectedTagIds)
        {
            _context.NoteTags.Add(new NoteTagModel
            {
                NoteId = noteModel.Id,
                TagId = tagId
            });
        }
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // Form was invalid, redo the dropdowns so the page can be shown again.
    ViewBag.Tags = new MultiSelectList(_context.Tags.Where(t => t.UserId == userId).ToList(), "Id", "Name", noteModel.SelectedTagIds);
    ViewBag.Folders = new SelectList(_context.Folders.Where(f => f.UserId == userId).ToList(), "Id", "Name", noteModel.FolderId);
    return View(noteModel);
}

        // GET: Note/Edit/5
        // Shows the edit form for a note, pre-fills the selected tags and folder.
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            var note = await _context.Notes
                .Include(n => n.NoteTags)
                .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

            if (note == null) return NotFound();

            // Pre-check the tags this note already has in the form.
            note.SelectedTagIds = note.NoteTags.Select(nt => nt.TagId).ToList();
            ViewBag.Tags = new MultiSelectList(_context.Tags.Where(t => t.UserId == userId).ToList(), "Id", "Name", note.SelectedTagIds);
            ViewBag.Folders = new SelectList(_context.Folders.Where(f => f.UserId == userId).ToList(), "Id", "Name", note.FolderId);
            return View(note);
        }

        // POST: Note/Edit/5
        // Updates the note, updates the tag links and saves to the database.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Content,SelectedTagIds,FolderId")] NoteModel noteModel)
        {
            if (id != noteModel.Id) return NotFound();

            var userId = _userManager.GetUserId(User);
            // Load the real note from the db, don't just trust what got posted.
            var existingNote = await _context.Notes
                .Include(n => n.NoteTags)
                .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

            if (existingNote == null) return NotFound();

            // Same check as Create.
            if (noteModel.FolderId.HasValue && !await _context.Folders.AnyAsync(f => f.Id == noteModel.FolderId && f.UserId == userId))
            {
                noteModel.FolderId = null;
            }

            if (ModelState.IsValid)
            {
                existingNote.Title = noteModel.Title;
                existingNote.Content = noteModel.Content;
                existingNote.FolderId = noteModel.FolderId;

// remove all the old tag links and add the new ones. This is simpler than trying to figure out which tags were added/removed.
                // wipe the old links and add the new ones back.
                _context.NoteTags.RemoveRange(existingNote.NoteTags);
                foreach (var tagId in noteModel.SelectedTagIds)
                {
                    _context.NoteTags.Add(new NoteTagModel { NoteId = existingNote.Id, TagId = tagId });
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
// Form was invalid, redo the dropdowns so the page can be shown again.
            ViewBag.Tags = new MultiSelectList(_context.Tags.Where(t => t.UserId == userId).ToList(), "Id", "Name", noteModel.SelectedTagIds);
            ViewBag.Folders = new SelectList(_context.Folders.Where(f => f.UserId == userId).ToList(), "Id", "Name", noteModel.FolderId);
            return View(noteModel);
        }

        // GET: Note/Delete/5
        // Shows a confirmation page before deleting a note. Only works for your own notes.
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            var note = await _context.Notes
                .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

            if (note == null) return NotFound();

            return View(note);
        }

        // POST: Note/Delete/5
        // Deletes the note and its tag links from the database. Only works for your own notes.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = _userManager.GetUserId(User);
            var note = await _context.Notes
                .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

            if (note != null)
            {
                _context.Notes.Remove(note);
                await _context.SaveChangesAsync();
            }
// Redirect back to the main notes page after deletion.
            return RedirectToAction(nameof(Index));
        }
    }
}
