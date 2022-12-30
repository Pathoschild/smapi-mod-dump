/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.BetterChests.Framework;

/// <summary>
///     Implementation of a Better Chest feature.
/// </summary>
internal abstract class Feature
{
    private bool _isActivated;

    /// <summary>
    ///     Sets whether the feature is currently activated.
    /// </summary>
    /// <param name="value">A value indicating whether the feature is currently enabled.</param>
    public void SetActivated(bool value)
    {
        if (this._isActivated == value)
        {
            return;
        }

        this._isActivated = value;
        if (this._isActivated)
        {
            this.Activate();
            return;
        }

        this.Deactivate();
    }

    /// <summary>
    ///     Activate this feature.
    /// </summary>
    protected abstract void Activate();

    /// <summary>
    ///     Deactivate this feature.
    /// </summary>
    protected abstract void Deactivate();
}