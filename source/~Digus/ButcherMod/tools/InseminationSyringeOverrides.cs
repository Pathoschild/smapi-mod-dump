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
using AnimalHusbandryMod.animals;
using AnimalHusbandryMod.animals.data;
using AnimalHusbandryMod.common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Tools;
using SObject = StardewValley.Object;

namespace AnimalHusbandryMod.tools
{
    public class InseminationSyringeOverrides : ToolOverridesBase
    {
        internal static string InseminationSyringeKey = "DIGUS.ANIMALHUSBANDRYMOD/InseminationSyringe";

        internal static readonly Dictionary<string, FarmAnimal> Animals = new Dictionary<string, FarmAnimal>();

        public static int InitialParentTileIndex = 518;
        public static int IndexOfMenuItemView = 518;
        public static int AttachmentMenuTile = 72;

        public static bool getOne(MilkPail __instance, ref Item __result)
        {
            if (!IsInseminationSyringe(__instance)) return true;

            __result = (Item)ToolsFactory.GetInseminationSyringe();
            return false;
        }

        public static void loadDisplayName(MilkPail __instance, ref string __result)
        {
            if (!IsInseminationSyringe(__instance)) return;

            __result = DataLoader.i18n.Get("Tool.InseminationSyringe.Name");
        }

        public static void loadDescription(MilkPail __instance, ref string __result)
        {
            if (!IsInseminationSyringe(__instance)) return;

            __result = DataLoader.i18n.Get("Tool.InseminationSyringe.Description");
        }

        public static void canBeTrashed(Tool __instance, ref bool __result)
        {
            if (!IsInseminationSyringe(__instance)) return;

            __result = true;
        }

        public static bool beginUsing(MilkPail __instance, GameLocation location, int x, int y, StardewValley.Farmer who, ref bool __result)
        {
            if (!IsInseminationSyringe(__instance)) return true;

            string inseminationSyringeId = __instance.modData[InseminationSyringeKey];

            x = (int)who.GetToolLocation(false).X;
            y = (int)who.GetToolLocation(false).Y;
            Rectangle rectangle = new Rectangle(x - Game1.tileSize / 2, y - Game1.tileSize / 2, Game1.tileSize, Game1.tileSize);

            if (!DataLoader.ModConfig.DisablePregnancy)
            {
                if (location is Farm)
                {
                    foreach (FarmAnimal farmAnimal in (location as Farm).animals.Values)
                    {
                        if (farmAnimal.GetBoundingBox().Intersects(rectangle))
                        {
                            Animals[inseminationSyringeId] = farmAnimal;
                            break;
                        }
                    }
                }
                else if (location is AnimalHouse)
                {
                    foreach (FarmAnimal farmAnimal in (location as AnimalHouse).animals.Values)
                    {
                        if (farmAnimal.GetBoundingBox().Intersects(rectangle))
                        {
                            Animals[inseminationSyringeId] = farmAnimal;
                            break;
                        }
                    }
                }
            }

            Animals.TryGetValue(inseminationSyringeId, out FarmAnimal animal);
            if (animal != null)
            {
                string dialogue = "";
                if (__instance.attachments[0] == null)
                {
                    Game1.showRedMessage(DataLoader.i18n.Get("Tool.InseminationSyringe.Empty"));
                    Animals[inseminationSyringeId] = null;
                }
                else if (AnimalExtension.GetAnimalFromType(animal.type.Value) == null)
                {
                    dialogue = DataLoader.i18n.Get("Tool.InseminationSyringe.CantBeInseminated", new { animalName = animal.displayName });
                }
                else if (IsEggAnimal(animal))
                {
                    dialogue = DataLoader.i18n.Get("Tool.InseminationSyringe.EggAnimal", new { animalName = animal.displayName });
                }
                else if (!((ImpregnatableAnimalItem)DataLoader.AnimalData.GetAnimalItem(animal)).MinimumDaysUtillBirth.HasValue)
                {
                    dialogue = DataLoader.i18n.Get("Tool.InseminationSyringe.CantBeInseminated", new { animalName = animal.displayName });
                }
                else if (animal.isBaby())
                {
                    dialogue = DataLoader.i18n.Get("Tool.InseminationSyringe.TooYoung", new { animalName = animal.displayName });
                }
                else if (PregnancyController.IsAnimalPregnant(animal))
                {
                    int daysUntilBirth = animal.GetDaysUntilBirth().Value;
                    if (daysUntilBirth > 1)
                    {
                        dialogue = DataLoader.i18n.Get("Tool.InseminationSyringe.AlreadyPregnant", new { animalName = animal.displayName, numberOfDays = daysUntilBirth });
                    }
                    else
                    {
                        dialogue = DataLoader.i18n.Get("Tool.InseminationSyringe.ReadyForBirth", new { animalName = animal.displayName });
                    }
                }
                else if (!CheckCorrectProduct(animal, __instance.attachments[0]))
                {
                    var data = DataLoader.Helper.Content.Load<Dictionary<int, string>>(@"Data\ObjectInformation.xnb", ContentSource.GameContent);
                    string produceName = data[animal.defaultProduceIndex.Value].Split('/')[4];
                    dialogue = DataLoader.i18n.Get("Tool.InseminationSyringe.CorrectItem", new { itemName = produceName });
                }
                else if (PregnancyController.CheckBuildingLimit(animal))
                {
                    dialogue = DataLoader.i18n.Get("Tool.InseminationSyringe.BuildingLimit", new { buildingType = animal.displayHouse });
                }
                else
                {
                    animal.doEmote(16, true);
                    if (who != null && Game1.player.Equals(who))
                    {
                        if (animal.sound.Value != null)
                        {
                            ICue animalSound = Game1.soundBank.GetCue(animal.sound.Value);
                            animalSound.Play();
                        }

                        DelayedAction.playSoundAfterDelay("fishingRodBend", 300, location);
                        DelayedAction.playSoundAfterDelay("fishingRodBend", 1200, location);
                    }
                    animal.pauseTimer = 1500;
                }
                if (dialogue.Length > 0)
                {
                    if (who != null && Game1.player.Equals(who))
                    {
                        DelayedAction.showDialogueAfterDelay(dialogue, 150);
                    }
                    Animals[inseminationSyringeId] = null;
                }
            }

            who.Halt();
            int currentFrame = who.FarmerSprite.currentFrame;
            if (animal != null)
            {
                who.FarmerSprite.animateOnce(287 + who.FacingDirection, 50f, 4);
            }
            else
            {
                who.FarmerSprite.animateOnce(new FarmerSprite.AnimationFrame[1] { new FarmerSprite.AnimationFrame(currentFrame, 0, false, who.FacingDirection == 3, new AnimatedSprite.endOfAnimationBehavior(StardewValley.Farmer.useTool), true) });
            }
            who.FarmerSprite.oldFrame = currentFrame;
            who.UsingTool = true;
            who.CanMove = false;

            __result = true;
            return false;
        }

