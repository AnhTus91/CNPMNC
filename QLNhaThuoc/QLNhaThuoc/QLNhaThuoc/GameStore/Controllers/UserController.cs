using GameStore.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GameStore.Controllers
{
    public class UserController : Controller
    {

        GameStoreEntities db = new GameStoreEntities();
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
           if(Session["userLogin"]!=null)
            {
                Session["userLogin"] = null;
                Session["hoTen"] = null;
                Session["email"] = null;
                Session["sdt"] = null;
                return RedirectToAction("Index", "Home");
            }    
            return RedirectToAction("Index","Home");
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
                    if(check.matkhau!=matkhau)
                    {
                        ViewBag.error = "Sai tên đăng nhập hoặc mật khẩu";
                        return View();
                    }
                    else
                    {
                        Session["hoTen"] =check.hoTen;
                        Session["email"] = check.email;
                        Session["sdt"] = check.sdt;
                        if(check.roleID==1)
                        Session["userLogin"] = check.username;
                        else
                        {
                            Session["userLogin"] = check.username;
                            Session["adminLogin"] = check.username;
                            return RedirectToAction("sanpham", "Admin");
                        }
                        return RedirectToAction("Index","Home");
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




    }
}