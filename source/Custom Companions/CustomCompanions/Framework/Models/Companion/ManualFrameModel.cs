/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/CustomCompanions
**
*************************************************/


namespace CustomCompanions.Framework.Models.Companion
{
    public class ManualFrameModel
    {
        public int Frame { get; set; }
        public int Duration { get; set; }
        public bool Flip { get; set; }

        public override string ToString()
        {
            return $"[Frame: {Frame} | Duration: {Duration}: | Flip: {Flip}]";
        }
    }
}
