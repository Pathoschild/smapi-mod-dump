/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jasisco5/UncannyValleyMod
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UncannyValleyMod
{
    //https://github.com/Pathoschild/StardewMods/blob/develop/ContentPatcher/Framework/Tokens/ValueProviders/ModConvention/ConventionDelegates.cs
    internal class WeaponToken : Token
    {
        /*********
         ** Fields
         *********/
        /// <summary>The state as of the last context update.</summary>
        public bool weaponObtained;

        /****
        ** Metadata
        ****/
        /// <summary>Whether the token may return multiple values for the given input.</summary>
        /// <param name="input">The input arguments, if applicable.</param>
        public bool CanHaveMultipleValues(string input = null)
        {
            return false;
        }

        /// <summary>Get whether the token always chooses from a set of known values for the given input. Mutually exclusive with <see cref="HasBoundedRangeValues"/>.</summary>
        /// <param name="input">The input arguments, if any.</param>
        /// <param name="allowedValues">The possible values for the input.</param>
        /// <remarks>Default unrestricted.</remarks>
        public bool HasBoundedValues(string? input, out IEnumerable<string> allowedValues)
        {
            List<String> allowed = new List<string>();
            allowed.Add("true");
            allowed.Add("false");
            allowedValues = allowed;
            return true;
        }


        /****
        ** State
        ****/
        /// <summary>Update the values when the context changes.</summary>
        /// <returns>Returns whether the value changed, which may trigger patch updates.</returns>
        public override bool UpdateContext()
        {
            if(saveModel != null)
            {
                bool oldState = this.weaponObtained;
                this.weaponObtained = saveModel.weaponObtained;
                return this.weaponObtained != oldState;
            }
            else { return false; }
        }

        /// <summary>Get whether the token is available for use.</summary>
        public bool IsReady()
        {
            return Context.IsWorldReady;
        }

        /// <summary>Get the current values.</summary>
        /// <param name="input">The input arguments, if applicable.</param>
        public IEnumerable<string> GetValues(string input = null)
        {
            yield return this.weaponObtained.ToString().ToLower();
        }
    }
}
