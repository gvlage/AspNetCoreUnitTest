using AutoMapper;
using AutoMapper.Configuration;
using Core.Application.Interfaces;
using Core.Application.Models;
using Core.Application.Notifications;
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace Core.Application.Services.Commands.Login
{
    public class LoginUserCommandHandler : IRequestHandler<UserForLoginDto, Response<UserLoggedDto>>
    {
        private readonly IDatingContext _context;
        private readonly IMediator _mediator;
        private readonly IConfiguration _config;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public LoginUserCommandHandler(IDatingContext context, IMediator mediator, IMapper mapper,
                                    IConfiguration config, UserManager<User> userManager)
        {
            _context = context;
            _mediator = mediator;
            _config = config;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<Response<UserLoggedDto>> Handle(UserForLoginDto request, CancellationToken cancellationToken)
        {
            var user = await _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.UserName == request.Username);

            if (user == null)
                throw new UnauthorizedAccessException("Login failed! User doesn't exist.");

            var result = await _userManager.CheckPasswordAsync(user, request.Password);

            if (result)
            {
                var userLogged = _mapper.Map<User, UserLoggedDto>(user);
                userLogged.Token = await GenerateJwtToken(user);

                return new Response<UserLoggedDto> { Data = userLogged };
            }

            throw new UnauthorizedAccessException("Login failed! Check user credentials and try again.");
        }

        private async Task<string> GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
            };

            var roles = await _userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenSecret = _config.GetSection("AppSettings:Token").Value;

            var key = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(tokenSecret));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Task.FromResult(tokenHandler.WriteToken(token)).Result;
        }
    }
}
