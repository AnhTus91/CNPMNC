using System;
using System.Collections.Generic;

namespace DAPMver1.Data;

public partial class HoaDon
{
    public string MaHoaDon { get; set; }

    public string MaDonHang { get; set; }

    public int MaNguoiDung { get; set; }

    public DateTime? NgayXuatHoaDon { get; set; }

    public double TongTien { get; set; }

    public string MaThanhToan { get; set; }

    public virtual DonHang MaDonHangNavigation { get; set; }

    public virtual NguoiDung MaNguoiDungNavigation { get; set; }

    public virtual ThanhToan MaThanhToanNavigation { get; set; }
}
