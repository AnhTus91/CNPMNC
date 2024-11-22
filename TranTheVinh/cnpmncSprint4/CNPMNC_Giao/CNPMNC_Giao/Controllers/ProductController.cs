using CNPMNC_Giao.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CNPMNC_Giao.Controllers
{
    public class ProductController : Controller
    {
        private readonly DAPMEntities db = new DAPMEntities();
        // GET: Product
        public ActionResult Index(int? categoryId)
        {
            // Lấy danh sách danh mục để hiển thị trên dropdown
            ViewBag.DanhMucList = db.DanhMuc.ToList();

            // Lấy danh sách sản phẩm
            var danhSachSanPham = db.SanPham.AsQueryable();

            // Nếu categoryId có giá trị, lọc sản phẩm theo danh mục
            if (categoryId.HasValue && categoryId.Value > 0)
            {
                danhSachSanPham = danhSachSanPham.Where(sp => sp.MaDanhMuc == categoryId.Value);

                // Lấy tên danh mục đã chọn để hiển thị tiêu đề
                var category = db.DanhMuc.Find(categoryId.Value);
                ViewBag.Title = category?.TenDanhMuc; // Cập nhật tiêu đề
            }
            else
            {
                ViewBag.Title = "Tất cả sản phẩm"; // Tiêu đề mặc định
            }

            // Truyền danh sách vào View
            return View(danhSachSanPham.ToList());
        }
        public ActionResult Search(string keyword)
        {
            // Kiểm tra nếu keyword có giá trị
            if (!string.IsNullOrEmpty(keyword))
            {
                // Tìm kiếm sản phẩm theo tên
                var sanPhams = db.SanPham
                                 .Where(sp => sp.TenSanPham.Contains(keyword))
                                 .OrderByDescending(sp => sp.NgayTao)
                                 .ToList();
                ViewBag.Keyword = keyword; // Gán từ khóa vào ViewBag để sử dụng trong View
                return View("Search", sanPhams); // Trả về danh sách sản phẩm tìm được
            }

            // Nếu không có từ khóa, trả về toàn bộ sản phẩm
            return RedirectToAction("Index");
        }
        public ActionResult ChiTietSP(int? id)
        {
            // Lấy thông tin sản phẩm cùng với đánh giá và người dùng
            var sanPham = db.SanPham
                .Include("DanhGiaSanPham.NguoiDung")
                .Include(s => s.KichCo)
                .FirstOrDefault(s => s.MaSanPham == id);

            if (sanPham == null)
            {
                ViewBag.error = "Sản phẩm không tồn tại";
                return RedirectToAction("Index", "Home");
            }

            // Lấy danh sách đánh giá cho sản phẩm
            var danhGiaList = db.DanhGiaSanPham
                .Where(dg => dg.MaSanPham == id)
                .Include(dg => dg.NguoiDung)
                .ToList();

            // Tính điểm trung bình của đánh giá
            double diemTrungBinh = 0;
            if (danhGiaList.Count > 0)
            {
                diemTrungBinh = danhGiaList.Average(dg => dg.DiemDanhGia); // giả sử SoSao là thuộc tính điểm đánh giá
            }

            ViewBag.DiemTrungBinh = diemTrungBinh; // Lưu điểm trung bình vào ViewBag
            ViewBag.DanhGiaList = danhGiaList; // Nếu cần sử dụng danh sách này ở đâu đó
            ViewBag.ListKichCo = db.KichCo.Where(kc => kc.MaSanPham == id).ToList();

            return View(sanPham);
        }

        [HttpPost]
        public ActionResult ThemDanhGia(int maSanPham, int diemDanhGia, string binhLuan)
        {
            if (Session["UserID"] == null)
            {
                // Redirect to login or show an error if the user is not logged in
                TempData["ErrorMessage"] = "Bạn cần đăng nhập để đánh giá sản phẩm.";
                return RedirectToAction("ChiTietSP", new { id = maSanPham });
            }

            var maNguoiDung = (int)Session["UserID"]; // Retrieve user ID from the session
            DanhGiaSanPham danhGia = new DanhGiaSanPham
            {
                MaSanPham = maSanPham,
                MaNguoiDung = maNguoiDung,
                DiemDanhGia = diemDanhGia,
                NgayDanhGia = DateTime.Now
            };

            db.DanhGiaSanPham.Add(danhGia);
            db.SaveChanges();

            return RedirectToAction("ChiTietSP", new { id = maSanPham });
        }

        public ActionResult RecommendedItems(int categoryId)
        {
            // Lấy danh sách sản phẩm theo danh mục
            var recommendedProducts = db.SanPham
          .Where(sp => sp.MaDanhMuc == categoryId)
          .OrderBy(x => Guid.NewGuid())
          .Take(3)
          .ToList();

            return PartialView("RecommendedItems", recommendedProducts); // Trả về PartialView
        }
        public ActionResult RecommendedItemsByDate()
        {

            var recommendedProducts = db.SanPham
                .OrderByDescending(sp => sp.NgayTao)
                .Take(6)
                .ToList();

            return PartialView("RecommendedItems", recommendedProducts);
        }

    }
}
