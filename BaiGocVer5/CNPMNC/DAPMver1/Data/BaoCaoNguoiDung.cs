using System;
using System.Collections.Generic;

namespace DAPMver1.Data;

public partial class BaoCaoNguoiDung
{
    public int MaBaoCao { get; set; }

    public int MaNguoiDung { get; set; }

    public string PhanHoi { get; set; }

    public DateTime? NgayTao { get; set; }

    public int? TrangThai { get; set; }

    public virtual NguoiDung MaNguoiDungNavigation { get; set; }

    public virtual ICollection<PhanHoiBaoCao> PhanHoiBaoCaos { get; set; } = new List<PhanHoiBaoCao>();
}
