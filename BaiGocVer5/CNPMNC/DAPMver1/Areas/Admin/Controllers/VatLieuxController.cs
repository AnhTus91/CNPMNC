using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DAPMver1.Data;

namespace DAPMver1.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class VatLieuxController : Controller
    {
        private readonly DapmTrangv1Context _context;

        public VatLieuxController(DapmTrangv1Context context)
        {
            _context = context;
        }

        // GET: Admin/VatLieux
        public async Task<IActionResult> Index()
        {
            return View(await _context.VatLieus.ToListAsync());
        }

        // GET: Admin/VatLieux/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vatLieu = await _context.VatLieus
                .FirstOrDefaultAsync(m => m.MaVatLieu == id);
            if (vatLieu == null)
            {
                return NotFound();
            }

            return View(vatLieu);
        }

        // GET: Admin/VatLieux/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/VatLieux/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaVatLieu,TenVatlieu,MoTa,NgayTao")] VatLieu vatLieu)
        {
            if (ModelState.IsValid)
            {
                _context.Add(vatLieu);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(vatLieu);
        }

        // GET: Admin/VatLieux/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vatLieu = await _context.VatLieus.FindAsync(id);
            if (vatLieu == null)
            {
                return NotFound();
            }
            return View(vatLieu);
        }

        // POST: Admin/VatLieux/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaVatLieu,TenVatlieu,MoTa,NgayTao")] VatLieu vatLieu)
        {
            if (id != vatLieu.MaVatLieu)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vatLieu);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VatLieuExists(vatLieu.MaVatLieu))
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
            return View(vatLieu);
        }

        // GET: Admin/VatLieux/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vatLieu = await _context.VatLieus
                .FirstOrDefaultAsync(m => m.MaVatLieu == id);
            if (vatLieu == null)
            {
                return NotFound();
            }

            return View(vatLieu);
        }

        // POST: Admin/VatLieux/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vatLieu = await _context.VatLieus.FindAsync(id);
            if (vatLieu != null)
            {
                _context.VatLieus.Remove(vatLieu);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VatLieuExists(int id)
        {
            return _context.VatLieus.Any(e => e.MaVatLieu == id);
        }
    }
}
