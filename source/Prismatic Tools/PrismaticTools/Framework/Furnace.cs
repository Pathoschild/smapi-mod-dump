using Harmony;
using Microsoft.Xna.Framework;
using StardewValley;

namespace PrismaticTools.Framework {
    [HarmonyPatch(typeof(Farmer), "getTallyOfObject")]
    internal class PrismaticGetTallyOfObject {
        static public void Postfix(ref int __result, int index, bool bigCraftable) {
            if (index == 382 && !bigCraftable) {
                if (__result <= 0) {
                    __result = 666666;
                    return;
                }
            }
        }
    }

    [HarmonyPatch(typeof(Object), "performObjectDropInAction")]
    internal class PrismaticPerformObjectDropInAction {

        static public bool Prefix(ref Object __instance, ref bool __result, ref Item dropInItem, bool probe, Farmer who) {
            if (!(dropInItem is Object))
                return false;
            Object object1 = dropInItem as Object;

            if (object1.ParentSheetIndex != 74) {
                return true;
            }

            if (__instance.name.Equals("Furnace")) {
                if (who.IsLocalPlayer && who.getTallyOfObject(382, false) == 666666) {
                    if (!probe && who.IsLocalPlayer)
                        Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12772"));
                    return false;
                }
                if (__instance.heldObject.Value == null && !probe) {
                    __instance.heldObject.Value = new Object(PrismaticBarItem.INDEX, 5, false, -1, 0);
                    __instance.MinutesUntilReady = 2400;
                    who.currentLocation.playSound("furnace");
                    __instance.initializeLightSource(__instance.TileLocation, false);
                    __instance.showNextIndex.Value = true;

                    Multiplayer multiplayer = ModEntry.ModHelper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();
                    multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite[1] {
                            new TemporaryAnimatedSprite(30, __instance.TileLocation * 64f + new Vector2(0.0f, -16f), Color.White, 4, false, 50f, 10, 64, (float) (((double) __instance.TileLocation.Y + 1.0) * 64.0 / 10000.0 + 9.99999974737875E-05), -1, 0) {
                              alphaFade = 0.005f
                            }
                        });
                    for (int index = who.Items.Count - 1; index >= 0; --index) {
                        if (who.Items[index] is Object && (who.Items[index] as Object).ParentSheetIndex == 382) {
                            --who.Items[index].Stack;
                            if (who.Items[index].Stack <= 0) {
                                who.Items[index] = (Item)null;
                                break;
                            }
                            break;
                        }
                    }
                    object1.Stack -= 1;
                    __result = object1.Stack <= 0;
                    return false;
                }
                if (__instance.heldObject.Value == null & probe) {
                    if (object1.ParentSheetIndex == 74) {
                            __instance.heldObject.Value = new Object();
                            __result = true;
                            return false;
                    }
                }
            }
            __result = false;
            return false;
        }
    }
}