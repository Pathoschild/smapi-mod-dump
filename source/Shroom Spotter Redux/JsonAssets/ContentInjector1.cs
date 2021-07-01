/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using JsonAssets.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceCore;
using SpaceShared;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.Crafting;

namespace JsonAssets
{
    public class ContentInjector1 : IAssetEditor, IAssetLoader
    {
        private delegate void injector(IAssetData asset);
        private Dictionary<string, injector> files;
        public ContentInjector1()
        {
            Func<string, string> normalize = Mod.instance.Helper.Content.NormalizeAssetName;

            //normalize with 
            files = new Dictionary<string, injector>()
            {
                {normalize("Data\\ObjectInformation"), injectDataObjectInformation},
                {normalize("Data\\ObjectContextTags"), injectDataObjectContextTags},
                {normalize("Data\\Crops"), injectDataCrops},
                {normalize("Data\\fruitTrees"), injectDataFruitTrees},
                {normalize("Data\\CookingRecipes"), injectDataCookingRecipes},
                {normalize("Data\\CraftingRecipes"), injectDataCraftingRecipes},
                {normalize("Data\\BigCraftablesInformation"), injectDataBigCraftablesInformation},
                {normalize("Data\\hats"), injectDataHats},
                {normalize("Data\\weapons"), injectDataWeapons},
                {normalize("Data\\ClothingInformation"), injectDataClothingInformation},
                {normalize("Data\\TailoringRecipes"), injectDataTailoringRecipes},
                {normalize("Data\\Boots"), injectDataBoots},
                {normalize("Maps\\springobjects"), injectMapsSpringobjects},
                {normalize("TileSheets\\crops"), injectTileSheetsCrops},
                {normalize("TileSheets\\fruitTrees"), injectTileSheetsFruitTrees},
                {normalize("TileSheets\\Craftables"), injectTileSheetsCraftables},
                {normalize("Characters\\Farmer\\hats"), injectCharactersFarmerHats},
                {normalize("TileSheets\\weapons"), injectTileSheetsWeapons},
                {normalize("Characters\\Farmer\\shirts"), injectCharactersFarmerShirts},
                {normalize("Characters\\Farmer\\pants"), injectCharactersFarmerPants},
                {normalize("Characters\\Farmer\\shoeColors"), injectCharactersFarmerShoeColors}
            };
        }

        public void InvalidateUsed()
        {
            Mod.instance.Helper.Content.InvalidateCache((a) =>
            {
                return files.ContainsKey(a.AssetName);
            });
        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            return files.ContainsKey(asset.AssetName);
        }

        public void Edit<T>(IAssetData asset)
        {
            if (!Mod.instance.didInit)
                return;

            files[asset.AssetName](asset);
        }

