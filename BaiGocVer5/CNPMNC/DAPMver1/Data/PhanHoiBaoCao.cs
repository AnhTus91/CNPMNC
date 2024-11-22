using System;
using System.Collections.Generic;

namespace DAPMver1.Data;

public partial class PhanHoiBaoCao
{
    public int MaPhanHoi { get; set; }

    public int MaBaoCao { get; set; }

    public int MaNguoiDung { get; set; }

    public string NoiDung { get; set; }

    public DateTime? NgayPhanHoi { get; set; }

    public bool NguoiTraLoi { get; set; }

    public virtual BaoCaoNguoiDung MaBaoCaoNavigation { get; set; }

    public virtual NguoiDung MaNguoiDungNavigation { get; set; }
}
