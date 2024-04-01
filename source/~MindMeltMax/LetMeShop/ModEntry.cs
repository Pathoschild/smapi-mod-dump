/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile.Dimensions;

namespace LetMeShop
{
    internal class ModEntry : Mod
    {
        private static IMonitor IMonitor;
        private static Config IConfig;

        public override void Entry(IModHelper helper)
        {
            Harmony harmony = new(Helper.ModRegistry.ModID);
            IMonitor = Monitor;
            IConfig = Helper.ReadConfig<Config>();

            harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.blacksmith)),
                prefix: new HarmonyMethod(typeof(ModEntry), nameof(blacksmithPrefix))
            );
        }

        private static bool blacksmithPrefix(Location tileLocation, GameLocation __instance)
        {
            try
            {
                foreach (var character in __instance.characters)
                {
                    if (character.Name != "Clint")
                        continue;
                    if ((character.Position / 64) != new Vector2(tileLocation.X, tileLocation.Y - 1) && (character.Position / 64) != new Vector2(tileLocation.X - 1, tileLocation.Y - 1))
                        return true;
                    character.faceDirection(2);
                    if (Game1.player.toolBeingUpgraded.Value is null && Game1.player.daysLeftForToolUpgrade.Value <= 0)
                        return true;
                    Response[] answerChoices;
                    if (canProcessGeodes() && IConfig.Geodes)
                    {
                        answerChoices =
                        [
                            new("Shop", Game1.content.LoadString("Strings\\Locations:Blacksmith_Clint_Shop")),
                            new("Process", Game1.content.LoadString("Strings\\Locations:Blacksmith_Clint_Geodes")),
                            new("Leave", Game1.content.LoadString("Strings\\Locations:Blacksmith_Clint_Leave"))
                        ];
                    }
                    else
                    {
                        answerChoices =
                        [
                            new("Shop", Game1.content.LoadString("Strings\\Locations:Blacksmith_Clint_Shop")),
                            new("Leave", Game1.content.LoadString("Strings\\Locations:Blacksmith_Clint_Leave"))
                        ];
                    }
                    __instance.createQuestionDialogue("", answerChoices, "Blacksmith");
                    return false;
                }
                return true;
            }
            catch (Exception ex) 
            { 
                IMonitor.Log($"Failed patching {nameof(GameLocation.blacksmith)}", LogLevel.Error);
                IMonitor.Log($"{ex.GetType().Name} - {ex.Message}\n{ex.StackTrace}"); return true; 
            }
        }

        private static bool canProcessGeodes()
        {
            foreach (var item in Game1.player.Items)
                if (Utility.IsGeode(item))
                    return true;
            return false;
        }
    }
}
