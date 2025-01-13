using Domains;
using Makeen._Planner.Task_Service;
using MediatR;
using Repository;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Application
{
    public class TaskCommandHandler(ITaskRepository repository, IUnitOfWork unitOfWork) : IRequestHandler<AddTaskCommand>
    {
        private readonly ITaskRepository _repository = repository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        public async System.Threading.Tasks.Task Handle(AddTaskCommand request, CancellationToken cancellationToken)
        {
            await _repository.AddAsync(request.ToModel());
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
