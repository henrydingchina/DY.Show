using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace DY.Show
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
            services.AddControllersWithViews();
            JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
            services.AddAuthentication(o =>
            {
                o.DefaultScheme = "Coookies";
                o.DefaultChallengeScheme = "oidc";
            })
            .AddCookie("Coookies")
            .AddOpenIdConnect("oidc", o =>
             {
                 o.Authority = "https://localhost:5001/";
                 o.ClientId = "Dy2_mvc";
                 o.ClientSecret = "sss";
                 o.ResponseType = "code";
                 o.SaveTokens = true;
                 o.GetClaimsFromUserInfoEndpoint = true;
                 o.Scope.Add("userprofile");
                 o.Events = new OpenIdConnectEvents()
                 {
                     OnRemoteFailure = context =>
                     {
                         context.HttpContext.SignOutAsync("Coookies");
                         context.HttpContext.SignOutAsync("oidc");
                         context.Response.Redirect("/");
                         context.HandleResponse();
                         return Task.FromResult(0);
                     }
                 };
             });
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
