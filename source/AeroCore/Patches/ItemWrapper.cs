/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Tlitookilakin/AeroCore
**
*************************************************/

using AeroCore.Utils;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Network;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using SObject = StardewValley.Object;

namespace AeroCore.Patches
{
    [HarmonyPatch]
    [ModInit]
    public class ItemWrapper
    {
        internal const string WrapFlag = "tlitoo.aero.itemWrapper";
        private static readonly MethodInfo ActionBase = typeof(SObject).MethodNamed(nameof(SObject.checkForAction));

        internal static void Init()
        {
            HarmonyMethod itemSwap = new(typeof(ItemWrapper), nameof(ReplaceItem));
            HarmonyMethod itemSwap2 = new(typeof(ItemWrapper), nameof(ReplaceItemAlt));
            ModEntry.harmony.Patch(typeof(Farmer).MethodNamed(nameof(Farmer.addItemToInventoryBool)), prefix: itemSwap);
            ModEntry.harmony.Patch(typeof(Farmer).MethodNamed(nameof(Farmer.addItemToInventory), new[] {
                typeof(Item), typeof(List<Item>)
            }), prefix: itemSwap);
            ModEntry.harmony.Patch(typeof(Farmer).MethodNamed(nameof(Farmer.addItemToInventory), new[] {
                typeof(Item), typeof(int)
            }), prefix: itemSwap);
            ModEntry.harmony.Patch(typeof(Utility).MethodNamed(nameof(Utility.addItemToInventory)), prefix: itemSwap);
            ModEntry.harmony.Patch(typeof(Utility).MethodNamed(nameof(Utility.checkItemFirstInventoryAdd)), prefix: itemSwap);
            ModEntry.harmony.Patch(typeof(Utility).MethodNamed(nameof(Utility.addItemToThisInventoryList)), prefix: itemSwap2);
            ModEntry.harmony.Patch(typeof(Utility).MethodNamed(nameof(Utility.canItemBeAddedToThisInventoryList)), prefix: itemSwap2);
        }

        public static SObject WrapItem(Item what, bool forceSubtype = false)
        {
            if (what is null || what.modData.ContainsKey(WrapFlag))
                return null;
            if (what is SObject obj && !forceSubtype)
                return obj;
            if (what.GetType() == typeof(SObject) && !(what as SObject).bigCraftable.Value)
                return what as SObject;
            Sign ret = new(Vector2.Zero, 37); // wood sign
            ret.displayItem.Value = what;
            ret.modData[WrapFlag] = "T";
            ret.bigCraftable.Value = false;
            return ret;
        }
        public static Item UnwrapItem(Item what)
            => what is Sign s and not null && what.modData.ContainsKey(WrapFlag) ? s.displayItem.Value : what;
        public static int UnwrapItemsInList(List<Item> items)
        {
            int count = 0;
            for(int i = 0; i < items.Count; i++)
            {
                if (items[i] is Sign s && s.modData.ContainsKey(WrapFlag))
                {
                    count++;
                    items[i] = UnwrapItem(s);
                }
            }
            return count;
        }

