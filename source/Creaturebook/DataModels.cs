/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KediDili/Creaturebook
**
*************************************************/

using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using Microsoft.Xna.Framework;

namespace Creaturebook
{
    public class ModConfig
    {
        public bool ShowScientificNames { get; set; } = true;
        public bool ShowDiscoveryDates { get; set; } = true;
        public KeybindList OpenMenuKeybind { get; set; } = KeybindList.Parse("LeftControl + LeftShift + B");
        public string WayToGetNotebook { get; set; } = "Letter";
        public bool EnableStickies { get; set; } = true;
    }
    public class ModData
    {
        public IDictionary<string, SDate> DiscoveryDates { get; set; } = new Dictionary<string, SDate>();
        public bool IsNotebookObtained { get; set; } = false;
    }
    public struct Chapter
    {
        public enum Categories
        {
            Animals,
            Plants,
            Monsters,
            Magical,
            Other
        }
        public string Title;

        public string Folder;

        public string CreatureNamePrefix;

        public Categories Category;

        public IContentPack FromContentPack;

        public List<Creature> Creatures;

        public List<Set> Sets;

        public string Author;

        public bool EnableSets;
    }
    public struct Set
    {
        public string InternalName;

        public string DisplayNameKey;

        public int[] CreaturesBelongingToThisSet;

        public int DiscoverWithThisItem;

        public Vector2[] OffsetsInMenu;

        public float[] ScalesInMenu;

        public bool NeedsSecondPage;
    }
    public struct Creature
    {
        public int ID;
        
        public string Name;
        
        public string ScientificName;

        public Vector2[] ImageOffsets;

        public float[] ImageScales;

        public int UseThisItem;

        public string Desc;

        public string[] OverrideDefaultNaming;

        public string Directory;

        public bool HasExtraImages;

        public bool HasScientificName;

        public bool HasFunFact;
    }
}
