/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MartyrPher/GetGlam
**
*************************************************/

using GetGlam.Framework.DataModels;
using GetGlam.Framework.Menus;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;
using System.IO;

namespace GetGlam.Framework
{
    /// <summary>
    /// Class that saves and loads character layouts.
    /// </summary>
    public class CharacterLoader
    {
        // Instance of ModEntry
        private ModEntry Entry;

        // Instance of PlayerChanger
        private PlayerChanger PlayerChanger;

        // Instance of DresserHandler
        private DresserHandler Dresser;

        // List that has all the players favorites
        public List<FavoriteModel> Favorites = new List<FavoriteModel>();

        // Max amount of favorites
        private int MaxFavorites = 40;

        /// <summary>
        /// CharacterLoader's Constructor.
        /// </summary>
        /// <param name="entry">The instance of ModEntry</param>
        /// <param name="changer">The instance of PlayerChanger</param>
        /// <param name="dresser">The instance of DresserHandler</param>
        public CharacterLoader(ModEntry entry, PlayerChanger changer, DresserHandler dresser)
        {
            // Set the fields to the instances
            Entry = entry;
            PlayerChanger = changer;
            Dresser = dresser;
        }

        /// <summary>
        /// Save the characters layout to a json file.
        /// </summary>
        /// <param name="baseIndex">The base index</param>
        /// <param name="faceIndex">The face index</param>
        /// <param name="noseIndex">The nose index</param>
        /// <param name="shoesIndex">The shoes index</param>
        /// <param name="dresserIndex">The dresser index</param>
        /// <param name="isBald">Whether the player is bald</param>
        public void SaveCharacterLayout(int baseIndex, int faceIndex, int noseIndex, int shoesIndex, int dresserIndex, bool isBald)
        {
            // Save all the current player index to the ConfigModel
            ConfigModel currentPlayerStyle = new ConfigModel();
            currentPlayerStyle.IsDefault = false;
            currentPlayerStyle.IsMale = Game1.player.isMale;
            currentPlayerStyle.BaseIndex = baseIndex;
            currentPlayerStyle.SkinIndex = Game1.player.skin.Value;
            currentPlayerStyle.HairIndex = Game1.player.hair.Value;
            currentPlayerStyle.FaceIndex = faceIndex;
            currentPlayerStyle.NoseIndex = noseIndex;
            currentPlayerStyle.ShoesIndex = shoesIndex;
            currentPlayerStyle.AccessoryIndex = Game1.player.accessory.Value;
            currentPlayerStyle.DresserIndex = dresserIndex;
            currentPlayerStyle.IsBald = isBald;
            currentPlayerStyle.HairColor = Game1.player.hairstyleColor.Get();
            currentPlayerStyle.EyeColor = Game1.player.newEyeColor.Get();
            currentPlayerStyle.Favorites = Favorites;

            // Write the config model to a json
            Entry.Helper.Data.WriteJsonFile<ConfigModel>(Path.Combine("Saves", $"{Constants.SaveFolderName}_current.json"), currentPlayerStyle);
        }

        /// <summary>
        /// Loads the character layout from a json file.
        /// </summary>
        /// <param name="menu">The instance of Glam Menu used to update indexes</param>
        public void LoadCharacterLayout(GlamMenu menu)
        {
            // Read the config
            ConfigModel currentPlayerStyle = Entry.Helper.Data.ReadJsonFile<ConfigModel>(Path.Combine("Saves", $"{Constants.SaveFolderName}_current.json"));

            // Don't try to load if it doesn't find the json
            if (currentPlayerStyle is null)
                return;

            // Update the dresser
            UpdateDresser(currentPlayerStyle.DresserIndex);

            if (!currentPlayerStyle.IsDefault)
            {
                menu.UpdateIndexes(currentPlayerStyle.BaseIndex, currentPlayerStyle.FaceIndex, currentPlayerStyle.NoseIndex, currentPlayerStyle.ShoesIndex, currentPlayerStyle.IsBald, currentPlayerStyle.DresserIndex);
                SetPlayerStyle(currentPlayerStyle);
            }

            //Add the favorites to the favorites list
            AddFavoritesToFavoriteList(currentPlayerStyle);

            //Lastly, change the base, THIS NEEDS TO BE LAST OR ELSE IT WON'T LOAD THE PLAYER STYLE
            if (!currentPlayerStyle.IsDefault)
                PlayerChanger.ChangePlayerBase(Game1.player.isMale, currentPlayerStyle.BaseIndex, currentPlayerStyle.FaceIndex, currentPlayerStyle.NoseIndex, currentPlayerStyle.ShoesIndex, currentPlayerStyle.IsBald);
        }

