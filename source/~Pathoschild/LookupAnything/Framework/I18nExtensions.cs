/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Netcode;
using Pathoschild.Stardew.LookupAnything.Framework.Constants;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.GameData.WildTrees;
using StardewValley.Mods;
using StardewValley.Network;
using StardewValley.Pathfinding;

namespace Pathoschild.Stardew.LookupAnything.Framework
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal partial class I18n
    {
        /*********
        ** Public methods
        *********/
        /// <summary>Get a separated list of values (like "A, B, C") using the separator for the current language.</summary>
        /// <param name="values">The values to list.</param>
        public static string List(IEnumerable<object> values)
        {
            return string.Join(I18n.Generic_ListSeparator(), values);
        }

        /// <summary>Get a translation for an enum value.</summary>
        /// <param name="stage">The tree growth stage.</param>
        public static string For(WildTreeGrowthStage stage)
        {
            string stageKey = stage == (WildTreeGrowthStage)4
                ? "smallTree"
                : stage.ToString();

            return I18n.GetByKey($"tree.stages.{stageKey}");
        }

        /// <summary>Get a translation for an enum value.</summary>
        /// <param name="quality">The item quality.</param>
        public static string For(ItemQuality quality)
        {
            return I18n.GetByKey($"quality.{quality.GetName()}");
        }

        /// <summary>Get a translation for an enum value.</summary>
        /// <param name="status">The friendship status.</param>
        /// <param name="wasHousemate">Whether the NPC is eligible to be a housemate, rather than spouse.</param>
        public static string For(FriendshipStatus status, bool wasHousemate)
        {
            if (wasHousemate && status == FriendshipStatus.Divorced)
                return I18n.FriendshipStatus_KickedOut();
            return I18n.GetByKey($"friendship-status.{status.ToString().ToLower()}");
        }

        /// <summary>Get a translation for an enum value.</summary>
        /// <param name="age">The child age.</param>
        public static string For(ChildAge age)
        {
            return I18n.GetByKey($"npc.child.age.{age.ToString().ToLower()}");
        }

        /// <summary>Get a value like <c>{{name}} loves this</c>, <c>{{name}} likes this</c>, etc.</summary>
        /// <param name="taste">The taste value returned by <see cref="StardewValley.Locations.MovieTheater.GetConcessionTasteForCharacter"/>.</param>
        /// <param name="name">The NPC name.</param>
        public static string ForMovieTasteLabel(string taste, string name)
        {
            return I18n.GetByKey($"item.movie-snack-preference.{taste}", new { name });
        }

        /// <summary>Select the correct translation based on the plural form.</summary>
        /// <param name="count">The number.</param>
        /// <param name="singleText">The singular form.</param>
        /// <param name="pluralText">The plural form.</param>
        public static string GetPlural(int count, string singleText, string pluralText)
        {
            return count == 1 ? singleText : pluralText;
        }

        /// <summary>Get a translated season name from the game.</summary>
        /// <param name="season">The English season name.</param>
        public static string GetSeasonName(Season season)
        {
            return Utility.getSeasonNameFromNumber((int)season);
        }

        /// <summary>Get translated season names from the game.</summary>
        /// <param name="seasons">The English season names.</param>
        public static IEnumerable<string> GetSeasonNames(IEnumerable<Season> seasons)
        {
            foreach (Season season in seasons)
                yield return I18n.GetSeasonName(season);
        }

        /// <summary>Get a human-readable representation of a value.</summary>
        /// <param name="value">The underlying value.</param>
        public static string? Stringify(object? value)
        {
            switch (value)
            {
                case null:
                    return null;

                // net types
                case NetBool net:
                    return I18n.Stringify(net.Value);
                case NetByte net:
                    return I18n.Stringify(net.Value);
                case NetColor net:
                    return I18n.Stringify(net.Value);
                case NetDancePartner net:
                    return I18n.Stringify(net.Value?.displayName);
                case NetDouble net:
                    return I18n.Stringify(net.Value);
                case NetFloat net:
                    return I18n.Stringify(net.Value);
                case NetGuid net:
                    return I18n.Stringify(net.Value);
                case NetInt net:
                    return I18n.Stringify(net.Value);
                case NetLocationRef net:
                    return I18n.Stringify(net.Value?.NameOrUniqueName);
                case NetLong net:
                    return I18n.Stringify(net.Value);
                case NetPoint net:
                    return I18n.Stringify(net.Value);
                case NetPosition net:
                    return I18n.Stringify(net.Value);
                case NetRectangle net:
                    return I18n.Stringify(net.Value);
                case NetString net:
                    return I18n.Stringify(net.Value);
                case NetVector2 net:
                    return I18n.Stringify(net.Value);

                // core types
                case bool boolean:
                    return boolean ? I18n.Generic_Yes() : I18n.Generic_No();
                case Color color:
                    return $"(r:{color.R} g:{color.G} b:{color.B} a:{color.A})";
                case SDate date:
                    return date.ToLocaleString(withYear: date.Year != Game1.year);
                case TimeSpan span:
                    {
                        List<string> parts = [];
                        if (span.Days > 0)
                            parts.Add(I18n.Generic_Days(span.Days));
                        if (span.Hours > 0)
                            parts.Add(I18n.Generic_Hours(span.Hours));
                        if (span.Minutes > 0)
                            parts.Add(I18n.Generic_Minutes(span.Minutes));
                        return I18n.List(parts);
                    }
                case Vector2 vector:
                    return $"({vector.X}, {vector.Y})";
                case Rectangle rect:
                    return $"(x:{rect.X}, y:{rect.Y}, width:{rect.Width}, height:{rect.Height})";

                // game types
                case AnimatedSprite sprite:
                    return $"(textureName: {sprite.textureName.Value}, currentFrame:{sprite.currentFrame}, loop:{sprite.loop}, sourceRect:{I18n.Stringify(sprite.sourceRect)})";
                case MarriageDialogueReference dialogue:
                    return $"(file: {dialogue.DialogueFile}, key: {dialogue.DialogueKey}, gendered: {dialogue.IsGendered}, substitutions: {I18n.Stringify(dialogue.Substitutions)})";
                case ModDataDictionary data when data.Any():
                    {
                        StringBuilder str = new StringBuilder();
                        str.AppendLine();
                        foreach (var pair in data.Pairs.OrderBy(p => p.Key))
                            str.AppendLine($"- {pair.Key}: {pair.Value}");
                        return str.ToString().TrimEnd();
                    }

                case SchedulePathDescription schedulePath:
                    return $"{schedulePath.time / 100:00}:{schedulePath.time % 100:00} {schedulePath.targetLocationName} ({schedulePath.targetTile.X}, {schedulePath.targetTile.Y}) {schedulePath.facingDirection} {schedulePath.endOfRouteMessage}";

                case Stats stats:
                    {
                        StringBuilder str = new StringBuilder();
                        str.AppendLine();
                        foreach ((string key, uint statValue) in stats.Values)
                            str.AppendLine($"- {key}: {I18n.Stringify(statValue)}");
                        return str.ToString().TrimEnd();
                    }
                case Warp warp:
                    return $"([{warp.X}, {warp.Y}] to {warp.TargetName}[{warp.TargetX}, {warp.TargetY}])";

                // enumerable
                case IEnumerable array when value is not string:
                    {
                        string[] values = (from val in array.Cast<object>() select I18n.Stringify(val) ?? "<null>").ToArray()!;
                        return "(" + I18n.List(values) + ")";
                    }

                default:
                    // key/value pair
                    {
                        Type type = value.GetType();
                        if (type.IsGenericType)
                        {
                            Type genericType = type.GetGenericTypeDefinition();
                            if (genericType == typeof(NetDictionary<,,,,>))
                            {
                                object? dict = type.GetProperty("FieldDict")?.GetValue(value);
                                return I18n.Stringify(dict);
                            }
                            if (genericType == typeof(KeyValuePair<,>))
                            {
                                string? k = I18n.Stringify(type.GetProperty(nameof(KeyValuePair<byte, byte>.Key))?.GetValue(value));
                                string? v = I18n.Stringify(type.GetProperty(nameof(KeyValuePair<byte, byte>.Value))?.GetValue(value));
                                return $"({k}: {v})";
                            }
                        }
                    }

                    // anything else
                    return value.ToString();
            }
        }
    }
}
