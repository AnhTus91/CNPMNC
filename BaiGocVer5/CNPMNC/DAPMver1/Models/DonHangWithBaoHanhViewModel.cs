using DAPMver1.Data;

namespace DAPMver1.Models
{
    public class DonHangWithBaoHanhViewModel
    {
        public DonHang DonHang { get; set; }
        public List<BaoHanh> BaoHanhList { get; set; }

        // Thuộc tính để phân biệt đơn hàng
        public string LoaiDonHang { get; set; } // Ví dụ: "Thường", "VIP", "Đặc biệt"
        public bool IsSpecial { get; set; } // Hoặc đánh dấu đơn hàng đặc biệt (true/false)
    }
}
