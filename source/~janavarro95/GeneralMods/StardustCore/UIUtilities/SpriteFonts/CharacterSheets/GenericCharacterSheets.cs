/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System.Collections.Generic;
using StardustCore.UIUtilities.SpriteFonts.Components;

namespace StardustCore.UIUtilities.SpriteFonts.CharacterSheets
{
    public class GenericCharacterSheets
    {
        public Dictionary<char, TexturedCharacter> CharacterAtlus;


        public GenericCharacterSheets()
        {

        }

        public GenericCharacterSheets(string Path)
        {

        }

        public virtual TexturedCharacter getTexturedCharacter(char c)
        {
            var original = this.CharacterAtlus[c];
            return TexturedCharacter.Copy(original);
        }

        public virtual GenericCharacterSheets create(string Path)
        {
            return new GenericCharacterSheets(Path);
        }
    }
}
