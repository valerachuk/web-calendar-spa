using AutoMapper;
using web_calendar_business.ViewModels;
using web_calendar_data.Entities;

namespace web_calendar_business
{
  public class MappingProfile : Profile
  {
    public MappingProfile()
    {
      CreateMap<User, UserViewModel>();
    }
  }
}
