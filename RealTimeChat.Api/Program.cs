using Microsoft.EntityFrameworkCore;
using RealTimeChat.Api.Data;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ChatContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

builder.Services.AddSignalR();

var app = builder.Build();

app.UseStaticFiles();

app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Login}");

app.Run();
