using System;
using System.Collections.Generic;
using System.IO;
using Kisekae.Config;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using SFarmer = StardewValley.Farmer;

namespace Kisekae.Framework {
    /// <summary>Encapsulates the underlying mod texture management.</summary>
    public class ContentHelper {
        /*********
        ** Properties
        *********/
        /// <summary>Key for local menu texture.</summary>
        public const string s_MenuTextureKey = "menuTextures";

        /// <summary>Provides mod API.</summary>
        private readonly IMod m_env;
        /// <summary>Base texture.</summary>
        private readonly Texture2D m_femaleBaseTexture = null;
        private readonly Texture2D m_maleBaseTexture = null;
        /// <summary>The spritesheet used to draw the player customisation menu.</summary>
        private Texture2D m_menuTextures;
        /// <summary>The texture heights of shoes in the female overrides.</summary>
        private static readonly int[] m_femaleShoeSpriteHeights = new int[21] { 15, 16, 14, 13, 12, 16, 16, 15, 16, 10, 13, 13, 13, 14, 14, 11, 14, 14, 14, 16, 13 };
        /// <summary>The texture heights of shoes in the male overrides.</summary>
        private static readonly int[] s_maleShoeSpriteHeights = new int[21] { 11, 16, 15, 14, 13, 16, 16, 14, 16, 12, 14, 14, 15, 15, 16, 13, 15, 16, 16, 16, 15 };
        /// <summary>Texture cache.</summary>
        private Dictionary<string, Texture2D> m_textureCache = new Dictionary<string, Texture2D>();
        /// <summary>Number of detected extended base texture</summary>
        private int[] m_nExt = new int[] { 0,0,0,0 };
        /// <summary>Relative path to texture</summary>
        private const string s_assetPath = "overrides";

        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        /// <param name="monitor">Encapsulates monitoring and logging.</param>
        /// <param name="serviceProvider">The service provided used to parse content files.</param>
        public ContentHelper(IMod env) {
            // construct
            m_env = env;
            // load textures
            ContentManager content = new ContentManager(Game1.content.ServiceProvider, Path.Combine(m_env.Helper.DirectoryPath, s_assetPath));
            try {
                this.m_menuTextures = content.Load<Texture2D>("menuTextures");
            } catch {
                m_env.Monitor.Log("Could not find load the menuTextures file.", LogLevel.Error);
            }
            try {
                m_maleBaseTexture = content.Load<Texture2D>("farmer_base");
                m_femaleBaseTexture = content.Load<Texture2D>("farmer_girl_base");
            } catch {
                m_env.Monitor.Log("Could not find base file.", LogLevel.Error);
            }
            //TODO: add base texture number limit detect
        }

        /// <summary>Get whether this instance can load the initial version of the given asset.</summary>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        public bool CanLoad<T>(IAssetInfo asset) {
            if (asset.AssetNameEquals(s_MenuTextureKey)) {
                return true;
            } else if (asset.AssetNameEquals(Path.Combine("Characters", "Farmer", "accessories"))) {
                return true;
            } else if (asset.AssetName.StartsWith("KisekaeBase_")) {
                return true;
            }
            return false;
        }

