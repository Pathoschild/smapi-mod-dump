/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using StardewValley.Tools;

namespace DeluxeJournal.Util
{
    /// <summary>Provides a means of querying game objects/data by their corresponding localized display names.</summary>
    public class LocalizedObjects
    {
        private readonly IDictionary<string, string> _items;
        private readonly IDictionary<string, string> _npcs;
        private readonly IDictionary<string, ToolDescription> _tools;
        private readonly IDictionary<string, BlueprintInfo> _blueprints;

        public LocalizedObjects(ITranslationHelper translation)
        {
            _items = CreateItemMap(translation.LocaleEnum == LocalizedContentManager.LanguageCode.en);
            _npcs = CreateNPCMap();
            _tools = CreateToolMap();
            _blueprints = CreateBlueprintMap();
        }

        /// <summary>Get an Item by display name.</summary>
        /// <param name="localizedName">Localized display name (the one that appears in-game).</param>
        /// <param name="fuzzy">Perform a fuzzy search if true, otherwise only return an Item with the exact name.</param>
        public Item? GetItem(string localizedName, bool fuzzy = false)
        {
            localizedName = localizedName.Trim().ToLowerInvariant();

            if (_tools.ContainsKey(localizedName))
            {
                return ToolHelper.GetToolFromDescription(_tools[localizedName].index, _tools[localizedName].upgradeLevel);
            }
            else if (GetValue(_items, localizedName, fuzzy) is string item)
            {
                return Utility.getItemFromStandardTextDescription(item, null);
            }

            return null;
        }

        /// <summary>Get an NPC by display name.</summary>
        /// <param name="localizedName">Localized display name (the one that appears in-game).</param>
        /// <param name="fuzzy">Perform a fuzzy search if true, otherwise only return the NPC with the exact name.</param>
        public NPC? GetNPC(string localizedName, bool fuzzy = false)
        {
            if (GetValue(_npcs, localizedName, fuzzy) is string npc)
            {
                return Game1.getCharacterFromName(npc);
            }

            return null;
        }

        /// <summary>Get BlueprintInfo given the display name.</summary>
        /// <param name="localizedName">Localized display name (the one that appears in-game).</param>
        /// <param name="fuzzy">Perform a fuzzy search if true, otherwise only match a blueprint with the exact name.</param>
        public BlueprintInfo? GetBlueprintInfo(string localizedName, bool fuzzy = false)
        {
            return GetValue(_blueprints, localizedName, fuzzy);
        }

        private static T? GetValue<T>(IDictionary<string, T> map, string key, bool fuzzy) where T : class
        {
            key = key.Trim().ToLowerInvariant();
            key = fuzzy ? Utility.fuzzySearch(key, map.Keys.ToList()) : key;

            if (key != null && map.ContainsKey(key))
            {
                return map[key];
            }

            return null;
        }

