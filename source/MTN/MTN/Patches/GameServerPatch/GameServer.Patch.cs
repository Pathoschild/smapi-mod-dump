using StardewValley;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using StardewValley.Locations;
using System.Reflection.Emit;

namespace MTN.Patches.GameServerPatch
{
    class GameServerPatch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {

            var codes = new List<CodeInstruction>(instructions);

            return codes.AsEnumerable();
        }
    }
}
