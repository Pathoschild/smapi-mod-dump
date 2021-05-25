/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AndyCrocker/StardewMods
**
*************************************************/

using FarmAnimalVarietyRedux.EqualityComparers;
using FarmAnimalVarietyRedux.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;
using System.Linq;

namespace FarmAnimalVarietyRedux
{
    /// <summary>Handles asset loading through SMAPI's content pipeline.</summary>
    public class AssetManager : IAssetLoader
    {
        /*********
        ** Fields
        *********/
        /// <summary>The registered assets.</summary>
        private List<ManagedAsset> RegisteredAssets = new List<ManagedAsset>();


        /*********
        ** Public Methods
        *********/
        /// <summary>Registers a shop icon for an animal to be loaded from game content.</summary>
        /// <param name="internalAnimalName">The internal name of the animal the shop icon is for.</param>
        /// <param name="relativeTexturePath">The path to the shop icon relative to the game content folder.</param>
        /// <param name="sourceRectangle">The source rectangle of the shop icon.</param>
        public void RegisterAsset(string internalAnimalName, string relativeTexturePath, Rectangle sourceRectangle)
        {
            var newManagedAsset = new ManagedAsset(internalAnimalName, relativeTexturePath, sourceRectangle);

            // remove a preexisting asset if one exists, this is the case if an animal's assets are being editing by another pack
            var managedAssetEqualityComparer = new ManagedAssetEqualityComparer();
            var registedAsset = RegisteredAssets.FirstOrDefault(ra => managedAssetEqualityComparer.Equals(ra, newManagedAsset));
            if (registedAsset != null)
                RegisteredAssets.Remove(registedAsset);

            RegisteredAssets.Add(newManagedAsset);
        }

        /// <summary>Registers a shop icon for an animal.</summary>
        /// <param name="internalAnimalName">The internal name of the animal the sprite sheet is for.</param>
        /// <param name="relativeTexturePath">The path to the sprite sheet relative to <paramref name="contentPackOwner"/>.</param>
        /// <param name="contentPackOwner">The content pack that owns the asset.</param>
        public void RegisterAsset(string internalAnimalName, string relativeTexturePath, IContentPack contentPackOwner)
        {
            var newManagedAsset = new ManagedAsset(internalAnimalName, relativeTexturePath, contentPackOwner);

            // remove a preexisting asset if one exists, this is the case if an animal's assets are being editing by another pack
            var managedAssetEqualityComparer = new ManagedAssetEqualityComparer();
            var registedAsset = RegisteredAssets.FirstOrDefault(ra => managedAssetEqualityComparer.Equals(ra, newManagedAsset));
            if (registedAsset != null)
                RegisteredAssets.Remove(registedAsset);

            RegisteredAssets.Add(newManagedAsset);
        }

        /// <summary>Registers a spritesheet for an animal to be loaded from game content.</summary>
        /// <param name="internalAnimalName">The internal name of the animal the sprite sheet is for.</param>
        /// <param name="internalAnimalSubtypeName">The internal name of the subtype the sprite sheet is for.</param>
        /// <param name="isBaby">Whether the sprite sheet is for the baby version of the animal.</param>
        /// <param name="isHarvested">Whether the sprite sheet is for the harvested version of the animal.</param>
        /// <param name="season">The season the sprite sheet is for of the animal.</param>
        /// <param name="relativeTexturePath">The path to the sprite sheet relative to the game content folder.</param>
        public void RegisterAsset(string internalAnimalName, string internalAnimalSubtypeName, bool isBaby, bool isHarvested, string season, string relativeTexturePath)
        {
            var newManagedAsset = new ManagedAsset(internalAnimalName, internalAnimalSubtypeName, isBaby, isHarvested, season, relativeTexturePath);

            // remove a preexisting asset if one exists, this is the case if an animal's asset is being editing by another content pack
            var managedAssetEqualityComparer = new ManagedAssetEqualityComparer();
            var registedAsset = RegisteredAssets.FirstOrDefault(ra => managedAssetEqualityComparer.Equals(ra, newManagedAsset));
            if (registedAsset != null)
                RegisteredAssets.Remove(registedAsset);

            RegisteredAssets.Add(newManagedAsset);
        }

