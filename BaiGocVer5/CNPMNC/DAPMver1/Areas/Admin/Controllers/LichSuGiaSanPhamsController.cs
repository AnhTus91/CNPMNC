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
    public class LichSuGiaSanPhamsController : Controller
    {
        private readonly DapmTrangv1Context _context;

        public LichSuGiaSanPhamsController(DapmTrangv1Context context)
        {
            _context = context;
        }

        // GET: Admin/LichSuGiaSanPhams
        public async Task<IActionResult> Index()
        {
            var dapmTrangv1Context = _context.LichSuGiaSanPhams.Include(l => l.MaSanPhamNavigation);
            return View(await dapmTrangv1Context.ToListAsync());
        }

        // GET: Admin/LichSuGiaSanPhams/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lichSuGiaSanPham = await _context.LichSuGiaSanPhams
                .Include(l => l.MaSanPhamNavigation)
                .FirstOrDefaultAsync(m => m.MaLichSu == id);
            if (lichSuGiaSanPham == null)
            {
                return NotFound();
            }

            return View(lichSuGiaSanPham);
        }

        // GET: Admin/LichSuGiaSanPhams/Create
        public IActionResult Create()
        {
            ViewData["MaSanPham"] = new SelectList(_context.SanPhams, "MaSanPham", "AnhSp");
            return View();
        }

        // POST: Admin/LichSuGiaSanPhams/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaLichSu,MaSanPham,GiaCu,GiaMoi,NgayCapNhat")] LichSuGiaSanPham lichSuGiaSanPham)
        {
            if (ModelState.IsValid)
            {
                _context.Add(lichSuGiaSanPham);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaSanPham"] = new SelectList(_context.SanPhams, "MaSanPham", "AnhSp", lichSuGiaSanPham.MaSanPham);
            return View(lichSuGiaSanPham);
        }

        // GET: Admin/LichSuGiaSanPhams/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lichSuGiaSanPham = await _context.LichSuGiaSanPhams.FindAsync(id);
            if (lichSuGiaSanPham == null)
            {
                return NotFound();
            }
            ViewData["MaSanPham"] = new SelectList(_context.SanPhams, "MaSanPham", "AnhSp", lichSuGiaSanPham.MaSanPham);
            return View(lichSuGiaSanPham);
        }

        // POST: Admin/LichSuGiaSanPhams/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaLichSu,MaSanPham,GiaCu,GiaMoi,NgayCapNhat")] LichSuGiaSanPham lichSuGiaSanPham)
        {
            if (id != lichSuGiaSanPham.MaLichSu)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lichSuGiaSanPham);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LichSuGiaSanPhamExists(lichSuGiaSanPham.MaLichSu))
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
            ViewData["MaSanPham"] = new SelectList(_context.SanPhams, "MaSanPham", "AnhSp", lichSuGiaSanPham.MaSanPham);
            return View(lichSuGiaSanPham);
        }

        // GET: Admin/LichSuGiaSanPhams/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lichSuGiaSanPham = await _context.LichSuGiaSanPhams
                .Include(l => l.MaSanPhamNavigation)
                .FirstOrDefaultAsync(m => m.MaLichSu == id);
            if (lichSuGiaSanPham == null)
            {
                return NotFound();
            }

            return View(lichSuGiaSanPham);
        }

        // POST: Admin/LichSuGiaSanPhams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lichSuGiaSanPham = await _context.LichSuGiaSanPhams.FindAsync(id);
            if (lichSuGiaSanPham != null)
            {
                _context.LichSuGiaSanPhams.Remove(lichSuGiaSanPham);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LichSuGiaSanPhamExists(int id)
        {
            return _context.LichSuGiaSanPhams.Any(e => e.MaLichSu == id);
        }
    }
}
