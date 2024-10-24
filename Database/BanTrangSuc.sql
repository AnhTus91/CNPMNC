create database DAPM
use DAPM

CREATE TABLE TaiKhoan
(
  MaTaiKhoan INT IDENTITY (1, 1)  NOT NULL,
  Email VARCHAR(50) NOT NULL,
  MatKhau VARCHAR(15) NOT NULL,
  VaiTro bit not null default 0,
  PRIMARY KEY (MaTaiKhoan)
);
CREATE TABLE ChucVu(
MaChucVu int identity(1,1) not null primary key,
TenChucVu Nvarchar(30)
);
CREATE TABLE NguoiDung
(
  MaNguoiDung INT IDENTITY (1, 1) NOT NULL,
  TenNguoiDung NVARCHAR(50) NOT NULL,
  DiaChi NVARCHAR(MAX) NOT NULL,
  SDT char(11) NOT NULL,
  AnhDaiDien VARCHAR(MAX) NOT NULL,
  MaChucVu int not null,
  MaTaiKhoan INT NOT NULL,
  PRIMARY KEY (MaNguoiDung),
  FOREIGN KEY (MaTaiKhoan) REFERENCES TaiKhoan(MaTaiKhoan),
   FOREIGN KEY (MaChucVu) REFERENCES ChucVu(MaChucVu)
);
CREATE TABLE DanhMuc
(
  MaDanhMuc INT IDENTITY (1, 1) NOT NULL,
  TenDanhMuc NVARCHAR(50) NOT NULL,
  MoTa NVARCHAR(MAX)
  PRIMARY KEY (MaDanhMuc)
);
CREATE TABLE NhaCungCap (
    MaNhaCungCap INT PRIMARY KEY IDENTITY(1,1),   
    TenNhaCungCap NVARCHAR(100) NOT NULL,        
    SDT Char(11) not null,                             
    DiaChi NVARCHAR(255),                         
	Email VARCHAR(50)
);

Create table VatLieu(
MaVatLieu INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
TenVatlieu NVARCHAR(100) not null,
MoTa nvarchar(255),
NgayTao DATETIME not null DEFAULT GETDATE() 
);

CREATE TABLE SanPham
(
  MaSanPham INT IDENTITY (1, 1) NOT NULL,
  TenSanPham NVARCHAR(50) NOT NULL,
  GiaTienMoi INT NOT NULL default 0,
  GiaTienCu INT NOT NULL default 0,
  MoTa NVARCHAR(max) NOT NULL,
  Size int not null default 1,
  Soluong int not null default 1,
  AnhSP VARCHAR(MAX) NOT NULL,
  MaVatLieu INT,
  MaDanhMuc INT NOT NULL,
  NgayTao date not null default GetDate(),
  MaNhaCungCap int,
  PRIMARY KEY (MaSanPham),
  FOREIGN KEY (MaVatLieu) REFERENCES VatLieu(MaVatLieu),
  FOREIGN KEY (MaDanhMuc) REFERENCES DanhMuc(MaDanhMuc),
  FOREIGN KEY (MaNhaCungCap) REFERENCES NhaCungCap(MaNhaCungCap)
);

CREATE TABLE VOUCHER(
MaVoucher INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
GiaTri int not null,
ThoiGianBatDau datetime not null default getdate(),
ThoiGianKetThuc datetime not null default getdate(),
TrangThai bit not null default 0,
NgayTao datetime not null default getdate(),
DieuKienApDung int default 0,
SoLuong int not null default 1
);

