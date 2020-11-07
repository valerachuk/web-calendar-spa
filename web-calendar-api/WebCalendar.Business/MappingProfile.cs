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
      _ = ev.Reiteration == null ? 
      it.MetaType = Constants.Enums.CalendarItemType.Event : 
      it.MetaType = Constants.Enums.CalendarItemType.RepeatableEvent
      );
    }
  }
}
