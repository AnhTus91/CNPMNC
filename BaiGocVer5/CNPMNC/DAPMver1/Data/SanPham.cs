using System;
using System.Collections.Generic;

namespace DAPMver1.Data;

public partial class SanPham
{
    public int MaSanPham { get; set; }

    public string TenSanPham { get; set; }

    public int GiaTienMoi { get; set; }

    public int GiaTienCu { get; set; }

    public string MoTa { get; set; }

    public string AnhSp { get; set; }

    public int MaVatLieu { get; set; }

    public int MaDanhMuc { get; set; }

    public DateOnly NgayTao { get; set; }

    public int MaNhaCungCap { get; set; }

    public virtual ICollection<BinhLuan> BinhLuans { get; set; } = new List<BinhLuan>();

    public virtual ICollection<DanhGiaSanPham> DanhGiaSanPhams { get; set; } = new List<DanhGiaSanPham>();

    public virtual ICollection<KichCo> KichCos { get; set; } = new List<KichCo>();

    public virtual ICollection<LichSuGiaSanPham> LichSuGiaSanPhams { get; set; } = new List<LichSuGiaSanPham>();

    public virtual ICollection<LichSuGiamGiaSanPham> LichSuGiamGiaSanPhams { get; set; } = new List<LichSuGiamGiaSanPham>();

    public virtual DanhMuc MaDanhMucNavigation { get; set; }

    public virtual NhaCungCap MaNhaCungCapNavigation { get; set; }

    public virtual VatLieu MaVatLieuNavigation { get; set; }

    public virtual ICollection<MauSac> MauSacs { get; set; } = new List<MauSac>();

    public virtual ICollection<SanPhamYeuThich> SanPhamYeuThiches { get; set; } = new List<SanPhamYeuThich>();
}
