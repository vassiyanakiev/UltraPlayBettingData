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
            services.AddControllers();
            services.AddHttpClient();
            services.AddHostedService<XmlFeedService>();

            services.AddDbContext<BettingContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("BettingDatabase")));

            

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
