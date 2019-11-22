using System;
using System.Reflection;
using StardewValley;
using Bookcase.Events;

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