        private void injectDataObjectInformation(IAssetData asset)
        {
            var data = asset.AsDictionary<int, string>().Data;
            foreach (var obj in Mod.instance.objects)
            {
                try
                {
                    Log.verbose($"Injecting to objects: {obj.GetObjectId()}: {obj.GetObjectInformation()}");
                    data.Add(obj.GetObjectId(), obj.GetObjectInformation());
                }
                catch (Exception e)
                {
                    Log.error($"Exception injecting object information for {obj.Name}: {e}");
                }
            }
        }
        private void injectDataObjectContextTags(IAssetData asset)
        {
            var data = asset.AsDictionary<string, string>().Data;
            foreach (var obj in Mod.instance.objects)
            {
                try
                {
                    string tags = string.Join(", ", obj.ContextTags);
                    Log.verbose($"Injecting to object context tags: {obj.Name}: {tags}");
                    if (data.ContainsKey(obj.Name) && data[obj.Name] == "")
                        data[obj.Name] = tags;
                    else
                        data.Add(obj.Name, tags);
                }
                catch (Exception e)
                {
                    Log.error($"Exception injecting object context tags for {obj.Name}: {e}");
                }
            }
        }
        private void injectDataCrops(IAssetData asset)
        {
            var data = asset.AsDictionary<int, string>().Data;
            foreach (var crop in Mod.instance.crops)
            {
                try
                {
                    Log.verbose($"Injecting to crops: {crop.GetSeedId()}: {crop.GetCropInformation()}");
                    data.Add(crop.GetSeedId(), crop.GetCropInformation());
                }
                catch (Exception e)
                {
                    Log.error($"Exception injecting crop for {crop.Name}: {e}");
                }
            }
        }
        private void injectDataFruitTrees(IAssetData asset)
        {
            var data = asset.AsDictionary<int, string>().Data;
            foreach (var fruitTree in Mod.instance.fruitTrees)
            {
                try
                {
                    Log.verbose($"Injecting to fruit trees: {fruitTree.GetSaplingId()}: {fruitTree.GetFruitTreeInformation()}");
                    data.Add(fruitTree.GetSaplingId(), fruitTree.GetFruitTreeInformation());
                }
                catch (Exception e)
                {
                    Log.error($"Exception injecting fruit tree for {fruitTree.Name}: {e}");
                }
            }
        }
        private void injectDataCookingRecipes(IAssetData asset)
        {
            var data = asset.AsDictionary<string, string>().Data;
            foreach (var obj in Mod.instance.objects)
            {
                try
                {
                    if (obj.Recipe == null)
                        continue;
                    if (obj.Category != ObjectData.Category_.Cooking)
                        continue;
                    Log.verbose($"Injecting to cooking recipes: {obj.Name}: {obj.Recipe.GetRecipeString(obj)}");
                    data.Add(obj.Name, obj.Recipe.GetRecipeString(obj));
                }
                catch (Exception e)
                {
                    Log.error($"Exception injecting cooking recipe for {obj.Name}: {e}");
                }
            }
        }
        private void injectDataCraftingRecipes(IAssetData asset)
        {
            var data = asset.AsDictionary<string, string>().Data;
            foreach (var obj in Mod.instance.objects)
            {
                try
                {
                    if (obj.Recipe == null)
                        continue;
                    if (obj.Category == ObjectData.Category_.Cooking)
                        continue;
                    Log.verbose($"Injecting to crafting recipes: {obj.Name}: {obj.Recipe.GetRecipeString(obj)}");
                    data.Add(obj.Name, obj.Recipe.GetRecipeString(obj));
                }
                catch (Exception e)
                {
                    Log.error($"Exception injecting crafting recipe for {obj.Name}: {e}");
                }
            }
            foreach (var big in Mod.instance.bigCraftables)
            {
                try
                {
                    if (big.Recipe == null)
                        continue;
                    Log.verbose($"Injecting to crafting recipes: {big.Name}: {big.Recipe.GetRecipeString(big)}");
                    data.Add(big.Name, big.Recipe.GetRecipeString(big));
                }
                catch (Exception e)
                {
                    Log.error($"Exception injecting crafting recipe for {big.Name}: {e}");
                }
            }
        }
        private void injectDataBigCraftablesInformation(IAssetData asset)
        {
            var data = asset.AsDictionary<int, string>().Data;
            foreach (var big in Mod.instance.bigCraftables)
            {
                try
                {
                    Log.verbose($"Injecting to big craftables: {big.GetCraftableId()}: {big.GetCraftableInformation()}");
                    data.Add(big.GetCraftableId(), big.GetCraftableInformation());
                }
                catch (Exception e)
                {
                    Log.error($"Exception injecting object information for {big.Name}: {e}");
                }
            }
        }
        private void injectDataHats(IAssetData asset)
        {
            var data = asset.AsDictionary<int, string>().Data;
            foreach (var hat in Mod.instance.hats)
            {
                try
                {
                    Log.verbose($"Injecting to hats: {hat.GetHatId()}: {hat.GetHatInformation()}");
                    data.Add(hat.GetHatId(), hat.GetHatInformation());
                }
                catch (Exception e)
                {
                    Log.error($"Exception injecting hat information for {hat.Name}: {e}");
                }
            }
        }
        private void injectDataWeapons(IAssetData asset)
        {
            var data = asset.AsDictionary<int, string>().Data;
            foreach (var weapon in Mod.instance.weapons)
            {
                try
                {
                    Log.verbose($"Injecting to weapons: {weapon.GetWeaponId()}: {weapon.GetWeaponInformation()}");
                    data.Add(weapon.GetWeaponId(), weapon.GetWeaponInformation());
                }
                catch (Exception e)
                {
                    Log.error($"Exception injecting weapon information for {weapon.Name}: {e}");
                }
            }
        }
        private void injectDataClothingInformation(IAssetData asset)
        {
            var data = asset.AsDictionary<int, string>().Data;
            foreach (var shirt in Mod.instance.shirts)
            {
                try
                {
                    Log.verbose($"Injecting to clothing information: {shirt.GetClothingId()}: {shirt.GetClothingInformation()}");
                    data.Add(shirt.GetClothingId(), shirt.GetClothingInformation());
                }
                catch (Exception e)
                {
                    Log.error($"Exception injecting clothing information for {shirt.Name}: {e}");
                }
            }
            foreach (var pants in Mod.instance.pantss)
            {
                try
                {
                    Log.verbose($"Injecting to clothing information: {pants.GetClothingId()}: {pants.GetClothingInformation()}");
                    data.Add(pants.GetClothingId(), pants.GetClothingInformation());
                }
                catch (Exception e)
                {
                    Log.error($"Exception injecting clothing information for {pants.Name}: {e}");
                }
            }
        }
        private void injectDataTailoringRecipes(IAssetData asset)
        {
            var data = asset.GetData<List<TailorItemRecipe>>();
            foreach (var recipe in Mod.instance.tailoring)
            {
                try
                {
                    Log.verbose($"Injecting to tailoring recipe: {recipe.ToGameData()}");
                    data.Add(recipe.ToGameData());
                }
                catch (Exception e)
                {
                    Log.error($"Exception injecting tailoring recipe: {e}");
                }
            }
        }
        private void injectDataBoots(IAssetData asset)
        {
            var data = asset.AsDictionary<int, string>().Data;
            foreach (var boots in Mod.instance.bootss)
            {
                try
                {
                    Log.verbose($"Injecting to boots: {boots.GetObjectId()}: {boots.GetBootsInformation()}");
                    data.Add(boots.GetObjectId(), boots.GetBootsInformation());
                }
                catch (Exception e)
                {
                    Log.error($"Exception injecting boots information for {boots.Name}: {e}");
                }
            }
        }
        private void injectMapsSpringobjects(IAssetData asset)
        {
            var oldTex = asset.AsImage().Data;
            asset.AsImage().ExtendImage( oldTex.Width, 4096 );
            //Texture2D newTex = new Texture2D(Game1.graphics.GraphicsDevice, oldTex.Width, Math.Max(oldTex.Height, 4096));
            //asset.ReplaceWith(newTex);
            //asset.AsImage().PatchImage(oldTex);

            foreach (var obj in Mod.instance.objects)
            {
                try
                {
                    Log.verbose($"Injecting {obj.Name} sprites @ {objectRect(obj.GetObjectId())}");
                    asset.AsImage().PatchExtendedTileSheet(obj.texture, null, objectRect(obj.GetObjectId()));
                    if (obj.IsColored)
                    {
                        Log.verbose($"Injecting {obj.Name} color sprites @ {objectRect(obj.GetObjectId() + 1)}");
                        asset.AsImage().PatchExtendedTileSheet( obj.textureColor, null, objectRect(obj.GetObjectId() + 1));
                    }

                    var rect = objectRect(obj.GetObjectId());
                    var target = TileSheetExtensions.GetAdjustedTileSheetTarget(asset.AssetName, rect);
                    int ts = target.TileSheet;
                    obj.tilesheet = asset.AssetName + (ts == 0 ? "" : (ts + 1).ToString());
                    obj.tilesheetX = rect.X;
                    obj.tilesheetY = target.Y;
                }
                catch (Exception e)
                {
                    Log.error($"Exception injecting sprite for {obj.Name}: {e}");
                }
            }

            foreach (var boots in Mod.instance.bootss)
            {
                try
                {
                    Log.verbose($"Injecting {boots.Name} sprites @ {objectRect(boots.GetObjectId())}");
                    asset.AsImage().PatchExtendedTileSheet(boots.texture, null, objectRect(boots.GetObjectId()));

                    var rect = objectRect(boots.GetObjectId());
                    var target = TileSheetExtensions.GetAdjustedTileSheetTarget(asset.AssetName, rect);
                    int ts = target.TileSheet;
                    boots.tilesheet = asset.AssetName + (ts == 0 ? "" : (ts + 1).ToString());
                    boots.tilesheetX = rect.X;
                    boots.tilesheetY = target.Y;
                }
                catch (Exception e)
                {
                    Log.error($"Exception injecting sprite for {boots.Name}: {e}");
                }
            }
        }
        private void injectTileSheetsCrops(IAssetData asset)
        {
            var oldTex = asset.AsImage().Data;
            Texture2D newTex = new Texture2D(Game1.graphics.GraphicsDevice, oldTex.Width, Math.Max(oldTex.Height, 4096));
            asset.ReplaceWith(newTex);
            asset.AsImage().PatchImage(oldTex);

            foreach (var crop in Mod.instance.crops)
            {
                try
                {
                    Log.verbose($"Injecting {crop.Name} crop images @ {cropRect(crop.GetCropSpriteIndex())}");
                    asset.AsImage().PatchExtendedTileSheet(crop.texture, null, cropRect(crop.GetCropSpriteIndex()));

                    var rect = cropRect(crop.GetCropSpriteIndex());
                    var target = TileSheetExtensions.GetAdjustedTileSheetTarget(asset.AssetName, rect);
                    int ts = target.TileSheet;
                    crop.tilesheet = asset.AssetName + (ts == 0 ? "" : (ts + 1).ToString());
                    crop.tilesheetX = rect.X;
                    crop.tilesheetY = target.Y;
                }
                catch (Exception e)
                {
                    Log.error($"Exception injecting crop sprite for {crop.Name}: {e}");
                }
            }
        }
        private void injectTileSheetsFruitTrees(IAssetData asset)
        {
            var oldTex = asset.AsImage().Data;
            Texture2D newTex = new Texture2D(Game1.graphics.GraphicsDevice, oldTex.Width, Math.Max(oldTex.Height, 4096));
            asset.ReplaceWith(newTex);
            asset.AsImage().PatchImage(oldTex);

            foreach (var fruitTree in Mod.instance.fruitTrees)
            {
                try
                {
                    Log.verbose($"Injecting {fruitTree.Name} fruit tree images @ {fruitTreeRect(fruitTree.GetFruitTreeIndex())}");
                    asset.AsImage().PatchExtendedTileSheet(fruitTree.texture, null, fruitTreeRect(fruitTree.GetFruitTreeIndex()));

                    var rect = fruitTreeRect(fruitTree.GetFruitTreeIndex());
                    var target = TileSheetExtensions.GetAdjustedTileSheetTarget(asset.AssetName, rect);
                    int ts = target.TileSheet;
                    fruitTree.tilesheet = asset.AssetName + (ts == 0 ? "" : (ts + 1).ToString());
                    fruitTree.tilesheetX = rect.X;
                    fruitTree.tilesheetY = target.Y;
                }
                catch (Exception e)
                {
                    Log.error($"Exception injecting fruit tree sprite for {fruitTree.Name}: {e}");
                }
            }
        }
        private void injectTileSheetsCraftables(IAssetData asset)
        {
            var oldTex = asset.AsImage().Data;
            Texture2D newTex = new Texture2D(Game1.graphics.GraphicsDevice, oldTex.Width, Math.Max(oldTex.Height, 4096));
            asset.ReplaceWith(newTex);
            asset.AsImage().PatchImage(oldTex);
            Log.trace($"Big craftables are now ({oldTex.Width}, {Math.Max(oldTex.Height, 4096)})");

            foreach (var big in Mod.instance.bigCraftables)
            {
                try
                {
                    Log.verbose($"Injecting {big.Name} sprites @ {bigCraftableRect(big.GetCraftableId())}");
                    asset.AsImage().PatchExtendedTileSheet(big.texture, null, bigCraftableRect(big.GetCraftableId()));
                    if (big.ReserveExtraIndexCount > 0)
                    {
                        for (int i = 0; i < big.ReserveExtraIndexCount; ++i)
                        {
                            Log.verbose($"Injecting {big.Name} reserved extra sprite {i + 1} @ {bigCraftableRect(big.GetCraftableId() + i + 1)}");
                            asset.AsImage().PatchExtendedTileSheet( big.extraTextures[i], null, bigCraftableRect(big.GetCraftableId() + i + 1));
                        }
                    }

                    var rect = bigCraftableRect(big.GetCraftableId());
                    int ts = TileSheetExtensions.GetAdjustedTileSheetTarget(asset.AssetName, rect).TileSheet;
                    big.tilesheet = asset.AssetName + (ts == 0 ? "" : (ts + 1).ToString());
                    big.tilesheetX = rect.X;
                    big.tilesheetY = rect.Y;
                }
                catch (Exception e)
                {
                    Log.error($"Exception injecting sprite for {big.Name}: {e}");
                }
            }
        }
        private void injectCharactersFarmerHats(IAssetData asset)
        {
            var oldTex = asset.AsImage().Data;
            Texture2D newTex = new Texture2D(Game1.graphics.GraphicsDevice, oldTex.Width, Math.Max(oldTex.Height, 4096));
            asset.ReplaceWith(newTex);
            asset.AsImage().PatchImage(oldTex);
            Log.trace($"Hats are now ({oldTex.Width}, {Math.Max(oldTex.Height, 4096)})");

            foreach (var hat in Mod.instance.hats)
            {
                try
                {
                    Log.verbose($"Injecting {hat.Name} sprites @ {hatRect(hat.GetHatId())}");
                    asset.AsImage().PatchExtendedTileSheet( hat.texture, null, hatRect(hat.GetHatId()));

                    var rect = hatRect(hat.GetHatId());
                    var target = TileSheetExtensions.GetAdjustedTileSheetTarget(asset.AssetName, rect);
                    int ts = target.TileSheet;
                    hat.tilesheet = asset.AssetName + (ts == 0 ? "" : (ts + 1).ToString());
                    hat.tilesheetX = rect.X;
                    hat.tilesheetY = target.Y;
                }
                catch (Exception e)
                {
                    Log.error($"Exception injecting sprite for {hat.Name}: {e}");
                }
            }
        }
        private void injectTileSheetsWeapons(IAssetData asset)
        {
            var oldTex = asset.AsImage().Data;
            Texture2D newTex = new Texture2D(Game1.graphics.GraphicsDevice, oldTex.Width, Math.Max(oldTex.Height, 4096));
            asset.ReplaceWith(newTex);
            asset.AsImage().PatchImage(oldTex);
            Log.trace($"Weapons are now ({oldTex.Width}, {Math.Max(oldTex.Height, 4096)})");

            foreach (var weapon in Mod.instance.weapons)
            {
                try
                {
                    Log.verbose($"Injecting {weapon.Name} sprites @ {weaponRect(weapon.GetWeaponId())}");
                    asset.AsImage().PatchImage(weapon.texture, null, weaponRect(weapon.GetWeaponId()));

                    var rect = weaponRect(weapon.GetWeaponId());
                    int ts = 0;// TileSheetExtensions.GetAdjustedTileSheetTarget(asset.AssetName, rect).TileSheet;
                    weapon.tilesheet = asset.AssetName + (ts == 0 ? "" : (ts + 1).ToString());
                    weapon.tilesheetX = rect.X;
                    weapon.tilesheetY = rect.Y;
                }
                catch (Exception e)
                {
                    Log.error($"Exception injecting sprite for {weapon.Name}: {e}");
                }
            }
        }
        private void injectCharactersFarmerShirts(IAssetData asset)
        {
            var oldTex = asset.AsImage().Data;
            asset.AsImage().ExtendImage( oldTex.Width, 4096 );
            Log.trace($"Shirts are now ({oldTex.Width}, {Math.Max(oldTex.Height, 4096)})");

            foreach (var shirt in Mod.instance.shirts)
            {
                try
                {
                    string rects = shirtRectPlain(shirt.GetMaleIndex()).ToString();
                    if (shirt.Dyeable)
                        rects += ", " + shirtRectDye(shirt.GetMaleIndex()).ToString();
                    if (shirt.HasFemaleVariant)
                    {
                        rects += ", " + shirtRectPlain(shirt.GetFemaleIndex()).ToString();
                        if (shirt.Dyeable)
                            rects += ", " + shirtRectDye(shirt.GetFemaleIndex()).ToString();
                    }

                    Log.verbose($"Injecting {shirt.Name} sprites @ {rects}");
                    asset.AsImage().PatchExtendedTileSheet(shirt.textureMale, null, shirtRectPlain(shirt.GetMaleIndex()));
                    if (shirt.Dyeable)
                        asset.AsImage().PatchExtendedTileSheet(shirt.textureMaleColor, null, shirtRectDye(shirt.GetMaleIndex()));
                    if (shirt.HasFemaleVariant)
                    {
                        asset.AsImage().PatchExtendedTileSheet(shirt.textureFemale, null, shirtRectPlain(shirt.GetFemaleIndex()));
                        if (shirt.Dyeable)
                            asset.AsImage().PatchExtendedTileSheet(shirt.textureFemaleColor, null, shirtRectDye(shirt.GetFemaleIndex()));
                    }
                }
                catch (Exception e)
                {
                    Log.error($"Exception injecting sprite for {shirt.Name}: {e}");
                }
            }
        }
        private void injectCharactersFarmerPants(IAssetData asset)
        {
            var oldTex = asset.AsImage().Data;
            Texture2D newTex = new Texture2D(Game1.graphics.GraphicsDevice, oldTex.Width, Math.Max(oldTex.Height, 4096));
            asset.ReplaceWith(newTex);
            asset.AsImage().PatchImage(oldTex);
            Log.trace($"Pants are now ({oldTex.Width}, {Math.Max(oldTex.Height, 4096)})");

            foreach (var pants in Mod.instance.pantss)
            {
                try
                {
                    Log.verbose($"Injecting {pants.Name} sprites @ {pantsRect(pants.GetTextureIndex())}");
                    asset.AsImage().PatchExtendedTileSheet(pants.texture, null, pantsRect(pants.GetTextureIndex()));
                }
                catch (Exception e)
                {
                    Log.error($"Exception injecting sprite for {pants.Name}: {e}");
                }
            }
        }
        private void injectCharactersFarmerShoeColors(IAssetData asset)
        {
            var oldTex = asset.AsImage().Data;
            Texture2D newTex = new Texture2D(Game1.graphics.GraphicsDevice, oldTex.Width, Math.Max(oldTex.Height, 4096));
            asset.ReplaceWith(newTex);
            asset.AsImage().PatchImage(oldTex);
            Log.trace($"Boots are now ({oldTex.Width}, {Math.Max(oldTex.Height, 4096)})");

            foreach (var boots in Mod.instance.bootss)
            {
                try
                {
                    Log.verbose($"Injecting {boots.Name} sprites @ {bootsRect(boots.GetTextureIndex())}");
                    asset.AsImage().PatchExtendedTileSheet(boots.textureColor, null, bootsRect(boots.GetTextureIndex()));
                }
                catch (Exception e)
                {
                    Log.error($"Exception injecting sprite for {boots.Name}: {e}");
                }
            }
        }
        internal static Rectangle objectRect(int index)
        {
            return new Rectangle(index % 24 * 16, index / 24 * 16, 16, 16);
        }
        internal static Rectangle cropRect(int index)
        {
            return new Rectangle(index % 2 * 128, index / 2 * 32, 128, 32);
        }
        internal static Rectangle fruitTreeRect(int index)
        {
            return new Rectangle(0, index * 80, 432, 80);
        }
        internal static Rectangle bigCraftableRect(int index)
        {
            return new Rectangle(index % 8 * 16, index / 8 * 32, 16, 32);
        }
        internal static Rectangle hatRect(int index)
        {
            return new Rectangle(index % 12 * 20, index / 12 * 80, 20, 80);
        }
        internal static Rectangle weaponRect(int index)
        {
            return new Rectangle(index % 8 * 16, index / 8 * 16, 16, 16);
        }
        internal static Rectangle shirtRectPlain(int index)
        {
            return new Rectangle(index % 16 * 8, index / 16 * 32, 8, 32);
        }
        internal static Rectangle shirtRectDye(int index)
        {
            var rect = shirtRectPlain(index);
            rect.X += 16 * 8;
            return rect;
        }
        internal static Rectangle pantsRect(int index)
        {
            return new Rectangle(index % 10 * 192, index / 10 * 688, 192, 688);
        }
        internal static Rectangle bootsRect(int index)
        {
            return new Rectangle(0, index, 4, 1);
        }

        public bool CanLoad<T>( IAssetInfo asset )
        {
            foreach ( var fence in Mod.instance.fences )
            {
                if ( asset.AssetNameEquals( "LooseSprites\\Fence" + fence.correspondingObject?.GetObjectId() ) )
                    return true;
            }
            return false;
        }

        public T Load<T>( IAssetInfo asset )
        {
            foreach ( var fence in Mod.instance.fences )
            {
                if ( asset.AssetNameEquals( "LooseSprites\\Fence" + fence.correspondingObject?.GetObjectId() ) )
                    return ( T ) ( object ) fence.texture;
            }
            return default( T );
        }
    }
}
