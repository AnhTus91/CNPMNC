using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DAPMver1.Data;

public partial class DapmTrangv1Context : DbContext
{
    public DapmTrangv1Context()
    {
    }

    public DapmTrangv1Context(DbContextOptions<DapmTrangv1Context> options)
        : base(options)
    {
    }

    public virtual DbSet<BaoCaoNguoiDung> BaoCaoNguoiDungs { get; set; }

    public virtual DbSet<BaoHanh> BaoHanhs { get; set; }

    public virtual DbSet<BinhLuan> BinhLuans { get; set; }

    public virtual DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; }

    public virtual DbSet<ChiTietGioHang> ChiTietGioHangs { get; set; }

    public virtual DbSet<ChucVu> ChucVus { get; set; }

    public virtual DbSet<DanhGiaSanPham> DanhGiaSanPhams { get; set; }

    public virtual DbSet<DanhMuc> DanhMucs { get; set; }

    public virtual DbSet<DonHang> DonHangs { get; set; }

    public virtual DbSet<GioHang> GioHangs { get; set; }

    public virtual DbSet<HoaDon> HoaDons { get; set; }

    public virtual DbSet<KichCo> KichCos { get; set; }

    public virtual DbSet<LichSuGiaSanPham> LichSuGiaSanPhams { get; set; }

    public virtual DbSet<LichSuGiamGiaSanPham> LichSuGiamGiaSanPhams { get; set; }

    public virtual DbSet<MauSac> MauSacs { get; set; }

    public virtual DbSet<NguoiDung> NguoiDungs { get; set; }

    public virtual DbSet<NhaCungCap> NhaCungCaps { get; set; }

    public virtual DbSet<PhanHoiBaoCao> PhanHoiBaoCaos { get; set; }

    public virtual DbSet<SanPham> SanPhams { get; set; }

    public virtual DbSet<SanPhamYeuThich> SanPhamYeuThiches { get; set; }

    public virtual DbSet<TaiKhoan> TaiKhoans { get; set; }

    public virtual DbSet<ThanhToan> ThanhToans { get; set; }

    public virtual DbSet<Thue> Thues { get; set; }

    public virtual DbSet<VatLieu> VatLieus { get; set; }

    public virtual DbSet<Voucher> Vouchers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=laptop-ut0ddbvf;Initial Catalog=DAPM_Trangv1;Integrated Security=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BaoCaoNguoiDung>(entity =>
        {
            entity.HasKey(e => e.MaBaoCao).HasName("PK__BaoCaoNg__25A9188C5A4BC6FF");

            entity.ToTable("BaoCaoNguoiDung");

            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TrangThai).HasDefaultValue(0);

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.BaoCaoNguoiDungs)
                .HasForeignKey(d => d.MaNguoiDung)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BaoCaoNgu__MaNgu__367C1819");
        });

        modelBuilder.Entity<BaoHanh>(entity =>
        {
            entity.HasKey(e => e.MaBaoHanh).HasName("PK__BaoHanh__A8DF52E50498B35B");

            entity.ToTable("BaoHanh");

            entity.Property(e => e.NgayBaoHanh)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NgayKetThuc).HasColumnType("datetime");

            entity.HasOne(d => d.MaKichCoNavigation).WithMany(p => p.BaoHanhs)
                .HasForeignKey(d => d.MaKichCo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BaoHanh__MaKichC__71D1E811");

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.BaoHanhs)
                .HasForeignKey(d => d.MaNguoiDung)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BaoHanh__MaNguoi__72C60C4A");
        });

        modelBuilder.Entity<BinhLuan>(entity =>
        {
            entity.HasKey(e => e.MaBinhLuan).HasName("PK__BinhLuan__87CB66A0AD40BC8B");

            entity.ToTable("BinhLuan");

            entity.Property(e => e.NgayBinhLuan)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NoiDung).IsRequired();

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.BinhLuans)
                .HasForeignKey(d => d.MaNguoiDung)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BinhLuan__MaNguo__07C12930");

            entity.HasOne(d => d.MaSanPhamNavigation).WithMany(p => p.BinhLuans)
                .HasForeignKey(d => d.MaSanPham)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BinhLuan__MaSanP__06CD04F7");
        });

        modelBuilder.Entity<ChiTietDonHang>(entity =>
        {
            entity.HasKey(e => e.MaChiTietDonHang).HasName("PK__ChiTietD__4B0B45DD348A9DBA");

            entity.ToTable("ChiTietDonHang");

            entity.Property(e => e.MaDonHang)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.MaDonHangNavigation).WithMany(p => p.ChiTietDonHangs)
                .HasForeignKey(d => d.MaDonHang)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTietDo__MaDon__02084FDA");

            entity.HasOne(d => d.MaKichCoNavigation).WithMany(p => p.ChiTietDonHangs)
                .HasForeignKey(d => d.MaKichCo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTietDo__MaKic__02FC7413");
        });

        modelBuilder.Entity<ChiTietGioHang>(entity =>
        {
            entity.HasKey(e => new { e.MaGioHang, e.MaKichCo }).HasName("PK__ChiTietG__98E7737BE69E4B84");

            entity.ToTable("ChiTietGioHang");

            entity.HasOne(d => d.MaGioHangNavigation).WithMany(p => p.ChiTietGioHangs)
                .HasForeignKey(d => d.MaGioHang)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTietGi__MaGio__10566F31");

            entity.HasOne(d => d.MaKichCoNavigation).WithMany(p => p.ChiTietGioHangs)
                .HasForeignKey(d => d.MaKichCo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTietGi__MaKic__114A936A");
        });

        modelBuilder.Entity<ChucVu>(entity =>
        {
            entity.HasKey(e => e.MaChucVu).HasName("PK__ChucVu__D463953387B34328");

            entity.ToTable("ChucVu");

            entity.Property(e => e.TenChucVu).HasMaxLength(30);
        });

        modelBuilder.Entity<DanhGiaSanPham>(entity =>
        {
            entity.HasKey(e => e.MaDanhGia).HasName("PK__DanhGiaS__AA9515BF8DC3F7BC");

            entity.ToTable("DanhGiaSanPham");

            entity.Property(e => e.NgayDanhGia)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.DanhGiaSanPhams)
                .HasForeignKey(d => d.MaNguoiDung)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DanhGiaSa__MaNgu__25518C17");

            entity.HasOne(d => d.MaSanPhamNavigation).WithMany(p => p.DanhGiaSanPhams)
                .HasForeignKey(d => d.MaSanPham)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DanhGiaSa__MaSan__245D67DE");
        });

        modelBuilder.Entity<DanhMuc>(entity =>
        {
            entity.HasKey(e => e.MaDanhMuc).HasName("PK__DanhMuc__B3750887C7A4D146");

            entity.ToTable("DanhMuc");

            entity.Property(e => e.TenDanhMuc)
                .IsRequired()
                .HasMaxLength(50);
        });

        modelBuilder.Entity<DonHang>(entity =>
        {
            entity.HasKey(e => e.MaDonHang).HasName("PK__DonHang__129584ADA1BE8712");

            entity.ToTable("DonHang");

            entity.Property(e => e.MaDonHang)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.DiaChiNguoiNhan)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.HinhThucNhanHang)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("Giao Hang");
            entity.Property(e => e.SdtnguoiNhan)
                .IsRequired()
                .HasMaxLength(11)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("SDTNguoiNhan");
            entity.Property(e => e.TenNguoiNhan)
                .IsRequired()
                .HasMaxLength(30);
            entity.Property(e => e.TongSl).HasColumnName("TongSL");
            entity.Property(e => e.TrangThai)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasOne(d => d.MaNguoiGuiNavigation).WithMany(p => p.DonHangs)
                .HasForeignKey(d => d.MaNguoiGui)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DonHang__MaNguoi__7B5B524B");

            entity.HasOne(d => d.MaThueNavigation).WithMany(p => p.DonHangs)
                .HasForeignKey(d => d.MaThue)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DonHang__MaThue__7C4F7684");

            entity.HasOne(d => d.MaVoucherNavigation).WithMany(p => p.DonHangs)
                .HasForeignKey(d => d.MaVoucher)
                .HasConstraintName("FK__DonHang__MaVouch__7A672E12");
        });

        modelBuilder.Entity<GioHang>(entity =>
        {
            entity.HasKey(e => e.MaGioHang).HasName("PK__GioHang__F5001DA3356C8B26");

            entity.ToTable("GioHang");

            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TongTien).HasDefaultValue(0.0);
            entity.Property(e => e.TrangThai)
                .HasMaxLength(50)
                .HasDefaultValue("Chua Thanh Toan");

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.GioHangs)
                .HasForeignKey(d => d.MaNguoiDung)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GioHang__MaNguoi__0D7A0286");
        });

        modelBuilder.Entity<HoaDon>(entity =>
        {
            entity.HasKey(e => e.MaHoaDon).HasName("PK__HoaDon__835ED13BC6375CFA");

            entity.ToTable("HoaDon");

            entity.Property(e => e.MaHoaDon)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MaDonHang)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MaThanhToan)
                .IsRequired()
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.NgayXuatHoaDon)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.MaDonHangNavigation).WithMany(p => p.HoaDons)
                .HasForeignKey(d => d.MaDonHang)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HoaDon__MaDonHan__1DB06A4F");

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.HoaDons)
                .HasForeignKey(d => d.MaNguoiDung)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HoaDon__MaNguoiD__1EA48E88");

            entity.HasOne(d => d.MaThanhToanNavigation).WithMany(p => p.HoaDons)
                .HasForeignKey(d => d.MaThanhToan)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HoaDon__MaThanhT__1F98B2C1");
        });

        modelBuilder.Entity<KichCo>(entity =>
        {
            entity.HasKey(e => e.MaKichCo).HasName("PK__KichCo__DE76ED875A54DAFC");

            entity.ToTable("KichCo");

            entity.Property(e => e.SoLuong).HasDefaultValue(1);

            entity.HasOne(d => d.MaSanPhamNavigation).WithMany(p => p.KichCos)
                .HasForeignKey(d => d.MaSanPham)
                .HasConstraintName("FK__KichCo__MaSanPha__6C190EBB");
        });

        modelBuilder.Entity<LichSuGiaSanPham>(entity =>
        {
            entity.HasKey(e => e.MaLichSu).HasName("PK__LichSuGi__C443222AEB224AB9");

            entity.ToTable("LichSuGiaSanPham");

            entity.Property(e => e.NgayCapNhat)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.MaSanPhamNavigation).WithMany(p => p.LichSuGiaSanPhams)
                .HasForeignKey(d => d.MaSanPham)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LichSuGia__MaSan__2DE6D218");
        });

        modelBuilder.Entity<LichSuGiamGiaSanPham>(entity =>
        {
            entity.HasKey(e => e.MaLichSuGiamGia).HasName("PK__LichSuGi__35BD950F2512FE8C");

            entity.ToTable("LichSuGiamGiaSanPham");

            entity.Property(e => e.NgayCapNhat)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.MaSanPhamNavigation).WithMany(p => p.LichSuGiamGiaSanPhams)
                .HasForeignKey(d => d.MaSanPham)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LichSuGia__MaSan__31B762FC");
        });

        modelBuilder.Entity<MauSac>(entity =>
        {
            entity.HasKey(e => e.MaMauSac).HasName("PK__MauSac__B9A911620A346D7B");

            entity.ToTable("MauSac");

            entity.Property(e => e.SoLuong).HasDefaultValue(1);
            entity.Property(e => e.TenMau)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasOne(d => d.MaSanPhamNavigation).WithMany(p => p.MauSacs)
                .HasForeignKey(d => d.MaSanPham)
                .HasConstraintName("FK__MauSac__MaSanPha__151B244E");
        });

        modelBuilder.Entity<NguoiDung>(entity =>
        {
            entity.HasKey(e => e.MaNguoiDung).HasName("PK__NguoiDun__C539D762404110A1");

            entity.ToTable("NguoiDung");

            entity.Property(e => e.AnhDaiDien)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.DiaChi).IsRequired();
            entity.Property(e => e.Sdt)
                .IsRequired()
                .HasMaxLength(11)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("SDT");
            entity.Property(e => e.TenNguoiDung)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasOne(d => d.MaChucVuNavigation).WithMany(p => p.NguoiDungs)
                .HasForeignKey(d => d.MaChucVu)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__NguoiDung__MaChu__4F7CD00D");

            entity.HasOne(d => d.MaTaiKhoanNavigation).WithMany(p => p.NguoiDungs)
                .HasForeignKey(d => d.MaTaiKhoan)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__NguoiDung__MaTai__4E88ABD4");
        });

        modelBuilder.Entity<NhaCungCap>(entity =>
        {
            entity.HasKey(e => e.MaNhaCungCap).HasName("PK__NhaCungC__53DA92053F48B804");

            entity.ToTable("NhaCungCap");

            entity.Property(e => e.DiaChi).HasMaxLength(255);
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Sdt)
                .IsRequired()
                .HasMaxLength(11)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("SDT");
            entity.Property(e => e.TenNhaCungCap)
                .IsRequired()
                .HasMaxLength(100);
        });

        modelBuilder.Entity<PhanHoiBaoCao>(entity =>
        {
            entity.HasKey(e => e.MaPhanHoi).HasName("PK__PhanHoiB__3458D20F753A7994");

            entity.ToTable("PhanHoiBaoCao");

            entity.Property(e => e.NgayPhanHoi)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NoiDung).IsRequired();

            entity.HasOne(d => d.MaBaoCaoNavigation).WithMany(p => p.PhanHoiBaoCaos)
                .HasForeignKey(d => d.MaBaoCao)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PhanHoiBa__MaBao__3A4CA8FD");

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.PhanHoiBaoCaos)
                .HasForeignKey(d => d.MaNguoiDung)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PhanHoiBa__MaNgu__3B40CD36");
        });

        modelBuilder.Entity<SanPham>(entity =>
        {
            entity.HasKey(e => e.MaSanPham).HasName("PK__SanPham__FAC7442DCCF2DC96");

            entity.ToTable("SanPham");

            entity.Property(e => e.AnhSp)
                .IsRequired()
                .IsUnicode(false)
                .HasColumnName("AnhSP");
            entity.Property(e => e.MoTa).IsRequired();
            entity.Property(e => e.NgayTao).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.TenSanPham)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasOne(d => d.MaDanhMucNavigation).WithMany(p => p.SanPhams)
                .HasForeignKey(d => d.MaDanhMuc)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SanPham__MaDanhM__6754599E");

            entity.HasOne(d => d.MaNhaCungCapNavigation).WithMany(p => p.SanPhams)
                .HasForeignKey(d => d.MaNhaCungCap)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SanPham__MaNhaCu__68487DD7");

            entity.HasOne(d => d.MaVatLieuNavigation).WithMany(p => p.SanPhams)
                .HasForeignKey(d => d.MaVatLieu)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SanPham__MaVatLi__66603565");
        });

        modelBuilder.Entity<SanPhamYeuThich>(entity =>
        {
            entity.HasKey(e => e.MaYeuThich).HasName("PK__SanPhamY__B9007E4CC2315D8C");

            entity.ToTable("SanPhamYeuThich");

            entity.Property(e => e.NgayThem)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.SanPhamYeuThiches)
                .HasForeignKey(d => d.MaNguoiDung)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SanPhamYe__MaNgu__2A164134");

            entity.HasOne(d => d.MaSanPhamNavigation).WithMany(p => p.SanPhamYeuThiches)
                .HasForeignKey(d => d.MaSanPham)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SanPhamYe__MaSan__29221CFB");
        });

        modelBuilder.Entity<TaiKhoan>(entity =>
        {
            entity.HasKey(e => e.MaTaiKhoan).HasName("PK__TaiKhoan__AD7C6529DB1421B0");

            entity.ToTable("TaiKhoan");

            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MatKhau)
                .IsRequired()
                .HasMaxLength(15)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ThanhToan>(entity =>
        {
            entity.HasKey(e => e.MaThanhToan).HasName("PK__ThanhToa__D4B25844553B25E4");

            entity.ToTable("ThanhToan");

            entity.Property(e => e.MaThanhToan)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.MaDonHang)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.NgayThanhToan)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PhuongThucThanhToan)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasOne(d => d.MaDonHangNavigation).WithMany(p => p.ThanhToans)
                .HasForeignKey(d => d.MaDonHang)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ThanhToan__MaDon__19DFD96B");
        });

        modelBuilder.Entity<Thue>(entity =>
        {
            entity.HasKey(e => e.MaThue).HasName("PK__Thue__9CC2FDA37A52E13C");

            entity.ToTable("Thue");

            entity.Property(e => e.TenThue)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.TrangThai).HasDefaultValue(true);
        });

        modelBuilder.Entity<VatLieu>(entity =>
        {
            entity.HasKey(e => e.MaVatLieu).HasName("PK__VatLieu__0A1700ACD82B0667");

            entity.ToTable("VatLieu");

            entity.Property(e => e.MoTa).HasMaxLength(255);
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TenVatlieu)
                .IsRequired()
                .HasMaxLength(100);
        });

        modelBuilder.Entity<Voucher>(entity =>
        {
            entity.HasKey(e => e.MaVoucher).HasName("PK__VOUCHER__0AAC5B11A8EC9037");

            entity.ToTable("VOUCHER");

            entity.Property(e => e.DieuKienApDung).HasDefaultValue(0);
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.SoLuong).HasDefaultValue(1);
            entity.Property(e => e.TenVoucher).IsRequired();
            entity.Property(e => e.ThoiGianBatDau)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ThoiGianKetThuc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
