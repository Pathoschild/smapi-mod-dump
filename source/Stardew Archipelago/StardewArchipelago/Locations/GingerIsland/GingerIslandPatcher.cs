/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Locations.CodeInjections;
using StardewArchipelago.Locations.GingerIsland.Boat;
using StardewArchipelago.Locations.GingerIsland.Parrots;
using StardewArchipelago.Locations.GingerIsland.WalnutRoom;
using StardewArchipelago.Stardew;
using StardewModdingAPI;
using StardewValley.Locations;

namespace StardewArchipelago.Locations.GingerIsland
{
    public class GingerIslandPatcher
    {
        private readonly ArchipelagoClient _archipelago;
        private readonly Harmony _harmony;
        private readonly IParrotReplacer[] _parrotReplacers;

        public GingerIslandPatcher(IMonitor monitor, IModHelper modHelper, Harmony harmony, ArchipelagoClient archipelago, LocationChecker locationChecker)
        {
            _archipelago = archipelago;
            _harmony = harmony;
            GingerIslandInitializer.Initialize(monitor, modHelper, _archipelago, locationChecker);
            _parrotReplacers = new IParrotReplacer[]
            {
                new IslandHutInjections(), new IslandNorthInjections(),
                new IslandSouthInjections(), new IslandWestInjections()
            };
        }

        public void PatchGingerIslandLocations()
        {
            if (_archipelago.SlotData.ExcludeGingerIsland)
            {
                return;
            }

            UnlockWalnutRoomBasedOnApItem();
            ReplaceBoatRepairWithChecks();
            ReplaceParrotsWithChecks();

        }

        private void UnlockWalnutRoomBasedOnApItem()
        {
            _harmony.Patch(
                original: AccessTools.Method(typeof(IslandWest), nameof(IslandWest.checkAction)),
                prefix: new HarmonyMethod(typeof(WalnutRoomDoorInjection), nameof(WalnutRoomDoorInjection.CheckAction_WalnutRoomDoorBasedOnAPItem_Prefix))
            );
        }

        private void ReplaceBoatRepairWithChecks()
        {
            _harmony.Patch(
                original: AccessTools.Method(typeof(BoatTunnel), nameof(BoatTunnel.checkAction)),
                prefix: new HarmonyMethod(typeof(BoatTunnelInjections), nameof(BoatTunnelInjections.CheckAction_BoatRepairAndUsage_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(BoatTunnel), nameof(BoatTunnel.answerDialogue)),
                prefix: new HarmonyMethod(typeof(BoatTunnelInjections), nameof(BoatTunnelInjections.AnswerDialogue_BoatRepairAndUsage_Prefix))
            );
        }

        private void ReplaceParrotsWithChecks()
        {
            foreach (var parrotReplacer in _parrotReplacers)
            {
                parrotReplacer.ReplaceParrots();
            }

            _harmony.Patch(
                original: AccessTools.Method(typeof(VolcanoDungeon), nameof(VolcanoDungeon.GenerateContents)),
                postfix: new HarmonyMethod(typeof(VolcanoDungeonInjections), nameof(VolcanoDungeonInjections.GenerateContents_ReplaceParrots_Postfix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(IslandWest), nameof(IslandWest.ApplyFarmHouseRestore)),
                prefix: new HarmonyMethod(typeof(IslandWestInjections), nameof(IslandWestInjections.ApplyFarmHouseRestore_RestoreOnlyCorrectParts_Prefix))
            );
        }
    }
}
