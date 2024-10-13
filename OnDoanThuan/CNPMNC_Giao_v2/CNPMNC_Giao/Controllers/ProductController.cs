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
        private readonly CNPMNCEntities db = new CNPMNCEntities();
        // GET: Product
        public ActionResult Index()
        {
            var danhSachSanPham = db.SanPham.ToList();

            // Truyền danh sách vào View
            return View(danhSachSanPham);
        }
    }
}