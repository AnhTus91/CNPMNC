using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CNPMNC_Giao.Models;

namespace CNPMNC_Giao.Areas.Admin.Controllers
{
    public class BinhLuansController : Controller
    {
        private CNPMNC1Entities db = new CNPMNC1Entities();

        // GET: Admin/BinhLuans
        public ActionResult Index()
        {
            var binhLuans = db.BinhLuan.Include(b => b.NguoiDung).Include(b => b.SanPham);
            return View(binhLuans.ToList());
        }

        // GET: Admin/BinhLuans/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BinhLuan binhLuan = db.BinhLuan.Find(id);
            if (binhLuan == null)
            {
                return HttpNotFound();
            }
            return View(binhLuan);
        }

        // GET: Admin/BinhLuans/Create
        public ActionResult Create()
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["ErrorMessage"] = "Bạn cần đăng nhập để bình luận.";
                return RedirectToAction("Index");
            }

            // Lấy thông tin người dùng dựa trên tên đăng nhập
            var username = User.Identity.Name;
            var nguoiDung = db.NguoiDung.FirstOrDefault(nd => nd.TenNguoiDung == username);

            if (nguoiDung == null || !HasPurchased(nguoiDung.MaNguoiDung))
            {
                TempData["ErrorMessage"] = "Bạn cần mua hàng để có thể bình luận.";
                return RedirectToAction("Index");
            }

            ViewBag.MaNguoiDung = new SelectList(db.NguoiDung, "MaNguoiDung", "TenNguoiDung");
            ViewBag.MaSanPham = new SelectList(db.SanPham, "MaSanPham", "TenSanPham");
            return View();
        }

        // POST: Admin/BinhLuans/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaBinhLuan,MaSanPham,MaNguoiDung,NoiDung,NgayBinhLuan")] BinhLuan binhLuan)
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["ErrorMessage"] = "Bạn cần đăng nhập để bình luận.";
                return RedirectToAction("Index");
            }

            var username = User.Identity.Name;
            var nguoiDung = db.NguoiDung.FirstOrDefault(nd => nd.TenNguoiDung == username);

            if (nguoiDung == null || !HasPurchased(nguoiDung.MaNguoiDung))
            {
                TempData["ErrorMessage"] = "Bạn cần mua hàng để có thể bình luận.";
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                binhLuan.MaNguoiDung = nguoiDung.MaNguoiDung;
                db.BinhLuan.Add(binhLuan);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MaNguoiDung = new SelectList(db.NguoiDung, "MaNguoiDung", "TenNguoiDung", binhLuan.MaNguoiDung);
            ViewBag.MaSanPham = new SelectList(db.SanPham, "MaSanPham", "TenSanPham", binhLuan.MaSanPham);
            return View(binhLuan);
        }

        private bool HasPurchased(int maNguoiDung)
        {
            // Kiểm tra xem người dùng có đơn hàng nào đã thanh toán hay chưa
            return db.DonHang.Any(dh => dh.MaNguoiGui == maNguoiDung && dh.TrangThai == "Đã thanh toán");
        }

        // GET: Admin/BinhLuans/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BinhLuan binhLuan = db.BinhLuan.Find(id);
            if (binhLuan == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaNguoiDung = new SelectList(db.NguoiDung, "MaNguoiDung", "TenNguoiDung", binhLuan.MaNguoiDung);
            ViewBag.MaSanPham = new SelectList(db.SanPham, "MaSanPham", "TenSanPham", binhLuan.MaSanPham);
            return View(binhLuan);
        }

        // POST: Admin/BinhLuans/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaBinhLuan,MaSanPham,MaNguoiDung,NoiDung,NgayBinhLuan")] BinhLuan binhLuan)
        {
            if (ModelState.IsValid)
            {
                db.Entry(binhLuan).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MaNguoiDung = new SelectList(db.NguoiDung, "MaNguoiDung", "TenNguoiDung", binhLuan.MaNguoiDung);
            ViewBag.MaSanPham = new SelectList(db.SanPham, "MaSanPham", "TenSanPham", binhLuan.MaSanPham);
            return View(binhLuan);
        }

        // GET: Admin/BinhLuans/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BinhLuan binhLuan = db.BinhLuan.Find(id);
            if (binhLuan == null)
            {
                return HttpNotFound();
            }
            return View(binhLuan);
        }

        // POST: Admin/BinhLuans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BinhLuan binhLuan = db.BinhLuan.Find(id);
            db.BinhLuan.Remove(binhLuan);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
