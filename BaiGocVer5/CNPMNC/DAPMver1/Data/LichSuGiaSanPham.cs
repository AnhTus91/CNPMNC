using System;
using System.Collections.Generic;

namespace DAPMver1.Data;

public partial class LichSuGiaSanPham
{
    public int MaLichSu { get; set; }

    public int MaSanPham { get; set; }

    public int GiaCu { get; set; }

    public int GiaMoi { get; set; }

    public DateTime? NgayCapNhat { get; set; }

    public virtual SanPham MaSanPhamNavigation { get; set; }
}
