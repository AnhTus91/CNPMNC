using System;
using System.Collections.Generic;

namespace DAPMver1.Data;

public partial class BaoHanh
{
    public int MaBaoHanh { get; set; }

    public int MaKichCo { get; set; }

    public int MaNguoiDung { get; set; }

    public DateTime? NgayBaoHanh { get; set; }

    public DateTime NgayKetThuc { get; set; }

    public string MoTa { get; set; }

    public bool TrangThai { get; set; }

    public virtual KichCo MaKichCoNavigation { get; set; }

    public virtual NguoiDung MaNguoiDungNavigation { get; set; }
}
