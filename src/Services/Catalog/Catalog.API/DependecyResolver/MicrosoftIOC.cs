using Catalog.API.Data;
using Catalog.API.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.API.DependecyResolver
{
    public static class MicrosoftIOC
    {
        public static void AddDependencies(this IServiceCollection services)
        {
            services.AddScoped<ICatalogContext, CatalogContext>();
            services.AddScoped<IProductRepository, ProductRepository>();

        }
    }
}
