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
            //var sanPham = db.SanPhams.Find(id);
            var sanPham = db.SanPham.Include(s => s.KichCo).FirstOrDefault(s => s.MaSanPham == id);

            if (sanPham == null)
            {
                ViewBag.error = "Sản phẩm không tồn tại";
                return RedirectToAction("Index", "Home");
            }
            var ListKichCo = db.KichCo.Where(kc => kc.MaSanPham == id).ToList();
            ViewBag.ListKichCo = ListKichCo;
            return View(sanPham);
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