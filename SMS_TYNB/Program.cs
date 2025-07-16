using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SMS_TYNB.Common;
using SMS_TYNB.Data;
using SMS_TYNB.Helper;
using SMS_TYNB.Models.Identity;
using SMS_TYNB.Models.Master;

namespace SMS_TYNB
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddControllersWithViews()
				.AddNewtonsoftJson(options =>
				{
					options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
				});
			//builder.Services.AddControllersWithViews();

			builder.Services.AddDbContext<SmsTynContext>(options =>
			{
				var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
				options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
			});

			builder.Services.AddIdentity<WpUsers, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
				.AddDefaultUI()
				.AddEntityFrameworkStores<SmsTynIdentityContext>()
				.AddDefaultTokenProviders();

			builder.Services.AddDbContext<SmsTynIdentityContext>(options =>
			{
				var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
				options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
			});

			builder.Services.AddApplicationServices();
			builder.Services.AddScoped<IEmailSender, EmailSender>();

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseStaticFiles();
			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");

			app.MapRazorPages();

			app.Run();
		}
	}
}
