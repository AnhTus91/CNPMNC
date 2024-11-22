using System;
using System.Collections.Generic;

namespace DAPMver1.Data;

public partial class NguoiDung
{
    public int MaNguoiDung { get; set; }

    public string TenNguoiDung { get; set; }

    public string DiaChi { get; set; }

    public string Sdt { get; set; }

    public string AnhDaiDien { get; set; }

    public int MaChucVu { get; set; }

    public int MaTaiKhoan { get; set; }

    public virtual ICollection<BaoCaoNguoiDung> BaoCaoNguoiDungs { get; set; } = new List<BaoCaoNguoiDung>();

    public virtual ICollection<BaoHanh> BaoHanhs { get; set; } = new List<BaoHanh>();

    public virtual ICollection<BinhLuan> BinhLuans { get; set; } = new List<BinhLuan>();

    public virtual ICollection<DanhGiaSanPham> DanhGiaSanPhams { get; set; } = new List<DanhGiaSanPham>();

    public virtual ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();

    public virtual ICollection<GioHang> GioHangs { get; set; } = new List<GioHang>();

    public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();

    public virtual ChucVu MaChucVuNavigation { get; set; }

    public virtual TaiKhoan MaTaiKhoanNavigation { get; set; }

    public virtual ICollection<PhanHoiBaoCao> PhanHoiBaoCaos { get; set; } = new List<PhanHoiBaoCao>();

    public virtual ICollection<SanPhamYeuThich> SanPhamYeuThiches { get; set; } = new List<SanPhamYeuThich>();
}
