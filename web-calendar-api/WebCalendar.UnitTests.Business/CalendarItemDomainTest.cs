using AutoMapper;
using Moq;
using System;
using System.Collections.Generic;
using WebCalendar.Business;
using WebCalendar.Business.Domains;
using WebCalendar.Business.Exceptions;
using WebCalendar.Business.ViewModels;
using WebCalendar.Constants.Enums;
using WebCalendar.Data.Entities;
using WebCalendar.Data.Repositories.Interfaces;
using Xunit;

namespace WebCalendar.UnitTests.Business
{
  public class CalendarItemDomainTest
  {
    private readonly IMapper _mapper;

    public CalendarItemDomainTest()
    {
      var mapperConfiguration = new MapperConfiguration(mc => mc.AddProfile(new MappingProfile()));
      _mapper = mapperConfiguration.CreateMapper();
    }

    [Fact]
    public void GetCalendarsItemsByTimeInterval_GettingCalendarItems_ShouldCallMethod()
    {
      // Arrange
      List<Event> expected = new List<Event>();

      var mockEventRepo = new Mock<ICalendarItemRepository>();
      var itemDomain = new CalendarItemDomain(mockEventRepo.Object, _mapper, null, null);

      DateTime start = new DateTime(2020, 4, 5);
      DateTime end = new DateTime(2020, 4, 6);
      int[] id = new int[] { 1, 2 };

      mockEventRepo
        .Setup(x => x.GetCalendarsEventsByTimeInterval(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int[]>()))
        .Returns(() => expected);

      // Act
      var actual = itemDomain.GetCalendarsItemsByTimeInterval(start, end, id, 123);

      // Assert
      mockEventRepo.Verify(item => item.GetCalendarsEventsByTimeInterval(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int[]>()), Times.Once());

    }

    [Theory]
    [InlineData(CalendarItemType.Event)]
    [InlineData(CalendarItemType.RepeatableEvent)]
    public void UpdateCalendarsItem_UpdateNotExistingEvent_ShouldThrowException(CalendarItemType type)
    {
      //Arrange
      var actualUpdatedEvent = new Event()
      {
        StartDateTime = new DateTime(2020, 4, 5),
        EndDateTime = new DateTime(2020, 4, 6)
      };
      var actualItem = new CalendarItemViewModel()
      {
        MetaType = type
      };
      var mockItemRepo = new Mock<ICalendarItemRepository>();
      var mockEventRepo = new Mock<IEventRepository>();
      var itemDomain = new CalendarItemDomain(mockItemRepo.Object, _mapper, null, mockEventRepo.Object);
      mockEventRepo.Setup(x => x.UpdateCalendarEvent(actualUpdatedEvent)).Verifiable();

      // Act and Assert
      Assert.Throws<NotFoundException>(() => itemDomain.UpdateCalendarsItem(actualItem));
      mockEventRepo.Verify(item => item.UpdateCalendarEvent(actualUpdatedEvent), Times.Never());
    }
  }
}
