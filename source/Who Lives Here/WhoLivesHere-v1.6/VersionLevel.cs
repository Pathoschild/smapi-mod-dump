/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Prism-99/WhoLivesHere
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using Prism99_Core.PatchingFramework;
using StardewValley.Buildings;
using Prism99_Core.Utilities;

namespace WhoLivesHere
{
    /// <summary>
    /// Version 1.6 logic
    /// </summary>
    internal static class VersionLevel
    {
        public static AnimalHouse? GetHoverBuilding()
        {
            Building building= Game1.currentLocation.getBuildingAt(Game1.currentCursorTile);

            if (building != null &&  building.indoors.Value != null && building.indoors.Value is AnimalHouse house)
                return house;

            return null;
        }
        public static int MaxCapacity(Building house)
        {
            return house.GetData().MaxOccupants;
        }
        public static bool WasFeed(FarmAnimal animal)
        {
            return animal.fullness.Value >= 200;
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
            //  single patch required for 1.6, all building type are same class
            //
            Patches.AddPatch(false, typeof(Building), "draw",
                new Type[] { typeof(SpriteBatch) }, typeof(WhoLivesHereLogic),
                nameof(WhoLivesHereLogic.BuildingDraw_Suffix), "Capture draw for drawing occupant images.",
                "Buildings");

            Patches.ApplyPatches("");
        }
        public static bool TryGetAnimalPortraitDetails(string animalType, out Point portraitSize, out string textName)
        {
            portraitSize = Point.Zero;
            textName = string.Empty;

            if (Game1.farmAnimalData.TryGetValue(animalType, out var animalData))
            {
                portraitSize = new Point(animalData.SpriteWidth, animalData.SpriteHeight);
                if (string.IsNullOrEmpty(animalData.Texture))
                {
                    textName = "Animals/" + animalType;
                }
                else
                {
                    textName = animalData.Texture;
                }
                return true;
            }

            return false;
        }
    }
}