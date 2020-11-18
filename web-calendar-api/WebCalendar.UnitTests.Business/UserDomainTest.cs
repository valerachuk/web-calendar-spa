using AutoMapper;
using Microsoft.Extensions.Options;
using Moq;
using WebCalendar.Business;
using WebCalendar.Business.Common;
using WebCalendar.Business.Domains;
using WebCalendar.Business.Exceptions;
using WebCalendar.Business.ViewModels;
using WebCalendar.Data.Entities;
using WebCalendar.Data.Repositories.Interfaces;
using Xunit;

namespace WebCalendar.UnitTests.Business
{
  public class UserDomainTest
  {
    private readonly IMapper _mapper;

    public UserDomainTest()
    {
      var mapperConfiguration = new MapperConfiguration(mc => mc.AddProfile(new MappingProfile()));
      _mapper = mapperConfiguration.CreateMapper();
    }

    [Fact]
    public void Authenticate_GettingInvalidUser_ShouldReturnNull()
    {
      // Arrange
      var loginViewModel = new LoginViewModel
      {
        Email = "someEmail1"
      };

      var mockUserRepository = new Mock<IUserRepository>();
      mockUserRepository
        .Setup(ur => ur.GetByEmail(It.IsAny<string>()))
        .Returns(() => null);

      var userDomain = new UserDomain(mockUserRepository.Object, _mapper, null);

      int? expectedUserId = null;

      // Act
      var actualUserId = userDomain.Authenticate(loginViewModel);

      // Assert
      Assert.Equal(expectedUserId, actualUserId);
      mockUserRepository.Verify(cr => cr.GetByEmail(loginViewModel.Email), Times.Once());

    }

    [Fact]
    public void Authenticate_GettingUserWithInvalidPassword_ShouldReturnNull()
    {
      // Arrange
      var loginViewModel = new LoginViewModel
      {
        Email = "someEmail1",
        Password = "somePassword12834284"
      };

      var userEntity = new User
      {
        Salt = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 },
        PasswordHash = new byte[] { 4, 5, 6, 8, 1, 2, 3, 4, 5 }
      };

      var mockUserRepository = new Mock<IUserRepository>();
      mockUserRepository
        .Setup(ur => ur.GetByEmail(It.IsAny<string>()))
        .Returns(() => userEntity);

      var userDomain = new UserDomain(mockUserRepository.Object, _mapper, null);

      int? expectedUserId = null;

      // Act
      var actualUserId = userDomain.Authenticate(loginViewModel);

      // Assert
      Assert.Equal(expectedUserId, actualUserId);
      mockUserRepository.Verify(cr => cr.GetByEmail(loginViewModel.Email), Times.Once());

    }

    [Fact]
    public void Authenticate_GettingUserWithValidPassword_ShouldReturnUserId()
    {
      // Arrange
      var loginViewModel = new LoginViewModel
      {
        Email = "someEmail1",
        Password = "somePassword12834284"
      };

      var userEntity = new User
      {
        Salt = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 },
        PasswordHash = new byte[] { 206, 92, 109, 236, 43, 248, 59, 147, 31, 50, 17, 242, 57, 207, 198, 37, 71, 20, 163, 190 },
        Id = 12214
      };

      var mockUserRepository = new Mock<IUserRepository>();
      mockUserRepository
        .Setup(ur => ur.GetByEmail(It.IsAny<string>()))
        .Returns(() => userEntity);

      var userDomain = new UserDomain(mockUserRepository.Object, _mapper, null);

      // Act
      var actualUserId = userDomain.Authenticate(loginViewModel);

