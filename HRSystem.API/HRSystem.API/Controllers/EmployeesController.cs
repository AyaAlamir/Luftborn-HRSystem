using System.Threading.Tasks;
using HRSystem.Application.Common;
using HRSystem.Application.Employees;
using HRSystem.Application.Employees.Commands;
using HRSystem.Application.Employees.Queries;
using HRSystem.Application.Localization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRSystem.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILocalizationService _localizer;

        public EmployeesController(IMediator mediator, ILocalizationService localizer)
        {
            _mediator = mediator;
            _localizer = localizer;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _mediator.Send(new GetEmployeesQuery());
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _mediator.Send(new GetEmployeeByIdQuery(id));
            if (!response.Success)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateEmployeeRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.Fail(_localizer.Get("ValidationFailed")));
            }

            var response = await _mediator.Send(new CreateEmployeeCommand(request));
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return CreatedAtAction(nameof(GetById), new { id = response.Data.Id }, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateEmployeeRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.Fail(_localizer.Get("ValidationFailed")));
            }

            var response = await _mediator.Send(new UpdateEmployeeCommand(id, request));
            if (!response.Success)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _mediator.Send(new DeleteEmployeeCommand(id));
            if (!response.Success)
            {
                return NotFound(response);
            }

            return Ok(response);
        }
    }
}
