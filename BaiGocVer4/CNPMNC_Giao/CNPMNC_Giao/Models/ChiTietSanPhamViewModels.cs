using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CNPMNC_Giao.Models
{
    public class ChiTietSanPhamViewModels
    {
        public SanPham SanPham { get; set; }
        public KichCo KichCo { get; set; }
        public IEnumerable<SanPham> GoiYSanPhams { get; set; }
        public List<SanPham> GoiYSanPhamsTheoGiaCaoNhat { get; internal set; }
        public double DiemTrungBinh { get; set; } 
    }
}