CREATE TABLE DonHang
(
  MaDonHang INT IDENTITY (1, 1) NOT NULL,
  NgayDatHang DATE NOT NULL,
  TrangThai NVARCHAR(50) NOT NULL,
  PhiVanChuyen FLOAT NOT NULL,
  TongTien FLOAT NOT NULL,
  MaNguoiGui int NOT NULL,
  SDTNguoiNhan char(11) NOT NULL,
  DiaChiNguoiNhan NVARCHAR(100) NOT NULL,
  TenNguoiNhan Nvarchar(30) not null,
  TongSL int not null default 0,
  TongSoTien int not null default 0,
  TienPhaiTra int not null default 0,
  HinhThucNhanHang NVARCHAR(50) DEFAULT 'Giao Hang',
  MaVoucher int,
  FOREIGN KEY (MaVoucher) REFERENCES Voucher(MaVoucher),
  FOREIGN KEY (MaNguoiGui) REFERENCES NguoiDung(MaNguoiDung),
  PRIMARY KEY (MaDonHang)
);

CREATE TABLE ChiTietDonHang
(
  MaDonHang INT   NOT NULL,
  MaSanPham int  NOT NULL,
  Soluong int not null default 0,
  DonGia int not null default 0,
  TongTien int not null default 0,
  PRIMARY KEY (MaDonHang,MaSanPham),
  FOREIGN KEY (MaDonHang) REFERENCES DonHang(MaDonHang),
  FOREIGN KEY (MaSanPham) REFERENCES SanPham(MaSanPham),

);

CREATE TABLE BinhLuan
(
  MaBinhLuan INT IDENTITY (1, 1) NOT NULL PRIMARY KEY,
  MaSanPham INT NOT NULL,
  MaNguoiDung INT NOT NULL,
  NoiDung NVARCHAR(MAX) NOT NULL,
  NgayBinhLuan DATETIME NOT NULL DEFAULT GETDATE(),
  FOREIGN KEY (MaSanPham) REFERENCES SanPham(MaSanPham),
  FOREIGN KEY (MaNguoiDung) REFERENCES NguoiDung(MaNguoiDung)
);
CREATE TABLE GioHang
(
    MaGioHang INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    MaNguoiDung INT NOT NULL,
    NgayTao DATETIME DEFAULT GETDATE(),
    TrangThai NVARCHAR(50) DEFAULT 'Chua Thanh Toan',
    TongTien FLOAT DEFAULT 0,
    FOREIGN KEY (MaNguoiDung) REFERENCES NguoiDung(MaNguoiDung)
);
CREATE TABLE ChiTietGioHang
(
    MaGioHang INT NOT NULL,
    MaSanPham INT NOT NULL,
    SoLuong INT NOT NULL,
    GiaBan FLOAT NOT NULL,
    PRIMARY KEY (MaGioHang, MaSanPham),
    FOREIGN KEY (MaGioHang) REFERENCES GioHang(MaGioHang),
    FOREIGN KEY (MaSanPham) REFERENCES SanPham(MaSanPham)
);
CREATE TABLE MauSac
(
	MaMauSac INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    MaSanPham INT,
    SoLuong int not null default 1,
	FOREIGN KEY (MaSanPham) REFERENCES SanPham(MaSanPham)
);
CREATE TABLE KichCo
(
	MaKichCo INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    MaSanPham INT,
    SoLuong int not null default 1,
	FOREIGN KEY (MaSanPham) REFERENCES SanPham(MaSanPham)
);
CREATE TABLE ThanhToan
(
    MaThanhToan INT IDENTITY(1,1) PRIMARY KEY, 
    MaDonHang INT NOT NULL,                      
    PhuongThucThanhToan NVARCHAR(50) NOT NULL,   
	NgayThanhToan DATETIME DEFAULT GETDATE(),    
    TongTien FLOAT NOT NULL,                     
    TrangThaiThanhToan bit not null default 0, 
    FOREIGN KEY (MaDonHang) REFERENCES DonHang(MaDonHang)     
);
CREATE TABLE HoaDon
(
    MaHoaDon INT IDENTITY(1,1) PRIMARY KEY,      
    MaDonHang INT NOT NULL,                      
    MaNguoiDung INT NOT NULL,                    
    NgayXuatHoaDon DATETIME DEFAULT GETDATE(),   
    TongTien FLOAT NOT NULL,                     
    MaThanhToan INT NOT NULL,                   
    FOREIGN KEY (MaDonHang) REFERENCES DonHang(MaDonHang),   
    FOREIGN KEY (MaNguoiDung) REFERENCES NguoiDung(MaNguoiDung), 
    FOREIGN KEY (MaThanhToan) REFERENCES ThanhToan(MaThanhToan)   
);

