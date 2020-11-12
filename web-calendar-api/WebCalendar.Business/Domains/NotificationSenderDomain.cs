using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using Hangfire;
using Microsoft.Extensions.Options;
using WebCalendar.Business.Common;
using WebCalendar.Business.Domains.Interfaces;
using WebCalendar.Data.Entities;
using WebCalendar.Data.Repositories.Interfaces;

namespace WebCalendar.Business.Domains
{
  public class NotificationSenderDomain : INotificationSenderDomain
  {
    private readonly IOptions<EmailNotificationsOptions> _emailSenderOptions;
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly IEventRepository _eventRepository;

    public NotificationSenderDomain(
      IOptions<EmailNotificationsOptions> emailSenderOptions,
      IEventRepository eventRepository,
      IBackgroundJobClient backgroundJobClient
    )
    {
      _backgroundJobClient = backgroundJobClient;
      _emailSenderOptions = emailSenderOptions;
      _eventRepository = eventRepository;
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

      @event.NotificationScheduleJobId = _backgroundJobClient.Schedule<NotificationSenderDomain>(notificationSender 
        => notificationSender.NotifyEventStarted(eventId), @event.StartDateTime - TimeSpan.FromMinutes((int)@event.NotificationTime.GetValueOrDefault()));
      // - DateTimeOffset.Now.Offset, because date in database contains as local, but EF thinks that it is UTC

      _eventRepository.UpdateEvent(@event);
    }

    public void ScheduleEventSeriesStartedNotification(int seriesId)
      => _backgroundJobClient.Enqueue<NotificationSenderDomain>(notificationSender =>
        notificationSender.EventSeriesStartedNotification(seriesId));

    public void EventSeriesStartedNotification(int seriesId)
    {
      foreach (var @event in _eventRepository.GetSeries(seriesId))
      {
        ScheduleEventStartedNotification(@event.Id);
      }
    }

    public void CancelScheduledNotification(params Event[] events)
    {
      foreach (var jobId in events.Where(evt => evt.NotificationScheduleJobId != null).Select(evt => evt.NotificationScheduleJobId))
      {
        _backgroundJobClient.Delete(jobId);
      }
    }

    public void NotifyEventCreated(int eventId)
    {
      var eventNotificationInfo = _eventRepository.GetEventNotificationInfo(eventId);
      if (!eventNotificationInfo.UserWantsReceiveEmailNotifications) return;

      var notificationMessage = $@"
        Hello <i>{ eventNotificationInfo.UserFirstName },</i>
        <br>Event {(eventNotificationInfo.IsSeries ? "series" : "")}
        <b>{ eventNotificationInfo.EventName }</b> has been created successfully
        in calendar <b>{ eventNotificationInfo.CalendarName }</b>.
      ";

      SendEmail(eventNotificationInfo.UserEmail, notificationMessage);
    }

    public void NotifyEventStarted(int eventId)
    {
      var eventNotificationInfo = _eventRepository.GetEventNotificationInfo(eventId);
      if (!eventNotificationInfo.UserWantsReceiveEmailNotifications) return;

      var notificationMessage = $@"
        Hello <i>{ eventNotificationInfo.UserFirstName },</i>
        <br>Event <b>{ eventNotificationInfo.EventName }</b>
        in calendar <b>{ eventNotificationInfo.CalendarName }</b>
        will begin at <b>{ eventNotificationInfo.StartDateTime:g}</b>.
      ";

      SendEmail(eventNotificationInfo.UserEmail, notificationMessage);
    }

    public void NotifyEventDeleted(int eventId, bool isSeries)
    {
      var eventNotificationInfo = _eventRepository.GetEventNotificationInfo(eventId);
      if (!eventNotificationInfo.UserWantsReceiveEmailNotifications) return;
      
      var notificationMessage = $@"
        Hello <i>{ eventNotificationInfo.UserFirstName },</i>
        <br>Event {(isSeries ? "series" : "")} 
        <b>{ eventNotificationInfo.EventName }</b>
        in calendar <b>{ eventNotificationInfo.CalendarName }</b>
        has been deleted.
      ";

      SendEmail(eventNotificationInfo.UserEmail, notificationMessage);
    }

  }
}
