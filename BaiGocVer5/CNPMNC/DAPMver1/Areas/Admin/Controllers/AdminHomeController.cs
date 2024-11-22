using DAPMver1.Data;
using Microsoft.AspNetCore.Mvc;

namespace DAPMver1.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminHomeController : Controller
    {
        private readonly DapmTrangv1Context _context;

        public AdminHomeController(DapmTrangv1Context context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var donhang = _context.DonHangs;
            int demSanPham = _context.SanPhams.Count();
            int demUser = _context.NguoiDungs.Count();
            int demDonHang = donhang.Count();
            int donHangThanhCong = donhang.Where(d => d.TrangThai == "Đã nhận").Count();
            int donHangDaHuy = donhang.Where(d => d.TrangThai == "Đã hủy").Count();


            double doanhThu = 0;
            if (donhang != null)
            {
                if (donhang.Where(d => d.TrangThai == "Đã nhận").Count() > 0)
                {
                    doanhThu = donhang.Where(d => d.TrangThai == "Đã nhận").Sum(s => s.TongSoTien);
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
