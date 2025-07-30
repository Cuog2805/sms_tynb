using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SMS_TYNB.Common;
using SMS_TYNB.Data;
using SMS_TYNB.Helper;
using SMS_TYNB.Models.Identity;
using SMS_TYNB.Models.Master;
using SMS_TYNB.BackgoundSercvice;

namespace SMS_TYNB
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var connectionString = "Server=localhost;Port=3306;Database=VnptSmsBrandName;Uid=root;Pwd=280503;CharSet=utf8mb4;";
            // Add services to the container.
            builder.Services.AddControllersWithViews()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
                });
            //builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<SmsTynContext>(options =>
            {
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            });

            builder.Services.AddRazorPages();

            builder.Services.AddIdentity<WpUsers, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddDefaultUI()
                .AddEntityFrameworkStores<SmsTynIdentityContext>()
                .AddDefaultTokenProviders();

            builder.Services.AddDbContext<SmsTynIdentityContext>(options =>
            {
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            });
            AddRateLimited(builder);
            builder.Services.AddApplicationServices();
            builder.Services.AddScoped<IEmailSender, EmailSender>();
            builder.Services.AddHostedService<FileCleanupService>();

            var app = builder.Build();
            app.UseMiddleware<MyIPRateLimitMiddleware>();
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.Use(async (context, next) =>
            {
                if (
                    context.Request.Path.Equals("/Identity/Account/Register", StringComparison.OrdinalIgnoreCase)
                    || context.Request.Path.Equals("/Identity/Account/ForgotPassword", StringComparison.OrdinalIgnoreCase)
                    || context.Request.Path.Equals("/Identity/Account/ForgotPasswordConfirmation", StringComparison.OrdinalIgnoreCase)
                    || context.Request.Path.Equals("/Identity/Account/ResetPassword", StringComparison.OrdinalIgnoreCase)
                    || context.Request.Path.Equals("/Identity/Account/ResetPasswordConfirmation", StringComparison.OrdinalIgnoreCase)
                )
                {
                    context.Response.StatusCode = 404;
                    await context.Response.WriteAsync("Not Found");
                    return;
                }

                await next();
            });

            app.UseStaticFiles();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapRazorPages();

            app.Run();
        }
        private static void AddRateLimited(WebApplicationBuilder builder)
        {
            builder.Services.AddOptions();
            builder.Services.AddMemoryCache();
            builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
            builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            builder.Services.AddMvc();
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
            builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        }
    }

}
