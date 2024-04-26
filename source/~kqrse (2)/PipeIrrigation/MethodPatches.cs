/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/kqrse/StardewValleyMods-aedenthorn
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Network;
using System.Collections.Generic;

namespace PipeIrrigation
{
    public partial class ModEntry
    {
        public static void FarmAnimal_dayUpdate_Postfix(FarmAnimal __instance, GameLocation environtment)
        {
            if (!Config.EnableMod)
                return;

        }
    }
}