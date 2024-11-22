namespace DAPMver1.Models
{
    public class District
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string ParentCode { get; set; } // Mã tỉnh/thành phố
        public string Type { get; set; }
    }

}
