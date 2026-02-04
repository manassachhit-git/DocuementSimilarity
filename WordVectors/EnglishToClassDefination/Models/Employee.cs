using EnglishToClassDefinition.ENUM;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishToClassDefinition.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; } = "New";
        public string LastName { get; set; } = "Employee";

        public EmploymentType Type { get; set; } = EmploymentType.FullTime;

        public DateTime DateOfJoining { get; set; } = DateTime.UtcNow;
        public DateTime? LastPromotionDate { get; set; } = null;

        // Nullable salary (interns/contractors may not have fixed salary)
        public decimal? Salary { get; set; } = null;

        // Complex object
        public Address HomeAddress { get; set; } = new Address();

        // List of complex objects
        public List<EmergencyContact> EmergencyContacts { get; set; } =
            new List<EmergencyContact>()
            {
            new EmergencyContact { Name = "Primary Contact", Relationship = "Parent", Phone = "000-000-0000" }
            };

        // Computed (ignored by schema usually)
        public string FullName => $"{FirstName} {LastName}";

        public String[] Hobbies { get; set; }
    }

}
