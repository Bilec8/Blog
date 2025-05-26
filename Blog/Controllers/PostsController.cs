using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Blog.Data;
using Blog.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Ganss.Xss;

namespace Blog.Controllers
{
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PostsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Posts
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Posts.Include(p => p.Author);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Author)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Posts/Create
        public IActionResult Create()
        {
        // ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("Title,Content")] Post post)
        {
            if (ModelState.IsValid)
            {
                var sanitizer = new HtmlSanitizer();
                post.Content = sanitizer.Sanitize(post.Content);

                post.AuthorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                post.CreatedAt = DateTime.Now;

                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            var errors = ModelState
            .Where(x => x.Value.Errors.Count > 0)
            .Select(x => new { Field = x.Key, Errors = x.Value.Errors.Select(e => e.ErrorMessage) })
            .ToArray();

             ViewBag.ModelStateErrors = errors;

            return View(post);
        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            if(!(User.Identity.IsAuthenticated && User.FindFirstValue(ClaimTypes.NameIdentifier) == post.AuthorId))
            {
                return Forbid();
            }

            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id", post.AuthorId);
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Content,CreatedAt,UpdatedAt,AuthorId")] Post post)
        {
            if (id != post.Id)
            {
                return NotFound();
            }

            if (!(User.Identity.IsAuthenticated && User.FindFirstValue(ClaimTypes.NameIdentifier) == post.AuthorId))
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var sanitizer = new HtmlSanitizer();
                    post.Content = sanitizer.Sanitize(post.Content);

                    var originalPost = await _context.Posts.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
                    post.AuthorId = originalPost.AuthorId;
                    post.UpdatedAt = DateTime.Now;
                    post.CreatedAt = originalPost.CreatedAt;
                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id", post.AuthorId);
            return View(post);
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Author)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            if (!(User.Identity.IsAuthenticated && User.FindFirstValue(ClaimTypes.NameIdentifier) == post.AuthorId))
            {
                return Forbid();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            if (!(User.Identity.IsAuthenticated && User.FindFirstValue(ClaimTypes.NameIdentifier) == post.AuthorId))
            {
                return Forbid();
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }
    }
}
