/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TehPers.Core.SourceGen
{
    [Generator]
    public class ConstructFromGenerator : ISourceGenerator
    {
        private const string constructFromAttrSource = $@"
// @generated

using System;

namespace TehPers.Core.SourceGen.ConstructFrom
{{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true, Inherited = false)]
    internal sealed class {Constants.ConstructFromAttrFullName} : Attribute
    {{
        public {Constants.ConstructFromAttrFullName}(Type fromType) {{
            // Intentionally left empty
        }}
    }}
}}
";

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new ConstructFromSyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            // Add the attribute declaration
            context.AddSource("constructFromAttr", ConstructFromGenerator.constructFromAttrSource);

            // Get the syntax receiver
            if (context.SyntaxReceiver is not ConstructFromSyntaxReceiver syntaxReceiver)
            {
                context.ReportInternalError("Unexpected error: could not retrieve the syntax.");
                return;
            }

            // Add the conversions
            var messages = new List<string>(syntaxReceiver.Messages);
            foreach (var candidate in syntaxReceiver.Candidates)
            {
                messages.Add(
                    $"Candidate type found: {candidate.TypeDeclaration.Identifier.ValueText} from {candidate.FromType}"
                );

                // Get the semantic model of the type declaration
                var semanticModel =
                    context.Compilation.GetSemanticModel(candidate.TypeDeclaration.SyntaxTree);
                var typeDeclarationSymbol = semanticModel.GetDeclaredSymbol(
                    candidate.TypeDeclaration,
                    context.CancellationToken
                );
                if (typeDeclarationSymbol is null)
                {
                    context.ReportInternalError(
                        "Unexpected error: could not get type information for fromType.",
                        location: candidate.FromType.GetLocation()
                    );
                    continue;
                }

                // TODO: reject generic types - not yet implemented
                if (typeDeclarationSymbol.IsGenericType)
                {
                    context.ReportInternalError(
                        "Generic types are currently not supported.",
                        location: candidate.FromType.GetLocation()
                    );
                    continue;
                }

                // Get the target type's symbol
                var fromTypeSymbolInfo = semanticModel.GetSymbolInfo(
                    candidate.FromType,
                    context.CancellationToken
                );
                if (fromTypeSymbolInfo.Symbol is not INamedTypeSymbol fromTypeSymbol)
                {
                    context.ReportInternalError(
                        "Unexpected error: could not get type information for fromType.",
                        location: candidate.FromType.GetLocation()
                    );
                    continue;
                }

                // Reject unbound generic types
                if (fromTypeSymbol.IsUnboundGenericType)
                {
                    context.ReportError(
                        Constants.UnboundGenericErrorId,
                        "Unbound generic types are not supported.",
                        location: candidate.FromType.GetLocation()
                    );
                    continue;
                }

                // Type kind name
                var typeKindName = typeDeclarationSymbol switch
                {
                    { IsRecord: true } => "record",
                    { TypeKind: TypeKind.Class } => "class",
                    { TypeKind: TypeKind.Struct } => "struct",
                    _ => null
                };
                if (typeKindName is null)
                {
                    context.ReportInternalError(
                        "Unexpected error: unsupported type kind.",
                        location: candidate.TypeDeclaration.Keyword.GetLocation()
                    );
                    continue;
                }

                // Mapped properties
                var fromTypeProperties = fromTypeSymbol.GetAllMembers<IPropertySymbol>()
                    .Where(p => !p.IsImplicitlyDeclared)
                    .ToList();
                var targetProperties = typeDeclarationSymbol.GetAllMembers<IPropertySymbol>()
                    .Where(p => !p.IsImplicitlyDeclared)
                    .ToList();
                var mappedProperties = targetProperties.Where(
                    property => fromTypeProperties.Any(
                        p => string.Equals(p.Name, property.Name, StringComparison.Ordinal)
                    )
                );
                var visitiedProperties = new HashSet<string>();
                var propertyMappings = new StringBuilder();
                var parameterMappings = new List<string>();
                foreach (var property in mappedProperties)
                {
                    // Avoid visiting the same property multiple times
                    if (!visitiedProperties.Add(property.Name))
                    {
                        continue;
                    }

                    // Check for record primary constructor parameters
                    if (candidate.TypeDeclaration is RecordDeclarationSyntax
                        {
                            ParameterList: { Parameters: { } recordParams }
                        }
                        && recordParams.Any(
                            param => string.Equals(
                                param.Identifier.ValueText,
                                property.Name,
                                StringComparison.Ordinal
                            )
                        ))
                    {
                        messages.Add(
                            $"Mapping record parameter: {property.GetFullyQualifiedName()}"
                        );
                        parameterMappings.Add($"from.{property.Name}");
                        continue;
                    }

                    // Create property mapping
                    messages.Add($"Mapping property: {property.GetFullyQualifiedName()}");
                    propertyMappings.AppendLine($"{property.Name} = from.{property.Name},");
                }

                // Create the constructor
                var targetFqn = typeDeclarationSymbol.GetFullyQualifiedName();
                var fromFqn = fromTypeSymbol.GetFullyQualifiedName();
                var constructInstance = parameterMappings.Any()
                    ? $"new {typeDeclarationSymbol.Name}({string.Join(", ", parameterMappings)})"
                    : $"new {typeDeclarationSymbol.Name}()";
                context.AddSource(
                    $"constructFrom_{fromFqn}_to_{targetFqn}",
                    @$"
// @generated

using System;

namespace {typeDeclarationSymbol.ContainingNamespace.GetFullyQualifiedName()} {{
    partial {typeKindName} {typeDeclarationSymbol.Name} {{
        /// <summary>
        /// Initializes a new instance of the <see cref=""{typeDeclarationSymbol.Name}""/> {typeKindName}
        /// from an instance of <see cref=""{fromFqn}""/>.
        /// </summary>
        /// <param name=""from"">The instance to convert from.</param>
        public static {typeDeclarationSymbol.Name} From({fromFqn} from) {{
            return {constructInstance} {{
                {string.Join("\n", propertyMappings)}
            }};
        }}
    }}
}}
"
                );
            }

            // Add messages
            context.AddSource(
                "constructFrom_messages",
                $@"
// @generated

/*
{string.Join("\n", messages.Select(msg => msg.Replace("*/", "* /")))}
*/"
            );
        }
    }
}
