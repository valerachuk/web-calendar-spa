using AutoMapper;
using Moq;
using WebCalendar.Business;
using WebCalendar.Business.Domains;
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

    [Fact]
    [Trait("Domains", "CalendarDomain")]
    public void DeleteCalendar_UserNotOwner_ThrowsForbiddenExceptions()
    {
      var calendar = new Calendar
      {
        Id = 3,
        UserId = 2
      };

      var mockCalendarRepo = new Mock<ICalendarRepository>();
      mockCalendarRepo
        .Setup(cr => cr.GetCalendar(It.IsAny<int>()))
        .Returns(() => calendar);
      var calendarDomain = new CalendarDomain(mockCalendarRepo.Object, null, null, null);

      Assert.Throws<ForbiddenException>(() => calendarDomain.DeleteCalendar(3, 5));

      mockCalendarRepo.Verify(cr => cr.GetCalendar(It.IsAny<int>()), Times.Once());
    }

    [Fact]
    [Trait("Domains", "CalendarDomain")]
    public void EditCalendar_UserNotOwner_ThrowsForbiddenExceptions()
    {
      var calendarVM = new CalendarViewModel
      {
        Id = 3,
        UserId = 2
      };

      var calendar = new Calendar
      {
        Id = 3,
        UserId = 2
      };

      var mockCalendarRepo = new Mock<ICalendarRepository>();
      mockCalendarRepo
        .Setup(cr => cr.GetCalendar(It.IsAny<int>()))
        .Returns(() => calendar);
      var calendarDomain = new CalendarDomain(mockCalendarRepo.Object, null, null, null);

      Assert.Throws<ForbiddenException>(() => calendarDomain.EditCalendar(calendarVM, 5));

      mockCalendarRepo.Verify(cr => cr.GetCalendar(It.IsAny<int>()), Times.Once());
    }
  }
}
