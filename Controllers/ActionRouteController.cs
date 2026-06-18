
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EBookStore.Models.Filters;
using EBookStore.Models.Database;

public class ActionRouteController : Controller
{
    private readonly ConnectionString _context;

    public ActionRouteController(ConnectionString context)
    {
        _context = context;
    }

    // GET: ACTIONROUTES
    public async Task<IActionResult> Index()    
    {
        return View(await _context.ActionRoutes.ToListAsync());
    }

    // GET: ACTIONROUTES/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var actionroute = await _context.ActionRoutes
            .FirstOrDefaultAsync(m => m.ID == id);
        if (actionroute == null)
        {
            return NotFound();
        }

        return View(actionroute);
    }

    // GET: ACTIONROUTES/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: ACTIONROUTES/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("ID,Controller,Action,Method,Status")] ActionRoute actionroute)
    {
        if (ModelState.IsValid)
        {
            _context.Add(actionroute);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(actionroute);
    }

    // GET: ACTIONROUTES/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var actionroute = await _context.ActionRoutes.FindAsync(id);
        if (actionroute == null)
        {
            return NotFound();
        }
        return View(actionroute);
    }

    // POST: ACTIONROUTES/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("ID,Controller,Action,Method,Status")] ActionRoute actionroute)
    {
        if (id != actionroute.ID)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(actionroute);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ActionRouteExists(actionroute.ID))
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
        return View(actionroute);
    }

    // GET: ACTIONROUTES/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var actionroute = await _context.ActionRoutes
            .FirstOrDefaultAsync(m => m.ID == id);
        if (actionroute == null)
        {
            return NotFound();
        }

        return View(actionroute);
    }

    // POST: ACTIONROUTES/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var actionroute = await _context.ActionRoutes.FindAsync(id);
        if (actionroute != null)
        {
            _context.ActionRoutes.Remove(actionroute);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool ActionRouteExists(int? id)
    {
        return _context.ActionRoutes.Any(e => e.ID == id);
    }
}
