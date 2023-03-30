/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile.Dimensions;

namespace DynamicDialogues.Framework
{
    internal class EventStarter
    {
        internal static void Call(string[] values)
        {
            try
            {

                if (values[0] != "none")
                {
                    Game1.player.addQuest(int.Parse(values[0]));
                }

                if (values[1] == "none")
                {
                    return;
                }

                foreach (GameLocation location4 in Game1.locations)
                {
                    string text16 = location4.Name;
                    if (text16 == "Pool")
                    {
                        text16 = "BathHouse_Pool";
                    }

                    Dictionary<string, string> location_events = null;
                    try
                    {
                        location_events = Game1.content.Load<Dictionary<string, string>>("Data\\Events\\" + text16);
                    }
                    catch (Exception)
                    {
                        continue;
                    }

                    if (location_events == null)
                    {
                        continue;
                    }

                    foreach (string key in location_events.Keys)
                    {
                        string[] array = key.Split('/');
                        if (!(array[0] == values[1]))
                        {
                            continue;
                        }

                        int event_id = -1;
                        if (int.TryParse(array[0], out event_id))
                        {
                            while (Game1.player.eventsSeen.Contains(event_id))
                            {
                                Game1.player.eventsSeen.Remove(event_id);
                            }
                        }

                        LocationRequest obj3 = Game1.getLocationRequest(text16);
                        obj3.OnLoad += delegate
                        {
                            Game1.player.currentLocation.currentEvent = new Event(location_events[key], event_id);
                        };

                        int x2 = 8;
                        int y2 = 8;
                        Utility.getDefaultWarpLocation(obj3.Name, ref x2, ref y2);
                        Game1.warpFarmer(obj3, x2, y2, Game1.player.FacingDirection);
                        ModEntry.Mon.Log("Starting event " + key);
                    }
                }

                Game1.debugOutput = ("Event not found.");
            }
            catch(Exception ex)
            {
                ModEntry.Mon.Log("Error: " + ex, StardewModdingAPI.LogLevel.Error);
            }
        }
    }
}
