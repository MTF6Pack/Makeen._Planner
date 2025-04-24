﻿//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Query;
//using Moq;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Threading;
//using System.Threading.Tasks;

//namespace MyAwesomeProjectTests
//{
//    public class TestAsyncQueryProvider<T> : IAsyncQueryProvider
//    {
//        private readonly IQueryProvider _inner;
//        public TestAsyncQueryProvider(IQueryProvider inner)
//        {
//            _inner = inner;
//        }

//        public IQueryable CreateQuery(Expression expression)
//        {
//            return new TestAsyncEnumerable<T>(expression);
//        }

//        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
//        {
//            return new TestAsyncEnumerable<TElement>(expression);
//        }

//        public object Execute(Expression expression)
//        {
//            return _inner.Execute(expression);
//        }

//        public TResult Execute<TResult>(Expression expression)
//        {
//            return _inner.Execute<TResult>(expression);
//        }

//        // This implements the IAsyncQueryProvider method.
//        public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
//        {
//            return Task.FromResult(Execute<TResult>(expression));
//        }

//        TResult IAsyncQueryProvider.ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
//        {
//            throw new NotImplementedException();
//        }
//    }

//    public class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
//    {
//        public TestAsyncEnumerable(IEnumerable<T> enumerable)
//            : base(enumerable)
//        { }

//        public TestAsyncEnumerable(Expression expression)
//            : base(expression)
//        { }

//        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
//        {
//            return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
//        }

//        IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);
//    }

//    public class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
//    {
//        private readonly IEnumerator<T> _inner;
//        public TestAsyncEnumerator(IEnumerator<T> inner)
//        {
//            _inner = inner;
//        }
//        public T Current => _inner.Current;
//        public ValueTask DisposeAsync()
//        {
//            _inner.Dispose();
//            return ValueTask.CompletedTask;
//        }
//        public ValueTask<bool> MoveNextAsync()
//        {
//            return new ValueTask<bool>(_inner.MoveNext());
//        }
//    }

//    public static class DbContextMock
//    {
//        public static DbSet<T> CreateMockDbSet<T>(IQueryable<T> data) where T : class
//        {
//            var mockSet = new Mock<DbSet<T>>();

//            // Setup asynchronous behavior.
//            mockSet.As<IAsyncEnumerable<T>>()
//                   .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
//                   .Returns(new TestAsyncEnumerable<T>(data).GetAsyncEnumerator());

//            // Setup IQueryable members.
//            mockSet.As<IQueryable<T>>().Setup(m => m.Provider)
//                   .Returns(new TestAsyncQueryProvider<T>(data.Provider));
//            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
//            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
//            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

//            return mockSet.Object;
//        }
//    }
//}