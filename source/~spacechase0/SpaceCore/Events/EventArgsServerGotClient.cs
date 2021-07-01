/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System.Diagnostics.CodeAnalysis;
using SpaceShared;

namespace SpaceCore.Events
{
    public class EventArgsServerGotClient
    {
        [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = DiagnosticMessages.IsPublicApi)]
        [SuppressMessage("ReSharper", "InconsistentNaming", Justification = DiagnosticMessages.IsPublicApi)]
        public long FarmerID { get; set; }
    }
}
