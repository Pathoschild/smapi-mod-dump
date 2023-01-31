/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using StardewModdingAPI.Events;

namespace LastDayToPlantRedux.Framework;

/// <summary>
/// Manages multiplayer stuff for this mod.
/// </summary>
internal static class MultiplayerManager
{
    /// <summary>
    /// Whether or the the code should check for a prestiged agriculturalist, for Walk of Life.
    /// </summary>
    private static bool shouldCheckPrestiged = false;

    /// <summary>
    /// Gets the farmer used as the agriculturalist farmer.
    /// </summary>
    internal static WeakReference<Farmer>? AgriculturalistFarmer { get; private set; } = null;

    /// <summary>
    /// Gets the farmer used as the prestiged agriculturalist farmer.
    /// </summary>
    internal static WeakReference<Farmer>? PrestigedAgriculturalistFarmer { get; private set; } = null;

    /// <summary>
    /// Gets the farmer who doesn't have any professions.
    /// </summary>
    internal static WeakReference<Farmer>? NormalFarmer { get; private set; } = null!;

    /// <summary>
    /// Checks to see if WoL is installed.
    /// </summary>
    /// <param name="registry">ModRegistry.</param>
    internal static void SetShouldCheckPrestiged(IModRegistry registry)
    {
        shouldCheckPrestiged = registry.IsLoaded("DaLion.ImmersiveProfessions") || registry.IsLoaded("DaLion.Overhaul");
    }

    /// <summary>
    /// Clears the references to these farmers.
    /// </summary>
    internal static void Reset()
    {
        AgriculturalistFarmer = null;
        PrestigedAgriculturalistFarmer = null;
    }

    /// <summary>
    /// Refresh farmers on day start.
    /// </summary>
    internal static void UpdateOnDayStart()
    {
        NormalFarmer = null;
        AgriculturalistFarmer = null;
        PrestigedAgriculturalistFarmer = null;
        if (!Context.IsMultiplayer)
        {
            _ = AssignProfessionFarmersIfNeeded(Game1.player);
        }
        else if (Context.ScreenId == 0)
        {
            IEnumerator<Farmer>? farmers = Game1.getOnlineFarmers().GetEnumerator();

            while ((NormalFarmer is null || AgriculturalistFarmer is null
            || (shouldCheckPrestiged && PrestigedAgriculturalistFarmer is null))
            && farmers.MoveNext())
            {
                _ = AssignProfessionFarmersIfNeeded(farmers.Current);
            }
        }
    }

    /// <summary>
    /// Checks to see if a newly connected farmer should try to be assigned a role.
    /// </summary>
    /// <param name="e">Event args.</param>
    internal static void OnPlayerConnected(PeerConnectedEventArgs e)
    {
        Farmer farmer = Game1.getFarmer(e.Peer.PlayerID);
        _ = AssignProfessionFarmersIfNeeded(farmer);

        if (!e.Peer.IsSplitScreen)
        {
            FarmerWatcher watcher = new();
            farmer.professions.OnArrayReplaced += watcher.Professions_OnArrayReplaced;
            farmer.professions.OnElementChanged += watcher.Professions_OnElementChanged;
        }
    }

    /// <summary>
    /// Removes a farmer from a role if they were disconnected.
    /// </summary>
    /// <param name="e">Event args.</param>
    internal static void OnPlayerDisconnected(PeerDisconnectedEventArgs e)
    {
        if (NormalFarmer is not null
            && (!NormalFarmer.TryGetTarget(out Farmer? normal) || normal.UniqueMultiplayerID == e.Peer.PlayerID))
        {
            NormalFarmer = null;
        }

        if (AgriculturalistFarmer is not null
            && (!AgriculturalistFarmer.TryGetTarget(out Farmer? farmer) || farmer.UniqueMultiplayerID == e.Peer.PlayerID))
        {
            AgriculturalistFarmer = null;
        }

        if (shouldCheckPrestiged && PrestigedAgriculturalistFarmer is not null
            && (!PrestigedAgriculturalistFarmer.TryGetTarget(out Farmer? prestigeFarmer) || prestigeFarmer.UniqueMultiplayerID == e.Peer.PlayerID))
        {
            PrestigedAgriculturalistFarmer = null;
        }

        IEnumerator<Farmer>? farmers = Game1.getOnlineFarmers().GetEnumerator();

        while ((NormalFarmer is null || AgriculturalistFarmer is null
            || (shouldCheckPrestiged && PrestigedAgriculturalistFarmer is null))
            && farmers.MoveNext())
        {
            _ = AssignProfessionFarmersIfNeeded(farmers.Current);
        }
    }

    private static bool AssignProfessionFarmersIfNeeded(Farmer farmer)
    {
        if (shouldCheckPrestiged && PrestigedAgriculturalistFarmer is null && farmer.professions.Contains(Farmer.agriculturist + 100))
        {
            ModEntry.ModMonitor.Log($"Assigning {farmer.Name} as prestiged agricultralist farmer.");
            PrestigedAgriculturalistFarmer = new WeakReference<Farmer>(farmer);
            return true;
        }
        else if (AgriculturalistFarmer is null && farmer.professions.Contains(Farmer.agriculturist)
            && !farmer.professions.Contains(Farmer.agriculturist + 100))
        {
            ModEntry.ModMonitor.Log($"Assigning {farmer.Name} as argicultralist farmer.");
            AgriculturalistFarmer = new WeakReference<Farmer>(farmer);
            return true;
        }
        else if (NormalFarmer is null && !farmer.professions.Contains(Farmer.agriculturist)
            && !farmer.professions.Contains(Farmer.agriculturist + 100))
        {
            ModEntry.ModMonitor.Log($"Assigning {farmer.Name} as normal farmer.");
            NormalFarmer = new WeakReference<Farmer>(farmer);
            return true;
        }

        return false;
    }
}
