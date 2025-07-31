using VnptSmsBrandName.Repository;
using VnptSmsBrandName.Service;

namespace VnptSmsBrandName.Common
{
	public static class CollectionRegisterService
	{
		public static IServiceCollection AddRepositories(this IServiceCollection services)
		{
			services.AddScoped<WpUsersRepository>();
			services.AddScoped<SmsConfigRepository>();
			services.AddScoped<ConfigRepository>();
			services.AddScoped<MEmployeeRepository>();
			services.AddScoped<MGroupRepository>();
			services.AddScoped<MGroupEmployeeRepository>();
			services.AddScoped<MSmsRepository>();
			services.AddScoped<MSmsEmployeeRepository>();
			services.AddScoped<MSmsFileRepository>();
			services.AddScoped<MFileRepository>();
			services.AddScoped<MHistoryRepository>();

			return services;
		}

		public static IServiceCollection AddServices(this IServiceCollection services)
		{
			services.AddScoped<ISmsConfigService, SmsConfigService>();
			services.AddScoped<IConfigService, ConfigService>();
			services.AddScoped<IDataTransportService, DataTransportService>();
			services.AddScoped<IMEmployeeService, MEmployeeService>();
			services.AddScoped<IMGroupService, MGroupService>();
			services.AddScoped<IMSmsService, MSmsService>();
			services.AddScoped<IMFileService, MFileService>();
			services.AddScoped<ICurrentUserService, CurrentUserService>();

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
