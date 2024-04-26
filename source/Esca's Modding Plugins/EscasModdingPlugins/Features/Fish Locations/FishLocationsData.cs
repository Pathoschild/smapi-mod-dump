/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/EscasModdingPlugins
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;

namespace EscasModdingPlugins
{
    /// <summary>The format used by each data value in the FishLocations asset.</summary>
    public class FishLocationsData : TileData
    {
        /* Obsolete fields */

        /// <summary>The replacement return value of <see cref="GameLocation.getFishingLocation"/> (if not null). Decides which group(s) of fish will be used from the Data/Locations asset.</summary>
        /// <remarks>In EMP v1.2.3 and later, this value should be ignored. Changes in SDV 1.6 have made this infeasible to continue supporting.</remarks>
        public int? UseZone { get; set; } = null;
        /// <summary>The replacement return value of <see cref="GameLocation.catchOceanCrabPotFishFromThisSpot"/> (if not null). Decides whether crab pots will use ocean or freshwater data from the Data/Fish asset.</summary>
        /// <remarks>In EMP v1.2.3 and later, this value should be ignored unless <sa</remarks>
        public bool? UseOceanCrabPots { get; set; } = null;

        /* Active fields */

        /// <summary>The replacement locationName value in <see cref="GameLocation.getFish"/> (if not null). Decides which location key will be used in the Data/Locations asset.</summary>
        public string UseLocation { get; set; } = null;
        /// <summary>The replacement tile value in <see cref="GameLocation.getFish"/> (if not null). Decides which tile will be used for fishing zones in the Data/Locations asset.</summary>
        public JsonVector2? UseTile { get; set; } = null;

        private List<string> useCrabPotTypes { get; set; } = null;
        /// <summary>The replacement list of fish types to return in <see cref="GameLocation.GetCrabPotFishForTile(Vector2)"/> (if not null). Decides which sets of crab pot "fish" can be caught.</summary>
        /// <remarks>As of SDV 1.6, these values are case-sensitive.</remarks>
        public List<string> UseCrabPotTypes
        {
            get
            {
                //if this field is not set, use the obsolete ocean/freshwater value instead, if provided
                if (useCrabPotTypes == null)
                {
                    if (UseOceanCrabPots == true)
                        return new List<string>(GameLocation.OceanCrabPotFishTypes); //use "ocean"
                    else if (UseOceanCrabPots == false)
                        return new List<string>(GameLocation.DefaultCrabPotFishTypes); //use "freshwater"
                }

                return useCrabPotTypes;
            }

            set
            {
                useCrabPotTypes = value;
            }
        }

        /// <inheritdoc/>
        public override bool TryParse(string raw)
        {
            string[] splitValue = raw.Split(new char[0], StringSplitOptions.RemoveEmptyEntries); //split the value around whitespace and remove empty entries

            if (splitValue.Length > 0) //if 1 entry exists
            {
                if (int.TryParse(splitValue[0], out _)) //if the first entry is a valid integer
                    return TryParseFormat1(splitValue); //assume it uses the old format
                else
                    return TryParseFormat2(splitValue); //assume it uses the new format
            }
            else
            {
                Monitor?.LogOnce($"Invalid tile property value: \"{raw}\". Reason: Value is blank.", LogLevel.Debug);
                return false;
            }
        }

        /// <summary>Parses a raw string, assuming it has the format used in EMP v1.0.0 - v1.2.0. Obsolete and partially ineffective in later versions.</summary>
        /// <param name="splitValue">The string to parse, split around whitespace with empty entries removed. Assumed to have at least one entry.</param>
        /// <returns>True if parsing succeeded; false otherwise. If false, this instance should be unmodified from its previous state.</returns>
        /// <remarks>
        /// Expected format:<br/>
        ///     "{UseZone} [UseLocation] [UseOceanCrabPots]"<br/>
        /// <br/>
        /// Argument types:<br/>
        ///     "int"<br/>
        ///     "int string"<br/>
        ///     "int string bool"<br/>
        /// <br/>
        /// Examples:<br/>
        ///     "0"<br/>
        ///     "0 Beach"<br/>
        ///     "0 Beach true"<br/>
        /// </remarks>
        private bool TryParseFormat1(string[] splitValue)
        {
            int useZone;
            string useLocation = null;
            bool? useOceanCrabPots = null;

            if (!int.TryParse(splitValue[0], out useZone)) //if the first entry is NOT a valid integer
            {
                Monitor?.LogOnce($"Invalid tile property value: \"{string.Join(" ", splitValue)}\". Reason: \"{splitValue[0]}\" is not a valid integer.", LogLevel.Debug);
                return false;
            }

            if (splitValue.Length > 1) //if 2 entries exist
                useLocation = splitValue[1];

            if (splitValue.Length > 2) //if 3 entries exist
            {
                if (!bool.TryParse(splitValue[2], out bool useOcean)) //if the third entry is NOT a valid boolean
                {
                    Monitor?.LogOnce($"Invalid tile property value: \"{string.Join(" ", splitValue)}\". Reason: \"{splitValue[2]}\" is not a valid boolean (true or false).", LogLevel.Debug);
                    return false;
                }
                else
                    useOceanCrabPots = useOcean;
            }

            //parsing succeeded; apply all parsed data to this instance
            UseZone = useZone;
            UseLocation = useLocation;
            UseOceanCrabPots = useOceanCrabPots;
            return true;
        }

