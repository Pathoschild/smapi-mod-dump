/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MartyrPher/GetGlam
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

namespace GetGlam.Framework
{
    public class PlayerChanger
    {
        // Instance of ModEntry
        private ModEntry Entry;

        // Instance of ContentPackHelper
        private ContentPackHelper PackHelper;

        // Whether the player is male
        private bool IsMale = false;

        // Whether the player is bald
        public bool IsBald = false;

        // The index of the face
        private int FaceIndex = 0;

        // The index of the nose
        private int NoseIndex = 0;

        // The index of the base
        private int BaseIndex = 0;

        // The index of the shoe
        private int ShoeIndex = 0;

        // An array of tweaks for the male shoe sprite heights
        private int[] MaleShoeSpriteHeights = new int[21] { 11, 16, 15, 14, 13, 16, 16, 14, 16, 12, 14, 14, 15, 15, 16, 13, 15, 16, 16, 16, 15 };

        // An array of tweaks for the female shoe sprite heights
        private int[] FemaleShoeSpriteHeights = new int[21] { 15, 16, 14, 13, 12, 16, 16, 15, 16, 10, 13, 13, 13, 14, 14, 11, 14, 14, 14, 16, 13 };

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="entry">Instance of ModEntry</param>
        /// <param name="packHelper">Instance of ContentPackHelper</param>
        public PlayerChanger(ModEntry entry, ContentPackHelper packHelper) 
        {
            Entry = entry;
            PackHelper = packHelper;
        }

        /// <summary>
        /// Loads the Player base texture.
        /// </summary>
        /// <returns>The new player base texture</returns>
        public Texture2D LoadPlayerBase()
        {
            Texture2D playerBase = LoadAndSetPlayerBaseTexture();

            if (Entry.Config.PatchFaceNose)
                SetPlayerFaceAndNose(playerBase);

            if (Entry.Config.PatchShoes)
                PatchPlayerShoes(playerBase);

            //Return the new player base
            return playerBase;
        }

        /// <summary>
        /// Loads textures for the player base.
        /// </summary>
        /// <returns>The new player base texture</returns>
        private Texture2D LoadAndSetPlayerBaseTexture()
        {
            // Create a new texture
            Texture2D playerBase;

            // Wrap in a try...catch in case they removed the base from the folder
            try
            {
                if (BaseIndex != 0)
                    playerBase = SetCustomBase();
                else
                    playerBase = SetDefaultBase();
            }
            catch
            {
                // Set to default base if the above loading fails
                Entry.Monitor.Log("There was a problem loading the base file, setting the base to default.", LogLevel.Warn);
                playerBase = SetDefaultBase();
            }

            return playerBase;
        }

        /// <summary>
        /// Sets the player base texture to a custom texture.
        /// </summary>
        /// <returns>The loaded custom base texture.</returns>
        private Texture2D SetCustomBase()
        {
            Texture2D playerBase;

            if (!IsBald)
                playerBase = IsMale ? PackHelper.MaleBaseTextureList[BaseIndex - 1] : PackHelper.FemaleBaseTextureList[BaseIndex - 1];
            else
                playerBase = IsMale ? PackHelper.MaleBaseBaldTextureList[BaseIndex - 1] : PackHelper.FemaleBaseBaldTextureList[BaseIndex - 1];
 
            return playerBase;
        }

        /// <summary>
        /// Sets the player base texture to the default texture.
        /// </summary>
        /// <returns>The loaded default base texture.</returns>
        private Texture2D SetDefaultBase()
        {
            Texture2D playerBase;
            if (IsBald)
                playerBase = Entry.Helper.Content.Load<Texture2D>(IsMale ? "Characters\\Farmer\\farmer_base_bald" : "Characters\\Farmer\\farmer_girl_base_bald", ContentSource.GameContent);
            else
                playerBase = Entry.Helper.Content.Load<Texture2D>(IsMale ? "Characters\\Farmer\\farmer_base" : "Characters\\Farmer\\farmer_girl_base", ContentSource.GameContent);

            BaseIndex = 0;
            return playerBase;
        }

