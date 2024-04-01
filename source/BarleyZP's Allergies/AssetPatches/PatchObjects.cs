/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lisyce/SDV_Allergies_Mod
**
*************************************************/

using StardewModdingAPI.Events;
using StardewValley.GameData.Objects;
using static BZP_Allergies.AllergenManager;

namespace BZP_Allergies.AssetPatches
{
    internal sealed class PatchObjects
    {

        public static void AddAllergen(AssetRequestedEventArgs e, Allergens allergen)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Objects"))
            {
                bool allergic = FarmerIsAllergic(allergen);

                e.Edit(asset =>
                {
                    // update items containing allergens
                    var editor = asset.AsDictionary<string, ObjectData>();

                    ISet<string> idsToEdit = GetObjectsWithAllergen(allergen, editor);
                    foreach (string id in idsToEdit)
                    {
                        // add context tags
                        ObjectData objectData = editor.Data[id];
                        objectData.ContextTags ??= new List<string>();
                        objectData.ContextTags.Add(GetAllergenContextTag(allergen));
                    }
                });
            }
        }

    }
}
