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
    public class NguoiDungsController : Controller
    {
        private readonly DapmTrangv1Context _context;

        public NguoiDungsController(DapmTrangv1Context context)
        {
            _context = context;
        }

        // GET: Admin/NguoiDungs
        public async Task<IActionResult> Index()
        {
            var dapmTrangv1Context = _context.NguoiDungs.Include(n => n.MaChucVuNavigation).Include(n => n.MaTaiKhoanNavigation);
            return View(await dapmTrangv1Context.ToListAsync());
        }

        // GET: Admin/NguoiDungs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nguoiDung = await _context.NguoiDungs
                .Include(n => n.MaChucVuNavigation)
                .Include(n => n.MaTaiKhoanNavigation)
                .FirstOrDefaultAsync(m => m.MaNguoiDung == id);
            if (nguoiDung == null)
            {
                return NotFound();
            }

            return View(nguoiDung);
        }

        // GET: Admin/NguoiDungs/Create
        public IActionResult Create()
        {
            ViewData["MaChucVu"] = new SelectList(_context.ChucVus, "MaChucVu", "MaChucVu");
            ViewData["MaTaiKhoan"] = new SelectList(_context.TaiKhoans, "MaTaiKhoan", "Email");
            return View();
        }

        // POST: Admin/NguoiDungs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaNguoiDung,TenNguoiDung,DiaChi,Sdt,AnhDaiDien,MaChucVu,MaTaiKhoan")] NguoiDung nguoiDung)
        {
            if (ModelState.IsValid)
            {
                _context.Add(nguoiDung);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaChucVu"] = new SelectList(_context.ChucVus, "MaChucVu", "MaChucVu", nguoiDung.MaChucVu);
            ViewData["MaTaiKhoan"] = new SelectList(_context.TaiKhoans, "MaTaiKhoan", "Email", nguoiDung.MaTaiKhoan);
            return View(nguoiDung);
        }

        // GET: Admin/NguoiDungs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nguoiDung = await _context.NguoiDungs.FindAsync(id);
            if (nguoiDung == null)
            {
                return NotFound();
            }
            ViewData["MaChucVu"] = new SelectList(_context.ChucVus, "MaChucVu", "MaChucVu", nguoiDung.MaChucVu);
            ViewData["MaTaiKhoan"] = new SelectList(_context.TaiKhoans, "MaTaiKhoan", "Email", nguoiDung.MaTaiKhoan);
            return View(nguoiDung);
        }

        // POST: Admin/NguoiDungs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaNguoiDung,TenNguoiDung,DiaChi,Sdt,AnhDaiDien,MaChucVu,MaTaiKhoan")] NguoiDung nguoiDung)
        {
            if (id != nguoiDung.MaNguoiDung)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(nguoiDung);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NguoiDungExists(nguoiDung.MaNguoiDung))
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
            ViewData["MaChucVu"] = new SelectList(_context.ChucVus, "MaChucVu", "MaChucVu", nguoiDung.MaChucVu);
            ViewData["MaTaiKhoan"] = new SelectList(_context.TaiKhoans, "MaTaiKhoan", "Email", nguoiDung.MaTaiKhoan);
            return View(nguoiDung);
        }

        // GET: Admin/NguoiDungs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nguoiDung = await _context.NguoiDungs
                .Include(n => n.MaChucVuNavigation)
                .Include(n => n.MaTaiKhoanNavigation)
                .FirstOrDefaultAsync(m => m.MaNguoiDung == id);
            if (nguoiDung == null)
            {
                return NotFound();
            }

            return View(nguoiDung);
        }

        // POST: Admin/NguoiDungs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var nguoiDung = await _context.NguoiDungs.FindAsync(id);
            if (nguoiDung != null)
            {
                _context.NguoiDungs.Remove(nguoiDung);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NguoiDungExists(int id)
        {
            return _context.NguoiDungs.Any(e => e.MaNguoiDung == id);
        }
        private bool TaiKhoanExists(int id)
        {
            return _context.TaiKhoans.Any(e => e.MaTaiKhoan == id);
        }
        // GET: Admin/TaiKhoans/Disable/5
        public async Task<IActionResult> Disable(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taiKhoan = await _context.TaiKhoans.FindAsync(id);
            if (taiKhoan == null)
            {
                return NotFound();
            }

            return View(taiKhoan); // Hiển thị view để xác nhận việc vô hiệu hóa
        }

        // POST: Admin/TaiKhoans/Disable/5
        [HttpPost, ActionName("Disable")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DisableConfirmed(int id)
        {
            var taiKhoan = await _context.TaiKhoans.FindAsync(id);
            if (taiKhoan == null)
            {
                return NotFound();
            }

            taiKhoan.VaiTro = true; // Cập nhật trạng thái tài khoản thành không kích hoạt
            _context.Update(taiKhoan);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Tài khoản đã được vô hiệu hóa thành công.";
            return RedirectToAction(nameof(Index));
        }
        // GET: Admin/TaiKhoans/Enable/5
        public async Task<IActionResult> Enable(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taiKhoan = await _context.TaiKhoans.FindAsync(id);
            if (taiKhoan == null)
            {
                return NotFound();
            }

            return View(taiKhoan); // Hiển thị view để xác nhận việc kích hoạt
        }

        // POST: Admin/TaiKhoans/Enable/5
        [HttpPost, ActionName("Enable")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnableConfirmed(int id)
        {
            var taiKhoan = await _context.TaiKhoans.FindAsync(id);
            if (taiKhoan == null)
            {
                return NotFound();
            }

            taiKhoan.VaiTro = false; // Đổi trạng thái sang kích hoạt
            _context.Update(taiKhoan);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Tài khoản đã được kích hoạt thành công.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/NguoiDungs/ChangeStatus/5
        public async Task<IActionResult> ChangeStatus(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nguoiDung = await _context.NguoiDungs
                .Include(n => n.MaTaiKhoanNavigation) // Include the related TaiKhoan
                .FirstOrDefaultAsync(n => n.MaNguoiDung == id);
            if (nguoiDung == null)
            {
                return NotFound();
            }

            return View(nguoiDung);
        }

        // POST: Admin/NguoiDungs/ChangeStatus/5
        [HttpPost, ActionName("ChangeStatus")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatusConfirmed(int id)
        {
            var nguoiDung = await _context.NguoiDungs
                .Include(n => n.MaTaiKhoanNavigation) // Include the related TaiKhoan
                .FirstOrDefaultAsync(n => n.MaNguoiDung == id);
            if (nguoiDung == null)
            {
                return NotFound();
            }

            // Chuyển đổi trạng thái VaiTro
            if (nguoiDung.MaTaiKhoanNavigation != null)
            {
                nguoiDung.MaTaiKhoanNavigation.VaiTro = !nguoiDung.MaTaiKhoanNavigation.VaiTro;

                // Cập nhật và lưu lại trạng thái mới
                _context.Update(nguoiDung.MaTaiKhoanNavigation);
                await _context.SaveChangesAsync();

                // Thông báo thành công dựa trên trạng thái mới
                string statusMessage = nguoiDung.MaTaiKhoanNavigation.VaiTro
                    ? "Tài khoản đã được vô hiệu hóa thành công."
                    : "Tài khoản đã được kích hoạt thành công.";
                TempData["SuccessMessage"] = statusMessage;
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể thay đổi trạng thái tài khoản vì thông tin liên quan không tồn tại.";
            }

            return RedirectToAction(nameof(Index));
        }

    }
}