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
using Microsoft.Xna.Framework;
using Pathoschild.Stardew.Automate;
using StardewValley;

namespace Omegasis.RevitalizeAutomateCompatibility.MachineWrappers
{
    public class BaseMachineWrapper : IMachine
    {
        public virtual string MachineTypeID => throw new NotImplementedException();

        public virtual GameLocation Location {get;set;}

        public virtual Rectangle TileArea {get;set;}

    public virtual ITrackedStack GetOutput()
        {
            throw new NotImplementedException();
        }

        public virtual MachineState GetState()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// True returns if something happenes for input. False returns that nothing happened.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual bool SetInput(IStorage input)
        {
            throw new NotImplementedException();
        }
    }
}
