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
    
    public partial class LichSuGiaSanPham
    {
        public int MaLichSu { get; set; }
        public int MaSanPham { get; set; }
        public int GiaCu { get; set; }
        public int GiaMoi { get; set; }
        public Nullable<System.DateTime> NgayCapNhat { get; set; }
    
        public virtual SanPham SanPham { get; set; }
    }
}
