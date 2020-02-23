using GetGlam.Framework.DataModels;
using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;
using System.IO;

namespace GetGlam.Framework
{
    /// <summary>Class that saves and loads character layouts</summary>
    public class CharacterLoader
    {
        //Instance of ModEntry
        private ModEntry Entry;

        //Instance of ContentPackHelper
        private ContentPackHelper PackHelper;

        //Instance of DresserHandler
        private DresserHandler Dresser;

        //List that has all the players favorites
        public List<FavoriteModel> Favorites = new List<FavoriteModel>();

        /// <summary>CharacterLoader's Constructor</summary>
        /// <param name="entry">The instance of ModEntry</param>
        /// <param name="packHelper">The instance of ContentPackHelper</param>
        /// <param name="dresser">The instance of DresserHandler</param>
        public CharacterLoader(ModEntry entry, ContentPackHelper packHelper, DresserHandler dresser)
        {
            //Set the fields to the instances
            Entry = entry;
            PackHelper = packHelper;
            Dresser = dresser;
        }

        /// <summary>Save the characters layout to a json file</summary>
        /// <param name="isMale">Whether the player is male</param>
        /// <param name="baseIndex">The base index</param>
        /// <param name="skinIndex">The skin index</param>
        /// <param name="hairIndex">The hair index</param>
        /// <param name="faceIndex">The face index</param>
        /// <param name="noseIndex">The nose index</param>
        /// <param name="shoesIndex">The shoes index</param>
        /// <param name="accessoryIndex">The accessory index</param>
        /// <param name="dresserIndex">The dresser index</param>
        /// <param name="isBald">Whether the player is bald</param>
        public void SaveCharacterLayout(bool isMale, int baseIndex, int skinIndex, int hairIndex, int faceIndex, int noseIndex, int shoesIndex, int accessoryIndex, int dresserIndex, bool isBald)
        {
            //Save all the current player index to the ConfigModel
            ConfigModel currentPlayerStyle = new ConfigModel();
            currentPlayerStyle.IsDefault = false;
            currentPlayerStyle.IsMale = isMale;
            currentPlayerStyle.BaseIndex = baseIndex;
            currentPlayerStyle.SkinIndex = skinIndex;
            currentPlayerStyle.HairIndex = hairIndex;
            currentPlayerStyle.FaceIndex = faceIndex;
            currentPlayerStyle.NoseIndex = noseIndex;
            currentPlayerStyle.ShoesIndex = shoesIndex;
            currentPlayerStyle.AccessoryIndex = accessoryIndex;
            currentPlayerStyle.DresserIndex = dresserIndex;
            currentPlayerStyle.IsBald = isBald;
            currentPlayerStyle.HairColor = Game1.player.hairstyleColor.Get();
            currentPlayerStyle.EyeColor = Game1.player.newEyeColor.Get();
            currentPlayerStyle.Favorites = Favorites;

            //Write the favorite model to a json
            Entry.Helper.Data.WriteJsonFile<ConfigModel>(Path.Combine("Saves", $"{Constants.SaveFolderName}_current.json"), currentPlayerStyle);
        }

