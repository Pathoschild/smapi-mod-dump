/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Prism-99/WhoLivesHere
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Prism99_Core.PatchingFramework;
using Prism99_Core.Utilities;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;

namespace WhoLivesHere
{
    /// <summary>
    /// Version 1.5 logic
    /// </summary>
    internal static class VersionLevel
    {
        public static AnimalHouse? GetHoverBuilding()
        {
            if (Game1.currentLocation is BuildableGameLocation gl)
            {
                Building building = gl.getBuildingAt(Game1.currentCursorTile);
                if (building != null && building.indoors.Value != null && building.indoors.Value is AnimalHouse house)
                    return house;
            }

            return null;
        }
        public static int MaxCapacity(Building house)
        {
            return house.buildingType.Value switch
            {
                "Big Coop" => 8,
                "Deluxe Coop" => 12,
                "Big Barn" => 8,
                "Deluxe Barn" => 12,
                _ => house.maxOccupants
            };
        }
        public static bool WasFeed(FarmAnimal animal)
        {
            return animal.fullness.Value > 200;
        }
        /// <summary>
        /// Add required harmony postafix patches required for drawing
        /// </summary>
        /// <param name="modId">Modid for Haromony initialization</param>
        /// <param name="logger">SDVLogger</param>
        public static void ApplyPatches(string modId, SDVLogger logger)
        {
            GamePatches Patches = new GamePatches();
            Patches.Initialize(modId, logger);
            //
            //  add drawing patches for both animal house types
            //
            Patches.AddPatch(false, typeof(Coop), "draw",
                new Type[] { typeof(SpriteBatch) }, typeof(WhoLivesHereLogic),
                nameof(WhoLivesHereLogic.BuildingDraw_Suffix), "Capture draw for drawing occupant images.",
                "Buildings");
            Patches.AddPatch(false, typeof(Barn), "draw",
                new Type[] { typeof(SpriteBatch) }, typeof(WhoLivesHereLogic),
                nameof(WhoLivesHereLogic.BuildingDraw_Suffix), "Capture draw for drawing occupant images.",
                "Buildings");

            Patches.ApplyPatches("");
        }
        public static bool TryGetAnimalPortraitDetails(string animalType, out Point portraitSize, out string textName)
        {
            portraitSize = Point.Zero;
            textName = null;

            Dictionary<string, string> animal_data = Game1.content.Load<Dictionary<string, string>>("Data/FarmAnimals");

            if (animal_data.TryGetValue(animalType, out var animalData))
            {
                int.TryParse(animalData.Split("/")[16], out int SpriteWidth);
                int.TryParse(animalData.Split("/")[17], out int SpriteHeight);
                portraitSize = new Point(SpriteWidth, SpriteHeight);
                textName = "Animals/" + animalType;
                return true;
            }

            return false;
        }
        public static void HudRendered(SpriteBatch b)
        {
            if (Game1.currentLocation is BuildableGameLocation bl)
                foreach (Building building in bl.buildings)
                    if (building.indoors.Value is AnimalHouse animalHouse)
                    {
                        bool anyHayMissing = animalHouse.numberOfObjectsWithName("Hay") < animalHouse.animalLimit.Value;
                        //var anyProduce = _config.DrawBubbleBuildingsWithProduce && building is Coop &&
                        //                 animalHouse.objects.Values.Any(o =>
                        //                     Array.BinarySearch(AnimalTaskEngine.CollectableAnimalProducts, o.ParentSheetIndex) >= 0);

                        Vector2 v = new Vector2(building.tileX.Value * Game1.tileSize - Game1.viewport.X + Game1.tileSize * 1.1f,
                            building.tileY.Value * Game1.tileSize - Game1.viewport.Y + Game1.tileSize / 2);

                        if (building is Barn)
                            v.Y += Game1.tileSize / 2f;

                        if (anyHayMissing)
                        {
                            WhoLivesHereLogic.DrawBubble(b, Game1.objectSpriteSheet, new Rectangle(160, 112, 16, 16), v);
                        }
                    }
        }
    }
}