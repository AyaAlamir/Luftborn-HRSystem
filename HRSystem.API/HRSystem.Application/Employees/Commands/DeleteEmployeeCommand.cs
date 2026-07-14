using HRSystem.Application.Common;
using MediatR;

namespace HRSystem.Application.Employees.Commands
{
    public class DeleteEmployeeCommand : IRequest<ApiResponse<bool>>
    {
        public DeleteEmployeeCommand(int id) { Id = id; }
        public int Id { get; }
    }
}
