/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Quests;
using StardewValley.Tools;
using Object = StardewValley.Object;

namespace EverlastingBaitsAndUnbreakableTacklesMod
{
    public class GameOverrides
    {
        public static bool DoDoneFishing(FishingRod __instance, ref bool consumeBaitAndTackle)
        {
            Farmer lastUser = DataLoader.Helper.Reflection.GetField<Farmer>(__instance,"lastUser").GetValue();
            if (consumeBaitAndTackle && lastUser != null && lastUser.IsLocalPlayer)
            {
                if (__instance.attachments[0] != null && __instance.attachments[0].Quality == 4)
                {
                    __instance.attachments[0].Stack++;
                }

                if (__instance.attachments[1] != null && __instance.attachments[1].Quality == 4)
                {
                    __instance.attachments[1].uses.Value--;
                }
            }

            return true;
        }

        public static bool CreateItem(CraftingRecipe __instance, ref Item __result)
        {
            BaitTackle? baitTackle = BaitTackleExtension.GetFromDescription(__instance.name);
            if (baitTackle.HasValue)
            {
                Object _Object = new Object(Vector2.Zero, (int)baitTackle.Value, 1);
                _Object.Quality = 4;
                __result = _Object;
                return false;
            }
            return true;
        }

        public static bool ClickCraftingRecipe(CraftingPage __instance, ref ClickableTextureComponent c)
        {
            CraftingRecipe craftingRecipe = DataLoader.Helper.Reflection.GetField<List<Dictionary<ClickableTextureComponent, CraftingRecipe>>>(__instance, "pagesOfCraftingRecipes").GetValue()[DataLoader.Helper.Reflection.GetField<int>(__instance, "currentCraftingPage").GetValue()][c];

            BaitTackle? baitTackle = BaitTackleExtension.GetFromDescription(craftingRecipe.name);
            if (baitTackle.HasValue)
            {
                foreach (Quest quest in Game1.player.questLog.Where(q => q.questType.Value == 1 && q.questTitle == craftingRecipe.name))
                {
                    quest.currentObjective = DataLoader.I18N.Get("Quest.LastObjective", new { Item = craftingRecipe.DisplayName });
                    Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Quest.cs.13636"), 2));
                }
            }

            return true;
        }

        public static bool TryToReceiveActiveObject(NPC __instance, ref Farmer who)
        {
            if (__instance.Name.Equals((object) "Willy"))
            {
                if (who.ActiveObject is Object o)
                {
                    if (Enum.IsDefined(typeof(BaitTackle), o.ParentSheetIndex) && o.Quality == 4)
                    {
                        BaitTackle baitTackle = (BaitTackle)o.ParentSheetIndex;
                        foreach (Quest quest in Game1.player.questLog.Where(q => q.questType.Value == 1 && q.questTitle == DataLoader.I18N.Get($"{baitTackle.ToString()}.Name")))
                        {
                            if (baitTackle != BaitTackle.UnbreakableDressedSpinner)
                            {
                                __instance.CurrentDialogue.Push(new Dialogue(DataLoader.I18N.Get("Quest.CompleteDialog"), __instance));
                            }
                            else
                            {
                                __instance.CurrentDialogue.Push(new Dialogue(DataLoader.I18N.Get("Quest.LastCompleteDialog"), __instance));
                            }
                            
                            Game1.drawDialogue(__instance);
                            Game1.player.reduceActiveItemByOne();
                            quest.questComplete();
                            who.mailReceived.Add(baitTackle.GetQuestName());
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public static void BobberBar(BobberBar __instance, ref float fishSize)
        {
            if (fishSize >= DataLoader.ModConfig.IridiumQualityFishMinimumSize)
            {
                if (Game1.player.CurrentTool is FishingRod fishingRod)
                {
                    if (DataLoader.ModConfig.IridiumQualityFishOnlyWithIridiumQualityBait || DataLoader.ModConfig.IridiumQualityFishOnlyWithWildBait)
                    {
                        if (fishingRod.attachments[0] != null)
                        {
                            if (DataLoader.ModConfig.IridiumQualityFishOnlyWithIridiumQualityBait && fishingRod.attachments[0].Quality != 4)
                            {
                                return;
                            }
                            if (DataLoader.ModConfig.IridiumQualityFishOnlyWithWildBait && fishingRod.attachments[0].ParentSheetIndex != 774)
                            {
                                return;
                            }
                        }
                        else
                        {
                            return;
                        }
                    }

                    if (Game1.random.NextDouble() < fishSize / 2.0)
                    {
                        DataLoader.Helper.Reflection.GetField<int>(__instance, "fishQuality").SetValue(4);
                    }
                }
            }
        }
    }
}
