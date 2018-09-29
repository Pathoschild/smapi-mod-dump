using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardustCore.UIUtilities.SpriteFonts.Components
{
    /// <summary>
    /// Used to determine spacing between TexturedCharacters.
    /// </summary>
    public class CharacterSpacing
    {
        /// <summary>
        /// Padding to offset this chaarcter by the previous character;
        /// </summary>
        public int LeftPadding;
        /// <summary>
        /// Padding to offset the following character by.
        /// </summary>
        public int RightPadding;
        /// <summary>
        /// Padding to offset the top character by. Not sure if this will be used.
        /// </summary>
        public int TopPadding;
        /// <summary>
        /// Padding to offset the bottom character by. Not sure if this will be used.
        /// </summary>
        public int BottomPadding;

        /// <summary>
        /// Empty constructor;
        /// </summary>
        public CharacterSpacing()
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="top"></param>
        /// <param name="bottom"></param>
        public CharacterSpacing(int left, int right, int top, int bottom)
        {
            this.LeftPadding = left;
            this.RightPadding = right;
            this.TopPadding = top;
            this.BottomPadding = bottom;
        }

        /// <summary>
        /// Save this to a .json file.
        /// </summary>
        /// <param name="path"></param>
        public void WriteToJson(string path)
        {
            StardustCore.ModCore.ModHelper.WriteJsonFile(path, this);
        }

        /// <summary>
        /// Read the data from the .json file.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static CharacterSpacing ReadFromJson(string path)
        {
            return StardustCore.ModCore.ModHelper.ReadJsonFile<CharacterSpacing>(path);
        }
    }
}
