/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lisyce/SDV_Allergies_Mod
**
*************************************************/

using StardewValley.GameData.Objects;
using StardewModdingAPI;
using System.Text.RegularExpressions;
using StardewValley;

namespace BZP_Allergies
{
    internal class AllergenManager : Initializable
    {
        public static readonly string ALLERIC_REACTION_DEBUFF = string.Format("{0}_allergic_reaction", ModEntry.MOD_ID);
        public static readonly string LACTASE_PILLS_BUFF = string.Format("{0}_buff_2", ModEntry.MOD_ID);
        public static readonly string REACTION_EVENT = string.Format("{0}_had_allergic_reaction", ModEntry.MOD_ID);

        public static readonly string ALLERGY_RELIEF_ID = string.Format("{0}_AllergyMedicine", ModEntry.MOD_ID);
        public static readonly string LACTASE_PILLS_ID = string.Format("{0}_LactasePills", ModEntry.MOD_ID);

        public static readonly string REACTION_DIALOGUE_KEY = string.Format("{0}_farmer_allergic_reaction", ModEntry.MOD_ID);

        public static Dictionary<string, ISet<string>> ALLERGEN_OBJECTS;

        public static Dictionary<string, string> ALLERGEN_TO_DISPLAY_NAME;

        public static Dictionary<string, ISet<string>> ALLERGEN_CONTEXT_TAGS;

        public static Dictionary<string, ISet<string>> ALLERGEN_CONTENT_PACK;

        public static void InitDefaultDicts()
        {
            ALLERGEN_OBJECTS = new()
            {
                { "egg", new HashSet<string>{
                    "194", "195", "201", "203", "211", "213", "220", "221", "223", "234", "240", "648",
                    "732"
                }},
                { "wheat", new HashSet<string>{
                    "198", "201", "202", "203", "206", "211", "214", "216", "220", "221", "222", "223",
                    "224", "234", "239", "241", "604", "608", "611", "618", "651", "731", "732", "246",
                    "262", "346"
                }},
                { "fish", new HashSet<string>{
                    "198", "202", "204", "212", "213", "214", "219", "225", "226", "227", "228", "242",
                    "265", "445"
                }},
                { "shellfish", new HashSet<string>{
                    "203", "218", "227", "228", "727", "728", "729", "730", "732", "733", "715", "372",
                    "717", "718", "719", "720", "723", "716", "721", "722"
                }},
                { "treenuts", new HashSet<string>{
                    "239", "607", "408"
                }},
                { "dairy", new HashSet<string>{
                    "195", "197", "199", "201", "206", "215", "232", "233", "236", "240", "243", "605",
                    "608", "727", "730", "904", "424", "426"
                }},
                { "mushroom", new HashSet<string>
                {
                    "404", "205", "606", "218", "420", "422", "281", "257", "773", "851"
                }}
            };

            ALLERGEN_TO_DISPLAY_NAME = new()
            {
                { "egg", "Eggs" },
                { "wheat", "Wheat" },
                { "fish", "Fish" },
                { "shellfish", "Shellfish" },
                { "treenuts", "Tree Nuts" },
                { "dairy", "Dairy" },
                { "mushroom", "Mushrooms" }
            };

            ALLERGEN_CONTEXT_TAGS = new()
            {
                { "egg", new HashSet<string>{ "egg_item", "mayo_item", "large_egg_item" } },
                { "wheat", new HashSet<string>() },
                { "fish", new HashSet<string>() },
                { "shellfish", new HashSet<string>() },
                { "treenuts", new HashSet<string>() },
                { "dairy", new HashSet<string>{ "milk_item", "large_milk_item", "cow_milk_item", "goat_milk_item" } },
                { "mushroom", new HashSet<string>() }
            };

            ALLERGEN_CONTENT_PACK = new();
        }

        public static void ThrowIfAllergenDoesntExist(string allergen)
        {
            if (!ALLERGEN_TO_DISPLAY_NAME.ContainsKey(allergen))
            {
                throw new Exception("No allergen found named " + allergen.ToString());
            }
        }

        public static string GetAllergenContextTag(string allergen)
        {
            ThrowIfAllergenDoesntExist(allergen);
            return ModEntry.MOD_ID + "_allergen_" + allergen.ToLower();
        }

        public static string GetMadeWithContextTag(string allergen)
        {
            ThrowIfAllergenDoesntExist(allergen);
            return ModEntry.MOD_ID + "_made_with_id_" + allergen.ToLower();
        }

        public static string GetAllergenDisplayName(string allergen)
        {
            ThrowIfAllergenDoesntExist(allergen);
            return ALLERGEN_TO_DISPLAY_NAME[allergen];
        }

