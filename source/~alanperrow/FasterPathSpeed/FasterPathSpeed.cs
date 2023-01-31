/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/alanperrow/StardewModding
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;

namespace FasterPathSpeed
{
    internal class FasterPathSpeed
    {
        #region Farmer Methods
        public static void GetFarmerMovementSpeed(Farmer who, ref float refMovementSpeed)
        {
            if ((Game1.CurrentEvent == null || Game1.CurrentEvent.playerControlSequence) && (!(Game1.CurrentEvent == null && who.hasBuff(19))) &&
                    (!ModEntry.Context.Config.IsPathSpeedBuffOnlyOnTheFarm || Game1.currentLocation.IsFarm))
            {
                bool isOnFeature = Game1.currentLocation.terrainFeatures.TryGetValue(who.getTileLocation(), out TerrainFeature terrainFeature);

                if (isOnFeature && terrainFeature is Flooring)
                {
                    float pathSpeedBoost = GetPathSpeedBoostByFlooringType(terrainFeature as Flooring);

                    float mult = who.movementMultiplier * Game1.currentGameTime.ElapsedGameTime.Milliseconds *
                        ((!Game1.eventUp && who.isRidingHorse()) ? (ModEntry.Context.Config.IsPathAffectHorseSpeed ? ModEntry.Context.Config.HorsePathSpeedBuffModifier : 0) : 1);

                    refMovementSpeed += (who.movementDirections.Count > 1) ? (0.7f * pathSpeedBoost * mult) : (pathSpeedBoost * mult);
                }
            }
        }

        public static float GetPathSpeedBoostByFlooringType(Flooring flooring)
        {
            if (ModEntry.Context.Config.IsUseCustomPathSpeedBuffValues)
            {
                switch (flooring.whichFloor.Value)
                {
                    case Flooring.wood:
                        return ModEntry.Context.Config.CustomPathSpeedBuffValues.Wood;
                    case Flooring.stone:
                        return ModEntry.Context.Config.CustomPathSpeedBuffValues.Stone;
                    case Flooring.ghost:
                        return ModEntry.Context.Config.CustomPathSpeedBuffValues.Ghost;
                    case Flooring.iceTile:
                        return ModEntry.Context.Config.CustomPathSpeedBuffValues.IceTile;
                    case Flooring.straw:
                        return ModEntry.Context.Config.CustomPathSpeedBuffValues.Straw;
                    case Flooring.gravel:
                        return ModEntry.Context.Config.CustomPathSpeedBuffValues.Gravel;
                    case Flooring.boardwalk:
                        return ModEntry.Context.Config.CustomPathSpeedBuffValues.Boardwalk;
                    case Flooring.colored_cobblestone:
                        return ModEntry.Context.Config.CustomPathSpeedBuffValues.ColoredCobblestone;
                    case Flooring.cobblestone:
                        return ModEntry.Context.Config.CustomPathSpeedBuffValues.Cobblestone;
                    case Flooring.steppingStone:
                        return ModEntry.Context.Config.CustomPathSpeedBuffValues.SteppingStone;
                    case Flooring.brick:
                        return ModEntry.Context.Config.CustomPathSpeedBuffValues.Brick;
                    case Flooring.plankFlooring:
                        return ModEntry.Context.Config.CustomPathSpeedBuffValues.PlankFlooring;
                    case Flooring.townFlooring:
                        return ModEntry.Context.Config.CustomPathSpeedBuffValues.TownFlooring;
                }
            }

            return ModEntry.Context.Config.DefaultPathSpeedBuff;
        }
        #endregion

        #region Object Methods
        public static void ObjectPlacementAction(Object obj, ref bool refSuccess, GameLocation location, int x, int y, Farmer who)
        {
            if (ObjectIsPath(obj) && ModEntry.Context.Config.IsEnablePathReplace)
            {
                Vector2 placementTile = new Vector2(x / 64, y / 64);

                if (!obj.bigCraftable.Value && !(obj is Furniture)
                    && location.terrainFeatures.TryGetValue(placementTile, out TerrainFeature terrainFeature)
                    && terrainFeature is Flooring flooring
                    && PathIds.WhichIds[flooring.whichFloor.Value] != obj.ParentSheetIndex)
                {
                    location.terrainFeatures.Remove(placementTile);
                    location.terrainFeatures.Add(placementTile, new Flooring(PathIds.WhichIds.IndexOf(obj.ParentSheetIndex)));

                    var replacedPath = new Object(PathIds.WhichIds[flooring.whichFloor.Value], 1);
                    if (!who.addItemToInventoryBool(replacedPath))
                    {
                        who.dropItem(replacedPath);
                    }

                    location.playSound(GetPathSoundStringByPathId(obj.ParentSheetIndex));

                    refSuccess = true;
                }
            }
        }

        private static bool ObjectIsPath(Object obj)
        {
            return PathIds.WhichIds.Contains(obj.ParentSheetIndex);
        }

        private static string GetPathSoundStringByPathId(int id)
        {
            switch (id)
            {
                case PathIds.Wood: return "axchop";
                case PathIds.Stone: return "thudStep";
                case PathIds.Ghost: return "axchop";
                case PathIds.IceTile: return "thudStep";
                case PathIds.Straw: return "thudStep";
                case PathIds.Brick: return "thudStep";
                case PathIds.Boardwalk: return "woodyStep";
                case PathIds.Gravel: return "dirtyHit";
                case PathIds.ColoredCobblestone: return "stoneStep";
                case PathIds.SteppingStone: return "stoneStep";
                case PathIds.Cobblestone: return "stoneStep";
                case PathIds.PlankFlooring: return "stoneStep";
                case PathIds.TownFlooring: return "stoneStep";
            }

            return "stoneStep";
        }
        #endregion
    }
}
