﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CNPMNC_Giao.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class CNPMNCEntities1 : DbContext
    {
        public CNPMNCEntities1()
            : base("name=CNPMNCEntities1")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
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
        public virtual DbSet<KichCo> KichCoes { get; set; }
        public virtual DbSet<LichSuGiamGiaSanPham> LichSuGiamGiaSanPhams { get; set; }
        public virtual DbSet<LichSuGiaSanPham> LichSuGiaSanPhams { get; set; }
        public virtual DbSet<MauSac> MauSacs { get; set; }
        public virtual DbSet<NguoiDung> NguoiDungs { get; set; }
        public virtual DbSet<NhaCungCap> NhaCungCaps { get; set; }
        public virtual DbSet<SanPham> SanPhams { get; set; }
        public virtual DbSet<SanPhamYeuThich> SanPhamYeuThiches { get; set; }
        public virtual DbSet<TaiKhoan> TaiKhoans { get; set; }
        public virtual DbSet<ThanhToan> ThanhToans { get; set; }
        public virtual DbSet<VatLieu> VatLieux { get; set; }
        public virtual DbSet<VOUCHER> VOUCHERs { get; set; }
    }
}
