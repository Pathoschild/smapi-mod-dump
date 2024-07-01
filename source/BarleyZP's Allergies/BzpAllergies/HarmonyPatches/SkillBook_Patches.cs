/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lisyce/SDV_Allergies_Mod
**
*************************************************/

using HarmonyLib;
using StardewValley.Menus;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;
using StardewModdingAPI;

namespace BZP_Allergies.HarmonyPatches
{
    internal class SkillBook_Patches
    {
        public static void Patch(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Object), "readBook"),
                postfix: new HarmonyMethod(typeof(SkillBook_Patches), nameof(ReadBook_Postfix))
            );
        }

        public static void ReadBook_Postfix(ref StardewValley.Object __instance)
        {
            try
            {
                string bookId = Traverse.Create(__instance).Property("ItemId").GetValue<string>();

                if (bookId == Constants.AllergyTeachBookId)
                {
                    if (!AllergenManager.ModDataGet(Game1.player, Constants.ModDataRandom, out string val) || val == "false")
                    {
                        return;  // we don't discover allergies
                    }

                    ISet<string> playerHas = AllergenManager.ModDataSetGet(Game1.player, Constants.ModDataHas);
                    ISet<string> playerDiscovered = AllergenManager.ModDataSetGet(Game1.player, Constants.ModDataDiscovered);

                    if (playerDiscovered.Count == playerHas.Count) return;

                    // pick a random one to discover
                    List<string> diff = playerHas.Except(playerDiscovered).ToList();
                    int discoverIdx = new Random().Next(0, diff.Count);
                    AllergenManager.DiscoverPlayerAllergy(diff[discoverIdx]);

                    if (!Game1.player.mailReceived.Contains("read_a_book"))
                    {
                        Game1.player.mailReceived.Add("read_a_book");
                    }

                    Game1.showGlobalMessage(ModEntry.Instance.Translation.Get("books.allergy-teach"));
                }
                else if (bookId == Constants.AllergyCookbookId)
                {
                    Game1.player.cookingRecipes.TryAdd(Constants.PlantMilkId, 0);
                    if (!Game1.player.mailReceived.Contains("read_a_book"))
                    {
                        Game1.player.mailReceived.Add("read_a_book");
                    }

                    Game1.showGlobalMessage(ModEntry.Instance.Translation.Get("books.allergy-cookbook"));

                }
            }
            catch (Exception ex)
            {
                ModEntry.Instance.Monitor.Log($"Failed in {nameof(ReadBook_Postfix)}:\n{ex}", LogLevel.Error);
            }
        }
    }
}
