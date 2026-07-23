using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudySpeech.Data;
using StudySpeech.Models;

namespace StudySpeech.Controllers
{// Controller for managing folders. Only allows access to authenticated users.
    [Authorize]
    public class FolderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public FolderController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Folder
        // Shows a list of folders for the current user.
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var folders = await _context.Folders
                .Where(f => f.UserId == userId)
                .ToListAsync();
            return View(folders);
        }

        // GET: Folder/Create
        // Shows the form to create a new folder.
        public IActionResult Create()
        {
            return View();
        }

        // POST: Folder/Create
        // Creates a new folder for the current user and saves it to the database.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] FolderModel folderModel)
        {
            if (ModelState.IsValid)
            {
                folderModel.UserId = _userManager.GetUserId(User) ?? string.Empty;
                _context.Add(folderModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(folderModel);
        }

        // GET: Folder/Delete/5
        // Shows a confirmation page before deleting a folder. Only works for your own folders.
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            var folderModel = await _context.Folders
                .FirstOrDefaultAsync(f => f.Id == id && f.UserId == userId);
            if (folderModel == null)
            {
                return NotFound();
            }

            return View(folderModel);
        }

        // POST: Folder/Delete/5
        // Deletes the folder from the database. Only works for your own folders.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, string? returnUrl)
        {
            var userId = _userManager.GetUserId(User);
            var folderModel = await _context.Folders
                .FirstOrDefaultAsync(f => f.Id == id && f.UserId == userId);
            if (folderModel != null)
            {
                _context.Folders.Remove(folderModel);
                await _context.SaveChangesAsync();
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
