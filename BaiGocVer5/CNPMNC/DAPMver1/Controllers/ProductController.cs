using DAPMver1.Data;
using DAPMver1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DAPMver1.Controllers
{
    public class ProductController : Controller
    {
        private readonly DapmTrangv1Context db;
        private const int PageSize = 10; // Số lượng sản phẩm trên mỗi trang

        public ProductController(DapmTrangv1Context context)
        {
            this.db = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IActionResult Index(int? categoryId)
        {
            var danhMucList = db.DanhMucs.ToList();
            ViewBag.DanhMucList = danhMucList;
            ViewBag.SelectedDanhMuc = categoryId;

            // Lọc sản phẩm theo danh mục (nếu có)
            var sanPhams = categoryId == null
                ? db.SanPhams.ToList()
                : db.SanPhams.Where(sp => sp.MaDanhMuc == categoryId).ToList();

            return View(sanPhams);
        }
        public IActionResult PhanLoai(int? categoryId)
        {
            var danhMucList = db.DanhMucs.ToList();
            ViewBag.DanhMucList = danhMucList;
            ViewBag.SelectedDanhMuc = categoryId;

            // Lọc sản phẩm theo danh mục (nếu có)
            var sanPhams = categoryId == null
                ? db.SanPhams.ToList()
                : db.SanPhams.Where(sp => sp.MaDanhMuc == categoryId).ToList();

            return View(sanPhams);
        }


        public ActionResult Search(string keyword)
        {

            if (!string.IsNullOrEmpty(keyword))
            {
                // Tìm kiếm sản phẩm theo tên
                var sanPhams = db.SanPhams
                                 .Where(sp => sp.TenSanPham.Contains(keyword))
                                 .OrderByDescending(sp => sp.NgayTao)
                                 .ToList();
                ViewBag.Keyword = keyword;
                return View("Search", sanPhams);
            }


            return RedirectToAction("Index");
        }

        public IActionResult ChiTietSP(int id)
        {
            // Nạp sản phẩm và bao gồm các thông tin liên quan đến danh mục, vật liệu, nhà cung cấp và đánh giá
            var data = db.SanPhams
                .Include(s => s.KichCos)
                .Include(s => s.MaDanhMucNavigation) // Bao gồm thông tin danh mục
                .Include(s => s.MaVatLieuNavigation) // Bao gồm thông tin vật liệu
                .Include(s => s.MaNhaCungCapNavigation) // Bao gồm thông tin nhà cung cấp
                .Include(s => s.DanhGiaSanPhams) // Bao gồm danh sách đánh giá của sản phẩm
                .Include(s => s.BinhLuans).ThenInclude(n=>n.MaNguoiDungNavigation) // Nạp danh sách bình luận
                .FirstOrDefault(s => s.MaSanPham == id);

            if (data == null)
            {
                TempData["Message"] = $"Không thấy sản phẩm có mã {id}";
                return Redirect("/404");
            }

            // Lấy danh sách sản phẩm gợi ý theo danh mục
            var goiYSP = db.SanPhams
                .Where(sp => sp.MaDanhMuc == data.MaDanhMuc && sp.MaSanPham != data.MaSanPham)
                .OrderByDescending(sp => sp.NgayTao)
                .Take(5) // Lấy 5 sản phẩm mới nhất
                .ToList();

            // Lấy danh sách sản phẩm gợi ý theo giá cao nhất
            var goiYTheoGiaCaoNhat = db.SanPhams
                .Where(sp => sp.MaDanhMuc == data.MaDanhMuc && sp.MaSanPham != data.MaSanPham)
                .OrderByDescending(sp => sp.GiaTienMoi)
                .Take(5)
                .ToList();

            // Tính điểm trung bình đánh giá
            var diemTrungBinh = data.DanhGiaSanPhams.Any()
                ? data.DanhGiaSanPhams.Average(dg => dg.DiemDanhGia)
                : 0; // Trường hợp không có đánh giá thì trả về 0

            var viewModel = new ChiTietSanPhamViewModels
            {
                SanPham = data,
                GoiYSanPhams = goiYSP,
                GoiYSanPhamsTheoGiaCaoNhat = goiYTheoGiaCaoNhat,
                DiemTrungBinh = diemTrungBinh // Truyền điểm trung bình vào ViewModel
            };

            return View("ChiTietSP", viewModel); // Gọi rõ ràng View "ChiTietSanPham"
        }
        [HttpPost]
        public IActionResult AddComment(int MaSanPham, string NoiDung)
        {
            // Lấy thông tin người dùng hiện tại (giả sử bạn đã có cơ chế đăng nhập)
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserID")))
            {
               
                return RedirectToAction("Login", "Account");
            }
            var userId = int.Parse(HttpContext.Session.GetString("UserID")); 
            var existingProduct = db.ChiTietDonHangs.Include(m=>m.MaDonHangNavigation).Include(m=>m.MaKichCoNavigation).ThenInclude(k=>k.MaSanPhamNavigation)
                          .FirstOrDefault(c => c.MaKichCoNavigation.MaSanPham == MaSanPham
                                            && c.MaDonHangNavigation.MaNguoiGui == userId
                                             && c.MaDonHangNavigation.TrangThai != "Đã Giao"); // Kiểm tra đơn hàng chưa giao
            if (existingProduct == null)
            {
                TempData["Message"] = "Sản phẩm này chưa có trong đơn hàng của bạn.";
                return RedirectToAction("ChiTietSP", new { id = MaSanPham });
            }

            // Kiểm tra trạng thái đơn hàng
            if (existingProduct.MaDonHangNavigation.TrangThai != "Đã nhận")
            {
                TempData["Message"] = "Bạn cần nhận đơn hàng để bình luận";
                return RedirectToAction("ChiTietSP", new { id = MaSanPham });
            }
            if (!string.IsNullOrEmpty(NoiDung))
            {
                var binhLuan = new BinhLuan
                {
                    MaSanPham = MaSanPham,
                    NoiDung = NoiDung,
                    NgayBinhLuan = DateTime.Now,
                    MaNguoiDung = userId
                };

                db.BinhLuans.Add(binhLuan);
                db.SaveChanges();

                // Quay lại chi tiết sản phẩm sau khi thêm bình luận
                return RedirectToAction("ChiTietSP", new { id = MaSanPham });
            }

            // Nếu không có nội dung bình luận, quay lại chi tiết sản phẩm
            TempData["Message"] = "Bình luận không được để trống!";
            return RedirectToAction("ChiTietSP", new { id = MaSanPham });
        }
        [HttpPost]
        public IActionResult ThemDanhGia(int maSanPham, int diemDanhGia, string binhLuan)
        {
            if (HttpContext.Session.GetString("UserID") == null)
            {
                // Redirect to login or show an error if the user is not logged in
                TempData["ErrorMessage"] = "Bạn cần đăng nhập để đánh giá sản phẩm.";
                return RedirectToAction("ChiTietSP", new { id = maSanPham });
            }
            if(diemDanhGia == 0)
            {
                TempData["ErrorMessage"] = "Bạn cần nhập số sao đánh giá.";
                return RedirectToAction("ChiTietSP", new { id = maSanPham });
            }

            var maNguoiDung = int.Parse(HttpContext.Session.GetString("UserID")); // Retrieve user ID from the session
            DanhGiaSanPham danhGia = new DanhGiaSanPham
            {
                MaSanPham = maSanPham,
                MaNguoiDung = maNguoiDung,
                DiemDanhGia = diemDanhGia,
                NgayDanhGia = DateTime.Now
            };

            db.DanhGiaSanPhams.Add(danhGia);
            db.SaveChanges();

            return RedirectToAction("ChiTietSP", new { id = maSanPham });
        }


    }
}