        public static ISet<string> GetObjectsWithAllergen(string allergen, IAssetDataForDictionary<string, ObjectData> data)
        {
            ThrowIfAllergenDoesntExist(allergen);

            // labeled items
            ISet<string> result = ALLERGEN_OBJECTS.GetValueOrDefault(allergen, new HashSet<string>());

            // fish special case
            if (allergen == "fish")
            {
                ISet<string> fishItems = GetFishItems(data);
                result.UnionWith(fishItems);
            }

            ISet<string> items = GetItemsWithContextTags(ALLERGEN_CONTEXT_TAGS.GetValueOrDefault(allergen, new HashSet<string>()), data);
            result.UnionWith(items);

            return result;
        }

        public static bool FarmerIsAllergic(string allergen)
        {
            ThrowIfAllergenDoesntExist(allergen);
            return ModEntry.Config.Farmer.Allergies.GetValueOrDefault(allergen, false);
        }

        public static bool FarmerIsAllergic (StardewValley.Object @object)
        {
            ISet<string> containsAllergens = GetAllergensInObject(@object);

            foreach (string a in containsAllergens)
            {
                if (FarmerIsAllergic(a))
                {
                    return true;
                }
            }
            return false;
        }

        public static ISet<string> GetAllergensInObject(StardewValley.Object? @object)
        {
            ISet<string> result = new HashSet<string>();
            if (@object == null)
            {
                return result;
            }

            // special case: preserves item
            List<StardewValley.Object> madeFrom = TryGetMadeFromObjects(@object);

            if (madeFrom.Count > 0)
            {
                foreach (StardewValley.Object madeFromObj in madeFrom)
                {
                    foreach (var tag in madeFromObj.GetContextTags())
                    {
                        if (tag.StartsWith(ModEntry.MOD_ID + "_allergen_"))
                        {
                            result.Add(tag.Split("_").Last());
                        }
                    }
                }
            }
            // special case: cooked item
            else if (@object.modData.TryGetValue("BarleyZP.BzpAllergies_CookedWith", out string cookedWith))
            {
                // try looking in the modData field for what the thing was crafted with
                foreach (string allergen in cookedWith.Split(","))
                {
                    result.Add(allergen);
                }
            }
            // else: boring normal item
            else
            {
                foreach (var tag in @object.GetContextTags())
                {
                    if (tag.StartsWith(ModEntry.MOD_ID + "_allergen_"))
                    {
                        result.Add(tag.Split("_").Last());
                    }
                }
            }

            return result;
        }

        public static List<StardewValley.Object> TryGetMadeFromObjects(StardewValley.Object @object)
        {
            List<StardewValley.Object> result = new();

            // get context tags
            ISet<string> tags = @object.GetContextTags();

            // find the "preserve_sheet_index_{id}" tag
            Regex rx = new(@"^preserve_sheet_index_\d+$");
            List<string> filteredTags = tags.Where(t => rx.IsMatch(t)).ToList();

            if (filteredTags.Count == 0)  // no preserves index
            {
                return result;
            }

            foreach (string tag in filteredTags)
            {
                // get the id of the object it was made from
                Match m = Regex.Match(tag, @"\d+");
                if (m.Success)
                {
                    string madeFromId = m.Value;
                    if (ItemRegistry.Create(madeFromId) is StardewValley.Object casted)
                    {
                        result.Add(casted);
                    }
                }
            }
            return result;
        }

        private static ISet<string> GetFishItems (IAssetDataForDictionary<string, ObjectData> data)
        {
            ISet<string> result = new HashSet<string>();

            foreach (var item in data.Data)
            {
                ObjectData v = item.Value;
                if (v.Category == StardewValley.Object.FishCategory)
                {
                    result.Add(item.Key);
                }
            }

            // remove shellfish
            ISet<string> shellfish = ALLERGEN_OBJECTS.GetValueOrDefault("shellfish", new HashSet<string>());
            
            foreach (var item in data.Data)
            {
                List<string> tags = item.Value.ContextTags ?? new();
                if (shellfish.Contains(item.Key) || tags.Contains(GetAllergenContextTag("shellfish")))
                {
                    result.Remove(item.Key);
                }
            }

            return result;
        }

        private static ISet<string> GetItemsWithContextTags (ISet<string> tags, IAssetDataForDictionary<string, ObjectData> data)
        {
            ISet<string> result = new HashSet<string>();

            foreach (var item in data.Data)
            {
                ObjectData v = item.Value;
                foreach (string tag in tags)
                {
                    if (v.ContextTags != null && v.ContextTags.Contains(tag))
                    {
                        result.Add(item.Key);
                    }
                }
            }

            return result;
        }
    }
}