      // Assert
      Assert.Equal(userEntity.Id, actualUserId);
      mockUserRepository.Verify(cr => cr.GetByEmail(loginViewModel.Email), Times.Once());

    }

    [Fact]
    public void Register_RegisteredUser_ShouldBeAuthenticated()
    {
      // Arrange
      var registerViewModel = new RegisterViewModel
      {
        Password = "myPassword12138951895"
      };

      var loginViewModel = new LoginViewModel
      {
        Password = registerViewModel.Password
      };

      var expectedUserId = 123123123;

      User registeredUser = null;

      var mockUserRepository = new Mock<IUserRepository>();
      mockUserRepository
        .Setup(ur => ur.Create(It.IsAny<User>()))
        .Callback<User>(u =>
        {
          u.Id = expectedUserId;
          registeredUser = u;
        });

      mockUserRepository
        .Setup(ur => ur.GetByEmail(It.IsAny<string>()))
        .Returns(() => registeredUser);

      var authOptions = Options.Create(new AuthOptions { SaltSize = 4 });
      var userDomain = new UserDomain(mockUserRepository.Object, _mapper, authOptions);

      // Act
      var actualRegisteredUserId = userDomain.Register(registerViewModel);

      // Assert
      Assert.Equal(expectedUserId, actualRegisteredUserId);
      mockUserRepository.Verify(ur => ur.Create(It.IsNotNull<User>()), Times.Once());

      var authUserId = userDomain.Authenticate(loginViewModel);
      Assert.Equal(expectedUserId, authUserId);

    }

    [Fact]
    public void HasUser_CheckingExistingUser_ShouldReturnTrue()
    {
      // Arrange
      var email = "myEmail123";

      var mockUserRepository = new Mock<IUserRepository>();
      mockUserRepository
        .Setup(ur => ur.GetByEmail(It.IsAny<string>()))
        .Returns(() => new User());

      var userDomain = new UserDomain(mockUserRepository.Object, _mapper, null);

      // Act
      var expectedUserStatus = userDomain.HasUser(email);

      // Assert
      Assert.True(expectedUserStatus);
      mockUserRepository.Verify(ur => ur.GetByEmail(email), Times.Once());

    }

    [Fact]
    public void HasUser_CheckingNonExistentUser_ShouldReturnFalse()
    {
      // Arrange
      var email = "myEmail123";

      var mockUserRepository = new Mock<IUserRepository>();
      mockUserRepository
        .Setup(ur => ur.GetByEmail(It.IsAny<string>()))
        .Returns(() => null);

      var userDomain = new UserDomain(mockUserRepository.Object, _mapper, null);

      // Act
      var expectedUserStatus = userDomain.HasUser(email);

      // Assert
      Assert.False(expectedUserStatus);
      mockUserRepository.Verify(ur => ur.GetByEmail(email), Times.Once());

    }

    [Fact]
    public void GetUser_GettingInvalidUser_ShouldFail()
    {
      // Arrange
      var mockUserRepository = new Mock<IUserRepository>();
      mockUserRepository
        .Setup(ur => ur.GetUser(It.IsAny<int>()))
        .Returns(() => null);

      var userDomain = new UserDomain(mockUserRepository.Object, _mapper, null);

      const int userId = 12;

      // Act
      Assert.Throws<NotFoundException>(() => userDomain.GetUser(userId));

      // Assert
      mockUserRepository.Verify(ur => ur.GetUser(userId), Times.Once());
    }

    [Fact]
    public void GetUser_GettingValidUser_ShouldReturnViewModel()
    {
      // Arrange
      var mockUserRepository = new Mock<IUserRepository>();
      mockUserRepository
        .Setup(ur => ur.GetUser(It.IsAny<int>()))
        .Returns(() => new User());

      var userDomain = new UserDomain(mockUserRepository.Object, _mapper, null);

      const int userId = 6642;

      // Act
      var actualUserViewModel = userDomain.GetUser(userId);

      // Assert
      Assert.NotNull(actualUserViewModel);
      mockUserRepository.Verify(ur => ur.GetUser(userId), Times.Once());
    }

    [Theory]
    [InlineData(1, 23)]
    [InlineData(int.MinValue, int.MaxValue)]
    [InlineData(0, int.MinValue)]
    public void EditUser_EditingAnotherUser_ShouldFail(int modelId, int jwtId)
    {
      // Arrange
      var mockUserRepository = new Mock<IUserRepository>();
      var userDomain = new UserDomain(mockUserRepository.Object, _mapper, null);
      var userViewModel = new UserViewModel
      {
        Id = modelId
      };

      // Act
      Assert.Throws<ForbiddenException>(() => userDomain.EditUser(userViewModel, jwtId));

      // Assert
      mockUserRepository.Verify(ur => ur.Edit(It.IsNotNull<User>()), Times.Never());

    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(0, 0)]
    [InlineData(int.MaxValue, int.MaxValue)]
    [InlineData(int.MinValue, int.MinValue)]
    public void EditUser_EditingUser_ShouldNotFail(int modelId, int jwtId)
    {
      // Arrange
      var mockUserRepository = new Mock<IUserRepository>();
      var userDomain = new UserDomain(mockUserRepository.Object, _mapper, null);
      var userViewModel = new UserViewModel
      {
        Id = modelId
      };

      // Act
      userDomain.EditUser(userViewModel, jwtId);

      // Assert
      mockUserRepository.Verify(ur => ur.Edit(It.IsNotNull<User>()), Times.Once());

    }

  }
}