        /// <summary>Load a matched asset.</summary>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        public T Load<T>(IAssetInfo asset) {
            if (asset.AssetNameEquals(s_MenuTextureKey)) {
                return (T)(object)m_menuTextures;
            } else if (asset.AssetNameEquals(Path.Combine("Characters", "Farmer", "accessories"))) {
                return m_env.Helper.Content.Load<T>("overrides/accessories");
            } else if (asset.AssetName.StartsWith("KisekaeBase_")) {
                string[] par = asset.AssetName.Split('_');
                if (par.Length <2 || (par[1] != "male" && par[1] != "female")) {
                    return Game1.content.Load<T>(Path.Combine("Characters", "Farmer", "farmer_base"));
                }
                int[] config = new int[] {0,0,0,0};
                for (int i = 2; i < Math.Min(6, par.Length); ++i) {
                    int n;
                    if (!int.TryParse(par[i], out n)) {
                        n = 0;
                    } else if (n<0 || n>=GetNumberOfTexture(LocalConfig.Attribute.Face + i - 2)) {
                        n = 0;
                    }
                    config[i-2] = n;
                }
                //m_env.Monitor.Log("load base:"+config[0]+"," + config[1] + "," + config[2] + "," + config[3] + ",");
                return (T)(object)LoadBaseTexture(par[1], config);
            }
            return default(T);
        }

        /// <summary>combine base texture.</summary>
        /// <param name="sexPrefix">Either male or female.</param>
        /// /// <param name="config">Index of face, nose, bottoms, and shoes.</param>
        public Texture2D LoadBaseTexture(string sexPrefix, int[] config) {
            Texture2D playerTextures = GetNewBaseFarmerTexture(sexPrefix == "male");
            PatchTexture(playerTextures, $"{sexPrefix}_face{config[0]}_nose{config[1]}.png", 0, 0);
            int[] ShoeSpriteHeights = sexPrefix == "male" ? s_maleShoeSpriteHeights : m_femaleShoeSpriteHeights;
            for (int i = 0; i < ShoeSpriteHeights.Length; i++) {
                PatchTexture(playerTextures, $"{sexPrefix}_shoes{config[3]}.png", 1 * i, (1 * i) * 4, 96, 32, ShoeSpriteHeights[i]);
            }
            PatchTexture(playerTextures, $"{sexPrefix}_bottoms.png", config[2], 3);

            return playerTextures;
        }

        /// <summary>Overwrite a sprite within a target texture.</summary>
        /// <param name="targetTexture">The texture to patch.</param>
        /// <param name="sourceTextureName">The name of the file in the mod's <c>overrides</c> folder from which to copy the sprite.</param>
        /// <param name="sourceID">The sprite ID within the source texture to copy.</param>
        /// <param name="targetID">The sprite ID within the target texture to overwrite.</param>
        /// <param name="gridWidth">The width of each sprite.</param>
        /// <param name="gridHeight">The height of each sprite.</param>
        /// <param name="adjustedHeight">The desired height of a portion of a sprite.</param>
        public void PatchTexture(Texture2D targetTexture, string sourceTextureName, int sourceID, int targetID, int gridWidth = 96, int gridHeight = 672, int adjustedHeight = 0) {
            Texture2D sourceTexture;
            if (m_textureCache.ContainsKey(sourceTextureName)) {
                sourceTexture = m_textureCache[sourceTextureName];
            } else {
                FileStream textureStream = new FileStream(Path.Combine(m_env.Helper.DirectoryPath, s_assetPath, sourceTextureName), FileMode.Open);
                sourceTexture = Texture2D.FromStream(Game1.graphics.GraphicsDevice, textureStream);
                m_textureCache[sourceTextureName] = sourceTexture;
            }
            PatchTexture(targetTexture, sourceTexture, sourceID, targetID, gridWidth, gridHeight, adjustedHeight);
        }

        /// <summary>Overwrite a sprite within a target texture.</summary>
        /// <param name="targetTexture">The texture to patch.</param>
        /// <param name="sourceTexture">The source texture.</param>
        /// <param name="sourceID">The sprite ID within the source texture to copy.</param>
        /// <param name="targetID">The sprite ID within the target texture to overwrite.</param>
        /// <param name="gridWidth">The width of each sprite.</param>
        /// <param name="gridHeight">The height of each sprite.</param>
        /// <param name="adjustedHeight">The desired height of a portion of a sprite.</param>
        public void PatchTexture(Texture2D targetTexture, Texture2D sourceTexture, int sourceID, int targetID, int gridWidth = 96, int gridHeight = 672, int adjustedHeight = 0) {
            Color[] data = new Color[gridWidth * (adjustedHeight == 0 ? gridHeight : adjustedHeight)];
            sourceTexture.GetData(0, GetSourceRect(sourceID, sourceTexture, gridWidth, gridHeight, adjustedHeight), data, 0, data.Length);
            targetTexture.SetData(0, GetSourceRect(targetID, targetTexture, gridWidth, gridHeight, adjustedHeight), data, 0, data.Length);
        }

        /// <summary>Build base texture by combining face, nose, bottom, and shoe choice.</summary>
        /// <param name="player">The player to patch.</param>
        /// <param name="config">The config to patch.</param>
        /// <param name="which">The config number to use.</param>
        public void PatchBaseTexture(SFarmer player, LocalConfig config, int which = 0) {
            if (config.MutiplayerFix) {
                player.FarmerRenderer.textureName.Set("KisekaeBase_" + (player.IsMale ? "male_" : "female_") + config.ChosenFace[which] + "_" + config.ChosenNose[which] + "_" + config.ChosenBottoms[which] + "_" + config.ChosenShoes[which]);
            } else {
                Texture2D playerTextures = LoadBaseTexture(player.isMale ? "male" : "female", new int[] { config.ChosenFace[which], config.ChosenNose[which], config.ChosenBottoms[which], config.ChosenShoes[which] });
                IReflectedField<Texture2D> curBaseTexture = m_env.Helper.Reflection.GetField<Texture2D>(player.FarmerRenderer, "baseTexture");
                curBaseTexture.SetValue(playerTextures);
            }
        }

        /// <summary>Get the base farmer texture.</summary>
        /// <param name="isMale">Whether to load the male texture (else female).</param>
        public Texture2D GetNewBaseFarmerTexture(bool isMale) {
            Texture2D s = isMale ? m_maleBaseTexture : m_femaleBaseTexture;
            Texture2D d = new Texture2D(s.GraphicsDevice, s.Width, s.Height);
            Color[] c = new Color[s.Width * s.Height];
            s.GetData(c);
            d.SetData(c);
            return d;
        }

        /// <summary>Get number of attribute texture.</summary>
        public int GetNumberOfTexture(LocalConfig.Attribute attr) {
            Texture2D texture;
            switch (attr) {
                case LocalConfig.Attribute.Skin:
                    texture = Game1.content.Load<Texture2D>(Path.Combine("Characters", "Farmer", "skinColors"));
                    return texture.Height;
                case LocalConfig.Attribute.Hair:
                    texture = FarmerRenderer.hairStylesTexture;
                    return (texture.Height / (32*3)) * (texture.Width / 16);
                case LocalConfig.Attribute.Shirt:
                    texture = FarmerRenderer.shirtsTexture;
                    return (texture.Height / (8*4)) * (texture.Width / 8);
                case LocalConfig.Attribute.Accessory:
                    texture = FarmerRenderer.accessoriesTexture;
                    return (texture.Height / (16*2)) * (texture.Width / 16);
                case LocalConfig.Attribute.EyeColor:
                    return 1 << 6;
                case LocalConfig.Attribute.HairColor:
                    return 1 << 6;
                case LocalConfig.Attribute.BottomsColor:
                    return 1 << 6;
                case LocalConfig.Attribute.ShoeColor:
                    texture = Game1.content.Load<Texture2D>(Path.Combine("Characters", "Farmer", "shoeColors"));
                    return texture.Height;
                // TODO: change to base texture number limit
                case LocalConfig.Attribute.Face:
                    return 2;
                case LocalConfig.Attribute.Nose:
                    return 3;
                case LocalConfig.Attribute.Bottoms:
                    return 12;
                case LocalConfig.Attribute.Shoes:
                    return 4;
                default:
                    return 0;
            }
        }

        /*********
        ** Private methods
        *********/
        /// <summary>Get a sprite source rectangle within a spritesheet.</summary>
        /// <param name="index">The sprite index within the spritesheet.</param>
        /// <param name="texture">The spritesheet.</param>
        /// <param name="gridWidth">The width of each sprite.</param>
        /// <param name="gridHeight">The height of each sprite.</param>
        /// <param name="adjustedHeight">The height of a portion of a sprite.</param>
        private Rectangle GetSourceRect(int index, Texture2D texture, int gridWidth, int gridHeight, int adjustedHeight) {
            return new Rectangle(
                index % (texture.Width / gridWidth) * gridWidth,
                index / (texture.Width / gridWidth) * gridHeight + (adjustedHeight == 0 ? 0 : 32 - adjustedHeight),
                gridWidth,
                adjustedHeight == 0 ? gridHeight : adjustedHeight
            );
        }
    }
}
