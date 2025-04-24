//using Microsoft.EntityFrameworkCore;
//using Moq;
//using System.Linq;

//namespace MyAwesomeProjectTests
//{
//    public static class DbContextMock
//    {
//        public static DbSet<T> CreateMockDbSet<T>(IQueryable<T> data) where T : class
//        {
//            var mockSet = new Mock<DbSet<T>>();

//            // Setup async provider
//            mockSet.As<IAsyncEnumerable<T>>()
//                   .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
//                   .Returns(new TestAsyncEnumerable<T>(data).GetAsyncEnumerator());

//            mockSet.As<IQueryable<T>>().Setup(m => m.Provider)
//                   .Returns(new TestAsyncQueryProvider<T>(data.Provider));
//            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
//            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
//            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

//            return mockSet.Object;
//        }
//    }
//}