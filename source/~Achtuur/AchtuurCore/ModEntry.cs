/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewTravelSkill
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AchtuurCore.Events;
using AchtuurCore.Patches;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace AchtuurCore
{
    internal class ModEntry : Mod
    {
        public static ModEntry Instance;
        public override void Entry(IModHelper helper)
        {
            ModEntry.Instance = this;

            HarmonyPatcher.ApplyPatches(this,
                new WateringPatcher()
            );

            EventPublisher.onFinishedWateringSoil += this.test_wateringevent;
        }

        private void test_wateringevent(object sender, WateringFinishedArgs e)
        {
            Instance.Monitor.Log($"{e.farmer} just watered {e.target}", LogLevel.Trace);
        }
        
    }
}
