using AutoMapper;
using HRSystem.Application.Employees;
using HRSystem.Domain.Entities;

namespace HRSystem.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Employee, EmployeeDto>().ReverseMap();
            CreateMap<CreateEmployeeRequest, Employee>();
            CreateMap<UpdateEmployeeRequest, Employee>();
        }
    }
}
