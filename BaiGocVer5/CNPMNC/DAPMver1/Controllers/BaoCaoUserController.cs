using DAPMver1.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DAPMver1.Controllers
{
    public class BaoCaoUserController : Controller
    {
        private DapmTrangv1Context _context;
        public BaoCaoUserController(DapmTrangv1Context db)
        {
            _context = db;
        }

        // GET: BaoCao/SubmitFeedback
        public IActionResult SubmitFeedback()
        {

               
            if (HttpContext.Session.GetString("UserID") == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var parseuser = int.Parse(HttpContext.Session.GetString("UserID"));
            // Lấy người dùng dựa trên Username
            var user = _context.NguoiDungs.FirstOrDefault(u => u.MaNguoiDung == parseuser);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Lấy toàn bộ phản hồi và trả lời liên quan đến người dùng
            var feedbackHistory = _context.PhanHoiBaoCaos
                                        .Where(ph => ph.MaNguoiDung == user.MaNguoiDung ||
                                                     ph.MaBaoCaoNavigation.MaNguoiDung == user.MaNguoiDung)
                                        .OrderByDescending(ph => ph.NgayPhanHoi)
                                        .ToList();

            return View(feedbackHistory);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitFeedback(string feedbackContent)
        {
            // Kiểm tra nếu người dùng đã đăng nhập và nội dung phản hồi không rỗng
           
            if (HttpContext.Session.GetString("UserID") != null && !string.IsNullOrEmpty(feedbackContent))
            {
                 var parseuser = int.Parse(HttpContext.Session.GetString("UserID"));
                // Tìm người dùng dựa trên Username từ session
                var user = _context.NguoiDungs.FirstOrDefault(u => u.MaNguoiDung == parseuser);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                // Tạo mới báo cáo người dùng
                var report = new BaoCaoNguoiDung
                {
                    MaNguoiDung = user.MaNguoiDung, // Sử dụng MaNguoiDung từ đối tượng người dùng
                    NgayTao = DateTime.Now,
                    TrangThai = 0 // Đặt trạng thái là chưa xử lý
                };
                _context.BaoCaoNguoiDungs.Add(report);
                _context.SaveChanges();

                // Lưu phản hồi của người dùng vào bảng PhanHoiBaoCao
                var feedback = new PhanHoiBaoCao
                {
                    MaBaoCao = report.MaBaoCao,
                    MaNguoiDung = report.MaNguoiDung,
                    NoiDung = feedbackContent,
                    NguoiTraLoi = false, // false biểu thị phản hồi từ người dùng
                    NgayPhanHoi = DateTime.Now
                };
                _context.PhanHoiBaoCaos.Add(feedback);
                _context.SaveChanges();

                // Điều hướng đến trang thông báo thành công
                return RedirectToAction("SubmitFeedback");
            }
            return RedirectToAction("Login", "Account");
        }


        public IActionResult FeedbackSuccess()
        {
            return View();
        }

        public IActionResult FeedbackHistory()
        {
            var username = HttpContext.Session.GetString("UserID");
            var parseuser = int.Parse(username);
                  if (username == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Lấy người dùng dựa trên Username
            var user = _context.NguoiDungs.FirstOrDefault(u => u.MaNguoiDung == parseuser);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Lấy toàn bộ phản hồi và trả lời liên quan đến người dùng
            var feedbackHistory = _context.PhanHoiBaoCaos
                                        .Where(ph => ph.MaNguoiDung == user.MaNguoiDung ||
                                                     ph.MaBaoCaoNavigation.MaNguoiDung == user.MaNguoiDung)
                                        .OrderByDescending(ph => ph.NgayPhanHoi)
                                        .ToList();

            return View(feedbackHistory);
        }
    }
}
