using AutoMapper;
using System.Linq;
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
      CreateMap<UserFile, FileViewModel>().ReverseMap();

      CreateMap<Event, EventViewModel>()
        .ForMember(e => e.Guests, opt => opt.MapFrom(s => s.Guests.Select(g => g.User)));

      CreateMap<EventGuests, UserViewModel>()
              .ForMember(e => e.Id, opt => opt.MapFrom(g => g.UserId));

      CreateMap<EventViewModel, Event>()
            .AfterMap((evm, e) =>
            {
              foreach (var guest in e.Guests)
              {
                guest.EventId = evm.Id;
              }
            });

      CreateMap<UserViewModel, EventGuests>()
            .ForMember(g => g.UserId, opt => opt.MapFrom(uvm => uvm.Id));

      CreateMap<Event, CalendarItemViewModel>().BeforeMap((ev, it) =>
        it.MetaType = 
        ev.Guests.Count == 0 ?
          (ev.Reiteration == null ?
          Constants.Enums.CalendarItemType.Event :
          Constants.Enums.CalendarItemType.RepeatableEvent) 
          :
          (ev.Reiteration == null ?
          Constants.Enums.CalendarItemType.SharedEvent :
          Constants.Enums.CalendarItemType.SharedRepeatableEvent)
      );
    }
  }
}
