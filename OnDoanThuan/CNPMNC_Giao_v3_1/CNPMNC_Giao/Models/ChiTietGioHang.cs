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
    
    public partial class ChiTietGioHang
    {
        public int MaGioHang { get; set; }
        public int MaSanPham { get; set; }
        public int SoLuong { get; set; }
        public double GiaBan { get; set; }
    
        public virtual GioHang GioHang { get; set; }
        public virtual SanPham SanPham { get; set; }

        public virtual ThanhToan ThanhToan { get; set; }
    }
}
