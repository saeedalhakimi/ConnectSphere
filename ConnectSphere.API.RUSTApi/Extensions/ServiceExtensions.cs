using ConnectSphere.API.Application.DomainRepositories.IRepositories;
using ConnectSphere.API.Application.Services;
using ConnectSphere.API.Infrastructure.Repositories;

namespace ConnectSphere.API.RUSTApi.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddRegistrationServices(this IServiceCollection services)
        {
            services.AddScoped<IErrorHandlingService, ErrorHandlingService>();
            services.AddScoped<IPersonRepository, PersonRepository>();
            services.AddScoped<ICountryRepository, CountryRepository>();
            services.AddScoped<IGovernmentalInfoRepository, GovernmentalInfoRepository>();
            return services;
        }
    }
}
