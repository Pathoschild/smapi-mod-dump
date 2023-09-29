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
using StardewArchipelago.Locations.CodeInjections;
using StardewArchipelago.Locations.GingerIsland.Boat;
using StardewArchipelago.Locations.GingerIsland.Parrots;
using StardewArchipelago.Locations.GingerIsland.VolcanoForge;
using StardewArchipelago.Locations.GingerIsland.WalnutRoom;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;

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
                new IslandSouthInjections(), new IslandWestInjections(),
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
            ReplaceFieldOfficeWithChecks();
            ReplaceCalderaWithChecks();
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

            _harmony.Patch(
                original: AccessTools.Method(typeof(BoatTunnel), nameof(BoatTunnel.draw)),
                postfix: new HarmonyMethod(typeof(BoatTunnelInjections), nameof(BoatTunnelInjections.Draw_DrawBoatSectionsBasedOnTasksCompleted_Postfix))
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
                original: AccessTools.Method(typeof(IslandNorth), nameof(IslandNorth.explosionAt)),
                prefix: new HarmonyMethod(typeof(IslandNorthInjections), nameof(IslandNorthInjections.ExplosionAt_CheckProfessorSnailLocation_Prefix))
            );
        }

        private void ReplaceFieldOfficeWithChecks()
        {
            _harmony.Patch(
                original: AccessTools.Method(typeof(Event), nameof(Event.command_addCraftingRecipe)),
                prefix: new HarmonyMethod(typeof(FieldOfficeInjections), nameof(FieldOfficeInjections.CommandAddCraftingRecipe_OstrichIncubator_Prefix))
            );
        }

        private void ReplaceCalderaWithChecks()
        {
            _harmony.Patch(
                original: AccessTools.Method(typeof(Chest), nameof(Chest.checkForAction)),
                prefix: new HarmonyMethod(typeof(CalderaInjections), nameof(CalderaInjections.CheckForAction_CalderaChest_Prefix))
            );
        }
    }
}
