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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using stardew_access.Translation;
using stardew_access.Utils;
using System.Text;

namespace stardew_access.Tiles;

public class AccessibleTile : ConditionalBase
{
    public class JsonSerializerFormat
    {
        public string? NameOrTranslationKey { get; set; }
        public string? DynamicNameOrTranslationKey { get; set; }
        public int[]? X { get; set; }
        public int[]? Y { get; set; }
        public string? DynamicCoordinates { get; set; }
        public string? Category { get; set; }
        public string[]? WithMods { get; set; }
        public string[]? Conditions { get; set; }
        public bool? IsEvent { get; set; }
        public string? Suffix { get; set; }
    }

    private readonly string? StaticNameOrTranslationKey;
    private readonly string? DynamicNameOrTranslationKey;
    private readonly Func<ConditionalBase, string>? DynamicNameOrTranslationKeyFunc;
    public string NameOrTranslationKey
    {
        get
        {
            if (DynamicNameOrTranslationKeyFunc != null)
            {
                return DynamicNameOrTranslationKeyFunc(this);
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

    private readonly Vector2[] StaticCoordinates;
    private readonly string? DynamicCoordinates;
    private readonly Func<ConditionalBase, Vector2[]>? DynamicCoordinatesFunc;
    public Vector2[] Coordinates
    {
        get
        {
            // Prioritize DynamicCoordinatesFunc if it exists
            if (DynamicCoordinatesFunc != null)
            {
                return DynamicCoordinatesFunc(this);
            }
            else
            {
                // Otherwise, return the StaticCoordinates array, empty or not
                return StaticCoordinates;
            }
        }
    }

    private readonly string[]? _withMods;
    private readonly string[]? _conditions;
    public readonly CATEGORY Category;
    public readonly bool IsEvent ;
    public readonly string? Suffix;

    public (string NameOrTranslationKey, CATEGORY Category) NameAndCategory
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

    // Super constructor
    public AccessibleTile(
        string? staticNameOrTranslationKey = null,
        string? dynamicNameOrTranslationKey = null,
        Vector2[]? staticCoordinates = null,
        string? dynamicCoordinates = null,
        CATEGORY? category = null,
        string[]? conditions = null,
        string[]? withMods = null,
        bool isEvent = false,
        string? suffix = null
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
        if (DynamicNameOrTranslationKey != null)
        {
            if (!AccessibleTileHelpers.TryGetNameHelper(DynamicNameOrTranslationKey, out DynamicNameOrTranslationKeyFunc))
            {
                throw new ArgumentException($"No helper function found for name or translation key: {DynamicNameOrTranslationKey}");
            }
        }

        StaticCoordinates = staticCoordinates ?? [];
        DynamicCoordinates = dynamicCoordinates;
        if (DynamicCoordinates != null)
        {
            if (!AccessibleTileHelpers.TryGetCoordinatesHelper(DynamicCoordinates, out DynamicCoordinatesFunc))
            {
                throw new ArgumentException($"No helper function found for coordinates: {DynamicCoordinates}");
            }
        }
        
        Category = category ?? CATEGORY.Other;
        IsEvent = isEvent;
        Suffix = suffix;
        _withMods = withMods;
        _conditions = conditions;
    }

    public override string ToString()
    {
        StringBuilder sb = new();
        sb.Append("AccessibleTile { ");
        sb.Append($"{NameOrTranslationKey}:{Category} ");

        // Iterate through and append each coordinate pair
        if (Coordinates != null)
        {
            sb.Append("at (");
            bool first = true;
            foreach (Vector2 coordinate in Coordinates)
            {
                if (!first)
                {
                    sb.Append(", ");
                }
                first = false;
                sb.Append($"{coordinate.X}, {coordinate.Y}");
            }
            sb.Append(')');
        }

        // ... append other properties or fields ...
        sb.Append(" }");
        return sb.ToString();
    }

    public JsonSerializerFormat SerializableFormat => new()
    {
        NameOrTranslationKey = StaticNameOrTranslationKey,
        DynamicNameOrTranslationKey = DynamicNameOrTranslationKey,
        X = StaticCoordinates?.Select(v => (int)v.X).Distinct().ToArray(),
        Y = StaticCoordinates?.Select(v => (int)v.Y).Distinct().ToArray(),
        DynamicCoordinates = DynamicCoordinates,
        Category = Category?.Key,
        WithMods = _withMods,
        Conditions = _conditions,
        IsEvent = IsEvent,
        Suffix = Suffix
    };

    public static AccessibleTile FromJObject(JObject jObject)
    {
        // Attempt to get string values, handling nulls
        #pragma warning disable CA1507 // Use nameof to express symbol names
        string? staticNameOrTranslationKey = jObject["NameOrTranslationKey"]?.Value<string>();
        #pragma warning restore CA1507 // Use nameof to express symbol names
        string? dynamicNameOrTranslationKey = jObject["DynamicNameOrTranslationKey"]?.Value<string>();
        string? dynamicCoordinates = jObject["DynamicCoordinates"]?.Value<string>();
        string? categoryKey = jObject["Category"]?.Value<string>();

        // Convert string arrays to string[]?, handling nulls
        string[]? withMods = jObject["WithMods"]?.ToObject<string[]>();
        string[]? conditions = jObject["Conditions"]?.ToObject<string[]>();
        bool? isEvent = jObject["IsEvent"]?.Value<bool?>() ?? false;
        string? suffix = jObject["Suffix"]?.Value<string>();
        if (suffix is null || suffix == null)
            Log.Trace($"asdf: Suffix is null, staticNameOrTranslationKey is {staticNameOrTranslationKey}");
        else if (suffix == "")
            Log.Trace($"asdf: suffix is empty, staticNameOrTranslationKey is {staticNameOrTranslationKey}");
        else
            Log.Trace($"asdf: suffix is \"{suffix}\", staticNameOrTranslationKey is {staticNameOrTranslationKey}");
        Log.Trace(jObject.ToString());
        // Parse X and Y arrays, handling potential errors and nulls
        int[]? xValues = jObject["X"]?.ToObject<int[]>();
        int[]? yValues = jObject["Y"]?.ToObject<int[]>();

        // Generate all combinations of X and Y values
        Vector2[]? staticCoordinates = null;
        if (xValues != null && yValues != null)
        {
            int totalCoordinates = xValues.Length * yValues.Length;
            staticCoordinates = new Vector2[totalCoordinates];
            int index = 0;
            foreach (int y in yValues)
            {
                foreach (int x in xValues)
                {
                    staticCoordinates[index++] = new Vector2(x, y);
                }
            }
        }

        // CATEGORY handling
        CATEGORY category = (!string.IsNullOrEmpty(categoryKey))
                            ? CATEGORY.FromString(categoryKey)
                            : CATEGORY.Other;

        // Pass parsed values to the constructor, which will handle invalid combinations
        return new AccessibleTile(
            staticNameOrTranslationKey,
            dynamicNameOrTranslationKey,
            staticCoordinates,
            dynamicCoordinates,
            category,
            conditions,
            withMods,
            isEvent ?? false, // Default to false if IsEvent is null
            suffix
        );
    }

}
