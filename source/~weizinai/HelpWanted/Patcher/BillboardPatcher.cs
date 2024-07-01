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
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using weizinai.StardewValleyMod.Common.Patcher;
using weizinai.StardewValleyMod.HelpWanted.Framework;
using weizinai.StardewValleyMod.HelpWanted.Framework.Menu;

namespace weizinai.StardewValleyMod.HelpWanted.Patcher;

internal class BillboardPatcher : BasePatcher
{
    private static ModConfig config = null!;

    public BillboardPatcher(ModConfig config)
    {
        BillboardPatcher.config = config;
    }

    public override void Apply(Harmony harmony)
    {
        harmony.Patch(this.RequireMethod<Billboard>(nameof(Billboard.draw), new[] { typeof(SpriteBatch) }), this.GetHarmonyMethod(nameof(DrawPrefix))
        );
    }

    private static bool DrawPrefix(bool ___dailyQuestBoard)
    {
        if (!___dailyQuestBoard) return true;
        Game1.activeClickableMenu = new VanillaQuestBoard(config);
        return false;
    }
}