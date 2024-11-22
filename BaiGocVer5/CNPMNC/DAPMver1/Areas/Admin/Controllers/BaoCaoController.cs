using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using DAPMver1.Data;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;


namespace DAPMver1.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BaoCaoController : Controller
    {
        private readonly DapmTrangv1Context _context;

        public BaoCaoController(DapmTrangv1Context context)
        {
            _context = context;
        }

        // List all feedback reports
        public IActionResult ManageReports()
        {
            if (HttpContext.Session.GetString("UserID") != null)
            {
                // Sử dụng Include với đối tượng điều hướng NguoiDung
                var reports = _context.BaoCaoNguoiDungs
                    .Include(bc => bc.MaNguoiDungNavigation) // Bao gồm NguoiDung liên kết
                    .ToList();
                return View(reports);
            }
            return RedirectToAction("Login", "Account", new { area = "" });
        }


        // Display feedback details for responding
        public IActionResult RespondReport(int id)
        {
            if (HttpContext.Session.GetString("UserID") == null)
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            // Load báo cáo cùng với phản hồi (PhanHoiBaoCaos)
            var report = _context.BaoCaoNguoiDungs
                                 .Include(b => b.PhanHoiBaoCaos) // Include phản hồi báo cáo
                                 .ThenInclude(p => p.MaNguoiDungNavigation) // Nếu cần thông tin người dùng phản hồi
                                 .FirstOrDefault(b => b.MaBaoCao == id);

            if (report == null)
            {
                return NotFound(); // Trả về lỗi nếu không tìm thấy báo cáo
            }

            return View(report);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RespondReport(int id, string responseMessage)
        {
            var username = HttpContext.Session.GetString("UserID");
            var parseuser = int.Parse(username);
            if (username != null && !string.IsNullOrEmpty(responseMessage))
            {
                try
                {
                    // Lấy MaNguoiDung từ username
                    var user = _context.NguoiDungs.FirstOrDefault(u => u.MaNguoiDung == parseuser);
                    if (user == null)
                    {
                        return RedirectToAction("Login", "Account", new { area = "" });
                    }

                    var feedback = new PhanHoiBaoCao
                    {
                        MaBaoCao = id,
                        MaNguoiDung = user.MaNguoiDung,
                        NoiDung = responseMessage,
                        NguoiTraLoi = true,
                        NgayPhanHoi = DateTime.Now
                    };

                    // Thêm phản hồi vào bảng PhanHoiBaoCao
                    _context.PhanHoiBaoCaos.Add(feedback);

                    // Lấy báo cáo và cập nhật trạng thái
                    var report = _context.BaoCaoNguoiDungs.Find(id);
                    if (report != null)
                    {
                        report.TrangThai = 1; // Đặt trạng thái thành đã xử lý
                        _context.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu
                    }

                    return RedirectToAction("RespondReport");
                }
                catch (Exception)
                {
                    ViewBag.Error = "Đã có lỗi xảy ra khi trả lời phản hồi.";
                    return View();
                }
            }
            return RedirectToAction("Login", "Account", new { area = "" });
        }
    }
}
