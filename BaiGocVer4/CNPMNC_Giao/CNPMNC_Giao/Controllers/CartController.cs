using CNPMNC_Giao.Models;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace CNPMNC_Giao.Controllers
{
    public class CartController : Controller
    {
        private CNPMNCEntities db = new CNPMNCEntities();

        // GET: Cart
        public ActionResult Index()
        {
            if (Session["UserID"] == null)
            {
                TempData["Message"] = "Bạn cần đăng nhập để xem giỏ hàng.";
                return RedirectToAction("Login", "User");
            }
            // Lấy MaNguoiDung từ Session
            var userId = (int)Session["UserID"];

            // Kiểm tra xem người dùng đã đăng nhập hay chưa
        

            // Chuyển đổi userId sang int
            if (Session["UserID"] == null)
            {
                TempData["Message"] = "Có lỗi xảy ra, vui lòng thử lại.";
                return RedirectToAction("Index", "Home");
            }

            // Lấy giỏ hàng và ánh xạ vào danh sách chi tiết giỏ hàng
            var cartItems = db.ChiTietGioHangs
      .Include("KichCo") // Bao gồm thông tin kích cỡ
      .Include("KichCo.SanPham") // Bao gồm thông tin sản phẩm
      .Where(c => c.GioHang.MaNguoiDung == userId
                  && c.GioHang.TrangThai == "Chưa thanh toán")
      .ToList();



            // Kiểm tra nếu giỏ hàng rỗng
            if (!cartItems.Any())
            {
                TempData["Message"] = "Giỏ hàng của bạn đang trống.";
            }

            return View(cartItems);
        }

        // Thêm sản phẩm vào giỏ hàng
        public ActionResult AddToCart(int idsp, int size, int soLuong)
        {
            if (Session["UserID"] == null)
            {
                TempData["Message"] = "Bạn cần đăng nhập để xem giỏ hàng.";
                return RedirectToAction("Login", "User");
            }
            // Lấy UserID từ Session
            var userId = (int)Session["UserID"];

            // Kiểm tra xem người dùng đã đăng nhập hay chưa
          

            var product = db.SanPhams.Find(idsp);
            var kichCo = db.KichCoes.FirstOrDefault(n => n.MaSanPham == idsp && n.SoKichCo == size);
            if (product == null || kichCo == null)
            {
                return HttpNotFound();
            }

            var cart = db.GioHangs.FirstOrDefault(g => g.MaNguoiDung == userId && g.TrangThai == "Chưa thanh toán");
            if (cart == null)
            {
                cart = new GioHang
                {
                    MaNguoiDung = userId,
                    NgayTao = DateTime.Now,
                    TrangThai = "Chưa thanh toán",
                    TongTien = 0
                };
                db.GioHangs.Add(cart);
                db.SaveChanges();
            }

            var cartItem = db.ChiTietGioHangs.FirstOrDefault(c => c.MaGioHang == cart.MaGioHang && c.MaKichCo == kichCo.MaKichCo);
            if (cartItem != null)
            {
                cartItem.SoLuong += soLuong;
            }
            else
            {
                cartItem = new ChiTietGioHang
                {
                    MaGioHang = cart.MaGioHang,
                    MaKichCo = kichCo.MaKichCo,
                    SoLuong = soLuong,
                    GiaBan = product.GiaTienMoi
                };
                db.ChiTietGioHangs.Add(cartItem);
            }

            cart.TongTien = cart.ChiTietGioHangs.Sum(c => c.SoLuong * c.GiaBan);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // Xóa sản phẩm khỏi giỏ hàng
        public ActionResult RemoveFromCart(int idsp, int size)
        {
            if (Session["UserID"] == null)
            {
                TempData["Message"] = "Bạn cần đăng nhập để xem giỏ hàng.";
                return RedirectToAction("Login", "User");
            }
            // Lấy UserID từ Session
            var userId = (int)Session["UserID"];
            var cart = db.GioHangs.FirstOrDefault(g => g.MaNguoiDung == userId && g.TrangThai == "Chưa thanh toán");
            if (cart == null) return RedirectToAction("Index");

            var cartItem = db.ChiTietGioHangs.FirstOrDefault(c => c.MaGioHang == cart.MaGioHang && c.MaKichCo == size && c.KichCo.MaSanPham == idsp);
            if (cartItem != null)
            {
                db.ChiTietGioHangs.Remove(cartItem);
                db.SaveChanges();
            }

            cart.TongTien = cart.ChiTietGioHangs.Sum(c => c.SoLuong * c.GiaBan);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // Cập nhật số lượng sản phẩm trong giỏ hàng
        public ActionResult UpdateQuantity(int idsp, int size, int quantity)
        {
            if (Session["UserID"] == null)
            {
                TempData["Message"] = "Bạn cần đăng nhập để xem giỏ hàng.";
                return RedirectToAction("Login", "User");
            }
            // Lấy UserID từ Session
            var userId = (int)Session["UserID"];

            var cart = db.GioHangs.FirstOrDefault(g => g.MaNguoiDung == userId && g.TrangThai == "Chưa thanh toán");
            if (cart == null) return RedirectToAction("Index");

            var cartItem = db.ChiTietGioHangs.FirstOrDefault(c => c.MaGioHang == cart.MaGioHang && c.MaKichCo == size && c.KichCo.MaSanPham == idsp);
            if (cartItem != null && quantity > 0)
            {
                cartItem.SoLuong = quantity;
                db.SaveChanges();
            }

            cart.TongTien = cart.ChiTietGioHangs.Sum(c => c.SoLuong * c.GiaBan);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult Checkout()
        {
            if (Session["UserID"] == null)
            {
                TempData["Message"] = "Bạn cần đăng nhập để xem giỏ hàng.";
                return RedirectToAction("Login", "User");
            }
            // Lấy MaNguoiDung từ Session
            var userId = (int)Session["UserID"];
            var cart = db.ChiTietGioHangs
      .Include("KichCo") // Bao gồm thông tin kích cỡ
      .Include("KichCo.SanPham") // Bao gồm thông tin sản phẩm
      .Where(c => c.GioHang.MaNguoiDung == userId
                  && c.GioHang.TrangThai == "Chưa thanh toán")
      .ToList();
            if (cart == null || !cart.Any())
            {
                return RedirectToAction("Index", "Cart"); // Nếu giỏ hàng rỗng, chuyển về giỏ hàng
            }

            // Tạo danh sách các phương thức thanh toán
            ViewBag.PaymentMethods = new List<string> { "Thanh toán khi nhận hàng", "Chuyển khoản ngân hàng", "Ví điện tử" };

            return View(cart); // Truyền giỏ hàng vào View
        }





        [HttpPost]
        public ActionResult ConfirmOrder(string paymentMethod, string orderOption, string TenNguoiNhanManual, string DiaChiNguoiNhanManual, string SDTNguoiNhanManual)
        {
            if (Session["UserID"] == null)
            {
                TempData["Message"] = "Bạn cần đăng nhập để xem giỏ hàng.";
                return RedirectToAction("Login", "User");
            }
            // Lấy MaNguoiDung từ Session
            var userId = (int)Session["UserID"];
            var Cart = db.ChiTietGioHangs
      .Include("KichCo") // Bao gồm thông tin kích cỡ
      .Include("KichCo.SanPham") // Bao gồm thông tin sản phẩm
      .Where(c => c.GioHang.MaNguoiDung == userId
                  && c.GioHang.TrangThai == "Chưa thanh toán")
      .ToList();
            if (Cart == null || !Cart.Any())
            {
                return RedirectToAction("Index", "Home");  // Giỏ hàng rỗng
            }

            // Kiểm tra xem người dùng đã đăng nhập chưa
            if (Session["UserID"] == null)
            {
                // Nếu chưa, yêu cầu đăng nhập
                return RedirectToAction("Login", "User");
            }

            int UserID = (int)Session["UserID"];
            var nguoiDung = db.NguoiDungs.FirstOrDefault(nd => nd.MaNguoiDung == UserID);

            if (nguoiDung == null)
            {
                // Nếu người dùng không tồn tại, yêu cầu đăng nhập lại
                return RedirectToAction("Login", "User");
            }

            // Tính tổng tiền giỏ hàng
            double tongTien = Cart.Sum(item => item.SoLuong * item.GiaBan);

            string tenNguoiNhan, diaChiNguoiNhan, sdtNguoiNhan;

            if (orderOption == "true")
            {
                // Sử dụng thông tin từ tài khoản
                tenNguoiNhan = nguoiDung.TenNguoiDung;
                diaChiNguoiNhan = nguoiDung.DiaChi;
                sdtNguoiNhan = nguoiDung.SDT;
            }
            else
            {
                // Sử dụng thông tin người nhận từ form
                tenNguoiNhan = TenNguoiNhanManual;
                diaChiNguoiNhan = DiaChiNguoiNhanManual;
                sdtNguoiNhan = SDTNguoiNhanManual;
            }

            // Lấy thông tin người nhận từ biểu mẫu
            //string tenNguoiNhan = Request.Form["TenNguoiNhan"];
            //string diaChiNguoiNhan = Request.Form["DiaChiNguoiNhan"];
            //string sdtNguoiNhan = Request.Form["SDTNguoiNhan"];

            int MaTaiKhoan = (int)Session["MaTaiKhoan"];
            TaiKhoan taiKhoan = db.TaiKhoans.FirstOrDefault(tk => tk.MaTaiKhoan == MaTaiKhoan);
            int MaChucVu = (int)Session["MaChucVu"];
            ChucVu chucVu = db.ChucVus.FirstOrDefault(tk => tk.MaChucVu == MaChucVu);
            nguoiDung = new NguoiDung
            {
                MaTaiKhoan = taiKhoan.MaTaiKhoan,
                MaChucVu = chucVu.MaChucVu
            };

            // Tạo đơn hàng mới
            DonHang donHang = new DonHang
            {
                MaDonHang = Guid.NewGuid().ToString("N").Substring(0, 12).ToUpper(),  // Tạo mã đơn hàng duy nhất
                NgayDatHang = DateTime.Now,
                TrangThai = "Chờ xử lý",
                PhiVanChuyen = 30000,  // Giả định phí vận chuyển
                TongTien = Cart.Sum(c => c.GiaBan * c.SoLuong), // Tính tổng tiền từ giỏ hàng

                // Gán giá trị cho các thuộc tính mới
                TongSL = Cart.Sum(c => c.SoLuong),  // Tổng số lượng sản phẩm trong giỏ hàng
                TongSoTien = (int)(Cart.Sum(c => c.GiaBan * c.SoLuong) + 30000),  // Tổng số tiền bao gồm phí vận chuyển
                TienPhaiTra = (int)(Cart.Sum(c => c.GiaBan * c.SoLuong) + 30000), // Số tiền phải trả cũng bao gồm phí vận chuyển
                MaNguoiGui = UserID,  // Thay thế bằng ID người dùng thực tế
                SDTNguoiNhan = sdtNguoiNhan,  // Giả định số điện thoại người nhận
                DiaChiNguoiNhan = diaChiNguoiNhan,  // Giả định địa chỉ
                TenNguoiNhan = tenNguoiNhan,  // Giả định tên người nhận
                HinhThucNhanHang = "Giao Hàng",

                //MaVoucher = 1,// Bạn có thể thay thế bằng mã voucher nếu có
            };

            // Lưu đơn hàng vào cơ sở dữ liệu
            db.DonHangs.Add(donHang);
            db.SaveChanges();

            foreach (var item in Cart)
            {
                ChiTietDonHang chiTietDonHang = new ChiTietDonHang
                {
                    MaDonHang = donHang.MaDonHang,
                    MaSanPham = (int)item.KichCo.MaSanPham,
                    Soluong = item.SoLuong,
                    DonGia = (int)item.GiaBan,
                    TongTien = (int)(item.SoLuong * item.GiaBan)
                };

                db.ChiTietDonHangs.Add(chiTietDonHang);
            }
            db.SaveChanges();
            // Sau khi thanh toán thành công, chuyển hướng đến trang PaymentSuccess
            return RedirectToAction("OrderSuccess", new { maDonHang = donHang.MaDonHang });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmOrderWithAccountInfo()
        {

            int email = (int)Session["MaTaiKhoan"];
            var currentUser = db.NguoiDungs.FirstOrDefault(nd => nd.MaTaiKhoan == email);

            if (currentUser != null)
            {
                DonHang donHang = new DonHang
                {
                    TenNguoiNhan = currentUser.TenNguoiDung,
                    DiaChiNguoiNhan = currentUser.DiaChi,
                    SDTNguoiNhan = currentUser.SDT,
                    NgayDatHang = DateTime.Now,
                };
                db.DonHangs.Add(donHang);
                db.SaveChanges();
                return RedirectToAction("OrderSuccess", new { id = donHang.MaDonHang });
            }
            return View();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CompleteOrder(int MaDonHang)
        {
            // Xử lý hoàn tất đơn hàng ở đây
            var donHang = db.DonHangs.Find(MaDonHang);
            if (donHang != null)
            {
                donHang.TrangThai = "Đã hoàn tất"; // hoặc bất kỳ trạng thái nào bạn muốn
                db.SaveChanges();

                return RedirectToAction("OrderSuccess", new { id = MaDonHang });
            }

            return RedirectToAction("Error"); // Xử lý lỗi nếu không tìm thấy đơn hàng
        }

        public ActionResult OrderSuccess(string orderId)
        {

            ViewBag.OrderId = orderId;
            return View();
        }


    }
}
