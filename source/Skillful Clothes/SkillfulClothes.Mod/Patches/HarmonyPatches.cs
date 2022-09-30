/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LunaticShade/StardewValley.SkillfulClothes
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using SkillfulClothes.Effects;
using SkillfulClothes.Effects.Buffs;
using SkillfulClothes.Types;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Monsters;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static StardewValley.Menus.ForgeMenu;
using HarmonyLib;

namespace SkillfulClothes.Patches
{
    class HarmonyPatches
    {
        static HatDrawWrapper hatDrawWrapper = new HatDrawWrapper();
        static ClothingDrawWrapper clothingDrawWrapper = new ClothingDrawWrapper();

        static FieldInfo craftStateFieldInfo;
        static FieldInfo craftDisplayedDescriptionFieldInfo;

        static Clothing shimTrimmedLuckPurpleShorts;
        static Clothing ShimTrimmedLuckPurpleShorts
        {
            get
            {
                if (shimTrimmedLuckPurpleShorts == null)
                {
                    shimTrimmedLuckPurpleShorts = new Clothing((int)Pants.TrimmedLuckyPurpleShorts);
                }
                return shimTrimmedLuckPurpleShorts;
            }
        }

        public static void Apply(String modId)
        {
            Harmony harmony = new Harmony(modId);            

            harmony.Patch(
                  original: AccessTools.Method(typeof(Item), "getDescriptionWidth"),
                  postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(HarmonyPatches.getDescriptionWidth))
               );

            // patch IClickableMenu.drawToolTip, since Harmony (1.2.0.1) is unable to patch Item.getExtraSpaceNeededForTooltipSpecialIcons() correctly (as it returns a struct)
            // see https://github.com/pardeike/Harmony/issues/159 and https://github.com/pardeike/Harmony/issues/77
            // seems to be fixed in Harmony 2.0.4.0
            harmony.Patch(
                  original: typeof(IClickableMenu).GetMethod(nameof(IClickableMenu.drawToolTip), System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public),
                  prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(HarmonyPatches.drawToolTip))
                );

