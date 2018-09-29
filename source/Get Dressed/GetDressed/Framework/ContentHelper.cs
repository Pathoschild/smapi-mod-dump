using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using SFarmer = StardewValley.Farmer;

namespace GetDressed.Framework
{
    /// <summary>Encapsulates the underlying mod texture management.</summary>
    internal class ContentHelper
    {
        /*********
        ** Properties
        *********/
        /// <summary>Provides simplified APIs for writing mods.</summary>
        private readonly IModHelper ModHelper;

        /// <summary>Encapsulates monitoring and logging.</summary>
        private readonly IMonitor Monitor;

        /// <summary>The underlying content manager.</summary>
        private readonly ContentManager Content;

        /// <summary>The spritesheet for player accessories.</summary>
        private readonly Texture2D AccessoriesTexture;


        /*********
        ** Accessors
        *********/
        /// <summary>The spritesheet used to draw the player customisation menu.</summary>
        public Texture2D MenuTextures { get; }


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        /// <param name="monitor">Encapsulates monitoring and logging.</param>
        /// <param name="serviceProvider">The service provided used to parse content files.</param>
        public ContentHelper(IModHelper helper, IMonitor monitor, IServiceProvider serviceProvider)
        {
            // construct
            this.ModHelper = helper;
            this.Monitor = monitor;
            this.Content = new ContentManager(serviceProvider, Path.Combine(this.ModHelper.DirectoryPath, "overrides"));

            // load textures
            try
            {
                this.AccessoriesTexture = this.Content.Load<Texture2D>("accessories");
                this.MenuTextures = this.Content.Load<Texture2D>("menuTextures");
            }
            catch
            {
                this.Monitor.Log("Could not find load the accessories or menuTextures file.", LogLevel.Error);
            }
        }

        /// <summary>Update a player's spritesheets to use the GetDressed textures.</summary>
        /// <param name="farmer">The player to update.</param>
        /// <param name="baseTexture">The base farmer texture to inject.</param>
        public void PatchFarmerRenderer(SFarmer farmer, Texture2D baseTexture)
        {
            farmer.FarmerRenderer = new FarmerRenderer(baseTexture) { heightOffset = farmer.isMale ? 0 : 4 };
            FarmerRenderer.accessoriesTexture = this.AccessoriesTexture;
            this.FixFarmerEffects(farmer);
        }

        /// <summary>Overwrite a sprite within a target texture.</summary>
        /// <param name="targetTexture">The texture to patch.</param>
        /// <param name="sourceTextureName">The name of the file in the mod's <c>overrides</c> folder from which to copy the sprite.</param>
        /// <param name="sourceID">The sprite ID within the source texture to copy.</param>
        /// <param name="targetID">The sprite ID within the target texture to overwrite.</param>
        /// <param name="gridWidth">The width of each sprite.</param>
        /// <param name="gridHeight">The height of each sprite.</param>
        /// <param name="adjustedHeight">The desired height of a portion of a sprite.</param>
        public void PatchTexture(ref Texture2D targetTexture, string sourceTextureName, int sourceID, int targetID, int gridWidth = 96, int gridHeight = 672, int adjustedHeight = 0)
        {
            using (FileStream textureStream = new FileStream(Path.Combine(this.Content.RootDirectory, sourceTextureName), FileMode.Open))
            using (Texture2D sourceTexture = Texture2D.FromStream(Game1.graphics.GraphicsDevice, textureStream))
            {
                Color[] data = new Color[gridWidth * (adjustedHeight == 0 ? gridHeight : adjustedHeight)];
                sourceTexture.GetData(0, GetSourceRect(sourceID, sourceTexture, gridWidth, gridHeight, adjustedHeight), data, 0, data.Length);
                targetTexture.SetData(0, GetSourceRect(targetID, targetTexture, gridWidth, gridHeight, adjustedHeight), data, 0, data.Length);
            }
        }

        /// <summary>Force a player character to render the correct accessories.</summary>
        /// <param name="farmer">The player to update.</param>
        public void FixFarmerEffects(SFarmer farmer)
        {
            farmer.changeShirt(farmer.shirt);
            farmer.changeEyeColor(farmer.newEyeColor);
            farmer.changeSkinColor(farmer.skin);
            if (farmer.boots != null)
                farmer.changeShoeColor(farmer.boots.indexInColorSheet);
        }

        /// <summary>Get the base farmer texture.</summary>
        /// <param name="isMale">Whether to load the male texture (else female).</param>
        public Texture2D GetBaseFarmerTexture(bool isMale)
        {
            ContentManager content = new ContentManager(Game1.content.ServiceProvider, Path.Combine(this.ModHelper.DirectoryPath, "overrides")); // get new texture
            try
            {
                return isMale
                    ? content.Load<Texture2D>("farmer_base")
                    : content.Load<Texture2D>("farmer_girl_base");
            }
            catch
            {
                this.Monitor.Log("Could not find base file.", LogLevel.Error);
            }
            return null;
        }

        /// <summary>Get a sprite source rectangle within a spritesheet.</summary>
        /// <param name="index">The sprite index within the spritesheet.</param>
        /// <param name="texture">The spritesheet.</param>
        /// <param name="gridWidth">The width of each sprite.</param>
        /// <param name="gridHeight">The height of each sprite.</param>
        /// <param name="adjustedHeight">The height of a portion of a sprite.</param>
        private Rectangle GetSourceRect(int index, Texture2D texture, int gridWidth, int gridHeight, int adjustedHeight)
        {
            return new Rectangle(index % (texture.Width / gridWidth) * gridWidth, index / (texture.Width / gridWidth) * gridHeight + (adjustedHeight == 0 ? 0 : 32 - adjustedHeight), gridWidth, adjustedHeight == 0 ? gridHeight : adjustedHeight);
        }
    }
}
