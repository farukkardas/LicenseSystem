using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Core.DependencyResolvers;
using Core.Extensions;
using Core.Utilities.IoC;
using Core.Utilities.Security.Encryption;
using Core.Utilities.Security.JWT.Concrete;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Hosting;
using DataAccess.Concrete;
using Microsoft.EntityFrameworkCore;

namespace WebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddCors(options => options.AddPolicy("ApiCorsPolicy",
            //    builder => { builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().SetIsOriginAllowed((host) => true); }));
            services.AddCors(options => options.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyMethod().AllowAnyHeader().AllowCredentials().SetIsOriginAllowed(origin => true);
            }));


            services.AddControllers();
            services.AddRouting(r => r.SuppressCheckForUnhandledSecurityMetadata = true);
            var tokenOptions = Configuration.GetSection("TokenOptions").Get<TokenOptions>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidIssuer = tokenOptions.Issuer,
                        ValidAudience = tokenOptions.Audience,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = SecurityKeyHelper.CreateSecurityKey(tokenOptions.SecurityKey)
                    };
                });

            services.AddDependencyResolvers(new ICoreModule[]
            {
                new CoreModule()
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "License API",
                    Description = "Licensing admin panel that you can use in your applications.",

                    Contact = new OpenApiContact
                    {
                        Name = "Faruk Kardas",
                        Email = "farukkardasx@gmail.com",
                        Url = new Uri("https://www.linkedin.com/in/faruk-kardas/"),
                    }
                });
            });
            services.AddSignalR();
            services.AddSignalRCore();
        //    services.AddDbContext<LicenseSystemContext>(x => x.UseSqlServer(Configuration.GetConnectionString("SQLProvider"), y => y.MigrationsAssembly("DataAccess")));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //app.UseCors("ApiCorsPolicy");
            app.UseCors();
            //Using static files from root directory
            app.UseStaticFiles(new StaticFileOptions
            {
                ServeUnknownFileTypes = true
            });

          
            app.Use((context, next) =>
            {
                context.Items["__CorsMiddlewareInvoked"] = true;
                return next();
            });



            if (env.IsDevelopment() || env.IsProduction())
            {
                {
                    app.UseDeveloperExceptionPage();
                    app.UseSwagger(c => { c.RouteTemplate = "/swagger/{documentName}/swagger.json"; });
                    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "LicenseSystem v1"));
                }
            }

            //  app.ConfigureCustomExceptionMiddleware();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseAuthentication();
            
            app.UseEndpoints(endpoints => { endpoints.MapControllers();});

        }
    }
}