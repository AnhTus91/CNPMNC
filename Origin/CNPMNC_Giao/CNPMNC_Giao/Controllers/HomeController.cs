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
        private CNPMNCEntities db = new CNPMNCEntities();
        public ActionResult Index()
        {

            return View();
        }

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

            if (Session["userLogin"] == null)
            {
                return RedirectToAction("Login", "User");
            }
            else
            {
                //string userID = (string)Session["userLogin"];
                //var donhang = from dh in db.DonHangs
                //              where dh.NguoiDung.TenNguoiDung == userID
                //              select dh;

                //donhang = donhang.Include("ChiTietDonHangs").OrderByDescending(e => e.trangThai);
                int userID = (int)Session["userLogin"]; // Chuyển đổi sang kiểu int theo dữ liệu của bạn
                var donhang = db.GioHangs
                                .Where(dh => dh.MaNguoiDung == userID)
                                .Include("ChiTietGioHangs")
                                .OrderByDescending(e => e.TrangThai)
                                .ToList();

                /*.Include("SanPham").Include("HinhAnh");*/



                return View(donhang.ToList());
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
                var dh = from donhang in db.DonHangs
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