        public static bool DoFunction(MilkPail __instance, GameLocation location, int x, int y, int power, StardewValley.Farmer who)
        {
            if (!IsInseminationSyringe(__instance)) return true;

            string inseminationSyringeId = __instance.modData[InseminationSyringeKey];

            BaseToolDoFunction(__instance ,location, x, y, power, who);
            __instance.CurrentParentTileIndex = InitialParentTileIndex;
            __instance.indexOfMenuItemView.Value = IndexOfMenuItemView;

            Animals.TryGetValue(inseminationSyringeId, out FarmAnimal animal);
            if (animal != null)
            {
                who.Stamina -= ((float)4f - (float)who.FarmingLevel * 0.2f);
                int daysUtillBirth = ((ImpregnatableAnimalItem)DataLoader.AnimalData.GetAnimalItem(animal)).MinimumDaysUtillBirth.Value;
                if (!DataLoader.ModConfig.DisableContestBonus && AnimalContestController.HasFertilityBonus(animal))
                {
                    daysUtillBirth -= (int)Math.Round(daysUtillBirth / 10.0, MidpointRounding.AwayFromZero);
                }
                PregnancyController.AddPregnancy(animal, daysUtillBirth);
                animal.allowReproduction.Value = false;
                --__instance.attachments[0].Stack;
                if (__instance.attachments[0].Stack <= 0)
                {
                    Game1.showGlobalMessage(DataLoader.i18n.Get("Tool.InseminationSyringe.ItemConsumed", new { itemName = __instance.attachments[0].DisplayName }));
                    __instance.attachments[0] = null;
                }
                Animals[inseminationSyringeId] = (FarmAnimal)null;
            }

            if (Game1.activeClickableMenu == null)
            {
                who.CanMove = true;
                who.completelyStopAnimatingOrDoingAction();
            }
            else
            {
                who.Halt();
            }
            who.UsingTool = false;
            who.canReleaseTool = true;

            DataLoader.Helper.Reflection.GetMethod(__instance, "finish").Invoke();
            return false;
        }

        public static bool canThisBeAttached(MilkPail __instance, SObject o, ref bool __result)
        {
            if (!IsInseminationSyringe(__instance)) return true;

            __result = o == null || DataLoader.AnimalData.SyringeItemsIds.Contains(o.ParentSheetIndex);
            return false;
        }

        public static bool attach(MilkPail __instance, SObject o, ref SObject __result)
        {
            if (!IsInseminationSyringe(__instance)) return true;

            if (o != null)
            {
                var tmp = __instance.attachments[0];
                if (tmp != null && tmp.canStackWith(o))
                {
                    tmp.Stack = o.addToStack(tmp);
                    if (tmp.Stack <= 0)
                    {
                        tmp = null;
                    }
                }
                __instance.attachments[0] = o;
                Game1.playSound("button1");
                __result = tmp;
                return false;
            }
            else
            {
                if (__instance.attachments[0] != null)
                {
                    var attachment = __instance.attachments[0];
                    __instance.attachments[0] = null;
                    Game1.playSound("dwop");
                    __result = attachment;
                    return false;
                }
            }
            __result = null;
            return false;
        }

        public static bool drawAttachments(MilkPail __instance, SpriteBatch b, int x, int y)
        {
            if (!IsInseminationSyringe(__instance)) return true;

            if (__instance.attachments[0] != null)
            {
                b.Draw(Game1.menuTexture, new Vector2(x, y), Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 10), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.86f);
                __instance.attachments[0].drawInMenu(b, new Vector2(x, y), 1f);
            }
            else
            {
                b.Draw(Game1.menuTexture, new Vector2(x, y), Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, AttachmentMenuTile), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.86f);
            }
            return false;
        }

        private static bool IsInseminationSyringe(Tool tool)
        {
            return tool.modData.ContainsKey(InseminationSyringeKey);
        }

        private static bool CheckCorrectProduct(FarmAnimal animal, SObject o)
        {
            return animal.defaultProduceIndex.Value == o.ParentSheetIndex
                   || (((ImpregnatableAnimalItem)DataLoader.AnimalData.GetAnimalItem(animal)).CanUseDeluxeItemForPregnancy
                       && animal.deluxeProduceIndex.Value == o.ParentSheetIndex);
        }

        public static bool IsEggAnimal(FarmAnimal animal)
        {
            switch (AnimalExtension.GetAnimalFromType(animal.type.Value))
            {
                case Animal.Duck:
                case Animal.Chicken:
                case Animal.Dinosaur:
                    return true;
                default:
                    return false;
            }
        }
    }
}
