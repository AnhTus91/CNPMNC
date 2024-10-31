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
        private readonly CNPMNC1Entities db = new CNPMNC1Entities();
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
                .Include(s => s.KichCoes)
                .Include(s => s.BinhLuans) // Đảm bảo các bình luận được tải về
                .FirstOrDefault(s => s.MaSanPham == id);

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




        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PostComment(int productId, string content)
        {
            // Lấy ID của người dùng hiện tại từ Session
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "User");
            }
            var userId = (int)Session["UserID"]; // Giả sử bạn đã lưu ID người dùng vào Session

            // Kiểm tra xem người dùng có đã mua sản phẩm này hay chưa
            if (!KiemTraDaMuaHang(userId, productId))
            {
                ModelState.AddModelError("", "Bạn phải mua hàng trước khi bình luận.");
                return RedirectToAction("Details", "Product", new { id = productId });
            }

            // Tạo đối tượng bình luận mới
            var binhLuan = new BinhLuan
            {
                MaSanPham = productId,
                MaNguoiDung = userId, // Giả sử bạn có thuộc tính này để lưu ID người dùng
                NoiDung = content,
                NgayBinhLuan = DateTime.Now // Ngày giờ hiện tại
            };

            // Thêm bình luận vào database
            db.BinhLuan.Add(binhLuan);
            db.SaveChanges();

            // Chuyển hướng về trang chi tiết sản phẩm
            return RedirectToAction("Details", "Product", new { id = productId });
        }

        // Phương thức kiểm tra xem người dùng đã mua sản phẩm hay chưa
        private bool KiemTraDaMuaHang(int userId, int productId)
        {
            return db.DonHang
                .Any(dh => dh.MaNguoiGui == userId &&
                            dh.ChiTietDonHangs.Any(ct => ct.MaSanPham == productId));
        }
        }
    }