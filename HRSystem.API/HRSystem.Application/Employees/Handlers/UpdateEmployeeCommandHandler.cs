using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HRSystem.Application.Common;
using HRSystem.Application.Employees.Commands;
using HRSystem.Application.Localization;
using HRSystem.Domain.Entities;
using HRSystem.Domain.Interfaces.Base;
using MediatR;

namespace HRSystem.Application.Employees.Handlers
{
    public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, ApiResponse<EmployeeDto>>
    {
        private readonly IBaseRepository<Employee> _repository;
        private readonly IMapper _mapper;
        private readonly ILocalizationService _localizer;

        public UpdateEmployeeCommandHandler(IBaseRepository<Employee> repository, IMapper mapper, ILocalizationService localizer)
        {
            _repository = repository;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<ApiResponse<EmployeeDto>> Handle(UpdateEmployeeCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var employee = await _repository.GetByIdAsync(command.Id);
                if (employee == null)
                {
                    return ApiResponse<EmployeeDto>.Fail(_localizer.Get("EmployeeNotFound"));
                }

                _mapper.Map(command.Request, employee);

                await _repository.UpdateAsync(employee);
                return ApiResponse<EmployeeDto>.Ok(_mapper.Map<EmployeeDto>(employee), _localizer.Get("EmployeeUpdated"));
            }
            catch (System.Exception)
            {
                return ApiResponse<EmployeeDto>.Fail(_localizer.Get("ErrorUpdatingEmployee"));
            }
        }
    }
}