        /// <summary>Loads the character layout from a json file</summary>
        /// <param name="menu">The instance of Glam Menu used ti update indexes</param>
        public void LoadCharacterLayout(GlamMenu menu)
        {
            //Read the config
            ConfigModel currentPlayerStyle = Entry.Helper.Data.ReadJsonFile<ConfigModel>(Path.Combine("Saves", $"{Constants.SaveFolderName}_current.json"));

            //Don't try to load if it doesn't find the json
            if (currentPlayerStyle is null)
                return;

            //Update the dresser
            if (Dresser.Texture.Height == 32)
                Dresser.TextureSourceRect.Y = 0;
            else
                Dresser.TextureSourceRect.Y = currentPlayerStyle.DresserIndex.Equals(1) ? 0 : currentPlayerStyle.DresserIndex * 32 - 32;

            Dresser.SetDresserTileSheetPoint(currentPlayerStyle.DresserIndex);

            if (!currentPlayerStyle.IsDefault)
            {
                menu.UpdateIndexes(currentPlayerStyle.BaseIndex, currentPlayerStyle.FaceIndex, currentPlayerStyle.NoseIndex, currentPlayerStyle.ShoesIndex, currentPlayerStyle.IsBald, currentPlayerStyle.DresserIndex);

                //Set the players gender, hair, accessory and skin
                Game1.player.changeGender(currentPlayerStyle.IsMale);
                Game1.player.hair.Set(currentPlayerStyle.HairIndex);
                Game1.player.accessory.Set(currentPlayerStyle.AccessoryIndex);
                Game1.player.FarmerRenderer.recolorSkin(currentPlayerStyle.SkinIndex);

                //Change the color of the hair and the eyes
                Game1.player.newEyeColor.Set(currentPlayerStyle.EyeColor);
                Game1.player.FarmerRenderer.recolorEyes(currentPlayerStyle.EyeColor);
                Game1.player.hairstyleColor.Set(currentPlayerStyle.HairColor);
            }

            //Add the favorites to the favorites list
            if (currentPlayerStyle.Favorites == null || currentPlayerStyle.Favorites.Count == 0)
            {
                FavoriteModel model = new FavoriteModel();
                for (int i = 0; i < 40; i++)
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

            //Lastly, change the base, THIS NEEDS TO BE LAST OR ELSE IT WON'T LOAD THE PLAYER STYLE
            if (!currentPlayerStyle.IsDefault)
                PackHelper.ChangePlayerBase(Game1.player.isMale, currentPlayerStyle.BaseIndex, currentPlayerStyle.FaceIndex, currentPlayerStyle.NoseIndex, currentPlayerStyle.ShoesIndex, currentPlayerStyle.IsBald);
        }

        /// <summary>Loads the farmers for the Load Save Menu</summary>
        /// <param name="farmer">The current farmer being patched</param>
        /// <param name="configModel">The config model for the current farmer</param>
        public void LoadFarmersLayoutForLoadMenu(Farmer farmer, ConfigModel configModel)
        {
            //Don't want to edit if it's a new config
            if (configModel.IsDefault)
                return;

            //Change gender, hair, accessory, and skin
            farmer.changeGender(configModel.IsMale);
            farmer.hair.Set(configModel.HairIndex);
            farmer.accessory.Set(configModel.AccessoryIndex);
            farmer.FarmerRenderer.recolorSkin(configModel.SkinIndex);
            farmer.FarmerRenderer.recolorEyes(configModel.EyeColor);
            farmer.changeHairColor(configModel.HairColor);

            //Change the base
            PackHelper.ChangePlayerBase(configModel.IsMale, configModel.BaseIndex, configModel.FaceIndex, configModel.NoseIndex, configModel.ShoesIndex, configModel.IsBald);
            //Entry.Helper.Reflection.GetField<Texture2D>(farmer.FarmerRenderer, "baseTexture").SetValue(PackHelper.LoadPlayerBase());
        }

        /// <summary>Loads a favorite from the favorite list</summary>
        /// <param name="whichPosition">The position in the List</param>
        /// <param name="menu">The instance of GlamMenu</param>
        /// <param name="wasFavoriteApplied">Whether the favorite was applied</param>
        public void LoadFavorite(int whichPosition, GlamMenu menu, bool wasFavoriteApplied)
        {
            //Change gender, hair, accessories, and skin
            Game1.player.changeGender(Favorites[whichPosition].IsMale);
            Game1.player.hair.Set(Favorites[whichPosition].HairIndex);
            Game1.player.accessory.Set(Favorites[whichPosition].AccessoryIndex);
            Game1.player.skin.Set(Favorites[whichPosition].SkinIndex);

            //Change the color of the hair and the eyes
            Game1.player.newEyeColor.Set(Favorites[whichPosition].EyeColor);
            Game1.player.FarmerRenderer.recolorEyes(Favorites[whichPosition].EyeColor);
            Game1.player.hairstyleColor.Set(Favorites[whichPosition].HairColor);

            //Chnage the player base to the index in the favorites
            PackHelper.ChangePlayerBase(
                Favorites[whichPosition].IsMale,
                Favorites[whichPosition].BaseIndex,
                Favorites[whichPosition].FaceIndex,
                Favorites[whichPosition].NoseIndex,
                Favorites[whichPosition].ShoeIndex,
                Favorites[whichPosition].IsBald
            );

            //If the favorite was applied then update the GlamMenu
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

        /// <summary>Saves a favorite to the favorite list</summary>
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

            //Check if there is an emtpy spot in the favorites list
            for (int i = 0; i < 40; i++)
            {
                if (Favorites[i].IsDefault)
                {
                    Entry.Monitor.Log("Adding Favorite to list.", LogLevel.Trace);
                    Favorites[i] = favModel;
                    return;
                }
                else if (i == 39 && !Favorites[i].IsDefault)
                {
                    Entry.Monitor.Log("Reached the maximum amount of favorites, try deleting some.", LogLevel.Warn);
                    return;
                }
            }
        }
    }
}
