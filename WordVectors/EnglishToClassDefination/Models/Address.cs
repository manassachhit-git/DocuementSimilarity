using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishToClassDefinition.Models
{
    public class Address
    {
        public string Street { get; set; } = "Unknown Street";
        public string City { get; set; } = "Unknown City";
        public string Country { get; set; } = "Unknown Country";
        public int ZipCode { get; set; } = 000000;
    }
}
