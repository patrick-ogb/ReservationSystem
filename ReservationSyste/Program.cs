using Hangfire;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using ReservationSyste.Data;
using ReservationSyste.Services.Interfices;
using ReservationSyste.Services.Repository;
using ReservationSyste.Utility;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
var dataStore = builder.Configuration.GetConnectionString("Data");
builder.Services.AddDbContext<ApplicationDbContext>(option => option.UseSqlServer(dataStore));
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var emailConfig = builder.Configuration.GetSection("RecaptchaOption").Get<RecaptchaOption>();
builder.Services.AddSingleton(emailConfig);

builder.Services.Configure<RecaptchaOption>(builder.Configuration.GetSection(nameof(RecaptchaOption)));

builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSingleton<ITempDataProvider, CookieTempDataProvider>();
builder.Services.AddSession(option =>
{
    option.IdleTimeout = TimeSpan.FromMinutes(128);
});

builder.Services.AddScoped<IReservationService, ReservationService>();

ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Or LicenseContext.Commercial

//CONFIGURE BACKGROUNDSERVICE
//builder.Services.AddHostedService<BackgroundWorkerService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
