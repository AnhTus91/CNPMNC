using CNPMNC_Giao.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace CNPMNC_Giao.Controllers
{
    public class UserController : Controller
    {
        DAPM_1Entities db = new DAPM_1Entities();
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
            //if (ModelState.IsValid)
            //{
            //    NguoiDung check = db.NguoiDungs.FirstOrDefault(s => s.TenNguoiDung == username);
            //    if (check == null)
            //    {
            //        ViewBag.error = "Sai tên đăng nhập hoặc mật khẩu";
            //        return View();
            //    }
            //    else
            //    {
            //        if (check.TaiKhoan.MatKhau != matkhau)
            //        {
            //            ViewBag.error = "Sai tên đăng nhập hoặc mật khẩu";
            //            return View();
            //        }
            //        else
            //        {
            //            Session["hoTen"] = check.TenNguoiDung;
            //            Session["email"] = check.TaiKhoan.Email;
            //            Session["sdt"] = check.SDT;
            //            if (check.TaiKhoan.VaiTro == false)
            //                Session["userLogin"] = check.TaiKhoan.Email;
            //            else
            //            {
            //                Session["userLogin"] = check.TenNguoiDung;
            //                Session["adminLogin"] = check.TaiKhoan.Email;
            //                return RedirectToAction("sanpham", "Admin");
            //            }
            //            return RedirectToAction("Index", "Home");
            //        }
            //    }

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
                        // Lấy thông tin người dùng liên quan
                        NguoiDung nguoiDung = check.NguoiDung.FirstOrDefault(); // Giả sử một tài khoản có một người dùng

                        // Lưu thông tin vào Session
                        //if (nguoiDung != null)
                        //{
                        //    Session["hoTen"] = nguoiDung.TenNguoiDung;
                        //    Session["sdt"] = nguoiDung.SDT;
                        //    Session["maTaiKhoan"] = nguoiDung.MaTaiKhoan;
                        //    Session["email"] = nguoiDung.TaiKhoan.Email;
                            
                        //}

                        Session["email"] = check.Email;

                        // Kiểm tra vai trò người dùng
                        if (check.VaiTro == false)
                        {
                            // Người dùng bình thường
                            Session["userLogin"] = check.Email;
                        }
                        else
                        {
                            // Admin
                            Session["userLogin"] = check.Email;
                            Session["adminLogin"] = check.Email;
                            return RedirectToAction("SanPham", "Admin");
                        }

                        // Điều hướng về trang chủ
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
            string email = Session["Email"]?.ToString();
           
            TaiKhoan taiKhoan = db.TaiKhoan.FirstOrDefault(tk => tk.Email == email);
            if (taiKhoan == null)
            {
                return RedirectToAction("Login", "User");
            }

            NguoiDung nguoiDung = new NguoiDung
            {
                MaTaiKhoan = taiKhoan.MaTaiKhoan,
              
            };

            return View(nguoiDung);
        }

        [HttpPost]
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
                return RedirectToAction("UserDetails", "User");
            }

            return View(nguoiDung);
        }

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
