using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishToClassDefinition.Utilities
{
    public class Utility
    {
        public static List<string> GetPropertyNames(Type type, string? prefix = null)
        {
            var props = new List<string>();
            foreach (var prop in type.GetProperties())
            {
                var propType = prop.PropertyType;
                var propName = string.IsNullOrEmpty(prefix) ? prop.Name : $"{prefix}.{prop.Name}";

                // If it's a class (but not string/primitive/array), recurse
                if (propType.IsClass && propType != typeof(string) && !propType.IsArray)
                {
                    props.AddRange(GetPropertyNames(propType, propName));
                }
                else
                {
                    props.Add(propName);
                }
            }
            return props;
        }

        public static string CleanJsonFences(string input)
        {
            input = input.Trim();
            if (input.StartsWith("```"))
            {
                int firstNewline = input.IndexOf('\n');
                int lastFence = input.LastIndexOf("```");
                if (firstNewline >= 0 && lastFence > firstNewline)
                {
                    input = input.Substring(firstNewline + 1, lastFence - firstNewline - 1).Trim();
                }
            }
            input = input.Trim('`');
            return input;
        }
    }
}
