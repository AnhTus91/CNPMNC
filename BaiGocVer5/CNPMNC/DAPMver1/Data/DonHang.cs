using System;
using System.Collections.Generic;

namespace DAPMver1.Data;

public partial class DonHang
{
    public string MaDonHang { get; set; }

    public DateOnly NgayDatHang { get; set; }

    public string TrangThai { get; set; }

    public double PhiVanChuyen { get; set; }

    public double TongTien { get; set; }

    public int MaNguoiGui { get; set; }

    public string SdtnguoiNhan { get; set; }

    public string DiaChiNguoiNhan { get; set; }

    public string TenNguoiNhan { get; set; }

    public int TongSl { get; set; }

    public int TongSoTien { get; set; }

    public int TienPhaiTra { get; set; }

    public int? MaVoucher { get; set; }

    public int MaThue { get; set; }

    public string HinhThucNhanHang { get; set; }

    public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();

    public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();

    public virtual NguoiDung MaNguoiGuiNavigation { get; set; }

    public virtual Thue MaThueNavigation { get; set; }

    public virtual Voucher MaVoucherNavigation { get; set; }

    public virtual ICollection<ThanhToan> ThanhToans { get; set; } = new List<ThanhToan>();

  
}
