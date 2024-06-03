using Microsoft.EntityFrameworkCore;
using UltraPlayBettingData.Data;
using UltraPlayBettingData.Services;

namespace UltraPlayBettingData
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<BettingContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("BettingDatabase")));

            services.AddControllers();
            services.AddSingleton<SportsFeedProcessor>();
            services.AddHostedService<FeedBackgroundService>();
            services.AddSingleton<UpdateService>();

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

    }
}
