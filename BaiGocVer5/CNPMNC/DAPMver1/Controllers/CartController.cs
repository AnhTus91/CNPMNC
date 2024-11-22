using DAPMver1.Data;
using DAPMver1.Models;
using DAPMver1.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PayPal.Api;
using System.Net.WebSockets;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;

namespace DAPMver1.Controllers
{
    public class CartController : Controller
    {
        private readonly DapmTrangv1Context _context;
        private readonly IVnPayService _vnPayService;
        private readonly PayPalService _payPalService;

        public CartController(DapmTrangv1Context context,IVnPayService vnPayService, PayPalService payPalService)
        {
            _context = context;
            _vnPayService = vnPayService;
            _payPalService = payPalService;
        }

        public IActionResult Index()
        {
            // Lấy MaNguoiDung từ Session
          
            // Kiểm tra xem người dùng đã đăng nhập hay chưa
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserID")))
            {
                TempData["Message"] = "Bạn cần đăng nhập để xem giỏ hàng.";
                return RedirectToAction("Login", "Account");
            }
            var userId = HttpContext.Session.GetString("UserID");

            // Chuyển đổi userId sang int
            int parsedUserId;
            if (!int.TryParse(userId, out parsedUserId))
            {
                TempData["Message"] = "Có lỗi xảy ra, vui lòng thử lại.";
                return RedirectToAction("Index", "Home");
            }

            // Lấy giỏ hàng và ánh xạ vào danh sách chi tiết giỏ hàng
            var cartItems = _context.ChiTietGioHangs
                .Include(c => c.MaKichCoNavigation)
                .ThenInclude(k => k.MaSanPhamNavigation) // Bao gồm thông tin sản phẩm
                .Where(c => c.MaGioHangNavigation.MaNguoiDung == parsedUserId
                   && c.MaGioHangNavigation.TrangThai == "Chưa thanh toán")
                 .ToList();
            var cart = _context.GioHangs
           .Include(g => g.ChiTietGioHangs)
           .ThenInclude(c => c.MaKichCoNavigation.MaSanPhamNavigation) // Bao gồm thông tin sản phẩm
           .FirstOrDefault(g => g.MaNguoiDung == parsedUserId && g.TrangThai == "Chưa thanh toán");

            TempData["CartTotal"] = cart.TongTien?.ToString("F2");
            // Kiểm tra nếu giỏ hàng rỗng
            if (!cartItems.Any())
            {
                TempData["Message"] = "Giỏ hàng của bạn đang trống.";
            }

            // Trả về giỏ hàng với các thông tin cần thiết
            return View(cartItems);
        }

        // Thêm sản phẩm vào giỏ hàng

