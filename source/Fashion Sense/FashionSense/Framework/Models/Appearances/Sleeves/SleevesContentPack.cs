/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/FashionSense
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;

namespace FashionSense.Framework.Models.Appearances.Sleeves
{
    public class SleevesContentPack : AppearanceContentPack
    {
        public SleevesModel BackSleeves { get; set; }
        public SleevesModel RightSleeves { get; set; }
        public SleevesModel FrontSleeves { get; set; }
        public SleevesModel LeftSleeves { get; set; }
        internal Texture2D ShirtToneTexture { get; set; }

        internal SleevesModel GetSleevesFromFacingDirection(int facingDirection)
        {
            SleevesModel SleevesModel = null;
            switch (facingDirection)
            {
                case 0:
                    SleevesModel = BackSleeves;
                    break;
                case 1:
                    SleevesModel = RightSleeves;
                    break;
                case 2:
                    SleevesModel = FrontSleeves;
                    break;
                case 3:
                    SleevesModel = LeftSleeves;
                    break;
            }

            return SleevesModel;
        }

        internal override void LinkId()
        {
            if (BackSleeves is AppearanceModel backModel && backModel is not null)
            {
                backModel.Pack = this;
            }
            if (RightSleeves is AppearanceModel rightModel && rightModel is not null)
            {
                rightModel.Pack = this;
            }
            if (FrontSleeves is AppearanceModel frontModel && frontModel is not null)
            {
                frontModel.Pack = this;
            }
            if (LeftSleeves is AppearanceModel leftModel && leftModel is not null)
            {
                leftModel.Pack = this;
            }
        }
    }
}
