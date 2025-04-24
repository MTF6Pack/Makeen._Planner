//using Application.Contracts.Tasks.Commands;
//using Application.Task_Service;
//using Domain;
//using Domain.Services;
//using Domain.TaskEnums;
//using Infrastructure.SignalR;
//using MediatR;
//using Microsoft.AspNetCore.SignalR;
//using Microsoft.Extensions.Logging;
//using Moq;
//using Persistence;
//using Persistence.Repository;
//using Persistence.Repository.Interface;
//using Xunit;
//using Task = System.Threading.Tasks.Task;

//namespace MyAwesomeProjectTests.Application
//{
//    public class TaskService
//    {
//        private readonly ITaskRepository _taskRepo;
//        private readonly IUserRepository _userRepo;
//        private readonly INotificationSender _notificationSender;

//        public TaskService(
//            ITaskRepository taskRepo,
//            IUserRepository userRepo,
//            INotificationSender notificationSender)
//        {
//            _taskRepo = taskRepo;
//            _userRepo = userRepo;
//            _notificationSender = notificationSender;
//        }

//        [Fact]
//        public async Task AddTask_ShouldAddTask_ForUserSelf()
//        {
//            var mockTaskRepo = new Mock<ITaskRepository>();
//            var mockUserRepo = new Mock<IUserRepository>();

//            // 👇 This is the key: pass a dummy or mock notification sender, but don’t setup anything
//            var mockNotificationSender = new Mock<INotificationSender>();

//            var service = new TaskService(
//                mockTaskRepo.Object,
//                mockUserRepo.Object,
//                mockNotificationSender.Object // 👈 just throw it in, we won’t use it
//            );

//            var command = new AddTaskCommand
//            {
//                Name = "Test task",
//                Description = "Whatever",
//                DeadLine = DateTime.Now.AddDays(1),
//                Alarm = Alarm.One_Hour
//            };

//            var userId = Guid.NewGuid();

//            mockUserRepo.Setup(repo => repo.Exists(userId)).ReturnsAsync(true);

//            // Act
//            await service.AddTask(command, userId);

//            // Assert
//            mockTaskRepo.Verify(repo => repo.AddAsync(It.IsAny<TaskEntity>()), Times.Once);
//        }
//    }
//}