        /// <summary>
        /// Sets the players style.
        /// </summary>
        /// <param name="currentPlayerStyle">The players current style.</param>
        private void SetPlayerStyle(ConfigModel currentPlayerStyle)
        {
            SetPlayerGender(currentPlayerStyle.IsMale);

            SetPlayerHairStyle(currentPlayerStyle.HairIndex);

            SetPlayerAccessory(currentPlayerStyle.AccessoryIndex);

            SetPlayerSkinColor(currentPlayerStyle.SkinIndex);

            SetPlayerEyeColor(currentPlayerStyle.EyeColor);

            SetPlayerHairColor(currentPlayerStyle.HairColor);
        }

        /// <summary>
        /// Adds Favorites to the Favorites List.
        /// </summary>
        /// <param name="currentPlayerStyle">Players current style.</param>
        private void AddFavoritesToFavoriteList(ConfigModel currentPlayerStyle)
        {
            if (currentPlayerStyle.Favorites == null || currentPlayerStyle.Favorites.Count == 0)
            {
                FavoriteModel model = new FavoriteModel();
                for (int i = 0; i < MaxFavorites; i++)
                {
                    currentPlayerStyle.Favorites = new List<FavoriteModel>();
                    currentPlayerStyle.Favorites.Add(model);
                    Favorites.Add(model);
                }
            }
            else
            {
                foreach (FavoriteModel model in currentPlayerStyle.Favorites)
                    Favorites.Add(model);
            }
        }

        /// <summary>
        /// Loads the farmers for the Load Save Menu.
        /// </summary>
        /// <param name="farmer">The current farmer being patched</param>
        /// <param name="configModel">The config model for the current farmer</param>
        public void LoadFarmersLayoutForLoadMenu(Farmer farmer, ConfigModel configModel)
        {
            // Don't want to edit if it's a new config
            if (configModel.IsDefault)
                return;

            // Set a particular farmer for the loading screen
            SetLoadScreenFarmerStyle(farmer, configModel);

            // Change the base
            PlayerChanger.ChangePlayerBase(configModel.IsMale, configModel.BaseIndex, configModel.FaceIndex, configModel.NoseIndex, configModel.ShoesIndex, configModel.IsBald);
        }

        /// <summary>
        /// Sets a specfic farmers style.
        /// </summary>
        /// <param name="farmer">The current farmer.</param>
        /// <param name="configModel">The config model for the farmer.</param>
        private void SetLoadScreenFarmerStyle(Farmer farmer, ConfigModel configModel)
        {
            farmer.changeGender(configModel.IsMale);
            farmer.hair.Set(configModel.HairIndex);
            farmer.accessory.Set(configModel.AccessoryIndex);
            farmer.FarmerRenderer.recolorSkin(configModel.SkinIndex, true);
            farmer.FarmerRenderer.recolorEyes(configModel.EyeColor);
            farmer.changeHairColor(configModel.HairColor);
        }

        /// <summary>
        /// Loads a favorite from the favorite list.
        /// </summary>
        /// <param name="whichPosition">The position in the List</param>
        /// <param name="menu">The instance of GlamMenu</param>
        /// <param name="wasFavoriteApplied">Whether the favorite was applied</param>
        public void LoadFavorite(int whichPosition, GlamMenu menu, bool wasFavoriteApplied)
        {
            SetPlayerGender(Favorites[whichPosition].IsMale);

            SetPlayerHairStyle(Favorites[whichPosition].HairIndex);

            SetPlayerAccessory(Favorites[whichPosition].AccessoryIndex);

            SetPlayerSkinColor(Favorites[whichPosition].SkinIndex);

            SetPlayerEyeColor(Favorites[whichPosition].EyeColor);

            SetPlayerHairColor(Favorites[whichPosition].HairColor);

            // Change the player base to the index in the favorites
            PlayerChanger.ChangePlayerBase(
                Favorites[whichPosition].IsMale,
                Favorites[whichPosition].BaseIndex,
                Favorites[whichPosition].FaceIndex,
                Favorites[whichPosition].NoseIndex,
                Favorites[whichPosition].ShoeIndex,
                Favorites[whichPosition].IsBald
            );

            // If the favorite was applied then update the GlamMenu
            if (wasFavoriteApplied)
            {
                menu.UpdateIndexes(
                    Favorites[whichPosition].BaseIndex,
                    Favorites[whichPosition].FaceIndex,
                    Favorites[whichPosition].NoseIndex,
                    Favorites[whichPosition].ShoeIndex,
                    Favorites[whichPosition].IsBald
                );
            }
        }

