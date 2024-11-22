using DAPMver1.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Configuration;
using static Azure.Core.HttpHeader;
using DAPMver1.Common;
using Common = DAPMver1.Common.Common;
using DAPMver1.Services;
using DAPMver1.Helpers; // Đảm bảo rằng namespace này là đúng


var builder = WebApplication.CreateBuilder(args);
//
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<Common>();
builder.Services.AddScoped<PayPalService>();
//db
builder.Services.AddDbContext<DapmTrangv1Context>(options =>
{
options.UseSqlServer(builder.Configuration["DAPM"]);
});
//
builder.Services.AddHttpClient();
//
builder.Services.AddScoped<DonHangServices>();
builder.Services.AddScoped<EmailService>();
//
builder.Services.AddSingleton<IVnPayService, VnPayService>();
//
builder.Services.AddSession(options =>
{
options.IdleTimeout = TimeSpan.FromMinutes(30); // Thời gian hết hạn session
options.Cookie.HttpOnly = true; // Bảo mật cookie session
options.Cookie.IsEssential = true; // Đảm bảo cookie luôn được gửi ngay cả khi người dùng không đồng ý với cookie khác
});


var app = builder.Build();
//
// Cấu hình routing hỗ trợ Areas
app.MapControllerRoute(
name: "areas",
pattern: "{area:exists}/{controller=AdminHome}/{action=Index}/{id?}");
//
app.UseSession();
// Configure the HTTP request pipeline.

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
name: "default",
pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run(); 
