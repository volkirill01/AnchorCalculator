using Core.AnchorCalculator.Entities;
using DAL.AnchorCalculator;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NLog;
using UI.AnchorCalculator.Extensions;
using UI.AnchorCalculator.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "/Nlog.config"));

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
	string connection = builder.Configuration.GetConnectionString("DefaultConnection");
	options.UseMySql(connection, ServerVersion.AutoDetect(connection));
});

// Add services to the container
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
	options.Password.RequiredLength = 12;
	options.Password.RequireNonAlphanumeric = false;
	options.Password.RequireLowercase = false;
	options.Password.RequireUppercase = false;
	options.Password.RequireDigit = false;
	options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 -._@+ " + "àáâãäå¸æçèéêëìíîïğñòóôõö÷øùúûüışÿÀÁÂÃÄÅ¨ÆÇÈÉÊËÌÍÎÏĞÑÒÓÔÕÖ×ØÙÚÛÜİŞß";
}).AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddDataProtection()
	.PersistKeysToDbContext<ApplicationDbContext>()
	.SetApplicationName("AnchorCalculator");
builder.Services.AddSingleton<LoggerManager>();
builder.Services.AddControllersWithViews();

builder.Services.AddTransient<MaterialService>();
builder.Services.AddTransient<AnchorService>();
builder.Services.AddTransient<SvgMakingService>();
builder.Services.AddTransient<CalculateService>();

WebApplication app = builder.Build();
app.UsePathBase("/anchor");

using (var scope = app.Services.CreateScope())
{
	var services = scope.ServiceProvider;
	try
	{
		var userManager = services.GetRequiredService<UserManager<User>>();
		var rolesManager = services.GetRequiredService<RoleManager<IdentityRole>>();
		await RoleInitializer.InitializeAsync(userManager, rolesManager);
	}
	catch (Exception ex)
	{
		var logger = services.GetRequiredService<ILogger<Program>>();
		logger.LogError(ex, "An error occurred while seeding the database.");
	}
};


// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");

	// The default HSTS value is 30 days
	app.UseHsts();
}

app.UseStaticFiles();
app.UseHttpsRedirection();

app.UseRouting();
app.MapControllers();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(name: "default", pattern: "{controller=Anchor}/{action=Index}/{id?}");

app.Run();
