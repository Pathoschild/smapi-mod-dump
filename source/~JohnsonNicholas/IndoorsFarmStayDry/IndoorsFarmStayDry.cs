/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JohnsonNicholas/SDVMods
**
*************************************************/

using System.Linq;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace IndoorsFarmStayDry
{
    public class IndoorsFarmStayDry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;
        }

        private void GameLoop_DayStarted(object sender, StardewModdingAPI.Events.DayStartedEventArgs e)
        {
            if (!Game1.isRaining || !Game1.IsMasterGame)
                return;

            foreach (var l in Game1.locations)
            {
                if (l.terrainFeatures.Pairs.Any() && l is Farm && l.map.Properties.ContainsKey("Underground") && l.map.Properties["Underground"] == "true")
                {
                    foreach (var k in l.terrainFeatures.Pairs)
                    {
                        if (k.Value is HoeDirt dirt)
                        {
                            dirt.state.Value = HoeDirt.dry;
                        }
                    }

                    //rewater
                    foreach (var o in l.objects.Pairs)
                    {
                        Monitor.Log($"Parent Sheet Index is {o.Value.ParentSheetIndex}", LogLevel.Info);
                        if (o.Value.ParentSheetIndex == 621 || o.Value.ParentSheetIndex == 599 ||
                            o.Value.ParentSheetIndex == 645 || o.Value.ParentSheetIndex == 1113)
                        {
                            o.Value.DayUpdate(l);
                        }
                    }
                }
            }
        }
    }
}
