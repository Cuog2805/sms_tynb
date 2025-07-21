using SMS_TYNB.Repository;
using SMS_TYNB.Service;
using SMS_TYNB.Service.Implement;

namespace SMS_TYNB.Common
{
	public static class CollectionRegisterService
	{
		public static IServiceCollection AddRepositories(this IServiceCollection services)
		{
			services.AddScoped<WpCanboRepository>();
			services.AddScoped<WpNhomRepository>();
			services.AddScoped<WpDanhmucRepository>();
			services.AddScoped<WpNhomCanboRepository>();
			services.AddScoped<WpSmsRepository>();
			services.AddScoped<WpSmsCanboRepository>();
			services.AddScoped<WpFileRepository>();
			services.AddScoped<WpUsersRepository>();
			services.AddScoped<SmsConfigRepository>();
			return services;
		}

		public static IServiceCollection AddServices(this IServiceCollection services)
		{
			services.AddScoped<IWpCanboService, WpCanboService>();
			services.AddScoped<IWpNhomService, WpNhomService>();
			services.AddScoped<IWpDanhmucService, WpDanhmucService>();
			services.AddScoped<IWpSmsService, WpSmsService>();
			services.AddScoped<IWpFileService, WpFileService>();
			services.AddScoped<ISmsConfigService, SmsConfigService>();
			return services;
		}
		public static IServiceCollection AddApplicationServices(this IServiceCollection services) 
		{
			services.AddRepositories();
			services.AddServices();
			return services;
		}
	}
}
