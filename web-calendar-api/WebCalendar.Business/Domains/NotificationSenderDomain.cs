using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using Hangfire;
using Microsoft.Extensions.Options;
using NLog;
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
    private readonly ILogger _logger;

    public NotificationSenderDomain(
      IOptions<EmailNotificationsOptions> emailSenderOptions,
      IEventRepository eventRepository,
      IBackgroundJobClient backgroundJobClient
    )
    {
      _backgroundJobClient = backgroundJobClient;
      _emailSenderOptions = emailSenderOptions;
      _eventRepository = eventRepository;
      _logger = LogManager.GetCurrentClassLogger();
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
        _logger.Warn($"Cannot send email to {recipientEmail} cause: {e.Message}");
      }
    }

    public void ScheduleEventCreatedNotification(int eventId)
      => _backgroundJobClient.Enqueue<NotificationSenderDomain>(notificationSender => notificationSender.NotifyEventCreated(eventId));

    public void ScheduleEventEditedNotification(int eventId)
      => _backgroundJobClient.Enqueue<NotificationSenderDomain>(notificationSender => notificationSender.NotifyEventEdited(eventId));

    public void ScheduleEventStartedNotification(int eventId)
    {
      var @event = _eventRepository.GetEvent(eventId);

      if (@event.StartDateTime < DateTime.Now)
        return;

      var notifyTime = @event.StartDateTime - TimeSpan.FromMinutes((int)@event.NotificationTime.GetValueOrDefault());
      var notifyTimeUTC = DateTime.SpecifyKind(notifyTime, DateTimeKind.Local);

      @event.NotificationScheduleJobId = _backgroundJobClient.Schedule<NotificationSenderDomain>(notificationSender
        => notificationSender.NotifyEventStarted(eventId), notifyTimeUTC);

      _eventRepository.UpdateEventStartedNotification(@event);
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
      if (eventNotificationInfo != null)
      {
        if (eventNotificationInfo == null && !eventNotificationInfo.UserWantsReceiveEmailNotifications) return;

        var notificationMessage = $@"
        Hello <i>{ eventNotificationInfo.UserFirstName },</i>
        <br>Event {(eventNotificationInfo.IsSeries ? "series" : "")}
        <b>{ eventNotificationInfo.EventName }</b> has been created successfully
        in calendar <b>{ eventNotificationInfo.CalendarName }</b>.
      ";

        SendEmail(eventNotificationInfo.UserEmail, notificationMessage);
        foreach (var guest in eventNotificationInfo.Guests)
        {
          notificationMessage = $@"
        Hello <i>{ guest.User.FirstName },</i>
        <br>Event {(eventNotificationInfo.IsSeries ? "series" : "")}
        <b>{ eventNotificationInfo.EventName }</b> has been created successfully in your default calendar.
        You were invited by <b>{eventNotificationInfo.UserFirstName}</b> ({eventNotificationInfo.UserEmail})";
          SendEmail(guest.User.Email, notificationMessage);
        }
      }
    }

    public void NotifyEventEdited(int eventId)
    {
      var eventNotificationInfo = _eventRepository.GetEventNotificationInfo(eventId);
      if (eventNotificationInfo != null)
      {
        if (eventNotificationInfo == null && !eventNotificationInfo.UserWantsReceiveEmailNotifications) return;

        var notificationMessage = $@"
        Hello <i>{ eventNotificationInfo.UserFirstName },</i>
        <br>Event {(eventNotificationInfo.IsSeries ? "series" : "")}
        <b>{ eventNotificationInfo.EventName }</b> has been edited successfully
        in calendar <b>{ eventNotificationInfo.CalendarName }</b>.
      ";

        SendEmail(eventNotificationInfo.UserEmail, notificationMessage);
        foreach (var guest in eventNotificationInfo.Guests)
        {
          notificationMessage = $@"
        Hello <i>{ guest.User.FirstName },</i>
        <br>Event {(eventNotificationInfo.IsSeries ? "series" : "")}
        <b>{ eventNotificationInfo.EventName }</b> has been edited successfully in your default calendar by
        <b>{eventNotificationInfo.UserFirstName}</b> ({eventNotificationInfo.UserEmail})";
          SendEmail(guest.User.Email, notificationMessage);
        }
      }
    }

    public void NotifyEventStarted(int eventId)
    {
      var eventNotificationInfo = _eventRepository.GetEventNotificationInfo(eventId);
      if (eventNotificationInfo != null)
      {
        if (!eventNotificationInfo.UserWantsReceiveEmailNotifications) return;

        var notificationMessage = $@"
        Hello <i>{ eventNotificationInfo.UserFirstName },</i>
        <br>Event <b>{ eventNotificationInfo.EventName }</b>
        in calendar <b>{ eventNotificationInfo.CalendarName }</b>
        will begin at <b>{ eventNotificationInfo.StartDateTime:g}</b>.
      ";

        SendEmail(eventNotificationInfo.UserEmail, notificationMessage);
        foreach (var guest in eventNotificationInfo.Guests)
        {
          notificationMessage = $@"
        Hello <i>{ guest.User.FirstName },</i>
        <br>Event <b>{ eventNotificationInfo.EventName }</b>
        in calendar <b>{ eventNotificationInfo.CalendarName }</b>
        will begin at <b>{ eventNotificationInfo.StartDateTime:g}</b>.
        { eventNotificationInfo.UserFirstName } is looking forward to you!";
          SendEmail(guest.User.Email, notificationMessage);
        }
      }
    }
    public void NotifyEventDeleted(int eventId, bool isSeries)
    {
      var eventNotificationInfo = _eventRepository.GetEventNotificationInfo(eventId);
      if (eventNotificationInfo != null)
      {
        if (eventNotificationInfo == null && !eventNotificationInfo.UserWantsReceiveEmailNotifications) return;

        var notificationMessage = $@"
        Hello <i>{ eventNotificationInfo.UserFirstName },</i>
        <br>Event {(isSeries ? "series" : "")} 
        <b>{ eventNotificationInfo.EventName }</b>
        in calendar <b>{ eventNotificationInfo.CalendarName }</b>
        has been deleted.
      ";

        SendEmail(eventNotificationInfo.UserEmail, notificationMessage);
        foreach (var guest in eventNotificationInfo.Guests)
        {
          notificationMessage = $@"
        Hello <i>{ guest.User.FirstName },</i>
        <br>Event {(eventNotificationInfo.IsSeries ? "series" : "")}
        <b>{ eventNotificationInfo.EventName }</b> has been deleted from your default calendar by
        <b>{eventNotificationInfo.UserFirstName}</b> ({eventNotificationInfo.UserEmail})";
          SendEmail(guest.User.Email, notificationMessage);
        }
      }
    }
  }
}
