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
  public class EventDomainTest
  {
    private readonly IMapper _mapper;

    public EventDomainTest()
    {
      var mapperConfiguration = new MapperConfiguration(mc => mc.AddProfile(new MappingProfile()));
      _mapper = mapperConfiguration.CreateMapper();
    }


    [Fact]
    public void GetEvent_GettingNotExistingEvent_ShouldThrowException()
    {
      //Arrange
      var mockEventRepo = new Mock<IEventRepository>();
      var eventDomain = new EventDomain(mockEventRepo.Object, null, null);
      mockEventRepo
        .Setup(evrep => evrep.GetEvent(It.IsAny<int>()))
        .Returns(() => null);

      // Act and Assert
      Assert.Throws<NotFoundException>(() => eventDomain.GetEvent(123));
      mockEventRepo.Verify(evrep => evrep.GetEvent(It.IsAny<int>()), Times.Once());
    }

    [Fact]
    public void UpdateCalendarEvent_UpdateNotExistingEvent_ShouldThrowException()
    {
      //Arrange
      int userId = 0;
      EventViewModel expectedVM = new EventViewModel();
      var mockEventRepo = new Mock<IEventRepository>();
      mockEventRepo
        .Setup(evrep => evrep.UpdateCalendarEvent(It.IsAny<Event>()))
        .Verifiable();
      var eventDomain = new EventDomain(mockEventRepo.Object, null, null);

      // Act and Assert
      Assert.Throws<NotFoundException>(() => eventDomain.UpdateCalendarEvent(expectedVM, userId));
      mockEventRepo.Verify(evrep => evrep.UpdateCalendarEvent(It.IsAny<Event>()), Times.Never());
    }

    [Fact]
    public void UpdateCalendarEventSeries_UpdateNotExistingEvents_ShouldThrowException()
    {
      //Arrange
      int userId = 0;
      EventViewModel actualVM = new EventViewModel();
      var mockEventRepo = new Mock<IEventRepository>();
      mockEventRepo
        .Setup(x => x.UpdateCalendarEventSeries(It.IsAny<Event>()))
        .Verifiable();
      var eventDomain = new EventDomain(mockEventRepo.Object, null, null);

      // Act and Assert
      Assert.Throws<NotFoundException>(() => eventDomain.UpdateCalendarEventSeries(actualVM, userId));
      mockEventRepo.Verify(evrep => evrep.UpdateCalendarEventSeries(It.IsAny<Event>()), Times.Never());
    }

    [Fact]
    public void DeleteCalendarEvent_DeleteNotExistingEvent_ShouldThrowException()
    {
      //Arrange
      int userId = 0;
      EventViewModel actualVM = new EventViewModel();
      var mockEventRepo = new Mock<IEventRepository>();
      mockEventRepo
        .Setup(evrep => evrep.DeleteCalendarEvent(It.IsAny<int>()))
        .Returns(() => null)
        .Verifiable();
      var eventDomain = new EventDomain(mockEventRepo.Object, null, null);

      // Act and Assert
      Assert.Throws<NotFoundException>(() => eventDomain.DeleteCalendarEvent(actualVM.Id, userId));
      mockEventRepo.Verify(evrep => evrep.DeleteCalendarEvent(It.IsAny<int>()), Times.Never());
    }

    [Fact]
    public void DeleteCalendarEventSeries_DeleteNotExistingEvents_ShouldThrowException()
    {
      //Arrange
      int userId = 0;
      EventViewModel actualVM = new EventViewModel();
      var mockEventRepo = new Mock<IEventRepository>();
      mockEventRepo
        .Setup(evrep => evrep.DeleteCalendarEventSeries(It.IsAny<int>()))
        .Returns(() => null)
        .Verifiable();
      var eventDomain = new EventDomain(mockEventRepo.Object, null, null);

      // Act and Assert
      Assert.Throws<NotFoundException>(() => eventDomain.DeleteCalendarEventSeries(actualVM.Id, userId));
      mockEventRepo.Verify(evrep => evrep.DeleteCalendarEventSeries(It.IsAny<int>()), Times.Never());
    }
  }
}
