/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/SolidFoundations
**
*************************************************/

using StardewValley.GameData.Buildings;
using System.Collections.Generic;

namespace SolidFoundations.Framework.Models.ContentPack
{
    public class ExtendedBuildingSkin : BuildingSkin
    {
        public string ID { get { return base.Id; } set { base.Id = value; } }

        public List<PaintMaskData> PaintMasks;

        public string PaintMaskTexture;
    }
}