        /// <summary>Registers a spritesheet for an animal.</summary>
        /// <param name="internalAnimalName">The internal name of the animal the sprite sheet is for.</param>
        /// <param name="internalAnimalSubtypeName">The internal name of the subtype the sprite sheet is for.</param>
        /// <param name="isBaby">Whether the sprite sheet is for the baby version of the animal.</param>
        /// <param name="isHarvested">Whether the sprite sheet is for the harvested version of the animal.</param>
        /// <param name="season">The season the sprite sheet is for of the animal.</param>
        /// <param name="relativeTexturePath">The path to the sprite sheet relative to <paramref name="contentPackOwner"/>.</param>
        /// <param name="contentPackOwner">The content pack that owns the asset.</param>
        public void RegisterAsset(string internalAnimalName, string internalAnimalSubtypeName, bool isBaby, bool isHarvested, string season, string relativeTexturePath, IContentPack contentPackOwner)
        {
            var newManagedAsset = new ManagedAsset(internalAnimalName, internalAnimalSubtypeName, isBaby, isHarvested, season, relativeTexturePath, contentPackOwner);

            // remove a preexisting asset if one exists, this is the case if an animal's asset is being editing by another content pack
            var managedAssetEqualityComparer = new ManagedAssetEqualityComparer();
            var registedAsset = RegisteredAssets.FirstOrDefault(ra => managedAssetEqualityComparer.Equals(ra, newManagedAsset));
            if (registedAsset != null)
                RegisteredAssets.Remove(registedAsset);

            RegisteredAssets.Add(newManagedAsset);
        }

        /// <summary>Determines whether an animal subtype has valid spritesheets for harvested versions.</summary>
        /// <param name="internalAnimalName">The internal name of the animal that contains the subtype.</param>
        /// <param name="internalAnimalSubtypeName">The internal name of the subtype to determine whether it has different spritesheets for harvested versions.</param>
        /// <returns><see langword="true"/> if the subtype has different spritesheets for harvested versions; otherwise, <see langword="false"/>.</returns>
        public bool HasDifferentSpriteSheetWhenHarvested(string internalAnimalName, string internalAnimalSubtypeName)
        {
            var managedAssets = RegisteredAssets.Where(registeredAsset => registeredAsset.InternalAnimalName.ToLower() == internalAnimalName.ToLower() && registeredAsset.InternalAnimalSubtypeName?.ToLower() == internalAnimalSubtypeName.ToLower() && registeredAsset.IsHarvested);
            if (managedAssets.Count() == 0)
                return false;

            // check if there's a season invariant spritesheet
            if (managedAssets.Any(managedAsset => string.IsNullOrEmpty(managedAsset.Season)))
                return true;

            // check if there's a sprite sheet for all seasons
            var hasSpringSpriteSheet = managedAssets.Any(managedAsset => managedAsset.Season.ToLower() == "spring");
            var hasSummerSpriteSheet = managedAssets.Any(managedAsset => managedAsset.Season.ToLower() == "summer");
            var hasFallSpriteSheet = managedAssets.Any(managedAsset => managedAsset.Season.ToLower() == "fall");
            var hasWinterSpriteSheet = managedAssets.Any(managedAsset => managedAsset.Season.ToLower() == "winter");
            return hasSpringSpriteSheet && hasSummerSpriteSheet && hasFallSpriteSheet && hasWinterSpriteSheet;
        }

        /// <summary>Gets the <see cref="ManagedAsset.RelativeTexturePath"/> of a shop icon.</summary>
        /// <param name="internalAnimalName">The internal name of the animal whose shop icon path should be received.</param>
        /// <returns>The path of the shop icon of the animal with an internal name of <paramref name="internalAnimalName"/>.</returns>
        public string GetShopIconPath(string internalAnimalName)
        {
            var managedAsset = RegisteredAssets.FirstOrDefault(registeredAsset => registeredAsset.InternalAnimalName.ToLower() == internalAnimalName.ToLower() && registeredAsset.IsShopIcon);
            return managedAsset?.RelativeTexturePath ?? "";
        }

