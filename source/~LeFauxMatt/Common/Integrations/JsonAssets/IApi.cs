/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.Common.Integrations.JsonAssets;

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
///     API for Json Assets.
/// </summary>
public interface IApi
{
    event EventHandler AddedItemsToShop;

    event EventHandler IdsAssigned;

    event EventHandler IdsFixed;

    event EventHandler ItemsRegistered;

    bool FixIdsInItem(Item item);

    void FixIdsInItemList(List<Item> items);

    void FixIdsInLocation(GameLocation location);

    IDictionary<string, int> GetAllBigCraftableIds();

    List<string> GetAllBigCraftablesFromContentPack(string cp);

    List<string> GetAllBootsFromContentPack(string cp);

    List<string> GetAllClothingFromContentPack(string cp);

    IDictionary<string, int> GetAllClothingIds();

    IDictionary<string, int> GetAllCropIds();

    List<string> GetAllCropsFromContentPack(string cp);

    IDictionary<string, int> GetAllFruitTreeIds();

    List<string> GetAllFruitTreesFromContentPack(string cp);

    IDictionary<string, int> GetAllHatIds();

    List<string> GetAllHatsFromContentPack(string cp);

    IDictionary<string, int> GetAllObjectIds();

    List<string> GetAllObjectsFromContentPack(string cp);

    IDictionary<string, int> GetAllWeaponIds();

    List<string> GetAllWeaponsFromContentPack(string cp);

    int GetBigCraftableId(string name);

    int GetClothingId(string name);

    int GetCropId(string name);

    int GetFruitTreeId(string name);

    int[] GetGiantCropIndexes();

    int GetHatId(string name);

    int GetObjectId(string name);

    int GetWeaponId(string name);

    /// <summary>Load a folder as a Json Assets content pack.</summary>
    /// <param name="path">The absolute path to the content pack folder.</param>
    void LoadAssets(string path);

    /// <summary>Load a folder as a Json Assets content pack.</summary>
    /// <param name="path">The absolute path to the content pack folder.</param>
    /// <param name="translations">
    ///     The translations to use for <c>TranslationKey</c> fields, or <c>null</c> to load the content
    ///     pack's <c>i18n</c> folder if present.
    /// </param>
    void LoadAssets(string path, ITranslationHelper translations);

    bool TryGetCustomSprite(object entity, out Texture2D texture, out Rectangle sourceRect);

    bool TryGetCustomSpriteSheet(object entity, out Texture2D texture, out Rectangle sourceRect);

    bool TryGetGiantCropSprite(int productID, out Lazy<Texture2D> texture);
}