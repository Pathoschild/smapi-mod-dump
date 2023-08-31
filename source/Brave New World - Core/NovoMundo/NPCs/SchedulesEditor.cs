/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DiogoAlbano/StardewValleyMods
**
*************************************************/

using StardewModdingAPI.Events;
using StardewValley;
using System.Collections.Generic;

namespace NovoMundo.NPCs
{
    public class Schedules_Editor
    {
        public void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            var vanillaNPCs = ModEntry.ModHelper.GameContent.Load<Dictionary<string, string>>("Data/NPCDispositions");
            foreach (string counter in vanillaNPCs.Keys)
            {
                var name = counter;
                NPCReset(name);
            }           
        }
        public void NPCReset(string name)
        {
            var npc = Game1.getCharacterFromName(name);
            if (npc != null)
            {
                if(name.Equals("Robin"))
                {
                    npc.shouldPlayRobinHammerAnimation.Value = false;
                    npc.ignoreScheduleToday = false;
                }
                npc.resetCurrentDialogue();
                npc.reloadDefaultLocation();
                Game1.warpCharacter(npc, npc.DefaultMap, npc.DefaultPosition / 64f);
            }          
        }
    }
}
