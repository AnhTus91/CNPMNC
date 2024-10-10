using GameStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GameStore.Areas.Admin.Controllers
{
    public class DonHangController : Controller
    {
		// GET: Admin/DonHang
		QLThuocSo1VNEntities1 db = new QLThuocSo1VNEntities1();
        public ActionResult Index()
        {
            var donhang=db.DonHangs;
            
            return View(donhang.ToList());
        }
        public ActionResult ChiTiet(string id)
        {
            var donhang = db.DonHangs.Where(s=>s.maDH==id);

            return View(donhang.ToList());
        }   
        public ActionResult CapNhat(string id)
        {
            var donhang = db.DonHangs.Where(s=>s.maDH==id);

            return View(donhang.ToList());
        }

        public RedirectToRouteResult ChapNhan(string id)
        {
            var donhang = db.DonHangs.Find(id);
            if (donhang != null)
            {
                donhang.trangThai = "Đã duyệt";
                db.SaveChanges();
                return RedirectToAction("");
            }
            Response.StatusCode = 404;  //you may want to set this to 200
            return RedirectToAction("NotFound");

 
        }
        public RedirectToRouteResult Huy(string id)
        {
            var donhang = db.DonHangs.Find(id);
            if (donhang != null)
            {
                donhang.trangThai = "Đã hủy";
                db.SaveChanges();
                return RedirectToAction("");
            }
            Response.StatusCode = 404;  //you may want to set this to 200
            return RedirectToAction("NotFound");
        }
        public RedirectToRouteResult GiaoHang(string id)
        {
            var donhang = db.DonHangs.Find(id);
            if (donhang != null)
            {
                donhang.trangThai = "Đang giao";
                db.SaveChanges();
                return RedirectToAction("");
            }
            Response.StatusCode = 404;  //you may want to set this to 200
            return RedirectToAction("NotFound");
        }
        public RedirectToRouteResult ThanhCong(string id)
        {
            var donhang = db.DonHangs.Find(id);
            if (donhang != null)
            {
                donhang.trangThai = "Thành công";
                db.SaveChanges();
                return RedirectToAction("");
            }
            Response.StatusCode = 404;  //you may want to set this to 200
            return RedirectToAction("NotFound");
        }
    }
}