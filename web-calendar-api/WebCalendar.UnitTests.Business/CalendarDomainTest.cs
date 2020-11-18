using AutoMapper;
using Moq;
using System.Collections.Generic;
using System.Linq;
using WebCalendar.Business;
using WebCalendar.Business.Domains;
using WebCalendar.Business.Domains.Interfaces;
using WebCalendar.Business.Exceptions;
using WebCalendar.Business.ViewModels;
using WebCalendar.Data.Entities;
using WebCalendar.Data.Repositories.Interfaces;
using Xunit;

namespace WebCalendar.UnitTests.Business
{
  public class CalendarDomainTest
  {
    private readonly IMapper _mapper;

    public CalendarDomainTest()
    {
      var mapperConfiguration = new MapperConfiguration(mc => mc.AddProfile(new MappingProfile()));
      _mapper = mapperConfiguration.CreateMapper();
    }

    [Theory]
    [Trait("Domains", "CalendarDomain")]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(128)]
    [InlineData(int.MinValue)]
    public void GetCalendar_GettingUnknownCalendar_ShouldFail(int calendarId)
    {
      // Arrange
      var mockCalendarRepo = new Mock<ICalendarRepository>();
      mockCalendarRepo
        .Setup(x => x.GetCalendar(It.IsAny<int>()))
        .Returns(() => null);
      var calendarDomain = new CalendarDomain(mockCalendarRepo.Object, null, null, null);

      // Act
      Assert.Throws<NotFoundException>(() => calendarDomain.GetCalendar(calendarId));

      // Assert
      mockCalendarRepo.Verify(cr => cr.GetCalendar(calendarId), Times.Once());
    }

    [Theory]
    [Trait("Domains", "CalendarDomain")]
    [InlineData(int.MinValue)]
    [InlineData(66)]
    [InlineData(13)]
    public void GetCalendar_GettingCalendar_ShouldReturnCalendar(int calendarId)
    {
      // Arrange
      var expected = new Calendar();

      var mockCalendarRepo = new Mock<ICalendarRepository>();
      mockCalendarRepo
        .Setup(x => x.GetCalendar(It.IsAny<int>()))
        .Returns(() => expected);

      var calendarDomain = new CalendarDomain(mockCalendarRepo.Object, null, null, null);

      // Act
      var actual = calendarDomain.GetCalendar(calendarId);

      // Assert
      Assert.Equal(expected, actual);
      mockCalendarRepo.Verify(cr => cr.GetCalendar(calendarId), Times.Once());
    }

    [Theory]
    [Trait("Domains", "CalendarDomain")]
    [InlineData(3, 444)]
    [InlineData(int.MinValue, 0)]
    [InlineData(33, 22)]
    public void AddCalendar_AddingCalendarForInvalidUser_ShouldFail(int calendarUserId, int actualUserId)
    {
      // Arrange
      var calendarViewModel = new CalendarViewModel
      {
        UserId = calendarUserId
      };

      var mockCalendarRepo = new Mock<ICalendarRepository>();
      var calendarDomain = new CalendarDomain(mockCalendarRepo.Object, null, null, null);

      // Act
      Assert.Throws<ForbiddenException>(() => calendarDomain.AddCalendar(calendarViewModel, actualUserId));

      // Assert
      mockCalendarRepo.Verify(cr => cr.AddCalendar(It.IsAny<Calendar>()), Times.Never());
    }

    [Theory]
    [Trait("Domains", "CalendarDomain")]
    [InlineData(12, -12)]
    [InlineData(33, int.MinValue)]
    [InlineData(int.MaxValue, 0)]
    public void AddCalendar_AddingCalendar_ShouldReturnCalendarId(int userId, int calendarId)
    {
      // Arrange
      var calendarViewModel = new CalendarViewModel
      {
        UserId = userId
      };

      var mockCalendarRepo = new Mock<ICalendarRepository>();
      mockCalendarRepo
        .Setup(x => x.AddCalendar(It.IsAny<Calendar>()))
        .Returns(() => calendarId);

      var calendarDomain = new CalendarDomain(mockCalendarRepo.Object, _mapper, null, null);

      // Act
      var actualCalendarId = calendarDomain.AddCalendar(calendarViewModel, userId);

      // Assert
      Assert.Equal(calendarId, actualCalendarId);
      mockCalendarRepo.Verify(cr => cr.AddCalendar(It.IsNotNull<Calendar>()), Times.Once());
    }

    [Theory]
    [Trait("Domains", "CalendarDomain")]
    [InlineData(3, 2)]
    [InlineData(5, 6)]
    public void GetUserCalendars_GettingUserCalendars_ReturnUserCalendars(int userId, int expectedCalendarsCount)
    {
      var mockReturn = new List<Calendar>()
      {
        new Calendar
        {
          UserId = 1
        },
        new Calendar
        {
          UserId = 1
        },
      };
      for (int i = 0; i < expectedCalendarsCount; i++)
        mockReturn.Add(new Calendar { UserId = userId });

      var mockCalendarRepo = new Mock<ICalendarRepository>();
      mockCalendarRepo
        .Setup(cr => cr.GetUserCalendars(It.IsAny<int>()))
        .Returns(() => mockReturn.Where(calendar => calendar.UserId == userId));

      var calendarDomain = new CalendarDomain(mockCalendarRepo.Object, _mapper, null, null);

      // Act
      var actualCalendars = calendarDomain.GetUserCalendars(userId);

      // Assert
      Assert.Equal(expectedCalendarsCount, actualCalendars.Count());
      mockCalendarRepo.Verify(cr => cr.GetUserCalendars(It.IsNotNull<int>()), Times.Once());
    }

