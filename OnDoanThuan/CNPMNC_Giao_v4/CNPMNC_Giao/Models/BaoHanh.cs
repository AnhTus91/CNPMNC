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
    
    public partial class BaoHanh
    {
        public int MaBaoHanh { get; set; }
        public int MaSanPham { get; set; }
        public int MaNguoiDung { get; set; }
        public Nullable<System.DateTime> NgayBaoHanh { get; set; }
        public System.DateTime NgayKetThuc { get; set; }
        public string MoTa { get; set; }
        public bool TrangThai { get; set; }
    
        public virtual NguoiDung NguoiDung { get; set; }
        public virtual SanPham SanPham { get; set; }
    }
}
