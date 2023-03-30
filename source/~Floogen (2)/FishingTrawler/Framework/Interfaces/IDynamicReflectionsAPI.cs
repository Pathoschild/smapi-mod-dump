/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/FishingTrawler
**
*************************************************/

namespace FishingTrawler.Framework.Interfaces
{
    public interface IDynamicReflectionsAPI
    {
        public bool IsDrawAnyReflection();
        public bool IsDrawingWaterReflection();
        public bool IsDrawingPuddleReflection();
        public bool IsDrawingMirrorReflection();

        public bool IsFilteringWater();
        public bool IsFilteringPuddles();
        public bool IsFilteringMirrors();
        public bool IsFilteringStars();
    }
}