CREATE TABLE DanhGiaSanPham
(
    MaDanhGia INT IDENTITY(1, 1) PRIMARY KEY,  
    MaSanPham INT NOT NULL,                    
    MaNguoiDung INT NOT NULL,                  
    DiemDanhGia INT CHECK (DiemDanhGia >= 1 AND DiemDanhGia <= 5) NOT NULL,                     
    NgayDanhGia DATETIME DEFAULT GETDATE(),    
    FOREIGN KEY (MaSanPham) REFERENCES SanPham(MaSanPham), 
    FOREIGN KEY (MaNguoiDung) REFERENCES NguoiDung(MaNguoiDung)  
);
CREATE TABLE SanPhamYeuThich
(
    MaYeuThich INT IDENTITY(1, 1) PRIMARY KEY,  
    MaSanPham INT NOT NULL,                     
    MaNguoiDung INT NOT NULL,                   
    NgayThem DATETIME DEFAULT GETDATE(),        
    FOREIGN KEY (MaSanPham) REFERENCES SanPham(MaSanPham),  
    FOREIGN KEY (MaNguoiDung) REFERENCES NguoiDung(MaNguoiDung)  
);
CREATE TABLE TinNhan
(
    MaTinNhan INT IDENTITY(1, 1) PRIMARY KEY,   
    MaNguoiGui INT NOT NULL,                     
    MaNguoiNhan INT NOT NULL,                    
    NoiDung NVARCHAR(MAX) NOT NULL,             
    ThoiGian DATETIME DEFAULT GETDATE(),         
    DaDoc BIT DEFAULT 0,                         
    FOREIGN KEY (MaNguoiGui) REFERENCES NguoiDung(MaNguoiDung),  
    FOREIGN KEY (MaNguoiNhan) REFERENCES NguoiDung(MaNguoiDung)   
);
CREATE TABLE CuocTroChuyen
(
    MaCuocTroChuyen INT IDENTITY(1, 1) PRIMARY KEY, 
    MaKhachHang INT NOT NULL,                         
    MaNhanVien INT NOT NULL,                         
    ThoiGianBatDau DATETIME DEFAULT GETDATE(),       
    ThoiGianKetThuc DATETIME NULL,                   
    FOREIGN KEY (MaKhachHang) REFERENCES NguoiDung(MaNguoiDung),  
    FOREIGN KEY (MaNhanVien) REFERENCES NguoiDung(MaNguoiDung)    
);
CREATE TABLE LichSuTinNhan
(
    MaLichSu INT IDENTITY(1, 1) PRIMARY KEY,        
    MaTinNhan INT NOT NULL,                          
    MaCuocTroChuyen INT NOT NULL,                    
    FOREIGN KEY (MaTinNhan) REFERENCES TinNhan(MaTinNhan),          
    FOREIGN KEY (MaCuocTroChuyen) REFERENCES CuocTroChuyen(MaCuocTroChuyen) 
);
CREATE TABLE LichSuGiaSanPham
(
    MaLichSu INT IDENTITY(1, 1) PRIMARY KEY,  
    MaSanPham INT NOT NULL,                   
    GiaCu INT NOT NULL,                       
    GiaMoi INT NOT NULL,                      
    NgayCapNhat DATETIME DEFAULT GETDATE(),   
    FOREIGN KEY (MaSanPham) REFERENCES SanPham(MaSanPham) 
);
