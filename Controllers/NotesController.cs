using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotesApp2.Data;
using NotesApp2.Models;

namespace NotesApp2.Controllers
{
    [Authorize]
    public class NotesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public NotesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var notes = await _context.Notes
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
            return View(notes);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Note note)
        {
            ModelState.Remove("UserId");
            ModelState.Remove("User");
            
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);
                if (userId == null)
                {
                    return Unauthorized();
                }
                
                note.UserId = userId;
                note.CreatedAt = DateTime.UtcNow;
                
                _context.Add(note);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            return View(note);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var userId = _userManager.GetUserId(User) ?? string.Empty;
            var note = await _context.Notes
                .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);
            
            if (note == null) return NotFound();

            return View(note);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Note note)
        {
            if (id != note.Id) return NotFound();

            var userId = _userManager.GetUserId(User) ?? string.Empty;
            if (note.UserId != userId) return Forbid();

            ModelState.Remove("User");
            
            if (ModelState.IsValid)
            {
                try
                {
                    note.UpdatedAt = DateTime.UtcNow;
                    _context.Update(note);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NoteExists(note.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(note);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userManager.GetUserId(User) ?? string.Empty;
            var note = await _context.Notes
                .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);
            
            if (note != null)
            {
                _context.Notes.Remove(note);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool NoteExists(int id)
        {
            var userId = _userManager.GetUserId(User) ?? string.Empty;
            return _context.Notes.Any(e => e.Id == id && e.UserId == userId);
        }
    }
}