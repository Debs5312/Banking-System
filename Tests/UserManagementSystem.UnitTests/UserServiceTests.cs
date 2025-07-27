using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using UserManagementSystem.Service;
using Models;
using Models.DTOs;
using Persistance;

namespace UserManagementSystem.UnitTests
{
    public class UserServiceTests : IDisposable
    {
        private readonly AppDBContext _dbContext;
        private readonly UserService _userService;
        private readonly Mock<ILogger<UserService>> _mockLogger;

        public UserServiceTests()
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            var options = new DbContextOptionsBuilder<AppDBContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .UseInternalServiceProvider(serviceProvider)
                .Options;
            _dbContext = new AppDBContext(options);

            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(c => c[It.Is<string>(s => s == "TokenKey")]).Returns("ThisIsA32ByteLongTokenKeyForTesting!");

            var tokenService = new TokenService(mockConfig.Object);
            _mockLogger = new Mock<ILogger<UserService>>();

            _userService = new UserService(_dbContext, tokenService, _mockLogger.Object);
        }

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [Fact]
        public async Task RegisterNewUser_Success()
        {
            var registrationDTO = new RegistrationDTO
            {
                UserName = "testuser",
                HashedPassword = "password123",
                Address = "123 Test St",
                AdharId = Guid.NewGuid().ToString()
            };

            var result = await _userService.RegisterNewUser(registrationDTO);

            Assert.Equal(RegistrationResult.Success, result);
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == "testuser");
            Assert.NotNull(user);
        }

        [Fact]
        public async Task RegisterNewUser_UserNameExists()
        {
            var existingUser = new User
            {
                UserName = "existinguser",
                Password = "hashedpassword",
                AdharId = Guid.NewGuid(),
                RegisteredDate = DateTime.UtcNow
            };
            await _dbContext.Users.AddAsync(existingUser);
            await _dbContext.SaveChangesAsync();

            var registrationDTO = new RegistrationDTO
            {
                UserName = "existinguser",
                HashedPassword = "password123",
                Address = "123 Test St",
                AdharId = Guid.NewGuid().ToString()
            };

            var result = await _userService.RegisterNewUser(registrationDTO);

            Assert.Equal(RegistrationResult.UserNameExists, result);
        }

        [Fact]
        public async Task RegisterNewUser_InvalidAdharId()
        {
            var registrationDTO = new RegistrationDTO
            {
                UserName = "newuser",
                HashedPassword = "password123",
                Address = "123 Test St",
                AdharId = "invalid-guid"
            };

            var result = await _userService.RegisterNewUser(registrationDTO);

            Assert.Equal(RegistrationResult.InvalidAdharId, result);
        }

        [Fact]
        public async Task RegisterNewUser_AdharIdInUse()
        {
            var adharId = Guid.NewGuid();
            var existingAdhar = new Adhar { Id = adharId };
            await _dbContext.Adhars.AddAsync(existingAdhar);
            await _dbContext.SaveChangesAsync();

            var registrationDTO = new RegistrationDTO
            {
                UserName = "newuser",
                HashedPassword = "password123",
                Address = "123 Test St",
                AdharId = adharId.ToString()
            };

            var result = await _userService.RegisterNewUser(registrationDTO);

            Assert.Equal(RegistrationResult.AdharIdInUse, result);
        }

        [Fact]
        public async Task LoginUser_Success()
        {
            var password = "password123";
            var hashedPassword = PasswordHashedService.Hash(password);
            var user = new User
            {
                UserName = "loginuser",
                Password = hashedPassword,
                AdharId = Guid.NewGuid(),
                RegisteredDate = DateTime.UtcNow
            };
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            var loginDTO = new LoginDTO
            {
                UserName = "loginuser",
                Password = password
            };

            var result = await _userService.LoginUser(loginDTO);

            Assert.True(result.LoggedIn);
            Assert.NotNull(result.Token);
            Assert.NotEmpty(result.Token);
            Assert.Equal("User is logged in successfully.", result.Message);
        }

        [Fact]
        public async Task LoginUser_Failure()
        {
            var loginDTO = new LoginDTO
            {
                UserName = "nonexistentuser",
                Password = "password123"
            };

            var result = await _userService.LoginUser(loginDTO);

            Assert.False(result.LoggedIn);
            Assert.Equal(string.Empty, result.Token);
            Assert.Equal("Invalid username or password.", result.Message);
        }

        [Fact]
        public async Task DeleteUser_Success()
        {
            var user = new User
            {
                UserName = "deleteuser",
                Password = "hashedpassword",
                AdharId = Guid.NewGuid(),
                RegisteredDate = DateTime.UtcNow
            };
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            var result = await _userService.DeleteUser(user.Id);

            Assert.True(result);
            var deletedUser = await _dbContext.Users.FindAsync(user.Id);
            Assert.Null(deletedUser);
        }

        [Fact]
        public async Task DeleteUser_NotFound()
        {
            var result = await _userService.DeleteUser(Guid.NewGuid());

            Assert.False(result);
        }

        [Fact]
        public async Task ReturnUsers_ReturnsList()
        {
            var user1 = new User
            {
                UserName = "user1",
                Password = "hashedpassword",
                AdharId = Guid.NewGuid(),
                RegisteredDate = DateTime.UtcNow
            };
            var user2 = new User
            {
                UserName = "user2",
                Password = "hashedpassword",
                AdharId = Guid.NewGuid(),
                RegisteredDate = DateTime.UtcNow
            };
            await _dbContext.Users.AddRangeAsync(user1, user2);
            await _dbContext.SaveChangesAsync();

            var users = await _userService.ReturnUsers();

            Assert.NotNull(users);
            Assert.Contains(users, u => u.UserName == "user1");
            Assert.Contains(users, u => u.UserName == "user2");
        }
    }
}