            // Patches for RingEffect
            harmony.Patch(
                  original: AccessTools.Method(typeof(Farmer), nameof(Farmer.isWearingRing)),
                  postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(HarmonyPatches.isWearingRing))
               );

            // Patches for ICustomBuff
            harmony.Patch(
                  original: AccessTools.Method(typeof(Buff), nameof(Buff.addBuff)),
                  postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(HarmonyPatches.addBuff))
               );

            harmony.Patch(
                  original: AccessTools.Method(typeof(Buff), nameof(Buff.removeBuff)),
                  postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(HarmonyPatches.removeBuff))
               );

            harmony.Patch(
                  original: AccessTools.Method(typeof(Buff), nameof(Buff.getClickableComponents)),
                  postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(HarmonyPatches.buff_getClickableComponents))
               );

            harmony.Patch(
                  original: AccessTools.Method(typeof(BuffsDisplay), nameof(BuffsDisplay.clearAllBuffs)),
                  prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(HarmonyPatches.clearAllBuffs))
                );

            // Patches for tailoring
            if (!EffectHelper.Config.AllItemsCanBeTailored)
            {
                craftStateFieldInfo = typeof(TailoringMenu).GetField("_craftState", BindingFlags.Instance | BindingFlags.NonPublic);
                craftDisplayedDescriptionFieldInfo = typeof(TailoringMenu).GetField("displayedDescription", BindingFlags.Instance | BindingFlags.NonPublic);

                harmony.Patch(
                      original: AccessTools.Method(typeof(TailoringMenu), "_ValidateCraft"),
                      postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(HarmonyPatches.ValidateCraft))
                    );
            }

            // Patches for GameLocation events

            // patch GameLocation.damageMonster as pre- and postfix which monsters are gone and thus must have been slain
            // not needed atm
          /*  harmony.Patch(
                  original: typeof(GameLocation).GetMethod(nameof(GameLocation.damageMonster), new Type[] { typeof(Microsoft.Xna.Framework.Rectangle), typeof(int), typeof(int), typeof(bool), typeof(float), typeof(int), typeof(float), typeof(float), typeof(bool), typeof(Farmer) }),
                  prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(HarmonyPatches.prefix_damageMonster))
                );

            harmony.Patch(
                  original: typeof(GameLocation).GetMethod(nameof(GameLocation.damageMonster), new Type[] { typeof(Microsoft.Xna.Framework.Rectangle), typeof(int), typeof(int), typeof(bool), typeof(float), typeof(int), typeof(float), typeof(float), typeof(bool), typeof(Farmer) }),
                  postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(HarmonyPatches.postfix_damageMonster))
                );*/

            // Patches for NPC
            harmony.Patch(
                 original: AccessTools.Method(typeof(NPC), nameof(NPC.checkAction)),
                 postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(HarmonyPatches.NPC_checkAction))
                );
        }

        static void drawToolTip(ref Item hoveredItem)
        {
            // replace the hoveredItem with a wrapper class which allows us to
            // control Item.getExtraSpaceNeededForTooltipSpecialIcons

            if (hoveredItem != null)
            {
                // The "Trimmed Lucky Purple Shorts" from Major Lewis's quest are not an actual clothing item
                if (hoveredItem.Category == 0 && hoveredItem.ParentSheetIndex == 71)
                {
                    hoveredItem = ShimTrimmedLuckPurpleShorts;
                }
            }

            if (ItemDefinitions.GetEffect(hoveredItem, out IEffect effect))
            {
                if (hoveredItem is Clothing clothing)
                {
                    clothingDrawWrapper.Assign(clothing, effect);
                    hoveredItem = clothingDrawWrapper;
                } else if (hoveredItem is StardewValley.Objects.Hat hat)
                {
                    hatDrawWrapper.Assign(hat, effect);
                    hoveredItem = hatDrawWrapper;
                }
            }
        }

        static int getDescriptionWidth(int __result, Item __instance)
        {
            // increase the width so that effect descriptions stay on one line and do not break
            if (ItemDefinitions.GetEffect(__instance, out IEffect effect))
            {
                return Math.Max(__result, EffectHelper.getDescriptionWidth(effect));
            }

            return __result;
        }

        static void addBuff(Buff __instance)
        {
            if (__instance is ICustomBuff cb)
            {
                cb.ApplyCustomEffect();
            }
        }

        static void removeBuff(Buff __instance)
        {
            if (__instance is ICustomBuff cb)
            {
                cb.RemoveCustomEffect(false);
            }
        }

        static List<ClickableTextureComponent> buff_getClickableComponents(List<ClickableTextureComponent> __result, Buff __instance)
        {
            if (__instance is ICustomBuff cb)
            {
                if (__result == null)
                {
                    __result = new List<ClickableTextureComponent>();
                }

                var addittonalIcons = cb.GetCustomBuffIcons();
                if (addittonalIcons != null)
                {
                    __result.AddRange(addittonalIcons);
                }                
            }

            return __result;
        }

        static void clearAllBuffs(BuffsDisplay __instance)
        {
            // call the removeEffect method of custom buffs, because the
            // base game doe snot call removeBuff of applied 'other buffs'
            foreach(var buff in __instance.otherBuffs.OfType<ICustomBuff>())
            {
                buff.RemoveCustomEffect(true);
            }
        }

        static bool isWearingRing(bool __result, int ringIndex)
        {
            return __result || EffectHelper.ClothingObserver.HasRingEffect(ringIndex);
        }

        static void prefix_damageMonster(GameLocation __instance, Farmer who, out List<Monster> __state)
        {
            if (who == null)
            {
                __state = null;
            } else
            {
                __state = __instance.characters.OfType<Monster>().ToList();
            }
        }

        static void postfix_damageMonster(GameLocation __instance, List<Monster> __state)
        {
            if (__state != null)
            {
                List<Monster> slainMonsters = __state.Except(__instance.characters.OfType<Monster>().Where(x => x.Health <= 0)).ToList();
                foreach (var m in slainMonsters)
                {
                    EffectHelper.Events.RaiseMonsterSlain(Game1.player, m);
                }
            }
        }

        static void ValidateCraft(TailoringMenu __instance)
        {                        
            // TODO: HUD message is displayed behind tailoring menu -> store once and show when tailoring menu gets closed
            if (__instance.craftResultDisplay.item != null && ItemDefinitions.GetExtInfo(__instance.craftResultDisplay.item, out ExtItemInfo extInfo))
            {
                if (extInfo.IsCraftingDisabled)
                {
                    if (extInfo.SoldBy != Shop.None)
                    {
                        // display a hint after closing the menu where the player might buy the item
                        TailoringPatches.AddTailoringMessage($"Hm, that did not quite work out. Maybe {extInfo.SoldBy.GetShopReferral()} sells something similar.", __instance.craftResultDisplay.item);
                    }

                    // indicate to the player that this is an invalid recipe now
                    int itemId = __instance.craftResultDisplay.item.ParentSheetIndex;
                    __instance.craftResultDisplay.item = null;
                    craftStateFieldInfo.SetValue(__instance, CraftState.InvalidRecipe);
                    craftDisplayedDescriptionFieldInfo.SetValue(__instance, Game1.content.LoadString("Strings\\UI:Tailor_InvalidRecipe"));                    

                    // delete the recipe, so that the player cannot craft the item
                    __instance._tailoringRecipes.RemoveAll(x => x.CraftedItemID == itemId);
                }
            }
        }

        static void NPC_checkAction(bool __result, NPC __instance)
        {            
            if (__result)
            {
                EffectHelper.Events.RaiseInteractedWithNPC(__instance);
            }
        }
    }
}
