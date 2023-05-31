/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/Archery
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Objects;

namespace Archery.Framework.Utilities
{
    internal class RendereringHelper
    {
        private static Color _skinDarkTone = new Color(107, 0, 58);
        private static Color _skinMediumTone = new Color(224, 107, 101);
        private static Color _skinLightTone = new Color(249, 174, 137);
        private static Color _sleeveDarkTone = new Color(80, 80, 80);
        private static Color _sleeveMediumTone = new Color(135, 135, 135);
        private static Color _sleeveLightTone = new Color(154, 154, 154);

        internal static Texture2D RecolorBowArms(Farmer farmer, Texture2D inputTexture)
        {
            // Get the base texture and create a mask
            Color[] data = new Color[inputTexture.Width * inputTexture.Height];
            inputTexture.GetData(data);
            Texture2D maskedTexture = new Texture2D(Game1.graphics.GraphicsDevice, inputTexture.Width, inputTexture.Height);

            // Get the skin tones
            int which = farmer.skin.Value;
            Texture2D skinColors = Archery.modHelper.GameContent.Load<Texture2D>("Characters/Farmer/skinColors");
            Color[] skinColorsData = new Color[skinColors.Width * skinColors.Height];
            if (which < 0)
            {
                which = skinColors.Height - 1;
            }
            if (which > skinColors.Height - 1)
            {
                which = 0;
            }
            skinColors.GetData(skinColorsData);
            Color skinDarkest = skinColorsData[which * 3 % (skinColors.Height * 3)];
            Color skinMedium = skinColorsData[which * 3 % (skinColors.Height * 3) + 1];
            Color skinLightest = skinColorsData[which * 3 % (skinColors.Height * 3) + 2];

            // Get the shirt 
            bool isSleevesShirt = farmer.GetShirtExtraData().Contains("Sleeveless");

            Color shirtColor = farmer.GetShirtColor();
            if (isSleevesShirt is false && Archery.apiManager.IsFashionSenseLoaded())
            {
                var responseToColor = Archery.apiManager.GetFashionSenseApi().GetAppearanceColor(Interfaces.IFashionSenseApi.Type.Sleeves, farmer);
                if (responseToColor.Key is true)
                {
                    shirtColor = responseToColor.Value;
                }
                else
                {
                    responseToColor = Archery.apiManager.GetFashionSenseApi().GetAppearanceColor(Interfaces.IFashionSenseApi.Type.Shirt, farmer);
                    if (responseToColor.Key is true)
                    {
                        shirtColor = responseToColor.Value;
                    }
                }
            }
            else if (Archery.apiManager.IsFashionSenseLoaded() is false)
            {
                Color[] shirtData = new Color[FarmerRenderer.shirtsTexture.Bounds.Width * FarmerRenderer.shirtsTexture.Bounds.Height];
                FarmerRenderer.shirtsTexture.GetData(shirtData);
                int index = ClampShirt(farmer.GetShirtIndex()) * 8 / 128 * 32 * FarmerRenderer.shirtsTexture.Bounds.Width + ClampShirt(farmer.GetShirtIndex()) * 8 % 128 + FarmerRenderer.shirtsTexture.Width * 4;

                shirtColor = Utility.MakeCompletelyOpaque(Utility.MultiplyColor(shirtData[index - FarmerRenderer.shirtsTexture.Width * 2], farmer.GetShirtColor()));
            }

            // Start the recoloring
            for (int i = 0; i < data.Length; i++)
            {
                // Skin tones
                if (data[i] == _skinLightTone)
                {
                    data[i] = skinLightest;
                }
                else if (data[i] == _skinMediumTone)
                {
                    data[i] = skinMedium;
                }
                else if (data[i] == _skinDarkTone)
                {
                    data[i] = skinDarkest;
                }

                // Sleeve tones
                if (data[i] == _sleeveLightTone)
                {
                    data[i] = isSleevesShirt ? skinLightest : Utility.MultiplyColor(_sleeveLightTone, shirtColor);
                }
                else if (data[i] == _sleeveMediumTone)
                {
                    data[i] = isSleevesShirt ? skinMedium : Utility.MultiplyColor(_sleeveMediumTone, shirtColor);
                }
                else if (data[i] == _sleeveDarkTone)
                {
                    data[i] = isSleevesShirt ? skinDarkest : Utility.MultiplyColor(_sleeveDarkTone, shirtColor);
                }
            }

            // Set the mask texture and push it to recoloredArmsTexture
            maskedTexture.SetData(data);
            return maskedTexture;
        }

        private static int ClampShirt(int shirt_value)
        {
            if (shirt_value > Clothing.GetMaxShirtValue() || shirt_value < 0)
            {
                return 0;
            }
            return shirt_value;
        }

    }
}
