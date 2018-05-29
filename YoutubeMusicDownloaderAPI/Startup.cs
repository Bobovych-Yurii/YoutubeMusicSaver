using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using YoutubeMusicDownloaderAPI.Middlewares;
using Microsoft.AspNetCore.Cors;


namespace YoutubeMusicDownloaderAPI
{
    public class Startup
    {
         public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().AddSessionStateTempDataProvider();
            services.AddSession(o => {
                o.IdleTimeout = TimeSpan.FromMinutes(20);
                
                });
            services.AddTransient<IOauthMIddleware,OauthMIddleware>();
            services.AddSingleton<IDownloadMIddleware,DowloadScryptMIddleware>();
            services.AddCors(o => o.AddPolicy("default", builder =>
            {
                builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
            }));
        }
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,IOauthMIddleware oauth)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSession();
            
            app.Use( async (context, next) =>
            {
                byte[] temp;
                string userId = context.Session.TryGetValue("userId",out temp) ? System.Text.Encoding.UTF8.GetString(temp) : "";
                Console.WriteLine(userId+"userID");
                if(userId == "") {
                        userId =  await oauth.Auth();
                        context.Session.Set("userId",System.Text.Encoding.UTF8.GetBytes(userId));
                    }
               await next.Invoke();
            });

            app.UseMvc();
        }
    }
}
