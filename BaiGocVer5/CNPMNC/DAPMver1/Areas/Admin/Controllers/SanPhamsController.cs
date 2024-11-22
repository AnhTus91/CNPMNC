using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DAPMver1.Data;
using Org.BouncyCastle.Crypto.Macs;

namespace DAPMver1.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SanPhamsController : Controller
    {
        private readonly DapmTrangv1Context _context;

        public SanPhamsController(DapmTrangv1Context context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString, int? maDanhMuc)
        {
            IQueryable<SanPham> sanPham = _context.SanPhams.Include(n=>n.KichCos)
                .Include(s => s.MaDanhMucNavigation)
                .Include(s => s.MaNhaCungCapNavigation)
                .Include(s => s.MaVatLieuNavigation);
            int? maChucVu = HttpContext.Session.GetInt32("MaChucVu");
            ViewData["MaChucVu"] = maChucVu;
            // Lọc theo danh mục nếu được chọn.
            if (maDanhMuc.HasValue && maDanhMuc.Value > 0)
            {
                sanPham = sanPham.Where(sp => sp.MaDanhMuc == maDanhMuc.Value);
                var category = await _context.DanhMucs.FindAsync(maDanhMuc.Value);
                ViewBag.Title = category?.TenDanhMuc ?? "Danh sách sản phẩm";
            }
            else
            {
                ViewBag.Title = "Tất cả sản phẩm";
            }

            // Lọc theo từ khóa tìm kiếm nếu có
            if (!string.IsNullOrEmpty(searchString))
            {
                sanPham = sanPham.Where(sp => sp.TenSanPham.Contains(searchString)); // Hoặc sử dụng thuộc tính phù hợp
            }

            // Gán danh sách danh mục và danh mục được chọn vào ViewBag.
            var danhMucs = await _context.DanhMucs.ToListAsync();
            ViewBag.DanhMucs = danhMucs.Select(d => new SelectListItem
            {
                Value = d.MaDanhMuc.ToString(),
                Text = d.TenDanhMuc
            }).ToList();
            ViewBag.SelectedDanhMuc = maDanhMuc;  // Truyền danh mục đã chọn

            return View(await sanPham.ToListAsync());
        }
        public ActionResult TaoKichCo()
        {
            ViewBag.SanPhams = _context.SanPhams.ToList(); // Lấy danh sách sản phẩm
            return View(new KichCo());
        }
        // GET: Admin/LichSuGiaSanPhams
        public async Task<IActionResult> IndexGiamGia()
        {
            var dapmTrangv1Context = _context.LichSuGiamGiaSanPhams.Include(l => l.MaSanPhamNavigation);
            return View(await dapmTrangv1Context.ToListAsync());
        }
        // POST: Admin/SanPhams/TaoKichCo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TaoKichCo(KichCo kichCoViewModel)
        {
            if (ModelState.IsValid)
            {
                bool isExist = _context.KichCos.Any(k => k.MaSanPham == kichCoViewModel.MaSanPham && k.SoKichCo == kichCoViewModel.SoKichCo);

                if (isExist)
                {
                    // Nếu kích cỡ đã tồn tại, thông báo lỗi
                    ModelState.AddModelError("SoKichCo", "Số kích cỡ này đã tồn tại cho sản phẩm này. Vui lòng nhập số kích cỡ khác.");

                    // Lấy lại danh sách sản phẩm để hiển thị lại trên form
                    ViewBag.SanPhams = _context.SanPhams.ToList();
                    return View(kichCoViewModel);
                }
                var kichCo = new KichCo
                {
                    MaSanPham = kichCoViewModel.MaSanPham,
                    SoKichCo = kichCoViewModel.SoKichCo,
                    SoLuong = kichCoViewModel.SoLuong
                };

                _context.KichCos.Add(kichCo);
                _context.SaveChanges();
                return RedirectToAction("Index"); // Redirect đến trang danh sách sản phẩm hoặc kích cỡ
            }

            // Nếu ModelState không hợp lệ, cần phải lấy lại danh sách sản phẩm
            ViewBag.SanPhams = _context.SanPhams.ToList(); // Lấy lại danh sách sản phẩm nếu có lỗi
            return View(kichCoViewModel);
        }
        // GET: Admin/SanPhams/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sanPham = await _context.SanPhams
                .Include(s => s.MaDanhMucNavigation)
                .Include(s => s.MaNhaCungCapNavigation)
                .Include(s => s.MaVatLieuNavigation)
                .FirstOrDefaultAsync(m => m.MaSanPham == id);
            if (sanPham == null)
            {
                return NotFound();
            }

            return View(sanPham);
        }

        // GET: Admin/SanPhams/Create
        public IActionResult Create()
        {
            ViewData["MaDanhMuc"] = new SelectList(_context.DanhMucs, "MaDanhMuc", "TenDanhMuc");
            ViewData["MaNhaCungCap"] = new SelectList(_context.NhaCungCaps, "MaNhaCungCap", "TenNhaCungCap");
            ViewData["MaVatLieu"] = new SelectList(_context.VatLieus, "MaVatLieu", "TenVatlieu");

            var sanPham = new SanPham
            {
                NgayTao = DateOnly.FromDateTime(DateTime.Now)
        };
            return View();
        }



        // POST: Admin/SanPhams/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SanPham sanPham, IFormFile file)
        {
            if (ModelState.IsValid)
            {
             
                // Kiểm tra xem file có được upload không
                if (file != null && file.Length > 0)
                {
                    // Tạo tên file duy nhất để tránh xung đột
                    var fileName = Path.GetFileName(file.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                    // Lưu file vào thư mục wwwroot/images
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // Gán đường dẫn của ảnh cho thuộc tính AnhSp của sản phẩm
                    sanPham.AnhSp = "/images/" + fileName; // Đảm bảo đường dẫn hợp lệ
                }
                sanPham.GiaTienCu = 0;
                sanPham.NgayTao = DateOnly.FromDateTime(DateTime.Now); // Gán ngày tạo
                _context.Add(sanPham); // Thêm sản phẩm vào DbContext
                await _context.SaveChangesAsync(); // Lưu thay đổi vào cơ sở dữ liệu
                return RedirectToAction(nameof(Index)); // Chuyển hướng đến trang index
            }

            // Nếu có lỗi, trả lại view với thông tin sản phẩm
            return View(sanPham);
        }
        // GET: Admin/SanPhams/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sanPham = await _context.SanPhams.FindAsync(id);
            if (sanPham == null)
            {
                return NotFound();
            }
            ViewData["MaDanhMuc"] = new SelectList(_context.DanhMucs, "MaDanhMuc", "MaDanhMuc", sanPham.MaDanhMuc);
            ViewData["MaNhaCungCap"] = new SelectList(_context.NhaCungCaps, "MaNhaCungCap", "MaNhaCungCap", sanPham.MaNhaCungCap);
            ViewData["MaVatLieu"] = new SelectList(_context.VatLieus, "MaVatLieu", "MaVatLieu", sanPham.MaVatLieu);
            return View(sanPham);
        }

        // POST: Admin/SanPhams/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaSanPham,TenSanPham,GiaTienMoi,GiaTienCu,MoTa,AnhSp,MaVatLieu,MaDanhMuc,NgayTao,MaNhaCungCap")] 
        SanPham sanPham, IFormFile file)
        {
            if (id != sanPham.MaSanPham)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existingSanPham = await _context.SanPhams.FindAsync(id);
                if (existingSanPham == null)
                {
                    return NotFound();
                }

                // Lưu giá cũ để tạo bản ghi lịch sử
                int giaCu = existingSanPham.GiaTienMoi;
                string anhSp = existingSanPham.AnhSp;
                try
                {
                    // Kiểm tra xem có tệp hình ảnh mới không
                    if (file != null && file.Length > 0)
                    {
                        // Tạo tên file duy nhất để tránh xung đột
                        var fileName = Path.GetFileName(file.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                        // Xóa hình ảnh cũ (nếu cần)
                      
                        // Lưu file vào thư mục wwwroot/images
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        // Gán đường dẫn của ảnh mới cho thuộc tính AnhSp
                        existingSanPham.AnhSp = "/images/" + fileName; // Đảm bảo đường dẫn hợp lệ
                    }
                    else
                    {
                        existingSanPham.AnhSp = anhSp;
                    }
                    // Kiểm tra và lưu lịch sử giá sản phẩm
                    if (sanPham.GiaTienMoi != giaCu)
                    {
                        var lichSuGia = new LichSuGiaSanPham
                        {
                            MaSanPham = sanPham.MaSanPham,
                            GiaCu = giaCu,
                            GiaMoi = sanPham.GiaTienMoi,
                            NgayCapNhat = DateTime.Now
                        };

                        _context.LichSuGiaSanPhams.Add(lichSuGia);
                        var lichSuGiamGia = new LichSuGiamGiaSanPham
                        {
                            MaSanPham = sanPham.MaSanPham,
                            GiaGiamCu = lichSuGia.GiaMoi - lichSuGia.GiaCu,
                            GiaGiamMoi = sanPham.GiaTienMoi - sanPham.GiaTienCu,
                            NgayCapNhat = DateTime.Now
                        };
                        _context.LichSuGiamGiaSanPhams.Add(lichSuGiamGia);
                    }

                    // Cập nhật thông tin sản phẩm, chỉ cập nhật giá mới
                    existingSanPham.GiaTienMoi = sanPham.GiaTienMoi; // Cập nhật giá mới
                    existingSanPham.GiaTienCu = giaCu; // Cập nhật giá mới
                    existingSanPham.TenSanPham = sanPham.TenSanPham;
                    existingSanPham.MaVatLieu = sanPham.MaVatLieu;
                    existingSanPham.MaDanhMuc = sanPham.MaDanhMuc;
                    existingSanPham.MaNhaCungCap = sanPham.MaNhaCungCap;
                    existingSanPham.MoTa = sanPham.MoTa;
                 
                    // Cập nhật thông tin sản phẩm
                    _context.Update(existingSanPham);
                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SanPhamExists(sanPham.MaSanPham))
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
            ViewData["MaDanhMuc"] = new SelectList(_context.DanhMucs, "MaDanhMuc", "MaDanhMuc", sanPham.MaDanhMuc);
            ViewData["MaNhaCungCap"] = new SelectList(_context.NhaCungCaps, "MaNhaCungCap", "MaNhaCungCap", sanPham.MaNhaCungCap);
            ViewData["MaVatLieu"] = new SelectList(_context.VatLieus, "MaVatLieu", "MaVatLieu", sanPham.MaVatLieu);
            return View(sanPham);
        }

        // GET: Admin/SanPhams/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sanPham = await _context.SanPhams
                .Include(s => s.MaDanhMucNavigation)
                .Include(s => s.MaNhaCungCapNavigation)
                .Include(s => s.MaVatLieuNavigation)
                .FirstOrDefaultAsync(m => m.MaSanPham == id);
            if (sanPham == null)
            {
                return NotFound();
            }

            return View(sanPham);
        }

        // POST: Admin/SanPhams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sanPham = await _context.SanPhams.FindAsync(id);
            if (sanPham != null)
            {
                _context.SanPhams.Remove(sanPham);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SanPhamExists(int id)
        {
            return _context.SanPhams.Any(e => e.MaSanPham == id);
        }
        public IActionResult CheckProductName(string productName)
        {
            var exists = _context.SanPhams.Any(sp => sp.TenSanPham.Equals(productName, StringComparison.OrdinalIgnoreCase));
            return Json(new { exists });
        }

    }
}
