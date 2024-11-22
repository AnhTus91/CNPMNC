using DAPMver1.Data;

namespace DAPMver1.Models
{
    public class ChiTietSanPhamViewModels
    {
        public SanPham SanPham { get; set; }
        public KichCo KichCo { get; set; }
        public IEnumerable<SanPham> GoiYSanPhams { get; set; }
        public List<SanPham> GoiYSanPhamsTheoGiaCaoNhat { get; internal set; }
        public double DiemTrungBinh { get; set; } // Thêm thuộc tính điểm trung bình
    }


}
