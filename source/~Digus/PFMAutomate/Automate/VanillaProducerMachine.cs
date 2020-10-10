/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
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
using SObject = StardewValley.Object;

namespace PFMAutomate.Automate
{
    public class VanillaProducerMachine : IMachine
    {
        public string MachineTypeID { get; }

        public GameLocation Location { get; }
        public Rectangle TileArea { get; }

        public IMachine OriginalMachine;
        public IMachine PfmMachine;

        public VanillaProducerMachine(IMachine originalMachine)
        {
            Location = originalMachine.Location;
            TileArea = originalMachine.TileArea;
            this.OriginalMachine = originalMachine;
            SObject entity = PFMAutomateModEntry.Helper.Reflection.GetProperty<SObject>(originalMachine, "Machine").GetValue();
            this.PfmMachine = new CustomProducerMachine(entity, originalMachine.Location, entity.TileLocation);
            MachineTypeID = "PFM.Vanilla." + entity.Name;
        }

        public MachineState GetState()
        {
            return this.PfmMachine.GetState();
        }

        public ITrackedStack GetOutput()
        {
            return this.PfmMachine.GetOutput();
        }

        public bool SetInput(IStorage input)
        {
            return PfmMachine.SetInput(input) || OriginalMachine.SetInput(input);
        }
    }
}
