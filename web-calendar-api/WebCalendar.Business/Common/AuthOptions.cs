using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace WebCalendar.Business.Common
{
  public class AuthOptions
  {
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public string Secret { get; set; }
    public int TokenLifetime { get; set; } //seconds
    public int SaltSize { get; set; }

    public SymmetricSecurityKey SymmetricSecurityKey => new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Secret));
  }
}
