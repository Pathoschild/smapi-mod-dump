/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lisyce/SDV_Allergies_Mod
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buffs;
using StardewValley.GameData.Buffs;
using StardewValley.GameData.Objects;
using System.Text.RegularExpressions;

namespace BZP_Allergies
{
    internal class Constants
    {
        public static readonly string ModDataRandom = "BarleyZP.BzpAllergies_Random";
        public static readonly string ModDataDiscovered = "BarleyZP.BzpAllergies_DiscoveredAllergies";
        public static readonly string ModDataHas = "BarleyZP.BzpAllergies_PlayerAllergies";
        public static readonly string ModDataMadeWith = "BarleyZP.BzpAllergies_MadeWith";

        public static readonly string ReactionDebuff = "BarleyZP.BzpAllergies_allergic_debuff";
        public static readonly string LactaseBuff = "BarleyZP.BzpAllergies_lactase_buff";

        public static readonly string ReactionEventId = "BarleyZP.BzpAllergies_had_allergic_reaction";

        public static readonly string AllergyReliefId = "BarleyZP.BzpAllergies_AllergyMedicine";
        public static readonly string LactasePillsId = "BarleyZP.BzpAllergies_LactasePills";
        public static readonly string PlantMilkId = "BarleyZP.BzpAllergies_PlantMilk";
        public static readonly string AllergyTeachBookId = "BarleyZP.BzpAllergies_AllergyTeachBook";
        public static readonly string AllergyCookbookId = "BarleyZP.BzpAllergies_AllergyCookbook";

        public static readonly string NpcReactionDialogueKey = "BarleyZP.BzpAllergies_farmer_allergic_reaction";
    }

    internal class AllergenManager
    {
        public static readonly Dictionary<string, AllergenModel> ALLERGEN_DATA = new();

        public static Dictionary<string, AllergenModel> ALLERGEN_DATA_ASSET => Game1.content.Load<Dictionary<string, AllergenModel>>("BarleyZP.BzpAllergies/AllergyData");

        public static void InitDefault()
        {
            ALLERGEN_DATA.Clear();
            ITranslationHelper translation = ModEntry.Instance.Translation;

            AllergenModel egg = new(translation.Get("allergies.egg"));
            egg.AddObjectIds(new HashSet<string>{
                    "194", "195", "201", "203", "211", "213", "220", "221", "223", "234", "240", "648",
                    "732"
                });
            egg.AddTags(new HashSet<string> { "egg_item", "mayo_item", "large_egg_item" });
            ALLERGEN_DATA["egg"] = egg;

            AllergenModel gluten = new(translation.Get("allergies.gluten"));
            gluten.AddObjectIds(new HashSet<string>{
                    "198", "201", "202", "203", "206", "211", "214", "216", "220", "221", "222", "223",
                    "224", "234", "239", "241", "604", "608", "611", "618", "651", "731", "732", "246",
                    "262", "346"
                });
            ALLERGEN_DATA["gluten"] = gluten;

            AllergenModel fish = new(translation.Get("allergies.fish"));
            fish.AddObjectIds(new HashSet<string>{
                    "198", "202", "204", "212", "213", "214", "219", "225", "226", "227", "228", "242",
                    "265", "445"
                });
            ALLERGEN_DATA["fish"] = fish;

            AllergenModel shellfish = new(translation.Get("allergies.shellfish"));
            shellfish.AddObjectIds(new HashSet<string>{
                    "203", "218", "227", "228", "727", "728", "729", "730", "732", "733", "715", "372",
                    "717", "718", "719", "720", "723", "716", "721", "722"
                });
            ALLERGEN_DATA["shellfish"] = shellfish;

            AllergenModel treenuts = new(translation.Get("allergies.treenuts"));
            treenuts.AddObjectIds(new HashSet<string>{
                    "239", "607", "408"
                });
            ALLERGEN_DATA["treenuts"] = treenuts;

            AllergenModel dairy = new(translation.Get("allergies.dairy"));
            dairy.AddObjectIds(new HashSet<string>{
                    "195", "197", "199", "201", "206", "215", "232", "233", "236", "240", "243", "605",
                    "608", "727", "730", "904", "424", "426"
                });
            dairy.AddTags(new HashSet<string> { "milk_item", "large_milk_item", "cow_milk_item", "goat_milk_item" });
            ALLERGEN_DATA["dairy"] = dairy;

            AllergenModel mushroom = new(translation.Get("allergies.mushroom"));
            mushroom.AddObjectIds(new HashSet<string>{
                    "404", "205", "606", "218", "420", "422", "281", "257", "773", "851"
                });
            ALLERGEN_DATA["mushroom"] = mushroom;
        }

