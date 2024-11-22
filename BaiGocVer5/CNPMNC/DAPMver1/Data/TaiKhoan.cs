using System;
using System.Collections.Generic;

namespace DAPMver1.Data;

public partial class TaiKhoan
{
    public int MaTaiKhoan { get; set; }

    public string Email { get; set; }

    public string MatKhau { get; set; }

    public bool VaiTro { get; set; }

    public virtual ICollection<NguoiDung> NguoiDungs { get; set; } = new List<NguoiDung>();
}
