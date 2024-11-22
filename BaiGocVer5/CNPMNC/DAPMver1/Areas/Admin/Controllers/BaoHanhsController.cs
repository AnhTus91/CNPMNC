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
    public class BaoHanhsController : Controller
    {
        private readonly DapmTrangv1Context _context;

        public BaoHanhsController(DapmTrangv1Context context)
        {
            _context = context;
        }

        // GET: Admin/BaoHanhs
        public async Task<IActionResult> Index()
        {
            var dapmTrangv1Context = _context.BaoHanhs.Include(b => b.MaKichCoNavigation).ThenInclude(l=>l.MaSanPhamNavigation).Include(b => b.MaNguoiDungNavigation);
            return View(await dapmTrangv1Context.ToListAsync());
        }

        // GET: Admin/BaoHanhs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var baoHanh = await _context.BaoHanhs
                .Include(b => b.MaKichCoNavigation)
                .Include(b => b.MaNguoiDungNavigation)
                .FirstOrDefaultAsync(m => m.MaBaoHanh == id);
            if (baoHanh == null)
            {
                return NotFound();
            }

            return View(baoHanh);
        }

        // GET: Admin/BaoHanhs/Create
        public IActionResult Create()
        {
            ViewData["MaKichCo"] = new SelectList(_context.KichCos, "MaKichCo", "MaKichCo");
            ViewData["MaNguoiDung"] = new SelectList(_context.NguoiDungs, "MaNguoiDung", "AnhDaiDien");
            return View();
        }

        // POST: Admin/BaoHanhs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaBaoHanh,MaKichCo,MaNguoiDung,NgayBaoHanh,NgayKetThuc,MoTa,TrangThai")] BaoHanh baoHanh)
        {
            if (ModelState.IsValid)
            {
                _context.Add(baoHanh);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaKichCo"] = new SelectList(_context.KichCos, "MaKichCo", "MaKichCo", baoHanh.MaKichCo);
            ViewData["MaNguoiDung"] = new SelectList(_context.NguoiDungs, "MaNguoiDung", "AnhDaiDien", baoHanh.MaNguoiDung);
            return View(baoHanh);
        }

        // GET: Admin/BaoHanhs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var baoHanh = await _context.BaoHanhs.FindAsync(id);
            if (baoHanh == null)
            {
                return NotFound();
            }
            ViewData["MaKichCo"] = new SelectList(_context.KichCos, "MaKichCo", "MaKichCo", baoHanh.MaKichCo);
            ViewData["MaNguoiDung"] = new SelectList(_context.NguoiDungs, "MaNguoiDung", "AnhDaiDien", baoHanh.MaNguoiDung);
            return View(baoHanh);
        }

        // POST: Admin/BaoHanhs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaBaoHanh,MaKichCo,MaNguoiDung,NgayBaoHanh,NgayKetThuc,MoTa,TrangThai")] BaoHanh baoHanh)
        {
            if (id != baoHanh.MaBaoHanh)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(baoHanh);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BaoHanhExists(baoHanh.MaBaoHanh))
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
            ViewData["MaKichCo"] = new SelectList(_context.KichCos, "MaKichCo", "MaKichCo", baoHanh.MaKichCo);
            ViewData["MaNguoiDung"] = new SelectList(_context.NguoiDungs, "MaNguoiDung", "AnhDaiDien", baoHanh.MaNguoiDung);
            return View(baoHanh);
        }

        // GET: Admin/BaoHanhs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var baoHanh = await _context.BaoHanhs
                .Include(b => b.MaKichCoNavigation)
                .Include(b => b.MaNguoiDungNavigation)
                .FirstOrDefaultAsync(m => m.MaBaoHanh == id);
            if (baoHanh == null)
            {
                return NotFound();
            }

            return View(baoHanh);
        }

        // POST: Admin/BaoHanhs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var baoHanh = await _context.BaoHanhs.FindAsync(id);
            if (baoHanh != null)
            {
                _context.BaoHanhs.Remove(baoHanh);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BaoHanhExists(int id)
        {
            return _context.BaoHanhs.Any(e => e.MaBaoHanh == id);
        }
    }
}
