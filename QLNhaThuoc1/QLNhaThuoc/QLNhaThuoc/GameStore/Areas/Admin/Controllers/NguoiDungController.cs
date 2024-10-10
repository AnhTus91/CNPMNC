using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GameStore.Models;

namespace GameStore.Areas.Admin.Controllers
{
    public class NguoiDungController : Controller
    {
        QLThuocSo1VNEntities1 db = new QLThuocSo1VNEntities1();

        // GET: Admin/NguoiDung
        public ActionResult Index()
        {
            var nguoiDung = db.NguoiDungs.Include(n => n.PhanQuyen);
      
            return View(nguoiDung.ToList());
        }

        // GET: Admin/NguoiDung/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NguoiDung nguoiDung = db.NguoiDungs.Find(id);
            if (nguoiDung == null)
            {
                return HttpNotFound();
            }
            return View(nguoiDung);
        }

        // GET: Admin/NguoiDung/Create
        public ActionResult Create()
        {
            ViewBag.roleID = new SelectList(db.PhanQuyens, "roleID", "roleName");
     
            return View();
        }

        // POST: Admin/NguoiDung/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "username,hoTen,email,sdt,matkhau,roleID")] NguoiDung nguoiDung)
        {
            if (ModelState.IsValid)
            {
                var user = db.NguoiDungs.SingleOrDefault(s => s.username==nguoiDung.username);
                if (user != null)
                {
                    ViewBag.err = "Tài khoản đã tồn tại";
                    return View(nguoiDung);
                }
                db.NguoiDungs.Add(nguoiDung);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.roleID = new SelectList(db.PhanQuyens, "roleID", "roleName", nguoiDung.roleID);
            return View(nguoiDung);
        }

        // GET: Admin/NguoiDung/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NguoiDung nguoiDung = db.NguoiDungs.Find(id);
            if (nguoiDung == null)
            {
                return HttpNotFound();
            }
            ViewBag.roleID = new SelectList(db.PhanQuyens, "roleID", "roleName", nguoiDung.roleID);
            return View(nguoiDung);
        }

        // POST: Admin/NguoiDung/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "username,hoTen,email,sdt,matkhau,roleID")] NguoiDung nguoiDung)
        {
            if (ModelState.IsValid)
            {
                db.Entry(nguoiDung).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.roleID = new SelectList(db.PhanQuyens, "roleID", "roleName", nguoiDung.roleID);
            return View(nguoiDung);
        }

        // GET: Admin/NguoiDung/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NguoiDung nguoiDung = db.NguoiDungs.Find(id);
            if (nguoiDung == null)
            {
                return HttpNotFound();
            }
            return View(nguoiDung);
        }

        // POST: Admin/NguoiDung/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            NguoiDung nguoiDung = db.NguoiDungs.Find(id);
            db.NguoiDungs.Remove(nguoiDung);
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
