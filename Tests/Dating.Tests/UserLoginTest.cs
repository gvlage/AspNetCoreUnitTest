using Api;
using AutoMapper;
using Core.Application.Models;
using Core.Application.Services.Commands.Login;
using Core.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Dating.Tests
{
    public class UserLoginTest : IClassFixture<TestFixture<Startup>>, IDisposable
    {
        private readonly TestFixture<Startup> _fixture;
        public readonly DatingDbContext _context;
        public readonly IMapper _mapper;
        public readonly IConfiguration _configuration;

        public UserLoginTest(TestFixture<Startup> fixture)
        {
            _fixture = fixture;
            _context = _fixture.CreateDbContext();
            _mapper = (IMapper)_fixture.Server.Host.Services.GetService(typeof(IMapper));
            _configuration = (IConfiguration)_fixture.Server.Host.Services.GetService(typeof(IConfiguration));
        }

        [Theory]
        [Trait("User", "Login_Successful")]
        [InlineData("userTest", "12345678")]
        public async void Login_Successful(string userName, string password)
        {
            //ARRANGE
            var fakeUserManager = new Mock<FakeUserManager>();
            _context.Users.AddRange(GetFakeData().AsQueryable());
            _context.SaveChanges();

            var userToCheck = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);

            fakeUserManager.Setup(x => x.Users)
                .Returns(_context.Users);

            fakeUserManager.Setup(x => x.CheckPasswordAsync(userToCheck, "12345678"))
                .ReturnsAsync(true);
            fakeUserManager.Setup(x => x.GetRolesAsync(userToCheck)).ReturnsAsync(new List<string> { "Admin", "Member", "Moderator" });

            var mediator = new Mock<IMediator>();

            LoginUserCommandHandler loginHandler = new LoginUserCommandHandler(_context, mediator.Object, _mapper, _configuration, fakeUserManager.Object);

            //ACT
            var result = await loginHandler.Handle(new UserForLoginDto { Username = userName, Password = password }, new System.Threading.CancellationToken());

            //ASSERT
            Assert.NotNull(result.Data.Token);
        }

        [Theory]
        [Trait("User", "Login_UnauthorizedAccess")]
        [InlineData("userTest", "12345345")]
        [InlineData("test", "12345678")]
        [InlineData("", "")]
        public async void Login_UnauthorizedAccess(string userName, string password)
        {
            //ARRANGE
            var fakeUserManager = new Mock<FakeUserManager>();
            _context.Users.AddRange(GetFakeData().AsQueryable());
            _context.SaveChanges();
            var userToCheck = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);

            fakeUserManager.Setup(x => x.Users)
                .Returns(_context.Users);

            fakeUserManager.Setup(x => x.CheckPasswordAsync(userToCheck, "12345678"))
                .ReturnsAsync(true);
            fakeUserManager.Setup(x => x.GetRolesAsync(userToCheck)).ReturnsAsync(new List<string> { "Admin", "Member", "Moderator" });

            var mediator = new Mock<IMediator>();

            LoginUserCommandHandler loginHandler = new LoginUserCommandHandler(_context, mediator.Object, _mapper,
                                                                                    _configuration, fakeUserManager.Object);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => loginHandler.Handle(new UserForLoginDto { Username = userName, Password = password }, new System.Threading.CancellationToken()));
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        #region Privates       

        private List<User> GetFakeData()
        {
            var users = new List<User>
            {
                new User { Id = 1, UserName = "userTest", PasswordHash = "12345678", Created = DateTime.Now, Active = true }
            };

            return users;
        }

        #endregion
    }
}
