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

namespace TehPers.Core.SourceGen
{
    internal static class GeneratorExecutionContextExtensions
    {
    
        public static void ReportInternalError(
            this GeneratorExecutionContext context,
            LocalizableString message,
            DiagnosticSeverity severity = DiagnosticSeverity.Error,
            Location? location = null
        )
        {
            context.ReportError(
                Constants.InternalErrorId,
                message,
                severity,
                location
            );
        }

        public static void ReportError(
            this GeneratorExecutionContext context,
            string id,
            LocalizableString message,
            DiagnosticSeverity severity = DiagnosticSeverity.Error,
            Location? location = null
        )
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    id,
                    "Compiler",
                    message,
                    severity,
                    severity,
                    true,
                    0,
                    location: location
                )
            );
        }
    }
}