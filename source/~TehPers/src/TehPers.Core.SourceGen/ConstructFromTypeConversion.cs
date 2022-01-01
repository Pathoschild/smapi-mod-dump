/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TehPers.Core.SourceGen
{
    internal class ConstructFromTypeConversion
    {
        public TypeDeclarationSyntax TypeDeclaration { get; }

        public TypeSyntax FromType { get; }

        public ConstructFromTypeConversion(TypeDeclarationSyntax typeDeclaration, TypeSyntax fromType)
        {
            this.TypeDeclaration = typeDeclaration;
            this.FromType = fromType;
        }
    }
}
