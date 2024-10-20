using CNPMNC_Giao.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CNPMNC_Giao.Controllers
{
    public class CartController : Controller
    {
        CNPMNCEntities db = new CNPMNCEntities();
        // GET: Cart
        public ActionResult XemGioHang()
        {
            int userId = Convert.ToInt32(Session["UserID"]);
            var gioHang = db.GioHangs.Include("ChiTietGioHangs.SanPham")
                .FirstOrDefault(g => g.MaNguoiDung == userId && g.TrangThai == "Chưa thanh toán");

            if (gioHang == null)
            {
                ViewBag.Message = "Giỏ hàng của bạn đang trống.";
                return View();
            }

            return View(gioHang);
        }
        [HttpPost]
        public ActionResult AddToCart(int MaSanPham, int KichCo, int SoLuong)
        {
            // Lấy sản phẩm và kích cỡ
            var sanPham = db.SanPhams.Find(MaSanPham);
            var kichCoSanPham = db.KichCoes.FirstOrDefault(k => k.MaSanPham == MaSanPham && k.MaKichCo == KichCo);

            if (sanPham == null || kichCoSanPham == null || kichCoSanPham.SoLuong < SoLuong)
            {
                // Xử lý nếu không tìm thấy sản phẩm hoặc số lượng không đủ
                return RedirectToAction("Details", "SanPham", new { id = MaSanPham });
            }

            // Lấy giỏ hàng hiện tại của người dùng
            int userId = Convert.ToInt32(Session["UserID"]);
            var gioHang = db.GioHangs.FirstOrDefault(g => g.MaNguoiDung == userId && g.TrangThai == "Chưa thanh toán");

            if (gioHang == null)
            {
                // Tạo mới giỏ hàng nếu chưa có
                gioHang = new GioHang
                {
                    MaNguoiDung = userId,
                    NgayTao = DateTime.Now,
                    TrangThai = "Chưa thanh toán",
                    TongTien = 0 // Tạm thời
                };
                db.GioHangs.Add(gioHang);
                db.SaveChanges();
            }

            // Kiểm tra sản phẩm đã có trong giỏ hàng chưa
            var chiTietGioHang = db.ChiTietGioHangs
                .FirstOrDefault(c => c.MaGioHang == gioHang.MaGioHang && c.MaSanPham == MaSanPham);

            if (chiTietGioHang == null)
            {
                // Thêm mới sản phẩm vào giỏ hàng
                chiTietGioHang = new ChiTietGioHang
                {
                    MaGioHang = gioHang.MaGioHang,
                    MaSanPham = MaSanPham,
                    SoLuong = SoLuong,
                    GiaBan = (double)sanPham.GiaTienMoi
                };
                db.ChiTietGioHangs.Add(chiTietGioHang);
            }
            else
            {
                // Cập nhật số lượng nếu sản phẩm đã có trong giỏ
                chiTietGioHang.SoLuong += SoLuong;
            }

            // Cập nhật số lượng tồn kho
            kichCoSanPham.SoLuong -= SoLuong;

            // Cập nhật tổng tiền
            gioHang.TongTien += chiTietGioHang.SoLuong * chiTietGioHang.GiaBan;

            db.SaveChanges();

            return RedirectToAction("XemGioHang", "Cart");
        }

        public ActionResult Checkout()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "User");
            }

            return View();
        }

        [HttpPost]
        public ActionResult Checkout(string diaChiNguoiNhan, string tenNguoiNhan, string sdtNguoiNhan, int maVoucher = 0)
        {
            if (Session["email"] == null)
            {
                return RedirectToAction("Login", "User");
            }

            // Lấy thông tin người dùng từ session
            string userEmail = Session["email"].ToString();
            TaiKhoan taiKhoan = db.TaiKhoans.FirstOrDefault(tk => tk.Email == userEmail);

            if (taiKhoan == null)
            {
                return RedirectToAction("Login", "User"); // Kiểm tra tài khoản
            }

            // Lấy thông tin người dùng dựa trên tài khoản
            NguoiDung nguoiDung = db.NguoiDungs.FirstOrDefault(nd => nd.MaTaiKhoan == taiKhoan.MaTaiKhoan);

            if (nguoiDung == null)
            {
                return RedirectToAction("Login", "User"); // Kiểm tra người dùng
            }

            // Tìm giỏ hàng của người dùng
            var gioHang = db.GioHangs.FirstOrDefault(g => g.MaNguoiDung == nguoiDung.MaNguoiDung && g.TrangThai == "Chưa thanh toán");

            if (gioHang == null || !gioHang.ChiTietGioHangs.Any())
            {
                // Nếu giỏ hàng trống hoặc không tìm thấy giỏ hàng
                return RedirectToAction("Index", "Home");
            }

            // Tạo đơn hàng mới
            var donHang = new DonHang
            {
                MaDonHang = Guid.NewGuid().ToString(), // Tạo mã đơn hàng duy nhất
                NgayDatHang = DateTime.Now,
                TrangThai = "Chưa giao", // Trạng thái mặc định
                PhiVanChuyen = 0, // Có thể thay đổi sau nếu cần
                TongTien = gioHang.TongTien ?? 0, // Tổng tiền từ giỏ hàng
                MaNguoiGui = nguoiDung.MaNguoiDung,
                SDTNguoiNhan = sdtNguoiNhan,
                DiaChiNguoiNhan = diaChiNguoiNhan,
                TenNguoiNhan = tenNguoiNhan,
                TongSL = gioHang.ChiTietGioHangs.Sum(ct => ct.SoLuong), // Tổng số lượng sản phẩm
                TongSoTien = (int)(gioHang.TongTien ?? 0), // Tổng số tiền
                TienPhaiTra = (int)(gioHang.TongTien ?? 0), // Tiền phải trả
                HinhThucNhanHang = "Giao hàng tận nơi", // Hoặc hình thức khác
                MaVoucher = maVoucher // Voucher nếu có
            };

            db.DonHangs.Add(donHang);
            db.SaveChanges(); // Lưu đơn hàng vào cơ sở dữ liệu

            // Thêm chi tiết đơn hàng
            foreach (var chiTiet in gioHang.ChiTietGioHangs)
            {
                var chiTietDonHang = new ChiTietDonHang
                {
                    MaDonHang = donHang.MaDonHang,
                    MaSanPham = chiTiet.MaSanPham,
                    Soluong = chiTiet.SoLuong,
                    DonGia = (int)chiTiet.GiaBan,
                    TongTien = (int)(chiTiet.SoLuong * chiTiet.GiaBan) // Tính tổng tiền cho từng chi tiết đơn hàng
                };

                db.ChiTietDonHangs.Add(chiTietDonHang);
            }

            db.SaveChanges(); // Lưu chi tiết đơn hàng vào cơ sở dữ liệu

            // Cập nhật trạng thái giỏ hàng
            gioHang.TrangThai = "Đã thanh toán";
            db.SaveChanges();

            return RedirectToAction("Success"); // Chuyển hướng đến trang thành công
        }
        public ActionResult Success()
        {
            return View();
        }
    }
}