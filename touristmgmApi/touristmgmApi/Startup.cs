using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime;
using touristmgmApi.Repository;
using touristmgmApi.BusinessLayer;
using touristmgmApi.ActionFilters;
using Microsoft.AspNetCore.Http;

namespace touristmgmApi
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
            services.AddCors(Options => {
                Options.AddPolicy("TouristMgmCORSPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithHeaders());
            });
            //var accessKey = Environment.GetEnvironmentVariable("ACCESS_KEY");
            //var secretKey = Environment.GetEnvironmentVariable("SECRET_KEY");
            var accessKey = "AKIAS575NZFZYW5LCH5K";
            var secretKey = "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";           
            

            var credentials = new BasicAWSCredentials(accessKey, secretKey);
            var config = new AmazonDynamoDBConfig()
            {
                RegionEndpoint = RegionEndpoint.USEast2
            };

            var client = new AmazonDynamoDBClient(credentials, config);
            services.AddSingleton<IAmazonDynamoDB>(client);
            services.AddSingleton<IDynamoDBContext, DynamoDBContext>();

            services.AddScoped<ITouristRepository, TouristRepository>();
            services.AddScoped<ITouristBusiness, TouristBusiness>();
            services.AddControllers();
            services.AddControllers((config) => {
                config.Filters.Add(new ValidateModelAttribute());
            });
            services.AddSwaggerGen();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors("TouristMgmCORSPolicy");
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
              //  app.UseDeveloperExceptionPage();
              
            }
            //app.UseSwagger();
            //app.UseSwaggerUI();
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Welcome to running ASP.NET Core on AWS Lambda");
                });
            });
        }
    }
}
