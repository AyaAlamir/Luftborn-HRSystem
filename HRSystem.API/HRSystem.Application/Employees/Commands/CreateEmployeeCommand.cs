using HRSystem.Application.Common;
using HRSystem.Application.Employees;
using MediatR;

namespace HRSystem.Application.Employees.Commands
{
    public class CreateEmployeeCommand : IRequest<ApiResponse<EmployeeDto>>
    {
        public CreateEmployeeCommand(CreateEmployeeRequest request) { Request = request; }
        public CreateEmployeeRequest Request { get; }
    }
}
