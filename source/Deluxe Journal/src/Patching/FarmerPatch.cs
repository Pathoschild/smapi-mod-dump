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
using StardewValley.Objects;
using StardewValley.Quests;
using DeluxeJournal.Events;
using DeluxeJournal.Framework.Events;

namespace DeluxeJournal.Patching
{
    /// <summary>Patches for <see cref="Farmer"/>.</summary>
    internal class FarmerPatch : PatchBase<FarmerPatch>
    {
        private EventManager EventManager { get; }

        public FarmerPatch(EventManager eventManager, IMonitor monitor) : base(monitor)
        {
            EventManager = eventManager;
            Instance = this;
        }

        private static void Prefix_OnItemReceived(Farmer __instance, Item item, int countAdded, Item mergedIntoStack, bool hideHudNotification = false)
        {
            try
            {
                if (__instance.IsLocalPlayer && ((item is SObject obj && !obj.HasBeenInInventory) || item is Ring))
                {
                    Instance.EventManager.ItemCollected.Raise(__instance, new ItemReceivedEventArgs(__instance, item, countAdded));
                }
            }
            catch (Exception ex)
            {
                Instance.LogError(ex, nameof(Prefix_OnItemReceived));
            }
        }

        private static void Postfix_onGiftGiven(Farmer __instance, NPC npc, SObject item)
        {
            try
            {
                Instance.EventManager.ItemGifted.Raise(__instance, new GiftEventArgs(__instance, npc, item));
            }
            catch (Exception ex)
            {
                Instance.LogError(ex, nameof(Postfix_onGiftGiven));
            }
        }

        private static void Postfix_checkForQuestComplete(Farmer __instance, NPC n, int number1, int number2, Item item, string str, int questType)
        {
            try
            {
                if (questType == Quest.type_crafting && (item is SObject || item is Ring))
                {
                    Instance.EventManager.ItemCrafted.Raise(__instance, new ItemReceivedEventArgs(__instance, item, item.Stack));
                }
            }
            catch (Exception ex)
            {
                Instance.LogError(ex, nameof(Postfix_checkForQuestComplete));
            }
        }

        public override void Apply(Harmony harmony)
        {
            Patch(harmony,
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.OnItemReceived)),
                prefix: new HarmonyMethod(typeof(FarmerPatch), nameof(Prefix_OnItemReceived))
            );

            Patch(harmony,
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.onGiftGiven)),
                postfix: new HarmonyMethod(typeof(FarmerPatch), nameof(Postfix_onGiftGiven))
            );

            Patch(harmony,
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.checkForQuestComplete)),
                postfix: new HarmonyMethod(typeof(FarmerPatch), nameof(Postfix_checkForQuestComplete))
            );
        }
    }
}
