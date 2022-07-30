/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

namespace SpecialOrdersExtended.Tokens;

/// <summary>
/// Abstract token that keeps a cache that's a list of strings
/// And can return either the list
/// or if any item exists in the list.
/// </summary>
internal abstract class AbstractToken
{
    private static readonly string[] Booleans = new[] { "true", "false" };

    /// <summary>
    /// Internal cache for token. Will be null if not ready.
    /// </summary>
    private List<string>? tokenCache = null;

    /// <summary>
    /// Whether or not the token allows input. Default, true.
    /// </summary>
    /// <returns>false - we don't need input.</returns>
    public virtual bool AllowsInput() => false;

    /// <summary>
    /// Whether or not the token will produce multiple outputs, depending on the input to the token.
    /// </summary>
    /// <param name="input">Input to token.</param>
    /// <returns>True - every token can have multiple outputs.</returns>
    public virtual bool CanHaveMultipleValues(string? input = null) => true;

    /// <summary>Get whether the token is available for use.</summary>
    /// <returns>True if token ready, false otherwise.</returns>
    public virtual bool IsReady() => this.tokenCache is not null;

    /// <summary>Validate that the provided input arguments are valid.</summary>
    /// <param name="input">The input arguments, if any.</param>
    /// <param name="error">The validation error, if any.</param>
    /// <returns>Returns whether validation succeeded.</returns>
    /// <remarks>Expect zero arguments.</remarks>
    public virtual bool TryValidateInput(string? input, out string error)
    {
        error = string.Empty;
        return true;
    }

    /// <summary>Get the current values.</summary>
    /// <param name="input">The input arguments, if applicable.</param>
    /// <returns>Values for the token, if any.</returns>
    public virtual IEnumerable<string> GetValues(string input)
    {
        if (this.tokenCache is null)
        {
            yield break;
        }
        else if (input is null)
        {
            foreach (string str in this.tokenCache)
            {
                yield return str;
            }
        }
    }

    /// <summary>Get whether the token always chooses from a set of known values for the given input. Mutually exclusive with <see cref="HasBoundedRangeValues"/>.</summary>
    /// <param name="input">The input arguments, if any.</param>
    /// <param name="allowedValues">The possible values for the input.</param>
    /// <returns>True if the inputs are bounded, false otherwise.</returns>
    public virtual bool HasBoundedValues(string input, out IEnumerable<string> allowedValues)
    {
        allowedValues = Booleans;
        return false;
    }

    /// <summary>Update the values when the context changes.</summary>
    /// <returns>Returns whether the value changed, which may trigger patch updates.</returns>
    public abstract bool UpdateContext();

    /// <summary>
    /// Checks a List of strings against the cache, updates the cache if necessary.
    /// </summary>
    /// <param name="newValues">The new values for the token.</param>
    /// <returns>true if cache updated, false otherwise.</returns>
    protected bool UpdateCache(List<string>? newValues)
    {
        if (newValues == this.tokenCache)
        {
            return false;
        }
        else
        {
            this.tokenCache = newValues;
            return true;
        }
    }
}
