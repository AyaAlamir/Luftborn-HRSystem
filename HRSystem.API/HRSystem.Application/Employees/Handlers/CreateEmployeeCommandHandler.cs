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
    public class CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeeCommand, ApiResponse<EmployeeDto>>
    {
        private readonly IBaseRepository<Employee> _repository;
        private readonly IMapper _mapper;
        private readonly ILocalizationService _localizer;

        public CreateEmployeeCommandHandler(IBaseRepository<Employee> repository, IMapper mapper, ILocalizationService localizer)
        {
            _repository = repository;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<ApiResponse<EmployeeDto>> Handle(CreateEmployeeCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var employee = _mapper.Map<Employee>(command.Request);
                await _repository.AddAsync(employee);
                return ApiResponse<EmployeeDto>.Ok(_mapper.Map<EmployeeDto>(employee), _localizer.Get("EmployeeCreated"));
            }
            catch (System.Exception)
            {
                return ApiResponse<EmployeeDto>.Fail(_localizer.Get("ErrorCreatingEmployee"));
            }
        }
    }
}
