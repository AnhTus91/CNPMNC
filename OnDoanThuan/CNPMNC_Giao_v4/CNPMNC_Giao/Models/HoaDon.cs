//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class HoaDon
    {
        public string MaHoaDon { get; set; }
        public string MaDonHang { get; set; }
        public int MaNguoiDung { get; set; }
        public Nullable<System.DateTime> NgayXuatHoaDon { get; set; }
        public double TongTien { get; set; }
        public int MaThanhToan { get; set; }
    
        public virtual DonHang DonHang { get; set; }
        public virtual NguoiDung NguoiDung { get; set; }
        public virtual ThanhToan ThanhToan { get; set; }
    }
}
