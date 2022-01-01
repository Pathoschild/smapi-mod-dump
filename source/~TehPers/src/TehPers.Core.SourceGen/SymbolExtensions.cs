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
using System.Collections.Generic;

namespace TehPers.Core.SourceGen
{
    internal static class SymbolExtensions
    {
        public static string GetFullyQualifiedName(this ISymbol symbol)
        {
            var parts = new Stack<string>();
            parts.Push(symbol.Name);

            if (symbol.ContainingSymbol is {} containingSymbol)
            {
                parts.Push(containingSymbol.Name);
                symbol = containingSymbol;
            }

            while (symbol.ContainingNamespace is { IsGlobalNamespace: false } namespaceSymbol)
            {
                parts.Push(namespaceSymbol.Name);
                symbol = namespaceSymbol;
            }

            return string.Join(".", parts);
        }

        public static IEnumerable<T> GetAllMembers<T>(this ITypeSymbol symbol)
            where T : ISymbol
        {
            var curSymbol = (ITypeSymbol?)symbol;
            while (curSymbol is not null)
            {
                foreach (var member in curSymbol.GetMembers().OfType<T>())
                {
                    yield return member;
                }

                curSymbol = curSymbol.BaseType;
            }
        }
    }
}
