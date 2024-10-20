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
    public class VatLieuxController : Controller
    {
        private DAPM_1Entities db = new DAPM_1Entities();

        // GET: Admin/VatLieux
        public ActionResult Index()
        {
            return View(db.VatLieu.ToList());
        }

        // GET: Admin/VatLieux/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VatLieu vatLieu = db.VatLieu.Find(id);
            if (vatLieu == null)
            {
                return HttpNotFound();
            }
            return View(vatLieu);
        }

        // GET: Admin/VatLieux/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/VatLieux/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaVatLieu,TenVatlieu,MoTa,NgayTao")] VatLieu vatLieu)
        {
            if (ModelState.IsValid)
            {
                db.VatLieu.Add(vatLieu);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(vatLieu);
        }

        // GET: Admin/VatLieux/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VatLieu vatLieu = db.VatLieu.Find(id);
            if (vatLieu == null)
            {
                return HttpNotFound();
            }
            return View(vatLieu);
        }

        // POST: Admin/VatLieux/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaVatLieu,TenVatlieu,MoTa,NgayTao")] VatLieu vatLieu)
        {
            if (ModelState.IsValid)
            {
                db.Entry(vatLieu).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(vatLieu);
        }

        // GET: Admin/VatLieux/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VatLieu vatLieu = db.VatLieu.Find(id);
            if (vatLieu == null)
            {
                return HttpNotFound();
            }
            return View(vatLieu);
        }

        // POST: Admin/VatLieux/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            VatLieu vatLieu = db.VatLieu.Find(id);
            db.VatLieu.Remove(vatLieu);
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