        /// <summary>
        /// Sets the players nose and face texture and patches it the base texture.
        /// </summary>
        /// <param name="playerBase">The current player base texture.</param>
        private void SetPlayerFaceAndNose(Texture2D playerBase)
        {
            // Try...catch for the face and nose if the key was not found in the dictionary
            try
            {
                Texture2D faceAndNoseTexture = null;
                // The base is 0 then load the default textures
                if (BaseIndex.Equals(0))
                    faceAndNoseTexture = SetDefaultFaceAndNose();
                else if (IsMaleAndNonZeroList(playerBase))
                    faceAndNoseTexture = SetCustomMaleFaceAndNose(playerBase);
                else if (IsFemaleAndNonZeroList(playerBase))
                    faceAndNoseTexture = SetCustomFemaleFaceAndNose(playerBase);

                PatchBaseTexture(playerBase, faceAndNoseTexture, 0, 0);
            }
            catch
            {
                // Set to default face and nose
                NoseIndex = 0;
                FaceIndex = 0;
            }
        }

        /// <summary>
        /// Sets the face and nose texture to the selected face nose index.
        /// </summary>
        /// <returns>The selected index nose face texture.</returns>
        private Texture2D SetDefaultFaceAndNose()
        {
            Texture2D faceAndNoseTexture;
            if (!IsBald)
                faceAndNoseTexture = Entry.Helper.Content.Load<Texture2D>(IsMale ? $"assets/male_face{FaceIndex}_nose{NoseIndex}.png" : $"assets/female_face{FaceIndex}_nose{NoseIndex}.png");
            else
                faceAndNoseTexture = Entry.Helper.Content.Load<Texture2D>(IsMale ? $"assets/male_face{FaceIndex}_nose{NoseIndex}_bald.png" : $"assets/female_face{FaceIndex}_nose{NoseIndex}_bald.png");
            return faceAndNoseTexture;
        }

        /// <summary>
        /// Whether the player is male and Face and Nose list are Non-Zero.
        /// </summary>
        /// <param name="playerBase">Current Player Base</param>
        /// <returns>If the player is male and lists are non-zero</returns>
        private bool IsMaleAndNonZeroList(Texture2D playerBase)
        {
            return IsMale && PackHelper.MaleBaseFaceNoseCount[playerBase][0] > 0 && PackHelper.MaleBaseFaceNoseCount[playerBase][1] > 0;
        }

        /// <summary>
        /// Sets a custom face nose option from the male face nose dictionary.
        /// </summary>
        /// <param name="playerBase">Current player base</param>
        /// <returns>The custom face nose texture</returns>
        private Texture2D SetCustomMaleFaceAndNose(Texture2D playerBase)
        {
            return PackHelper.MaleFaceAndNoseTextureDict[playerBase][$"male_face{FaceIndex}_nose{NoseIndex}.png"];
        }

        /// <summary>
        /// Whether the player is female and Face and Nose list are Non-Zero.
        /// </summary>
        /// <param name="playerBase">Current Player Base</param>
        /// <returns>If the player is female and lists are non-zero</returns>
        private bool IsFemaleAndNonZeroList(Texture2D playerBase)
        {
            return !IsMale && PackHelper.FemaleBaseFaceNoseCount[playerBase][0] > 0 && PackHelper.FemaleBaseFaceNoseCount[playerBase][1] > 0;
        }

        /// <summary>
        /// Sets a custom face nose option from the female face nose dictionary.
        /// </summary>
        /// <param name="playerBase">Current player base</param>
        /// <returns>The custom female face nose texture</returns>
        private Texture2D SetCustomFemaleFaceAndNose(Texture2D playerBase)
        {
            return PackHelper.FemaleFaceAndNoseTextureDict[playerBase][$"female_face{FaceIndex}_nose{NoseIndex}.png"];
        }

        /// <summary>
        /// Patchs player base images with shoes.
        /// </summary>
        /// <param name="playerBase">Current player base.</param>
        private void PatchPlayerShoes(Texture2D playerBase)
        {
            Texture2D shoeTexture;
            // Create new shoeTexture and check the shoe index to load the right shoe
            try
            {
                if (ShoeIndex == 0)
                    shoeTexture = SetDefaultShoes();
                else
                    shoeTexture = SetCustomShoes();
            }
            catch
            {
                // This error pops up when the user saves at a shoe index then removes the content pack
                Entry.Monitor.Log("Could not find a shoe at the index. Did you remove a pack that added shoes? Setting to default.", LogLevel.Warn);
                ShoeIndex = 0;

                shoeTexture = SetDefaultShoes();
            }

            // Patch the player base with the shoes
            int[] shoeHeights = IsMale ? MaleShoeSpriteHeights : FemaleShoeSpriteHeights;
            for (int i = 0; i < shoeHeights.Length; i++)
                PatchBaseTexture(playerBase, shoeTexture, 1 * i, (1 * i) * 3, 96, 32, shoeHeights[i]);
        }

