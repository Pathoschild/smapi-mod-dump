using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomNPCFramework.Framework.ModularNPCS.ColorCollections
{
    /// <summary>
    /// Collection of colors to be used for the StandardCharacterAnimation object.
    /// </summary>
    public class StandardColorCollection
    {
        /// <summary>
        /// The draw color to be used for the body sprite for the npc.
        /// </summary>
        public Color bodyColor;
        /// <summary>
        /// The draw color to be used for the eye sprite for the npc.
        /// </summary>
        public Color eyeColor;
        /// <summary>
        /// The draw color to be used for the hair sprite for the npc.
        /// </summary>
        public Color hairColor;
        /// <summary>
        /// The draw color to be used for the shirt sprite for the npc.
        /// </summary>
        public Color shirtColor;
        /// <summary>
        /// The draw color to be used for the bottoms/pants sprite for the npc.
        /// </summary>
        public Color bottomsColor;
        /// <summary>
        /// The draw color to be used for the shoes sprite for the npc.
        /// </summary>
        public Color shoesColor;

        /// <summary>
        /// Default constrctor that sets all of the draw colors to white.
        /// </summary>
        public StandardColorCollection()
        {
            defaultColor(this.bodyColor);
            defaultColor(this.eyeColor);
            defaultColor(this.hairColor);
            defaultColor(this.shirtColor);
            defaultColor(this.bottomsColor);
            defaultColor(this.shoesColor);
        }

        /// <summary>
        /// Constructor that takes different colors as parameters.
        /// </summary>
        /// <param name="BodyColor">Color for the body texture.</param>
        /// <param name="EyeColor">Color for the eyes texture.</param>
        /// <param name="HairColor">Color for the hair texture.</param>
        /// <param name="ShirtColor">Color for the shirt texture.</param>
        /// <param name="BottomsColor">Color for the bottoms texture.</param>
        /// <param name="ShoesColor">Color for the shoes texture.</param>
        public StandardColorCollection(Color? BodyColor, Color? EyeColor, Color? HairColor, Color? ShirtColor, Color? BottomsColor, Color? ShoesColor)
        {
            this.bodyColor = (Color)BodyColor.GetValueOrDefault(Color.White);
            this.eyeColor = (Color)EyeColor.GetValueOrDefault(Color.White);
            this.hairColor = (Color)HairColor.GetValueOrDefault(Color.White);
            this.shirtColor = (Color)ShirtColor.GetValueOrDefault(Color.White);
            this.bottomsColor = (Color)BottomsColor.GetValueOrDefault(Color.White);
            this.shoesColor = (Color)ShoesColor.GetValueOrDefault(Color.White);

            defaultColor(this.bodyColor);
            defaultColor(this.eyeColor);
            defaultColor(this.hairColor);
            defaultColor(this.shirtColor);
            defaultColor(this.bottomsColor);
            defaultColor(this.shoesColor);
        }

        /// <summary>
        /// If a color is null, make it white.
        /// </summary>
        /// <param name="color"></param>
        public void defaultColor(Color color)
        {
            if (color == null) color = Color.White;
        }

        /// <summary>
        /// Used to mix colors together. 
        /// </summary>
        /// <param name="cBase">The base color to mix.</param>
        /// <param name="cMult">The modifier color to mix.</param>
        /// <returns>A color that is a mix between the two colors passed in.</returns>
        public static Color colorMult(Color cBase, Color cMult)
        {
            Vector3 color1 = cBase.ToVector3();
            Vector3 color2 = cMult.ToVector3();
            Vector3 mixColor = color1 * color2;
            Color value = new Color(mixColor);
            return value;
        }
    }
}
