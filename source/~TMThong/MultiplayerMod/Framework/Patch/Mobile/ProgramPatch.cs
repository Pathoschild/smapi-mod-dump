/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TMThong/Stardew-Mods
**
*************************************************/

using StardewValley;
using StardewValley.Network;
using StardewValley.SDKs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerMod.Framework.Patch.Mobile
{
    internal class ProgramPatch
    {
        public static SDKHelper sdk
        {
            get
            {
                return ModUtilities.Helper.Reflection.GetField<SDKHelper>(typeof(Program), "sdk").GetValue();
            }
            set
            {
                ModUtilities.Helper.Reflection.GetField<SDKHelper>(typeof(Program), "sdk").SetValue(value);
            }
        }       
    }
}
