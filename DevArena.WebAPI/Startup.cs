﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Swagger;

namespace DevArena.WebAPI
{
    public class Startup
    {

        private const string Is4Url = "http://localhost:5001";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllHeaders",
                    builder =>
                    {
                        builder.AllowAnyOrigin();
                        builder.AllowAnyHeader();
                        builder.AllowAnyMethod();
                    });
            });

            services.AddMvc()
                .AddJsonOptions(options =>
                    {
                        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    });

            //services.AddAuthentication();
            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = "Bearer";
                })
            .AddIdentityServerAuthentication(options =>
            {
                options.Authority = Is4Url;
                options.RequireHttpsMetadata = false;
                options.ApiName = "devarena.api";
                options.SupportedTokens = SupportedTokens.Both;
            });
            services.AddAuthorization();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Info{Title = "DevArena protected API", Version="v1"});
                options.AddSecurityDefinition(
                    "oauth2",
                    new OAuth2Scheme
                    {
                        Type="oauth2",
                        Flow="implicit",
                        AuthorizationUrl = $"{Is4Url}/connect/authorize",
                        TokenUrl= $"{Is4Url}/connect/token",
                        Scopes = new Dictionary<string, string>
                        {
                            //{ "devarena.api.limited_access", "Dev Arena limited access" },
                            //{ "devarena.api.full_access", "Dev Arena full access" },
                            { "devarena.api", "Dev Arena protected API" }
                        }
                    });

                options.OperationFilter<AuthorizeCheckOperationFilter>();
                options.DescribeAllEnumsAsStrings();
            });

            services.AddMvc();

            //services.
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            

            app.UseAuthentication();
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Swagger DevArena Protected API");
                options.DocExpansion(DocExpansion.None);
                options.DocumentTitle = "DevArena Protected API Service";

                //IS4
                options.OAuthClientId("apiswagger.clientid");
                options.OAuthClientSecret("apiswaggersecret");
            });

            app.UseMvc(routes =>
            {
                routes.MapSpaFallbackRoute("spa-fallback", defaults:new {controller="Values", action="Index"});
            });
        }
        
    }
}
