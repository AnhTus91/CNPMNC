using System;
using System.Collections.Generic;

namespace DAPMver1.Data;

public partial class ThanhToan
{
    public string MaThanhToan { get; set; }

    public string MaDonHang { get; set; }

    public string PhuongThucThanhToan { get; set; }

    public DateTime? NgayThanhToan { get; set; }

    public double TongTien { get; set; }

    public bool TrangThaiThanhToan { get; set; }

    public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();

    public virtual DonHang MaDonHangNavigation { get; set; }
}
