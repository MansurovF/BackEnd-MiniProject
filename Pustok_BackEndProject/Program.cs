using Microsoft.EntityFrameworkCore;
using Pustok_BackEndProject.DataAccessLayer;
using Pustok_BackEndProject.Interfaces;
using Pustok_BackEndProject.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews().AddNewtonsoftJson()
    .AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(15);
});

builder.Services.AddScoped<ILayoutService, LayoutService>();
builder.Services.AddHttpContextAccessor();
var app = builder.Build();

app.UseSession();
app.UseStaticFiles();

app.MapControllerRoute(
            name: "areas",
            pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}"
          );
app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");

app.Run();
    