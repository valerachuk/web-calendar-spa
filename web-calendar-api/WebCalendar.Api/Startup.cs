using System;
using AutoMapper;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using NLog;
using NLog.Extensions.Logging;
using WebCalendar.Api.Middleware;
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


      // Hangfire
      services.AddHangfire(configuration => configuration
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseSqlServerStorage(Configuration.GetConnectionString("DefaultConnection"), new SqlServerStorageOptions
        {
          CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
          SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
          QueuePollInterval = TimeSpan.Zero,
          UseRecommendedIsolationLevel = true,
          DisableGlobalLocks = true
        }));

      services.AddHangfireServer();


      // Repositories
      services.AddTransient<IUserRepository, UserRepository>();
      services.AddTransient<ICalendarRepository, CalendarRepository>();
      services.AddTransient<IEventRepository, EventRepository>();
      services.AddTransient<ICalendarItemRepository, CalendarItemRepository>();
      services.AddTransient<IFileRepository, FileRepository>();

      // Domains
      services.AddTransient<IUserDomain, UserDomain>();
      services.AddTransient<ICalendarDomain, CalendarDomain>();
      services.AddTransient<IEventDomain, EventDomain>();
      services.AddTransient<ICalendarItemDomain, CalendarItemDomain>();
      services.AddTransient<INotificationSenderDomain, NotificationSenderDomain>();
      services.AddTransient<IFileDomain, FileDomain>();

      // AutoMapper
      var mapConfig = new MapperConfiguration(mc => mc.AddProfile(new MappingProfile()));
      services.AddSingleton(mapConfig.CreateMapper());

      // Email notifications
      var emailSenderSection = Configuration.GetSection("EmailNotifications");
      services.Configure<EmailNotificationsOptions>(emailSenderSection);

      var logConfig = Configuration.GetSection("NLog");
      LogManager.Configuration = new NLogLoggingConfiguration(logConfig);
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseMiddleware<ExceptionMiddleware>();

      app.UseHttpsRedirection();

      app.UseRouting();
      app.UseCors();

      app.UseAuthentication();
      app.UseAuthorization();

      GlobalConfiguration.Configuration.UseActivator(new HangfireActivator(serviceProvider));
      app.UseHangfireDashboard();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
        endpoints.MapHangfireDashboard();
      });
    }
  }
}
