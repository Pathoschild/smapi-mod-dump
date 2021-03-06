/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TehPers.CoreMod.Api.Conflux.Matching;

namespace TehPers.CoreMod.ContentPacks.Data {
    internal class RecipeData {
        public string Name { get; set; } = null;
        public bool IsCooking { get; set; } = false;
        public List<Part> Ingredients { get; set; } = new List<Part>();
        public List<Part> Results { get; set; } = new List<Part>();

        public abstract class Part {
            public int Quantity { get; set; } = 1;
        }

        public class GamePart : Part {
            public int Id { get; set; }
            public ItemType Type { get; set; } = ItemType.OBJECT;
        }

        public class ModPart : Part {
            public string Id { get; set; }
        }

        public enum ItemType {
            OBJECT,
            BIGCRAFTABLE,
            WEAPON,
            HAT
        }

        public class PartJsonConverter : JsonConverter<Part> {
            public override void WriteJson(JsonWriter writer, Part value, JsonSerializer serializer) {
                throw new NotSupportedException();
            }

            public override Part ReadJson(JsonReader reader, Type objectType, Part existingValue, bool hasExistingValue, JsonSerializer serializer) {
                // Try to get the recipe part as an object
                if (!(JToken.ReadFrom(reader) is JObject objToken)) {
                    throw new InvalidOperationException("Recipe part must be an object");
                }

                // Try to get the ID
                if (!objToken.TryGetValue("id", StringComparison.OrdinalIgnoreCase, out JToken idToken)) {
                    throw new InvalidOperationException("Recipe part must have an ID which must either be a string or a whole number");
                }

                // Make sure the ID is a value type
                if (!(idToken is JValue idValueToken)) {
                    throw new InvalidOperationException("Recipe part's ID must either be a string or a whole number");
                }

                // Try to get the quantity, defaulting to 1
                int quantity;
                if (objToken.TryGetValue("quantity", StringComparison.OrdinalIgnoreCase, out JToken quantityToken) && quantityToken is JValue quantityValueToken && quantityValueToken.Value is IConvertible convertibleQuantity) {
                    quantity = convertibleQuantity.ToInt32(CultureInfo.InvariantCulture);
                } else {
                    quantity = 1;
                }

                // Make sure the quantity is valid
                if (quantity <= 0) {
                    throw new InvalidOperationException("Recipe part's quantity must be greater than 0.");
                }

                // Create a part with the given ID
                return idValueToken.Value.Match<object, Part>()
                    .When<string>(id => new ModPart {
                        Id = id,
                        Quantity = quantity
                    })
                    .When<IConvertible>(id => ConvertToGamePart(Convert.ToInt32(id)))
                    .ElseThrow(value => new InvalidOperationException($"Unexpected ID for recipe: {value}"));

                Part ConvertToGamePart(int id) {
                    // Check if there is a string representing the type
                    if (!(objToken.TryGetValue("type", out JToken typeToken) && typeToken is JValue typeValueToken && typeValueToken.Value is string type)) {
                        throw new InvalidOperationException("Recipe part must include a type");
                    }

                    // Try to convert the type string into the enum value
                    if (!Enum.TryParse(type, true, out ItemType itemType)) {
                        throw new InvalidOperationException($"Recipe part's type must be one of the following types: {string.Join(", ", Enum.GetNames(typeof(ItemType)).Select(name => $"\"{name}\""))}");
                    }

                    // Return the part
                    return new GamePart {
                        Id = id,
                        Quantity = quantity,
                        Type = itemType
                    };
                }
            }

            public override bool CanWrite => false;
        }
    }
}