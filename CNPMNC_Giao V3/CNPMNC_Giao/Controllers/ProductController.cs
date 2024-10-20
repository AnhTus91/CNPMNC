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
        private readonly DAPM_1Entities db = new DAPM_1Entities();
        // GET: Product
        public ActionResult Index(int? categoryId)
        {
            ViewBag.DanhMucList = db.DanhMuc.ToList();

      
            var danhSachSanPham = db.SanPham.AsQueryable();

          
            if (categoryId.HasValue && categoryId.Value > 0)
            {
                danhSachSanPham = danhSachSanPham.Where(sp => sp.MaDanhMuc == categoryId.Value);

                var category = db.DanhMuc.Find(categoryId.Value);
                ViewBag.Title = category?.TenDanhMuc; 
            }
            else
            {
                ViewBag.Title = "Tất cả sản phẩm";
            }

            // Truyền danh sách vào View
            return View(danhSachSanPham.ToList());
        }
        public ActionResult Search(string keyword)
        {
        
            if (!string.IsNullOrEmpty(keyword))
            {
                // Tìm kiếm sản phẩm theo tên
                var sanPhams = db.SanPham
                                 .Where(sp => sp.TenSanPham.Contains(keyword))
                                 .OrderByDescending(sp => sp.NgayTao)
                                 .ToList();
                ViewBag.Keyword = keyword; 
                return View("Search", sanPhams); 
            }


            return RedirectToAction("Index");
        }
        // Action: Chi tiết sản phẩm
        public ActionResult Details(int? id)
        {
            var sanPham = db.SanPham
               .Include(sp => sp.NhaCungCap) // Include supplier details if needed
               .Include(sp => sp.VatLieu)    // Include material details if needed
               .FirstOrDefault(sp => sp.MaSanPham == id);

            if (sanPham == null)
            {
                return HttpNotFound();
            }

            // Lấy danh sách kích cỡ từ bảng KichCo dựa trên MaSanPham
            var availableSizes = db.KichCo
                .Where(kc => kc.MaSanPham == sanPham.MaSanPham)
                .Select(kc => kc.SoKichCo) // Lấy cột SoKichCo
                .ToList();

            ViewBag.AvailableSizes = availableSizes; // Pass sizes to the view

            return View(sanPham);
        }
    }
}