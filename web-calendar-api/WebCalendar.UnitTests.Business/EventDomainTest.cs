using AutoMapper;
using Moq;
using Newtonsoft.Json;
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
  public class EventDomainTest
  {
    private readonly IMapper _mapper;

    public EventDomainTest()
    {
      var mapperConfiguration = new MapperConfiguration(mc => mc.AddProfile(new MappingProfile()));
      _mapper = mapperConfiguration.CreateMapper();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(128)]
    public void GetEvent_GettingEvent_ShouldReturnEvent(int id)
    {
      // Arrange
      EventViewModel actualVM = new EventViewModel();
      Event actual = new Event();

      var mockEventRepo = new Mock<IEventRepository>();
      mockEventRepo
        .Setup(x => x.GetEvent(It.IsAny<int>()))
        .Returns(() => actual);

      var eventDomain = new EventDomain(mockEventRepo.Object, _mapper, null);

      // Act
      EventViewModel expected = eventDomain.GetEvent(id);

      actualVM = _mapper.Map(actual, actualVM);

      // Assert
      var expectedJSON = JsonConvert.SerializeObject(expected);
      var actualJSON = JsonConvert.SerializeObject(actualVM);
      Assert.Equal(expectedJSON, actualJSON);
      mockEventRepo.Verify(cr => cr.GetEvent(id), Times.Once());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(128)]
    [InlineData(int.MinValue)]
    public void GetEvent_GettingNotExistingEvent_ShouldThrowEcxeption(int id)
    {
      //Arrange
      var mockEventRepo = new Mock<IEventRepository>();
      var eventDomain = new EventDomain(mockEventRepo.Object, null, null);

      // Act and Assert
      Assert.Throws<NotFoundException>(() => eventDomain.GetEvent(id));
    }

    [Fact]
    public void UpdateCalendarEvent_UpdateNotExistingEvent_ShouldThrowException()
    {
      //Arrange
      int userId = 0;
      EventViewModel actualVM = new EventViewModel();
      var mockEventRepo = new Mock<IEventRepository>();
      var eventDomain = new EventDomain(mockEventRepo.Object, null, null);

      // Act and Assert
      Assert.Throws<NotFoundException>(() => eventDomain.UpdateCalendarEvent(actualVM, userId));
      mockEventRepo.Verify(cr => cr.UpdateCalendarEvent(It.IsAny<Event>()), Times.Never());
    }

    [Fact]
    public void UpdateCalendarEventSeries_UpdateNotExistingEvents_ShouldThrowException()
    {
      //Arrange
      int userId = 0;
      EventViewModel actualVM = new EventViewModel();
      var mockEventRepo = new Mock<IEventRepository>();
      var eventDomain = new EventDomain(mockEventRepo.Object, null, null);

      // Act and Assert
      Assert.Throws<NotFoundException>(() => eventDomain.UpdateCalendarEventSeries(actualVM, userId));
      mockEventRepo.Verify(cr => cr.UpdateCalendarEventSeries(It.IsAny<Event>()), Times.Never());
    }

    [Fact]
    public void DeleteCalendarEvent_DeleteNotExistingEvent_ShouldThrowException()
    {
      //Arrange
      int userId = 0;
      EventViewModel actualVM = new EventViewModel();
      var mockEventRepo = new Mock<IEventRepository>();
      var eventDomain = new EventDomain(mockEventRepo.Object, null, null);

      // Act and Assert
      Assert.Throws<NotFoundException>(() => eventDomain.DeleteCalendarEvent(actualVM.Id, userId));
      }

    [Fact]
    public void DeleteCalendarEventSeries_DeleteNotExistingEvents_ShouldThrowException()
    {
      //Arrange
      int userId = 0;
      EventViewModel actualVM = new EventViewModel();
      var mockEventRepo = new Mock<IEventRepository>();
      var eventDomain = new EventDomain(mockEventRepo.Object, null, null);

      // Act and Assert
      Assert.Throws<NotFoundException>(() => eventDomain.DeleteCalendarEventSeries(actualVM.Id, userId));
      }
  }
}
