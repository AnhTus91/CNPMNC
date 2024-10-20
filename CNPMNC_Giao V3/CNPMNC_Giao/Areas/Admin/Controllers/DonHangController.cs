using CNPMNC_Giao.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CNPMNC_Giao.Areas.Admin.Controllers
{
    public class DonHangController : Controller
    {
        private DAPM_1Entities db = new DAPM_1Entities();
        // GET: Admin/DonHang
        public ActionResult Index()
        {
            var donhang = db.DonHang;

            return View(donhang.ToList());
        }
        public ActionResult ChiTiet(string id)
        {
            var donhang = db.DonHang.Where(s => s.MaDonHang == id);

            return View(donhang.ToList());
        }
        public ActionResult CapNhat(string id)
        {
            var donhang = db.DonHang.Where(s => s.MaDonHang == id);

            return View(donhang.ToList());
        }

        public RedirectToRouteResult ChapNhan(string id)
        {
            var donhang = db.DonHang.Find(id);
            if (donhang != null)
            {
                donhang.TrangThai = "Đã duyệt";
                db.SaveChanges();
                return RedirectToAction("");
            }
            Response.StatusCode = 404;  //you may want to set this to 200
            return RedirectToAction("NotFound");


        }
        public RedirectToRouteResult Huy(string id)
        {
            var donhang = db.DonHang.Find(id);
            if (donhang != null)
            {
                donhang.TrangThai = "Đã hủy";
                db.SaveChanges();
                return RedirectToAction("");
            }
            Response.StatusCode = 404;  //you may want to set this to 200
            return RedirectToAction("NotFound");
        }
        public RedirectToRouteResult GiaoHang(string id)
        {
            var donhang = db.DonHang.Find(id);
            if (donhang != null)
            {
                donhang.TrangThai = "Đang giao";
                db.SaveChanges();
                return RedirectToAction("");
            }
            Response.StatusCode = 404;  //you may want to set this to 200
            return RedirectToAction("NotFound");
        }
        public RedirectToRouteResult ThanhCong(string id)
        {
            var donhang = db.DonHang.Find(id);
            if (donhang != null)
            {
                donhang.TrangThai = "Thành công";
                db.SaveChanges();
                return RedirectToAction("");
            }
            Response.StatusCode = 404;  //you may want to set this to 200
            return RedirectToAction("NotFound");
        }
    }
}