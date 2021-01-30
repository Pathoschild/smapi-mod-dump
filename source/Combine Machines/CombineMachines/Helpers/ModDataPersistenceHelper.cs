/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Videogamers0/SDV-CombineMachines
**
*************************************************/

using Harmony;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SObject = StardewValley.Object;

namespace CombineMachines.Helpers
{
    /// <summary>Several chunks of game code create copies of an item without preserving the modData that was attached to the item instance. 
    /// This class is intended to ensure that particular KeyValuePairs in the <see cref="Item.modData"/> dictionary are preserved.<para/>
    /// For example, modData is normally lost when a machine is placed or un-placed on a tile, or when right-clicking an item in your inventory to grab 1 quantity of it.</summary>
    public static class ModDataPersistenceHelper
    {
        private static ReadOnlyCollection<string> TrackedKeys = new List<string>().AsReadOnly();

        /// <summary>Intended to be invoked once during the mod entry</summary>
        /// <param name="ModDataKeys">The Keys of the ModData KeyValuePairs that should be maintained.</param>
        internal static void Entry(IModHelper helper, params string[] ModDataKeys)
        {
            TrackedKeys = ModDataKeys.Distinct().ToList().AsReadOnly();

            //  EDIT: These 2 event listeners probably aren't needed anymore? I think a hotfix update added similar logic to the vanilla game somewhere around Update 1.5.1 or 1.5.2,
            helper.Events.GameLoop.UpdateTicking += GameLoop_UpdateTicking;
            helper.Events.World.ObjectListChanged += World_ObjectListChanged;

            HarmonyInstance Harmony = HarmonyInstance.Create(ModEntry.ModInstance.ModManifest.UniqueID);

            //  EDIT: This patch probably isn't needed anymore? I think a hotfix update added similar logic to the vanilla game somewhere around Update 1.5.1 or 1.5.2
            //  Patch Item.getOne to copy the modData to the return value
            Harmony.Patch(
                original: AccessTools.Method(typeof(SObject), nameof(SObject.getOne)),
                postfix: new HarmonyMethod(typeof(GetOnePatch), nameof(GetOnePatch.Postfix))
            );
        }

        [HarmonyPatch(typeof(SObject), nameof(SObject.getOne))]
        public static class GetOnePatch
        {
            public static void Postfix(SObject __instance, Item __result)
            {
                try
                {
                    CopyTrackedModData(__instance, __result);
                }
                catch (Exception ex)
                {
                    ModEntry.Logger.Log(string.Format("Unhandled Error in {0}.{1}:\n{2}", nameof(GetOnePatch), nameof(Postfix), ex), LogLevel.Error);
                }
            }
        }

        private static Item PreviousHeldItem = null;
        private static void GameLoop_UpdateTicking(object sender, UpdateTickingEventArgs e)
        {
            PreviousHeldItem = Game1.player.CurrentItem;
        }

        private static void World_ObjectListChanged(object sender, ObjectListChangedEventArgs e)
        {
            //  Detect when a player places a machine 
            //  (Game1.player.CurrentItem changes from non-null to null, and ObjectListChangedEventArgs.Added should have a corresponding new item placed to a tile)
            if (e.Location == Game1.currentLocation)
            {
                if (PreviousHeldItem != null && PreviousHeldItem is SObject PreviousHeldObject && Game1.player.CurrentItem == null)
                {
                    foreach (var KVP in e.Added)
                    {
                        Vector2 Location = KVP.Key;
                        SObject Item = KVP.Value;

                        if (Item.bigCraftable.Value == PreviousHeldObject.bigCraftable.Value && Item.ParentSheetIndex == PreviousHeldObject.ParentSheetIndex)
                        {
                            CopyTrackedModData(PreviousHeldObject, Item);
                        }
                    }
                }
            }
        }

        private static void CopyTrackedModData(Item Source, Item Target)
        {
            if (Source?.modData != null && Target?.modData != null)
            {
                foreach (string ModDataKey in TrackedKeys)
                {
                    if (Source.modData.TryGetValue(ModDataKey, out string Value))
                    {
                        Target.modData[ModDataKey] = Value;
                    }
                }
            }
        }
    }
}
