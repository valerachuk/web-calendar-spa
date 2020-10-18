using System.Collections.Generic;
using AutoMapper;
using WebCalendar.Business.Domains.Interfaces;
using WebCalendar.Business.ViewModels;
using WebCalendar.Data.Entities;
using WebCalendar.Data.Repositories;
using WebCalendar.Data.Repositories.Interfaces;

namespace WebCalendar.Business.Domains
{
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
