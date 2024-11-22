using DAPMver1.Data;
using Microsoft.EntityFrameworkCore;

namespace DAPMver1.Helpers
{
    public class DonHangServices
    {
        private readonly DapmTrangv1Context _context;

        public DonHangServices(DapmTrangv1Context context)
        {
            _context = context;
        }

        // Hàm lọc đơn hàng theo trạng thái
        public async Task<List<DonHang>> LocDonHangTheoTrangThaiAsync(string trangThai)
        {
            return await _context.DonHangs
                                 .Where(donHang => donHang.TrangThai == trangThai)
                                 .ToListAsync();
        }
    }
}
