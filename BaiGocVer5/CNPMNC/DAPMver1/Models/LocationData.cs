namespace DAPMver1.Models
{
    public class LocationData
    {
        public Dictionary<string, Province> Provinces { get; set; }
        public Dictionary<string, District> Districts { get; set; }
        public Dictionary<string, Ward> Wards { get; set; }
    }
}
