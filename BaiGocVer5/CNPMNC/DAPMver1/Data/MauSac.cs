using System;
using System.Collections.Generic;

namespace DAPMver1.Data;

public partial class MauSac
{
    public int MaMauSac { get; set; }

    public int? MaSanPham { get; set; }

    public string TenMau { get; set; }

    public int SoLuong { get; set; }

    public virtual SanPham MaSanPhamNavigation { get; set; }
}
