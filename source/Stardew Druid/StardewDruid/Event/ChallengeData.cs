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

namespace StardewDruid.Event
{
    public static class ChallengeData
    {

        public static ChallengeHandle ChallengeInstance(Mod Mod, Vector2 target, Rite rite, Quest quest)
        {

            ChallengeHandle challengeHandle;

            string questName = quest.name;

            questName = questName.Replace("Two", "");

            switch (questName)
            {

                case "challengeCanoli":

                    challengeHandle = new Event.Canoli(Mod, target, rite, quest);

                    break;

                case "challengeMariner":

                    challengeHandle = new Event.Mariner(Mod, target, rite, quest);

                    break;

                case "challengeSandDragon": // figureSandDragon

                    challengeHandle = new Event.SandDragon(Mod, target, rite, quest);

                    break;

                case "challengeStars":

                    challengeHandle = new Event.Infestation(Mod, target, rite, quest);

                    break;

                case "challengeWater":

                    challengeHandle = new Event.Graveyard(Mod, target, rite, quest);

                    break;

                case "challengeGemShrine":

                    challengeHandle = new Event.GemShrine(Mod, target, rite, quest);

                    break;

                default: //case "challengeEarth":

                    challengeHandle = new Event.Aquifer(Mod, target, rite, quest);

                    break;
            }

            return challengeHandle;

        }

    }

}
