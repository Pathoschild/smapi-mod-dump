/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DiogoAlbano/StardewValleyMods
**
*************************************************/

using StardewValley;
using System.Collections.Generic;
using HarmonyLib;
using StardewValley.Buildings;
using Microsoft.Xna.Framework;

namespace NovoMundo.Patcher
{
    public class Patching
    {
        public void Apply_Harmony(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.carpenters)),
                postfix: new HarmonyMethod(typeof(Patching), nameof(Patching.Carpenters_ChangeQuestionMenu))
            );
            harmony.Patch(
                AccessTools.Method(typeof(GameLocation), nameof(GameLocation.answerDialogueAction), new[] { typeof(string), typeof(string[]) }),
                postfix: new HarmonyMethod(GetType(), nameof(AnswerDialogueActionPostfix))
            );
            harmony.Patch(
              original: AccessTools.Method(typeof(Farm), "resetLocalState"),
              postfix: new HarmonyMethod(typeof(Patching), nameof(Patching.Farm_resetLocalState_Postfix))
           );
        }
        public static void Farm_resetLocalState_Postfix()
        {
            if (!Game1.getFarm().isThereABuildingUnderConstruction() || Game1.getFarm().getBuildingUnderConstruction().daysOfConstructionLeft.Value <= 0)
                return;
            Game1.getFarm().removeTemporarySpritesWithIDLocal(16846f);
            Building building = Game1.getFarm().getBuildingUnderConstruction();
            Game1.getFarm().temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(399, 262, (building.daysOfConstructionLeft.Value == 1) ? 29 : 9, 43), new Vector2(building.tileX.Value + building.tilesWide.Value / 2, building.tileY.Value + building.tilesHigh.Value / 2) * 64f + new Vector2(-16f, -144f), false, 0f, Color.White)
            {
                id = 16846f,
                scale = 4f,
                interval = 999999f,
                animationLength = 1,
                totalNumberOfLoops = 99999,
                layerDepth = ((building.tileY.Value + building.tilesHigh.Value / 2) * 64 + 32) / 10000f
            });

        }
        public static void Carpenters_ChangeQuestionMenu()
        {

            List<Response> list = new List<Response>
            {
                new Response("RepairHatShop", ModEntry.ModHelper.Translation.Get("QuestionDialoguesChoicesRepairHatShop")),
                new Response("Leave", ModEntry.ModHelper.Translation.Get("QuestionDialoguesNoChoice"))
            };
            Game1.currentLocation.createQuestionDialogue(ModEntry.ModHelper.Translation.Get("QuestionDialoguesChoicesSpecialJobsTitle"), list.ToArray(), "carpenter");
        }
        public static void AnswerDialogueActionPostfix(string questionAndAnswer, string[] questionParams)
        {
            if (questionAndAnswer == "uphouse_Yes")
            {
                houseUpgradeAccept();
            }
        }
        public static void houseUpgradeAccept()
        {
            switch (Game1.player.HouseUpgradeLevel)
            {
                case 0:
                    if (Game1.player.Money >= 10000 && Game1.player.hasItemInInventory(388, 450))
                    {
                        Game1.player.daysUntilHouseUpgrade.Value = 3;
                        Game1.player.Money -= 10000;
                        Game1.player.removeItemsFromInventory(388, 450);
                        if (Game1.currentLocation.getCharacterFromName("Robin") != null)
                        {
                            Game1.drawDialogue(Game1.getCharacterFromName("Robin"), "Aceitei.");
                        }
                        else
                        {
                            Game1.drawObjectDialogue("Robin aceitou.");
                        }
                    }
                    else if (Game1.player.Money < 10000)
                    {
                        if (Game1.currentLocation.getCharacterFromName("Robin") != null)
                        {
                            Game1.drawDialogue(Game1.getCharacterFromName("Robin"), "Falta dinheiro.");
                        }
                        else
                        {
                            Game1.drawObjectDialogue("Falta dinheiro.");
                        }
                    }
                    else
                    {
                        if (Game1.currentLocation.getCharacterFromName("Robin") != null)
                        {
                            Game1.drawDialogue(Game1.getCharacterFromName("Robin"), "Falta madeira.");
                        }
                        else
                        {
                            Game1.drawObjectDialogue("Falta madeira.");
                        }
                    }

                    break;
                case 1:
                    if (Game1.player.Money >= 50000 && Game1.player.hasItemInInventory(709, 150))
                    {
                        Game1.player.daysUntilHouseUpgrade.Value = 3;
                        Game1.player.Money -= 50000;
                        Game1.player.removeItemsFromInventory(709, 150);
                        if (Game1.currentLocation.getCharacterFromName("Robin") != null)
                        {
                            Game1.drawDialogue(Game1.getCharacterFromName("Robin"), "Aceitei.");
                        }
                        else
                        {
                            Game1.drawObjectDialogue("Robin aceitou.");
                        }
                    }
                    else if (Game1.player.Money < 50000)
                    {
                        if (Game1.currentLocation.getCharacterFromName("Robin") != null)
                        {
                            Game1.drawDialogue(Game1.getCharacterFromName("Robin"), "Falta dinheiro.");
                        }
                        else
                        {
                            Game1.drawObjectDialogue("Falta dinheiro.");
                        }
                    }
                    else
                    {
                        if (Game1.currentLocation.getCharacterFromName("Robin") != null)
                        {
                            Game1.drawDialogue(Game1.getCharacterFromName("Robin"), "Falta madeira de lei.");
                        }
                        else
                        {
                            Game1.drawObjectDialogue("Falta madeira de lei.");
                        }
                    }
                    break;
                case 2:
                    if (Game1.player.Money >= 100000)
                    {
                        Game1.player.daysUntilHouseUpgrade.Value = 3;
                        Game1.player.Money -= 100000;
                        if (Game1.currentLocation.getCharacterFromName("Robin") != null)
                        {
                            Game1.drawDialogue(Game1.getCharacterFromName("Robin"), "Aceitei.");
                        }
                        else
                        {
                            Game1.drawObjectDialogue("Robin aceitou.");
                        }
                    }
                    else if (Game1.player.Money < 100000)
                    {
                        if (Game1.currentLocation.getCharacterFromName("Robin") != null)
                        {
                            Game1.drawDialogue(Game1.getCharacterFromName("Robin"), "Falta dinheiro.");
                        }
                        else
                        {
                            Game1.drawObjectDialogue("Falta dinheiro.");
                        }
                    }

                    break;
            }

        }
    }

}


