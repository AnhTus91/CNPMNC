using DAPMver1.Data;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Document = iTextSharp.text.Document;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;


namespace DAPMver1.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HoaDonController : Controller
    {
        private readonly DapmTrangv1Context db;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public HoaDonController(DapmTrangv1Context db, IWebHostEnvironment webHostEnvironment)
        {
            this.db = db;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var hoaDon = db.HoaDons.Include(n => n.MaDonHangNavigation).Include(n => n.MaNguoiDungNavigation).ToList();

            return View(hoaDon);
        }
        public IActionResult Export(string id, string maDonHang)
        {
            var invoice = db.HoaDons.Include(n => n.MaDonHangNavigation).Include(n => n.MaNguoiDungNavigation).Include(n => n.MaThanhToanNavigation)
                .FirstOrDefault(i => i.MaHoaDon == id);

            var orderItems = db.ChiTietDonHangs
     .Include(i => i.MaKichCoNavigation.MaSanPhamNavigation)
     .Where(i => i.MaDonHang == maDonHang)
     .ToList();


            if (invoice == null)
            {
                return NotFound();
            }

            using (var memoryStream = new MemoryStream())
            {
                Document document = new Document();
                PdfWriter.GetInstance(document, memoryStream);

                string fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arial.ttf");
                var baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                var font = new Font(baseFont, 12, Font.NORMAL);

                document.Open();
                document.Add(new Paragraph($"Mã Hóa Đơn: {invoice.MaHoaDon}", font));
                document.Add(new Paragraph($"Mã đơn hàng: {invoice.MaDonHangNavigation.MaDonHang}", font));
                if (invoice.MaThanhToanNavigation.PhuongThucThanhToan == "Trực tuyến")
                {
                    document.Add(new Paragraph($"Mã token trực tuyến: {invoice.MaThanhToanNavigation.MaThanhToan}", font));
                }
                else
                {
                    document.Add(new Paragraph($"Mã token trực tuyến: không có.", font));

                }
                document.Add(new Paragraph($"Ngày Xuất: {invoice.NgayXuatHoaDon?.ToString("dd/MM/yyyy")}", font));
                if(invoice.MaThanhToanNavigation.TrangThaiThanhToan == true)
                {
                    document.Add(new Paragraph($"Trạng thái: Đã thanh toán", font));

                }
                else
                {
                    document.Add(new Paragraph($"Trạng thái: Chưa thanh toán", font));

                }
                document.Add(new Paragraph($"Phương thức thanh toán: {invoice.MaThanhToanNavigation.PhuongThucThanhToan}", font));
                document.Add(new Paragraph($"Ngày thực hiện thanh toán: {invoice.MaThanhToanNavigation.NgayThanhToan}", font));
                document.Add(new Paragraph($"Người Đặt đơn: {invoice.MaNguoiDungNavigation.TenNguoiDung}", font));
                document.Add(new Paragraph($"Phí Vận Chuyển: {invoice.MaDonHangNavigation.PhiVanChuyen}", font));
                document.Add(new Paragraph("    "));
                if (orderItems.Any())
                {
                    PdfPTable table = new PdfPTable(5)
                    {
                        WidthPercentage = 100
                    };

                    float[] widths = new float[] { 3f, 3f, 1f, 1f, 1f };
                    table.SetWidths(widths);

                    table.AddCell(new PdfPCell(new Phrase("Ảnh", font)) { HorizontalAlignment = Element.ALIGN_CENTER, BackgroundColor = BaseColor.LIGHT_GRAY });
                    table.AddCell(new PdfPCell(new Phrase("Danh mục", font)) { HorizontalAlignment = Element.ALIGN_CENTER, BackgroundColor = BaseColor.LIGHT_GRAY });
                    table.AddCell(new PdfPCell(new Phrase("Số lượng", font)) { HorizontalAlignment = Element.ALIGN_CENTER, BackgroundColor = BaseColor.LIGHT_GRAY });
                    table.AddCell(new PdfPCell(new Phrase("Đơn giá", font)) { HorizontalAlignment = Element.ALIGN_CENTER, BackgroundColor = BaseColor.LIGHT_GRAY });
                    table.AddCell(new PdfPCell(new Phrase("Thành tiền", font)) { HorizontalAlignment = Element.ALIGN_CENTER, BackgroundColor = BaseColor.LIGHT_GRAY });

                    foreach (var item in orderItems)
                    {
                        var imageCell = new PdfPCell();
                        if (!string.IsNullOrEmpty(item.MaKichCoNavigation.MaSanPhamNavigation.AnhSp))
                        {
                            // Xây dựng đường dẫn tuyệt đối tới ảnh bằng cách loại bỏ ký tự "/" đầu tiên nếu có
                            string relativeImagePath = item.MaKichCoNavigation.MaSanPhamNavigation.AnhSp.TrimStart('/');
                            string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, relativeImagePath);

                            if (System.IO.File.Exists(imagePath)) // Kiểm tra xem file có tồn tại không
                            {
                                // Tải ảnh từ đường dẫn tuyệt đối
                                Image productImage = Image.GetInstance(imagePath);
                                productImage.ScaleAbsolute(50, 50); // Đặt kích thước ảnh
                                imageCell.AddElement(productImage);
                            }
                            else
                            {
                                // Thêm ô trống nếu ảnh không tồn tại
                                imageCell.AddElement(new Phrase("Không tìm thấy ảnh"));
                            }
                        }
                        else
                        {
                            // Thêm ô trống nếu không có đường dẫn ảnh
                            imageCell.AddElement(new Phrase("Không có ảnh"));
                        }

                        table.AddCell(imageCell);

                        table.AddCell(new PdfPCell(new Phrase(item.MaKichCoNavigation.MaSanPhamNavigation.TenSanPham, font)));
                        table.AddCell(new PdfPCell(new Phrase(item.Soluong.ToString(), font)) { HorizontalAlignment = Element.ALIGN_CENTER });
                        table.AddCell(new PdfPCell(new Phrase(item.DonGia.ToString("N0") + "đ", font)) { HorizontalAlignment = Element.ALIGN_RIGHT });

                        var thanhTien = item.Soluong * item.DonGia;
                        table.AddCell(new PdfPCell(new Phrase(thanhTien.ToString("N0") + "đ", font)) { HorizontalAlignment = Element.ALIGN_RIGHT });
                    }

                    document.Add(table);
                }
                else
                {
                    document.Add(new Paragraph("Không có sản phẩm nào trong đơn hàng này.", font));
                }

                document.Add(new Paragraph($"Tổng Tiền: {invoice.TongTien}", font));

                document.Close();
                return File(memoryStream.ToArray(), "application/pdf", $"Invoice_{invoice.MaHoaDon}.pdf");
            }
        }
    }
}
