using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace EnglishToClassDefinition.SchemaBuilder
{
    public static class ClassSchemaBuilder
    {
        public static Dictionary<string, object?> Build(Type type, HashSet<Type>? visited = null)
        {
            visited ??= new HashSet<Type>();

            if (visited.Contains(type))
                return new Dictionary<string, object?>();

            visited.Add(type);

            var schema = new Dictionary<string, object?>();

            // Create instance to read default values
            object? instance = null;
            try { instance = Activator.CreateInstance(type); } catch { }

            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var propType = prop.PropertyType;
                var entry = GetSchemaForType(propType, visited);

                // Attach default value if available
                if (instance != null && prop.GetIndexParameters().Length == 0)
                {
                    var defaultValue = prop.GetValue(instance);
                    if (defaultValue != null &&
                        (IsPrimitiveOrString(propType) || propType.IsArray))
                    {
                        if (entry is Dictionary<string, object?> dic)
                        {
                            dic["default"] = defaultValue;
                        }
                    }
                }

                // ADD REQUIRED FLAG
                if (entry is Dictionary<string, object?> dict)
                {
                    dict["required"] = IsRequired(prop);

                    // Add validation attributes
                    AddValidationAttributesToSchema(prop, dict);
                }

                schema[prop.Name] = entry;
            }

            return schema;
        }

        /// <summary>
        /// Adds validation attributes (Range, Regex, ErrorMessage) to the schema dictionary.
        /// </summary>
        private static void AddValidationAttributesToSchema(PropertyInfo prop, Dictionary<string, object?> dict)
        {
            // Range
            var rangeAttr = prop.GetCustomAttribute<RangeAttribute>();
            if (rangeAttr != null)
            {
                dict["minimum"] = rangeAttr.Minimum;
                dict["maximum"] = rangeAttr.Maximum;
                if (!string.IsNullOrWhiteSpace(rangeAttr.ErrorMessage))
                    dict["errorMessage"] = rangeAttr.ErrorMessage;
            }

            // RegularExpression
            var regexAttr = prop.GetCustomAttribute<RegularExpressionAttribute>();
            if (regexAttr != null)
            {
                dict["pattern"] = regexAttr.Pattern;
                if (!string.IsNullOrWhiteSpace(regexAttr.ErrorMessage))
                    dict["errorMessage"] = regexAttr.ErrorMessage;
            }

            // Other validation attributes with ErrorMessage
            var validationAttrs = prop.GetCustomAttributes<ValidationAttribute>();
            foreach (var attr in validationAttrs)
            {
                if (!string.IsNullOrWhiteSpace(attr.ErrorMessage))
                {
                    dict["errorMessage"] = attr.ErrorMessage;
                }
            }
        }

        private static object? GetSchemaForType(Type type, HashSet<Type> visited)
        {
            // Handle nullable types (e.g. int?)
            if (Nullable.GetUnderlyingType(type) is Type underlyingNullable)
                return GetSchemaForType(underlyingNullable, visited);

            // Primitive & simple types
            if (IsPrimitiveOrString(type))
                return new Dictionary<string, object?>
                {
                    ["type"] = MapPrimitiveToString(type)
                };

            // Arrays
            if (type.IsArray)
            {
                var elementType = type.GetElementType();
                return new Dictionary<string, object?>
                {
                    ["type"] = "array",
                    ["items"] = GetSchemaForType(elementType!, visited),
                    ["splitRule"] = "Split into separate elements using commas"
                };
            }

            // IList / List<T>
            if (typeof(IList).IsAssignableFrom(type) ||
                type.IsGenericType &&
                 typeof(IEnumerable<>).IsAssignableFrom(type.GetGenericTypeDefinition()))
            {
                Type element = type.IsArray ? type.GetElementType()! : type.GetGenericArguments()[0];

                var itemSchema = GetSchemaForType(element, visited);

                // Wrap complex items properly
                if (itemSchema is Dictionary<string, object?> itemDict &&
                    !itemDict.ContainsKey("type"))
                {
                    itemSchema = new Dictionary<string, object?>
                    {
                        ["type"] = "object",
                        ["properties"] = itemDict
                    };
                }

                return new Dictionary<string, object?>
                {
                    ["type"] = "array",
                    ["items"] = itemSchema
                };
            }

            // if type is Enum
            if (type.IsEnum)
            {
                return new Dictionary<string, object?>
                {
                    ["type"] = "enum",
                    ["values"] = Enum.GetNames(type),
                    ["default"] = Enum.GetNames(type).First()
                };
            }

            // Dictionary<string, T>
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                var keyType = type.GetGenericArguments()[0];
                var valueType = type.GetGenericArguments()[1];

                // Only string keys are allowed in JSON
                if (keyType != typeof(string))
                {
                    throw new NotSupportedException("Only Dictionary<string, T> is supported.");
                }

                return new Dictionary<string, object?>
                {
                    ["type"] = "dictionary",
                    ["keyType"] = "string",
                    ["value"] = GetSchemaForType(valueType, visited)
                };
            }

            // Polymorphic types: Interface or abstract class
            if (type.IsInterface || type.IsAbstract)
            {
                var implementations = AppDomain.CurrentDomain
                    .GetAssemblies()
                    .SelectMany(a =>
                    {
                        try { return a.GetTypes(); }
                        catch { return Array.Empty<Type>(); }
                    })
                    .Where(t => type.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract)
                    .ToList();

                return new Dictionary<string, object?>
                {
                    ["type"] = "object",
                    ["oneOf"] = implementations.Select(impl => new Dictionary<string, object?>
                    {
                        ["name"] = impl.Name,
                        ["properties"] = Build(impl, visited)
                    }).ToList()
                };
            }

            // Complex class
            if (type.IsClass)
            {
                return new Dictionary<string, object?>
                {
                    ["type"] = "object",
                    ["properties"] = Build(type, visited)
                };
            }


            // Fallback
            return new Dictionary<string, object?> { ["type"] = "object" };
        }

        private static bool IsPrimitiveOrString(Type t)
        {
            return t.IsPrimitive ||
                   t == typeof(string) ||
                   t == typeof(decimal);
        }

        private static string MapPrimitiveToString(Type t)
        {
            if (t == typeof(string)) return "string";
            if (t == typeof(int)) return "int";
            if (t == typeof(long)) return "long";
            if (t == typeof(bool)) return "bool";
            if (t == typeof(float)) return "float";
            if (t == typeof(double)) return "double";
            if (t == typeof(decimal)) return "decimal";

            return "string";
        }

        private static bool IsRequired(PropertyInfo prop)
        {
            // [Required] attribute
            if (prop.GetCustomAttribute<RequiredAttribute>() != null)
                return true;

            // Non-nullable value types are always required
            var t = prop.PropertyType;
            if (t.IsValueType && Nullable.GetUnderlyingType(t) == null)
                return true;

            return false;
        }
    }

}

