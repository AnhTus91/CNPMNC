using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CNPMNC_Giao.Models
{
    public class DoanhThuStatisticsViewModel
    {
        public string MaDonHang { get; set; }
        public DateTime NgayDatHang { get; set; }
        public double TongTien { get; set; } // Ensure this is double
    }
    public class DoanhThuTheoNgayViewModel
    {
        public DateTime Ngay { get; set; }
        public double TongDoanhThu { get; set; }
        public int SoDonHang { get; set; }
    }


}