    [Theory]
    [Trait("Domains", "CalendarDomain")]
    [InlineData(3, 2, 5)]
    [InlineData(5, 6, 9)]
    public void DeleteCalendar_UserNotOwner_ThrowsForbiddenExceptions(int calendarId, int calendarUserId, int actualUserId)
    {
      var calendar = new Calendar
      {
        Id = calendarId,
        UserId = calendarUserId
      };

      var mockCalendarRepo = new Mock<ICalendarRepository>();
      mockCalendarRepo
        .Setup(cr => cr.GetCalendar(It.IsAny<int>()))
        .Returns(() => calendar);
      var calendarDomain = new CalendarDomain(mockCalendarRepo.Object, null, null, null);

      // Act
      Assert.Throws<ForbiddenException>(() => calendarDomain.DeleteCalendar(calendarId, actualUserId));

      // Assert
      mockCalendarRepo.Verify(cr => cr.GetCalendar(It.IsAny<int>()), Times.Once());
    }

    [Theory]
    [Trait("Domains", "CalendarDomain")]
    [InlineData(3, 2, 2)]
    [InlineData(5, 6, 6)]
    public void DeleteCalendar_CalendarOwner_ReturnTrue(int calendarId, int calendarUserId, int actualUserId)
    {
      var calendar = new Calendar
      {
        Id = calendarId,
        UserId = calendarUserId
      };

      var mockCalendarRepo = new Mock<ICalendarRepository>();
      mockCalendarRepo
        .Setup(cr => cr.GetCalendar(It.IsAny<int>()))
        .Returns(() => calendar);      
      mockCalendarRepo
        .Setup(cr => cr.DeleteCalendar(It.IsAny<int>()))
        .Returns(() => true);
      var calendarDomain = new CalendarDomain(mockCalendarRepo.Object, null, new Mock<INotificationSenderDomain>().Object, new Mock<IEventRepository>().Object);

      // Act
      Assert.True(calendarDomain.DeleteCalendar(calendarId, actualUserId));

      // Assert
      mockCalendarRepo.Verify(cr => cr.GetCalendar(It.IsAny<int>()), Times.Once());
      mockCalendarRepo.Verify(cr => cr.DeleteCalendar(It.IsAny<int>()), Times.Once());
    }

    [Theory]
    [Trait("Domains", "CalendarDomain")]
    [InlineData(3, 2, 5)]
    [InlineData(5, 6, 9)]
    public void EditCalendar_UserNotOwner_ThrowsForbiddenExceptions(int calendarId, int calendarUserId, int actualUserId)
    {
      var calendarVM = new CalendarViewModel
      {
        Id = calendarId,
        UserId = calendarUserId
      };

      var calendar = new Calendar
      {
        Id = calendarId,
        UserId = calendarUserId
      };

      var mockCalendarRepo = new Mock<ICalendarRepository>();
      mockCalendarRepo
        .Setup(cr => cr.GetCalendar(It.IsAny<int>()))
        .Returns(() => calendar);
      var calendarDomain = new CalendarDomain(mockCalendarRepo.Object, null, null, null);

      // Act
      Assert.Throws<ForbiddenException>(() => calendarDomain.EditCalendar(calendarVM, actualUserId));

      // Assert
      mockCalendarRepo.Verify(cr => cr.GetCalendar(It.IsAny<int>()), Times.Once());
    }

    [Theory]
    [Trait("Domains", "CalendarDomain")]
    [InlineData(3, 2, 2)]
    [InlineData(5, 6, 6)]
    public void EditCalendar_CalendarOwner_ReturnTrue(int calendarId, int calendarUserId, int actualUserId)
    {
      var calendarVM = new CalendarViewModel
      {
        Id = calendarId,
        UserId = calendarUserId
      };

      var calendar = new Calendar
      {
        Id = calendarId,
        UserId = calendarUserId
      };

      var mockCalendarRepo = new Mock<ICalendarRepository>();
      mockCalendarRepo
        .Setup(cr => cr.GetCalendar(It.IsAny<int>()))
        .Returns(() => calendar);      
      mockCalendarRepo
        .Setup(cr => cr.EditCalendar(It.IsAny<Calendar>()))
        .Returns(() => true);
      var calendarDomain = new CalendarDomain(mockCalendarRepo.Object, _mapper, null, null);

      // Act
      Assert.True(calendarDomain.EditCalendar(calendarVM, actualUserId));

      // Assert
      mockCalendarRepo.Verify(cr => cr.GetCalendar(It.IsAny<int>()), Times.Once());
      mockCalendarRepo.Verify(cr => cr.EditCalendar(It.IsAny<Calendar>()), Times.Once());
    }
  }
}
