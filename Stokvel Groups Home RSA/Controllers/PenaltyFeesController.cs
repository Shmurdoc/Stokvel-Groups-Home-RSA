using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Stokvel_Groups_Home_RSA.Data;
using Stokvel_Groups_Home_RSA.Interface.IRepo;
using Stokvel_Groups_Home_RSA.Models;

namespace Stokvel_Groups_Home_RSA.Controllers
{
    public class PenaltyFeesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public PenaltyFeesController(IUnitOfWork unitOfWork)
        {
           _unitOfWork = unitOfWork;
        }

        // GET: PenaltyFees
        public async Task<IActionResult> Index(int groupId, int accountId)
        {
            if (groupId == 0)
            {
                return NotFound();
            }
            var penaltyList = new List<PenaltyFee>();
            var applicationDbContext = await _unitOfWork.PenaltyFeeRepository.GetAllAsync(includeProperties: "Invoice");

            var account = await _unitOfWork.AccountsRepository.GetAllAsync(x=>x.GroupId == groupId);

            foreach (var item in applicationDbContext)
            {
                if (User.IsInRole("Admin")){
                    if (account.Any(x => x.AccountId == item.Invoice.AccountId))
                    {
                        penaltyList.Add(item);
                    }
                }else if (User.IsInRole("Member"))
                {
                    if (item.Invoice.AccountId == accountId)
                    {
                        penaltyList.Add(item);
                    }
                }
                
            }

            return View(penaltyList);
        }

        // GET: PenaltyFees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var penaltyFee = await _unitOfWork.PenaltyFeeRepository.GetList()
                .Include(p => p.Invoice)
                .FirstOrDefaultAsync(m => m.PenaltyFeeId == id);

            if (penaltyFee == null)
            {
                return NotFound();
            }

            return View(penaltyFee);
        }


        // GET: PenaltyFees/Create
        public async Task<IActionResult> Create()
        {
            var invoices = await _unitOfWork.InvoicesRepository.GetAllAsync();
            ViewData["InvoiceId"] = new SelectList(invoices, "InvoiceId", "InvoiceId");
            return View();
        }

        // POST: PenaltyFees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PenaltyFeeId,InvoiceId,PenaltyDate,PenaltyAmount,PenaltyLevel")] PenaltyFee? penaltyFee)
        {
            if (penaltyFee == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                await _unitOfWork.PenaltyFeeRepository.Add(penaltyFee);
                await _unitOfWork.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var invoices = await _unitOfWork.InvoicesRepository.GetAllAsync();
            ViewData["InvoiceId"] = new SelectList(invoices, "InvoiceId", "InvoiceId", penaltyFee.InvoiceId);
            return View(penaltyFee);
        }

        // GET: PenaltyFees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var penaltyFee = await _unitOfWork.PenaltyFeeRepository.GetByIdAsync(id);
            if (penaltyFee == null)
            {
                return NotFound();
            }
            var invoices = await _unitOfWork.InvoicesRepository.GetAllAsync();
            ViewData["InvoiceId"] = new SelectList(invoices, "InvoiceId", "InvoiceId", penaltyFee.InvoiceId);
            return View(penaltyFee);
        }

        // POST: PenaltyFees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PenaltyFeeId,InvoiceId,PenaltyDate,PenaltyAmount,PenaltyLevel")] PenaltyFee? penaltyFee)
        {
            if (id != penaltyFee.PenaltyFeeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _unitOfWork.PenaltyFeeRepository.Update(penaltyFee);
                    await _unitOfWork.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (await PenaltyFeeExists(penaltyFee.PenaltyFeeId))
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
            var invoices = await _unitOfWork.InvoicesRepository.GetAllAsync();
            ViewData["InvoiceId"] = new SelectList(invoices, "InvoiceId", "InvoiceId", penaltyFee.InvoiceId);
            return View(penaltyFee);
        }

        // GET: PenaltyFees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var penaltyFee = await _unitOfWork.PenaltyFeeRepository.GetList()
                .Include(p => p.Invoice)
                .FirstOrDefaultAsync(m => m.PenaltyFeeId == id);
            if (penaltyFee == null)
            {
                return NotFound();
            }

            return View(penaltyFee);
        }

        // POST: PenaltyFees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (await _unitOfWork.PenaltyFeeRepository.GetByIdAsync(id) == null)
            {
                return Problem("Entity set 'ApplicationDbContext.PenaltyFee'  is null.");
            }
            var penaltyFee = await _unitOfWork.PenaltyFeeRepository.GetByIdAsync(id);
            if (penaltyFee != null)
            {
                await _unitOfWork.PenaltyFeeRepository.RemoveAsync(penaltyFee);
            }
            
            await _unitOfWork.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> PenaltyFeeExists(int id)
        {
            var result = await _unitOfWork.PenaltyFeeRepository.GetAllAsync();
          return (result.Any(e => e.PenaltyFeeId == id));
        }
    }
}