        [HarmonyPatch(typeof(Sign), nameof(Sign.draw))]
        [HarmonyPrefix]
        internal static bool DrawPatch(Sign __instance, SpriteBatch spriteBatch, int x, int y, float alpha)
        {
            if (!__instance.modData.ContainsKey(WrapFlag))
                return true;
            Vector2 pos = Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64, y * 64));
            float depth = __instance.getBoundingBox(new Vector2(x, y)).Bottom;
            spriteBatch.Draw(Game1.shadowTexture, new(pos.X + 32, pos.Y + 51 + 4),
                Game1.shadowTexture.Bounds, Color.White * alpha, 0f,
                new Vector2(Game1.shadowTexture.Bounds.Center.X, Game1.shadowTexture.Bounds.Center.Y),
                4f, SpriteEffects.None, depth / 15000f);
            __instance.displayItem.Value?.drawInMenu(spriteBatch, pos, 1f, alpha, depth / 10000f);
            return false;
        }

        [HarmonyPatch(typeof(SObject), nameof(SObject.drawInMenu), new[] {
            typeof(SpriteBatch), typeof(Vector2), typeof(float), typeof(float), 
            typeof(float), typeof(StackDrawType), typeof(Color), typeof(bool)
        })]
        [HarmonyPrefix]
        internal static bool DrawInvPatch(SObject __instance, SpriteBatch spriteBatch, Vector2 location, 
            float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
        {
            if (__instance is Sign s && s.modData.ContainsKey(WrapFlag))
                s.displayItem.Value?.drawInMenu(spriteBatch, location, scaleSize, transparency, layerDepth, drawStackNumber, color, drawShadow);
            else
                return true;
            return false;
        }

        [HarmonyPatch(typeof(Sign), nameof(Sign.checkForAction))]
        [HarmonyPrefix]
        internal static bool ActionPatch(Sign __instance, Farmer who, bool justCheckingForActivity, ref bool __result)
        {
            if (!__instance.modData.ContainsKey(WrapFlag))
                return true;
            __result = false;
            if (!justCheckingForActivity && (!__instance.modData.ContainsKey("tlitoo.mumps.noPickup") || __instance.IsSpawnedObject) &&
                who.couldInventoryAcceptThisItem(__instance.displayItem.Value))
            {
                if (who.IsLocalPlayer)
                {
                    who.currentLocation.localSound("pickUpItem");
                    DelayedAction.playSoundAfterDelay("coin", 300);
                }
                who.animateOnce(279 + who.FacingDirection);
                who.addItemToInventoryBool(__instance.displayItem.Value.getOne());
                who.currentLocation.Objects.Remove(__instance.TileLocation);
            }
            return false;
        }

        [HarmonyPatch(typeof(SObject), nameof(SObject.placementAction))]
        [HarmonyPrefix]
        internal static bool PlacePatch(SObject __instance, ref bool __result, GameLocation location, int x, int y)
        {
            if (!__instance.modData.ContainsKey(WrapFlag) || __instance is not Sign os)
                return true;
            Vector2 pos = new(x / 64, y / 64);
            __result = !location.Objects.TryGetValue(pos, out _);
            if (__result)
            {
                Sign s = new(pos, 37); // wood sign
                s.displayItem.Value = os.displayItem.Value;
                s.modData[WrapFlag] = "T";
                //s.bigCraftable.Value = false;
                location.Objects.Add(pos, s);
            }
            return false;
        }

        [HarmonyPatch(typeof(Utility), nameof(Utility.canGrabSomethingFromHere))]
        [HarmonyTranspiler]
        internal static IEnumerable<CodeInstruction> CanGrab(IEnumerable<CodeInstruction> codes, ILGenerator gen)
            => grabPatch.Run(codes, gen);

        private static ILHelper grabPatch = new ILHelper(ModEntry.monitor, "grabbable wrapper")
            .SkipTo(new CodeInstruction[]
            {
                new (OpCodes.Ldfld, typeof(SObject).FieldNamed(nameof(SObject.readyForHarvest)))
            })
            .Skip(3)
            .Add(new CodeInstruction[]
            {
                new(OpCodes.Call, typeof(Game1).PropertyGetter(nameof(Game1.currentLocation))),
                new(OpCodes.Callvirt, typeof(GameLocation).PropertyGetter(nameof(GameLocation.Objects))),
                new(OpCodes.Ldloc_0),
                new(OpCodes.Callvirt, typeof(OverlaidDictionary).MethodNamed("get_Item")),
                new(OpCodes.Ldfld, typeof(Item).FieldNamed(nameof(Item.modData))),
                new(OpCodes.Ldstr, WrapFlag),
                new(OpCodes.Callvirt, typeof(ModDataDictionary).MethodNamed(nameof(ModDataDictionary.ContainsKey)))
            })
            .AddJump(OpCodes.Brtrue, "found")
            .SkipTo(new CodeInstruction[]
            {
                new(OpCodes.Ldc_I4_6)
            })
            .AddLabel("found")
            .Finish();

        [HarmonyPatch(typeof(SObject), nameof(SObject.getOne))]
        [HarmonyPostfix]
        internal static Item GetOnePatch(Item what, SObject __instance)
        {
            return (__instance is Sign s && __instance.modData.ContainsKey(WrapFlag)) ?
                s.heldObject.Value?.getOne() : what;
        }

        private static void ReplaceItem(ref Item item)
            => item = item is Sign s && s.modData.ContainsKey(WrapFlag) ? s.displayItem.Value : item;
        private static void ReplaceItemAlt(ref Item i)
            => i = i is Sign s && s.modData.ContainsKey(WrapFlag) ? s.displayItem.Value : i;
    }
}
