//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GameStore.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class BinhLuan
    {
        public int MaBinhLuan { get; set; }
        public int maSP { get; set; }
        public int MaNguoiDung { get; set; }
        public string NoiDung { get; set; }
        public System.DateTime NgayBinhLuan { get; set; }
    
        public virtual NguoiDung NguoiDung { get; set; }
        public virtual SanPham SanPham { get; set; }
    }
}
