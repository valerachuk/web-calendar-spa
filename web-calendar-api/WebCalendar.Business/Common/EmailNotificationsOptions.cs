namespace WebCalendar.Business.Common
{
  public class EmailNotificationsOptions
  {
    public string Host { get; set; }
    public int Port { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }
    public string EmailSubject { get; set; }
    public string SenderDisplayName { get; set; }
    public string HTMLEmailTemplatePath { get; set; }
  }

}
