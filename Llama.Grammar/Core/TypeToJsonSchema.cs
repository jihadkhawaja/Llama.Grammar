using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Collections;

namespace Llama.Grammar.Core
{
    internal static class TypeToJsonSchema
    {
        internal static string Convert<T>()
        {
            var schema = new JsonObject
            {
                ["$schema"] = "http://json-schema.org/draft-07/schema#",
                ["type"] = "object",
                ["properties"] = GenerateProperties(typeof(T)),
                ["required"] = GenerateRequired(typeof(T))
            };

            return schema.ToJsonString();
        }

        private static JsonObject GenerateProperties(Type type)
        {
            var properties = new JsonObject();
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in props)
            {
                properties[prop.Name] = GeneratePropertySchema(prop);
            }

            return properties;
        }

        private static JsonArray GenerateRequired(Type type)
        {
            var required = new JsonArray();
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in props)
            {
                var propType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                if (prop.PropertyType.IsValueType && propType != typeof(bool))
                {
                    required.Add(prop.Name);
                }
            }

            return required;
        }

        private static JsonObject GeneratePropertySchema(PropertyInfo prop)
        {
            var schema = new JsonObject();
            var propType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

            if (propType.IsEnum)
            {
                schema["type"] = "string";
                var enumValues = new JsonArray();
                foreach (var value in Enum.GetNames(propType))
                {
                    enumValues.Add(value);
                }
                schema["enum"] = enumValues;
            }
            else if (propType == typeof(string))
            {
                schema["type"] = "string";
            }
            else if (propType == typeof(int) || propType == typeof(long))
            {
                schema["type"] = "integer";
            }
            else if (propType == typeof(float) || propType == typeof(double) || propType == typeof(decimal))
            {
                schema["type"] = "number";
            }
            else if (propType == typeof(bool))
            {
                schema["type"] = "boolean";
            }
            else if (propType == typeof(DateTime))
            {
                schema["type"] = "string";
                schema["format"] = "date-time";
            }
            else if (typeof(IEnumerable).IsAssignableFrom(propType) && propType != typeof(string))
            {
                schema["type"] = "array";
                var elementType = propType.IsArray ? 
                    propType.GetElementType() : 
                    propType.GetGenericArguments().FirstOrDefault();

                var itemSchema = new JsonObject();
                if (elementType == null)
                {
                    itemSchema["type"] = "string"; // default fallback for unknown element types
                }
                else if (elementType.IsClass && elementType != typeof(string))
                {
                    itemSchema = GenerateProperties(elementType);
                    itemSchema["type"] = "object";
                }
                else
                {
                    itemSchema["type"] = GetJsonType(elementType);
                    if (elementType.IsEnum)
                    {
                        var enumValues = new JsonArray();
                        foreach (var value in Enum.GetNames(elementType))
                        {
                            enumValues.Add(value);
                        }
                        itemSchema["enum"] = enumValues;
                    }
                }
                schema["items"] = itemSchema;
            }
            else if (propType.IsClass)
            {
                schema["type"] = "object";
                schema["properties"] = GenerateProperties(propType);
                schema["required"] = GenerateRequired(propType);
            }

            if (Nullable.GetUnderlyingType(prop.PropertyType) != null)
            {
                schema["nullable"] = true;
            }

            return schema;
        }

        private static string GetJsonType(Type type)
        {
            if (type == typeof(string) || type.IsEnum)
                return "string";
            if (type == typeof(int) || type == typeof(long))
                return "integer";
            if (type == typeof(float) || type == typeof(double) || type == typeof(decimal))
                return "number";
            if (type == typeof(bool))
                return "boolean";
            if (type == typeof(DateTime))
                return "string";
            if (type.IsClass)
                return "object";
            
            return "string"; // default fallback
        }
    }
}