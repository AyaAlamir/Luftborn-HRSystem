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
    public class GetEmployeeByIdQueryHandler : IRequestHandler<GetEmployeeByIdQuery, ApiResponse<EmployeeDto>>
    {
        private readonly IBaseRepository<Employee> _repository;
        private readonly IMapper _mapper;
        private readonly ILocalizationService _localizer;

        public GetEmployeeByIdQueryHandler(IBaseRepository<Employee> repository, IMapper mapper, ILocalizationService localizer)
        {
            _repository = repository;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<ApiResponse<EmployeeDto>> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var employee = await _repository.GetByIdAsync(request.Id);
                if (employee == null)
                {
                    return ApiResponse<EmployeeDto>.Fail(_localizer.Get("EmployeeNotFound"));
                }
                return ApiResponse<EmployeeDto>.Ok(_mapper.Map<EmployeeDto>(employee), _localizer.Get("EmployeeLoaded"));
            }
            catch (System.Exception)
            {
                return ApiResponse<EmployeeDto>.Fail(_localizer.Get("ErrorRetrievingEmployee"));
            }
        }
    }
}
