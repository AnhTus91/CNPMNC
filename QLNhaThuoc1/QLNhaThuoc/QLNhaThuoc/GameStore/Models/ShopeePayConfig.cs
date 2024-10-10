using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System;

namespace GameStore.Models
{
	public class ShopeePayConfig
	{
		public static string ShopID = "124139"; // ID cửa hàng
		public static string ShopAccount = "SANDBOX.7a8be3779d2e1d000648"; // Tài khoản cửa hàng
		public static string ShopPassword = "0aab44b5176d1fbe"; // Mật khẩu cửa hàng
		public static string ShopeePayLoginUrl = "https://banhang.test-stable.shopee.vn"; // URL đăng nhập cửa hàng
		public static string BuyerAccount = "SANDBOX_BUYER.310330fa6ff024"; // Tài khoản người mua
		public static string BuyerPassword = "58c628216227bd1e"; // Mật khẩu người mua
		public static string BuyerLoginUrl = "https://test-stable.shopee.vn/shop/124139"; // URL đăng nhập của người mua
	}
}
