/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

namespace Omegasis.StardustCore.UIUtilities.SpriteFonts.Components
{
    /// <summary>Used to determine spacing between TexturedCharacters.</summary>
    public class CharacterSpacing
    {
        /// <summary>Padding to offset this chaarcter by the previous character;</summary>
        public int LeftPadding;

        /// <summary>Padding to offset the following character by.</summary>
        public int RightPadding;

        /// <summary>Padding to offset the top character by. Not sure if this will be used.</summary>
        public int TopPadding;

        /// <summary>Padding to offset the bottom character by. Not sure if this will be used.</summary>
        public int BottomPadding;

        /// <summary>Empty constructor;</summary>
        public CharacterSpacing() { }

        /// <summary>Construct an instance.</summary>
        public CharacterSpacing(int left, int right, int top, int bottom)
        {
            this.LeftPadding = left;
            this.RightPadding = right;
            this.TopPadding = top;
            this.BottomPadding = bottom;
        }
    }
}
