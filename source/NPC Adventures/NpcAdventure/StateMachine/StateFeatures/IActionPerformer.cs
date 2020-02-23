using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NpcAdventure.StateMachine.StateFeatures
{
    interface IActionPerformer
    {
        bool CanPerformAction { get; }
        bool PerformAction(Farmer who, GameLocation location);
    }
}
