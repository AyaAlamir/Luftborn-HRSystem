using System.Threading;
using System.Threading.Tasks;
using HRSystem.Application.Common;
using HRSystem.Application.Employees.Commands;
using HRSystem.Application.Localization;
using HRSystem.Domain.Entities;
using HRSystem.Domain.Interfaces.Base;
using MediatR;

namespace HRSystem.Application.Employees.Handlers
{
    public class DeleteEmployeeCommandHandler : IRequestHandler<DeleteEmployeeCommand, ApiResponse<bool>>
    {
        private readonly IBaseRepository<Employee> _repository;
        private readonly ILocalizationService _localizer;

        public DeleteEmployeeCommandHandler(IBaseRepository<Employee> repository, ILocalizationService localizer)
        {
            _repository = repository;
            _localizer = localizer;
        }

        public async Task<ApiResponse<bool>> Handle(DeleteEmployeeCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var employee = await _repository.GetByIdAsync(command.Id);
                if (employee == null)
                {
                    return ApiResponse<bool>.Fail(_localizer.Get("EmployeeNotFound"));
                }

                await _repository.DeleteAsync(command.Id);
                return ApiResponse<bool>.Ok(true, _localizer.Get("EmployeeDeleted"));
            }
            catch (System.Exception)
            {
                return ApiResponse<bool>.Fail(_localizer.Get("ErrorDeletingEmployee"));
            }
        }
    }
}
