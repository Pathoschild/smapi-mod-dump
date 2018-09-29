using Microsoft.Xna.Framework.Graphics;
using StardustCore.UIUtilities.SpriteFonts.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardustCore.UIUtilities.SpriteFonts.CharacterSheets
{
    public class GenericCharacterSheets
    {
        public Dictionary<char, TexturedCharacter> CharacterAtlus;

        public virtual TexturedCharacter getTexturedCharacter(char c)
        {
            return new TexturedCharacter();
        }
    }
}
