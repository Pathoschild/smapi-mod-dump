/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/thimadera/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using SObject = StardewValley.Object;

namespace StackEverythingRedux.Patches
{
    internal class TryToPlaceItemPatches
    {
        public static bool Prefix(ref bool __result, GameLocation location, Item item, int x, int y)
        {
            __result = TryToPlaceItem(location, item, x, y);
            return false;
        }

        private static bool TryToPlaceItem(GameLocation location, Item item, int x, int y)
        {
            if (item is null or Tool)
            {
                return false;
            }
            Vector2 tileLocation = new(x / 64, y / 64);
            if (Utility.playerCanPlaceItemHere(location, item, x, y, Game1.player))
            {
                if (item is Furniture)
                {
                    Game1.player.ActiveObject = null;
                }

                if (((SObject)item).placementAction(location, x, y, Game1.player))
                {
                    Game1.player.reduceActiveItemByOne();
                }
                else if (item is Furniture furniture)
                {
                    Game1.player.ActiveObject = furniture;
                }
                else if (item is Wallpaper)
                {
                    return false;
                }

                if (item is Furniture f && f.Stack > 1)
                {
                    Furniture copy = Copy(f);
                    if (copy != null)
                    {
                        copy.TileLocation = f.TileLocation;
                        copy.boundingBox.Value = f.boundingBox.Value;
                        copy.defaultBoundingBox.Value = f.defaultBoundingBox.Value;
                        copy.Stack = f.Stack - 1;
                        copy.updateDrawPosition();
                        Game1.player.ActiveObject = copy;
                    }

                    f.Stack = 1;
                }

                return true;
            }
            if (Utility.isPlacementForbiddenHere(location) && item != null && item.isPlaceable())
            {
                if (Game1.didPlayerJustClickAtAll(ignoreNonMouseHeldInput: true))
                {
                    Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13053"));
                }
            }
            else if (item is Furniture furniture && Game1.didPlayerJustLeftClick(ignoreNonMouseHeldInput: true))
            {
                switch (furniture.GetAdditionalFurniturePlacementStatus(location, x, y, Game1.player))
                {
                    case 1:
                        Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Furniture.cs.12629"));
                        break;
                    case 2:
                        Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Furniture.cs.12632"));
                        break;
                    case 3:
                        Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Furniture.cs.12633"));
                        break;
                    case 4:
                        Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Furniture.cs.12632"));
                        break;
                }
            }
            if (item.Category == -19 && location.terrainFeatures.TryGetValue(tileLocation, out TerrainFeature terrainFeature) && terrainFeature is HoeDirt dirt)
            {
                switch (dirt.CheckApplyFertilizerRules(item.QualifiedItemId))
                {
                    case HoeDirtFertilizerApplyStatus.HasThisFertilizer:
                        return false;
                    case HoeDirtFertilizerApplyStatus.HasAnotherFertilizer:
                        if (Game1.didPlayerJustClickAtAll(ignoreNonMouseHeldInput: true))
                        {
                            Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:HoeDirt.cs.13916-2"));
                        }
                        return false;
                    case HoeDirtFertilizerApplyStatus.CropAlreadySprouted:
                        if (Game1.didPlayerJustClickAtAll(ignoreNonMouseHeldInput: true))
                        {
                            Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:HoeDirt.cs.13916"));
                        }
                        return false;
                }
            }
            _ = Utility.playerCanPlaceItemHere(location, item, x, y, Game1.player, show_error: true);
            return false;
        }

        private static Furniture Copy(Furniture obj)
        {
            Furniture furniture = obj.getOne() as Furniture;

            int attempts = 0;
            while (!furniture.boundingBox.Value.Equals(obj.boundingBox.Value) && attempts < 8)
            {
                furniture.rotate();
                attempts++;
            }

            return furniture;
        }
    }
}
