/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/TestModWorkflow
**
*************************************************/

using StardewModdingAPI;

namespace TestMod
{
    public class TestMod : StardewModdingAPI.Mod
    {
        public override void Entry(IModHelper helper)
        {
            Monitor.Log("Meow!", LogLevel.Info);
        }
    }
}
