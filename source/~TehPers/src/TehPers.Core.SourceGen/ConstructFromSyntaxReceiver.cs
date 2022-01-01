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
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace TehPers.Core.SourceGen
{
    internal class ConstructFromSyntaxReceiver : ISyntaxReceiver
    {
        public List<ConstructFromTypeConversion> Candidates { get; } = new();

        public List<string> Messages { get; } = new();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is not TypeDeclarationSyntax typeDeclaration)
            {
                return;
            }

            this.Candidates.AddRange(
                typeDeclaration.AttributeLists.SelectMany(attrList => attrList.Attributes)
                    .SelectMany(
                        attr => attr switch
                        {
                            {
                                Name: IdentifierNameSyntax
                                {
                                    Identifier: { ValueText: Constants.ConstructFromAttrShortName }
                                },
                                ArgumentList: { Arguments: { Count: > 0 } arguments }
                            } when arguments[0].Expression is TypeOfExpressionSyntax
                            {
                                Type: { } fromType
                            } => new[]
                            {
                                new ConstructFromTypeConversion(typeDeclaration, fromType)
                            },
                            // {
                            //     Name: QualifiedNameSyntax
                            //     {
                            //         Left: QualifiedNameSyntax
                            //         {
                            //             Right: IdentifierNameSyntax
                            //             {
                            //                 Identifier: { ValueText: "ConstructFrom" }
                            //             }
                            //         },
                            //         Right: IdentifierNameSyntax
                            //         {
                            //             Identifier: { ValueText: "ConvertFrom" }
                            //         }
                            //     }
                            // }
                            _ => Enumerable.Empty<ConstructFromTypeConversion>(),
                        }
                    )
            );
        }
    }
}
