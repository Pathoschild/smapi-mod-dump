/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using StardewValley;

namespace DeluxeJournal.Util
{
    public static class FlavoredItemHelper
    {
        /// <summary>Encode a flavored item ID using the base preserve item ID and the corresponding ingredient item ID.</summary>
        /// <remarks>
        /// The flavored item ID encoding is of the form "<c>(O){preserveId}/{ingredientId}</c>". For example, an
        /// "Apple Jelly" item would have the ID "<c>(O)344/613</c>".
        /// </remarks>
        /// <param name="preserveId">The base preserve item ID.</param>
        /// <param name="ingredientId">The ingredient item ID.</param>
        /// <returns>The encoded flavored item ID.</returns>
        public static string EncodeFlavoredItemId(string preserveId, string ingredientId)
        {
            preserveId = preserveId.StartsWith('(') ? preserveId : ItemRegistry.type_object + preserveId;
            ingredientId = ingredientId.StartsWith(ItemRegistry.type_object) ? ingredientId[3..] : ingredientId;

            return $"{preserveId}/{ingredientId}";
        }

        /// <summary>Decode the flavored item ID into its preserve and ingredient IDs.</summary>
        /// <param name="flavoredId">The encoded flavored item ID.</param>
        /// <param name="preserveId">The decoded qualified perserve item ID.</param>
        /// <param name="ingredientId">The decoded ingredient item ID.</param>
        /// <param name="qualifyIngredientId">Qualify the ingredient ID.</param>
        /// <returns>
        /// <c>true</c> if the flavored item ID was of the correct format and could be decoded;
        /// <c>false</c> if the operation failed. See <see cref="EncodeFlavoredItemId(string, string)"/>
        /// for flavored item ID format details.
        /// </returns>
        public static bool DecodeFlavoredItemId(string flavoredId, out string preserveId, out string ingredientId, bool qualifyIngredientId = true)
        {
            int seperatorIndex = flavoredId.IndexOf('/');

            if (seperatorIndex < 0)
            {
                preserveId = string.Empty;
                ingredientId = string.Empty;
                return false;
            }

            preserveId = flavoredId[..seperatorIndex];
            ingredientId = flavoredId[(seperatorIndex + 1)..];

            if (qualifyIngredientId)
            {
                ingredientId = ItemRegistry.type_object + ingredientId;
            }

            return true;
        }

        /// <summary>Get the preserve ID from an encoded flavored item ID.</summary>
        /// <param name="flavoredId">The encoded flavored item ID.</param>
        /// <returns>
        /// The preserve item ID or return back the <paramref name="flavoredId"/> if invalid format.
        /// See <see cref="EncodeFlavoredItemId(string, string)"/> for flavored item ID format details.
        /// </returns>
        public static string GetPreserveId(string flavoredId)
        {
            int seperatorIndex = flavoredId.IndexOf('/');

            if (seperatorIndex < 0)
            {
                return flavoredId;
            }

            return flavoredId[..seperatorIndex];
        }

        /// <summary>Get the ingredient ID from an encoded flavored item ID.</summary>
        /// <param name="flavoredId">The encoded flavored item ID.</param>
        /// <param name="qualify">Qualify the ingredient ID.</param>
        /// <returns>
        /// The ingredient item ID or an empty string if invalid format.
        /// See <see cref="EncodeFlavoredItemId(string, string)"/> for flavored item ID format details.
        /// </returns>
        public static string GetIngredientId(string flavoredId, bool qualify = true)
        {
            int seperatorIndex = flavoredId.IndexOf('/') + 1;

            if (seperatorIndex < 0)
            {
                return string.Empty;
            }

            return qualify ? ItemRegistry.type_object + flavoredId[seperatorIndex..] : flavoredId[seperatorIndex..];
        }

