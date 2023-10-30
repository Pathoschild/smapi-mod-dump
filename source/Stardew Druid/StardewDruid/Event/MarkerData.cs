/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using StardewDruid.Cast;
using StardewDruid.Map;
using Microsoft.Xna.Framework;
using StardewValley;
using System.Collections.Generic;
using static StardewValley.Minigames.TargetGame;

namespace StardewDruid.Event
{
    public static class MarkerData
    {

        public static MarkerHandle MarkerInstance(Mod Mod, GameLocation location, Quest quest)
        {

            MarkerHandle markerHandle;

            string questName = quest.name;

            questName = questName.Replace("Two", "");

            switch (questName)
            {
                case "challengeEarth":

                    markerHandle = new Event.Trash(Mod, location, quest);

                    break;

                default: //case "challengeEarth":

                    markerHandle = new Event.MarkerHandle(Mod, location, quest);

                    markerHandle.targetVector = quest.vectorList["markerVector"];

                    break;
            }

            return markerHandle;

        }

    }


}
