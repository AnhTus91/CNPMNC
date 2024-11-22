namespace DAPMver1.Models
{
    public class ChiTietGioHangViewModel
    {
        public int MaSanPham { get; set; }
        public string TenSanPham { get; set; }
        public string AnhSp { get; set; }
        public double GiaBan { get; set; }
        public int SoLuong { get; set; }
        public int MaKichCo { get; set; } // Thêm thuộc tính MaKichCo
        public int SoKichCo { get; set; } // Lưu kích cỡ (từ KichCo)
        public double TongTien => GiaBan * SoLuong; // Tính tổng tiền
    }

}
