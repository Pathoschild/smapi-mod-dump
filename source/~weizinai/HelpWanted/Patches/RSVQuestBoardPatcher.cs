/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using Common.Patch;
using HarmonyLib;
using HelpWanted.Framework;
using HelpWanted.Framework.Menu;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace HelpWanted.Patches;

internal class RSVQuestBoardPatcher : BasePatcher
{
    private static ModConfig config = null!;

    public RSVQuestBoardPatcher(ModConfig config)
    {
        RSVQuestBoardPatcher.config = config;
    }

    public override void Apply(Harmony harmony)
    {
        harmony.Patch(
            AccessTools.Method(Type.GetType("RidgesideVillage.Questing.RSVQuestBoard,RidgesideVillage"), "draw", new[] { typeof(SpriteBatch) }),
            GetHarmonyMethod(nameof(DrawPrefix))
        );
    }

    private static bool DrawPrefix(string ___boardType)
    {
        if (___boardType != "VillageQuestBoard" || !config.EnableRSVQuestBoard) return true;
        Game1.activeClickableMenu = new RSVQuestBoard(config);
        return false;
    }
}