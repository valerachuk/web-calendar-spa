using System.Collections.Generic;
using AutoMapper;
using web_calendar_business.ViewModels;
using web_calendar_data.Entities;
using web_calendar_data.Repositories;

namespace web_calendar_business.Domains
{
  public interface IUserDomain
  {
    IEnumerable<UserViewModel> Get();
  }

  public class UserDomain : IUserDomain
  {
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserDomain(IUserRepository userRepository, IMapper mapper)
    {
      _userRepository = userRepository;
      _mapper = mapper;
    }

    public IEnumerable<UserViewModel> Get() =>
      _mapper.Map<IEnumerable<User>, IEnumerable<UserViewModel>>(_userRepository.Get());
  }
}
