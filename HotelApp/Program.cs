using LogicaDeNegocio.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// ✅ Autenticación (Google + Cookies)
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/Login/Login";           // ruta al login
    options.LogoutPath = "/Login/Logout";         // ruta al logout
    options.AccessDeniedPath = "/Login/AccessDenied"; // ✅ ruta corregida
})
.AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
{
    options.ClientId = builder.Configuration.GetSection("GoogleKeys:ClientId").Value;
    options.ClientSecret = builder.Configuration.GetSection("GoogleKeys:ClientSecret").Value;
});

// ✅ Controladores y vistas
builder.Services.AddControllersWithViews();

// ✅ Base de datos
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("ConexionHotel"),
        b => b.MigrationsAssembly("LogicaDeNegocio"))
    );

var app = builder.Build();

// ✅ Configuración del pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 🔹 Esta línea es OBLIGATORIA para que funcione la autenticación
app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Login}/{id?}");

app.Run();

app.UseAuthentication();  // ✅ Esto activa la autenticación
app.UseAuthorization();   // ✅ Esto activa la autorización
