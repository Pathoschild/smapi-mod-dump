using System;
using Harmony;
using StardewModdingAPI;
using StardewValley;
using System.Reflection;
using StardewModdingAPI.Events;

namespace MultipleGiftsForSpouse
{
    public class ModEntry : Mod
    {
        public static ModConfig config;
        public static IMonitor monitor;

        public override void Entry(IModHelper helper)
        {
            config = Helper.ReadConfig<ModConfig>();
            monitor = Monitor;

            HarmonyInstance harmony = HarmonyInstance.Create("Redwood.MultipleGiftsForSpouse");
            Type[] types = new Type[]{typeof(Farmer)};
            MethodInfo originalMethod = typeof(NPC).GetMethod("tryToReceiveActiveObject");
            MethodInfo patchingMethod0 = typeof(PatchedSpouseGiftLimit).GetMethod("Prefix");
            MethodInfo patchingMethod1 = typeof(PatchedSpouseGiftLimit).GetMethod("Postfix");
            harmony.Patch(originalMethod, new HarmonyMethod(patchingMethod0), new HarmonyMethod(patchingMethod1));

            TimeEvents.AfterDayStarted += TimeEvents_AfterDayStarted;
        }

        private void TimeEvents_AfterDayStarted(object sender, EventArgs e)
        {
            PatchedSpouseGiftLimit.giftsGiven = 0;
        }
    }

    class PatchedSpouseGiftLimit
    {
        public static bool isGiftableSpouse;
        public static int giftsGiven;

        static public void Prefix(NPC __instance, Farmer who)
        {
            isGiftableSpouse = (who.spouse != null && who.spouse.Equals(__instance.Name)) &&
                               who.friendshipData[__instance.Name].GiftsToday == 0;
        }

        static public void Postfix(NPC __instance, Farmer who)
        {
            if (isGiftableSpouse && who.friendshipData[__instance.Name].GiftsToday == 1)
            {
                giftsGiven += 1;
                if (giftsGiven < ModEntry.config.giftLimit)
                    who.friendshipData[__instance.Name].GiftsToday = 0;
            }
        }
    }
}
