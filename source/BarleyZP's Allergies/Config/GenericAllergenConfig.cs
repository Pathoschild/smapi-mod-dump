/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lisyce/SDV_Allergies_Mod
**
*************************************************/

namespace BZP_Allergies.Config
{
    internal class GenericAllergenConfig
    {
        public Dictionary<string, bool> Allergies;

        public GenericAllergenConfig()
        {
            Allergies = new();

            // get content pack configurations
            foreach (string id in AllergenManager.ALLERGEN_TO_DISPLAY_NAME.Keys)
            {
                Allergies.Add(id, false);
            }
        }
    }
}
