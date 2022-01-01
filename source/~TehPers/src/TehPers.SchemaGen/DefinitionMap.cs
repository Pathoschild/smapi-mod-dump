/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Namotion.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TehPers.FishingOverhaul.SchemaGen
{
    public class DefinitionMap
    {
        private static readonly Dictionary<Type, JObject> predefinedSchemas = new()
        {
            [typeof(bool)] = new() { ["type"] = "boolean" },
            [typeof(string)] = new() { ["type"] = "string" },
            [typeof(byte)] = new()
            {
                ["type"] = "integer",
                ["minimum"] = byte.MinValue,
                ["maximum"] = byte.MaxValue,
            },
            [typeof(sbyte)] = new()
            {
                ["type"] = "integer",
                ["minimum"] = sbyte.MinValue,
                ["maximum"] = sbyte.MaxValue,
            },
            [typeof(ushort)] = new()
            {
                ["type"] = "integer",
                ["minimum"] = ushort.MinValue,
                ["maximum"] = ushort.MaxValue,
            },
            [typeof(short)] = new()
            {
                ["type"] = "integer",
                ["minimum"] = short.MinValue,
                ["maximum"] = short.MaxValue,
            },
            [typeof(uint)] = new()
            {
                ["type"] = "integer",
                ["minimum"] = uint.MinValue,
                ["maximum"] = uint.MaxValue,
            },
            [typeof(int)] = new()
            {
                ["type"] = "integer",
                ["minimum"] = int.MinValue,
                ["maximum"] = int.MaxValue,
            },
            [typeof(ulong)] = new()
            {
                ["type"] = "integer",
                ["minimum"] = ulong.MinValue,
                ["maximum"] = ulong.MaxValue,
            },
            [typeof(long)] = new()
            {
                ["type"] = "integer",
                ["minimum"] = long.MinValue,
                ["maximum"] = long.MaxValue,
            },
            [typeof(float)] = new() { ["type"] = "number" },
            [typeof(double)] = new() { ["type"] = "number" },
            [typeof(decimal)] = new() { ["type"] = "number" },
            [typeof(DateTime)] = new()
            {
                ["type"] = "string",
                ["format"] = "date-time",
            },
            [typeof(DateTimeOffset)] = new()
            {
                ["type"] = "string",
                ["format"] = "date-time",
            },
            [typeof(byte[])] = new() { ["type"] = "string" },
            [typeof(Type)] = new() { ["type"] = "string" },
            [typeof(Guid)] = new()
            {
                ["type"] = "string",
                ["format"] = "uuid",
            },
        };

        private static readonly HashSet<Type> arrayTypes = new()
        {
            typeof(IEnumerable<>),
            typeof(IReadOnlyList<>),
            typeof(IList<>),
            typeof(List<>),
            typeof(IImmutableList<>),
            typeof(ImmutableArray<>),
            typeof(ImmutableList<>),
        };

        private static readonly HashSet<Type> dictionaryTypes = new()
        {
            typeof(IReadOnlyDictionary<,>),
            typeof(IDictionary<,>),
            typeof(Dictionary<,>),
            typeof(IImmutableDictionary<,>),
            typeof(ImmutableDictionary<,>),
        };

        public Dictionary<string, JObject> Definitions { get; } = new();

        private readonly Dictionary<string, JObject> knownDefinitions;

        public DefinitionMap()
        {
            this.knownDefinitions = new();
        }

        public DefinitionMap(DefinitionMap knownDefinitions)
        {
            this.knownDefinitions =
                knownDefinitions.Definitions.ToDictionary(kv => kv.Key, kv => kv.Value);
        }

        private string GetDefinitionName(Type type)
        {
            return type.FullName
                ?? throw new ArgumentException("Type not supported for definitions", nameof(type));
        }

        public bool Assign(Type type, JObject schema)
        {
            return this.Definitions.TryAdd(this.GetDefinitionName(type), schema);
        }

        public JObject Register(ContextualType contextualType, bool forceNotNull = false)
        {
            var schemaChoices = this.CreateSchemas(contextualType, forceNotNull).ToList();
            if (schemaChoices.Count == 1 && !schemaChoices[0].ContainsKey("$ref"))
            {
                // Single choice - just return the definition itself
                return schemaChoices[0];
            }

            // Create union definition: { "oneOf": [...] }
            var oneOf = new JArray();
            foreach (var schemaChoice in schemaChoices)
            {
                oneOf.Add(schemaChoice);
            }

            return new()
            {
                ["oneOf"] = oneOf,
            };
        }

        private IEnumerable<JObject> CreateSchemas(ContextualType contextualType, bool forceNotNull)
        {
            // Nullable types
            if (!forceNotNull && contextualType.Nullability is not Nullability.NotNullable)
            {
                yield return new() { ["type"] = "null" };
            }

            // Get inner type
            var type = contextualType.Type;

            // Predefined schemas
            if (DefinitionMap.predefinedSchemas.TryGetValue(type, out var predefinedDef))
            {
                // Predefined type
                yield return (JObject)predefinedDef.DeepClone();
                yield break;
            }

            // Arrays
            if (type.IsArray)
            {
                // Array
                if (type.GetArrayRank() != 1)
                {
                    throw new InvalidOperationException(
                        "Schema generation doesn't work with multidimensional arrays."
                    );
                }

                yield return new()
                {
                    ["type"] = "array",
                    ["items"] = this.Register(contextualType.ElementType!),
                };
                yield break;
            }

            // Special generic types
            if (type.IsGenericType)
            {
                // Get generic type definition (like List<>)
                var genericDef = type.GetGenericTypeDefinition();

                // Array types
                if (DefinitionMap.arrayTypes.Contains(genericDef))
                {
                    yield return new()
                    {
                        ["type"] = "array",
                        ["items"] = this.Register(contextualType.GenericArguments[0]),
                    };
                    yield break;
                }

                // Dictionary types
                if (DefinitionMap.dictionaryTypes.Contains(genericDef))
                {
                    // Check if key can be converted to string
                    if (!this.IsStringish(contextualType.GenericArguments[0]))
                    {
                        throw new InvalidOperationException(
                            "Schema generation doesn't work with dictionaries with non-string keys."
                        );
                    }

                    // TODO: pattern properties for some key types
                    yield return new()
                    {
                        ["type"] = "object",
                        ["additionalProperties"] =
                            this.Register(contextualType.GenericArguments[1]),
                    };
                    yield break;
                }
            }

            // Custom types
            yield return this.CreateRef(contextualType);
        }

        private bool IsStringish(ContextualType contextualType)
        {
            // Check type
            var innerType = contextualType.Type;
            if (innerType == typeof(string))
            {
                return true;
            }

            // Check type converters
            return TypeDescriptor.GetConverter(innerType) is { } converter
                && converter.CanConvertTo(typeof(string))
                && converter.CanConvertFrom(typeof(string));
        }

        private JObject CreateRef(ContextualType contextualType)
        {
            var definitionName = this.GetDefinitionName(contextualType.Type);

            // Check if already defined
            if (this.Definitions.ContainsKey(definitionName))
            {
                return this.CreateRefObject(definitionName);
            }

            // Check if known definition
            if (this.knownDefinitions.Remove(definitionName, out var knownDefinition))
            {
                this.Definitions[definitionName] = knownDefinition;
                return this.CreateRefObject(definitionName);
            }

            // Set the definition for now to handle recursion
            this.Definitions[definitionName] = new();
            this.Definitions[definitionName] = this.CreateCustomTypeSchema(contextualType);
            return this.CreateRefObject(definitionName);
        }

        private JObject CreateRefObject(string definitionName)
        {
            return new() { ["$ref"] = $"#/definitions/{definitionName}" };
        }

        private JObject CreateCustomTypeSchema(ContextualType contextualType)
        {
            var type = contextualType.Type;
            var typeDescription = type.GetCustomAttributes<DescriptionAttribute>(false)
                .Select(attr => attr.Description)
                .FirstOrDefault();
            typeDescription ??= type.GetXmlDocsSummary().Replace("\n", " ");

            return type switch
            {
                { IsEnum: true } => GenerateForEnum(),
                { IsClass: true } or { IsValueType: true } => GenerateForClassOrStruct(),
                _ => throw new InvalidOperationException(),
            };

            JObject GenerateForEnum()
            {
                // Basic information
                var result = new JObject { ["type"] = "string" };
                if (!string.IsNullOrWhiteSpace(typeDescription))
                {
                    result["description"] = typeDescription;
                }

                // Enum variants
                var variants = new JArray();
                result["enum"] = variants;
                foreach (var variantName in type.GetEnumNames())
                {
                    variants.Add(variantName);
                }

                return result;
            }

            JObject GenerateForClassOrStruct()
            {
                // Basic information
                var result = new JObject
                {
                    ["type"] = "object",
                    ["additionalProperties"] = false,
                };
                if (!string.IsNullOrWhiteSpace(typeDescription))
                {
                    result["description"] = typeDescription;
                }

                var required = new HashSet<string>();
                var properties = new JObject();

                // Properties
                var members = DefinitionMap.GetHierarchy(contextualType)
                    .SelectMany(
                        checkedType => checkedType.Fields.Select(field => new MemberData(field))
                            .Concat(checkedType.Properties.Select(prop => new MemberData(prop)))
                            // Ignore static members
                            .Where(member => !member.IsStatic)
                            // Ignore non-public members with no [JsonProperty] attribute
                            .Where(
                                member => member.IsFullyPublic
                                    || member.Accessor.MemberInfo
                                        .GetCustomAttributes<JsonPropertyAttribute>()
                                        .Any()
                            )
                            // Ignore members with [JsonIgnore]
                            .Where(
                                member => !member.Accessor.MemberInfo
                                    .GetCustomAttributes<JsonIgnoreAttribute>()
                                    .Any()
                            )
                    );
                foreach (var memberData in members)
                {
                    // Get property name
                    var name =
                        memberData.Accessor.MemberInfo.GetCustomAttributes<JsonPropertyAttribute>()
                            .Select(attr => attr.PropertyName)
                            .FirstOrDefault()
                        ?? memberData.Accessor.MemberInfo.Name;

                    // Mark property as required if needed
                    var isRequired = memberData.Accessor.MemberInfo
                        .GetCustomAttributes<JsonRequiredAttribute>(true)
                        .Any();
                    if (isRequired)
                    {
                        required.Add(name);
                    }

                    // Create property schema
                    var propSchema = this.CreatePropertySchema(memberData);
                    properties[name] = propSchema;
                }

                if (required.Count > 0)
                {
                    result["required"] = new JArray(required.Cast<object>().ToArray());
                }

                if (properties.Count > 0)
                {
                    result["properties"] = properties;
                }

                return result;
            }
        }

        private static IEnumerable<ContextualType> GetHierarchy(ContextualType contextualType)
        {
            yield return contextualType;
            while (contextualType is { BaseType: { } baseType })
            {
                yield return baseType;
                contextualType = baseType;
            }
        }

        private JObject CreatePropertySchema(MemberData member)
        {
            // Create raw schema
            var schema = this.Register(member.Accessor.AccessorType);

            // Add description
            var description = member.Accessor.MemberInfo
                .GetCustomAttributes<DescriptionAttribute>(true)
                .Select(attr => attr.Description)
                .FirstOrDefault();
            description ??= member.Accessor.GetXmlDocsSummary().Replace("\n", " ");
            if (!string.IsNullOrWhiteSpace(description))
            {
                schema["description"] = description;
            }

            // Add default value
            var defaultValue = member.Accessor.MemberInfo
                .GetCustomAttributes<DefaultValueAttribute>(true)
                .FirstOrDefault();
            if (defaultValue is { Value: var val })
            {
                schema["default"] = val is not null ? JToken.FromObject(val) : JValue.CreateNull();
            }

            return schema;
        }
    }
}