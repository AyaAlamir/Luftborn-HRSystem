using HRSystem.Application.Common;
using MediatR;

namespace HRSystem.Application.Employees.Queries
{
    public class GetEmployeeByIdQuery : IRequest<ApiResponse<EmployeeDto>>
    {
        public GetEmployeeByIdQuery(int id) { Id = id; }
        public int Id { get; }
    }
}
