/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using HarmonyLib;
using StardewArchipelago.Archipelago;
using StardewModdingAPI;
using StardewValley;

namespace StardewArchipelago.Items
{
    public class ItemPatcher
    {
        private readonly Harmony _harmony;

        public ItemPatcher(IMonitor monitor, IModHelper helper, Harmony harmony, ArchipelagoClient archipelago)
        {
            _harmony = harmony;
            PlayerBuffInjections.Initialize(monitor, helper, archipelago);
        }

        public void PatchApItems()
        {
            PatchPlayerBuffs();
        }

        private void PatchPlayerBuffs()
        {
            _harmony.Patch(
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.getMovementSpeed)),
                postfix: new HarmonyMethod(typeof(PlayerBuffInjections), nameof(PlayerBuffInjections.GetMovementSpeed_AddApBuffs_Postfix))
            );
            _harmony.Patch(
                original: AccessTools.Method(typeof(Farmer), typeof(Farmer).GetProperty(nameof(Farmer.DailyLuck)).GetGetMethod().Name),
                postfix: new HarmonyMethod(typeof(PlayerBuffInjections), nameof(PlayerBuffInjections.DailyLuck_AddApBuffs_Postfix))
            );
        }
    }
}
