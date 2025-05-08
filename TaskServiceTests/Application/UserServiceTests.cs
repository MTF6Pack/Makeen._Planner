//using Application.Contracts;
//using Application.Contracts.Users.Commands;
//using Application.DataSeeder;
//using Application.UserAndOtp.Services;
//using Domain;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.Extensions.Configuration;
//using Moq;
//using Xunit;
//using Task = System.Threading.Tasks.Task;

//namespace MyAwesomeProjectTests.Application
//{
//    public class UserServiceTests
//    {
//        private readonly Mock<UserManager<User>> _userManagerMock;
//        private readonly Mock<IEmailConfirmService> _emailConfirmServiceMock;
//        private readonly Mock<IFileStorageService> _fileStorageServiceMock;
//        private readonly JwtTokenService _jwtTokenService;
//        private readonly UserService _userService;

//        public UserServiceTests()
//        {
//            _userManagerMock = CreateUserManagerMock();
//            _emailConfirmServiceMock = new Mock<IEmailConfirmService>();
//            _fileStorageServiceMock = new Mock<IFileStorageService>();

//            var inMemorySettings = new Dictionary<string, string> { { "JWT:Key", "dummy-secret-key" } };
//            IConfiguration configuration = new ConfigurationBuilder()
//                .AddInMemoryCollection(inMemorySettings!).Build();
//            _jwtTokenService = new JwtTokenService(configuration);

//            _userService = new UserService(
//                _userManagerMock.Object,
//                _jwtTokenService,
//                _emailConfirmServiceMock.Object,
//                _fileStorageServiceMock.Object
//            );
//        }

//        [Fact]
//        public async Task SignUp_ShouldCreateUserAndSendConfirmation_WhenUserDoesNotExist()
//        {
//            var addUserCommand = new AddUserCommand
//            {
//                Email = "test@example.com",
//                Password = "P@ssw0rd",
//                ConfirmPassword = "P@ssw0rd"
//            };

//            // Mock the behavior of FindByEmailAsync to return null (user does not exist)
//            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((User)null!);

//            // Mock CreateAsync to return success
//            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

//            // Mock GenerateEmailConfirmationTokenAsync to return a dummy token
//            _userManagerMock.Setup(x => x.GenerateEmailConfirmationTokenAsync(It.IsAny<User>())).ReturnsAsync("dummy-token");

//            // Mock SendConfirmEmailAsync to simulate sending an email
//            _emailConfirmServiceMock.Setup(x => x.SendConfirmEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

//            // Act
//            var result = await _userService.SignUP(addUserCommand);

//            // Assert
//            Assert.Equal("User registered successfully. Please check your email to confirm your account.", result);
//        }

//        // Other tests...

//        private static Mock<UserManager<User>> CreateUserManagerMock()
//        {
//            var store = new Mock<IUserStore<User>>();
//            return new Mock<UserManager<User>>(store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
//        }
//    }
//}