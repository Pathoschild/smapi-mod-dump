/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mushymato/StardewMods
**
*************************************************/

using System.Diagnostics.CodeAnalysis;
using StardewObject = StardewValley.Object;

namespace SprinklerAttachments.API
{
    public interface ISprinklerAttachmentsAPI
    {
        void ApplySowing(StardewObject sprinkler);
        bool TryGetSprinklerAttachment(StardewObject sprinkler, [NotNullWhen(true)] out StardewObject? attachment);
        bool TryGetIntakeChest(StardewObject sprinkler, [NotNullWhen(true)] out StardewObject? attachment);
        bool IsSowing(StardewObject attachment);
        bool IsPressurize(StardewObject attachment);

    }
}
