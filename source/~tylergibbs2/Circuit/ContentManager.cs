/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using Circuit.Locations;
using Circuit.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley;

namespace Circuit
{
    internal static class ContentManager
    {
        /// <summary>
        /// All assets that have been modified.
        /// The dictionary key is the name of the asset as the game requests it,
        /// the value is the filepath of the asset to replace it with.
        /// </summary>
        private static readonly Dictionary<string, string> ModifiedAssets = new()
        {
            { "Maps/CircuitSpawnRoom", "assets/Maps/Spawn_Room.tmx" }
        };

        /// <summary>
        /// Event handler for when a game asset is requested to be loaded.
        /// This handles loading all custom assets as defined in <see cref="ModifiedAssets"/>.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="AssetRequestedEventArgs"/> instance containing the event data.</param>
        /// <exception cref="NotImplementedException">If a file attempting to be loaded has no implemented way to be loaded.</exception>
        public static void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
        {
            if (ModifiedAssets.TryGetValue(e.Name.BaseName, out string? fromPath))
            {
                string extension = Path.GetExtension(fromPath);
                switch (extension)
                {
                    case ".tmx":
                        e.LoadFromModFile<xTile.Map>(fromPath, AssetLoadPriority.High);
                        return;
                    case ".png":
                        e.LoadFromModFile<Texture2D>(fromPath, AssetLoadPriority.High);
                        return;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        public static void LoadMaps()
        {
            CircuitSpawnRoom location = new("Maps/CircuitSpawnRoom", "CircuitSpawnRoom");

            Vector2 computerLocation = new(26, 32);
            location.Objects.Add(computerLocation, new SetupComputer(computerLocation));

            Game1.locations.Add(location);
        }

        public static void UnloadMaps(object? sender, SavingEventArgs e)
        {
            GameLocation location = Game1.getLocationFromName("CircuitSpawnRoom");

            if (location is not null)
                Game1.locations.Remove(location);
        }
    }
}
