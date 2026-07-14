using System;
using System.ComponentModel.DataAnnotations;

namespace HRSystem.Application.Employees
{
    public class CreateEmployeeRequest
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        [Required]
        public string Department { get; set; }

        [Range(0, 999999999)]
        public decimal Salary { get; set; }

        public DateTime HireDate { get; set; }
    }
}
