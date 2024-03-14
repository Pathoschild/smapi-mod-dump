/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sonozuki/StardewMods
**
*************************************************/

using StardewModdingAPI;
using System.Reflection;

namespace Outerwear
{
    /// <summary>Contains miscellaneous helper methods.</summary>
    public static class Utilities
    {
        /*********
        ** Public Methods
        *********/
        /// <summary>Converts a token into a numerical id.</summary>
        /// <param name="token">The token to convert.</param>
        /// <returns>A numerical id.</returns>
        public static int ResolveToken(string token)
        {
            // ensure it's actually a token
            if (!token.Contains(":"))
            {
                // check the inputted value is a number
                if (int.TryParse(token, out int id))
                {
                    return id;
                }
                else
                {
                    ModEntry.Instance.Monitor.Log($"The value: '{token}' isn't a valid token and isn't a number", LogLevel.Error);
                    return -1;
                }
            }

            // ensure there are enough sections of the token to be valid
            var splitToken = token.Split(':');
            if (splitToken.Length != 3)
            {
                ModEntry.Instance.Monitor.Log("Invalid number of arguments passed. Correct layout is: '[uniqueId]:[apiMethodName]:[valueToPass]'", LogLevel.Error);
                return -1;
            }

            var uniqueId = splitToken[0];
            var apiMethodName = splitToken[1];
            var valueToPass = splitToken[2];

            // ensure an api could be found with the unique id
            var api = ModEntry.Instance.Helper.ModRegistry.GetApi(uniqueId);
            if (api == null)
            {
                ModEntry.Instance.Monitor.Log($"No api could be found provided by: {uniqueId}", LogLevel.Error);
                return -1;
            }

            // ensure the api has the correct method
            var apiMethodInfo = api.GetType().GetMethod(apiMethodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            if (apiMethodInfo == null)
            {
                ModEntry.Instance.Monitor.Log($"No api method with the name: {apiMethodName} could be found for api provided by: {uniqueId}", LogLevel.Error);
                return -1;
            }

            // ensure the api returned a valid value
            if (!int.TryParse(apiMethodInfo.Invoke(api, new[] { valueToPass }).ToString(), out var apiResult) || apiResult == -1)
            {
                ModEntry.Instance.Monitor.Log($"No valid value was returned from method: {apiMethodName} in api provided by: {uniqueId} with a passed value of: {valueToPass}", LogLevel.Error);
                return -1;
            }

            return apiResult;
        }
    }
}
