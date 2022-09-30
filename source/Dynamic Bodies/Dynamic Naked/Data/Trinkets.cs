/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ribeena/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewValley;


namespace DynamicBodies.Data
{
    public class Trinkets
    {

        
        public struct Settings
        {
            public Dictionary<string, Dictionary<int, int>> anim_frames;
            public bool usesUniqueLeftSprite;
            public int yOffset;
            public int extraWidth;
            public int[] layers;
            public bool primaryColor;
            public bool secondaryColor;
            public int cost;
            public string metadata;
        }

        public Dictionary<string, Settings> trinkets = new Dictionary<string, Settings>();

        public class ContentPackTrinketOption : ContentPackOption
        {

            public Settings settings;
            public ContentPackTrinketOption(string name, string file, string author, IContentPack contentPack, Settings settings)
                : base(name, file, author, contentPack)
            {
                this.settings = settings;
            }
        }

        public List<ContentPackOption> GetTrinketStyles(IContentPack contentPack, int layer)
        {
            List<ContentPackOption> options = new List<ContentPackOption>();

            foreach (KeyValuePair<string, Settings> kvp in trinkets)
            {
                if (contentPack.HasFile($"Trinkets\\{kvp.Key}.png")) {
                    ContentPackTrinketOption option = new ContentPackTrinketOption(kvp.Key, kvp.Key, contentPack.Manifest.Author, contentPack, kvp.Value);
                    if ((kvp.Value.layers.Length > 0 && kvp.Value.layers.Contains(layer)) || kvp.Value.layers.Length == 0)
                    {
                        options.Add(option);
                    }
                }
                else
                {
                    ModEntry.monitor.Log($"{contentPack.Manifest.Name} is missing a trinket file for '{kvp.Key}.png'", LogLevel.Debug);
                }
            }
            return options;
        }

    }
}
