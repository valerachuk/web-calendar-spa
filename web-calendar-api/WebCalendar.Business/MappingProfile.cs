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


      //CreateMap<Event, EventViewModel>()
      //  .ForMember(ev => ev.Guests, opt => opt.MapFrom(x => x.Guests.Select(y => y.User).ToList()));
      //CreateMap<EventViewModel, Event>()
      //  .ForMember(ev => ev.Guests, opt => opt.MapFrom(x => x.Guests));


      CreateMap<Event, EventViewModel>()
        .ForMember(d => d.Guests, opt => opt.MapFrom(s => s.Guests.Select(x => x.User)));

      CreateMap<EventGuests, UserViewModel>()
              .ForMember(d => d.Id, opt => opt.MapFrom(s => s.UserId));

      CreateMap<EventViewModel, Event>()
            .AfterMap((s, d) =>
            {
              foreach (var guest in d.Guests)
              {
                guest.EventId = s.Id;
              }
            });

      CreateMap<UserViewModel, EventGuests>()
            .ForMember(d => d.UserId, opt => opt.MapFrom(s => s.Id));

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
