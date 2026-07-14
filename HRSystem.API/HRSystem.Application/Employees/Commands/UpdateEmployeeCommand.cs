using HRSystem.Application.Common;
using HRSystem.Application.Employees;
using MediatR;

namespace HRSystem.Application.Employees.Commands
{
    public class UpdateEmployeeCommand : IRequest<ApiResponse<EmployeeDto>>
    {
        public UpdateEmployeeCommand(int id, UpdateEmployeeRequest request)
        {
            Id = id;
            Request = request;
        }

        public int Id { get; }
        public UpdateEmployeeRequest Request { get; }
    }
}
