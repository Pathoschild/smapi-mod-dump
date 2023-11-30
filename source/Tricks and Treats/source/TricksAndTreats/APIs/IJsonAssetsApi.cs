/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/cl4r3/Halloween-Mod-Jam-2023
**
*************************************************/


using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

namespace TricksAndTreats
{
    public interface IJsonAssetsApi
    {
        event EventHandler AddedItemsToShop;
        event EventHandler ItemsRegistered;

        /// <summary>
        /// Raised when JA assigns IDs.
        /// </summary>
        /// 
        event EventHandler IdsAssigned;

        /// <summary>
        /// Raised when JA tries to fix IDs.
        /// </summary>
        event EventHandler IdsFixed;

        /// <summary>Load a folder as a Json Assets content pack.</summary>
        /// <param name="path">The absolute path to the content pack folder.</param>
        /// <param name="translations">The translations to use for <c>TranslationKey</c> fields, or <c>null</c> to load the content pack's <c>i18n</c> folder if present.</param>
        void LoadAssets(string path, ITranslationHelper translations);

        /// <summary>
        /// Gets the object ID of an object declared through Json Assets.
        /// </summary>
        /// <param name="name">Name of object.</param>
        /// <returns>Integer object ID, or -1 if not found.</returns>
        int GetObjectId(string name);

        /// <summary>
        /// Gets the ID of an crop declared through Json Assets.
        /// </summary>
        /// <param name="name">Name of crop.</param>
        /// <returns>Integer crop ID, or -1 if not found.</returns>
        int GetCropId(string name);

        /// <summary>
        /// Gets the ID of a fruit tree declared through Json Assets.
        /// </summary>
        /// <param name="name">Name of fruit tree.</param>
        /// <returns>Integer fruit tree ID, or -1 if not found.</returns>
        int GetFruitTreeId(string name);

        /// <summary>
        /// Gets the ID of a bigCraftable declared through Json Assets.
        /// </summary>
        /// <param name="name">Name of the BigCraftable.</param>
        /// <returns>Integer BigCraftable ID, or -1 if not found.</returns>
        int GetBigCraftableId(string name);

        int GetHatId(string name);
        int GetWeaponId(string name);
        int GetClothingId(string name);

        IDictionary<string, int> GetAllObjectIds();
        IDictionary<string, int> GetAllCropIds();
        IDictionary<string, int> GetAllFruitTreeIds();
        IDictionary<string, int> GetAllBigCraftableIds();
        IDictionary<string, int> GetAllHatIds();
        IDictionary<string, int> GetAllWeaponIds();
        IDictionary<string, int> GetAllClothingIds();

        List<string> GetAllObjectsFromContentPack(string cp);
        List<string> GetAllCropsFromContentPack(string cp);
        List<string> GetAllFruitTreesFromContentPack(string cp);
        List<string> GetAllBigCraftablesFromContentPack(string cp);
        List<string> GetAllHatsFromContentPack(string cp);
        List<string> GetAllWeaponsFromContentPack(string cp);
        List<string> GetAllClothingFromContentPack(string cp);
        List<string> GetAllBootsFromContentPack(string cp);

        /// <summary>
        /// Fixes the ids in an item.
        /// </summary>
        /// <param name="item">Item to fix.</param>
        /// <returns>True if the item is to be removed, false otherwise.</returns>
        bool FixIdsInItem(Item item);
        void FixIdsInItemList(List<Item> items);
        void FixIdsInLocation(GameLocation location);

        /// <summary>
        /// Tries to get the custom sprite for a JA-registered item.
        /// </summary>
        /// <param name="entity">Entity to get the sprite of.</param>
        /// <param name="texture">out param, texture.</param>
        /// <param name="sourceRect">out param, source rectangle.</param>
        /// <returns>True if successful, false otherwise.</returns>
        bool TryGetCustomSprite(object entity, out Texture2D texture, out Rectangle sourceRect);

        bool TryGetCustomSpriteSheet(object entity, out Texture2D texture, out Rectangle sourceRect);

        /// <summary>
        /// Gets the texture of a Giant Crop.
        /// </summary>
        /// <param name="productID">The product ID.</param>
        /// <param name="texture">Out param, the texture.</param>
        /// <returns>True if successful, false otherwise.</returns>
        bool TryGetGiantCropSprite(int productID, out Lazy<Texture2D> texture);

        /// <summary>
        /// Gets the giant crops JA recognizes.
        /// </summary>
        /// <returns></returns>
        int[] GetGiantCropIndexes();
    }
}
