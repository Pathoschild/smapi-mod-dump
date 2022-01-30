/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

using DaLion.Stardew.Professions.Framework.SuperMode;

namespace DaLion.Stardew.Professions.Framework.Events.Multiplayer;

#region using directives

using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;

#endregion using directives

internal class ToggledSuperModeModMessageReceivedEvent : ModMessageReceivedEvent
{
    /// <inheritdoc />
    protected override void OnModMessageReceivedImpl(object sender, ModMessageReceivedEventArgs e)
    {
        if (e.FromModID != ModEntry.Manifest.UniqueID || !e.Type.StartsWith("ToggledSuperMode")) return;

        var key = e.ReadAs<SuperModeIndex>();
        if (!ModEntry.State.Value.ActivePeerSuperModes.ContainsKey(key))
            ModEntry.State.Value.ActivePeerSuperModes[key] = new();

        switch (e.Type.Split('/')[1])
        {
            case "On":
                Log.D($"Player {e.FromPlayerID} has enabled Super Mode.");
                ModEntry.State.Value.ActivePeerSuperModes[key].Add(e.FromPlayerID);
                var glowingColor = key switch
                {
                    SuperModeIndex.Brute => Color.OrangeRed,
                    SuperModeIndex.Poacher => Color.MediumPurple,
                    SuperModeIndex.Desperado => Color.DarkGoldenrod,
                    SuperModeIndex.Piper => Color.LimeGreen,
                    _ => Color.White
                };
                Game1.getFarmer(e.FromPlayerID).startGlowing(glowingColor, false, 0.05f);
                break;

            case "Off":
                Log.D($"Player {e.FromPlayerID}'s Super Mode has ended.");
                ModEntry.State.Value.ActivePeerSuperModes[key].Remove(e.FromPlayerID);
                Game1.getFarmer(e.FromPlayerID).stopGlowing();
                break;
        }
    }
}