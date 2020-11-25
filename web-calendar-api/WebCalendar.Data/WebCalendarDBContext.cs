using Microsoft.EntityFrameworkCore;
using WebCalendar.Data.Entities;

namespace WebCalendar.Data
{
  public class WebCalendarDbContext : DbContext, IWebCalendarDbContext
  {
    public WebCalendarDbContext(DbContextOptions contextOptions) : base(contextOptions)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      var userModelBuilder = modelBuilder.Entity<User>();

      userModelBuilder
        .HasIndex(u => u.Email)
        .IsUnique();

      userModelBuilder
        .Property(user => user.Email)
        .IsRequired();

      userModelBuilder
        .Property(user => user.FirstName)
        .IsRequired();

      userModelBuilder
        .Property(user => user.LastName)
        .IsRequired();

      userModelBuilder
        .Property(user => user.PasswordHash)
        .IsRequired();

      userModelBuilder
        .Property(user => user.Salt)
        .IsRequired();

      userModelBuilder
        .Property(user => user.ReceiveEmailNotifications)
        .HasDefaultValue(true);

      userModelBuilder
        .HasData(new[]
        {
          new User
          {
            Id = 1, FirstName = "FN1", LastName = "LN1", Email = "user1@mail.com",
            PasswordHash = new byte[] {1, 2, 3, 4, 5}, Salt = new byte[] { 1, 2 }
          },
          new User
          {
            Id = 2, FirstName = "FN2", LastName = "LN2", Email = "user2@mail.com",
            PasswordHash = new byte[] {1, 2, 3, 4, 5}, Salt = new byte[] { 1, 2 }
          },
          new User
          {
            Id = 3, FirstName = "FN3", LastName = "LN3", Email = "user3@mail.com",
            PasswordHash = new byte[] {1, 2, 3, 4, 5}, Salt = new byte[] { 1, 2 }
          }
        });

      var calendarModelBuilder = modelBuilder.Entity<Calendar>();

      calendarModelBuilder
        .Property(calendar => calendar.Name)
        .IsRequired()
        .HasMaxLength(100);

      calendarModelBuilder
        .Property(calendar => calendar.Description)
        .HasMaxLength(1000);

      calendarModelBuilder
        .HasOne(calendar => calendar.User)
        .WithMany(user => user.Calendars)
        .HasForeignKey(calendar => calendar.UserId)
        .IsRequired();

      var eventModelBuilder = modelBuilder.Entity<Event>();

      eventModelBuilder
        .Property(calendarEvent => calendarEvent.Name)
        .IsRequired()
        .HasMaxLength(100);

      eventModelBuilder
        .Property(calendarEvent => calendarEvent.Venue)
        .HasMaxLength(100);

      eventModelBuilder
        .Property(calendarEvent => calendarEvent.StartDateTime)
        .IsRequired();

      eventModelBuilder
       .Property(calendarEvent => calendarEvent.EndDateTime)
       .IsRequired();

      modelBuilder.HasSequence<int>("SeriesId_seq")
        .StartsAt(1)
        .IncrementsBy(1);

      eventModelBuilder
        .Property(calendarEvent => calendarEvent.SeriesId)
        .HasDefaultValueSql("NEXT VALUE FOR shared.SeriesId_seq");

      eventModelBuilder
        .HasOne(calendarEvent => calendarEvent.Calendar)
        .WithMany(calendar => calendar.Events)
        .HasForeignKey(calendarEvent => calendarEvent.CalendarId)
        .IsRequired();

      eventModelBuilder
        .HasOne(ev => ev.File)
        .WithOne(f => f.Event)
        .HasForeignKey<Event>(ev => ev.FileId);

      var logModelBuilder = modelBuilder.Entity<Log>();

      logModelBuilder
        .Property(log => log.Message)
        .IsRequired();

      logModelBuilder
        .Property(log => log.Level)
        .IsRequired();

      logModelBuilder
        .Property(log => log.DateTime)
        .IsRequired();

      var eventGuestsModelBuilder = modelBuilder
        .Entity<EventGuests>()
        .HasKey(eg => new
        {
          eg.UserId,
          eg.EventId
        });

      modelBuilder.Entity<EventGuests>()
      .HasOne<Event>(ev => ev.Event)
      .WithMany(s => s.Guests)
      .HasForeignKey(sc => sc.EventId);

      modelBuilder.Entity<EventGuests>()
        .HasOne<User>(sc => sc.User)
        .WithMany(s => s.SharedEvents)
        .HasForeignKey(sc => sc.UserId);


      var fileBuilder = modelBuilder.Entity<UserFile>();

      fileBuilder
        .Property(ef => ef.Path)
          .IsRequired();

      fileBuilder
        .Property(ef => ef.Size)
          .IsRequired();

      fileBuilder
        .Property(ef => ef.Name)
          .IsRequired();

      fileBuilder
        .Property(ef => ef.UploadDate)
          .IsRequired();
    }
    public DbSet<User> Users { get; set; }
    public DbSet<Calendar> Calendars { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<EventGuests> EventGuests { get; set; }
    public DbSet<UserFile> EventFile { get; set; }
  }
}
