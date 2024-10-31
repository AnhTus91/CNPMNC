using CNPMNC_Giao.Models;
using Microsoft.AspNetCore.Mvc; // Đảm bảo sử dụng Microsoft.AspNetCore.Mvc
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

public class GioHangsController : Controller
{
    private readonly DAPM_1Entities _context;

    public GioHangsController(DAPM_1Entities context)
    {
        _context = context;
    }

    // Thêm sản phẩm vào giỏ hàng
    [HttpPost]
    public IActionResult ThemVaoGioHang(int MaSanPham, int SoLuong)
    {
        var sanPham = _context.SanPham.Find(MaSanPham);
        if (sanPham == null)
        {
            return NotFound(); // Trả về 404 nếu không tìm thấy sản phẩm
        }

        // Lấy giỏ hàng của người dùng (giả sử MaNguoiDung = 1)
        var gioHang = _context.GioHang.Include(g => g.ChiTietGioHang).FirstOrDefault(g => g.MaNguoiDung == 1) ?? new GioHang
        {
            MaNguoiDung = 1,
            NgayTao = DateTime.Now,
            ChiTietGioHang = new List<ChiTietGioHang>() // Khởi tạo danh sách chi tiết giỏ hàng
        };

        // Thêm chi tiết vào giỏ hàng
        var chiTiet = new ChiTietGioHang
        {
            MaSanPham = MaSanPham,
            SoLuong = SoLuong,
            GiaBan = sanPham.GiaTienMoi,
            SanPham = sanPham
        };

        gioHang.ChiTietGioHang.Add(chiTiet);
        gioHang.TongTien += chiTiet.GiaBan * chiTiet.SoLuong;

        // Nếu giỏ hàng đã tồn tại thì cập nhật, nếu không thì thêm mới
        if (_context.GioHang.Any(g => g.MaGioHang == gioHang.MaGioHang))
        {
            _context.GioHang.Update(gioHang);
        }
        else
        {
            _context.GioHangs.Add(gioHang);
        }

        _context.SaveChanges();

        return RedirectToAction("Index");  // Chuyển sang trang giỏ hàng
    }

    public IActionResult Index()
    {
        var gioHang = _context.GioHang.Include(g => g.ChiTietGioHang).FirstOrDefault(g => g.MaNguoiDung == 1);
        if (gioHang == null)
        {
            return NotFound(); // Trả về 404 nếu giỏ hàng không tìm thấy
        }
        return View(gioHang);
    }

}
