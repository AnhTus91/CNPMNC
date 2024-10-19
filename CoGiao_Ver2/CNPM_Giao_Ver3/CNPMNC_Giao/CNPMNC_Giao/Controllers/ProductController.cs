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
        public ActionResult ChiTietSP(int? id)
        {
            //var sanPham = db.SanPhams.Find(id);
            var sanPham = db.SanPhams.Include(s => s.KichCoes).FirstOrDefault(s => s.MaSanPham == id);

            if (sanPham == null)
            {
                ViewBag.error = "Sản phẩm không tồn tại";
                return RedirectToAction("Index","Home");
            }    
            var ListKichCo = db.KichCoes.Where(kc => kc.MaSanPham == id).ToList();
            ViewBag.ListKichCo = ListKichCo;
            return View(sanPham);
        }

    }
}