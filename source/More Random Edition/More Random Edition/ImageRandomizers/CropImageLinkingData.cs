/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

namespace Randomizer
{
    /// <summary>
    /// Linking data to be used  by the Object and CropGrowth image builders so that the 
    /// images get manipulated in the same way (matching crop/seed/plant graphics, etc)
    /// </summary>
    public class CropImageLinkingData
    {
        /// <summary>
        /// What image was just selected to be pasted into the base image
        /// </summary>
        public string ImageName { get; set; }

        /// <summary>
        /// What value was set for the hue shift - used to make the linked images get the same value
        /// </summary>
        public int HueShiftValue { get; set; }

        /// <summary>
        /// The seed item of the linked data
        /// Used to determine whether it's a trellis, and to generate an appropriate packet color
        /// </summary>
        public SeedItem SeedItem { get; set; }

        public CropImageLinkingData(string imageName, SeedItem seedItem)
        {
            ImageName = imageName;
            SeedItem = seedItem;
        }
    }
}
