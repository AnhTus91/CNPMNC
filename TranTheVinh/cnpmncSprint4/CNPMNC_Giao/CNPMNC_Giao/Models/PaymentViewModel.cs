using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CNPMNC_Giao.Models
{
    public class PaymentViewModel
    {
        public List<ChiTietGioHang> ChiTietGioHangs { get; set; }
        public string TenNguoiNhan { get; set; }
        public string DiaChiNguoiNhan { get; set; }
        public string SDTNguoiNhan { get; set; }
        public List<string> PaymentMethods { get; set; }
    }
}