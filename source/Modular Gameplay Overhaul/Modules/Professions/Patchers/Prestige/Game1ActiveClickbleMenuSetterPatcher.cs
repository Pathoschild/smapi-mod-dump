/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Professions.Patchers.Prestige;

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
    internal Game1ActiveClickbleMenuSetterPatcher()
    {
        this.Target = this.RequirePropertySetter<Game1>(nameof(Game1.activeClickableMenu));
    }

    #region harmony patches

    /// <summary>Reload profession sprites on level-up.</summary>
    [HarmonyPostfix]
    private static void FarmerStaminaSetterPostfix(IClickableMenu value)
    {
        if (value is not LevelUpMenu { isProfessionChooser: true } levelup)
        {
            return;
        }

        var level = Reflector.GetUnboundFieldGetter<LevelUpMenu, int>("currentLevel").Invoke(levelup);
        if (level > 10)
        {
            ModHelper.GameContent.InvalidateCacheAndLocalized("LooseSprites/Cursors");
        }
    }

    #endregion harmony patches
}