        /// <summary>Gets the source rectangle of a shop icon asset.</summary>
        /// <param name="internalAnimalName">The internal name of the animal whose shop icon source rectangle should be received.</param>
        /// <returns>The source rectangle of the shop icon of the animal with an internal name of <paramref name="internalAnimalName"/>.</returns>
        public Rectangle GetShopIconSourceRectangle(string internalAnimalName)
        {
            var managedAsset = RegisteredAssets.FirstOrDefault(registeredAsset => registeredAsset.InternalAnimalName.ToLower() == internalAnimalName.ToLower() && registeredAsset.IsShopIcon);
            return managedAsset?.SourceRectangle ?? new Rectangle(0, 0, 32, 16);
        }

        /// <inheritdoc/>
        public bool CanLoad<T>(IAssetInfo asset) => asset.AssetName.ToLower().StartsWith("favr");

        /// <inheritdoc/>
        public T Load<T>(IAssetInfo asset)
        {
            // ensure asset name is valid
            var assetName = asset.AssetName.Substring(4); // remove 'favr' prefix
            var splitAssetName = assetName.Split(',');
            if (splitAssetName.Length == 2)
            {
                // parse asset name
                var internalAnimalName = splitAssetName[0];
                var shopIconString = splitAssetName[1];
                if (shopIconString.ToLower() != "shopicon")
                    throw new ContentLoadException($"Failed to load FAVR shop icon: {asset.AssetName} as it's an invalid format");

                // retrieve asset
                var managedAsset = RegisteredAssets.FirstOrDefault(registeredAsset => registeredAsset.InternalAnimalName.ToLower() == internalAnimalName.ToLower() && registeredAsset.IsShopIcon);

                // load asset through content pack content pipeline
                if (managedAsset.IsGameContent)
                    return (T)(object)Game1.content.Load<Texture2D>(managedAsset.RelativeTexturePath);
                else
                    return (T)(object)managedAsset.ContentPackAssetOwner.LoadAsset<Texture2D>(managedAsset.RelativeTexturePath);
            }
            else if (splitAssetName.Length == 5)
            {
                // parse asset name
                var internalAnimalName = splitAssetName[0];
                var internalAnimalSubtypeName = splitAssetName[1];
                if (!bool.TryParse(splitAssetName[2], out var isBaby))
                    throw new ContentLoadException($"Failed to parse: {splitAssetName[2]} as a bool in the asset name: {asset.AssetName}");
                if (!bool.TryParse(splitAssetName[3], out var isHarvested))
                    throw new ContentLoadException($"Failed to parse: {splitAssetName[3]} as a bool in the asset name: {asset.AssetName}");
                var season = splitAssetName[4];
                if (string.IsNullOrEmpty(season))
                    season = null; // ensure that season is null when it's blank

                // retrieve asset
                var subtypeAssets = RegisteredAssets.Where(registeredAsset => registeredAsset.InternalAnimalName.ToLower() == internalAnimalName.ToLower() && registeredAsset.InternalAnimalSubtypeName?.ToLower() == internalAnimalSubtypeName.ToLower());

                // cache asset paths
                var adultSpriteSheet = subtypeAssets.FirstOrDefault(subtypeAsset => !subtypeAsset.IsBaby && !subtypeAsset.IsHarvested && subtypeAsset.Season == null);
                var harvestedSpriteSheet = subtypeAssets.FirstOrDefault(subtypeAsset => subtypeAsset.IsHarvested && subtypeAsset.Season == null);
                var babySpriteSheet = subtypeAssets.FirstOrDefault(subtypeAsset => subtypeAsset.IsBaby && subtypeAsset.Season == null);
                var springAdultSpriteSheet = subtypeAssets.FirstOrDefault(subtypeAsset => !subtypeAsset.IsBaby && !subtypeAsset.IsHarvested && subtypeAsset.Season?.ToLower() == "spring");
                var springHarvestedSpriteSheet = subtypeAssets.FirstOrDefault(subtypeAsset => subtypeAsset.IsHarvested && subtypeAsset.Season?.ToLower() == "spring");
                var springBabySpriteSheet = subtypeAssets.FirstOrDefault(subtypeAsset => subtypeAsset.IsBaby && subtypeAsset.Season?.ToLower() == "spring");
                var summerAdultSpriteSheet = subtypeAssets.FirstOrDefault(subtypeAsset => !subtypeAsset.IsBaby && !subtypeAsset.IsHarvested && subtypeAsset.Season?.ToLower() == "summer");
                var summerHarvestedSpriteSheet = subtypeAssets.FirstOrDefault(subtypeAsset => subtypeAsset.IsHarvested && subtypeAsset.Season?.ToLower() == "summer");
                var summerBabySpriteSheet = subtypeAssets.FirstOrDefault(subtypeAsset => subtypeAsset.IsBaby && subtypeAsset.Season?.ToLower() == "summer");
                var fallAdultSpriteSheet = subtypeAssets.FirstOrDefault(subtypeAsset => !subtypeAsset.IsBaby && !subtypeAsset.IsHarvested && subtypeAsset.Season?.ToLower() == "fall");
                var fallHarvestedSpriteSheet = subtypeAssets.FirstOrDefault(subtypeAsset => subtypeAsset.IsHarvested && subtypeAsset.Season?.ToLower() == "fall");
                var fallBabySpriteSheet = subtypeAssets.FirstOrDefault(subtypeAsset => subtypeAsset.IsBaby && subtypeAsset.Season?.ToLower() == "fall");
                var winterAdultSpriteSheet = subtypeAssets.FirstOrDefault(subtypeAsset => !subtypeAsset.IsBaby && !subtypeAsset.IsHarvested && subtypeAsset.Season?.ToLower() == "winter");
                var winterHarvestedSpriteSheet = subtypeAssets.FirstOrDefault(subtypeAsset => subtypeAsset.IsHarvested && subtypeAsset.Season?.ToLower() == "winter");
                var winterBabySpriteSheet = subtypeAssets.FirstOrDefault(subtypeAsset => subtypeAsset.IsBaby && subtypeAsset.Season?.ToLower() == "winter");

                var managedAsset = (isBaby, isHarvested, season) switch
                {
                    (false, false, "spring") => springAdultSpriteSheet ?? adultSpriteSheet,
                    (false, false, "summer") => summerAdultSpriteSheet ?? adultSpriteSheet,
                    (false, false, "fall") => fallAdultSpriteSheet ?? adultSpriteSheet,
                    (false, false, "winter") => winterAdultSpriteSheet ?? adultSpriteSheet,
                    (false, true, "spring") => springHarvestedSpriteSheet ?? harvestedSpriteSheet ?? springAdultSpriteSheet ?? adultSpriteSheet,
                    (false, true, "summer") => summerHarvestedSpriteSheet ?? harvestedSpriteSheet ?? summerAdultSpriteSheet ?? adultSpriteSheet,
                    (false, true, "fall") => fallHarvestedSpriteSheet ?? harvestedSpriteSheet ?? fallAdultSpriteSheet ?? adultSpriteSheet,
                    (false, true, "winter") => winterHarvestedSpriteSheet ?? harvestedSpriteSheet ?? winterAdultSpriteSheet ?? adultSpriteSheet,
                    (true, false, "spring") => springBabySpriteSheet ?? babySpriteSheet ?? springAdultSpriteSheet ?? adultSpriteSheet,
                    (true, false, "summer") => summerBabySpriteSheet ?? babySpriteSheet ?? summerAdultSpriteSheet ?? adultSpriteSheet,
                    (true, false, "fall") => fallBabySpriteSheet ?? babySpriteSheet ?? fallAdultSpriteSheet ?? adultSpriteSheet,
                    (true, false, "winter") => winterBabySpriteSheet ?? babySpriteSheet ?? winterAdultSpriteSheet ?? adultSpriteSheet,
                    (true, _, _) => babySpriteSheet ?? adultSpriteSheet,
                    _ => adultSpriteSheet ?? springAdultSpriteSheet ?? summerAdultSpriteSheet ?? fallAdultSpriteSheet ?? winterAdultSpriteSheet
                };

                if (managedAsset == null)
                    throw new ContentLoadException($"Failed to find a valid asset for: {asset.AssetName}");

                // load asset through content pack content pipeline
                if (managedAsset.IsGameContent)
                    return (T)(object)Game1.content.Load<Texture2D>(managedAsset.RelativeTexturePath);
                else
                    return (T)(object)managedAsset.ContentPackAssetOwner.LoadAsset<Texture2D>(managedAsset.RelativeTexturePath);
            }
            else
                throw new ContentLoadException($"Failed to load FAVR asset: {asset.AssetName} as it's an invalid format");
        }
    }
}
