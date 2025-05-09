using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Shop.DataAccess.Data;
using Shop.DataAccess.Implementation;
using Shop.Entities.Repository;
using myshop.Utilities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));
// 2) ADD IDENTITY
builder.Services
    .AddIdentity<IdentityUser, IdentityRole>(options =>
    {
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromHours(4);
    }).AddDefaultTokenProviders().AddDefaultUI()
    .AddEntityFrameworkStores<ApplicationDbContext>();
   /* options => {
        // you can tweak password/lockout/user settings here
        options.SignIn.RequireConfirmedAccount = false;
        *//*options.Password.RequireDigit = true;
        options.Password.RequiredLength = 6;*//*
    }*/


// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
builder.Services.AddSingleton<IEmailSender, EmailSender>();
builder.Services.AddScoped<IUnitOfWork,UnitOfWork>();
builder.Services.AddHttpContextAccessor();
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

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "admin",
    pattern: "{area=Admin}/{controller=Product}/{action=Index}/{id?}"); //{id?}


app.Run();
