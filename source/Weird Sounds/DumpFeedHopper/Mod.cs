/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/alcmoe/SVMods
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using Object = StardewValley.Object;

namespace DumpFeedHopper;

public class Mod: StardewModdingAPI.Mod
{
    public override void Entry(IModHelper helper)
    {
        var harmony = new Harmony(ModManifest.UniqueID);
        harmony.Patch(
            original: AccessTools.Method(typeof(Object), "CheckForActionOnFeedHopper"),
            prefix: new HarmonyMethod(typeof(Game1Patcher), nameof(Game1Patcher.FeedHopperPrefix))
        );
    }

    private class Game1Patcher
    {
        public static bool FeedHopperPrefix(Object __instance, ref bool __result, Farmer who, bool justCheckingForActivity)
        {
            __result = CheckForActionOnFeedHopper();
            return false;
            bool CheckForActionOnFeedHopper() {
                if (justCheckingForActivity) {
                    return true;
                }
                if (who.ActiveObject != null) {
                    return false;
                }
                if (who.freeSpotsInInventory() > 0) {
                    var location = __instance.Location;
                    var rootLocation = location.GetRootLocation();
                    var piecesHay = rootLocation.piecesOfHay.Value;
                    if (piecesHay > 0) {
                        if (location is AnimalHouse i) {
                            var piecesOfHayToRemove = Math.Min(i.animalsThatLiveHere.Count, piecesHay);
                            piecesOfHayToRemove = Math.Max(1, piecesOfHayToRemove);
                            var alreadyHay = i.numberOfObjectsWithName("Hay");
                            piecesOfHayToRemove = alreadyHay == i.animalLimit.Value ? Math.Min(i.animalLimit.Value, piecesHay) : Math.Min(piecesOfHayToRemove, i.animalLimit.Value - alreadyHay);
                            if (piecesOfHayToRemove != 0 && Game1.player.couldInventoryAcceptThisItem("(O)178", piecesOfHayToRemove))
                            {
                                rootLocation.piecesOfHay.Value -= Math.Max(1, piecesOfHayToRemove);
                                who.addItemToInventoryBool(ItemRegistry.Create("(O)178", piecesOfHayToRemove));
                                Game1.playSound("shwip");
                            }
                        } else if (Game1.player.couldInventoryAcceptThisItem("(O)178", 1)) {
                            rootLocation.piecesOfHay.Value--;
                            who.addItemToInventoryBool(ItemRegistry.Create("(O)178"));
                            Game1.playSound("shwip");
                        }
                        if (rootLocation.piecesOfHay.Value <= 0) {
                            __instance.showNextIndex.Value = false;
                        }
                        return true;
                    }
                    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12942"));
                } else {
                    Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
                }
                return true;
            }
        }
    }
}