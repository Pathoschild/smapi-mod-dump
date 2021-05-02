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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley.Minigames;

namespace Revitalize.Framework.Objects.InformationFiles.Furniture
{
    public class ArcadeCabinetInformation
    {

        public IMinigame minigame;
        public bool freezeState;

        public ArcadeCabinetInformation()
        {
            this.minigame = null;
            this.freezeState = false;
        }
        public ArcadeCabinetInformation(IMinigame Minigame, bool FreezeState)
        {
            this.minigame = Minigame;
            this.freezeState = FreezeState;
        }
    }
}
