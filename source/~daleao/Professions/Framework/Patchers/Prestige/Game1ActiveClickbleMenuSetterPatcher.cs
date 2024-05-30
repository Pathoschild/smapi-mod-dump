/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Professions.Framework.Patchers.Prestige;

#region using directives

using DaLion.Shared.Extensions.SMAPI;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class Game1ActiveClickbleMenuSetterPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="Game1ActiveClickbleMenuSetterPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal Game1ActiveClickbleMenuSetterPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequirePropertySetter<Game1>(nameof(Game1.activeClickableMenu));
    }

    #region harmony patches

    /// <summary>Reload profession sprites on level-up.</summary>
    [HarmonyPostfix]
    private static void Game1ActiveClickbleMenuSetterPostfix(IClickableMenu? value)
    {
        if (value is LevelUpMenu { isProfessionChooser: true } levelup)
        {
            var level = Reflector.GetUnboundFieldGetter<LevelUpMenu, int>("currentLevel").Invoke(levelup);
            if (level > 10)
            {
                ModHelper.GameContent.InvalidateCacheAndLocalized("LooseSprites/Cursors");
            }
        }
    }

    #endregion harmony patches
}
