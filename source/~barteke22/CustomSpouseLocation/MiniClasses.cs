/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/barteke22/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using System.Collections.Generic;

namespace CustomSpouseLocation
{
    class DictionaryEditor
    {
        public Dictionary<string, List<string>> enabledNPCs;
        public Dictionary<string, Rectangle> hoverNames = new Dictionary<string, Rectangle>();
        public Dictionary<string, string> dataStrings = new Dictionary<string, string>();
        public string dataEditing = null;
        public int dataIndex = 0;
        public Vector2 scrollBar;
        public Rectangle boundsLeftRight;
        public Rectangle boundsTopBottom;
        public int scrollState;
        public int contentBottom;

        public DictionaryEditor(Dictionary<string, List<KeyValuePair<string, Vector2>>> dictionary, int which)
        {
            enabledNPCs = new Dictionary<string, List<string>>();
            foreach (var npc in dictionary)
            {
                for (int i = 0; i < npc.Value.Count; i++)
                {
                    if (which == 0) dataStrings[npc.Key + "_" + i] = npc.Value[i].Value.X + "," + npc.Value[i].Value.Y;
                    else dataStrings[npc.Key + "_" + i] = npc.Value[i].Key + "/" + npc.Value[i].Value.X + "," + npc.Value[i].Value.Y;

                    if (enabledNPCs.ContainsKey(npc.Key)) enabledNPCs[npc.Key].Add(npc.Key + "_" + i);
                    else enabledNPCs[npc.Key] = new List<string>() { npc.Key + "_" + i };
                }
            }
            scrollState = Game1.input.GetMouseState().ScrollWheelValue;
        }
    }
}
