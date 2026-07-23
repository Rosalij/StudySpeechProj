using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudySpeech.Data;
using StudySpeech.Models;
// Controller for managing tags. Only allows access to authenticated users.

namespace StudySpeech.Controllers
{
    [Authorize]
    public class TagController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

// Initializes a new instance of the TagController class with the specified database context and user manager.
        public TagController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        // GET: Tag
        // Shows a list of tags for the current user.
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var tags = await _context.Tags
                .Where(t => t.UserId == userId)
                .ToListAsync();
            return View(tags);
        }

        // GET: Tag/Create
        // Shows the form to create a new tag.
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tag/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // Creates a new tag for the current user and saves it to the database.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] TagModel tagModel)
        {
            if (ModelState.IsValid)
            {
                tagModel.UserId = _userManager.GetUserId(User) ?? string.Empty;
                _context.Add(tagModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tagModel);
        }

        // GET: Tag/Delete/5
        // Shows a confirmation page before deleting a tag. Only works for your own tags.
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
// Checks if the tag belongs to the current user before showing the delete confirmation page.
            var userId = _userManager.GetUserId(User);
            var tagModel = await _context.Tags
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);
            if (tagModel == null)
            {
                return NotFound();
            }

            return View(tagModel);
        }

        // POST: Tag/Delete/5
        // Deletes the tag from the database. Only works for your own tags.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, string? returnUrl)
        {
            var userId = _userManager.GetUserId(User);
            var tagModel = await _context.Tags
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
            if (tagModel != null)
            {
                _context.Tags.Remove(tagModel);
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
