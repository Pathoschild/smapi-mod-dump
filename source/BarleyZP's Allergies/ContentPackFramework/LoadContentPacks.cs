/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lisyce/SDV_Allergies_Mod
**
*************************************************/

using BZP_Allergies.Config;
using StardewModdingAPI;

namespace BZP_Allergies.ContentPackFramework
{
    internal class LoadContentPacks : Initializable
    {
        public static void LoadPacks(IEnumerable<IContentPack> packs, ModConfig config)
        {
            foreach (IContentPack contentPack in packs)
            {
                Monitor.Log($"Reading content pack: {contentPack.Manifest.Name} {contentPack.Manifest.Version}", LogLevel.Info);
                if (!ProcessPack(contentPack, config))
                {
                    Monitor.Log($"Unable to read content pack: {contentPack.Manifest.Name} {contentPack.Manifest.Version}", LogLevel.Error);
                }
            }
        }

        private static string SanitizeAllergenId(string id)
        {
            // remove "," "_" and lowercase
            return id.Replace(",", "").Replace("_", "").ToLower();
        }

        private static bool ProcessPack(IContentPack pack, ModConfig config)
        {
            ContentModel? content = pack.ReadJsonFile<ContentModel>("content.json");
            if (content == null)
            {
                // show 'required file missing' error
                Monitor.Log("Pack is missing a content.json, or it is empty.", LogLevel.Error);
                return false;
            }

            // check format
            if (content.Format == null || !content.Format.Equals("1.0.0"))
            {
                Monitor.Log("Valid content format was not specified. Valid formats are: \"1.0.0\"", LogLevel.Error);
                return false;
            }

            // custom allergens
            if (!AllergenManager.ALLERGEN_CONTENT_PACK.ContainsKey(pack.Manifest.UniqueID))
            {
                AllergenManager.ALLERGEN_CONTENT_PACK.Add(pack.Manifest.UniqueID, new HashSet<string>());
            }

            foreach (var pair in content.CustomAllergens)
            {
                CustomAllergen allergen = pair.Value;
                string allergenId = SanitizeAllergenId(pair.Key);

                if (allergen.Name == null)
                {
                    Monitor.Log("No Name was specified for allergen with Id " + allergenId, LogLevel.Error);
                    return false;
                }

                if (!AllergenManager.ALLERGEN_OBJECTS.ContainsKey(allergenId))
                {
                    AllergenManager.ALLERGEN_OBJECTS.Add(allergenId, new HashSet<string>());
                }

                if (!AllergenManager.ALLERGEN_TO_DISPLAY_NAME.ContainsKey(allergenId))
                {
                    AllergenManager.ALLERGEN_TO_DISPLAY_NAME.Add(allergenId, allergen.Name);
                }

                AllergenManager.ALLERGEN_CONTENT_PACK[pack.Manifest.UniqueID].Add(allergenId);

                if (!config.Farmer.Allergies.ContainsKey(allergenId))
                {
                    config.Farmer.Allergies.Add(allergenId, false);
                }
            }

            // allergen assignments
            foreach (var pair in content.AllergenAssignments)
            {
                AllergenAssignments allergenAssign = pair.Value;
                string allergenId = SanitizeAllergenId(pair.Key);

                if (allergenId == null)
                {
                    Monitor.Log("No AllergenId was specified for allergen assignment with Id " + allergenId, LogLevel.Error);
                    return false;
                }

                // object Ids
                foreach (string id in allergenAssign.ObjectIds)
                {
                    AllergenManager.ALLERGEN_OBJECTS[allergenId].Add(id);
                }

                // context tags
                if (allergenAssign.ContextTags.Count > 0 && !AllergenManager.ALLERGEN_CONTEXT_TAGS.ContainsKey(allergenId))
                {
                    AllergenManager.ALLERGEN_CONTEXT_TAGS.Add(allergenId, new HashSet<string>());
                }

                foreach (string tag in allergenAssign.ContextTags)
                {
                    AllergenManager.ALLERGEN_CONTEXT_TAGS[allergenId].Add(tag);
                }
            }

            return true;  // no errors :)
        }
    }
}
