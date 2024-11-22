using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DAPMver1.Data;
using DAPMver1.Helpers;
using DAPMver1.Models;

namespace DAPMver1.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DonHangsController : Controller
    {
        private readonly DapmTrangv1Context _context;
        private readonly DonHangServices _donHangService;
        private const int PageSize = 10; // Số đơn hàng hiển thị trên mỗi trang

        public DonHangsController(DapmTrangv1Context context, DonHangServices donHangService)
        {
            _context = context;
            _donHangService = donHangService;
        }

        // GET: Admin/DonHangs
        public async Task<IActionResult> Index( string trangThai)
        {
            // Lấy danh sách trạng thái để hiển thị trong dropdown
            var trangThaiList = new List<SelectListItem>
{
    new SelectListItem { Text = "Tất cả", Value = "" },
    new SelectListItem { Text = "Chờ xử lý", Value = "Chờ xử lý" },
     new SelectListItem { Text = "Thanh toán PayPal", Value = "Đang chờ thanh toán PayPal" },
    new SelectListItem { Text = "Đang chuẩn bị", Value = "Đang chuẩn bị" },
    new SelectListItem { Text = "Đã nhận", Value = "Đã nhận" },
    new SelectListItem { Text = "Hủy", Value = "Đã hủy" }
};

            // Gán danh sách trạng thái vào ViewBag để hiển thị dropdown trong view
            ViewBag.TrangThaiList = trangThaiList;
            ViewBag.SelectedTrangThai = trangThai;

            // Lọc đơn hàng theo trạng thái nếu có
            var donHangs = string.IsNullOrEmpty(trangThai)
                ? _context.DonHangs.ToList()
                : _context.DonHangs.Where(d => d.TrangThai == trangThai).ToList();

            return View(donHangs);

        }
        public async Task<List<DonHang>> GetDonHangByTrangThaiAsync(string trangThai)
        {
            return await _context.DonHangs
                .Where(dh => dh.TrangThai == trangThai)
                .ToListAsync();
        }
        // GET: Admin/DonHangs/Details/5
        public IActionResult Details(string id)
        {


            var donHang = _context.DonHangs
                .Include(d => d.MaNguoiGuiNavigation)
                .Include(d => d.MaVoucherNavigation).Include(n => n.ChiTietDonHangs)
                .ThenInclude(k => k.MaKichCoNavigation).ThenInclude(s => s.MaSanPhamNavigation)
                .Where(m => m.MaDonHang == id).ToList();


            return View(donHang);
        }

        // GET: Admin/DonHangs/Create


        private bool DonHangExists(string id)
        {
            return _context.DonHangs.Any(e => e.MaDonHang == id);
        }
        [HttpPost]
        [Route("admin/donhang/capnhattrangthai")]
        [HttpPost]
        public IActionResult CapNhatTrangThai(string id, string trangthai)
        {
            var donhang = _context.DonHangs.Find(id);
            if (donhang == null)
            {
                return NotFound();
            }

            // Kiểm tra nếu trạng thái là "Đã hủy" hoặc "Đã nhận", không cho phép cập nhật
            if (donhang.TrangThai == "Đã hủy" || donhang.TrangThai == "Đã nhận")
            {
                return Json(new { success = false, message = "Không thể cập nhật trạng thái đơn hàng đã hủy hoặc đã nhận." });
            }

            // Cập nhật trạng thái nếu không bị cấm
            donhang.TrangThai = trangthai;
            _context.SaveChanges();

            return Json(new { success = true });
        }

        [HttpPost]
        [Route("admin/donhang/locdonhang")]
        public IActionResult LocDonHang(string trangthai)
        {
            var donHangs = string.IsNullOrEmpty(trangthai)
                            ? _context.DonHangs.ToList()
                            : _context.DonHangs.Where(d => d.TrangThai == trangthai).ToList();

            return PartialView("_DonHangList", donHangs);  // Trả về PartialView chứa danh sách đơn hàng
        }


        // Xem chi tiết đơn hàng
        public IActionResult ChiTiet(string id)
        {
            var donhang = _context.DonHangs.Where(s => s.MaDonHang == id).ToList();

            if (donhang == null || !donhang.Any())
            {
                return NotFound();
            }

            return View(donhang);
        }

        // Cập nhật đơn hàng
        public IActionResult CapNhat(string id)
        {
            var donhang = _context.DonHangs.Where(s => s.MaDonHang == id).ToList();

            if (donhang == null || !donhang.Any())
            {
                return NotFound();
            }

            return View(donhang);
        }

        // Chấp nhận đơn hàng
        public IActionResult ChapNhan(string id)
        {
            var donhang = _context.DonHangs.Find(id);
            if (donhang != null)
            {
                donhang.TrangThai = "Đã duyệt";
                _context.SaveChanges();
                return RedirectToAction("Index"); // Chuyển đến trang danh sách đơn hàng
            }

            return NotFound();
        }

        // Hủy đơn hàng
        [HttpPost]
        public IActionResult HuyDonHang(string id)
        {
            var donhang = _context.DonHangs.Find(id);
            if (donhang != null)
            {
                donhang.TrangThai = "Đã hủy";
                _context.SaveChanges();
                return Json(new { success = true });
            }

            return NotFound();
        }

        // Giao hàng
        public IActionResult GiaoHang(string id)
        {
            var donhang = _context.DonHangs.Find(id);
            if (donhang != null)
            {
                donhang.TrangThai = "Đang giao";
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return NotFound();
        }

        // Đánh dấu đơn hàng thành công
        public IActionResult ThanhCong(string id)
        {
            var donhang = _context.DonHangs.Find(id);
            if (donhang != null)
            {
                donhang.TrangThai = "Thành công";
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return NotFound();
        }
        public IActionResult DoanhThu()
        {
            var doanhThuData = _context.DonHangs
                .GroupBy(dh => dh.NgayDatHang) // Group by date only (ignores time part)
                .Select(g => new DoanhThuStatisticsViewModel
                {
                    NgayDatHang = g.Key,
                    SoDonHang = g.Count() // Count orders per day
                })
                .ToList();

            return View(doanhThuData);
        }

        public IActionResult SoLuongDaBan()
        {
            // Retrieve the data from the database
            var soLuongBan = _context.ChiTietDonHangs
                .Join(_context.DonHangs, cdh => cdh.MaDonHang, dh => dh.MaDonHang, (cdh, dh) => new { cdh, dh })
                .Where(x => x.dh.TrangThai == "Đã Nhận")
                .Join(_context.SanPhams, x => x.cdh.MaKichCoNavigation.MaSanPham, sp => sp.MaSanPham, (x, sp) => new { x.cdh, sp })
                .GroupBy(x => new { x.sp.MaSanPham, x.sp.TenSanPham })
                .Select(g => new
                {
                    MaSanPham = g.Key.MaSanPham,
                    TenSanPham = g.Key.TenSanPham,
                    SoLuongDaBan = g.Sum(x => x.cdh.Soluong)
                })
                .OrderByDescending(x => x.SoLuongDaBan)
                .ToList();

            // Ensure soLuongBan is not empty before calculating max and min
            if (soLuongBan.Any())
            {
                var maxSoLuongDaBan = soLuongBan.Max(x => x.SoLuongDaBan);
                var minSoLuongDaBan = soLuongBan.Min(x => x.SoLuongDaBan);

                ViewBag.TenSanPham = soLuongBan.Select(x => x.TenSanPham).ToArray();
                ViewBag.SoLuongDaBan = soLuongBan.Select(x => x.SoLuongDaBan).ToArray();

                var danhGiaSanPham = soLuongBan.Select(x => new
                {
                    MaSanPham = x.MaSanPham,
                    TenSanPham = x.TenSanPham,
                    SoLuongDaBan = x.SoLuongDaBan,
                    NhanXet = GetProductComment(x.SoLuongDaBan, maxSoLuongDaBan, minSoLuongDaBan), // Truyền maxSoLuongDaBan và minSoLuongDaBan vào
                    Url = Url.Action("ChiTietSP", "Product", new { id = x.MaSanPham })
                }).ToList();

                ViewBag.DanhGiaSanPham = danhGiaSanPham;
            }
            else
            {
                // Handle the case when no data is available
                ViewBag.TenSanPham = new string[] { };
                ViewBag.SoLuongDaBan = new double[] { };
                ViewBag.DanhGiaSanPham = new List<object>(); // Or any appropriate fallback
            }

            return View();
        }

        // Hàm nhận xét sản phẩm, với maxSoLuongDaBan và minSoLuongDaBan để xác định sản phẩm bán chạy nhất và ít được mua nhất
        private string GetProductComment(int soLuongDaBan, int maxSoLuongDaBan, int minSoLuongDaBan)
        {
            if (soLuongDaBan == maxSoLuongDaBan)
                return "Sản phẩm bán chạy nhất!";
            else if (soLuongDaBan == minSoLuongDaBan)
                return "Sản phẩm ít được mua.";
            else if (soLuongDaBan >= 50)
                return "Sản phẩm khá phổ biến.";
            else if (soLuongDaBan >= 20)
                return "Sản phẩm đang được ưa chuộng.";
            else
                return "Sản phẩm ít được mua.";
        }
      


    }
}
    

