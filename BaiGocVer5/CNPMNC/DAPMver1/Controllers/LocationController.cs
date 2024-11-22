using DAPMver1.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DAPMver1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : Controller
    {
        private readonly LocationData _locationData;
        public LocationController()
        {
            var jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "locations.json");
            var jsonData = System.IO.File.ReadAllText(jsonFilePath);
            _locationData = JsonConvert.DeserializeObject<LocationData>(jsonData);
        }
        [HttpGet("GetProvinces")]
        public IActionResult GetProvinces()
        {
            return Ok(_locationData.Provinces.Values.Select(p => new { p.Code, p.Name }));
        }

        [HttpGet("GetDistricts/{provinceCode}")]
        public IActionResult GetDistricts(string provinceCode)
        {
            var districts = _locationData.Districts.Values
                .Where(d => d.ParentCode == provinceCode)
                .Select(d => new { d.Code, d.Name });
            return Ok(districts);
        }

        [HttpGet("GetWards/{districtCode}")]
        public IActionResult GetWards(string districtCode)
        {
            var wards = _locationData.Wards.Values
                .Where(w => w.ParentCode == districtCode)
                .Select(w => new { w.Code, w.Name });
            return Ok(wards);
        }
    }
}
