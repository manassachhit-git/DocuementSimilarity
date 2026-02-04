using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EnglishToClassDefinition.SchemaBuilder
{
    public static class SchemaBuilder
    {
        private static readonly HashSet<Type> _processedTypes = new HashSet<Type>();

        public static Dictionary<string, object> BuildSchema<T>() => BuildSchema(typeof(T));

        private static Dictionary<string, object> BuildSchema(Type type)
        {
            if (_processedTypes.Contains(type))
                return new Dictionary<string, object>(); // Prevent recursion infinite loops

            _processedTypes.Add(type);

            var schema = new Dictionary<string, object>();

            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                schema[prop.Name] = BuildPropertySchema(prop);
            }

            return schema;
        }

        private static Dictionary<string, object> BuildPropertySchema(PropertyInfo prop)
        {
            var propSchema = new Dictionary<string, object>();

            Type propType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

            // Check if property is required
            if (prop.GetCustomAttribute<RequiredAttribute>() != null)
            {
                propSchema["required"] = true;
            }

            // Enums
            if (propType.IsEnum)
            {
                propSchema["type"] = "string";
                propSchema["enum"] = Enum.GetNames(propType);
                return propSchema;
            }

            // Primitive types mapping
            if (IsSimpleType(propType))
            {
                propSchema["type"] = MapToJsonType(propType);

                // Default value if assigned in class
                var defaultValue = GetDefaultValueFromProperty(prop);
                if (defaultValue != null)
                    propSchema["default"] = defaultValue;

                return propSchema;
            }

            // Collections
            if (typeof(System.Collections.IEnumerable).IsAssignableFrom(propType) &&
                propType != typeof(string))
            {
                Type itemType = propType.IsArray ? propType.GetElementType() :
                                propType.GetGenericArguments().FirstOrDefault();

                propSchema["type"] = "array";
                propSchema["items"] = itemType != null ? BuildSchema(itemType) : new Dictionary<string, object>();
                return propSchema;
            }

            // Complex objects
            propSchema["type"] = "object";
            propSchema["properties"] = BuildSchema(propType);

            return propSchema;
        }

        private static bool IsSimpleType(Type type)
        {
            return type.IsPrimitive ||
                   type == typeof(string) ||
                   type == typeof(decimal) ||
                   type == typeof(DateTime) ||
                   type == typeof(Guid);
        }

        private static string MapToJsonType(Type type)
        {
            if (type == typeof(string)) return "string";
            if (type == typeof(int) || type == typeof(long)) return "integer";
            if (type == typeof(decimal) || type == typeof(double) || type == typeof(float)) return "number";
            if (type == typeof(bool)) return "boolean";
            if (type == typeof(DateTime)) return "string"; // Could add format date/datetime
            return "string";
        }

        private static object GetDefaultValueFromProperty(PropertyInfo prop)
        {
            var instance = Activator.CreateInstance(prop.DeclaringType!);
            var defaultValue = prop.GetValue(instance);

            if (defaultValue == null ||
                defaultValue.Equals(GetDefault(prop.PropertyType)))
                return null;

            return defaultValue;
        }

        private static object GetDefault(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type)! : null!;
        }
    }
}
