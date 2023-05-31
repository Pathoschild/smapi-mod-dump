/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omegasis.Revitalize.Framework.SaveData.Player
{
    public class PlayerObtainedItems: SaveDataInfo
    {
        public HashSet<string> obtainedItems = new HashSet<string>();
        public PlayerObtainedItems()
        {
        }

        
    }
}
