using GameStore.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GameStore.Controllers
{
    public class GioHangController : Controller
    {
        GameStoreEntities db = new GameStoreEntities();
        // GET: GioHang
        public ActionResult Index()
        {
            List<Cart> ShoppingCart = Session["ShoppingCart"] as List<Cart>;
            return View(ShoppingCart);
        }   
        //Thêm sản phẩm vào giỏ hàng
        public RedirectToRouteResult AddToCart(int maSP)
        {
          
            if (Session["ShoppingCart"] == null) // Nếu giỏ hàng chưa được khởi tạo
            {
                Session["ShoppingCart"] = new List<Cart>();  // Khởi tạo Session["giohang"] là 1 List<CartItem>
            }

            List<Cart> ShoppingCart = Session["ShoppingCart"] as List<Cart>;  // Gán qua biến giohang dễ code

            // Kiểm tra xem sản phẩm khách đang chọn đã có trong giỏ hàng chưa

            if (ShoppingCart.FirstOrDefault(m => m.Id == maSP) == null) // ko co sp nay trong gio hang
            {
              // tim sp theo sanPhamID

                var sanpham = from sp in db.SanPhams
                              where sp.maSP == maSP
                              select sp;

             var query=  sanpham.Include("HinhAnh").ToList();

                Cart newItem = new Cart()
                {
                    Id = maSP,
                    Name = query.FirstOrDefault().tenSP,
                    Amount = 1,
                    Photo = query.FirstOrDefault().hinhAnh1,
                    Price = (int)query.FirstOrDefault().giaTien,

                };  // Tạo ra 1 CartItem mới

                ShoppingCart.Add(newItem);  // Thêm CartItem vào giỏ 
            }
            else
            {
                // Nếu sản phẩm khách chọn đã có trong giỏ hàng thì không thêm vào giỏ nữa mà tăng số lượng lên.
                Cart cardItem = ShoppingCart.FirstOrDefault(m => m.Id == maSP);
                cardItem.Amount++;
            }

            
                return RedirectToAction("ChiTietSP", "games", new { id = maSP });
            
               
        }
        //cập nhật sp trong giỏ hàng
        public RedirectToRouteResult UpdateAmount(int maSP, int newAmount)
        {
            // tìm carditem muon sua
            List<Cart> ShoppingCart = Session["ShoppingCart"] as List<Cart>;
            Cart EditAmount = ShoppingCart.FirstOrDefault(m => m.Id == maSP);
            if (EditAmount != null)
            {
                EditAmount.Amount = newAmount;
            }
            return RedirectToAction("Index");

        }
        //xóa sp trong giỏ hàng
        public RedirectToRouteResult RemoveItem(int maSP)
        {
            List<Cart> shoppingCart = Session["ShoppingCart"] as List<Cart>;
            Cart DelItem = shoppingCart.FirstOrDefault(m => m.Id == maSP);
            if (DelItem != null)
            {
                shoppingCart.Remove(DelItem);
            }
            return RedirectToAction("Index");
        }

        //thanh toán

        public ActionResult ThanhToan()
        {
            if (Session["userLogin"] != null)
            {
                if (Session["ShoppingCart"] != null)
                {
                    List<Cart> ShoppingCart = Session["ShoppingCart"] as List<Cart>;
                    if (ShoppingCart.Count() <= 0)
                    {
                        return RedirectToAction("Index", "GioHang");
                    }
                    else
                    {
                        return View(ShoppingCart);
                    }
                }
                return RedirectToAction("Index", "GioHang");


            }   
            else
                return RedirectToAction("Login", "User");
        }


        [HttpPost]
        public JsonResult ThanhToan( string address)
        {
            try
            {
                List<Cart> ShoppingCart = Session["ShoppingCart"] as List<Cart>;

                string username = "";

                if (Session["userLogin"] != null)
                    username = (string)Session["userLogin"];

                double tongTien = 0;
                int soLuong = ShoppingCart.Count();
                string maDonHang = Guid.NewGuid().ToString();

                foreach (var item in ShoppingCart)
                {
                    tongTien += item.Money;
                }

                var donhang = new DonHang();

                donhang.trangThai = "Đang chờ";
                donhang.tongTien = tongTien;
                donhang.username = username;
                donhang.soLuong = soLuong;
                donhang.diachi = address;
                donhang.maDH = maDonHang;
                donhang.createdAt = DateTime.Now;
                donhang.updatedAt = DateTime.Now;

                db.Configuration.ValidateOnSaveEnabled = false;
                db.DonHangs.Add(donhang);
                db.SaveChanges();

                foreach (var item in ShoppingCart)
                {
                    var chitiet = new ChiTietDonHang();
                    chitiet.maDH = maDonHang;
                    chitiet.maSP = item.Id;
                    chitiet.soLuong = item.Amount;
                    chitiet.tongTien = (int)item.Money;

                    db.ChiTietDonHangs.Add(chitiet);
                    db.SaveChanges();

                }
                Session["ShoppingCart"] = null;
                return Json("succes",JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
           
           
           

        }
    }
}