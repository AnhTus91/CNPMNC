using CNPMNC_Giao.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CNPMNC_Giao.Areas.Admin.Controllers
{
    public class DonHangController : Controller
    {
        private CNPMNC_v2Entities1 db = new CNPMNC_v2Entities1();
        // GET: Admin/DonHang
        public ActionResult Index()
        {
            var donhang = db.DonHang;

            return View(donhang.ToList());
        }
       
        public ActionResult ChiTiet(string id)
        {
            var donhang = db.DonHang.FirstOrDefault(s => s.MaDonHang == id);

            if (donhang == null)
            {
                return HttpNotFound();
            }

            // Include the SanPham entity in the ChiTietDonHang query
            var chitietdonhang = db.ChiTietDonHang
                                   .Where(s => s.MaDonHang == id)
                                   
                                   .ToList();

            // Pass both the order and its details to the view
            var model = new Tuple<DonHang, List<ChiTietDonHang>>(donhang, chitietdonhang);

            return View(model);
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
        public ActionResult ExportInvoice(string id)
        {
            var donhang = db.DonHang.FirstOrDefault(s => s.MaDonHang == id);
            if (donhang == null)
            {
                return HttpNotFound();
            }

            var chitietdonhang = db.ChiTietDonHang
                                    .Where(s => s.MaDonHang == id)
                                    .ToList();

            MemoryStream workStream = new MemoryStream();
            Document document = new Document();
            PdfWriter.GetInstance(document, workStream).CloseStream = false;

            // Load the Roboto font from your project directory
            string fontPath = Server.MapPath("~/fonts/Roboto-Regular.ttf"); // Adjust the path as necessary
            BaseFont robotoBaseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            Font robotoFont = new Font(robotoBaseFont, 12);

            document.Open();
            document.Add(new Paragraph("Hóa Đơn", robotoFont));
            document.Add(new Paragraph("Mã Đơn Hàng: " + donhang.MaDonHang, robotoFont));
            document.Add(new Paragraph("Ngày Đặt: " + donhang.NgayDatHang.ToString("dd/MM/yyyy"), robotoFont));
            document.Add(new Paragraph("Tên Khách Hàng: " + donhang.TenNguoiNhan, robotoFont));
            document.Add(new Paragraph("Tổng Số Tiền: " + donhang.TongTien.ToString("#,##0").Replace(',', '.') + " VNĐ", robotoFont));


            document.Add(new Paragraph("Chi Tiết Đơn Hàng:", robotoFont));
            foreach (var item in chitietdonhang)
            {
                document.Add(new Paragraph($"Sản Phẩm: {item.SanPham.TenSanPham}, Số Lượng: {item.Soluong}, Giá Bán: {item.DonGia.ToString("#,##0").Replace(',', '.')} VNĐ, Tổng Tiền: {item.TongTien.ToString("#,##0").Replace(',', '.')} VNĐ", robotoFont));
            }

            document.Close();


            byte[] byteInfo = workStream.ToArray();
            workStream.Write(byteInfo, 0, byteInfo.Length);
            workStream.Position = 0;

            return File(workStream, "application/pdf", $"Invoice_{id}.pdf");
        }
        public ActionResult ThongKeDoanhThu()
        {
            var doanhThuData = db.DonHang
                .Where(dh => dh.TrangThai == "Chờ xử lý") // Adjust the status as per your data
                .Select(dh => new DoanhThuStatisticsViewModel
                {
                    MaDonHang = dh.MaDonHang,
                    NgayDatHang = dh.NgayDatHang,
                    TongTien = dh.TongTien // Convert double to decimal
                })
                .ToList();

            return View(doanhThuData);
        }
        public ActionResult ThongKeDoanhThuTheoNgay()
        {
            var doanhThuData = db.DonHang
                .GroupBy(dh => dh.NgayDatHang) // Nhóm theo ngày đặt hàng
                .Select(g => new DoanhThuTheoNgayViewModel
                {
                    Ngay = g.Key, // Ngày đặt hàng
                    TongDoanhThu = g.Sum(dh => dh.TongTien), // Tính tổng doanh thu theo ngày
                    SoDonHang = g.Count() // Tính số đơn hàng theo ngày
                })
                .OrderBy(d => d.Ngay) // Sắp xếp theo ngày
                .ToList();

            return View(doanhThuData);
        }

    }
}