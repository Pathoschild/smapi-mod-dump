using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using SObject = StardewValley.Object;

namespace SilentOak.QualityProducts.Patches.BetterMeadIcons
{
    internal static class SpriteLoader
    {
        private static IModHelper Helper;
        private static IMonitor Monitor;

        private static Texture2D MeadTexture;

        public static bool Init(IModHelper helper, IMonitor monitor, QualityProductsConfig config)
        {
            Helper = helper;
            Monitor = monitor;

            try
            {
                MeadTexture = Helper.Content.Load<Texture2D>(config.TextureForMeadTypes);
                Monitor.Log($"Custom texture \"{config.TextureForMeadTypes}\" loaded.");
                return true;
            }
            catch (ArgumentException)
            {
                Monitor.Log($"Invalid path \"{config.TextureForMeadTypes}\" for texture. Custom textures disabled.", LogLevel.Warn);
            }
            catch (ContentLoadException)
            {
                Monitor.Log($"File \"{config.TextureForMeadTypes}\" could not be loaded. Custom textures disabled.", LogLevel.Warn);
            }
            return false;
        }

        public static bool TryLoadSprite(SObject @object, out Texture2D texture, out Rectangle sourceRect)
        {
            if (
                @object == null
                || (bool)@object.bigCraftable 
                || @object.ParentSheetIndex != 459
                || @object.honeyType == null
                || !@object.honeyType.Value.HasValue
                || @object.honeyType.Value.Value == SObject.HoneyType.Wild
                )
            {
                texture = default;
                sourceRect = default;
                return false;
            }

            texture = MeadTexture; 
            switch (@object.honeyType.Value.Value)
            {
                case SObject.HoneyType.Tulip:
                    sourceRect = new Rectangle(0, 0, 16, 16);
                    break;

                case SObject.HoneyType.BlueJazz:
                    sourceRect = new Rectangle(16, 0, 16, 16);
                    break;

                case SObject.HoneyType.SummerSpangle:
                    sourceRect = new Rectangle(32, 0, 16, 16);
                    break;

                case SObject.HoneyType.Poppy:
                    sourceRect = new Rectangle(48, 0, 16, 16);
                    break;

                case SObject.HoneyType.FairyRose:
                    sourceRect = new Rectangle(64, 0, 16, 16);
                    break;

                default:
                    sourceRect = default;
                    break;
            }

            return true;
        }
    }
}