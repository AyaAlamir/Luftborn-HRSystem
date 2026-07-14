using System.Collections.Generic;
using HRSystem.Application.Common;
using MediatR;

namespace HRSystem.Application.Employees.Queries
{
    public class GetEmployeesQuery : IRequest<ApiResponse<IEnumerable<EmployeeDto>>>
    {
    }
}
