using System;
using System.Collections.Generic;

namespace DAPMver1.Data;

public partial class VatLieu
{
    public int MaVatLieu { get; set; }

    public string TenVatlieu { get; set; }

    public string MoTa { get; set; }

    public DateTime NgayTao { get; set; }

    public virtual ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
}
