/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

using System;

namespace CustomFarmingRedux
{
    public class MachineHandler
    {
        public Func<StardewValley.Object, StardewValley.Object, string, string, StardewValley.Object> GetOutput { get; set; } = null;
        public Func<StardewValley.Object, StardewValley.Object, string, bool> CheckInput { get; set; } = null;
        public Action<StardewValley.Object> ClickAction { get; set; } = null;


        public MachineHandler(Func<StardewValley.Object, StardewValley.Object, string, string, StardewValley.Object> outputHandler = null, Func<StardewValley.Object, StardewValley.Object, string, bool> inputHandler = null, Action<StardewValley.Object> clickActionHandler = null)
        {
            GetOutput = outputHandler;
            CheckInput = inputHandler;
            ClickAction = clickActionHandler;
        }
    }
}
