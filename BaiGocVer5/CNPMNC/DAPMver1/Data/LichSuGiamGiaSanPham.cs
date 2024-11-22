using System;
using System.Collections.Generic;

namespace DAPMver1.Data;

public partial class LichSuGiamGiaSanPham
{
    public int MaLichSuGiamGia { get; set; }

    public int MaSanPham { get; set; }

    public int GiaGiamCu { get; set; }

    public int GiaGiamMoi { get; set; }

    public DateTime? NgayCapNhat { get; set; }

    public virtual SanPham MaSanPhamNavigation { get; set; }
}
