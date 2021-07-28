/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapTK.MapExtras
{
    internal class IntegratedMapMergesHandler
    {

        public IntegratedMapMergesHandler(IModHelper helper)
        {
            helper.Content.AssetEditors.Add(new IntegratedMapEditsAssetEditor(helper));
        }
    }
}
