using Newtonsoft.Json;

namespace GetDressed.Framework
{
    /// <summary>The JSON model for a per-save settings file.</summary>
    public class LocalConfig
    {
        /*********
        ** Properties
        *********/
        /****
        ** Metadata
        ****/
        /// <summary>Whether this save hasn't been loaded with GetDressed yet.</summary>
        public bool FirstRun { get; set; } = true;

        /// <summary>The name of the associated save.</summary>
        [JsonIgnore]
        public string SaveName { get; set; }

        /****
        ** Settings
        ****/
        /// <summary>The accessory chosen for each favorite.</summary>
        public int[] ChosenAccessory = new int[ModConstants.MaxFavorites];

        /// <summary>The face chosen for each favorite.</summary>
        public int[] ChosenFace = new int[ModConstants.MaxFavorites];

        /// <summary>The nose chosen for each favorite.</summary>
        public int[] ChosenNose = new int[ModConstants.MaxFavorites];

        /// <summary>The bottom chosen for each favorite.</summary>
        public int[] ChosenBottoms = new int[ModConstants.MaxFavorites];

        /// <summary>The shoes chosen for each favorite.</summary>
        public int[] ChosenShoes = new int[ModConstants.MaxFavorites];

        /// <summary>The skin chosen for each favorite.</summary>
        public int[] ChosenSkin = new int[ModConstants.MaxFavorites];

        /// <summary>The shirt chosen for each favorite.</summary>
        public int[] ChosenShirt = new int[ModConstants.MaxFavorites];

        /// <summary>The hair style chosen for each favorite.</summary>
        public int[] ChosenHairstyle = new int[ModConstants.MaxFavorites];

        /// <summary>The hair color for each favorite.</summary>
        public uint[] ChosenHairColor = new uint[ModConstants.MaxFavorites];

        /// <summary>The eye color chosen for each favorite.</summary>
        public uint[] ChosenEyeColor = new uint[ModConstants.MaxFavorites];

        /// <summary>The bottom color chosen for each favorite.</summary>
        public uint[] ChosenBottomsColor = new uint[ModConstants.MaxFavorites];
    }
}
