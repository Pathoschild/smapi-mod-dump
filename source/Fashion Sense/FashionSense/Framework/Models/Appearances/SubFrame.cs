/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/FashionSense
**
*************************************************/

namespace FashionSense.Framework.Models.Appearances
{
    public class SubFrame
    {
        public enum Type
        {
            Unknown,
            Normal,
            SlingshotFrontArm,
            SlingshotBackArm
        }


        public int Frame { get; set; }
        public Type Handling { get; set; } = Type.Normal;
    }
}
