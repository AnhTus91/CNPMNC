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
        private CNPMNCEntities db = new CNPMNCEntities();

        // GET: Admin/SanPhams
        public ActionResult Index(string searchString)
        {
            // Lấy danh sách sản phẩm, bao gồm các bảng liên kết (DanhMuc, NhaCungCap, VatLieu)
            var sanPhams = db.SanPhams.Include(s => s.DanhMuc)
                                     .Include(s => s.NhaCungCap)
                                     .Include(s => s.VatLieu)
                                     .Include(s=>s.KichCoes)
                                     ;

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

            var sanPham = new SanPham
            {
                NgayTao = DateTime.Now //Ngày Tạo là ngày hôm nay
            };
            return View();
        }

        // POST: Admin/SanPhams/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SanPham sanPham, HttpPostedFileBase AnhSanPham)
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


                db.SanPhams.Add(sanPham);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MaDanhMuc = new SelectList(db.DanhMucs, "MaDanhMuc", "TenDanhMuc", sanPham.MaDanhMuc);
            ViewBag.MaNhaCungCap = new SelectList(db.NhaCungCaps, "MaNhaCungCap", "TenNhaCungCap", sanPham.MaNhaCungCap);
            ViewBag.MaVatLieu = new SelectList(db.VatLieux, "MaVatLieu", "TenVatlieu", sanPham.MaVatLieu);

            return View(sanPham);
        }

        // GET: Admin/SanPhams/Edit/5
        public ActionResult Edit(int? id)
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
            ViewBag.MaDanhMuc = new SelectList(db.DanhMucs, "MaDanhMuc", "TenDanhMuc", sanPham.MaDanhMuc);
            ViewBag.MaNhaCungCap = new SelectList(db.NhaCungCaps, "MaNhaCungCap", "TenNhaCungCap", sanPham.MaNhaCungCap);
            ViewBag.MaVatLieu = new SelectList(db.VatLieux, "MaVatLieu", "TenVatlieu", sanPham.MaVatLieu);
            return View(sanPham);
        }

        // POST: Admin/SanPhams/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaSanPham,TenSanPham,GiaTienMoi,GiaTienCu,MoTa,AnhSP,MaVatLieu,NgayTao,MaDanhMuc,MaNhaCungCap")]
SanPham sanPham, HttpPostedFileBase AnhSanPham)
        {
            if (ModelState.IsValid)
            {
                var existingProduct = db.SanPhams.AsNoTracking().FirstOrDefault(sp => sp.MaSanPham == sanPham.MaSanPham);
                if (existingProduct == null)
                {
                    return HttpNotFound();
                }

                // Nếu có ảnh mới được tải lên
                if (AnhSanPham != null && AnhSanPham.ContentLength > 0)
                {
                    var idImage = Guid.NewGuid().ToString();
                    int index = AnhSanPham.FileName.IndexOf('.');
                    string _FileName = idImage + "." + AnhSanPham.FileName.Substring(index + 1);
                    string path = Path.Combine(Server.MapPath("~/AnhSanPham"), _FileName);
                    AnhSanPham.SaveAs(path);

                    // Xóa ảnh cũ (nếu có)
                    if (!string.IsNullOrEmpty(existingProduct.AnhSP))
                    {
                        string oldPath = Path.Combine(Server.MapPath("~/AnhSanPham"), existingProduct.AnhSP);
                        if (System.IO.File.Exists(oldPath))
                        {
                            System.IO.File.Delete(oldPath);
                        }
                    }

                    sanPham.AnhSP = _FileName; // Lưu ảnh mới vào DB
                }
                else
                {
                    // Nếu không có ảnh mới, giữ lại ảnh cũ
                    sanPham.AnhSP = existingProduct.AnhSP;
                }

                // Thiết lập Ngày Tạo nếu chưa được gán
                if (sanPham.NgayTao == default(DateTime))
                {
                    sanPham.NgayTao = DateTime.Now.Date;
                }

                db.Entry(sanPham).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            // Tạo lại danh sách cho dropdown nếu ModelState không hợp lệ
            ViewBag.MaDanhMuc = new SelectList(db.DanhMucs, "MaDanhMuc", "TenDanhMuc", sanPham.MaDanhMuc);
            ViewBag.MaNhaCungCap = new SelectList(db.NhaCungCaps, "MaNhaCungCap", "TenNhaCungCap", sanPham.MaNhaCungCap);
            ViewBag.MaVatLieu = new SelectList(db.VatLieux, "MaVatLieu", "TenVatlieu", sanPham.MaVatLieu);

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
        public ActionResult Search(string keyword)
        {
            // Kiểm tra nếu keyword có giá trị
            if (!string.IsNullOrEmpty(keyword))
            {
                // Tìm kiếm sản phẩm theo tên
                var sanPhams = db.SanPhams
                                 .Where(sp => sp.TenSanPham.Contains(keyword))
                                 .OrderByDescending(sp => sp.NgayTao)
                                 .ToList();
                ViewBag.Keyword = keyword; // Gán từ khóa vào ViewBag để sử dụng trong View
                return View("Search", sanPhams); // Trả về danh sách sản phẩm tìm được
            }

            // Nếu không có từ khóa, trả về toàn bộ sản phẩm
            return RedirectToAction("Index");
        }



        public ActionResult TaoKichCo()
        {
            ViewBag.SanPhams = db.SanPhams.ToList(); // Lấy danh sách sản phẩm
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

                db.KichCoes.Add(kichCo);
                db.SaveChanges();
                return RedirectToAction("Index"); // Redirect đến trang danh sách sản phẩm hoặc kích cỡ
            }

            // Nếu ModelState không hợp lệ, cần phải lấy lại danh sách sản phẩm
            ViewBag.SanPhams = db.SanPhams.ToList(); // Lấy lại danh sách sản phẩm nếu có lỗi
            return View(kichCoViewModel);
        }

    }
}

