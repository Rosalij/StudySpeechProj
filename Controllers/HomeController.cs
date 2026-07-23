using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudySpeech.Data;
using StudySpeech.Models;

namespace StudySpeech.Controllers;

// Controller for the home page and privacy page. Does not require authentication.  
public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

// Shows a list of sample notes on the home page. Does not require authentication.
    public async Task<IActionResult> Index()
    { 
        var sampleNotes = await _context.Notes
            .Include(n => n.NoteTags)
                .ThenInclude(nt => nt.Tag)
            .Where(n => n.IsSample)
            .OrderBy(n => n.Id)
            .ToListAsync();

        return View(sampleNotes);
    }


    public IActionResult Error() // Shows the error page with the request ID. Does not require authentication.
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
