using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HumanResource.Application.Helper;
using HumanResource.Application.Helper.Dtos;
using HumanResource.Data.EF;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Http;
using HumanResoureAPI.Hubs;
using HumanResoureAPI.Common.Systems;
using HumanResource.Application.Mail;
using HumanResource.Application.Notifi;
using HumanResource.Application.Hub;

namespace HumanResoureAPI
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
            services.AddControllers().AddJsonOptions(opt => opt.JsonSerializerOptions.PropertyNamingPolicy = null);
            services.AddDbContext<humanDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("HumanConnect")));
            services.AddDbContext<onlineDbContext>(options => options.UseSqlServer("server=112.78.2.74;database=nhi99444_AppOffice;User ID=nhi99444_sa;Password=Thong1996@"));
            // Read json settings
            services.Configure<TokenManagement>(Configuration.GetSection("tokenManagement"));

            services.AddCors();

            var tokenManagement = Configuration.GetSection("tokenManagement").Get<TokenManagement>();
            var secret = Encoding.ASCII.GetBytes(tokenManagement.Secret);

            // Add Authentication
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = false;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(secret),
                    //ValidIssuer = tokenManagement.Issuer,
                    //ValidAudience = tokenManagement.Audience,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = System.TimeSpan.Zero
                };
            });
            services.Configure<SmtpSettings>(Configuration.GetSection("smtpSettings"));
            services.AddSingleton<IMailer, Mailer>();
            services.AddScoped<INotifi, Notifiler>();
            services.AddScoped<IHubNotifi, SignalAppOffice>();
            services.AddSignalR();
            services.AddScoped<IAuthentication, AuthenticationService>();
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
                app.UseHsts();
            }

            app.UseCors(builder =>
            {
                builder.WithOrigins("http://localhost:4567", "http://172.16.10.2:4567/", "http://172.16.10.2:4567", "http://localhost:4200", "http://113.190.242.215:4556")
                .AllowAnyHeader().AllowAnyMethod().AllowCredentials();
            });
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseHttpsRedirection();
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Resources")),
                RequestPath = new PathString("/Resources")
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<SignalAppOffice>("/signalrtc");
            });
        }
    }
}
