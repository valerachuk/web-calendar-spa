using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using WebCalendar.Business;
using WebCalendar.Business.Common;
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

      // CORS
      var frontOptions = Configuration.GetSection("Front").Get<FrontOptions>();
      services.AddCors(options =>
      {
        options.AddDefaultPolicy(builder =>
        {
          builder
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
            .WithOrigins(frontOptions.AddressFront);
        });
      });

      // Auth
      var authOptionsSection = Configuration.GetSection("Auth");
      services.Configure<AuthOptions>(authOptionsSection);

      var authOptions = authOptionsSection.Get<AuthOptions>();
      services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
          options.RequireHttpsMetadata = true;
          options.TokenValidationParameters = new TokenValidationParameters
          {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = authOptions.SymmetricSecurityKey,

            ValidateAudience = true,
            ValidAudience = authOptions.Audience,

            ValidateIssuer = true,
            ValidIssuer = authOptions.Issuer,

            ValidateLifetime = true
          };
        });

      // Repositories
      services.AddTransient<IUserRepository, UserRepository>();
      services.AddTransient<ICalendarRepository, CalendarRepository>();
      services.AddTransient<IEventRepository, EventRepository>();

      // Domains
      services.AddTransient<IUserDomain, UserDomain>();
      services.AddTransient<ICalendarDomain, CalendarDomain>();
      services.AddTransient<IEventDomain, EventDomain>();

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
      app.UseCors();

      app.UseAuthentication();
      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });
    }
  }
}
