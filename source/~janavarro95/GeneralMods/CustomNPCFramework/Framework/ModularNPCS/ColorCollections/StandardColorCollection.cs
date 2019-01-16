using Microsoft.Xna.Framework;

namespace CustomNPCFramework.Framework.ModularNpcs.ColorCollections
{
    /// <summary>Collection of colors to be used for the StandardCharacterAnimation object.</summary>
    public class StandardColorCollection
    {
        /// <summary>The draw color to be used for the body sprite for the npc.</summary>
        public Color bodyColor;

        /// <summary>The draw color to be used for the eye sprite for the npc.</summary>
        public Color eyeColor;

        /// <summary>The draw color to be used for the hair sprite for the npc.</summary>
        public Color hairColor;

        /// <summary>The draw color to be used for the shirt sprite for the npc.</summary>
        public Color shirtColor;

        /// <summary>The draw color to be used for the bottoms/pants sprite for the npc.</summary>
        public Color bottomsColor;

        /// <summary>The draw color to be used for the shoes sprite for the npc.</summary>
        public Color shoesColor;

        /// <summary>Construct an instance.</summary>
        public StandardColorCollection()
        {
            this.defaultColor(this.bodyColor);
            this.defaultColor(this.eyeColor);
            this.defaultColor(this.hairColor);
            this.defaultColor(this.shirtColor);
            this.defaultColor(this.bottomsColor);
            this.defaultColor(this.shoesColor);
        }

        /// <summary>Construct an instance.</summary>
        /// <param name="bodyColor">Color for the body texture.</param>
        /// <param name="eyeColor">Color for the eyes texture.</param>
        /// <param name="hairColor">Color for the hair texture.</param>
        /// <param name="shirtColor">Color for the shirt texture.</param>
        /// <param name="bottomsColor">Color for the bottoms texture.</param>
        /// <param name="shoesColor">Color for the shoes texture.</param>
        public StandardColorCollection(Color? bodyColor, Color? eyeColor, Color? hairColor, Color? shirtColor, Color? bottomsColor, Color? shoesColor)
        {
            this.bodyColor = bodyColor.GetValueOrDefault(Color.White);
            this.eyeColor = eyeColor.GetValueOrDefault(Color.White);
            this.hairColor = hairColor.GetValueOrDefault(Color.White);
            this.shirtColor = shirtColor.GetValueOrDefault(Color.White);
            this.bottomsColor = bottomsColor.GetValueOrDefault(Color.White);
            this.shoesColor = shoesColor.GetValueOrDefault(Color.White);

            this.defaultColor(this.bodyColor);
            this.defaultColor(this.eyeColor);
            this.defaultColor(this.hairColor);
            this.defaultColor(this.shirtColor);
            this.defaultColor(this.bottomsColor);
            this.defaultColor(this.shoesColor);
        }

        /// <summary>If a color is null, make it white.</summary>
        /// <param name="color">The color to check.</param>
        public void defaultColor(Color color)
        {
            if (color == null)
                color = Color.White;
        }

        /// <summary>Used to mix colors together.</summary>
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
