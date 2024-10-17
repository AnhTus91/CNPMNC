using CNPMNC_Giao.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CNPMNC_Giao.Areas.Admin.Controllers
{
    public class AdminHomeController : Controller
    {
        private CNPMNCEntities db = new CNPMNCEntities();
        // GET: Admin/AdminHome
        public ActionResult Index()
        {
            var donhang = db.DonHangs;
            int demSanPham = db.SanPhams.Count();
            int demUser = db.NguoiDungs.Count();
            int demDonHang = donhang.Count();
            int donHangThanhCong = donhang.Where(d => d.TrangThai == "Thành công").Count();
            int donHangDaHuy = donhang.Where(d => d.TrangThai == "Đã hủy").Count();


            double doanhThu = 0;
            if (donhang != null)
            {
                if (donhang.Where(d => d.TrangThai == "Thành công").Count() > 0)
                {
                    doanhThu = donhang.Where(d => d.TrangThai == "Thành công").Sum(s => s.TongSoTien);
                }
            }


            ViewBag.demSanPham = demSanPham;
            ViewBag.demUser = demUser;
            ViewBag.demDonHang = demDonHang;
            ViewBag.donHangThanhCong = donHangThanhCong;
            ViewBag.donHangDaHuy = donHangDaHuy;
            ViewBag.doanhThu = doanhThu;
            return View();
        }
    }
}
