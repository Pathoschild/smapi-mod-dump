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
using StardewValley.Menus;
using DeluxeJournal.Events;
using DeluxeJournal.Framework.Events;

namespace DeluxeJournal.Patching
{
    /// <summary>Patches for <see cref="PurchaseAnimalsMenu"/>.</summary>
    internal class PurchaseAnimalsMenuPatch : PatchBase<PurchaseAnimalsMenuPatch>
    {
        private EventManager EventManager { get; }

        public PurchaseAnimalsMenuPatch(EventManager eventManager, IMonitor monitor) : base(monitor)
        {
            EventManager = eventManager;
            Instance = this;
        }

        public static void Prefix_setUpForReturnAfterPurchaseAnimal(PurchaseAnimalsMenu __instance)
        {
            try
            {
                if (__instance.animalBeingPurchased is FarmAnimal purchasedAnimal)
                {
                    var eventArgs = new FarmAnimalEventArgs(Game1.player, purchasedAnimal.type.Value, purchasedAnimal.GetAnimalData());
                    Instance.EventManager.FarmAnimalPurchased.Raise(null, eventArgs);
                }
            }
            catch (Exception ex)
            {
                Instance.LogError(ex, nameof(Prefix_setUpForReturnAfterPurchaseAnimal));
            }
        }

        public override void Apply(Harmony harmony)
        {
            Patch(harmony,
                original: AccessTools.Method(typeof(PurchaseAnimalsMenu), nameof(PurchaseAnimalsMenu.setUpForReturnAfterPurchasingAnimal)),
                postfix: new HarmonyMethod(typeof(PurchaseAnimalsMenuPatch), nameof(Prefix_setUpForReturnAfterPurchaseAnimal))
            );
        }
    }
}
