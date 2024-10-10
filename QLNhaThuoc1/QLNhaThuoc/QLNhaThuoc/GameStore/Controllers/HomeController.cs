using GameStore.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GameStore.Controllers
{
    public class HomeController : Controller
    {
		QLThuocSo1VNEntities1 db = new QLThuocSo1VNEntities1();
        public ActionResult Index()
        {

            return View();
        }
     
        public JsonResult GetData()
        {
            int cartCount = 0;
            if (Session["ShoppingCart"] != null) // Nếu giỏ hàng chưa được khởi tạo
            {
                List<Cart> ShoppingCart = Session["ShoppingCart"] as List<Cart>;
                cartCount = ShoppingCart.Count();

            }
           
            return Json(cartCount,JsonRequestBehavior.AllowGet);
        }

        public ActionResult DonHang()
        {
         
            if (Session["userLogin"] == null)
            {
                return RedirectToAction("Login", "User");
            }
            else
            {
                string userID = (string)Session["userLogin"];
                var donhang = from dh in db.DonHangs
                           where dh.username == userID
                           select dh;

                donhang = donhang.Include("ChiTietDonHangs").OrderByDescending(e=>e.trangThai);

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
                string userID = (string)Session["userLogin"];
                var dh = from donhang in db.DonHangs
                         where donhang.maDH == maDH && donhang.username ==userID
                         select donhang;
                dh.FirstOrDefault().updatedAt = DateTime.Now;
                dh.FirstOrDefault().trangThai = "Đã hủy";
                db.SaveChanges();
                return RedirectToAction("DonHang","Home");
            }
                
            
        }
    }
}