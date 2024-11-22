using System;
using System.Collections.Generic;

namespace DAPMver1.Data;

public partial class DanhMuc
{
    public int MaDanhMuc { get; set; }

    public string TenDanhMuc { get; set; }

    public string MoTa { get; set; }

    public virtual ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
}
