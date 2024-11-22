using System;
using System.Collections.Generic;

namespace DAPMver1.Data;

public partial class NhaCungCap
{
    public int MaNhaCungCap { get; set; }

    public string TenNhaCungCap { get; set; }

    public string Sdt { get; set; }

    public string DiaChi { get; set; }

    public string Email { get; set; }

    public virtual ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
}
