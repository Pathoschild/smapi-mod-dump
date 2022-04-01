/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Events.Multiplayer;

#region using directives

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

using Extensions;
using Ultimate;

#endregion using directives

internal class ToggledUltimateModMessageReceivedEvent : ModMessageReceivedEvent
{
    /// <inheritdoc />
    protected override void OnModMessageReceivedImpl(object sender, ModMessageReceivedEventArgs e)
    {
        if (e.FromModID != ModEntry.Manifest.UniqueID || !e.Type.StartsWith("ToggledUltimate")) return;

        var who = Game1.getFarmer(e.FromPlayerID);
        if (who is null)
        {
            Log.W($"Unknown player {e.FromPlayerID} has toggled their Ultimate.");
            return;
        }

        var state = e.ReadAs<string>();
        UltimateIndex index;
        switch (state)
        {
            case "Active":
                Log.D($"{who.Name} activated their Ultimate.");
                index = who.ReadDataAs<UltimateIndex>(DataField.UltimateIndex);
                var glowingColor = index switch
                {
                    UltimateIndex.Brute => Color.OrangeRed,
                    UltimateIndex.Poacher => Color.MediumPurple,
                    UltimateIndex.Desperado => Color.DarkGoldenrod,
                    _ => Color.White
                };

                if (glowingColor != Color.White)
                    who.startGlowing(glowingColor, false, 0.05f);

                if (Context.IsMainPlayer && index == UltimateIndex.Poacher)
                    ModEntry.HostState.PoachersInAmbush.Add(e.FromPlayerID);

                break;

            case "Inactive":
                Log.D($"{who.Name}'s Ultimate has ended.");
                index = who.ReadDataAs<UltimateIndex>(DataField.UltimateIndex);
                who.stopGlowing();
                if (Context.IsMainPlayer && index == UltimateIndex.Poacher)
                    ModEntry.HostState.PoachersInAmbush.Remove(e.FromPlayerID);

                break;
        }
    }
}