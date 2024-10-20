using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using CNPMNC_Giao.Models;

namespace CNPMNC_Giao.Areas.Admin.Controllers
{
    public class SanPhamsController : Controller
    {
        private DAPM_1Entities db = new DAPM_1Entities();

        // GET: Admin/SanPhams
        public ActionResult Index(string searchString)
        {
            // Lấy danh sách sản phẩm, bao gồm các bảng liên kết (DanhMuc, NhaCungCap, VatLieu)
            var sanPhams = db.SanPham.Include(s => s.DanhMuc)
                                     .Include(s => s.NhaCungCap)
                                     .Include(s => s.VatLieu);

            // Nếu có chuỗi tìm kiếm, lọc sản phẩm theo tên, danh mục, nhà cung cấp
            if (!string.IsNullOrEmpty(searchString))
            {
                sanPhams = sanPhams.Where(s => s.TenSanPham.Contains(searchString) ||
                                               s.DanhMuc.TenDanhMuc.Contains(searchString) ||
                                               s.NhaCungCap.TenNhaCungCap.Contains(searchString) ||
                                               s.VatLieu.TenVatlieu.Contains(searchString));
            }

            // Trả về danh sách sản phẩm sau khi lọc (nếu có)
            return View(sanPhams.ToList());
        }

        // GET: Admin/SanPhams/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SanPham sanPham = db.SanPham.Find(id);
            if (sanPham == null)
            {
                return HttpNotFound();
            }
            var kichCos = sanPham.KichCo.ToList();

            // Gửi thông tin đến view
            ViewBag.KichCos = kichCos;

            //ViewBag.KichCos = new SelectList(sanPham.KichCo, "MaKichCo", "SoKichCo"); // Giả định "SoKichCo" là tên hiển thị


            return View(sanPham);

        }

        // GET: Admin/SanPhams/Create
        public ActionResult Create()
        {
            ViewBag.MaDanhMuc = new SelectList(db.DanhMuc, "MaDanhMuc", "TenDanhMuc");
            ViewBag.MaNhaCungCap = new SelectList(db.NhaCungCap, "MaNhaCungCap", "TenNhaCungCap");
            ViewBag.MaVatLieu = new SelectList(db.VatLieu, "MaVatLieu", "TenVatlieu");


            var sanPham = new SanPham
            {
                NgayTao = DateTime.Now.Date //Ngày Tạo là ngày hôm nay
            };

            return View(sanPham);

        }

        // POST: Admin/SanPhams/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaSanPham,TenSanPham,GiaTienMoi,GiaTienCu,MoTa,AnhSP,MaVatLieu,MaDanhMuc,NgayTao,MaNhaCungCap")] SanPham sanPham, HttpPostedFileBase AnhSanPham)
        {
            if (ModelState.IsValid)
            {
                var idImage = Guid.NewGuid().ToString();
                string _FileName = "";
                int index = AnhSanPham.FileName.IndexOf('.');
                _FileName = idImage.ToString() + "." + AnhSanPham.FileName.Substring(index + 1);
                string path1 = Path.Combine(Server.MapPath("~/AnhSanPham"), _FileName);
                AnhSanPham.SaveAs(path1);
                sanPham.AnhSP = _FileName;


                // Nếu Ngày Tạo chưa được gán, thiết lập nó là ngày hôm nay
                if (sanPham.NgayTao == default(DateTime))
                {
                    sanPham.NgayTao = DateTime.Now.Date; 
                }
               

                db.SanPham.Add(sanPham);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MaDanhMuc = new SelectList(db.DanhMuc, "MaDanhMuc", "TenDanhMuc", sanPham.MaDanhMuc);
            ViewBag.MaNhaCungCap = new SelectList(db.NhaCungCap, "MaNhaCungCap", "TenNhaCungCap", sanPham.MaNhaCungCap);
            ViewBag.MaVatLieu = new SelectList(db.VatLieu, "MaVatLieu", "TenVatlieu", sanPham.MaVatLieu);

            return View(sanPham);
        }

        // GET: Admin/SanPhams/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Tìm sản phẩm theo ID
            SanPham sanPham = db.SanPham.Find(id);
            if (sanPham == null)
            {
                return HttpNotFound();
            }

            // Tạo các danh sách cho dropdown
            ViewBag.MaDanhMuc = new SelectList(db.DanhMuc, "MaDanhMuc", "TenDanhMuc", sanPham.MaDanhMuc);
            ViewBag.MaNhaCungCap = new SelectList(db.NhaCungCap, "MaNhaCungCap", "TenNhaCungCap", sanPham.MaNhaCungCap);
            ViewBag.MaVatLieu = new SelectList(db.VatLieu, "MaVatLieu", "TenVatlieu", sanPham.MaVatLieu);



            return View(sanPham);
        }

        // POST: Admin/SanPhams/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaSanPham,TenSanPham,GiaTienMoi,GiaTienCu,MoTa,AnhSP,MaVatLieu,MaDanhMuc,NgayTao,MaNhaCungCap")] SanPham sanPham)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sanPham).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            // Tạo lại danh sách cho dropdown nếu ModelState không hợp lệ
            ViewBag.MaDanhMuc = new SelectList(db.DanhMuc, "MaDanhMuc", "TenDanhMuc", sanPham.MaDanhMuc);
            ViewBag.MaNhaCungCap = new SelectList(db.NhaCungCap, "MaNhaCungCap", "TenNhaCungCap", sanPham.MaNhaCungCap);
            ViewBag.MaVatLieu = new SelectList(db.VatLieu, "MaVatLieu", "TenVatlieu", sanPham.MaVatLieu);



            return View(sanPham);
        }


        // GET: Admin/SanPhams/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SanPham sanPham = db.SanPham.Find(id);
            if (sanPham == null)
            {
                return HttpNotFound();
            }
            return View(sanPham);
        }

        // POST: Admin/SanPhams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SanPham sanPham = db.SanPham.Find(id);
            db.SanPham.Remove(sanPham);
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
        //=========================
        // GET: Admin/SanPhams/TaoKichCo
        // GET: Admin/SanPhams/TaoKichCo
        public ActionResult TaoKichCo()
        {
            ViewBag.SanPhams = db.SanPham.ToList(); // Lấy danh sách sản phẩm
            return View(new KichCo());
        }

        // POST: Admin/SanPhams/TaoKichCo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TaoKichCo(KichCo kichCoViewModel)
        {
            if (ModelState.IsValid)
            {
                var kichCo = new KichCo
                {
                    MaSanPham = kichCoViewModel.MaSanPham,
                    SoKichCo = kichCoViewModel.SoKichCo,
                    SoLuong = kichCoViewModel.SoLuong
                };

                db.KichCo.Add(kichCo);
                db.SaveChanges();
                return RedirectToAction("Index"); // Redirect đến trang danh sách sản phẩm hoặc kích cỡ
            }

            // Nếu ModelState không hợp lệ, cần phải lấy lại danh sách sản phẩm
            ViewBag.SanPhams = db.SanPham.ToList(); // Lấy lại danh sách sản phẩm nếu có lỗi
            return View(kichCoViewModel);
        }

    }
}
