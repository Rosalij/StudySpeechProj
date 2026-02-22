using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudySpeech.Data;
using StudySpeech.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace StudySpeech.Controllers
{
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
[HttpGet]
public async Task<IActionResult> Speak(int id)
{
    var userId = _userManager.GetUserId(User);

    var note = await _context.Notes
        .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

    if (note == null)
        return NotFound();

    var audioBytes = await _speechService.TextToSpeechAsync(note.Content);

    return File(audioBytes, "audio/mpeg");
}



        // GET: Note
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var notes = await _context.Notes
                .Where(n => n.UserId == userId)
                .ToListAsync();
            return View(notes);
        }

        // GET: Note/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            var note = await _context.Notes
                .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

            if (note == null) return NotFound();

            return View(note);
        }
// GET: Note/Create
public IActionResult Create()
{
    ViewBag.Tags = new MultiSelectList(_context.Tags.ToList(), "Id", "Name");
    return View();
}

// POST: Note/Create
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create([Bind("Title,Content,SelectedTagIds")] NoteModel noteModel)
{
    if (ModelState.IsValid)
    {
        noteModel.UserId = _userManager.GetUserId(User);
        noteModel.UserName = User.Identity?.Name ?? "Unknown";
        noteModel.CreatedAt = DateTime.Now;

        _context.Notes.Add(noteModel);
        await _context.SaveChangesAsync();

        // Link selected tags
        if (noteModel.NoteTags != null)
        {
            foreach (var tag in noteModel.NoteTags)
            {
                _context.NoteTags.Add(new NoteTagModel
                {
                    NoteId = noteModel.Id,
                    TagId = tag.TagId
                });
            }
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    ViewBag.Tags = new MultiSelectList(_context.Tags.ToList(), "Id", "Name", noteModel.NoteTags.Select(t => t.TagId));
    return View(noteModel);
}


        // POST: Note/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Content")] NoteModel noteModel)
        {
            if (id != noteModel.Id) return NotFound();

            var userId = _userManager.GetUserId(User);
            var existingNote = await _context.Notes
                .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

            if (existingNote == null) return NotFound();

            if (ModelState.IsValid)
            {
                existingNote.Title = noteModel.Title;
                existingNote.Content = noteModel.Content;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(noteModel);
        }

        // GET: Note/Delete/5
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

            return RedirectToAction(nameof(Index));
        }
    }
}