        /// <summary>
        /// Sets the shoes to the default.
        /// </summary>
        /// <returns>The default shoe texture.</returns>
        private Texture2D SetDefaultShoes()
        {
            return Entry.Helper.Content.Load<Texture2D>(IsMale ? "assets/farmer_base_shoes.png" : "assets/farmer_girl_base_shoes.png");
        }

        /// <summary>
        /// Sets the shoes to a custom texture.
        /// </summary>
        /// <returns>The custom shoe texture.</returns>
        private Texture2D SetCustomShoes()
        {
            return IsMale ? PackHelper.MaleShoeTextureList[ShoeIndex - 1] : PackHelper.FemaleShoeTextureList[ShoeIndex - 1];
        }

        /// <summary>
        /// Changes the player base by setting the texture name.
        /// </summary>
        /// <param name="isMale">Whether the player is male</param>
        /// <param name="baseIndex">The base index</param>
        /// <param name="faceIndex">The face index</param>
        /// <param name="noseIndex">The nose index</param>
        /// <param name="shoeIndex">The shoe index</param>
        /// <param name="isBald">Whether the player is bald</param>
        public void ChangePlayerBase(bool isMale, int baseIndex, int faceIndex, int noseIndex, int shoeIndex, bool isBald)
        {
            //Set the fields
            IsMale = isMale;
            BaseIndex = baseIndex;
            FaceIndex = faceIndex;
            NoseIndex = noseIndex;
            ShoeIndex = shoeIndex;
            IsBald = isBald;

            //Check if the base is 0, if it's not we want to load the base version
            if (baseIndex != 0)
                Game1.player.FarmerRenderer.textureName.Set($"GetGlam_IsMale:{IsMale}_BaseIndex:{baseIndex}_FaceIndex:{faceIndex}_NoseIndex:{noseIndex}_ShoeIndex:{shoeIndex}_isBald:{isBald}");
            else
                Game1.player.FarmerRenderer.textureName.Set($"GetGlam_IsMale:{IsMale}_FaceIndex:{faceIndex}_NoseIndex:{noseIndex}_ShoeIndex:{shoeIndex}_isBald:{isBald}");
        }

        /// <summary>
        /// Patches a texture using Texture2D.GetData and Texture2D.SetData.
        /// </summary>
        /// <param name="targetTexture">The target texture</param>
        /// <param name="sourceTexture">The source texture</param>
        /// <param name="sourceID">The source ID</param>
        /// <param name="targetID">The target ID</param>
        /// <param name="gridWidth">The grids width</param>
        /// <param name="gridHeight">The grids height</param>
        /// <param name="adjustedHeight">The adjusted height</param>
        private void PatchBaseTexture(Texture2D targetTexture, Texture2D sourceTexture, int sourceID, int targetID, int gridWidth = 96, int gridHeight = 672, int adjustedHeight = 0)
        {
            Color[] colorData = new Color[gridWidth * (adjustedHeight == 0 ? gridHeight : adjustedHeight)];
            sourceTexture.GetData(0, GetSourceRect(sourceID, sourceTexture, gridWidth, gridHeight, adjustedHeight), colorData, 0, colorData.Length);
            targetTexture.SetData(0, GetSourceRect(targetID, targetTexture, gridWidth, gridHeight, adjustedHeight), colorData, 0, colorData.Length);
        }

        /// <summary>
        /// Get the source rect for an image.
        /// </summary>
        /// <param name="index">The index</param>
        /// <param name="texture">The target texture to get the source rect</param>
        /// <param name="gridWidth">The grids width</param>
        /// <param name="gridHeight">The grids height</param>
        /// <param name="adjustedHeight">The adjusted height</param>
        /// <returns>The textures source rect</returns>
        private Rectangle GetSourceRect(int index, Texture2D texture, int gridWidth, int gridHeight, int adjustedHeight)
        {
            return new Rectangle(
                index % (texture.Width / gridWidth) * gridWidth,
                index / (texture.Width / gridWidth) * gridHeight + (adjustedHeight == 0 ? 0 : 32 - adjustedHeight),
                gridWidth,
                adjustedHeight == 0 ? gridHeight : adjustedHeight
            );
        }
    }
}
