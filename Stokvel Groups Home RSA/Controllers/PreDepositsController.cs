using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Stokvel_Groups_Home_RSA.Data;
using Stokvel_Groups_Home_RSA.Models;

namespace Stokvel_Groups_Home_RSA.Controllers
{
    public class PreDepositsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PreDepositsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: PreDeposits
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.PreDeposits.Include(p => p.Account);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: PreDeposits/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.PreDeposits == null)
            {
                return NotFound();
            }

            var preDeposit = await _context.PreDeposits
                .Include(p => p.Account)
                .FirstOrDefaultAsync(m => m.PreDepositId == id);
            if (preDeposit == null)
            {
                return NotFound();
            }

            return View(preDeposit);
        }

        // GET: PreDeposits/Create
        public IActionResult Create()
        {
            ViewData["AccountId"] = new SelectList(_context.Accounts, "AccountId", "AccountId");
            return View();
        }

        // POST: PreDeposits/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PreDepositId,PreDepositDate,Amount,AccountId")] PreDeposit preDeposit)
        {
            if (ModelState.IsValid)
            {
                _context.Add(preDeposit);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AccountId"] = new SelectList(_context.Accounts, "AccountId", "AccountId", preDeposit.AccountId);
            return View(preDeposit);
        }

        // GET: PreDeposits/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.PreDeposits == null)
            {
                return NotFound();
            }

            var preDeposit = await _context.PreDeposits.FindAsync(id);
            if (preDeposit == null)
            {
                return NotFound();
            }
            ViewData["AccountId"] = new SelectList(_context.Accounts, "AccountId", "AccountId", preDeposit.AccountId);
            return View(preDeposit);
        }

        // POST: PreDeposits/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PreDepositId,PreDepositDate,Amount,AccountId")] PreDeposit preDeposit)
        {
            if (id != preDeposit.PreDepositId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(preDeposit);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PreDepositExists(preDeposit.PreDepositId))
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
            ViewData["AccountId"] = new SelectList(_context.Accounts, "AccountId", "AccountId", preDeposit.AccountId);
            return View(preDeposit);
        }

        // GET: PreDeposits/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.PreDeposits == null)
            {
                return NotFound();
            }

            var preDeposit = await _context.PreDeposits
                .Include(p => p.Account)
                .FirstOrDefaultAsync(m => m.PreDepositId == id);
            if (preDeposit == null)
            {
                return NotFound();
            }

            return View(preDeposit);
        }

        // POST: PreDeposits/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.PreDeposits == null)
            {
                return Problem("Entity set 'ApplicationDbContext.PreDeposits'  is null.");
            }
            var preDeposit = await _context.PreDeposits.FindAsync(id);
            if (preDeposit != null)
            {
                _context.PreDeposits.Remove(preDeposit);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PreDepositExists(int id)
        {
          return (_context.PreDeposits?.Any(e => e.PreDepositId == id)).GetValueOrDefault();
        }
    }
}
