using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CNPMNC_Giao.Models;

namespace CNPMNC_Giao.Controllers
{
    public class ChiTietGioHangsController : Controller
    {
        private DAPM_1Entities db = new DAPM_1Entities();

        // GET: ChiTietGioHangs
        public ActionResult Index()
        {
            var chiTietGioHangs = db.ChiTietGioHangs.Include(c => c.GioHang).Include(c => c.SanPham);
            return View(chiTietGioHangs.ToList());
        }

        // GET: ChiTietGioHangs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChiTietGioHang chiTietGioHang = db.ChiTietGioHangs.Find(id);
            if (chiTietGioHang == null)
            {
                return HttpNotFound();
            }
            return View(chiTietGioHang);
        }

     
    }
}
