﻿using CNPMNC_Giao.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CNPMNC_Giao.Areas.Admin.Controllers
{
    public class DonHangController : Controller
    {
        private CNPMNCEntities db = new CNPMNCEntities();
        // GET: Admin/DonHang
        public ActionResult Index()
        {
            var donhang = db.DonHangs;

            return View(donhang.ToList());
        }
        [HttpPost]
        public ActionResult CapNhatTrangThai(string id, string trangThai)
        {
            var donhang = db.DonHangs.Find(id);
            if (donhang == null)
            {
                return HttpNotFound();
            }

            donhang.TrangThai = trangThai;
            db.SaveChanges();

            return Json(new { success = true });
        }
        public ActionResult ChiTiet(string id)
        {
            var donhang = db.DonHangs.Where(s => s.MaDonHang == id);

            return View(donhang.ToList());
        }
        public ActionResult CapNhat(string id)
        {
            var donhang = db.DonHangs.Where(s => s.MaDonHang == id);

            return View(donhang.ToList());
        }

        public RedirectToRouteResult ChapNhan(string id)
        {
            var donhang = db.DonHangs.Find(id);
            if (donhang != null)
            {
                donhang.TrangThai = "Đã duyệt";
                db.SaveChanges();
                return RedirectToAction("");
            }
            Response.StatusCode = 404;  //you may want to set this to 200
            return RedirectToAction("NotFound");


        }
        //public RedirectToRouteResult Huy(string id)
        //{
        //    var donhang = db.DonHang.Find(id);
        //    if (donhang != null)
        //    {
        //        donhang.TrangThai = "Đã hủy";
        //        db.SaveChanges();
        //        return RedirectToAction("");
        //    }
        //    Response.StatusCode = 404;  //you may want to set this to 200
        //    return RedirectToAction("NotFound");
        //}
        [HttpPost]
        public ActionResult HuyDonHang(string id)
        {
            var donhang = db.DonHangs.Find(id);
            if (donhang == null)
            {
                return HttpNotFound();
            }

            donhang.TrangThai = "Đã hủy";
            db.SaveChanges();

            return Json(new { success = true });
        }
        public RedirectToRouteResult GiaoHang(string id)
        {
            var donhang = db.DonHangs.Find(id);
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
            var donhang = db.DonHangs.Find(id);
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