        /// <summary>
        /// Saves a favorite to the favorite list.
        /// </summary>
        /// <param name="isMale">Whether the player is male</param>
        /// <param name="baseIndex">The base index</param>
        /// <param name="skinIndex">The skin index</param>
        /// <param name="hairIndex">The hair index</param>
        /// <param name="faceIndex">The face index</param>
        /// <param name="noseIndex">The nose index</param>
        /// <param name="shoesIndex">The shoes index</param>
        /// <param name="accessoryIndex">The accessory index</param>
        /// <param name="isBald">Whether the player is bald</param>
        public void SaveFavoriteToList(bool isMale, int baseIndex, int skinIndex, int hairIndex, int faceIndex, int noseIndex, int shoesIndex, int accessoryIndex, bool isBald)
        {
            //Set all the stuff
            FavoriteModel favModel = new FavoriteModel();
            favModel.IsDefault = false;
            favModel.IsMale = isMale;
            favModel.BaseIndex = baseIndex;
            favModel.SkinIndex = skinIndex;
            favModel.HairIndex = hairIndex;
            favModel.FaceIndex = faceIndex;
            favModel.NoseIndex = noseIndex;
            favModel.ShoeIndex = shoesIndex;
            favModel.AccessoryIndex = accessoryIndex;
            favModel.IsBald = isBald;

            favModel.EyeColor = Game1.player.newEyeColor.Value;
            favModel.HairColor = Game1.player.hairstyleColor.Value;

            // Check if there is an emtpy spot in the favorites list
            CheckForEmptyFavoriteSpot(favModel);
        }

        /// <summary>
        /// Updates Dresser SourceRect and Sets TileSheetPoint.
        /// </summary>
        /// <param name="dresserIndex">Index of the Dresser.</param>
        private void UpdateDresser(int dresserIndex)
        {
            if (Dresser.PackHelper.DresserTextureHeight == 32)
                Dresser.TextureSourceRect.Y = 0;
            else
                Dresser.TextureSourceRect.Y = dresserIndex * 32 - 32;

            Dresser.SetDresserTileSheetPoint(dresserIndex);
            Dresser.UpdateDresserInFarmHouse();
        }

        /// <summary>
        /// Sets the players gender.
        /// </summary>
        /// <param name="gender"></param>
        private void SetPlayerGender(bool gender)
        {
            Game1.player.changeGender(gender);
        }

        /// <summary>
        /// Sets the players hairstyle.
        /// </summary>
        /// <param name="hairIndex">Index of the hairstyle.</param>
        public void SetPlayerHairStyle(int hairIndex)
        {
            Game1.player.hair.Set(hairIndex);
        }

        /// <summary>
        /// Sets the players accessory.
        /// </summary>
        /// <param name="accessoryIndex">The accessory index.</param>
        private void SetPlayerAccessory(int accessoryIndex)
        {
            Game1.player.accessory.Set(accessoryIndex);
        }

        /// <summary>
        /// Set the players skin color.
        /// </summary>
        /// <param name="skinIndex">The index of the skin.</param>
        private void SetPlayerSkinColor(int skinIndex)
        {
            Game1.player.changeSkinColor(skinIndex, true);
        }

        /// <summary>
        /// Sets the players eye color.
        /// </summary>
        /// <param name="eyeColor">The current eye color.</param>
        private void SetPlayerEyeColor(Color eyeColor)
        {
            Game1.player.newEyeColor.Set(eyeColor);
            Game1.player.FarmerRenderer.recolorEyes(eyeColor);
        }

        /// <summary>
        /// Sets the players hair color.
        /// </summary>
        /// <param name="hairColor">The color of the hair.</param>
        private void SetPlayerHairColor(Color hairColor) 
        {
            Game1.player.hairstyleColor.Set(hairColor);
        }

        /// <summary>
        /// Checks the favorites for an empty spot to save the favorite.
        /// </summary>
        /// <param name="favoriteModel">The favorite being saved.</param>
        private void CheckForEmptyFavoriteSpot(FavoriteModel favoriteModel)
        {
            for (int currentFavoriteSpot = 0; currentFavoriteSpot < MaxFavorites; currentFavoriteSpot++)
            {
                // If the spot is a default
                if (Favorites[currentFavoriteSpot].IsDefault)
                {
                    // Add the favorite
                    Entry.Monitor.Log("Adding Favorite to list.", LogLevel.Trace);
                    Favorites[currentFavoriteSpot] = favoriteModel;
                    return;
                }
                else if (currentFavoriteSpot == 39 && !Favorites[currentFavoriteSpot].IsDefault)
                {
                    // Say favorites are full, then break out.
                    Entry.Monitor.Log("Reached the maximum amount of favorites, try deleting some.", LogLevel.Warn);
                    return;
                }
            }
        }
    }
}
