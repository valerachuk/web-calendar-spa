using System;
using Microsoft.EntityFrameworkCore;
using WebCalendar.Data;

namespace WebCalendar.TestData
{
  public class WebCalendarInMemoryDbContextFixture : IDisposable
  {
    public WebCalendarDbContext Context { get; private set; }

    public WebCalendarInMemoryDbContextFixture()
    {
      var options = new DbContextOptionsBuilder();
      options.UseInMemoryDatabase("WebCalendarTestInMemoryDatabase");

      Context = new WebCalendarDbContext(options.Options);
      Context.Database.EnsureCreated(); // Runs OnModelCreating method
    }

    public void Dispose()
    {
      Context.Dispose();
    }
  }
}
