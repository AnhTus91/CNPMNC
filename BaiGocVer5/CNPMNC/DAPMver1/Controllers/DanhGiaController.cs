using DAPMver1.Data;
using Microsoft.AspNetCore.Mvc;

namespace DAPMver1.Controllers
{
    public class DanhGiaController : Controller
    {
        private readonly DapmTrangv1Context _context;
        public DanhGiaController(DapmTrangv1Context context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(int maSanPham, int diemDanhGia)
        {
            // Lấy thông tin người dùng từ session
            var maTaiKhoan = HttpContext.Session.GetString("MaTaiKhoan");
            var userId = HttpContext.Session.GetString("UserID");

            if (maTaiKhoan == null || userId == null)
            {
                // Nếu chưa đăng nhập, chuyển hướng đến trang đăng nhập
                return RedirectToAction("Login", "Account");
            }

            // Tạo một đối tượng đánh giá sản phẩm mới
            var danhGia = new DanhGiaSanPham
            {
                MaSanPham = maSanPham,
                MaNguoiDung = int.Parse(userId), // Chuyển đổi sang int nếu cần
                DiemDanhGia = diemDanhGia,
                NgayDanhGia = DateTime.Now
            };

            // Lưu đánh giá vào cơ sở dữ liệu
            _context.DanhGiaSanPhams.Add(danhGia);
            _context.SaveChanges();

            // Chuyển hướng về trang chi tiết sản phẩm
            return RedirectToAction("ChiTietSP", "Product", new { id = maSanPham });
        }
    }
}