        public static void ThrowIfAllergenDoesntExist(string allergen)
        {
            if (!ALLERGEN_DATA_ASSET.ContainsKey(allergen))
            {
                throw new Exception("No allergen found with Id " + allergen.ToString());
            }
        }

        public static string GetAllergenDisplayName(string allergen)
        {
            ThrowIfAllergenDoesntExist(allergen);
            return ALLERGEN_DATA_ASSET[allergen].DisplayName;
        }

        public static bool ModDataSetContains(IHaveModData obj, string key, string item)
        {
            return ModDataSetGet(obj, key).Contains(item);
        }

        // items cannot contain commas
        public static bool ModDataSetAdd(IHaveModData obj, string key, string item)
        {
            if (ModDataSetContains(obj, key, item)) return false;  // don't add duplicates
            item = item.Replace(",", "");  // sanitize

            if (ModDataGet(obj, key, out string val) && val.Length > 0)
            {
                obj.modData[key] = val + "," + item;
            }
            else
            {
                obj.modData[key] = item;
            }
            return true;
        }

        public static bool ModDataSetRemove(IHaveModData obj, string key, string item)
        {
            item = item.Replace(",", "");  // sanitize

            ISet<string> currVal = ModDataSetGet(obj, key);
            bool retVal = currVal.Remove(item);

            obj.modData[key] = string.Join(',', currVal);

            return retVal;
        }

        public static ISet<string> ModDataSetGet(IHaveModData obj, string key)
        { 
            if (ModDataGet(obj, key, out string val) && val.Length > 0)
            {
                return val.Split(',').ToHashSet();
            }
            return new HashSet<string>();
        }

        public static bool ModDataGet(IHaveModData obj, string key, out string val)
        {
            if (obj.modData.TryGetValue(key, out string datastr))
            {
                val = datastr;
                return true;
            }
            val = "";
            return false;
        }

