/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Stardew-Valley-Modding/Bookcase
**
*************************************************/

using Bookcase.Events;
using StardewValley;
using System;
using System.Reflection;

namespace Bookcase.Patches {

    class PatchGainExperience : IGamePatch {

        public Type TargetType => typeof(Farmer);

        public MethodBase TargetMethod => TargetType.GetMethod("gainExperience");

        public static bool Prefix(Farmer __instance, ref int which, ref int howMuch) {

            FarmerGainExperienceEvent evt = new FarmerGainExperienceEvent(__instance, which, howMuch);
            bool canceled = BookcaseEvents.OnSkillEXPGain.Post(evt);

            which = evt.SkillType;
            howMuch = evt.Amount;

            // If event was canceled, or EXP is less than 0, prevent exp gain.
            return !canceled && evt.Amount > 0;
        }
    }
}