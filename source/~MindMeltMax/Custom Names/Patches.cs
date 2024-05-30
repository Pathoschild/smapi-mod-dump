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
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Companions;
using StardewValley.Menus;
using StardewValley.Objects;
using System.Reflection;
using System.Reflection.Emit;

namespace CustomNames
{
    internal static class Patches
    {
        private static FieldInfo? extraPositionField;
        private static readonly PerScreen<int> curIndex = new(() => 0);
        private static readonly PerScreen<string> name = new(() => "");
        private static readonly PerScreen<int> nameFadeTimer = new(() => 0);

        internal static void Patch(string id)
        {
            Harmony harmony = new(id);

            extraPositionField = typeof(FlyingCompanion).GetField("extraPosition", BindingFlags.Instance | BindingFlags.NonPublic);

            harmony.Patch(
                original: AccessTools.PropertyGetter(typeof(Tool), nameof(Tool.DisplayName)),
                postfix: new(typeof(Patches), nameof(Item_getDisplayName_Postfix))
            );

            harmony.Patch(
                original: AccessTools.PropertyGetter(typeof(Ring), nameof(Ring.DisplayName)),
                postfix: new(typeof(Patches), nameof(Item_getDisplayName_Postfix))
            );

            harmony.Patch(
                original: AccessTools.PropertyGetter(typeof(Object), nameof(Object.DisplayName)),
                postfix: new(typeof(Patches), nameof(Item_getDisplayName_Postfix))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(FlyingCompanion), nameof(FlyingCompanion.Draw)),
                postfix: new(typeof(Patches), nameof(Companion_Draw_Postfix))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(HungryFrogCompanion), nameof(HungryFrogCompanion.Draw)),
                postfix: new(typeof(Patches), nameof(Companion_Draw_Postfix))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Toolbar), nameof(Toolbar.draw), [typeof(SpriteBatch)]),
                transpiler: new(typeof(Patches), nameof(Toolbar_Draw_Transpiler))
            );

            ForgeMenuExtensions.Patch(harmony);
        }

        private static void drawCurrentItemName(int index, Toolbar toolbar, SpriteBatch b)
        {
            if (nameFadeTimer.Value > 0)
                --nameFadeTimer.Value;
            if (!ModEntry.IConfig.ShowToolbarName)
                return;
            if (index != Game1.player.CurrentToolIndex) //Inserted into loop, so check for indeces which are no the current tool index
                return;
            if (index != curIndex.Value) //Update current tool index and check if it has a custom name property
            {
                curIndex.Value = index;
                if (!(Game1.player.Items[index]?.modData.TryGetValue(ModEntry.ModDataKey, out string _name) ?? false))
                {
                    name.Value = "";
                    return;
                }
                name.Value = _name;
                nameFadeTimer.Value = 500;
            }
            if (string.IsNullOrWhiteSpace(name.Value)) //No name, no draw
                return;
            int y = toolbar.yPositionOnScreen - 8; //Toolbar is on the top
            if (toolbar.yPositionOnScreen > Game1.uiViewport.Height / 2)
                y = toolbar.yPositionOnScreen - toolbar.height + 56; //Toolbar is on the bottom
            float opacity = nameFadeTimer.Value switch //This makes it fade out... I could make it a bit better... maybe later
            {
                0 => 0f,
                <= 100 => 0.2f,
                <= 200 => 0.4f,
                <= 300 => 0.6f,
                <= 400 => 0.8f,
                _ => 1
            };
            if (opacity > 0)
                b.DrawString(Game1.dialogueFont, name.Value, new(Game1.uiViewport.Width / 2 - 444 + toolbar.width / 2 - Game1.dialogueFont.MeasureString(name.Value).X / 2, y), Color.Aquamarine * opacity);
        }

        internal static void Item_getDisplayName_Postfix(Item __instance, ref string __result)
        {
            if ((__instance is not Tool && __instance is not Ring && __instance is not Trinket) || !__instance.modData.TryGetValue(ModEntry.ModDataKey, out string value))
                return;
            __result = value; //Use the custom name property instead of the default display name
        }

        internal static void Companion_Draw_Postfix(Companion __instance, SpriteBatch b) //Draw the name of the companion when you hover over them
        {
            if (!ModEntry.IConfig.ShowCompanionName)
                return;
            int index = Game1.player.companions.IndexOf(__instance);
            if (index == -1) 
                return;
            Vector2 extraPosition = Vector2.Zero;
            if (__instance is FlyingCompanion)
                extraPosition = (Vector2)extraPositionField!.GetValue(__instance)!;
            Rectangle rect = Game1.GlobalToLocal(Game1.uiViewport, new Rectangle((int)(__instance.Position.X + __instance.Owner.drawOffset.X + extraPosition.X - 32), (int)(__instance.Position.Y + __instance.Owner.drawOffset.Y - (__instance.height * 4) + extraPosition.Y - 32), 64, __instance is HungryFrogCompanion ? 32 : 64));
            if (rect.Contains(Game1.getMouseX(), Game1.getMouseY()))
                if (Game1.player.trinketItems[index].modData.TryGetValue(ModEntry.ModDataKey, out string value))
                    SpriteText.drawSmallTextBubble(b, value, new(rect.X + 32, rect.Y + 32 - 64), 256, (float)(0.98000001907348633 + __instance.Position.X / 64 * 9.9999997473787516E-05 + __instance.Position.X / 64 * 9.9999999747524271E-07));
        }

        internal static IEnumerable<CodeInstruction> Toolbar_Draw_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var mi_dcin = AccessTools.Method(typeof(Patches), nameof(drawCurrentItemName));
            CodeMatcher matcher = new(instructions);

            matcher.Start()
                   .MatchStartForward([new(OpCodes.Ldloc_S), new(OpCodes.Ldc_I4_1), new(OpCodes.Add), new(OpCodes.Stloc_S)]) //Skip first for loop
                   .Advance(1)
                   .MatchStartForward([new(OpCodes.Ldloc_S), new(OpCodes.Ldc_I4_1), new(OpCodes.Add), new(OpCodes.Stloc_S)]) //Inject into second for loop
                   .Advance(1)
                   .Insert([new(OpCodes.Ldarg_0), new(OpCodes.Ldarg_1), new(OpCodes.Call, mi_dcin), new(OpCodes.Ldloc_S, 6)]);

            return matcher.Instructions();
        }
    }
}
