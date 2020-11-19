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
      CreateMap<Event, CalendarItemViewModel>().BeforeMap((ev, it) =>
      it.MetaType = ev.Reiteration == null ? 
      Constants.Enums.CalendarItemType.Event : 
      Constants.Enums.CalendarItemType.RepeatableEvent
      );
    }
  }
}
