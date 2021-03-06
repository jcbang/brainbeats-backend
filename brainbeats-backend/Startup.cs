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

namespace brainbeats_backend {
public class Startup {
  public Startup(IConfiguration configuration) {
    Configuration = configuration;
  }

  public IConfiguration Configuration { get; }

  // This method gets called by the runtime. Use this method to add services to
  // the container.
  public static void ConfigureServices(IServiceCollection services) {
    services.AddControllers();
    services.AddCors(options => {
      options.AddPolicy("Allow-All", builder => { builder.AllowAnyOrigin(); });
    });
  }

  // This method gets called by the runtime. Use this method to configure the
  // HTTP request pipeline.
  public static void Configure(IApplicationBuilder app,
                               IWebHostEnvironment env) {
    if (env.IsDevelopment()) {
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
