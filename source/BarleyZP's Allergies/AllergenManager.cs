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

namespace BZP_Allergies
{
    internal class AllergenManager : Initializable
    {
        public enum Allergens {
            EGG,
            WHEAT,
            FISH,
            SHELLFISH,
            TREE_NUTS,
            DAIRY
        }

        public static readonly string ALLERIC_REACTION_DEBUFF = string.Format("{0}_allergic_reaction", ModEntry.MOD_ID);
        public static readonly string LACTASE_PILLS_BUFF = string.Format("{0}_buff_2", ModEntry.MOD_ID);
        public static readonly string REACTION_EVENT = string.Format("{0}_had_allergic_reaction", ModEntry.MOD_ID);

        public static readonly string ALLERGY_RELIEF_ID = string.Format("{0}_AllergyMedicine", ModEntry.MOD_ID);
        public static readonly string LACTASE_PILLS_ID = string.Format("{0}_LactasePills", ModEntry.MOD_ID);

        public static readonly string REACTION_DIALOGUE_KEY = string.Format("{0}_farmer_allergic_reaction", ModEntry.MOD_ID);

        private static readonly Dictionary<Allergens, ISet<string>> ENUM_TO_ALLERGEN_OBJECTS = new()
        {
            { Allergens.EGG, new HashSet<string>{
                "194", "195", "201", "203", "211", "213", "220", "221", "223", "234", "240", "648",
                "732"
            }},
            { Allergens.WHEAT, new HashSet<string>{
                "198", "201", "202", "203", "206", "211", "214", "216", "220", "221", "222", "223",
                "224", "234", "239", "241", "604", "608", "611", "618", "651", "731", "732", "246",
                "262"
            }},
            { Allergens.FISH, new HashSet<string>{
                "198", "202", "204", "212", "213", "214", "219", "225", "226", "227", "228", "242",
                "265", "447", "445", "812", "SmokedFish"
            }},
            { Allergens.SHELLFISH, new HashSet<string>{
                "203", "218", "227", "228", "727", "728", "729", "730", "732", "733", "447", "812",
                "SmokedFish", "715", "372", "717", "718", "719", "720", "723", "716", "721", "722"
            }},
            { Allergens.TREE_NUTS, new HashSet<string>{
                "239", "607", "408"
            }},
            { Allergens.DAIRY, new HashSet<string>{
                "195", "197", "199", "201", "206", "215", "232", "233", "236", "240", "243", "605",
                "608", "727", "730", "904", "424", "426"
            }}
        };

        private static readonly Dictionary<Allergens, string> ENUM_TO_CONTEXT_TAG = new()
        {
            { Allergens.EGG, string.Format("{0}_egg", ModEntry.MOD_ID) },
            { Allergens.WHEAT, string.Format("{0}_wheat", ModEntry.MOD_ID) },
            { Allergens.FISH, string.Format("{0}_fish", ModEntry.MOD_ID) },
            { Allergens.SHELLFISH, string.Format("{0}_shellfish", ModEntry.MOD_ID) },
            { Allergens.TREE_NUTS, string.Format("{0}_treenuts", ModEntry.MOD_ID) },
            { Allergens.DAIRY, string.Format("{0}_dairy", ModEntry.MOD_ID) }
        };

        private static readonly Dictionary<Allergens, string> ENUM_TO_STRING = new()
        {
            { Allergens.EGG, "Eggs" },
            { Allergens.WHEAT, "Wheat" },
            { Allergens.FISH, "Fish" },
            { Allergens.SHELLFISH, "Shellfish" },
            { Allergens.TREE_NUTS, "Tree Nuts" },
            { Allergens.DAIRY, "Dairy" }
        };

        public static string GetAllergenContextTag(Allergens allergen)
        {
            string result = ENUM_TO_CONTEXT_TAG.GetValueOrDefault(allergen, "");
            if (result.Equals(""))
            {
                throw new Exception("No context tags were defined for the allergen " + allergen.ToString());
            }
            return result;
        }

        public static string GetAllergenReadableString(Allergens allergen)
        {
            string result = ENUM_TO_STRING.GetValueOrDefault(allergen, "");
            if (result.Equals(""))
            {
                throw new Exception("No readable string was defined for the allergen " + allergen.ToString());
            }
            return result;
        }

