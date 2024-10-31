using CNPMNC_Giao.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace CNPMNC_Giao.Controllers
{
    public class UserController : Controller
    {
        CNPMNC_v2Entities db = new CNPMNC_v2Entities();
        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        //view
        public ActionResult Register()
        {
            return View();
        }
        //view
        public ActionResult Login()
        {
            return View();
        }
        public RedirectToRouteResult DangXuat()
        {
            if (Session["userLogin"] != null)
            {
                Session["userLogin"] = null;
                Session["hoTen"] = null;
                Session["email"] = null;
                Session["sdt"] = null;
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string matkhau)
        {
            // Giả sử bạn có bảng TaiKhoan và đang xác thực qua email và mật khẩu
            var loggedInUser = db.TaiKhoan
                                 .FirstOrDefault(u => u.Email == email && u.MatKhau == matkhau);

            if (ModelState.IsValid)
            {
                // Tìm kiếm tài khoản dựa trên email
                TaiKhoan check = db.TaiKhoan.FirstOrDefault(s => s.Email == email);
                if (check == null)
                {
                    // Nếu không tìm thấy tài khoản
                    ViewBag.error = "Sai email đăng nhập hoặc mật khẩu";
                    return View();
                }
                else
                {
                    if (check.MatKhau != matkhau)
                    {
                        ViewBag.error = "Sai email đăng nhập hoặc mật khẩu";
                        return View();
                    }
                    else
                    {
                        // Kiểm tra thông tin người dùng
                        NguoiDung nguoiDung = db.NguoiDung.FirstOrDefault(nd => nd.MaTaiKhoan == check.MaTaiKhoan);

                        // Nếu không có thông tin người dùng, chuyển hướng đến trang tạo thông tin người dùng
                        if (nguoiDung == null)
                        {
                            TempData["email"] = email; // Lưu email vào TempData để sử dụng trên trang tạo thông tin
                            Session["Email"] = email;
                            return RedirectToAction("CreateUserDetails", "User");
                        }

                        // Lưu thông tin người dùng vào Session
                        Session["MaTaiKhoan"] = check.MaTaiKhoan;
                        Session["UserID"] = nguoiDung.MaNguoiDung;
                        Session["MaChucVu"] = nguoiDung.MaChucVu;
                        Session["ChucVu"] = nguoiDung.ChucVu.TenChucVu;
                        Session["email"] = check.Email;
                        // Khi người dùng đăng nhập thành công
                        Session["MaNguoiDung"] = nguoiDung.MaNguoiDung; // Lưu mã người dùng vào Session

                        Session["TenNguoiDung"] = nguoiDung.TenNguoiDung;
                        Session["DiaChi"] = nguoiDung.DiaChi;
                        Session["SDT"] = nguoiDung.SDT;
                        Session["userLogin"] = check.Email;
                        if (nguoiDung.MaChucVu == 2)  // Người dùng bình thường
                        {
                            Session["userLogin"] = nguoiDung.TenNguoiDung;
                        }
                        else if (nguoiDung.MaChucVu == 4 || nguoiDung.MaChucVu == 3)  // Quản trị viên
                        {
                            Session["userLogin"] = nguoiDung.TenNguoiDung;
                            Session["adminLogin"] = nguoiDung.TenNguoiDung;
                            return RedirectToAction("Index", "AdminHome", new { area = "Admin" });
                        }

                        if (loggedInUser != null)
                        {
                            // Lưu vai trò vào Session
                            Session["VaiTro"] = loggedInUser.VaiTro == true ? "Admin" : "Employee";
                        }
                        else
                        {
                            // Xử lý nếu đăng nhập không thành công
                            ViewBag.ErrorMessage = "Sai email hoặc mật khẩu.";
                        }

                        if (nguoiDung.MaChucVu == 2 || nguoiDung.MaChucVu == 3)
                        {

                            return RedirectToAction("AdminHome", "Admin");
                        }
                        // Nếu người dùng đã có thông tin, điều hướng đến trang chủ
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(string email, string matkhau)
        {
            if (ModelState.IsValid)
            {
                NguoiDung user = new NguoiDung();
                // Kiểm tra xem email đã tồn tại trong bảng TaiKhoan chưa
                TaiKhoan check = db.TaiKhoan.FirstOrDefault(s => s.Email == email);
                if (check == null)
                {
                    // Tạo tài khoản mới
                    TaiKhoan newAccount = new TaiKhoan
                    {
                        Email = email,
                        MatKhau = matkhau
                    };

                    // Thêm tài khoản mới vào cơ sở dữ liệu
                    db.TaiKhoan.Add(newAccount);
                    db.SaveChanges();

                    // Gán tài khoản vừa tạo vào người dùng mới

                    // Gán mã chức vụ cho người dùng (ví dụ MaChucVu = 1 có thể là người dùng thông thường)



                    return RedirectToAction("Login");
                }
                else
                {
                    // Nếu tài khoản đã tồn tại
                    ViewBag.error = "Email đã tồn tại";
                    return View();
                }
            }

            return View();
        }

        //đơn hàng 
        public ActionResult OrderHistory(int id)
        {
            // Kiểm tra xem người dùng có đăng nhập không
            if (Session["MaNguoiDung"] == null)
            {
                return RedirectToAction("Login", "User");
            }

            // Lấy thông tin người dùng dựa trên id
            var nguoiDung = db.NguoiDung.FirstOrDefault(nd => nd.MaNguoiDung == id);

            if (nguoiDung == null)
            {
                return HttpNotFound();
            }

            // Lấy danh sách đơn hàng của người dùng
            var donHangList = db.DonHang.Where(dh => dh.MaNguoiGui == id).OrderByDescending(dh => dh.NgayDatHang).ToList();

            return View(donHangList);
        }
        public ActionResult OrderDetails(string id)
        {
            // Tìm đơn hàng theo mã
            var donHang = db.DonHang.FirstOrDefault(dh => dh.MaDonHang == id);

            if (donHang == null)
            {
                return HttpNotFound();
            }

            // Lấy chi tiết đơn hàng
            var chiTietDonHang = db.ChiTietDonHang.Where(ct => ct.MaDonHang == id).ToList();

            ViewBag.ChiTietDonHang = chiTietDonHang;

            return View(donHang);
        }
        public ActionResult CancelOrder(string id)
        {
            var donHang = db.DonHang.FirstOrDefault(dh => dh.MaDonHang == id);

            if (donHang == null)
            {
                return HttpNotFound();
            }

            // Chỉ cho phép hủy đơn hàng nếu trạng thái không phải là "Đang chuẩn bị" hoặc "Đang xử lý"
            if (donHang.TrangThai == "Chờ xử lý")
            {
                donHang.TrangThai = "Đã hủy";
                db.SaveChanges();
            }else
            {
                TempData["ErrorMessage"] = "Đơn hàng không thể hủy";
            }

            return RedirectToAction("OrderHistory", new { id = donHang.MaNguoiGui });
        }


        //    // Quên mật khẩu
        //    public ActionResult ForgotPassword()
        //    {
        //        return View();
        //    }

        //    [HttpPost]
        //    [ValidateAntiForgeryToken]
        //    public ActionResult ForgotPassword(string email)
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            // Kiểm tra xem email có tồn tại trong hệ thống không
        //            var user = db.NguoiDungs.FirstOrDefault(u => u.email == email);
        //            if (user != null)
        //            {
        //                // Tạo mã khôi phục
        //                string recoveryCode = Guid.NewGuid().ToString(); // Tạo mã khôi phục
        //                // Gửi mã khôi phục qua email
        //                string subject = "Khôi phục mật khẩu";
        //                string content = $"Mã khôi phục của bạn là: {recoveryCode}";

        //                if (Common.Common.SendMail(user.hoTen, subject, content, user.email))
        //                {
        //                    Session["RecoveryCode"] = recoveryCode; // Lưu mã khôi phục vào session để kiểm tra sau này
        //                    Session["Email"] = user.email; // Lưu email để khôi phục sau
        //                    ViewBag.Message = "Mã khôi phục đã được gửi tới email của bạn.";
        //                    return RedirectToAction("VerifyRecoveryCode");
        //                }
        //                else
        //                {
        //                    ViewBag.Error = "Có lỗi xảy ra trong việc gửi email.";
        //                }
        //            }
        //            else
        //            {
        //                ViewBag.Error = "Email không tồn tại.";
        //            }
        //        }
        //        return View();
        //    }

        //    // Xác nhận mã khôi phục
        //    [HttpGet]
        //    public ActionResult VerifyRecoveryCode()
        //    {
        //        return View();
        //    }

        //    [HttpPost]
        //    [ValidateAntiForgeryToken]
        //    public ActionResult VerifyRecoveryCode(string recoveryCode)
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            string sessionRecoveryCode = (string)Session["RecoveryCode"];
        //            string email = (string)Session["Email"];

        //            if (sessionRecoveryCode == recoveryCode)
        //            {
        //                // Mã khôi phục hợp lệ, chuyển sang trang đặt lại mật khẩu
        //                return RedirectToAction("ResetPassword", new { email = email });
        //            }
        //            else
        //            {
        //                ViewBag.Error = "Mã khôi phục không hợp lệ.";
        //            }
        //        }
        //        return View();
        //    }


        //    // Đặt lại mật khẩu
        //    [HttpGet]
        //    public ActionResult ResetPassword()
        //    {
        //        return View();
        //    }

        //    [HttpPost]
        //    [ValidateAntiForgeryToken]
        //    public ActionResult ResetPassword(string newPassword, string confirmPassword)
        //    {
        //        string email = (string)Session["Email"];
        //        var user = db.NguoiDungs.FirstOrDefault(u => u.email == email);

        //        if (user != null && newPassword == confirmPassword)
        //        {
        //            // Cập nhật mật khẩu mới
        //            user.matkhau = newPassword;
        //            db.Entry(user).State = System.Data.Entity.EntityState.Modified;
        //            db.SaveChanges();

        //            // Xóa session RecoveryCode và Email
        //            Session["RecoveryCode"] = null;
        //            Session["Email"] = null;

        //            ViewBag.Message = "Mật khẩu đã được đặt lại thành công.";
        //            return RedirectToAction("Login");
        //        }
        //        else
        //        {
        //            ViewBag.Error = "Mật khẩu không khớp hoặc có lỗi xảy ra.";
        //        }

        //        return View();
        //    }
        public ActionResult UserDetails()
        {
            string email = Session["email"]?.ToString();
            if (email == null)
            {
                return RedirectToAction("Login", "User");
            }

            TaiKhoan taiKhoan = db.TaiKhoan.FirstOrDefault(tk => tk.Email == email);
            NguoiDung nguoiDung = db.NguoiDung.FirstOrDefault(nd => nd.MaTaiKhoan == taiKhoan.MaTaiKhoan);
            if (nguoiDung == null)
            {
                return RedirectToAction("CreateUserDetails");
            }

            return View(nguoiDung);
        }
        public ActionResult CreateUserDetails()
        {
            // Lấy email từ TempData nếu có
            ViewBag.Email = TempData["email"] as string;
            string email = Session["Email"]?.ToString();

            TaiKhoan taiKhoan = db.TaiKhoan.FirstOrDefault(tk => tk.Email == email);
            NguoiDung nguoiDung = new NguoiDung
            {
                MaTaiKhoan = taiKhoan.MaTaiKhoan,

            };
            return View(nguoiDung);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateUserDetails(NguoiDung nguoiDung, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                // Handle file upload
                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(ImageFile.FileName);
                    string directoryPath = Server.MapPath("~/Images");

                    // Create the directory if it doesn't exist
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }
                    nguoiDung.MaChucVu = 1;
                    string path = Path.Combine(directoryPath, fileName);
                    ImageFile.SaveAs(path);
                    nguoiDung.AnhDaiDien = "/Images/" + fileName; // Save the path to the image in the model

                }



                db.NguoiDung.Add(nguoiDung);
                db.SaveChanges();
                // Lưu thông tin vào Session sau khi tạo
                Session["UserID"] = nguoiDung.MaNguoiDung;
                Session["TenNguoiDung"] = nguoiDung.TenNguoiDung;
                Session["DiaChi"] = nguoiDung.DiaChi;
                Session["SDT"] = nguoiDung.SDT;// Lưu email
                return RedirectToAction("UserDetails", "User");
            }

            return View(nguoiDung);
        }


        //[HttpPost]
        //    public ActionResult CreateUserDetails(NguoiDung nguoiDung, HttpPostedFileBase ImageFile)
        //    {
        //        if (ModelState.IsValid)
        //        {

        //            db.NguoiDungs.Add(nguoiDung);
        //            db.SaveChanges();
        //            return RedirectToAction("UserDetails", "User");
        //        }

        //        return View(nguoiDung);
        //    }

        public ActionResult EditUserDetails()
        {
            string email = Session["Email"]?.ToString();
            if (email == null)
            {
                return RedirectToAction("Login", "User");
            }

            TaiKhoan taiKhoan = db.TaiKhoan.FirstOrDefault(tk => tk.Email == email);
            NguoiDung nguoiDung = db.NguoiDung.FirstOrDefault(nd => nd.MaTaiKhoan == taiKhoan.MaTaiKhoan);

            return View(nguoiDung);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUserDetails(NguoiDung nguoiDung, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra xem có file nào được upload không
                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    // Đường dẫn thư mục lưu trữ ảnh
                    string fileName = Path.GetFileName(ImageFile.FileName);
                    string path = Path.Combine(Server.MapPath("~/Images"), fileName);

                    // Lưu file vào thư mục
                    ImageFile.SaveAs(path);

                    // Cập nhật đường dẫn ảnh vào model
                    nguoiDung.AnhDaiDien = Path.Combine("/Images", fileName);
                }

                // Tìm người dùng trong cơ sở dữ liệu
                var existingUser = db.NguoiDung.Find(nguoiDung.MaNguoiDung);
                if (existingUser != null)
                {
                    existingUser.TenNguoiDung = nguoiDung.TenNguoiDung;
                    existingUser.DiaChi = nguoiDung.DiaChi;
                    existingUser.SDT = nguoiDung.SDT;
                    existingUser.AnhDaiDien = nguoiDung.AnhDaiDien;

                    db.Entry(existingUser).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }

                // Chuyển hướng đến trang thông tin người dùng
                return RedirectToAction("UserDetails", new { id = nguoiDung.MaNguoiDung });
            }
            // Nếu có lỗi, trả lại view
            return View(nguoiDung);
        }
       
       
    }

}