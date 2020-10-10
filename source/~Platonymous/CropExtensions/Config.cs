/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

using System.Collections.Generic;

namespace CropExtensions
{
    public class Config
    {
        public bool DetailedCropSeasons { get; set; } = true;

        public Dictionary<string, CropDetails> Presets { get; set; } = new Dictionary<string, CropDetails>();
    }
}
