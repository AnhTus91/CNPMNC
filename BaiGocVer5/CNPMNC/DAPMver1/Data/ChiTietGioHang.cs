using System;
using System.Collections.Generic;

namespace DAPMver1.Data;

public partial class ChiTietGioHang
{
    public int MaGioHang { get; set; }

    public int MaKichCo { get; set; }

    public int SoLuong { get; set; }

    public double GiaBan { get; set; }

    public virtual GioHang MaGioHangNavigation { get; set; }

    public virtual KichCo MaKichCoNavigation { get; set; }
}