        private static IDictionary<string, string> CreateItemMap(bool isLocaleEnglish)
        {
            IDictionary<int, string> furnitureData = Game1.content.Load<Dictionary<int, string>>("Data\\Furniture");
            IDictionary<int, string> weaponData = Game1.content.Load<Dictionary<int, string>>("Data\\weapons");
            IDictionary<int, string> bootsData = Game1.content.Load<Dictionary<int, string>>("Data\\Boots");
            IDictionary<int, string> hatsData = Game1.content.Load<Dictionary<int, string>>("Data\\hats");
            IDictionary<string, string> map = new Dictionary<string, string>();
            string text;
            string[] values;

            foreach (int key in Game1.objectInformation.Keys)
            {
                if ((text = Game1.objectInformation[key]) != null)
                {
                    values = text.Split('/');

                    if (values[0] != "Weeds" && (values[0] != "Stone" || key == SObject.stone))
                    {
                        map[values[4].ToLowerInvariant()] = (values[3] == "Ring" ? "R " : "O ") + key + " 1";
                    }
                }
            }

            foreach (int key in Game1.bigCraftablesInformation.Keys)
            {
                if ((text = Game1.bigCraftablesInformation[key]) != null)
                {
                    values = text.Split('/');

                    if (CraftingRecipe.craftingRecipes.ContainsKey(values[0]))
                    {
                        map[values[isLocaleEnglish ? 0 : values.Length - 1].Trim().ToLowerInvariant()] = "BO " + key + " 1";
                    }
                }
            }

            foreach (int key in Game1.clothingInformation.Keys)
            {
                if ((text = Game1.clothingInformation[key]) != null)
                {
                    map[text.Split('/')[isLocaleEnglish ? 0 : 1].ToLowerInvariant()] = "C " + key + " 1";
                }
            }

            foreach (int key in furnitureData.Keys)
            {
                if ((text = furnitureData[key]) != null)
                {
                    values = text.Split('/');
                    map[values[isLocaleEnglish ? 0 : values.Length - 1].Trim().ToLowerInvariant()] = "F " + key + " 1";
                }
            }

            foreach (int key in weaponData.Keys)
            {
                if ((text = weaponData[key]) != null)
                {
                    values = text.Split('/');
                    map[values[isLocaleEnglish ? 0 : values.Length - 1].Trim().ToLowerInvariant()] = "W " + key + " 1";
                }
            }

            foreach (int key in bootsData.Keys)
            {
                if ((text = bootsData[key]) != null)
                {
                    values = text.Split('/');
                    map[values[isLocaleEnglish ? 0 : values.Length - 1].Trim().ToLowerInvariant()] = "B " + key + " 1";
                }
            }

            foreach (int key in hatsData.Keys)
            {
                if ((text = hatsData[key]) != null)
                {
                    values = text.Split('/');
                    map[values[isLocaleEnglish ? 0 : values.Length - 1].Trim().ToLowerInvariant()] = "H " + key + " 1";
                }
            }

            return map;
        }

        private static IDictionary<string, ToolDescription> CreateToolMap()
        {
            IDictionary<string, ToolDescription> map = new Dictionary<string, ToolDescription>();
            Tool[] tools = {
                new Axe(),
                new Hoe(),
                new Pickaxe(),
                new WateringCan(),
                new FishingRod(),
                new Pan(),
                new Shears(),
                new MilkPail(),
                new Wand()
            };

            foreach (Tool tool in tools)
            {
                int maxLevel = 0;

                switch (tool.GetType().Name)
                {
                    case nameof(Axe):
                    case nameof(Hoe):
                    case nameof(Pickaxe):
                    case nameof(WateringCan):
                        maxLevel = Tool.iridium;
                        break;
                    case nameof(FishingRod):
                        maxLevel = 3;
                        break;
                }

                for (int level = 0; level <= maxLevel; level++)
                {
                    tool.UpgradeLevel = level;
                    map[tool.DisplayName.ToLowerInvariant()] = ToolHelper.GetToolDescription(tool);
                }
            }

            return map;
        }

        private static IDictionary<string, string> CreateNPCMap()
        {
            IDictionary<string, string> npcData = Game1.content.Load<Dictionary<string, string>>("Data\\NPCDispositions");
            IDictionary<string, string> map = new Dictionary<string, string>();

            foreach (KeyValuePair<string, string> pair in npcData)
            {
                map[pair.Value.Split('/')[11].ToLowerInvariant()] = pair.Key;
            }

            return map;
        }

        private static IDictionary<string, BlueprintInfo> CreateBlueprintMap()
        {
            IDictionary<string, string> blueprintData = Game1.content.Load<Dictionary<string, string>>("Data\\Blueprints");
            IDictionary<string, BlueprintInfo> map = new Dictionary<string, BlueprintInfo>();

            foreach (KeyValuePair<string, string> pair in blueprintData)
            {
                string[] fields = pair.Value.Split('/');

                if (fields[0] == "animal")
                {
                    map[fields[4].ToLowerInvariant()] = new BlueprintInfo(pair.Key, fields[4], "Animal", int.Parse(fields[1]));
                }
                else if (fields.Length > 17)
                {
                    map[fields[8].ToLowerInvariant()] = new BlueprintInfo(pair.Key, fields[8], fields[10], int.Parse(fields[17]));
                }
            }

            return map;
        }
    }
}