        public static bool FarmerIsAllergic(string allergen)
        {
            ThrowIfAllergenDoesntExist(allergen);
            return ModDataSetContains(Game1.player, Constants.ModDataHas, allergen);
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

            if (madeFrom.Any())
            {
                foreach (StardewValley.Object madeFromObj in madeFrom)
                {
                    result.UnionWith(GetAllergensInObject(madeFromObj));
                }
            }
            // special case: cooked/milled/crafted (etc) item
            else if (@object.modData.ContainsKey(Constants.ModDataMadeWith))
            {
                // try looking in the modData field for what the thing was crafted with
                foreach (string allergenId in ModDataSetGet(@object, Constants.ModDataMadeWith))
                {
                    if (ALLERGEN_DATA_ASSET.ContainsKey(allergenId))  // allergy still exists
                    {
                        result.Add(allergenId);
                    }
                    else
                    {
                        ModDataSetRemove(@object, Constants.ModDataMadeWith, allergenId);  // clean up for future
                    }
                }
            }
            // else: boring normal item
            else
            {
                foreach (var allergenData in ALLERGEN_DATA_ASSET)
                {
                    // do we have this allergen?
                    if (allergenData.Value.ObjectIds.Contains(@object.ItemId))
                    {
                        result.Add(allergenData.Key);
                    }

                    // are any of this objects context tags in the context tags for the allergen?
                    if (allergenData.Value.ContextTags.Intersect(@object.GetContextTags()).Any())
                    {
                        result.Add(allergenData.Key);
                    }
                }

                // is it fish category (-4) and NOT a shellfish?
                if (@object.Category == StardewValley.Object.FishCategory &&
                    !ALLERGEN_DATA_ASSET["shellfish"].ObjectIds.Contains(@object.ItemId) &&
                    !ALLERGEN_DATA_ASSET["shellfish"].ContextTags.Intersect(@object.GetContextTags()).Any())
                {
                    result.Add("fish");
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

        public static List<string> RollRandomKAllergies(int k)
        {
            Random random = new();

            if (k == -1)
            {
                // generate k from binomial distribution with p = 0.5
                k = 1;
                
                int trials = ALLERGEN_DATA_ASSET.Count - 1;
                for (int i = 0; i < trials; i++)
                {
                    if (random.NextDouble() < 0.5)
                    {
                        k++;
                    }
                }
            }

            // select k random allergens
            List<string> result = new();
            List<string> possibleAllergies = ALLERGEN_DATA_ASSET.Keys.ToList();
            for (int i = 0; i < k; i++)
            {
                int idx = random.Next(possibleAllergies.Count);
                result.Add(possibleAllergies[idx]);
                possibleAllergies.RemoveAt(idx);
            }

            return result;
        }

        public static bool PlayerHasDiscoveredAllergy(string allergyId)
        {
            return ModDataSetContains(Game1.player, Constants.ModDataDiscovered, allergyId);
        }

        public static void TogglePlayerHasAllergy(string allergyId, bool has)
        {
            if (!has)
            {
                ModDataSetRemove(Game1.player, Constants.ModDataHas, allergyId);
            }
            else
            {
                ModDataSetAdd(Game1.player, Constants.ModDataHas, allergyId);
            }
        }

        public static Buff GetAllergicReactionBuff(string itemSource, string actionSource, int durationSeconds)
        {
            Texture2D sprites = Game1.content.Load<Texture2D>("BarleyZP.BzpAllergies/Sprites");

            float mult = ModEntry.Instance.Config.DebuffSeverityMultiplier;
            if (mult <= 0) mult = 1;

            BuffAttributesData buffAttributesData = new()
            {
                Speed = -2 * mult,
                Defense = -1 * mult,
                Attack = -1 * mult
            };

            BuffEffects effects = new(buffAttributesData);

            string desc = actionSource switch
            {
                "consume" => ModEntry.Instance.Translation.Get("debuff.consume"),
                "hold" => ModEntry.Instance.Translation.Get("debuff.hold"),
                "cook" => ModEntry.Instance.Translation.Get("debuff.cook"),
                _ => throw new NotImplementedException()
            };

            Buff reactionBuff = new(Constants.ReactionDebuff, "food", itemSource,
                durationSeconds * 1000, sprites, 2, effects,
                true, ModEntry.Instance.Translation.Get("debuff.name"), desc)
            {
                glow = Microsoft.Xna.Framework.Color.Green
            };

            return reactionBuff;
        }

        public static void CheckForAllergiesToDiscover(Farmer farmer, ISet<string> allergens)
        {
            if (!farmer.mailReceived.Contains(ModEntry.MOD_ID + "_harvey_ad"))
            {
                Game1.addMailForTomorrow(ModEntry.MOD_ID + "_harvey_ad");
            }

            // discover allergies
            foreach (string allergen in allergens)
            {
                if (FarmerIsAllergic(allergen) && DiscoverPlayerAllergy(allergen))
                {
                    Game1.showGlobalMessage(ModEntry.Instance.Translation.Get("books.allergy-teach"));
                    Game1.playSound("newArtifact");
                    break;
                }
            }
        }

        public static bool DiscoverPlayerAllergy(string allergyId)
        {
            if (ModDataGet(Game1.player, Constants.ModDataRandom, out string val) && val == "true")
            {
                return ModDataSetAdd(Game1.player, Constants.ModDataDiscovered, allergyId);
            }

            return false;
        }

        public static IEnumerable<string> ReadAllergyCookbookToken()
        {
            List<string> result = new();

            if (!Context.IsWorldReady || Game1.player.stats.Get(Constants.AllergyCookbookId) == 0)
            {
                result.Add("false");
            }
            else {
                result.Add("true");
            }

            return result;
        }
    }

    internal class AllergenModel
    {
        public List<string> ObjectIds { get; set; }
        public List<string> ContextTags { get; set; }
        public string DisplayName { get; set; }

        public string? AddedByContentPackId { get; set; }

        public AllergenModel (string displayName, string? addedByContentPackId = null)
        {
            ObjectIds = new();
            ContextTags = new();
            DisplayName = displayName;
            AddedByContentPackId = addedByContentPackId;
        }

        public void AddObjectIds (IEnumerable<string> ids)
        {
            foreach (string id in ids)
            {
                ObjectIds.Add(id);
            }
        }

        public void AddTags(IEnumerable<string> tags)
        {
            foreach (string tag in tags)
            {
                ContextTags.Add(tag);
            }
        }
    }
}