        /// <summary>Convert a list of flavored item IDs to a list of base item IDs with the encoded flavor information removed.</summary>
        /// <param name="flavoredIds">List of encoded flavor IDs.</param>
        /// <param name="baseIds">List of base item IDs.</param>
        /// <param name="qualifyIngredientId">Qualify the ingredient ID.</param>
        /// <param name="keepOnDecodeError">Keep IDs that could not be decoded. Lists that may have a mix of flavored and unflavored IDs should enable this.</param>
        /// <returns>
        /// The removed ingredient ID or <c>null</c> if none was found. If the <paramref name="flavoredIds"/>
        /// contains a mix of preserve types, then it always returns the last one seen.
        /// </returns>
        public static string? ConvertFlavoredList(IList<string> flavoredIds, out IList<string> baseIds, bool qualifyIngredientId = true, bool keepOnDecodeError = true)
        {
            string? ingredientId = null;
            baseIds = new List<string>();

            foreach (string flavoredId in flavoredIds)
            {
                if (DecodeFlavoredItemId(flavoredId, out string preserveId, out string ingredientIdDecode, qualifyIngredientId))
                {
                    ingredientId = ingredientIdDecode;
                    baseIds.Add(preserveId);
                }
                else if (keepOnDecodeError)
                {
                    baseIds.Add(flavoredId);
                    continue;
                }
            }

            return ingredientId;
        }

        /// <summary>Create a flavored item instance from an encoded flavored item ID.</summary>
        /// <param name="flavoredId">The encoded flavored item ID.</param>
        /// <returns>
        /// An item instance or <c>null</c> if one could not be created.
        /// See <see cref="EncodeFlavoredItemId(string, string)"/> for flavored item ID format details.
        /// </returns>
        public static Item? CreateFlavoredItem(string flavoredId)
        {
            if (DecodeFlavoredItemId(flavoredId, out string preserveId, out string ingredientId))
            {
                return CreateFlavoredItem(GetPreserveType(preserveId), ingredientId);
            }

            return null;
        }

        /// <summary>Create a flavored item instance from a preserved item ID and ingredient item ID.</summary>
        /// <param name="preserveId">The qualified preserve item ID.</param>
        /// <param name="ingredientId">The ingredient item ID.</param>
        /// <returns>
        /// An item instance or <c>null</c> if one could not be created.
        /// See <see cref="EncodeFlavoredItemId(string, string)"/> for flavored item ID format details.
        /// </returns>
        public static Item? CreateFlavoredItem(string preserveId, string ingredientId)
        {
            return CreateFlavoredItem(GetPreserveType(preserveId), ingredientId);
        }

        /// <summary>Create a flavored item instance from a <see cref="StardewValley.Object.PreserveType"/> enum value and ingredient item ID.</summary>
        /// <param name="preserveType">The preserve type.</param>
        /// <param name="ingredientId">The ingredient item ID.</param>
        /// <returns>
        /// An item instance or <c>null</c> if one could not be created.
        /// See <see cref="EncodeFlavoredItemId(string, string)"/> for flavored item ID format details.
        /// </returns>
        public static Item? CreateFlavoredItem(SObject.PreserveType preserveType, string ingredientId)
        {
            if (ItemRegistry.Create(ingredientId, allowNull: true) is not SObject ingredient)
            {
                return null;
            }

            return ItemRegistry.GetObjectTypeDefinition().CreateFlavoredItem(preserveType, ingredient);
        }

        /// <summary>Get the <see cref="StardewValley.Object.PreserveType"/> enum value given the preserve item ID.</summary>
        /// <param name="preserveId">The qualified preserve item ID.</param>
        public static SObject.PreserveType GetPreserveType(string preserveId)
        {
            return preserveId switch
            {
                "(O)348" => SObject.PreserveType.Wine,
                "(O)344" => SObject.PreserveType.Jelly,
                "(O)342" => SObject.PreserveType.Pickle,
                "(O)350" => SObject.PreserveType.Juice,
                "(O)812" => SObject.PreserveType.Roe,
                "(O)447" => SObject.PreserveType.AgedRoe,
                "(O)340" => SObject.PreserveType.Honey,
                "(O)SpecificBait" => SObject.PreserveType.Bait,
                "(O)DriedFruit" => SObject.PreserveType.DriedFruit,
                "(O)DriedMushrooms" => SObject.PreserveType.DriedMushroom,
                "(O)SmokedFish" => SObject.PreserveType.SmokedFish,
                _ => SObject.PreserveType.Wine
            };
        }
    }
}
