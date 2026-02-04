using EnglishToClassDefinition.ENUM;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishToClassDefinition.Models
{
    public class ComplexEmployee
    {
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public int Age { get; set; } = 25;

        public EmploymentType Type { get; set; } // Enum

        public DateTime DateOfJoining { get; set; }
        public DateTime? LastPromotionDate { get; set; }

        [Range(30000, 200000)]
        public decimal Salary { get; set; }

        public Address HomeAddress { get; set; } // Nested object

        public List<EmergencyContact> EmergencyContacts { get; set; } =
            new List<EmergencyContact>()
            {
            new EmergencyContact { Name = "Primary Contact", Relationship = "Parent", Phone = "000-000-0000" }
            };
        public string[] Skills { get; set; } // Array of primitives

        public string[] Hobbies { get; set; }

        // 🔥 Complex: List of nested objects
        public List<Project> Projects { get; set; }

        // 🔥 Complex: Dictionary type
        public Dictionary<string, string> Attributes { get; set; }

        // 🔥 Complex: Polymorphic-like wrapper (variant object)
        public IRoleDetails RoleDetails { get; set; }

        // Computed (ignored by schema usually)
        public string FullName => $"{FirstName} {LastName}";
    }

    // Complex nested list items
    public class Project
    {
        public string ProjectName { get; set; }
        public string Status { get; set; }
        public DateTime StartedOn { get; set; }
        public DateTime? EndedOn { get; set; }
    }

    // Polymorphic-like interface
    public interface IRoleDetails { }

    public class ManagerRole : IRoleDetails
    {
        public int TeamSize { get; set; }
    }

    public class DeveloperRole : IRoleDetails
    {
        public string PrimaryLanguage { get; set; }
        public bool IsFullStack { get; set; }
    }
}
