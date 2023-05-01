/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/SolidFoundations
**
*************************************************/

using SolidFoundations.Framework.Models.ContentPack;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidFoundations.Framework.External.ContentPatcher
{
    internal class IsBuildingHereToken
    {
        /// <summary>Get whether the token allows input arguments (e.g. an NPC name for a relationship token).</summary>
        /// <remarks>Default false.</remarks>
        public bool AllowsInput() { return true; }

        /// <summary>Whether the token requires input arguments to work, and does not provide values without it (see <see cref="AllowsInput"/>).</summary>
        /// <remarks>Default false.</remarks>
        public bool RequiresInput() { return true; }

        /// <summary>Whether the token may return multiple values for the given input.</summary>
        /// <param name="input">The input arguments, if any.</param>
        /// <remarks>Default true.</remarks>
        public bool CanHaveMultipleValues(string input = null) { return false; }

        /// <summary>Validate that the provided input arguments are valid.</summary>
        /// <param name="input">The input arguments, if any.</param>
        /// <param name="error">The validation error, if any.</param>
        /// <returns>Returns whether validation succeeded.</returns>
        /// <remarks>Default true.</remarks>
        public bool TryValidateInput(string input, out string error)
        {
            error = String.Empty;

            var splitToken = input.Split('@');
            if (splitToken.Length != 2)
            {
                error = $"Token doesn't follow the convention of BuildingName@LocationName: {input}";
                return false;
            }

            return true;
        }

        /// <summary>Update the values when the context changes.</summary>
        /// <returns>Returns whether the value changed, which may trigger patch updates.</returns>
        public bool UpdateContext()
        {
            return true;
        }

        /// <summary>Get whether the token is available for use.</summary>
        public bool IsReady()
        {
            return Context.IsWorldReady;
        }

        /// <summary>Get the current values.</summary>
        /// <param name="input">The input arguments, if any.</param>
        public IEnumerable<string> GetValues(string input)
        {
            var splitToken = input.Split('@');
            if (!IsReady() || splitToken.Length != 2)
            {
                yield return false.ToString();
                yield break;
            }

            string buildingName = splitToken[0];
            string locationName = splitToken[1];
            if (Game1.getLocationFromName(locationName) is BuildableGameLocation location && location is not null && location.buildings is not null)
            {
                foreach (GenericBuilding building in location.buildings.Where(b => b is GenericBuilding))
                {
                    if (building.Id.Equals(buildingName, StringComparison.OrdinalIgnoreCase))
                    {
                        yield return true.ToString();
                        yield break;
                    }
                }

                foreach (Building building in location.buildings.Where(b => b is not GenericBuilding))
                {
                    if (building.buildingType.Value.Equals(buildingName, StringComparison.OrdinalIgnoreCase))
                    {
                        yield return true.ToString();
                        yield break;
                    }
                }
            }

            yield return false.ToString();
            yield break;
        }
    }
}
