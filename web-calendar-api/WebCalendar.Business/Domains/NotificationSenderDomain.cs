using System.IO;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using WebCalendar.Business.Common;
using WebCalendar.Business.Domains.Interfaces;
using WebCalendar.Data.Repositories.Interfaces;

namespace WebCalendar.Business.Domains
{
  public class NotificationSenderDomain : INotificationSenderDomain
  {
    private readonly IOptions<EmailNotificationsOptions> _emailSenderOptions;
    private readonly IUserRepository _userRepository;

    public NotificationSenderDomain(
      IOptions<EmailNotificationsOptions> emailSenderOptions,
      IUserRepository userRepository
      )
    {
      _userRepository = userRepository;
      _emailSenderOptions = emailSenderOptions;
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
        Body = emailTemplate.Replace("__CONTENT__", message),
        Subject = emailSenderOptions.EmailSubject,
        IsBodyHtml = true
      };

      client.Send(mailMessage);
    }

    public void NotifyEventCreated(string eventName, int userId)
    {
      var user = _userRepository.GetUser(userId);
      if (!user.ReceiveEmailNotifications) return;

      var notificationMessage = $@"
        Hello <i>{ user.FirstName },</i>
        <br>Event <b>{ eventName }</b> has been successfully created.
      ";

      SendEmail(user.Email, notificationMessage);
    }
  }
}
