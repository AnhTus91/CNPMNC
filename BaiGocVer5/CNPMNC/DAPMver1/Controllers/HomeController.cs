using DAPMver1.Data;
using DAPMver1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace DAPMver1.Controllers
{
    public class HomeController : Controller
    {
        private readonly DapmTrangv1Context db;

        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        public HomeController(ILogger<HomeController> logger, DapmTrangv1Context _db, IConfiguration configuration)
        {
            db = _db;
            _logger = logger;
            _configuration = configuration; 
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult StoreLocation()
        {
            ViewBag.ApiKey = _configuration["GoogleMaps:ApiKey"];
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult DonHang()
        {

            if (HttpContext.Session.GetString("UserID") == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                int userID = int.Parse(HttpContext.Session.GetString("UserID"));
                var donhang = from dh in db.DonHangs
                              where dh.MaNguoiGuiNavigation.MaNguoiDung == userID
                              select dh;

                var donHangList = db.DonHangs.Where(dh => dh.MaNguoiGui == userID).OrderByDescending(dh => dh.NgayDatHang).ToList();




                return View(donhang.ToList());
            }

        }


        public IActionResult HuyDonHang(string maDH)
        {
            if (HttpContext.Session.GetString("UserID") == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                int userID = int.Parse(HttpContext.Session.GetString("UserID"));
                var dh = db.DonHangs
                           .FirstOrDefault(donhang => donhang.MaDonHang == maDH && donhang.MaNguoiGui == userID);

                if (dh != null)
                {
                    dh.NgayDatHang = DateOnly.FromDateTime(DateTime.Now);
                    dh.TrangThai = "Đã hủy";
                    db.SaveChanges();
                }

                return RedirectToAction("DonHang", "Home");
            }
        }

    }
}