namespace DAPMver1.Models
{
    public class Ward
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string ParentCode { get; set; } // Mã quận/huyện
        public string Type { get; set; }
    }

}
