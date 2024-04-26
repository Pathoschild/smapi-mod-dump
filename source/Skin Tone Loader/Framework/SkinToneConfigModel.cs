/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/HeyImAmethyst/SkinToneLoader
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinToneLoader.Framework
{
    /// <summary>
    /// Class the allows the player to save their customization layout.
    /// </summary>
    public class SkinToneConfigModel
    {
        // The save folders name
        public string SaveFolderName;

        // The skin index
        public int SkinIndex = 0;
    }
}
