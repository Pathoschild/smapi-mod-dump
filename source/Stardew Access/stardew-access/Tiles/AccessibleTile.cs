/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/stardew-access/stardew-access
**
*************************************************/

using Microsoft.Xna.Framework;
using stardew_access.Translation;
using stardew_access.Utils;
using System.Text;

namespace stardew_access.Tiles
{
    public class AccessibleTile : ConditionalBase
    {
        public class JsonSerializerFormat
        {
            public string NameOrTranslationKey { get; set; }
            public int[] X { get; set; }
            public int[] Y { get; set; }
            public string Category { get; set; }
            public string[]? WithMods { get; set; }
            public string[]? Conditions { get; set; }
        }
        
        private readonly string? StaticNameOrTranslationKey;
        private readonly Func<ConditionalBase, string>? DynamicNameOrTranslationKey;
        public string NameOrTranslationKey
        {
            get
            {
                if (DynamicNameOrTranslationKey != null)
                {
                    return DynamicNameOrTranslationKey(this);
                }
                else if (StaticNameOrTranslationKey != null)
                {
                    return StaticNameOrTranslationKey;
                }
                else
                {
                    return "Unnamed";
                }
            }
        }

        private readonly Vector2 StaticCoordinates;
        private readonly Func<ConditionalBase, Vector2>? DynamicCoordinates;
        private string[]? _withMods;
        private string[]? _conditions;
        public Vector2 Coordinates
        {
            get
            {
                if (StaticCoordinates != Vector2.Zero)
                {
                    return StaticCoordinates;
                }
                else if (DynamicCoordinates != null)
                {
                    return DynamicCoordinates(this);
                }
                else
                {
                    return Vector2.Zero;
                }
            }
        }

        public readonly CATEGORY Category;

        public readonly bool IsEvent ;

        // Super constructor
        public AccessibleTile(
            string? staticNameOrTranslationKey = null,
            Func<ConditionalBase, string>? dynamicNameOrTranslationKey = null,
            Vector2? staticCoordinates = null,
            Func<ConditionalBase, Vector2>? dynamicCoordinates = null,
            CATEGORY? category = null,
            string[]? conditions = null,
            string[]? withMods = null,
            bool isEvent = false
        ) : base(conditions, withMods)
        {
            // Error handling for invalid combinations
            if (staticNameOrTranslationKey == null && dynamicNameOrTranslationKey == null)
            {
                throw new ArgumentException("At least one of static or dynamic name must be provided.");
            }

            if (!(staticCoordinates == null ^ dynamicCoordinates == null))
            {
                throw new ArgumentException("Exactly one of static or dynamic coordinates must be provided.");
            }

            // Set properties
            StaticNameOrTranslationKey = staticNameOrTranslationKey;
            DynamicNameOrTranslationKey = dynamicNameOrTranslationKey;
            StaticCoordinates = staticCoordinates ?? Vector2.Zero;
            DynamicCoordinates = dynamicCoordinates;
            Category = category ?? CATEGORY.Others;
            IsEvent = isEvent;
            _withMods = withMods;
            _conditions = conditions;
        }

        public AccessibleTile(
            string? staticNameOrTranslationKey,
            Vector2? staticCoordinates,
            CATEGORY category,
            string[]? conditions = null,
            string[]? withMods = null,
            bool isEvent = false
        ) : this (staticNameOrTranslationKey, null, staticCoordinates, null, category, conditions, withMods, isEvent)
        {
        }

        public AccessibleTile(
            string? staticNameOrTranslationKey,
            Func<ConditionalBase, Vector2>? dynamicCoordinates,
            CATEGORY category,
            string[]? conditions = null,
            string[]? withMods = null,
            bool isEvent = false
        ) : this (staticNameOrTranslationKey, null, null, dynamicCoordinates, category, conditions, withMods, isEvent)
        {
        }

        public AccessibleTile(
            Func<ConditionalBase, string> dynamicNameOrTranslationKey,
            Vector2? staticCoordinates,
            CATEGORY category,
            string[]? conditions = null,
            string[]? withMods = null,
            bool isEvent = false
        ) : this (null, dynamicNameOrTranslationKey, staticCoordinates, null, category, conditions, withMods, isEvent)
        {
        }

        public AccessibleTile(
            Func<ConditionalBase, string> dynamicNameOrTranslationKey,
            Func<ConditionalBase, Vector2>? dynamicCoordinates,
            CATEGORY category,
            string[]? conditions = null,
            string[]? withMods = null,
            bool isEvent = false
        ) : this (null, dynamicNameOrTranslationKey, null, dynamicCoordinates, category, conditions, withMods, isEvent)
        {
        }

        public AccessibleTile(
            string? staticNameOrTranslationKey,
            Func<ConditionalBase, string> dynamicNameOrTranslationKey,
            Vector2? staticCoordinates,
            CATEGORY category,
            string[]? conditions = null,
            string[]? withMods = null,
            bool isEvent = false
        ) : this (staticNameOrTranslationKey, dynamicNameOrTranslationKey, staticCoordinates, null, category, conditions, withMods, isEvent)
        {
        }

        public AccessibleTile(
            string? staticNameOrTranslationKey,
            Func<ConditionalBase, string> dynamicNameOrTranslationKey,
            Func<ConditionalBase, Vector2>? dynamicCoordinates,
            CATEGORY category,
            string[]? conditions = null,
            string[]? withMods = null,
            bool isEvent = false
        ) : this (staticNameOrTranslationKey, dynamicNameOrTranslationKey, null, dynamicCoordinates, category, conditions, withMods, isEvent)
        {
        }

        public override string ToString()
        {
            StringBuilder sb = new();
            sb.Append("AccessibleTile { ");
            sb.Append($"{NameOrTranslationKey}:{Category} ");
            sb.Append($"at ({StaticCoordinates.X}, {StaticCoordinates.Y})");
            // ... append other properties or fields ...
            sb.Append(" }");
            return sb.ToString();
        }

        public JsonSerializerFormat SerializableFormat =>
            new()
            {
                NameOrTranslationKey = NameOrTranslationKey,
                X = new []{(int)Coordinates.X},
                Y = new []{(int)Coordinates.Y},
                Category = Category.Key,
                WithMods = _withMods,
                Conditions = _conditions
            };

        public (string nameOrTranslationKey, CATEGORY category) NameAndCategory
        {
            get
            {
                return (
                    Translator.Instance.Translate(
                        NameOrTranslationKey, 
                        translationCategory: TranslationCategory.StaticTiles,
                        disableWarning: true
                    ),
                    Category
                );
            }
        }
    }
}
