using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WebCalendar.Business.Common;
using WebCalendar.Business.Domains.Interfaces;
using WebCalendar.Business.ViewModels;
using WebCalendar.Data.Entities;
using WebCalendar.Data.Repositories.Interfaces;
using WebCalendar.Business.Exceptions;

namespace WebCalendar.Business.Domains
{
  public class UserDomain : IUserDomain
  {
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IOptions<AuthOptions> _authOptions;
    private readonly SHA1 _sha1 = SHA1.Create();
    private readonly RNGCryptoServiceProvider _secureRandom = new RNGCryptoServiceProvider();

    public UserDomain(IUserRepository userRepository, IMapper mapper, IOptions<AuthOptions> authOptions)
    {
      _userRepository = userRepository;
      _mapper = mapper;
      _authOptions = authOptions;
    }

    public int? Authenticate(LoginViewModel login)
    {
      var user = _userRepository.GetByEmail(login.Email);
      if (user == null) return null;

      var isPasswordValid = ComputePasswordHash(user.Salt, login.Password)
        .SequenceEqual(user.PasswordHash);

      return isPasswordValid ? user.Id : (int?)null;
    }

    public int Register(RegisterViewModel register)
    {
      var user = _mapper.Map<User>(register);
      user.Salt = CreateSalt();
      user.PasswordHash = ComputePasswordHash(user.Salt, register.Password);

      _userRepository.Create(user);
      return user.Id;
    }

    public bool HasUser(string email) => _userRepository.GetByEmail(email) != null;

    private byte[] CreateSalt()
    {
      var salt = new byte[_authOptions.Value.SaltSize];
      _secureRandom.GetBytes(salt);
      return salt;
    }

    private byte[] ComputePasswordHash(IEnumerable<byte> salt, string password)
    {
      return _sha1.ComputeHash(salt.Concat(Encoding.UTF8.GetBytes(password)).ToArray());
    }

    public string GenerateJWT(int userId)
    {
      var user = GetUser(userId);
      var authOptions = _authOptions.Value;

      var securityKey = authOptions.SymmetricSecurityKey;
      var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

      var token = new JwtSecurityToken(
        authOptions.Issuer,
        authOptions.Audience,
        new[]
        {
          new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
          new Claim("firstName", user.FirstName),
          new Claim("lastName", user.LastName),
          new Claim("email", user.Email),
          new Claim("notifications", user.ReceiveEmailNotifications.ToString()),
        },
        expires: DateTime.Now.AddSeconds(authOptions.TokenLifetime),
        signingCredentials: credentials);

      return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public UserViewModel GetUser(int id) {
      var user = _userRepository.GetUser(id);
      if (user == null)
        throw new NotFoundException("User not found");

      return _mapper.Map<User, UserViewModel>(user);
    }

    public IEnumerable<UserViewModel> GetAllUsersExceptCurrent(int id)
    {
      return _mapper.Map<IEnumerable<User>, IEnumerable<UserViewModel>>(_userRepository.GetUsersExceptCurrent(id));
    }

    public bool EditUser(UserViewModel userView, int authedUserId)
    {
      if (userView.Id != authedUserId)
        throw new ForbiddenException("Unauthorized");

      return _userRepository.Edit(_mapper.Map<UserViewModel, User>(userView));
    }
  }
}
