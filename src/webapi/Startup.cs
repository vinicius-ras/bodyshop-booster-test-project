using BodyShopBoosterTest.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BodyShopBoosterTest
{
    /// <summary>Startup class for configuring the web host.</summary>
    public class Startup
    {
        /// <summary>Configures the services and dependency injection features for the web host.</summary>
        /// <param name="services">The collection of services, where new services and dependency injection features are to be added or modified.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddDbContextPool<AppDbContext>(opts => {
                opts.UseSqlServer("name=ConnectionStrings:DatabaseConnectionString");
            });
        }


        /// <summary>Configures the HTTP middleware for the web host.</summary>
        /// <param name="app">Reference to an object which is used to build and configure the HTTP middleware components.</param>
        /// <param name="env">Container-injected reference to an object providing execution environment information.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
