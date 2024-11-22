using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DAPMver1.Data;

namespace DAPMver1.Areas.Admin
{
    [Area("Admin")]
    public class HoaDonsController : Controller
    {
        private readonly DapmTrangv1Context _context;

        public HoaDonsController(DapmTrangv1Context context)
        {
            _context = context;
        }

        // GET: Admin/HoaDons
        public async Task<IActionResult> Index()
        {
            var dapmTrangv1Context = _context.HoaDons.Include(h => h.MaDonHangNavigation).Include(h => h.MaNguoiDungNavigation).Include(h => h.MaThanhToanNavigation);
            return View(await dapmTrangv1Context.ToListAsync());
        }

        // GET: Admin/HoaDons/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoaDon = await _context.HoaDons
                .Include(h => h.MaDonHangNavigation)
                .Include(h => h.MaNguoiDungNavigation)
                .Include(h => h.MaThanhToanNavigation)
                .FirstOrDefaultAsync(m => m.MaHoaDon == id);
            if (hoaDon == null)
            {
                return NotFound();
            }

            return View(hoaDon);
        }

        // GET: Admin/HoaDons/Create
        public IActionResult Create()
        {
            ViewData["MaDonHang"] = new SelectList(_context.DonHangs, "MaDonHang", "MaDonHang");
            ViewData["MaNguoiDung"] = new SelectList(_context.NguoiDungs, "MaNguoiDung", "AnhDaiDien");
            ViewData["MaThanhToan"] = new SelectList(_context.ThanhToans, "MaThanhToan", "MaDonHang");
            return View();
        }

        // POST: Admin/HoaDons/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaHoaDon,MaDonHang,MaNguoiDung,NgayXuatHoaDon,TongTien,MaThanhToan")] HoaDon hoaDon)
        {
            if (ModelState.IsValid)
            {
                _context.Add(hoaDon);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaDonHang"] = new SelectList(_context.DonHangs, "MaDonHang", "MaDonHang", hoaDon.MaDonHang);
            ViewData["MaNguoiDung"] = new SelectList(_context.NguoiDungs, "MaNguoiDung", "AnhDaiDien", hoaDon.MaNguoiDung);
            ViewData["MaThanhToan"] = new SelectList(_context.ThanhToans, "MaThanhToan", "MaDonHang", hoaDon.MaThanhToan);
            return View(hoaDon);
        }

        // GET: Admin/HoaDons/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoaDon = await _context.HoaDons.FindAsync(id);
            if (hoaDon == null)
            {
                return NotFound();
            }
            ViewData["MaDonHang"] = new SelectList(_context.DonHangs, "MaDonHang", "MaDonHang", hoaDon.MaDonHang);
            ViewData["MaNguoiDung"] = new SelectList(_context.NguoiDungs, "MaNguoiDung", "AnhDaiDien", hoaDon.MaNguoiDung);
            ViewData["MaThanhToan"] = new SelectList(_context.ThanhToans, "MaThanhToan", "MaDonHang", hoaDon.MaThanhToan);
            return View(hoaDon);
        }

        // POST: Admin/HoaDons/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaHoaDon,MaDonHang,MaNguoiDung,NgayXuatHoaDon,TongTien,MaThanhToan")] HoaDon hoaDon)
        {
            if (id != hoaDon.MaHoaDon)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hoaDon);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HoaDonExists(hoaDon.MaHoaDon))
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
            ViewData["MaDonHang"] = new SelectList(_context.DonHangs, "MaDonHang", "MaDonHang", hoaDon.MaDonHang);
            ViewData["MaNguoiDung"] = new SelectList(_context.NguoiDungs, "MaNguoiDung", "AnhDaiDien", hoaDon.MaNguoiDung);
            ViewData["MaThanhToan"] = new SelectList(_context.ThanhToans, "MaThanhToan", "MaDonHang", hoaDon.MaThanhToan);
            return View(hoaDon);
        }

        // GET: Admin/HoaDons/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoaDon = await _context.HoaDons
                .Include(h => h.MaDonHangNavigation)
                .Include(h => h.MaNguoiDungNavigation)
                .Include(h => h.MaThanhToanNavigation)
                .FirstOrDefaultAsync(m => m.MaHoaDon == id);
            if (hoaDon == null)
            {
                return NotFound();
            }

            return View(hoaDon);
        }

        // POST: Admin/HoaDons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var hoaDon = await _context.HoaDons.FindAsync(id);
            if (hoaDon != null)
            {
                _context.HoaDons.Remove(hoaDon);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HoaDonExists(string id)
        {
            return _context.HoaDons.Any(e => e.MaHoaDon == id);
        }
    }
}
