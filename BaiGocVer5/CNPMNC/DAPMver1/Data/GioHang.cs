using System;
using System.Collections.Generic;

namespace DAPMver1.Data;

public partial class GioHang
{
    public int MaGioHang { get; set; }

    public int MaNguoiDung { get; set; }

    public DateTime? NgayTao { get; set; }

    public string TrangThai { get; set; }

    public double? TongTien { get; set; }

    public virtual ICollection<ChiTietGioHang> ChiTietGioHangs { get; set; } = new List<ChiTietGioHang>();

    public virtual NguoiDung MaNguoiDungNavigation { get; set; }
}