        /// <summary>Parses a raw string, assuming it has the format used in EMP v1.2.3 and later.</summary>
        /// <param name="splitValue">The string to parse, split around whitespace with empty entries removed. Assumed to have at least one entry.</param>
        /// <returns>True if parsing succeeded; false otherwise. If false, this instance should be unmodified from its previous state.</returns>
        /// <remarks>
        /// Expected format:<br/>
        ///     "[UseLocation] [UseTile.X] [UseTile.Y] [UseCrabPotTypes]"<br/>
        /// <br/>
        /// Argument types:<br/>
        ///     "string"<br/>
        ///     "string int int"<br/>
        ///     "string int int strings"<br/>
        ///     "string strings"<br/>
        /// <br/>
        /// Examples:<br/>
        ///     "Beach"<br/>
        ///     "Beach 0 20"<br/>
        ///     "Beach 0 20 ocean freshwater CustomType"<br/>
        ///     "Beach ocean freshwater CustomType"<br/>
        /// </remarks>
        private bool TryParseFormat2(string[] splitValue)
        {
            string useLocation = null;
            JsonVector2? useTile = null;
            List<string> useCrabPotTypes = null;

            if (!string.IsNullOrWhiteSpace(splitValue[0]) && !splitValue[0].Equals("null", StringComparison.OrdinalIgnoreCase)) //if the first entry is NOT blank/empty or the word "null"
                useLocation = splitValue[0]; //use it

            if (splitValue.Length > 1) //if 2 entries exist
            {
                if (int.TryParse(splitValue[1], out int x)) //if the second entry is an integer
                {
                    if (splitValue.Length > 2) //if 3 entries exist
                    {
                        if (int.TryParse(splitValue[2], out int y)) //if the third entry is also an integer
                        {
                            useTile = new JsonVector2(x, y); //use entries 2 and 3 as tile coordinates

                            if (splitValue.Length > 3) //if 4 entries exist
                            {
                                useCrabPotTypes = new List<string>();
                                for (int index = 3; index < splitValue.Length; index++) //for each remaining entry
                                {
                                    useCrabPotTypes.Add(splitValue[index]); //add it to the crab type list
                                }
                            }
                        }
                        else //if the third entry is NOT a valid integer
                        {
                            Monitor?.LogOnce($"Invalid tile property value: \"{string.Join(" ", splitValue)}\". Reason: \"{splitValue[1]}\" is a valid integer, but \"{splitValue[2]}\" is not.", LogLevel.Debug);
                            return false;
                        }
                    }
                    else //if only 2 entries exist
                    {
                        Monitor?.LogOnce($"Invalid tile property value: \"{string.Join(" ", splitValue)}\". Reason: \"{splitValue[1]}\" is a valid integer, but no second integer was provided.", LogLevel.Debug);
                        return false;
                    }
                }
                else //if the second entry is NOT an integer, assume it's a crab type string
                {
                    useCrabPotTypes = new List<string>();
                    for (int index = 1; index < splitValue.Length; index++) //for each remaining entry
                    {
                        useCrabPotTypes.Add(splitValue[index]); //add it to the crab type list
                    }
                }
            }

            //parsing succeeded; apply all parsed data to this instance
            UseLocation = useLocation;
            UseTile = useTile;
            UseCrabPotTypes = useCrabPotTypes;
            return true;
        }
    }
}
