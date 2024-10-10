using GameStore.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.Mvc;


namespace GameStore.Controllers
{
    public class UserController : Controller
    {
		QLThuocSo1VNEntities1 db = new QLThuocSo1VNEntities1();
        //view
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
        public ActionResult Login(string username, string matkhau)
        {
            if (ModelState.IsValid)
            {
                NguoiDung check = db.NguoiDungs.FirstOrDefault(s => s.username == username);
                if (check == null)
                {
                    ViewBag.error = "Sai tên đăng nhập hoặc mật khẩu";
                    return View();
                }
                else
                {
                    if (check.matkhau != matkhau)
                    {
                        ViewBag.error = "Sai tên đăng nhập hoặc mật khẩu";
                        return View();
                    }
                    else
                    {
                        Session["hoTen"] = check.hoTen;
                        Session["email"] = check.email;
                        Session["sdt"] = check.sdt;
                        if (check.roleID == 1)
                            Session["userLogin"] = check.username;
                        else
                        {
                            Session["userLogin"] = check.username;
                            Session["adminLogin"] = check.username;
                            return RedirectToAction("sanpham", "Admin");
                        }
                        return RedirectToAction("Index", "Home");
                    }
                }


            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(NguoiDung user)
        {
            if (ModelState.IsValid)
            {
                NguoiDung check = db.NguoiDungs.FirstOrDefault(s => s.username == user.username);
                if (check == null)
                {
                    user.roleID = 1;

                    db.Configuration.ValidateOnSaveEnabled = false;

                    db.NguoiDungs.Add(user);
                    db.SaveChanges();
                    return RedirectToAction("Login");
                }
                else
                {
                    ViewBag.error = "Tài khoản đã tồn tại";
                    return View();
                }


            }
            return View();


        }

         // Quên mật khẩu
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(string email)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra xem email có tồn tại trong hệ thống không
                var user = db.NguoiDungs.FirstOrDefault(u => u.email == email);
                if (user != null)
                {
                    // Tạo mã khôi phục
                    string recoveryCode = Guid.NewGuid().ToString(); // Tạo mã khôi phục

                    // Gửi mã khôi phục qua email
                    string subject = "Khôi phục mật khẩu";
                    string content = $"Mã khôi phục của bạn là: {recoveryCode}";

                    if (Common.Common.SendMail(user.hoTen, subject, content, user.email))
                    {
                        Session["RecoveryCode"] = recoveryCode; // Lưu mã khôi phục vào session để kiểm tra sau này
                        Session["Email"] = user.email; // Lưu email để khôi phục sau
                        ViewBag.Message = "Mã khôi phục đã được gửi tới email của bạn.";
                        return RedirectToAction("VerifyRecoveryCode");
                    }
                    else
                    {
                        ViewBag.Error = "Có lỗi xảy ra trong việc gửi email.";
                    }
                }
                else
                {
                    ViewBag.Error = "Email không tồn tại.";
                }
            }
            return View();
        }

        // Xác nhận mã khôi phục
        [HttpGet]
        public ActionResult VerifyRecoveryCode()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VerifyRecoveryCode(string recoveryCode)
        {
            if (ModelState.IsValid)
            {
                string sessionRecoveryCode = (string)Session["RecoveryCode"];
                string email = (string)Session["Email"];

                if (sessionRecoveryCode == recoveryCode)
                {
                    // Mã khôi phục hợp lệ, chuyển sang trang đặt lại mật khẩu
                    return RedirectToAction("ResetPassword", new { email = email });
                }
                else
                {
                    ViewBag.Error = "Mã khôi phục không hợp lệ.";
                }
            }
            return View();
        }


        // Đặt lại mật khẩu
        [HttpGet]
        public ActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(string newPassword, string confirmPassword)
        {
            string email = (string)Session["Email"];
            var user = db.NguoiDungs.FirstOrDefault(u => u.email == email);

            if (user != null && newPassword == confirmPassword)
            {
                // Cập nhật mật khẩu mới
                user.matkhau = newPassword;
                db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                // Xóa session RecoveryCode và Email
                Session["RecoveryCode"] = null;
                Session["Email"] = null;

                ViewBag.Message = "Mật khẩu đã được đặt lại thành công.";
                return RedirectToAction("Login");
            }
            else
            {
                ViewBag.Error = "Mật khẩu không khớp hoặc có lỗi xảy ra.";
            }

            return View();
        }
    }
}
