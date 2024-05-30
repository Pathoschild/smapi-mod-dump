/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;
using DeluxeJournal.Events;
using DeluxeJournal.Framework.Events;

namespace DeluxeJournal.Patching
{
    /// <summary>Patches for <see cref="CarpenterMenu"/>.</summary>
    internal class CarpenterMenuPatch : PatchBase<CarpenterMenuPatch>
    {
        private EventManager EventManager { get; }

        public CarpenterMenuPatch(EventManager eventManager, IMonitor monitor) : base(monitor)
        {
            EventManager = eventManager;
            Instance = this;
        }

        private static void Postfix_returnToCarpentryMenuAfterSuccessfulBuild(CarpenterMenu __instance, bool ___upgrading)
        {
            try
            {
                // Only handle upgrades, since ModEvents.World.BuildingListChanged is used to broadly detect additions
                if (___upgrading && Game1.GetBuildingUnderConstruction() is Building building)
                {
                    Instance.EventManager.BuildingConstructed.Broadcast(new BuildingConstructedEventArgs(Game1.getFarm(), building, true));
                }
            }
            catch (Exception ex)
            {
                Instance.LogError(ex, nameof(Postfix_returnToCarpentryMenuAfterSuccessfulBuild));
            }
        }

        public override void Apply(Harmony harmony)
        {
            Patch(harmony,
                original: AccessTools.Method(typeof(CarpenterMenu), nameof(CarpenterMenu.returnToCarpentryMenuAfterSuccessfulBuild)),
                postfix: new HarmonyMethod(typeof(CarpenterMenuPatch), nameof(Postfix_returnToCarpentryMenuAfterSuccessfulBuild))
            );
        }
    }
}
