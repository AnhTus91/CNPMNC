using System;
using System.Collections.Generic;

namespace DAPMver1.Data;

public partial class Voucher
{
    public int MaVoucher { get; set; }

    public string TenVoucher { get; set; }

    public int GiaTri { get; set; }

    public DateTime ThoiGianBatDau { get; set; }

    public DateTime ThoiGianKetThuc { get; set; }

    public bool TrangThai { get; set; }

    public DateTime NgayTao { get; set; }

    public int? DieuKienApDung { get; set; }

    public int SoLuong { get; set; }

    public virtual ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();
}
