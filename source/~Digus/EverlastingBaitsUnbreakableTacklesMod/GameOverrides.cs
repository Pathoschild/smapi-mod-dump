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
                if (!DataLoader.ModConfig.DisableBaits && __instance.attachments.Count >= 1 && __instance.attachments[0] != null && __instance.attachments[0].Quality == 4)
                {
                    __instance.attachments[0].Stack++;
                }

                if (!DataLoader.ModConfig.DisableTackles && __instance.attachments.Count >= 2 && __instance.attachments[1] != null && __instance.attachments[1].Quality == 4)
                {
                    __instance.attachments[1].uses.Value--;
                }

                if (!DataLoader.ModConfig.DisableTackles && __instance.attachments.Count >= 3 && __instance.attachments[2] != null && __instance.attachments[2].Quality == 4)
                {
                    __instance.attachments[2].uses.Value--;
                }
            }

            return true;
        }

        public static bool CreateItem(CraftingRecipe __instance, ref Item __result)
        {
            BaitTackle baitTackle = BaitTackle.GetFromDescription(__instance.name);
            if (baitTackle != null)
            {
                Object _Object = ItemRegistry.Create<Object>(baitTackle.Id, 1, 4);
                __result = _Object;
                return false;
            }
            return true;
        }

        public static void ClickCraftingRecipe(CraftingPage __instance, ref ClickableTextureComponent c)
        {
            CraftingRecipe craftingRecipe = DataLoader.Helper.Reflection.GetField<List<Dictionary<ClickableTextureComponent, CraftingRecipe>>>(__instance, "pagesOfCraftingRecipes").GetValue()[DataLoader.Helper.Reflection.GetField<int>(__instance, "currentCraftingPage").GetValue()][c];

            BaitTackle baitTackle = BaitTackle.GetFromDescription(craftingRecipe.name);
            if (baitTackle != null)
            {
                foreach (Quest quest in Game1.player.questLog.Where(q => q.questType.Value == 1 && q.questTitle == craftingRecipe.name))
                {
                    quest.currentObjective = DataLoader.I18N.Get("Quest.LastObjective", new { Item = craftingRecipe.DisplayName });
                    Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Quest.cs.13636"), 2));
                }
            }
        }

        public static bool TryToReceiveActiveObject(NPC __instance, ref Farmer who, bool probe, ref bool __result)
        {
            if (__instance.Name.Equals((object) "Willy"))
            {
                if (who.ActiveObject is { } o)
                {
                    if (BaitTackle.GetFromId(o.ItemId) is {} baitTackle && o.Quality == 4)
                    {
                        foreach (Quest quest in Game1.player.questLog.Where(q => q.questType.Value == 1 && q.questTitle == DataLoader.I18N.Get($"{baitTackle}.Name")))
                        {
                            __result = true;
                            if (!probe)
                            {
                                who.Halt();
                                who.faceGeneralDirection(__instance.getStandingPosition(), 0, opposite: false, useTileCalculations: false);
                                if (BaitTackle.UnbreakableDressedSpinner.Equals(baitTackle))
                                {
                                    __instance.CurrentDialogue.Push(new Dialogue(__instance, "Quest.LastCompleteDialog",
                                        DataLoader.I18N.Get("Quest.LastCompleteDialog")));
                                }
                                else
                                {
                                    __instance.CurrentDialogue.Push(new Dialogue(__instance, "Quest.CompleteDialog",
                                        DataLoader.I18N.Get("Quest.CompleteDialog")));
                                }

                                Game1.drawDialogue(__instance);
                                Game1.player.reduceActiveItemByOne();
                                quest.questComplete();
                                who.mailReceived.Add(baitTackle.GetQuestName());
                            }
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
                            if (DataLoader.ModConfig.IridiumQualityFishOnlyWithWildBait && fishingRod.attachments[0].ItemId != "774")
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
