using Microsoft.AspNetCore.Mvc.Rendering;

namespace DAPMver1.Models
{
    public class DonHangViewModel
    {
        public string MaDonHang { get; set; } // Mã đơn hàng
        public DateOnly NgayDatHang { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public string TrangThai { get; set; } = "Đang xử lý";
        public double PhiVanChuyen { get; set; }
        public string SdtnguoiNhan { get; set; }
        public string DiaChiNguoiNhan { get; set; }
        public string TenNguoiNhan { get; set; }
        public string HinhThucNhanHang { get; set; }
        public int? MaVoucher { get; set; }
        public int MaThue { get; set; }
        public double TongTien { get; set; }
        public double TienPhaiTra { get; set; }

        // Thêm lựa chọn sản phẩm
        public int MaSanPham { get; set; }
        public List<ChiTietDonHangViewModel> ChiTietDonHangs { get; set; } = new List<ChiTietDonHangViewModel>();

        public IEnumerable<SelectListItem> SanPhams { get; set; } // Danh sách sản phẩm
    }

    public class ChiTietDonHangViewModel
    {
        public int MaKichCo { get; set; }
        public int SoLuong { get; set; }
        public int DonGia { get; set; }
        public int TongTien => SoLuong * DonGia;

        public IEnumerable<SelectListItem> KichCoList { get; set; } // Danh sách kích cỡ
    }


}
