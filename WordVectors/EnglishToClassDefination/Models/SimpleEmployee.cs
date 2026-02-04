using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishToClassDefinition.Models
{
    public class SimpleEmployee
    {
        [Required]
        public string? Name { get; set; }
        public int Age { get; set; } = 25;
        public string? Department { get; set; }
        public Address Address { get; set; } 
    }
}