        public IActionResult AddToCart(int idsp, int size, int soLuong)
        {
            // Lấy UserID từ Session
             if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserID")))
            {
                // Nếu chưa đăng nhập, chuyển hướng đến trang đăng nhập
                return RedirectToAction("Login", "Account");
            }
            var userId = HttpContext.Session.GetString("UserID");

            // Tìm sản phẩm dựa trên id
            var product = _context.SanPhams.Find(idsp);
            var kichCo = _context.KichCos.FirstOrDefault(n => n.MaSanPham == idsp && n.SoKichCo == size);
            if (product == null || kichCo == null)
            {
                return NotFound();
            }
            if(kichCo.SoLuong <soLuong)
            {
                TempData["ErrorMessage"] = "Số lượng sản phẩm không đủ.";
                return RedirectToAction("ChiTietSP","Product", new { id = idsp });

            }
            // Lấy giỏ hàng hiện tại của người dùng hoặc tạo mới nếu chưa có
            var cart = _context.GioHangs
                               .FirstOrDefault(g => g.MaNguoiDung == int.Parse(userId) && g.TrangThai == "Chưa thanh toán");
            if (cart == null)
            {
                cart = new GioHang
                {
                    MaNguoiDung = int.Parse(userId),
                    NgayTao = DateTime.Now,
                    TrangThai = "Chưa thanh toán",
                    TongTien = 0
                };
                _context.GioHangs.Add(cart);
                _context.SaveChanges();
            }

            // Kiểm tra sản phẩm đã có trong chi tiết giỏ hàng chưa
            var cartItem = _context.ChiTietGioHangs
                                   .FirstOrDefault(c => c.MaGioHang == cart.MaGioHang
                                   && c.MaKichCo == kichCo.MaKichCo);
            if (cartItem != null)
            {
                // Tăng số lượng nếu sản phẩm đã có trong giỏ hàng
                cartItem.SoLuong += soLuong;
            }
            else
            {
                // Thêm sản phẩm mới vào chi tiết giỏ hàng
                cartItem = new ChiTietGioHang
                {
                    MaGioHang = cart.MaGioHang,
                    MaKichCo = kichCo.MaKichCo, // sử dụng kích cỡ được truyền vào
                    SoLuong = soLuong,
                    GiaBan = product.GiaTienMoi
                };
                _context.ChiTietGioHangs.Add(cartItem);
            }

            // Cập nhật tổng tiền của giỏ hàng
            cart.TongTien = cart.ChiTietGioHangs.Sum(c => c.SoLuong * c.GiaBan);
            kichCo.SoLuong -= soLuong;
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        // Xóa sản phẩm khỏi giỏ hàng

        public IActionResult RemoveFromCart(int idsp, int size)
        {
            // Lấy UserID từ Session
            var userId = HttpContext.Session.GetString("UserID");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            // Lấy giỏ hàng của người dùng
            var cart = _context.GioHangs
                .FirstOrDefault(g => g.MaNguoiDung == int.Parse(userId) 
                && g.TrangThai == "Chưa thanh toán");
            if (cart == null) return RedirectToAction("Index");

            // Tìm sản phẩm trong chi tiết giỏ hàng và xóa
            var cartItem = _context.ChiTietGioHangs
                .FirstOrDefault(c => c.MaGioHang == cart.MaGioHang 
                && c.MaKichCo == size && c.MaKichCoNavigation.MaSanPham == idsp); // Sửa dòng này
            if (cartItem != null)
            {
                _context.ChiTietGioHangs.Remove(cartItem);
                _context.SaveChanges();
            }

            // Cập nhật lại tổng tiền của giỏ hàng
            cart.TongTien = cart.ChiTietGioHangs.Sum(c => c.SoLuong * c.GiaBan);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }



        // Cập nhật số lượng sản phẩm trong giỏ hàng
        public IActionResult UpdateQuantity(int idsp, int size, int quantity)
        {
            // Lấy UserID từ Session
            var userId = HttpContext.Session.GetString("UserID");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            // Lấy giỏ hàng của người dùng
            var cart = _context.GioHangs.FirstOrDefault(g => g.MaNguoiDung == int.Parse(userId) && g.TrangThai == "Chưa thanh toán");
            if (cart == null) return RedirectToAction("Index");
            var product = _context.SanPhams.Find(idsp);
            var kichCo = _context.KichCos.Include(k=>k.MaSanPhamNavigation).FirstOrDefault(n => n.MaSanPham == idsp && n.MaKichCo == size);

              if (kichCo == null)
            {
                TempData["Message"] = "Kích cỡ không tồn tại.";
                return RedirectToAction("Index");
            }

            // Kiểm tra số lượng yêu cầu có vượt quá số lượng trong kho không
            if (quantity > kichCo.SoLuong)
            {
                TempData["Message"] = "Số lượng vượt quá tồn kho.";
                return RedirectToAction("Index");
            }
            // Tìm sản phẩm trong chi tiết giỏ hàng và cập nhật số lượng
            var cartItem = _context.ChiTietGioHangs.FirstOrDefault(c => c.MaGioHang == cart.MaGioHang && c.MaKichCo == size && c.MaKichCoNavigation.MaSanPham == idsp);
            if (cartItem != null && quantity > 0)
            {
                cartItem.SoLuong = quantity;
                _context.SaveChanges();
            }

            // Cập nhật lại tổng tiền của giỏ hàng
            cart.TongTien = cart.ChiTietGioHangs.Sum(c => c.SoLuong * c.GiaBan);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }


     

        public IActionResult Checkout()
        {
            if (HttpContext.Session.GetString("UserID") == null)
            {
                TempData["Message"] = "Bạn cần đăng nhập để xem giỏ hàng.";
                return RedirectToAction("Login", "User");
            }
            // Lấy MaNguoiDung từ Session
            var userId = HttpContext.Session.GetString("UserID");
            int parsedUserId;
            int.TryParse(userId, out parsedUserId);
            var cartItems = _context.ChiTietGioHangs.Include(c=> c.MaGioHangNavigation)
             .Include(c => c.MaKichCoNavigation)
          .ThenInclude(k => k.MaSanPhamNavigation) // Bao gồm thông tin sản phẩm
             .Where(c => c.MaGioHangNavigation.MaNguoiDung == parsedUserId
                  && c.MaGioHangNavigation.TrangThai == "Chưa thanh toán")
         .ToList();
            if (cartItems == null || !cartItems.Any())
            {
                return RedirectToAction("Index", "Cart"); // Nếu giỏ hàng rỗng, chuyển về giỏ hàng
            }

            // Tạo danh sách các phương thức thanh toán
            ViewBag.PaymentMethods = new List<string> { "Thanh toán khi nhận hàng", "Chuyển khoản ngân hàng", "Ví điện tử" };
            ViewData["TenNguoiDung"] = HttpContext.Session.GetString("TenNguoiDung");
            ViewData["DiaChi"] = HttpContext.Session.GetString("DiaChi");
            ViewData["SDT"] = HttpContext.Session.GetString("SDT");
            var cart = _context.GioHangs
            .Include(g => g.ChiTietGioHangs)
            .ThenInclude(c => c.MaKichCoNavigation.MaSanPhamNavigation) // Bao gồm thông tin sản phẩm
            .FirstOrDefault(g => g.MaNguoiDung == parsedUserId && g.TrangThai == "Chưa thanh toán");

            TempData["CartTotal"] = cart.TongTien?.ToString("F2");
            return View(cartItems); // Truyền giỏ hàng vào View
        }



        [HttpPost]
        public async Task<IActionResult> ConfirmOrder(
                    string paymentMethod ,
                    string orderOption,
                    string TenNguoiNhanManual,
                    string SoNhaManual, string QuanHuyenManual,
                    string PhuongXaManual,
                    string ThanhPhoManual,
                    string SDTNguoiNhanManual)
        {
        
            var UserID = HttpContext.Session.GetString("UserID");
            var parseUserID = int.Parse(UserID);
            NguoiDung nguoiDung = _context.NguoiDungs.FirstOrDefault(n => n.MaNguoiDung
            == parseUserID );
            var cart = _context.GioHangs
                           .FirstOrDefault(g => g.MaNguoiDung == parseUserID
                           && g.TrangThai == "Chưa thanh toán");
           
             var itemcart = _context.ChiTietGioHangs.Where(i=>i.MaGioHang==cart.MaGioHang).ToList();
            double discountAmount = 0;
            if (TempData["DiscountAmount"] != null)
            {
                double.TryParse(TempData["DiscountAmount"].ToString(), out discountAmount);
            }
            if (HttpContext.Session.GetString("UserID") == null)
            {
                TempData["Message"] = "Bạn cần đăng nhập để xem giỏ hàng.";
                return RedirectToAction("Login", "User");
            }
            // Lấy MaNguoiDung từ Session
            var userId = HttpContext.Session.GetString("UserID");
            var parseuserId = int.Parse(userId);
            var cartItems = _context.ChiTietGioHangs
         .Include(c => c.MaKichCoNavigation)
             .ThenInclude(k => k.MaSanPhamNavigation) // Bao gồm thông tin sản phẩm
         .Where(c => c.MaGioHangNavigation.MaNguoiDung == parseuserId
                     && c.MaGioHangNavigation.TrangThai == "Chưa thanh toán")
         .ToList();

            if (cartItems == null || !cartItems.Any())
            {
                return RedirectToAction("Index", "Home");  // Giỏ hàng rỗng
            }

            // Kiểm tra xem người dùng đã đăng nhập chưa
            if (HttpContext.Session.GetString("UserID") == null)
            {
                // Nếu chưa, yêu cầu đăng nhập
                return RedirectToAction("Login", "User");
            }
            if (nguoiDung == null)
            {
                // Nếu người dùng không tồn tại, yêu cầu đăng nhập lại
                return RedirectToAction("Login", "User");
            }

            // Tính tổng tiền giỏ hàng
            double tongTien = cartItems.Sum(item => item.SoLuong * item.GiaBan);

            string tenNguoiNhan, diaChiNguoiNhan, sdtNguoiNhan;

            if (orderOption == "true")
            {
                // Sử dụng thông tin từ tài khoản
                tenNguoiNhan = nguoiDung.TenNguoiDung;
                diaChiNguoiNhan = nguoiDung.DiaChi;
                sdtNguoiNhan = nguoiDung.Sdt;
            }
            else
            {
                // Sử dụng thông tin người nhận từ form
       
                tenNguoiNhan = TenNguoiNhanManual;
                diaChiNguoiNhan = $"{SoNhaManual}, {PhuongXaManual}, {QuanHuyenManual}, {ThanhPhoManual}";
                sdtNguoiNhan = SDTNguoiNhanManual;
           
            }
            var thue = _context.Thues.FirstOrDefault(t => t.MaThue == 1);
            double tyLeThue = thue.GiaTri / 100.0; // Giả sử tỷ lệ thuế lưu dưới dạng phần trăm
            double tongTienTruocThue = ((int)cart.TongTien - discountAmount); // Tổng tiền trước thuế (bao gồm phí vận chuyển)
            double tongTienSauThue = tongTienTruocThue * (1 + tyLeThue) + 30000;


            DonHang donHang = new DonHang
            {
                MaDonHang = Guid.NewGuid().ToString("N").Substring(0, 12).ToUpper(),
                NgayDatHang = DateOnly.FromDateTime(DateTime.Now),
                TrangThai = "Chờ xử lý",
                PhiVanChuyen = 30000,
                TongTien = (int)cart.TongTien, // Tổng tiền trước phí vận chuyển
                TongSl = cartItems.Sum(c => c.SoLuong),
                TongSoTien = (int)Math.Floor((double)tongTienSauThue),
                TienPhaiTra = (int)Math.Floor((double)(tongTienSauThue)),
                MaNguoiGui = parseUserID,
                SdtnguoiNhan = sdtNguoiNhan,

                DiaChiNguoiNhan = diaChiNguoiNhan,
                TenNguoiNhan = tenNguoiNhan,
                HinhThucNhanHang = "Giao Hàng",
                MaThue = 1
            };

            // Lưu đơn hàng vào cơ sở dữ liệu
            _context.DonHangs.Add(donHang);
            _context.SaveChanges();
            HttpContext.Session.SetString("MaDH", donHang.MaDonHang.ToString());
            foreach (var item in cartItems)
            {
                ChiTietDonHang chiTietDonHang = new ChiTietDonHang
                {
                    MaDonHang = donHang.MaDonHang,
                    MaKichCo = (int)item.MaKichCoNavigation.MaKichCo,
                    Soluong = item.SoLuong,
                    DonGia = (int)item.GiaBan,
                    TongTien = (int)(item.SoLuong * item.GiaBan)
                };

                _context.ChiTietDonHangs.Add(chiTietDonHang);
            }
            _context.SaveChanges();
            _context.ChiTietGioHangs.RemoveRange(cartItems);
            _context.SaveChanges();
          // Sau khi thanh toán thành công, chuyển hướng đến trang PaymentSuccess

            var maDonHang = donHang.MaDonHang;
            if (paymentMethod == "PayPal")
            {
                var payPalService = HttpContext.RequestServices.GetService<PayPalService>();
                var exchangeRate = 24000.0; // Tỷ giá USD
                var amountInUSD = donHang.TienPhaiTra / exchangeRate;
                var approvalUrl = await payPalService.CreateOrderAsync((double)amountInUSD, "USD");

                // Cập nhật trạng thái đơn hàng nếu cần thiết (optional)
                donHang.TrangThai = "Thanh toán PayPal";
                _context.SaveChanges();
                // Chuyển hướng tới PayPal để thanh toán

                return Redirect(approvalUrl);
               
            }
            CreatePayment(donHang, paymentMethod);
            CreateInvoice(donHang, parseUserID);

            SendMailOrder(donHang, nguoiDung);

            return RedirectToAction("OrderSuccess", "Cart", new { maDonHang });
        }
        public IActionResult OrderSuccess(string maDonHang)
        {
            var order = _context.DonHangs.FirstOrDefault(o => o.MaDonHang == maDonHang);

            if (order == null)
            {
                return NotFound(); // Xử lý trường hợp không tìm thấy đơn hàng
            }

            ViewBag.OrderId = maDonHang;
            ViewBag.OrderDetails = order;
            return View();
        }
        public IActionResult OrderSuccessPaypal(string token, string PayerID)
        {
            ViewBag.Token = token;
            ViewBag.PayerID = PayerID;
            CreatePaymentPaypal(token);
            CreateInvoicePayPal(token);
            SendMailOrderPaypal(token);

            return View();
        }
        private void SendMailOrder(DonHang donhang, NguoiDung nguoidung)
        {
            var orderItems = _context.ChiTietDonHangs
           .Include(i => i.MaKichCoNavigation)
           .ThenInclude(k=>k.MaSanPhamNavigation)
           .Include(i => i.MaDonHangNavigation)
           .Where(i => i.MaDonHang == donhang.MaDonHang)
           .ToList();
            var userId = HttpContext.Session.GetString("UserID");
            var parseuserId = int.Parse(userId);
            var cart = _context.GioHangs
           .Include(g => g.ChiTietGioHangs)
           .ThenInclude(c => c.MaKichCoNavigation.MaSanPhamNavigation) // Bao gồm thông tin sản phẩm
           .FirstOrDefault(g => g.MaNguoiDung == parseuserId && g.TrangThai == "Chưa thanh toán");

            var thanhToan = _context.ThanhToans
                  .Include(i => i.MaDonHangNavigation).FirstOrDefault(m=>m.MaDonHang == donhang.MaDonHang);
            // Gửi email thông báo
            string subject = "Thông báo đặt hàng thành công";

            // Tạo nội dung email thông báo thành công với màu xanh và cỡ chữ lớn cho lời chào, và mã đơn hàng in đậm
            string content = $"<p style='color: #4CAF50; font-size: 20px;'>Xin chào {nguoidung.TenNguoiDung},</p>" +
                             $"<p>Đơn hàng mã <strong>{donhang.MaDonHang}</strong> của bạn đã đặt thành công.<br><br>" +
                             $"Đơn hàng sẽ sớm được giao đến bạn.<br><br>Cảm ơn bạn đã tin tưởng và sử dụng dịch vụ của chúng tôi!</p>";

            // Khởi tạo chuỗi thông tin đơn hàng
            content += $"<p>Thông tin đơn hàng của bạn: <br><br> Tên người nhận: {donhang.TenNguoiNhan}<br>";
            if(thanhToan.PhuongThucThanhToan == "Trực tuyến") 
            {
                content += $"<p>Hình thức thanh toán: {thanhToan.PhuongThucThanhToan}";
                if (thanhToan.TrangThaiThanhToan)
                {
                    content += $"<p>Trạng thái thanh toán: Đã thanh toán";

                }
                content += $"<p>Trạng thái thanh toán: Chưa thanh toán";

                content += $"<p>Mã token thanh toán: {thanhToan.MaThanhToan}";
            }
            else
            {
                content += $"<p>Hình thức thanh toán: {thanhToan.PhuongThucThanhToan}";
                if (thanhToan.TrangThaiThanhToan)
                {
                    content += $"<p>Hình thức thanh toán: Đã thanh toán";

                }
                content += $"<p>Hình thức thanh toán: Chưa thanh toán";

                content += $"<p>Mã token thanh toán: {thanhToan.MaThanhToan}";

            }
            // Tạo bảng hiển thị các sản phẩm trong đơn hàng với màu sắc
            content += "<table border='1' cellpadding='5' cellspacing='0' style='border-collapse: collapse; width: 100%;'>";
            content += "<tr style='background-color: #4CAF50; color: white;'>";
            content += "<th>Tên Sản Phẩm</th>" +
                "<th>Số Lượng</th>" +
                "<th>Đơn Giá</th>" +
                "<th>Thành Tiền</th>";
            content += "</tr>";

            // Lặp qua danh sách các mặt hàng trong đơn hàng và hiển thị chúng trong bảng
            foreach (var item in orderItems)
            {
                var productName = item.MaKichCoNavigation.MaSanPhamNavigation.TenSanPham;
                var quantity = item.Soluong;
                var price = item.DonGia;
                var total = price * quantity;

                // Dòng dữ liệu sản phẩm với màu nền xen kẽ
                content += $"<tr style='background-color: #f9f9f9;'>";
                content += $"<td style='text-align: center' >{productName}</td>" +
                    $"<td style='text-align: center' >{quantity}</td>" +
                    $"<td style='text-align: center' >{price}VNĐ</td>" +
                    $"<td style='text-align: center' >{total}VNĐ</td>";
                content += "</tr>";
            }

            // Đóng bảng
            content += "</table>";
            content += $"Tổng tiền: {donhang.TongTien}";
            // Thêm thông tin liên lạc và tổng tiền
            content += $"<br>Số điện thoại: {donhang.SdtnguoiNhan}<br>Địa chỉ: {donhang.DiaChiNguoiNhan}" +
                //$"<br>Giá trị giảm giá: {donhang.MaKhuyenMaiNavigation.GiaTri}.000 VNĐ" +
                $"<br>Thuế: {donhang.MaThueNavigation.TenThue}: {donhang.MaThueNavigation.GiaTri} %";
            content += $"<br>Tổng tiền sau khi giảm giá: {donhang.TienPhaiTra} VNĐ";

            // Gọi phương thức gửi email
            if (Common.Common.SendMail("Trang sức Thuận Tài", subject, content, HttpContext.Session.GetString("email").ToString())) // Email người dùng nằm ở trường `Email`
            {
                ViewBag.Message = "Thông báo giao hàng đã được gửi qua email của khách hàng.";
            }
            else
            {
                ViewBag.Error = "Có lỗi xảy ra trong quá trình gửi email thông báo.";
            }
        }
        private void SendMailOrderPaypal(string token)
        {
            DonHang dh = _context.DonHangs.FirstOrDefault(m => m.MaDonHang == HttpContext.Session.GetString("MaDH").ToString());

            var orderItems = _context.ChiTietDonHangs
           .Include(i => i.MaKichCoNavigation)
           .ThenInclude(k => k.MaSanPhamNavigation)
           .Include(i => i.MaDonHangNavigation)
           .Where(i => i.MaDonHang == dh.MaDonHang)
           .ToList();
            var userId = HttpContext.Session.GetString("UserID");
            var parseuserId = int.Parse(userId);
            NguoiDung nguoidung = _context.NguoiDungs.FirstOrDefault(n => n.MaTaiKhoan == parseuserId);
            var cart = _context.GioHangs
           .Include(g => g.ChiTietGioHangs)
           .ThenInclude(c => c.MaKichCoNavigation.MaSanPhamNavigation) // Bao gồm thông tin sản phẩm
           .FirstOrDefault(g => g.MaNguoiDung == parseuserId && g.TrangThai == "Chưa thanh toán");

            var thanhToan = _context.ThanhToans
                  .Include(i => i.MaDonHangNavigation).FirstOrDefault(m => m.MaThanhToan == token);
            // Gửi email thông báo
            string subject = "Thông báo đặt hàng thành công";

            // Tạo nội dung email thông báo thành công với màu xanh và cỡ chữ lớn cho lời chào, và mã đơn hàng in đậm
            string content = $"<p style='color: #4CAF50; font-size: 20px;'>Xin chào {nguoidung.TenNguoiDung},</p>" +
                             $"<p>Đơn hàng mã <strong>{dh.MaDonHang}</strong> của bạn đã đặt thành công.<br><br>" +
                             $"Đơn hàng sẽ sớm được giao đến bạn.<br><br>Cảm ơn bạn đã tin tưởng và sử dụng dịch vụ của chúng tôi!</p>";

            // Khởi tạo chuỗi thông tin đơn hàng
            content += $"<p>Thông tin đơn hàng của bạn: <br><br> Tên người nhận: {dh.TenNguoiNhan}<br>";
            if (thanhToan.PhuongThucThanhToan == "Trực tuyến")
            {
                content += $"<p>Hình thức thanh toán: {thanhToan.PhuongThucThanhToan}";
                if (thanhToan.TrangThaiThanhToan== true)
                {
                    content += $"<p>Trạng thái thanh toán: Đã thanh toán";

                }
                content += $"<p>Trạng thái thanh toán: Chưa thanh toán";

                content += $"<p>Mã token thanh toán: {thanhToan.MaThanhToan}";
            }
            else { 
                content += $"<p>Hình thức thanh toán: {thanhToan.PhuongThucThanhToan}";
                if (thanhToan.TrangThaiThanhToan == true)
                {
                    content += $"<p>Trạng thái thanh toán: Đã thanh toán";

                }
                content += $"<p>Trạng thái thanh toán: Chưa thanh toán";
            }


            // Tạo bảng hiển thị các sản phẩm trong đơn hàng với màu sắc
            content += "<table border='1' cellpadding='5' cellspacing='0' style='border-collapse: collapse; width: 100%;'>";
            content += "<tr style='background-color: #4CAF50; color: white;'>";
            content += "<th>Tên Sản Phẩm</th>" +
                "<th>Số Lượng</th>" +
                "<th>Đơn Giá</th>" +
                "<th>Thành Tiền</th>";
            content += "</tr>";

            // Lặp qua danh sách các mặt hàng trong đơn hàng và hiển thị chúng trong bảng
            foreach (var item in orderItems)
            {
                var productName = item.MaKichCoNavigation.MaSanPhamNavigation.TenSanPham;
                var quantity = item.Soluong;
                var price = item.DonGia;
                var total = price * quantity;

                // Dòng dữ liệu sản phẩm với màu nền xen kẽ
                content += $"<tr style='background-color: #f9f9f9;'>";
                content += $"<td style='text-align: center' >{productName}</td>" +
                    $"<td style='text-align: center' >{quantity}</td>" +
                    $"<td style='text-align: center' >{price}VNĐ</td>" +
                    $"<td style='text-align: center' >{total}VNĐ</td>";
                content += "</tr>";
            }

            // Đóng bảng
            content += "</table>";


            // Thêm thông tin liên lạc và tổng tiền
            content += $"<br>Số điện thoại: {dh.SdtnguoiNhan}<br>Địa chỉ: {dh.DiaChiNguoiNhan}" +
                     $"<br>Tổng tiền sau khi giảm giá: {dh.TienPhaiTra} VNĐ";


            // Bây giờ bạn có thể gửi email với subject và content này


            // Gọi phương thức gửi email
            if (Common.Common.SendMail("Trang sức Thuận Tài", subject, content, HttpContext.Session.GetString("email").ToString())) // Email người dùng nằm ở trường `Email`
            {
                ViewBag.Message = "Thông báo giao hàng đã được gửi qua email của khách hàng.";
            }
            else
            {
                ViewBag.Error = "Có lỗi xảy ra trong quá trình gửi email thông báo.";
            }
        }
        private void CreateInvoice(DonHang donHang, int userId)
        {
            var idPayment = _context.ThanhToans.FirstOrDefault(m => m.MaDonHang == donHang.MaDonHang);
            // Tạo hóa đơn mới
            HoaDon hoaDon = new HoaDon
            {
                MaHoaDon = Guid.NewGuid().ToString("N").Substring(0, 12).ToUpper(), // Tạo mã hóa đơn duy nhất
                MaDonHang = donHang.MaDonHang,
                MaNguoiDung = userId,
                NgayXuatHoaDon = DateTime.Now,
                TongTien = donHang.TongSoTien, // Tổng tiền hóa đơn bằng tổng số tiền đơn hàng
                MaThanhToan = idPayment.MaThanhToan // Bạn có thể thay thế bằng mã thanh toán thực tế nếu cần
            };

            // Lưu hóa đơn vào cơ sở dữ liệu
            _context.HoaDons.Add(hoaDon);
            _context.SaveChanges();
        }
        private void CreateInvoicePayPal(string payment)
        {
            DonHang dh = _context.DonHangs.FirstOrDefault(m => m.MaDonHang == HttpContext.Session.GetString("MaDH").ToString());
            // Tạo hóa đơn mới
            HoaDon hoaDon = new HoaDon
            {
                MaHoaDon = Guid.NewGuid().ToString("N").Substring(0, 12).ToUpper(), // Tạo mã hóa đơn duy nhất
                MaDonHang = dh.MaDonHang,
                MaNguoiDung = int.Parse(HttpContext.Session.GetString("UserID")),
                NgayXuatHoaDon = DateTime.Now,
                TongTien = dh.TongSoTien, // Tổng tiền hóa đơn bằng tổng số tiền đơn hàng
                MaThanhToan = payment // Bạn có thể thay thế bằng mã thanh toán thực tế nếu cần
            };

            // Lưu hóa đơn vào cơ sở dữ liệu
            _context.HoaDons.Add(hoaDon);
            _context.SaveChanges();
        }
        private void CreatePayment(DonHang donHang, string paymentmethod)
        {
          
                // Thanh toán tiền mặt
                ThanhToan thanhToan = new ThanhToan
                {
                    MaThanhToan = Guid.NewGuid().ToString("N").Substring(12),
                    MaDonHang = donHang.MaDonHang,
                    PhuongThucThanhToan = "Tiền mặt",
                    NgayThanhToan = DateTime.Now,
                    TongTien = donHang.TongSoTien, // Tổng tiền hóa đơn bằng tổng số tiền đơn hàng
                    TrangThaiThanhToan = false
                };
                _context.ThanhToans.Add(thanhToan);
                _context.SaveChanges();
            
        }
        private void CreatePaymentPaypal( string payment)
        {
            DonHang dh = _context.DonHangs.FirstOrDefault(m => m.MaDonHang == HttpContext.Session.GetString("MaDH"));

            // Thanh toán qua PayPal
            ThanhToan thanhToan = new ThanhToan
                {
                    MaThanhToan = payment, // Lưu token nếu có
                    MaDonHang = dh.MaDonHang,
                    PhuongThucThanhToan = "Trực tuyến",
                    NgayThanhToan = DateTime.Now,
                    TongTien = dh.TongSoTien, // Tổng tiền hóa đơn bằng tổng số tiền đơn hàng
                    TrangThaiThanhToan = true
                };
                _context.ThanhToans.Add(thanhToan);
                _context.SaveChanges();
            
            
        }
       

     

        [Authorize]
        public IActionResult PaymentFail()
        {
            return View();
        }

        [Authorize]
        public IActionResult PaymentCallBack()
        {
            var respone = _vnPayService.PaymentExecute(Request.Query);
            if(respone == null || respone.VnPayResponseCode != "00")
            {
                TempData["Message"] = $"Lỗi thanh toán VNPay: {respone.VnPayResponseCode}";
                return RedirectToAction("PaymentFail");
            }
            TempData["Message"] = $"Thanh toán VNPay thành công";
            return RedirectToAction("PaymentSuccess");
        }
        public IActionResult PaymentSuccess()
        {
            return View();
        }

       

        public async Task<IActionResult> PaymentFailure()
        {
            // Kiểm tra thông tin từ PayPal
            var token = HttpContext.Request.Query["token"].ToString();
            if (string.IsNullOrEmpty(token))
            {
                TempData["Message"] = "Giao dịch đã bị huỷ. Vui lòng thử lại.";
                return RedirectToAction("Index", "GioHang");
            }

            // Thực hiện kiểm tra với PayPal để xác nhận trạng thái
            var capturedOrder = await _payPalService.CaptureOrderAsync(token);

            if (capturedOrder.Status != "COMPLETED")
            {
                TempData["Message"] = "Giao dịch tạm hoãn. Bạn có thể thử lại.";
            }
            else
            {
                TempData["Message"] = "Giao dịch đã hoàn tất!";
            }

            return RedirectToAction("Index", "GioHang");
        }

        [HttpPost]
        public IActionResult ApplyVoucher(string voucherCode)
        {
            // Kiểm tra nếu người dùng đã đăng nhập
            var userId = HttpContext.Session.GetString("UserID");
            if (string.IsNullOrEmpty(userId))
            {
                TempData["Message"] = "Bạn cần đăng nhập để áp dụng voucher.";
                return RedirectToAction("Index");
            }

            // Chuyển đổi userId sang int
            int parsedUserId;
            if (!int.TryParse(userId, out parsedUserId))
            {
                TempData["Message"] = "Có lỗi xảy ra, vui lòng thử lại.";
                return RedirectToAction("Index");
            }

            // Lấy giỏ hàng của người dùng
            var cart = _context.GioHangs
                .Include(g => g.ChiTietGioHangs)
                .ThenInclude(c => c.MaKichCoNavigation.MaSanPhamNavigation) // Bao gồm thông tin sản phẩm
                .FirstOrDefault(g => g.MaNguoiDung == parsedUserId && g.TrangThai == "Chưa thanh toán");

            if (cart == null)
            {
                TempData["Message"] = "Giỏ hàng của bạn không tồn tại.";
                return RedirectToAction("Index");
            }

            // Kiểm tra mã voucher
            var voucher = _context.Vouchers
                .FirstOrDefault(v => v.TenVoucher == voucherCode && v.TrangThai
                                     && v.ThoiGianBatDau <= DateTime.Now
                                     && v.ThoiGianKetThuc >= DateTime.Now
                                     && v.SoLuong > 0);

            if (voucher == null)
            {
                TempData["Message"] = "Mã voucher không hợp lệ hoặc đã hết hạn.";
                return RedirectToAction("Index");
            }

            // Tính tổng tiền giỏ hàng
            var totalAmount = cart.ChiTietGioHangs
                .Sum(c => c.SoLuong * c.GiaBan);

            // Tính toán giảm giá
            var discountAmount = voucher.GiaTri > totalAmount ? totalAmount : voucher.GiaTri;

            // Cập nhật giỏ hàng với thông tin giảm giá
            cart.TongTien = totalAmount - discountAmount;

            // Giảm số lượng voucher
            voucher.SoLuong -= 1;

            _context.SaveChanges();

            // Lưu thông tin giảm giá vào TempData để hiển thị
            TempData["DiscountAmount"] = discountAmount.ToString("F2"); // Chuyển thành chuỗi định dạng 2 số thập phân
            TempData["CartTotal"] = cart.TongTien?.ToString("F2"); // Chuyển thành chuỗi định dạng 2 số thập phân

            TempData["Message"] = "Áp dụng voucher thành công!";

            return RedirectToAction("Index");
        }


    }
}