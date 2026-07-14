using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HRSystem.Application.Common;
using HRSystem.Application.Employees.Queries;
using HRSystem.Application.Localization;
using HRSystem.Domain.Entities;
using HRSystem.Domain.Interfaces.Base;
using MediatR;

namespace HRSystem.Application.Employees.Handlers
{
    public class GetEmployeesQueryHandler : IRequestHandler<GetEmployeesQuery, ApiResponse<IEnumerable<EmployeeDto>>>
    {
        private readonly IBaseRepository<Employee> _repository;
        private readonly IMapper _mapper;
        private readonly ILocalizationService _localizer;

        public GetEmployeesQueryHandler(IBaseRepository<Employee> repository, IMapper mapper, ILocalizationService localizer)
        {
            _repository = repository;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<ApiResponse<IEnumerable<EmployeeDto>>> Handle(GetEmployeesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var employees = await _repository.GetAllAsync();
                var dtos = _mapper.Map<IEnumerable<EmployeeDto>>(employees);
                return ApiResponse<IEnumerable<EmployeeDto>>.Ok(dtos, _localizer.Get("EmployeesLoaded"));
            }
            catch (System.Exception)
            {
                return ApiResponse<IEnumerable<EmployeeDto>>.Fail(_localizer.Get("ErrorRetrievingEmployees"));
            }
        }
    }
}
