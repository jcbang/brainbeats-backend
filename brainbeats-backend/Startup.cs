using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos;

namespace brainbeats_backend
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            // Initialize Database
            string dbName = Configuration["Database:DatabaseName"];
            string endpoint = Configuration["Database:Endpoint"];
            string key = Configuration["Database:Key"];

            // Create Database if it does not yet exist
            CosmosClient client = new CosmosClient(endpoint, key);
            client.CreateDatabaseIfNotExistsAsync(dbName).Wait();

            // Create Containers if they do not yet exist
            Database db = client.GetDatabase(dbName);
            db.CreateContainerIfNotExistsAsync("Users", "/email").Wait();
            db.CreateContainerIfNotExistsAsync("Beats", "/id").Wait();
            db.CreateContainerIfNotExistsAsync("Playlists", "/id").Wait();
            db.CreateContainerIfNotExistsAsync("Samples", "/id").Wait();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to
        // the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddCors(options =>
            {
                options.AddPolicy("Allow-All", builder => { builder.AllowAnyOrigin(); });
            });
        }

        // This method gets called by the runtime. Use this method to configure the
        // HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors();

            app.UseAuthorization();

            app.UseEndpoints(
                endpoints => { endpoints.MapControllers().RequireCors("Allow-All"); });
        }
    }
}
