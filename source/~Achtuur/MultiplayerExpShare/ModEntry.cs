/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewTravelSkill
**
*************************************************/

using AchtuurCore.Events;
using StardewModdingAPI;
using StardewValley;
using System;

namespace MultiplayerExpShare
{
    public class ModEntry : Mod
    {
        public static ModEntry Instance;

        public static Farmer[] GetNearbyPlayers() 
        {
            // return all players that are close to the main player
        }

        public override void Entry(IModHelper helper)
        {

            HarmonyPatcher.ApplyPatches(
                new GainExperiencePatch()
            );

            ModEntry.Instance = this;
            I18n.Init(helper.Translation);

        }

    }
}
