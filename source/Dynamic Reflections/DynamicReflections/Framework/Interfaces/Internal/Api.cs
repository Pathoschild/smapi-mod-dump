/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/DynamicReflections
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicReflections.Framework.Interfaces.Internal
{
    public interface IApi
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

    public class Api : IApi
    {
        public bool IsDrawAnyReflection()
        {
            return DynamicReflections.isDrawingWaterReflection || DynamicReflections.isDrawingWaterReflection || DynamicReflections.isDrawingPuddles;
        }

        public bool IsDrawingWaterReflection()
        {
            return DynamicReflections.isDrawingWaterReflection;
        }

        public bool IsDrawingPuddleReflection()
        {
            return DynamicReflections.isDrawingPuddles;
        }

        public bool IsDrawingMirrorReflection()
        {
            return DynamicReflections.isDrawingMirrorReflection;
        }


        public bool IsFilteringWater()
        {
            return DynamicReflections.isFilteringWater;
        }

        public bool IsFilteringPuddles()
        {
            return DynamicReflections.isFilteringPuddles;
        }

        public bool IsFilteringMirrors()
        {
            return DynamicReflections.isFilteringMirror;
        }

        public bool IsFilteringStars()
        {
            return DynamicReflections.isFilteringStar;
        }
    }
}
