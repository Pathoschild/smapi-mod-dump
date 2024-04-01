/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JoXW100/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;

namespace PlannedParenthood
{
    public class PlannedParenthoodAPI
    {
        public string GetPartnerTonight()
        {
            return ModEntry.Config.ModEnabled ? ModEntry.partnerName : null;
        }
    }
}