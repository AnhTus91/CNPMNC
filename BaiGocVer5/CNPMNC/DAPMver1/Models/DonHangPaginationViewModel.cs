using DAPMver1.Data;

namespace DAPMver1.Models
{
    public class DonHangPaginationViewModel
    {
        public List<DonHang> DonHangs { get; set; }  // Danh sách đơn hàng
        public int CurrentPage { get; set; }         // Trang hiện tại
        public int TotalPages { get; set; }
    }
}
