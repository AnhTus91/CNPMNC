using GameStore.Models;
using PayPal.Api;
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
		QLThuocSo1VNEntities1 db = new QLThuocSo1VNEntities1();
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

             var query=  sanpham.Include("HinhAnhs").ToList();

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

                    db.ChiTietDonHang.Add(chitiet);
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
		//=============================
		public ActionResult Payment()
		{
			List<Cart> ShoppingCart = Session["ShoppingCart"] as List<Cart>;
			return View(ShoppingCart); // Đảm bảo trả về một IEnumerable<Cart> không null		
                                   }

			[HttpPost]
		public ActionResult ProcessPayment(string fullName, decimal amount)
		{
			var apiContext = new APIContext(new OAuthTokenCredential(
				System.Configuration.ConfigurationManager.AppSettings["PayPal:ClientId"],
				System.Configuration.ConfigurationManager.AppSettings["PayPal:ClientSecret"]).GetAccessToken());

			var payer = new Payer() { payment_method = "paypal" };

			var redirectUrls = new RedirectUrls()
			{
				cancel_url = Url.Action("Cancel", "GioHang", null, Request.Url.Scheme),
				return_url = Url.Action("Success", "GioHang", null, Request.Url.Scheme)
			};

			var itemList = new ItemList() { items = new List<Item>() };
			itemList.items.Add(new Item()
			{
				name = "Đơn hàng của " + fullName,
				currency = "USD",
				price = amount.ToString(),
				quantity = "1",
				sku = "sku"
			});

			var amountDetails = new Amount() { currency = "USD", total = amount.ToString() };

			var transactionList = new List<Transaction>()
		{
			new Transaction()
			{
				description = "Thanh toán đơn hàng",
				invoice_number = Guid.NewGuid().ToString(), // Thay thế bằng mã đơn hàng của bạn
                amount = amountDetails,
				item_list = itemList
			}
		};

			var payment = new Payment()
			{
				intent = "sale",
				payer = payer,
				transactions = transactionList,
				redirect_urls = redirectUrls
			};

			var createdPayment = payment.Create(apiContext);

			// Chuyển hướng người dùng tới PayPal
			var approvalUrl = createdPayment.links.FirstOrDefault(x => x.rel.ToLower().Trim() == "approval_url").href;

			return Redirect(approvalUrl);
		}

		public ActionResult Success()
		{
			// Xử lý thanh toán thành công (cập nhật trạng thái đơn hàng, v.v.)
			return View();
		}

		public ActionResult Cancel()
		{
			// Xử lý khi người dùng hủy thanh toán
			return View();
		}

	}
}