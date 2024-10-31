using CNPMNC_Giao.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CNPMNC_Giao.Controllers
{
    public class HomeController : Controller
    {
        private CNPMNC1Entities db = new CNPMNC1Entities();
        public ActionResult Index()
        {
            var sanPhams = db.SanPham.OrderByDescending(sp => sp.NgayTao).ToList();
            return View(sanPhams);
        }
        //public ActionResult Search(string keyword)
        //{
        //    // Kiểm tra nếu keyword có giá trị
        //    if (!string.IsNullOrEmpty(keyword))
        //    {
        //        // Tìm kiếm sản phẩm theo tên
        //        var sanPhams = db.SanPham
        //                         .Where(sp => sp.TenSanPham.Contains(keyword))
        //                         .OrderByDescending(sp => sp.NgayTao)
        //                         .ToList();
        //        return View("Index", sanPhams); // Trả về danh sách sản phẩm tìm được
        //    }

        //    // Nếu không có từ khóa, trả về toàn bộ sản phẩm
        //    return RedirectToAction("Index");
        //}
        public JsonResult GetData()
        {
            int cartCount = 0;
            if (Session["ShoppingCart"] != null) // Nếu giỏ hàng chưa được khởi tạo
            {
                List<ChiTietGioHang> ShoppingCart = Session["ShoppingCart"] as List<ChiTietGioHang>;
                cartCount = ShoppingCart.Count();

            }

            return Json(cartCount, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DonHang()
        {
            // Kiểm tra session đăng nhập
            if (Session["userLogin"] == null)
            {
                return RedirectToAction("Login", "User");
            }
            else
            {
                // Kiểm tra và chuyển đổi giá trị từ session thành kiểu int
                if (int.TryParse(Session["userLogin"].ToString(), out int userID))
                {
                    // Truy vấn đơn hàng của người dùng
                    var donhang = db.GioHang
                                    .Where(dh => dh.MaNguoiDung == userID)
                                    .Include(dh => dh.ChiTietGioHangs) // Sử dụng lambda thay vì chuỗi
                                    .OrderByDescending(e => e.TrangThai)
                                    .ToList();

                    return View(donhang);
                }
                else
                {
                    // Nếu không thể chuyển đổi về int, chuyển hướng về trang đăng nhập
                    return RedirectToAction("Login", "User");
                }
            }
        }



        public RedirectToRouteResult HuyDonHang(string maDH)
        {
            if (Session["userLogin"] == null)
            {
                return RedirectToAction("Login", "User");
            }
            else
            {
                int userID = (int)Session["userLogin"];
                var dh = from donhang in db.DonHang
                         where donhang.MaDonHang == maDH && donhang.MaNguoiGui == userID
                         select donhang;
                dh.FirstOrDefault().NgayDatHang = DateTime.Now;
                dh.FirstOrDefault().TrangThai = "Đã hủy";
                db.SaveChanges();
                return RedirectToAction("DonHang", "Home");
            }


        }
    }
}