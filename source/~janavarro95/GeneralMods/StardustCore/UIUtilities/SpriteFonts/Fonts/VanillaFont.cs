using Microsoft.Xna.Framework;
using StardustCore.UIUtilities.SpriteFonts.CharacterSheets;
using StardustCore.UIUtilities.SpriteFonts.Components;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardustCore.UIUtilities.SpriteFonts.Fonts
{
    public class VanillaFont :GenericFont
    {
        /// <summary>
        /// Constructor;
        /// </summary>
        public VanillaFont()
        {
            this.path = Path.Combine(SpriteFonts.FontDirectory, "Vanilla");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            this.characterSheet = new VanillaCharacterSheet(path);
        }

        /// <summary>
        ///  Takes a string and returns a textured string in it's place.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public TexturedString ParseString(string str)
        {
            List<TexturedCharacter> characters=new List<TexturedCharacter>();
            foreach(var chr in str)
            {
               characters.Add(characterSheet.getTexturedCharacter(chr));
            }
            var tStr = new TexturedString(str,new Microsoft.Xna.Framework.Vector2(0, 0), characters);
            return tStr;
        }

        /// <summary>
        /// Takes a string and returns a textured string in it's place. Also sets the new position.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="Position"></param>
        /// <returns></returns>
        public TexturedString ParseString(string str,Vector2 Position)
        {
            List<TexturedCharacter> characters = new List<TexturedCharacter>();
            foreach (var chr in str)
            {
                characters.Add(characterSheet.getTexturedCharacter(chr));
            }
            var tStr = new TexturedString(str,Position, characters);
            return tStr;
        }

        /// <summary>
        /// Takes a string and returns a textured string in it's place. Also sets the new position and string color.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="Position"></param>
        /// <param name="stringColor"></param>
        /// <returns></returns>
        public TexturedString ParseString(string str, Vector2 Position, Color stringColor, bool useRightPadding = true)
        {
            List<TexturedCharacter> characters = new List<TexturedCharacter>();
            foreach (var chr in str)
            {
                var c = characterSheet.getTexturedCharacter(chr);
                c.drawColor = stringColor;
                characters.Add(c);
            }
            var tStr = new TexturedString(str,Position, characters,useRightPadding);
            return tStr;
        }

        /// <summary>
        /// Takes a string and returns a textured string in it's place. Also sets the new position, label and string color.
        /// </summary>
        /// <param name="label">The label for the string.</param>
        /// <param name="str">The string that wil be parsed into textured characters.</param>
        /// <param name="Position">The position to draw the textured string.</param>
        /// <param name="stringColor">The color of the textured string.</param>
        /// <returns></returns>
        public TexturedString ParseString(string label,string str, Vector2 Position, Color stringColor, bool useRightPadding = true)
        {
            List<TexturedCharacter> characters = new List<TexturedCharacter>();
            foreach (var chr in str)
            {
                var c = characterSheet.getTexturedCharacter(chr);
                c.drawColor = stringColor;
                characters.Add(c);
            }
            var tStr = new TexturedString(label, Position, characters,false);
            return tStr;
        }

        /// <summary>
        /// Takes a string and returns a textured string in it's place. Also sets the new position and string color.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="Position"></param>
        /// <param name="stringColor">The color for the individual characters.</param>
        /// <returns></returns>
        public TexturedString ParseString(string str, Vector2 Position, List<Color> stringColor)
        {
            List<TexturedCharacter> characters = new List<TexturedCharacter>();
            int index = 0;
            foreach (var chr in str)
            {
                var c = characterSheet.getTexturedCharacter(chr);
                c.drawColor = stringColor.ElementAt(index);
                characters.Add(c);
                index++;
            }
            var tStr = new TexturedString(str,Position, characters);
            return tStr;
        }


    }
}
