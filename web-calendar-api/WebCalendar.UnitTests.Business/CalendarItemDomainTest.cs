using AutoMapper;
using Moq;
using System;
using System.Collections.Generic;
using WebCalendar.Business;
using WebCalendar.Business.Domains;
using WebCalendar.Business.Exceptions;
using WebCalendar.Business.ViewModels;
using WebCalendar.Data.Entities;
using WebCalendar.Data.Repositories.Interfaces;
using Xunit;

namespace WebCalendar.UnitTests.Business
{
  [Collection("Sequential")]
  public class CalendarItemDomainTest
  {
    private readonly IMapper _mapper;

    public CalendarItemDomainTest()
    {
      var mapperConfiguration = new MapperConfiguration(mc => mc.AddProfile(new MappingProfile()));
      _mapper = mapperConfiguration.CreateMapper();
    }

    [Fact]
    public void GetCalendarsItemsByTimeInterval_GettingCalendarItems_ShouldReturnItems()
    {
      // Arrange
      List<CalendarItemViewModel> actualVM = new List<CalendarItemViewModel>();
      List<Event> actual = new List<Event>();

      var mockEventRepo = new Mock<ICalendarItemRepository>();
      mockEventRepo
        .Setup(x => x.GetCalendarsEventsByTimeInterval(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int[]>()))
        .Returns(() => actual);

      var itemDomain = new CalendarItemDomain(mockEventRepo.Object, _mapper, null);

      DateTime start = new DateTime(2020, 4, 5);
      DateTime end = new DateTime(2020, 4, 6);
      int[] id = new int[] { 1, 2};

      // Act
      var expected = itemDomain.GetCalendarsItemsByTimeInterval(start, end, id);

      actualVM = _mapper.Map(actual, actualVM);

      // Assert
      Assert.Equal(expected, _mapper.Map<IEnumerable<Event>, IEnumerable<CalendarItemViewModel>>(actual));
    }

    [Fact]
    public void UpdateCalendarsItem_UpdateEvent_ShouldUpdateEvent()
    {
      //Arrange
      var actualItem = new CalendarItemViewModel()
      {
        MetaType = Constants.Enums.CalendarItemType.Event
      };
      var mockEventRepo = new Mock<ICalendarItemRepository>();
      var itemDomain = new CalendarItemDomain(mockEventRepo.Object, _mapper, null);

      // Act and Assert
      Assert.Throws<NotFoundException>(() => itemDomain.UpdateCalendarsItem(actualItem));
      mockEventRepo.Verify(cr => cr.UpdateCalendarsEventTime(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>()), Times.Once());
    }

    [Fact]
    public void UpdateCalendarsItem_UpdateNotExistingItem_ShouldThrowException()
    {
      //Arrange
      var actualItem = new CalendarItemViewModel()
      {
        MetaType = Constants.Enums.CalendarItemType.RepeatableEvent
      };
      var mockEventRepo = new Mock<ICalendarItemRepository>();
      var itemDomain = new CalendarItemDomain(mockEventRepo.Object, _mapper, null);

      // Act and Assert
      Assert.Throws<NotFoundException>(() => itemDomain.UpdateCalendarsItem(actualItem));
      mockEventRepo.Verify(cr => cr.UpdateCalendarsEventTime(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>()), Times.Once());
    }
  }
}
