/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Locations;
using weizinai.StardewValleyMod.Common.Patcher;
using weizinai.StardewValleyMod.HelpWanted.Framework;
using weizinai.StardewValleyMod.HelpWanted.Framework.Menu;

namespace weizinai.StardewValleyMod.HelpWanted.Patcher;

internal class TownPatcher : BasePatcher
{
    public override void Apply(Harmony harmony)
    {
        harmony.Patch(this.RequireMethod<Town>(nameof(Town.draw), new[] { typeof(SpriteBatch) }),
            postfix: this.GetHarmonyMethod(nameof(DrawPostfix))
        );
    }

    private static void DrawPostfix(SpriteBatch spriteBatch)
    {
        if (!QuestManager.VanillaQuestList.Any() && !VanillaQuestBoard.QuestNotes.Any()) return;

        var yOffset = 4f * (float)Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250.0), 2);
        spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(2692f, 3528f + yOffset)),
            new Rectangle(395, 497, 3, 8), Color.White, 0f, new Vector2(1f, 4f), 4f + Math.Max(0f, 0.25f - yOffset / 16f),
            SpriteEffects.None, 1f);
    }
}