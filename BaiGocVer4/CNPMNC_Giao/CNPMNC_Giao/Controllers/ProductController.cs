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
        private readonly CNPMNCEntities db = new CNPMNCEntities();
        // GET: Product
        public ActionResult Index(int? categoryId)
        {
            // Lấy danh sách danh mục để hiển thị trên dropdown
            ViewBag.DanhMucList = db.DanhMucs.ToList();

            // Lấy danh sách sản phẩm
            var danhSachSanPham = db.SanPhams.AsQueryable();

            // Nếu categoryId có giá trị, lọc sản phẩm theo danh mục
            if (categoryId.HasValue && categoryId.Value > 0)
            {
                danhSachSanPham = danhSachSanPham.Where(sp => sp.MaDanhMuc == categoryId.Value);

                // Lấy tên danh mục đã chọn để hiển thị tiêu đề
                var category = db.DanhMucs.Find(categoryId.Value);
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
                var sanPhams = db.SanPhams
                                 .Where(sp => sp.TenSanPham.Contains(keyword))
                                 .OrderByDescending(sp => sp.NgayTao)
                                 .ToList();
                ViewBag.Keyword = keyword; // Gán từ khóa vào ViewBag để sử dụng trong View
                return View("Search", sanPhams); // Trả về danh sách sản phẩm tìm được
            }

            // Nếu không có từ khóa, trả về toàn bộ sản phẩm
            return RedirectToAction("Index");
        }
        /// <summary>
        /// //////////
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ChiTietSP(int? id)
        {
            //var sanPham = db.SanPhams.Find(id);
            var sanPham = db.SanPhams
                .Include(s => s.KichCoes)
                 .Include(s => s.DanhMuc) // Bao gồm thông tin danh mục
                .Include(s => s.VatLieu) // Bao gồm thông tin vật liệu
                .Include(s => s.NhaCungCap) // Bao gồm thông tin nhà cung cấp
                .Include(s => s.DanhGiaSanPhams) // Bao gồm danh sách đánh giá của sản phẩm

                .FirstOrDefault(s => s.MaSanPham == id);

           

            if (sanPham == null)
            {
                ViewBag.error = "Sản phẩm không tồn tại";
                return RedirectToAction("Index","Home");
            }    
            
            var goiYSP = db.SanPhams
      .Where(sp => sp.MaDanhMuc == sanPham.MaDanhMuc && sp.MaSanPham != sanPham.MaSanPham)
      .OrderByDescending(sp => sp.NgayTao)
      .Take(5)
      .ToList();

            var goiYTheoGiaCaoNhat = db.SanPhams
                .Where(sp => sp.MaDanhMuc == sanPham.MaDanhMuc && sp.MaSanPham != sanPham.MaSanPham)
                .OrderByDescending(sp => sp.GiaTienMoi)
                .Take(5)
                .ToList();

            // Calculate average rating
            var diemTrungBinh = sanPham.DanhGiaSanPhams.Any()
                ? sanPham.DanhGiaSanPhams.Average(dg => dg.DiemDanhGia)
                : 0;
            var viewModel = new ChiTietSanPhamViewModels
            {
                SanPham = sanPham,
                GoiYSanPhams = goiYSP,
                GoiYSanPhamsTheoGiaCaoNhat = goiYTheoGiaCaoNhat,
                DiemTrungBinh = diemTrungBinh
            };
            return View("ChiTietSP", viewModel);
        }
        /// <summary>
        /// /
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public ActionResult RecommendedItems(int categoryId)
        {
            // Lấy danh sách sản phẩm theo danh mục
            var recommendedProducts = db.SanPhams
          .Where(sp => sp.MaDanhMuc == categoryId)
          .OrderBy(x => Guid.NewGuid())
          .Take(3)
          .ToList();

            return PartialView("RecommendedItems", recommendedProducts); // Trả về PartialView
        }
        public ActionResult RecommendedItemsByDate()
        {

            var recommendedProducts = db.SanPhams
                .OrderByDescending(sp => sp.NgayTao)
                .Take(6)
                .ToList();

            return PartialView("RecommendedItems", recommendedProducts);
        }

    }
}
