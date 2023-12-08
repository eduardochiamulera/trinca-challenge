using Microsoft.Extensions.DependencyInjection;
using Services.Services;
using Services.Services.Interfaces;

namespace Services
{
	public static class ServiceCollectionExtensions
    {
		public static IServiceCollection AddServicesDependencies(this IServiceCollection services)
			=> services.AddTransient<IBbqService, BbqService>()
			   .AddTransient<IPersonService, PersonService>();

    }
}
