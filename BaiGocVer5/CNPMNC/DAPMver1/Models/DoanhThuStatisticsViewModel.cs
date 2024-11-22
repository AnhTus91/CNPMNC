namespace DAPMver1.Models
{
    public class DoanhThuStatisticsViewModel
    {
        public string MaDonHang { get; set; }
        public DateOnly NgayDatHang { get; set; }
        public double TongTien { get; set; } // Ensure this is double
        public int SoDonHang { get; set; }
    }
}
