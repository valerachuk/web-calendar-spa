using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using Hangfire;
using Microsoft.Extensions.Options;
using WebCalendar.Business.Common;
using WebCalendar.Business.Domains.Interfaces;
using WebCalendar.Data.Repositories.Interfaces;

namespace WebCalendar.Business.Domains
{
  public class NotificationSenderDomain : INotificationSenderDomain
  {
    private readonly IOptions<EmailNotificationsOptions> _emailSenderOptions;
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly IUserRepository _userRepository;
    private readonly IEventRepository _eventRepository;
    private readonly ICalendarRepository _calendarRepository;

    public NotificationSenderDomain(
      IOptions<EmailNotificationsOptions> emailSenderOptions,
      IUserRepository userRepository,
      IBackgroundJobClient backgroundJobClient,
      IEventRepository eventRepository,
      ICalendarRepository calendarRepository
      )
    {
      _backgroundJobClient = backgroundJobClient;
      _userRepository = userRepository;
      _emailSenderOptions = emailSenderOptions;
      _eventRepository = eventRepository;
      _calendarRepository = calendarRepository;
    }

    private void SendEmail(string recipientEmail, string message)
    {
      var emailSenderOptions = _emailSenderOptions.Value;

      using var client = new SmtpClient(emailSenderOptions.Host, emailSenderOptions.Port)
      {
        Credentials = new NetworkCredential(emailSenderOptions.Login, emailSenderOptions.Password),
        EnableSsl = true
      };

      var emailTemplate = File.ReadAllText(emailSenderOptions.HTMLEmailTemplatePath);
      using var mailMessage = new MailMessage(new MailAddress(emailSenderOptions.Login, emailSenderOptions.SenderDisplayName), new MailAddress(recipientEmail))
      {
        Body = emailTemplate
          .Replace("__CONTENT__", message)
          .Replace("__TIMESTAMP__", DateTime.Now.ToString("T")),
        Subject = emailSenderOptions.EmailSubject,
        IsBodyHtml = true
      };

      try
      {
        client.Send(mailMessage);
      }
      catch (Exception e)
      {
        Console.WriteLine($"Cannot send email to ${recipientEmail} cause: {e.Message}");
      }
    }

    public void ScheduleEventCreatedNotification(int eventId) =>
      _backgroundJobClient.Enqueue<NotificationSenderDomain>(notificationSender => notificationSender.NotifyEventCreated(eventId));

    public void ScheduleEventStartedNotification(int eventId)
    {
      var @event = _eventRepository.GetEvent(eventId);
      if (@event.NotificationTime == null) return;

      @event.NotificationJobId = _backgroundJobClient.Schedule<NotificationSenderDomain>(notificationSender 
        => notificationSender.NotifyEventStarted(eventId), @event.StartDateTime - TimeSpan.FromMinutes((int)@event.NotificationTime) - DateTimeOffset.Now.Offset);
      // - DateTimeOffset.Now.Offset, because date in database contains as local, but EF thinks that it is UTC

      _eventRepository.UpdateEvent(@event);
    }

    public void NotifyEventCreated(int eventId)
    {
      var @event = _eventRepository.GetEvent(eventId);  // can't name variable "event"
      var calendar = _calendarRepository.GetCalendar(@event.CalendarId);
      var user = _userRepository.GetUser(calendar.UserId);
      if (!user.ReceiveEmailNotifications) return;

      var notificationMessage = $@"
        Hello <i>{ user.FirstName },</i>
        <br>Event {(@event.Reiteration != null ? "series" : "")}
        <b>{ @event.Name }</b> has been created successfully
        in calendar <b>{ calendar.Name }</b>.
      ";

      SendEmail(user.Email, notificationMessage);
    }

    public void NotifyEventStarted(int eventId)
    {
      var @event = _eventRepository.GetEvent(eventId);  // can't name variable "event"
      var calendar = _calendarRepository.GetCalendar(@event.CalendarId);
      var user = _userRepository.GetUser(calendar.UserId);
      if (!user.ReceiveEmailNotifications) return;

      var notificationMessage = $@"
        Hello <i>{ user.FirstName },</i>
        <br>Event <b>{ @event.Name }</b>
        in calendar <b>{ calendar.Name }</b>
        will begin at <b>{ @event.StartDateTime:g}</b>.
      ";

      SendEmail(user.Email, notificationMessage);
    }

  }
}
