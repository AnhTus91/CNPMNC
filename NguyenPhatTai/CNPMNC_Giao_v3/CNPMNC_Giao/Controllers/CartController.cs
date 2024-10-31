using CNPMNC_Giao.Models;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CNPMNC_Giao.Controllers
{
    public class CartController : Controller
    {
        private CNPMNC1Entities db = new CNPMNC1Entities();

        // GET: Cart
        public ActionResult Index()
        {
            var cart = Session["Cart"] as List<ChiTietGioHang>;
            if (cart == null)
            {
                cart = new List<ChiTietGioHang>();
                Session["Cart"] = cart;
            }
            return View(cart);
        }

        // Thêm sản phẩm vào giỏ hàng
        public ActionResult AddToCart(int id, int size, int SoLuong)
        {
            // Tìm sản phẩm dựa trên id
            var product = db.SanPham.Find(id);
            if (product == null)
            {
                return HttpNotFound(); // Kiểm tra nếu sản phẩm không tồn tại
            }

            // Lấy giỏ hàng từ Session, nếu chưa có thì khởi tạo mới
            var cart = Session["Cart"] as List<ChiTietGioHang>;
            if (cart == null)
            {
                cart = new List<ChiTietGioHang>();
            }

            // Kiểm tra sản phẩm đã có trong giỏ hàng chưa
            var cartItem = cart.FirstOrDefault(c => c.MaSanPham == id);
            if (cartItem != null)
            {
                // Tăng số lượng nếu sản phẩm đã có trong giỏ hàng
                cartItem.SoLuong += SoLuong;
            }
            else
            {
                // Thêm sản phẩm mới vào giỏ hàng
                cart.Add(new ChiTietGioHang
                {
                    MaSanPham = product.MaSanPham,
                    SoLuong = SoLuong,
                    GiaBan = product.GiaTienMoi,
                
                    SanPham = product // Liên kết sản phẩm với ChiTietGioHang
                });
            }

            // Cập nhật lại giỏ hàng trong Session
            Session["Cart"] = cart;

            // Chuyển hướng về trang giỏ hàng
            return RedirectToAction("Index");
        }


        // Xóa sản phẩm khỏi giỏ hàng
        public ActionResult RemoveFromCart(int id)
        {
            var cart = Session["Cart"] as List<ChiTietGioHang>;
            var cartItem = cart.FirstOrDefault(c => c.MaSanPham == id);

            if (cartItem != null)
            {
                cart.Remove(cartItem);
            }

            Session["Cart"] = cart;
            return RedirectToAction("Index");
        }

        // Cập nhật số lượng sản phẩm trong giỏ hàng
        public ActionResult UpdateQuantity(int id, int quantity)
        {
            var cart = Session["Cart"] as List<ChiTietGioHang>;
            var cartItem = cart.FirstOrDefault(c => c.MaSanPham == id);

            if (cartItem != null && quantity > 0)
            {
                cartItem.SoLuong = quantity;
            }

            Session["Cart"] = cart;
            return RedirectToAction("Index");
        }

        public ActionResult Checkout()
        {
            var cart = Session["Cart"] as List<ChiTietGioHang>;
            if (cart == null || !cart.Any())
            {
                return RedirectToAction("Index", "Cart"); // Nếu giỏ hàng rỗng, chuyển về giỏ hàng
            }

            // Tạo danh sách các phương thức thanh toán
            ViewBag.PaymentMethods = new List<string> { "Thanh toán khi nhận hàng", "Chuyển khoản ngân hàng", "Ví điện tử" };

            return View(cart); // Truyền giỏ hàng vào View
        }

        [HttpPost]
        public ActionResult ConfirmOrder(string paymentMethod)
        {
           // Lấy thông tin giỏ hàng từ session
    var Cart = Session["Cart"] as List<ChiTietGioHang>;
    
    if (Cart == null || !Cart.Any())
    {
        return RedirectToAction("Index", "Home");  // Giỏ hàng rỗng
    }

    // Kiểm tra xem người dùng đã đăng nhập chưa
    if (Session["MaNguoiDung"] == null)
    {
        // Nếu chưa, yêu cầu đăng nhập
        return RedirectToAction("Login", "User");
    }

    int maNguoiDung = (int)Session["MaNguoiDung"];
    var nguoiDung = db.NguoiDung.FirstOrDefault(nd => nd.MaNguoiDung == maNguoiDung);

    if (nguoiDung == null)
    {
        // Nếu người dùng không tồn tại, yêu cầu đăng nhập lại
        return RedirectToAction("Login", "User");
    }

    // Tính tổng tiền giỏ hàng
    double tongTien = Cart.Sum(item => item.SoLuong * item.GiaBan);

    // Lấy thông tin người nhận từ biểu mẫu
    string tenNguoiNhan = Request.Form["TenNguoiNhan"];
    string diaChiNguoiNhan = Request.Form["DiaChiNguoiNhan"];
    string sdtNguoiNhan = Request.Form["SDTNguoiNhan"];

            int MaTaiKhoan = (int)Session["MaTaiKhoan"];
            TaiKhoan taiKhoan = db.TaiKhoan.FirstOrDefault(tk => tk.MaTaiKhoan == MaTaiKhoan);
            int MaChucVu = (int)Session["MaChucVu"];
            ChucVu chucVu = db.ChucVu.FirstOrDefault(tk => tk.MaChucVu == MaChucVu);
            nguoiDung = new NguoiDung
            {
                MaTaiKhoan = taiKhoan.MaTaiKhoan, 
                MaChucVu = chucVu.MaChucVu
            };

    // Tạo đơn hàng mới
    DonHang donHang = new DonHang
    {
        MaDonHang ="fbehfefh23",  // Tạo mã đơn hàng duy nhất
        NgayDatHang = DateTime.Now,
        TrangThai = "Chờ xử lý",
        PhiVanChuyen = 30000,  // Giả định phí vận chuyển
        TongTien = Cart.Sum(c => c.GiaBan * c.SoLuong), // Tính tổng tiền từ giỏ hàng

        // Gán giá trị cho các thuộc tính mới
        TongSL = Cart.Sum(c => c.SoLuong),  // Tổng số lượng sản phẩm trong giỏ hàng
        TongSoTien = (int)(Cart.Sum(c => c.GiaBan * c.SoLuong) + 30000),  // Tổng số tiền bao gồm phí vận chuyển
        TienPhaiTra = (int)(Cart.Sum(c => c.GiaBan * c.SoLuong) + 30000), // Số tiền phải trả cũng bao gồm phí vận chuyển
        MaNguoiGui = maNguoiDung,  // Thay thế bằng ID người dùng thực tế
        SDTNguoiNhan = "0908855943",  // Giả định số điện thoại người nhận
        DiaChiNguoiNhan =" diaChiNguoiNhan",  // Giả định địa chỉ
        TenNguoiNhan = "tenNguoiNhan",  // Giả định tên người nhận
        HinhThucNhanHang = "Giao Hàng",
        //MaVoucher = 1,// Bạn có thể thay thế bằng mã voucher nếu có
    };

    // Lưu đơn hàng vào cơ sở dữ liệu
    db.DonHang.Add(donHang);
    db.SaveChanges();

    // Sau khi thanh toán thành công, chuyển hướng đến trang PaymentSuccess
    return RedirectToAction("OrderSuccess", new { maDonHang = donHang.MaDonHang });
        }

        public ActionResult OrderSuccess(string orderId)
        {
            ViewBag.OrderId = orderId;
            return View();
        }
        [HttpPost]
        public ActionResult XacNhanDonHang(string hoTen, string soDienThoai, string diaChi, string HinhThucNhanHang, int TongSoTien)
        {
            var donHang = new DonHang
            {

                TenNguoiNhan = hoTen,
                SDTNguoiNhan = soDienThoai,
                DiaChiNguoiNhan = diaChi,
                NgayDatHang = DateTime.Now,
                TrangThai = "Chờ xác nhận",
                HinhThucNhanHang = HinhThucNhanHang
            };

            db.DonHang.Add(donHang);
            db.SaveChanges();

            return RedirectToAction("OrderSuccess");
        }
    }
    

    


}
