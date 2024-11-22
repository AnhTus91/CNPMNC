using System;
using System.Collections.Generic;

namespace DAPMver1.Data;

public partial class Thue
{
    public int MaThue { get; set; }

    public string TenThue { get; set; }

    public double GiaTri { get; set; }

    public string MoTa { get; set; }

    public bool? TrangThai { get; set; }

    public virtual ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();
}
