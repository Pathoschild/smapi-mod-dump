/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using System.Reflection.Emit;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using weizinai.StardewValleyMod.AutoBreakGeode.Framework;
using weizinai.StardewValleyMod.Common.Patcher;
using weizinai.StardewValleyMod.Common.UI;

namespace weizinai.StardewValleyMod.AutoBreakGeode.Patcher;

internal class GeodeMenuPatcher : BasePatcher
{
    private static RootElement? ui;
    private static ModConfig config = null!;

    public GeodeMenuPatcher(ModConfig config)
    {
        GeodeMenuPatcher.config = config;
    }

    public override void Apply(Harmony harmony)
    {
        harmony.Patch(this.RequireConstructor<GeodeMenu>(),
            postfix: this.GetHarmonyMethod(nameof(GeodeMenuPostfix))
        );
        harmony.Patch(this.RequireMethod<GeodeMenu>(nameof(GeodeMenu.update)),
            postfix: this.GetHarmonyMethod(nameof(UpdatePostfix))
        );
        harmony.Patch(this.RequireMethod<GeodeMenu>(nameof(GeodeMenu.draw), new[] { typeof(SpriteBatch) }),
            transpiler: this.GetHarmonyMethod(nameof(DrawTranspiler))
        );
    }

    private static void GeodeMenuPostfix(ClickableComponent ___geodeSpot)
    {
        if (!config.DrawBeginButton) return;

        ui = new RootElement();
        var button = new Button(I18n.UI_BeginButton_Name(), Vector2.Zero)
        {
            OnHover = (element, _) => (element as Button)!.TextureColor = Color.White * 0.7f,
            OffHover = element => (element as Button)!.TextureColor = Color.White,
            OnLeftClick = () => ModEntry.AutoBreakGeode = !ModEntry.AutoBreakGeode
        };
        var x = ___geodeSpot.bounds.X;
        var y = ___geodeSpot.bounds.Y;
        button.LocalPosition = new Vector2(x, y);
        ui.AddChild(button);
    }

    private static void UpdatePostfix()
    {
        if (ui is null) return;

        ui.Update();
        ui.ReceiveLeftClick();
    }

    private static IEnumerable<CodeInstruction> DrawTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        var codes = instructions.ToList();
        var parameters = new[]
        {
            typeof(Texture2D), typeof(Vector2), typeof(Rectangle), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float)
        };
        var index = codes.FindIndex(code => 
            code.opcode == OpCodes.Callvirt && code.operand.Equals(AccessTools.Method(typeof(SpriteBatch), nameof(SpriteBatch.Draw), parameters)));
        codes.Insert(index + 1, new CodeInstruction(OpCodes.Ldarg_1));
        codes.Insert(index + 2, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(GeodeMenuPatcher), nameof(DrawButton))));
        return codes.AsEnumerable();
    }

    private static void DrawButton(SpriteBatch spriteBatch)
    {
        if (ui is null) return;

        ui.Draw(spriteBatch);
        ui.PerformHoverAction(spriteBatch);
    }
}