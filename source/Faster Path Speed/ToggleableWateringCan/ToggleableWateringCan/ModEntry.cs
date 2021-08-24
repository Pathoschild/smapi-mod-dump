/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/alanperrow/StardewModding
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace ToggleableWateringCan
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            if (Context.IsWorldReady)
            {
                helper.Events.Input.ButtonPressed += TestMethod;
            }
        }

        private void TestMethod(object sender, ButtonPressedEventArgs e)
        {
            Monitor.Log("test", LogLevel.Debug);
        }
    }
}
