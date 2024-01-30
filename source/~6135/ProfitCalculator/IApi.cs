/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/6135/StardewValley.ProfitCalculator
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;

namespace JsonAssets
{
    /// <summary>
    ///
    /// </summary>
    public interface IApi
    {
        /// <summary>Load a folder as a Json Assets content pack.</summary>
        /// <param name="path">The absolute path to the content pack folder.</param>
        void LoadAssets(string path);

        /// <summary>Load a folder as a Json Assets content pack.</summary>
        /// <param name="path">The absolute path to the content pack folder.</param>
        /// <param name="translations">The translations to use for <c>TranslationKey</c> fields, or <c>null</c> to load the content pack's <c>i18n</c> folder if present.</param>
        void LoadAssets(string path, ITranslationHelper translations);

        /// <summary>
        ///
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        int GetObjectId(string name);

        /// <summary>
        ///
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        int GetCropId(string name);

        /// <summary>
        ///
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        int GetFruitTreeId(string name);

        /// <summary>
        ///
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        int GetBigCraftableId(string name);

        /// <summary>
        ///
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        int GetHatId(string name);

        /// <summary>
        ///
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        int GetWeaponId(string name);

        /// <summary>
        ///
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        int GetClothingId(string name);

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        IDictionary<string, int> GetAllObjectIds();

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        IDictionary<string, int> GetAllCropIds();

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        IDictionary<string, int> GetAllFruitTreeIds();

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        IDictionary<string, int> GetAllBigCraftableIds();

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        IDictionary<string, int> GetAllHatIds();

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        IDictionary<string, int> GetAllWeaponIds();

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        IDictionary<string, int> GetAllClothingIds();

        /// <summary>
        ///
        /// </summary>
        /// <param name="cp"></param>
        /// <returns></returns>
        List<string> GetAllObjectsFromContentPack(string cp);

        /// <summary>
        ///
        /// </summary>
        /// <param name="cp"></param>
        /// <returns></returns>
        List<string> GetAllCropsFromContentPack(string cp);

        /// <summary>
        ///
        /// </summary>
        /// <param name="cp"></param>
        /// <returns></returns>
        List<string> GetAllFruitTreesFromContentPack(string cp);

        /// <summary>
        ///
        /// </summary>
        /// <param name="cp"></param>
        /// <returns></returns>
        List<string> GetAllBigCraftablesFromContentPack(string cp);

        /// <summary>
        ///
        /// </summary>
        /// <param name="cp"></param>
        /// <returns></returns>
        List<string> GetAllHatsFromContentPack(string cp);

        /// <summary>
        ///
        /// </summary>
        /// <param name="cp"></param>
        /// <returns></returns>
        List<string> GetAllWeaponsFromContentPack(string cp);

        /// <summary>
        ///
        /// </summary>
        /// <param name="cp"></param>
        /// <returns></returns>
        List<string> GetAllClothingFromContentPack(string cp);

        /// <summary>
        ///
        /// </summary>
        /// <param name="cp"></param>
        /// <returns></returns>
        List<string> GetAllBootsFromContentPack(string cp);

        /// <summary>
        ///
        /// </summary>
        event EventHandler ItemsRegistered;

        /// <summary>
        ///
        /// </summary>
        event EventHandler IdsAssigned;

        /// <summary>
        ///
        /// </summary>
        event EventHandler AddedItemsToShop;

        /// <summary>
        ///
        /// </summary>
        event EventHandler IdsFixed;

        /// <summary>
        ///
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool FixIdsInItem(Item item);

        /// <summary>
        ///
        /// </summary>
        /// <param name="items"></param>
        void FixIdsInItemList(List<Item> items);

        /// <summary>
        ///
        /// </summary>
        /// <param name="location"></param>
        void FixIdsInLocation(GameLocation location);

        /// <summary>
        ///
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="texture"></param>
        /// <param name="sourceRect"></param>
        /// <returns></returns>
        bool TryGetCustomSprite(object entity, out Texture2D texture, out Rectangle sourceRect);

        /// <summary>
        ///
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="texture"></param>
        /// <param name="sourceRect"></param>
        /// <returns></returns>
        bool TryGetCustomSpriteSheet(object entity, out Texture2D texture, out Rectangle sourceRect);

        /// <summary>
        ///
        /// </summary>
        /// <param name="productID"></param>
        /// <param name="texture"></param>
        /// <returns></returns>
        bool TryGetGiantCropSprite(int productID, out Lazy<Texture2D> texture);

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        int[] GetGiantCropIndexes();
    }
}