        public static ISet<string> GetObjectsWithAllergen(Allergens allergen, IAssetDataForDictionary<string, ObjectData> data)
        {
            // labeled items
            ISet<string> result = ENUM_TO_ALLERGEN_OBJECTS.GetValueOrDefault(allergen, new HashSet<string>());

            // category items
            if (allergen == Allergens.EGG)
            {
                ISet<string> rawEggItems = GetItemsWithContextTags(new List<string> { "egg_item", "mayo_item", "large_egg_item" }, data);
                result.UnionWith(rawEggItems);
            }
            else if (allergen == Allergens.FISH)
            {
                ISet<string> fishItems = GetFishItems(data);
                result.UnionWith(fishItems);
            }
            else if (allergen == Allergens.DAIRY)
            {
                ISet<string> dairyItems = GetItemsWithContextTags(new List<string> { "milk_item", "large_milk_item", "cow_milk_item", "goat_milk_item" }, data);
                result.UnionWith(dairyItems);
            }

            if (result.Count == 0)
            {
                throw new Exception("No objects have been assigned the allergen " + allergen.ToString());
            }
            return result;

            
        }
        public static bool FarmerIsAllergic(Allergens allergen)
        {
            switch (allergen)
            {
                case Allergens.EGG:
                    return ModEntry.Config.Farmer.EggAllergy;
                case Allergens.WHEAT:
                    return ModEntry.Config.Farmer.WheatAllergy;
                case Allergens.FISH:
                    return ModEntry.Config.Farmer.FishAllergy;
                case Allergens.SHELLFISH:
                    return ModEntry.Config.Farmer.ShellfishAllergy;
                case Allergens.TREE_NUTS:
                    return ModEntry.Config.Farmer.TreenutAllergy;
                case Allergens.DAIRY:
                    return ModEntry.Config.Farmer.DairyAllergy;
                default:
                    return false;
            }
        }

        public static bool FarmerIsAllergic (StardewValley.Object @object)
        {
            // special case: roe, aged roe, or smoked fish
            // need to differentiate fish vs shellfish ingredient
            List<string> fishShellfishDifferentiation = new() { "(O)447", "(O)812", "(O)SmokedFish" };
            if (fishShellfishDifferentiation.Contains(@object.QualifiedItemId))
            {
                try
                {
                    // get context tags
                    ISet<string> tags = @object.GetContextTags();

                    // find the "preserve_sheet_index_{id}" tag
                    Regex rx = new(@"^preserve_sheet_index_\d+$");
                    List<string> filtered_tags = tags.Where(t => rx.IsMatch(t)).ToList();
                    string preserve_sheet_tag = filtered_tags[0];

                    // get the id of the object it was made from
                    Match m = Regex.Match(preserve_sheet_tag, @"\d+");
                    if (!m.Success)
                    {
                        throw new Exception("No regex match for item id in preserve_sheet_index context tag");
                    }

                    string madeFromId = m.Value;
                    // load Data/Objects for context tags
                    IDictionary<string, ObjectData> objData = GameContent.Load<Dictionary<string, ObjectData>>("Data/Objects");

                    // !isShellfish = isFish since these can only be made from one of the two
                    bool isShellfish = objData[madeFromId].ContextTags.Contains(GetAllergenContextTag(Allergens.SHELLFISH));

                    if (isShellfish && FarmerIsAllergic(Allergens.SHELLFISH))
                    {
                        return true;
                    }
                    else
                    {
                        return !isShellfish && FarmerIsAllergic(Allergens.FISH);
                    }
                }
                catch (Exception ex)
                {
                    Monitor.Log($"Failed in {nameof(FarmerIsAllergic)}:\n{ex}", LogLevel.Error);
                    Monitor.Log("Unable to determine whether eaten Object was fish or shellfish");
                    // we failed to determine, so let's just fall through and
                    // return whether the farmer is allergic to fish or shellfish
                }
            }

            // check each of the allergens
            foreach (Allergens a in Enum.GetValues<Allergens>())
            {
                if (@object.HasContextTag(GetAllergenContextTag(a)) && FarmerIsAllergic(a))
                {
                    return true;
                }
            }

            return false;
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
            ISet<string> shellfish = ENUM_TO_ALLERGEN_OBJECTS.GetValueOrDefault(Allergens.SHELLFISH, new HashSet<string>());
            
            foreach (var item in data.Data)
            {
                List<string> tags = item.Value.ContextTags ?? new();
                if (shellfish.Contains(item.Key) || tags.Contains(GetAllergenContextTag(Allergens.SHELLFISH)))
                {
                    result.Remove(item.Key);
                }
            }

            return result;
        }

        private static ISet<string> GetItemsWithContextTags (List<string> tags, IAssetDataForDictionary<string, ObjectData> data)
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
