/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Combat.Patchers;

#region using directives

using System.Reflection;
using DaLion.Overhaul.Modules.Combat.Extensions;
using DaLion.Shared.Constants;
using DaLion.Shared.Extensions;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class BreakableContainerReleaseContentsPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="BreakableContainerReleaseContentsPatcher"/> class.</summary>
    internal BreakableContainerReleaseContentsPatcher()
    {
        this.Target = this.RequireMethod<BreakableContainer>(nameof(BreakableContainer.releaseContents));
    }

    #region harmony patches

    /// <summary>Better weapon odds from mine containers.</summary>
    [HarmonyPrefix]
    private static bool BreakableContainerReleaseContentsPrefix(BreakableContainer __instance, GameLocation location, Farmer who)
    {
        if (!CombatModule.Config.EnableWeaponOverhaul || location is not MineShaft shaft)
        {
            return true; // run original logic
        }

        try
        {
            var r = new Random(Guid.NewGuid().GetHashCode());
            if (r.NextDouble() < 0.2)
            {
                CombatModule.State.ContainerDropAccumulator += 0.005d;
                return false; // don't run original logic
            }

            var x = (int)__instance.TileLocation.X;
            var y = (int)__instance.TileLocation.Y;
            var mineLevel = shaft.mineLevel;
            if (shaft.isContainerPlatform(x, y))
            {
                shaft.updateMineLevelData(0, -1);
            }

            if (r.NextDouble() < CombatModule.State.ContainerDropAccumulator)
            {
                CombatModule.State.ContainerDropAccumulator = 0.05;
                var drop = MineShaft.getSpecialItemForThisMineLevel(mineLevel, x, y);
                if (drop is MeleeWeapon weapon)
                {
                    weapon.RandomizeDamage();
                }

                Game1.createItemDebris(
                    drop,
                    (new Vector2(x, y) * Game1.tileSize) + new Vector2(32f, 32f),
                    r.Next(4),
                    location);
                return false; // don't run original logic
            }

            if (r.NextDouble() < 0.05 && Game1.player.team.SpecialOrderRuleActive("DROP_QI_BEANS"))
            {
                Game1.createMultipleObjectDebris(890, x, y, r.Next(1, 3), who.UniqueMultiplayerID, location);
            }

            if (shaft.GetAdditionalDifficulty() > 0)
            {
                // qi gem
                if (r.NextDouble() < 0.0068)
                {
                    Game1.createMultipleObjectDebris(858, x, y, 1, location);
                }

                // trimmed lucky purple shorts ?
                if (r.NextDouble() < 0.0085)
                {
                    Game1.createItemDebris(
                        new SObject(Vector2.Zero, 71),
                        (new Vector2(x, y) * 64f) + new Vector2(32f),
                        0);
                }

                if (r.NextDouble() > 0.85)
                {
                    switch (r.Next(11))
                    {
                        case 1:
                            Game1.createMultipleObjectDebris(ObjectIds.Coal, x, y, r.Next(1, 3), location);
                            break;
                        case 2:
                            Game1.createMultipleObjectDebris(ObjectIds.Wood, x, y, r.Next(2, 6), location);
                            break;
                        case 3:
                            Game1.createMultipleObjectDebris(ObjectIds.Stone, x, y, r.Next(2, 6), location);
                            break;
                        case 4:
                            Game1.createMultipleObjectDebris(ObjectIds.Hardwood, x, y, 1, location); // hardwood
                            break;
                        case 5:
                            Game1.createMultipleObjectDebris(62 + (r.Next(7) * 2), x, y, 1, location); // gemstone
                            break;
                        case 6:
                            Game1.createMultipleObjectDebris(r.Next(535, 538), x, y, 1, location); // geode
                            break;
                        case 7:
                            Game1.createMultipleObjectDebris(80 + (r.Next(3) * 2), x, y, 1, location); // forage mineral
                            break;
                        case 8:
                            Game1.createMultipleObjectDebris(r.Next(218, 245), x, y, 1, location); // food
                            break;
                        case 9:
                            Game1.createMultipleObjectDebris(r.Next(286, 288), x, y, 1, location); // bomb
                            break;
                        case 10:
                            Game1.createMultipleObjectDebris(r.Next(918, 921), x, y, 1, location); // level-3 fertilizer
                            break;
                        default:
                            Game1.createMultipleObjectDebris(
                                r.Choose(ObjectIds.CopperOre, ObjectIds.IronOre, ObjectIds.GoldOre, ObjectIds.IridiumOre),
                                x,
                                y,
                                r.Next(1, 4),
                                location);
                            break;
                    }

                    CombatModule.State.ContainerDropAccumulator += 0.005d;
                    return false; // don't run original logic
                }
            }

            if (r.NextDouble() < 0.65)
            {
                if (r.NextDouble() < 0.8)
                {
                    switch (r.Next(7))
                    {
                        case 0:
                            Game1.createMultipleObjectDebris(ObjectIds.Coal, x, y, r.Next(1, 3), location);
                            break;
                        case 1:
                            Game1.createMultipleObjectDebris(ObjectIds.Wood, x, y, r.Next(2, 6), location);
                            break;
                        case 2:
                            Game1.createMultipleObjectDebris(ObjectIds.Stone, x, y, r.Next(2, 6), location);
                            break;
                        case 3: // ore
                            Game1.createMultipleObjectDebris(
                                mineLevel < 40
                                    ? ObjectIds.CopperOre
                                    : mineLevel < 80
                                        ? ObjectIds.IronOre
                                        : mineLevel < 120
                                            ? ObjectIds.GoldOre
                                            : ObjectIds.IridiumOre,
                                x,
                                y,
                                r.Next(1, 4),
                                location);
                            break;
                        case 4: // ore - 1
                            Game1.createMultipleObjectDebris(
                                mineLevel < 40
                                    ? ObjectIds.Stone
                                    : mineLevel < 80
                                        ? ObjectIds.CopperOre
                                        : mineLevel < 120
                                            ? ObjectIds.IronOre
                                            : ObjectIds.GoldOre,
                                x,
                                y,
                                r.Next(2, 6),
                                location);
                            break;
                        case 5: // ore - 2
                            Game1.createMultipleObjectDebris(
                                mineLevel < 40
                                    ? ObjectIds.Wood
                                    : mineLevel < 80
                                        ? ObjectIds.Stone
                                        : mineLevel < 120
                                            ? ObjectIds.CopperOre
                                            : ObjectIds.IronOre,
                                x,
                                y,
                                r.Next(2, 6),
                                location);
                            break;
                        case 6: // etc.
                            Game1.createMultipleObjectDebris(
                                mineLevel > 120 ? ObjectIds.BoneFragment : ObjectIds.MixedSeeds, x, y, 1, location);
                            break;
                    }
                }
                else
                {
                    Game1.createMultipleObjectDebris(ObjectIds.CaveCarrot, x, y, r.Next(1, 3), location);
                }
            }
            else if (r.NextDouble() < 0.4)
            {
                switch (r.Next(5))
                {
                    case 0:
                        Game1.createMultipleObjectDebris(shaft.ChooseGem(), x, y, 1, location);
                        break;
                    case 1:
                        Game1.createMultipleObjectDebris(shaft.ChooseGeode(), x, y, 1, location);
                        break;
                    case 2:
                        Game1.createMultipleObjectDebris(shaft.ChooseForageMineral(), x, y, 1, location);
                        break;
                    case 3:
                        Game1.createMultipleObjectDebris(ObjectIds.Hardwood, x, y, 1, location);
                        break;
                    case 4: // fertilizer or sap
                        var what = who.timesReachedMineBottom > 0 && r.NextDouble() < 0.5
                            ? mineLevel < 80
                                ? r.Choose(
                                    ObjectIds.BasicFertilizer,
                                    ObjectIds.BasicRetainingSoil,
                                    ObjectIds.SpeedGro)
                                : r.Choose(
                                    ObjectIds.QualityFertilizer,
                                    ObjectIds.BasicRetainingSoil,
                                    ObjectIds.SpeedGro)
                            : ObjectIds.Sap;
                        Game1.createMultipleObjectDebris(
                            what,
                            x,
                            y,
                            what == ObjectIds.Sap ? r.Next(2, 4) : 1,
                            location);
                        break;
                }
            }

            CombatModule.State.ContainerDropAccumulator += 0.01d;
            return false; // don't run original logic
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches
}
