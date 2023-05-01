/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Weapons.Patchers;

#region using directives

using DaLion.Overhaul.Modules.Weapons.Extensions;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationMonsterDropPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationMonsterDropPatcher"/> class.</summary>
    internal GameLocationMonsterDropPatcher()
    {
        this.Target = this.RequireMethod<GameLocation>(nameof(GameLocation.monsterDrop));
    }

    #region harmony patches

    /// <summary>Drop Obsidian Edge and Lava Katana.</summary>
    [HarmonyPostfix]
    private static void GameLocationMonsterDropPostfix(
        GameLocation __instance,
        Monster monster,
        int x,
        int y,
        Farmer who)
    {
        if (!WeaponsModule.Config.EnableRebalance)
        {
            return;
        }

        MeleeWeapon? drop = null;
        switch (monster)
        {
            case ShadowBrute:
            case ShadowGirl:
            case ShadowGuy:
            case ShadowShaman:
                if (Game1.mine is null)
                {
                    return;
                }

                if (Game1.mine.GetAdditionalDifficulty() > 0 &&
                    Game1.random.NextDouble() < 0.015 + (who.LuckLevel * 0.002f))
                {
                    drop = new MeleeWeapon(ItemIDs.ObsidianEdge);
                }

                break;
            case Bat bat when bat.magmaSprite.Value:
                if (Game1.random.NextDouble() < 0.01 + (who.LuckLevel * 0.003f))
                {
                    drop = new MeleeWeapon(ItemIDs.LavaKatana);
                }

                break;
        }

        if (drop is null)
        {
            return;
        }

        drop.RandomizeDamage(2d);
        __instance.debris.Add(new Debris(drop, new Vector2(x, y)));
    }

    #endregion harmony patches
}
