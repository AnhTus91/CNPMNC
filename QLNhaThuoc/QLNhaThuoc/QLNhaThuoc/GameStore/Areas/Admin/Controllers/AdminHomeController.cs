using GameStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GameStore.Areas.Admin.Controllers
{
    public class AdminHomeController : Controller
    {
        GameStoreEntities db = new GameStoreEntities();
 
        // GET: Admin/Home
        public ActionResult Index()
        {

            var donhang = db.DonHangs;
            int demSanPham = db.SanPhams.Count();
            int demUser =db.NguoiDungs.Count();
            int demDonHang = donhang.Count();
            int donHangThanhCong = donhang.Where(d => d.trangThai == "Thành công").Count();
            int donHangDaHuy =donhang.Where(d => d.trangThai =="Đã hủy").Count();


            double doanhThu = 0;
            if (donhang != null)
            {
                if (donhang.Where(d => d.trangThai == "Thành công").Count()>0)
                {
                    doanhThu = donhang.Where(d => d.trangThai == "Thành công").Sum(s => s.tongTien);
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