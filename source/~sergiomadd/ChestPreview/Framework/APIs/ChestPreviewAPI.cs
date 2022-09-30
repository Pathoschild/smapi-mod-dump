/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sergiomadd/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChestPreview.Framework;

namespace ChestPreview.Framework.APIs
{
    public class ChestPreviewAPI : IChestPreviewAPI
    {
        public string GetPreviewSizeString()
        {
            return Conversor.GetSizeName(ModEntry.config.Size);
        }
        public int GetPreviewSizeInt()
        {
            return (int)ModEntry.CurrentSize;
        }
        public float GetPreviewScale()
        {
            return Conversor.GetCurrentSizeValue();
        }
    }
}
