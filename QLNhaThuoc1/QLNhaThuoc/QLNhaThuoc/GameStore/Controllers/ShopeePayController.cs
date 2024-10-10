using GameStore.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace GameStore.Controllers
{
    public class ShopeePayController : Controller
    {
		// GET: ShopeePay
		public ActionResult Index()
		{
			// Trang hiển thị thông tin thanh toán ShopeePay
			return View();
		}

		[HttpPost]
		public async Task<ActionResult> CreatePayment()
		{
			var paymentData = new
			{
				ShopID = "124139",
				TotalAmount = 100000,  // số tiền thanh toán
				Currency = "VND"
			};

			var paymentResponse = await CreateShopeePayPayment(paymentData);

			if (paymentResponse.IsSuccess)
			{
				return Redirect(paymentResponse.PaymentUrl);  // Chuyển hướng đến URL thanh toán
			}
			else
			{
				// Hiển thị lỗi nếu có
				ViewBag.ErrorMessage = paymentResponse.Message; // Ghi lại thông tin lỗi
				return View("Error");
			}
		}



		private async Task<dynamic> CreateShopeePayPayment(dynamic paymentData)
		{
			try
			{
				using (var client = new HttpClient())
				{

					var apiUrl = "https://banhang.test-stable.shopee.vn/api/v1/payment/create";


					var json = JsonConvert.SerializeObject(paymentData);
					var content = new StringContent(json, Encoding.UTF8, "application/json");

					var response = await client.PostAsync(apiUrl, content);

					if (response.IsSuccessStatusCode)
					{
						var responseContent = await response.Content.ReadAsStringAsync();
						var result = JsonConvert.DeserializeObject<dynamic>(responseContent);

						// Kiểm tra kết quả trả về từ ShopeePay
						return new
						{
							IsSuccess = result?.IsSuccess == true,
							PaymentUrl = result?.PaymentUrl,
							Message = result?.Message // Ghi lại thông báo từ API nếu cần
						};
					}
					else
					{
						// Lỗi từ API
						return new { IsSuccess = false, Message = $"Lỗi API: {response.ReasonPhrase}" };
					}
				}
			}
			catch (Exception ex)
			{
				// Bắt lỗi và ghi lại thông tin
				return new { IsSuccess = false, Message = $"Lỗi: {ex.Message}" };
			}
		}




	}
}