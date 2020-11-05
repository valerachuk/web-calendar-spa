using AutoMapper;
using WebCalendar.Business.ViewModels;
using WebCalendar.Data.Entities;

namespace WebCalendar.Business
{
  public class MappingProfile : Profile
  {
    public MappingProfile()
    {
      CreateMap<User, UserViewModel>().ReverseMap();
      CreateMap<RegisterViewModel, User>();
      CreateMap<Calendar, CalendarViewModel>().ReverseMap();
      CreateMap<Event, EventViewModel>().ReverseMap();
      CreateMap<Event, CalendarItemViewModel>();
    }
  }
}
