using MessagePack.Resolvers;
using Microsoft.EntityFrameworkCore;
using RealTimeChat.Api.Data;
using RealTimeChat.Api.Hubs;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ChatContext>(o => o.UseSqlServer(connectionString));

builder.Services
    .AddControllersWithViews()
    .AddRazorRuntimeCompilation();

builder.Services
    .AddSignalR(c => c.EnableDetailedErrors = true)
    .AddMessagePackProtocol(c => c.SerializerOptions.WithResolver(NativeGuidResolver.Instance));

var app = builder.Build();

app.UseStaticFiles();

app.UseRouting();

app.MapHub<ChatHub>("/ChatHub");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=LogIn}");

app.Run();
