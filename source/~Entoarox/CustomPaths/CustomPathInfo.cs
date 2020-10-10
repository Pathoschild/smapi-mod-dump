/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Entoarox/StardewMods
**
*************************************************/

using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace Entoarox.CustomPaths
{
    public class CustomPathInfo
    {
        /*********
        ** Fields
        *********/
        private readonly bool Seasonal;
        private readonly Dictionary<string, Texture2D> Textures;


        /*********
        ** Accessors
        *********/
        public int Alternates;
        public string Name;
        public int Price;
        public string Requirements;
        public string Salesman;
        public int Speed;
        public bool Animated;
        public int[] Frames;
        public int MillisPerFrame;


        /*********
        ** Public methods
        *********/
        internal CustomPathInfo(Texture2D texture, CustomPathConfig config)
        {
            this.Seasonal = false;
            this.Textures = new Dictionary<string, Texture2D> { { "default", texture } };
            this.Name = config.Name;
            this.Alternates = config.Alternatives;
            this.Price = config.Price;
            this.Salesman = config.Salesman;
            this.Requirements = config.Requirements;
            this.Speed = config.SpeedBoost;
            this.Animated = config.Animated;
            this.Frames = config.Frames;
            this.MillisPerFrame = config.MillisPerFrame;
        }

        internal CustomPathInfo(Dictionary<string, Texture2D> textures, CustomPathConfig config)
        {
            this.Seasonal = true;
            this.Textures = textures;
            this.Name = config.Name;
            this.Alternates = config.Alternatives;
            this.Price = config.Price;
            this.Salesman = config.Salesman;
            this.Requirements = config.Requirements;
            this.Speed = config.SpeedBoost;
        }

        public Texture2D GetTexture()
        {
            return this.Textures[this.Seasonal ? Game1.currentSeason : "default"];
        }
    }
}
