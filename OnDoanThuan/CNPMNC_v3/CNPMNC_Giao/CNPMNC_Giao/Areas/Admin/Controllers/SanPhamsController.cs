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
    public class SanPhamsController : Controller
    {
        private CNPMNCEntities1 db = new CNPMNCEntities1();

        // GET: Admin/SanPhams
        public ActionResult Index(string searchString)
        {
            // Lấy danh sách sản phẩm, bao gồm các bảng liên kết (DanhMuc, NhaCungCap, VatLieu)
            var sanPhams = db.SanPhams.Include(s => s.DanhMuc)
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
            SanPham sanPham = db.SanPhams.Find(id);
            if (sanPham == null)
            {
                return HttpNotFound();
            }
            return View(sanPham);
        }

        // GET: Admin/SanPhams/Create
        public ActionResult Create()
        {
            ViewBag.MaDanhMuc = new SelectList(db.DanhMucs, "MaDanhMuc", "TenDanhMuc");
            ViewBag.MaNhaCungCap = new SelectList(db.NhaCungCaps, "MaNhaCungCap", "TenNhaCungCap");
            ViewBag.MaVatLieu = new SelectList(db.VatLieux, "MaVatLieu", "TenVatlieu");
            ViewBag.MaVoucher = new SelectList(db.VOUCHERs, "MaVoucher", "GiaTri");

            var sanPham = new SanPham
            {
                NgayTao = DateTime.Now //Ngày Tạo là ngày hôm nay
            };

            return View(sanPham);

        }

        // POST: Admin/SanPhams/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaSanPham,TenSanPham,GiaTienMoi,GiaTienCu,MoTa,AnhSP,MaVatLieu,MaDanhMuc,NgayTao,MaNhaCungCap,MaVoucher")] SanPham sanPham)
        {
            if (ModelState.IsValid)
            {
                // Nếu Ngày Tạo chưa được gán, thiết lập nó là ngày hôm nay
                if (sanPham.NgayTao == default(DateTime))
                {
                    sanPham.NgayTao = DateTime.Now.Date; // Đảm bảo chỉ lưu ngày
                }

                db.SanPhams.Add(sanPham);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MaDanhMuc = new SelectList(db.DanhMucs, "MaDanhMuc", "TenDanhMuc", sanPham.MaDanhMuc);
            ViewBag.MaNhaCungCap = new SelectList(db.NhaCungCaps, "MaNhaCungCap", "TenNhaCungCap", sanPham.MaNhaCungCap);
            ViewBag.MaVatLieu = new SelectList(db.VatLieux, "MaVatLieu", "TenVatlieu", sanPham.MaVatLieu);
            ViewBag.MaVoucher = new SelectList(db.VOUCHERs, "MaVoucher", "TenVoucher", sanPham.MaVoucher);
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
            SanPham sanPham = db.SanPhams.Find(id);
            if (sanPham == null)
            {
                return HttpNotFound();
            }

            // Tạo các danh sách cho dropdown
            ViewBag.MaDanhMuc = new SelectList(db.DanhMucs, "MaDanhMuc", "TenDanhMuc", sanPham.MaDanhMuc);
            ViewBag.MaNhaCungCap = new SelectList(db.NhaCungCaps, "MaNhaCungCap", "TenNhaCungCap", sanPham.MaNhaCungCap);
            ViewBag.MaVatLieu = new SelectList(db.VatLieux, "MaVatLieu", "TenVatlieu", sanPham.MaVatLieu);

            // Chỉnh sửa đây để không sử dụng TenVoucher
            ViewBag.MaVoucher = new SelectList(db.VOUCHERs, "MaVoucher", "GiaTri", sanPham.MaVoucher);

            return View(sanPham);
        }

        // POST: Admin/SanPhams/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaSanPham,TenSanPham,GiaTienMoi,GiaTienCu,MoTa,AnhSP,MaVatLieu,MaDanhMuc,NgayTao,MaNhaCungCap,MaVoucher")] SanPham sanPham)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sanPham).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            // Tạo lại danh sách cho dropdown nếu ModelState không hợp lệ
            ViewBag.MaDanhMuc = new SelectList(db.DanhMucs, "MaDanhMuc", "TenDanhMuc", sanPham.MaDanhMuc);
            ViewBag.MaNhaCungCap = new SelectList(db.NhaCungCaps, "MaNhaCungCap", "TenNhaCungCap", sanPham.MaNhaCungCap);
            ViewBag.MaVatLieu = new SelectList(db.VatLieux, "MaVatLieu", "TenVatlieu", sanPham.MaVatLieu);

            // Chỉnh sửa đây để không sử dụng TenVoucher
            ViewBag.MaVoucher = new SelectList(db.VOUCHERs, "MaVoucher", "GiaTri", sanPham.MaVoucher);

            return View(sanPham);
        }


        // GET: Admin/SanPhams/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SanPham sanPham = db.SanPhams.Find(id);
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
            SanPham sanPham = db.SanPhams.Find(id);
            db.SanPhams.Remove(sanPham);
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
        public ActionResult TaoMoiDanhMuc()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TaoMoiDanhMuc([Bind(Include = "MaDanhMuc,TenDanhMuc,MoTa")] DanhMuc danhMuc)
        {
            if (ModelState.IsValid)
            {
                db.DanhMucs.Add(danhMuc);
                db.SaveChanges();
                return RedirectToAction("Index","DanhMucs");
            }

            return View(danhMuc);
        }
        //=========================
        public ActionResult TaoMoiNhaCungCap()
        {
            return View();
        }

        // POST: Admin/NhaCungCaps/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TaoMoiNhaCungCap([Bind(Include = "MaNhaCungCap,TenNhaCungCap,SDT,DiaChi,Email")] NhaCungCap nhaCungCap)
        {
            if (ModelState.IsValid)
            {
                db.NhaCungCaps.Add(nhaCungCap);
                db.SaveChanges();
                return RedirectToAction("Index","NhaCungCaps");
            }

            return View(nhaCungCap);
        }
        //============================
        public ActionResult TaoMoiVatLieu()
        {
            return View();
        }

        // POST: Admin/VatLieux/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TaoMoiVatLieu([Bind(Include = "MaVatLieu,TenVatlieu,MoTa,NgayTao")] VatLieu vatLieu)
        {
            if (ModelState.IsValid)
            {
                db.VatLieux.Add(vatLieu);
                db.SaveChanges();
                return RedirectToAction("Index","VatLieux");
            }

            return View(vatLieu);
        }
        public ActionResult TaoMoiVoucher()
        {
            return View();
        }

        // POST: Admin/VatLieux/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TaoMoiVoucher([Bind(Include = "MaVoucher,GiaTri,ThoiGianBatDau,ThoiGianKetThuc,TrangThai,NgayTao,DieuKienApDung,SoLuong")] VOUCHER vOUCHER)
        {
            if (ModelState.IsValid)
            {
                db.VOUCHERs.Add(vOUCHER);
                db.SaveChanges();
                return RedirectToAction("Index", "VOUCHER");
            }

            return View(vOUCHER);
        }
    }
}
