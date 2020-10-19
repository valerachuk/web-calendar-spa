  using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebCalendar.Business;
using WebCalendar.Business.Domains;
  using WebCalendar.Business.Domains.Interfaces;
  using WebCalendar.Data;
using WebCalendar.Data.Repositories;
  using WebCalendar.Data.Repositories.Interfaces;

  namespace WebCalendar.Api
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
      services.AddControllers();
      services.AddDbContext<IWebCalendarDbContext, WebCalendarDbContext>(builder =>
        builder.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

      // Repositories
      services.AddTransient<IUserRepository, UserRepository>();

      // Domains
      services.AddTransient<IUserDomain, UserDomain>();

      // AutoMapper
      var mapConfig = new MapperConfiguration(mc => mc.AddProfile(new MappingProfile()));
      services.AddSingleton(mapConfig.CreateMapper());

    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseHttpsRedirection();

      app.UseRouting();

      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });
    }
  }
}