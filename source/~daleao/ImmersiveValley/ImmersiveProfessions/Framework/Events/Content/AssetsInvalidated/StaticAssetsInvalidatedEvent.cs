/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Events.Content;

#region using directives

using Common.Events;
using JetBrains.Annotations;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using System.Linq;
using Textures;

#endregion using directives

[UsedImplicitly]
internal sealed class StaticAssetsInvalidatedEvent : AssetsInvalidatedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal StaticAssetsInvalidatedEvent(ProfessionEventManager manager)
        : base(manager)
    {
        AlwaysHooked = true;
    }

    /// <inheritdoc />
    protected override void OnAssetsInvalidatedImpl(object? sender, AssetsInvalidatedEventArgs e)
    {
        if (e.NamesWithoutLocale.Any(name => name.IsEquivalentTo($"{ModEntry.Manifest.UniqueID}/SkillBars")))
        {
            Textures.BarsTx =
                ModEntry.ModHelper.GameContent.Load<Texture2D>($"{ModEntry.Manifest.UniqueID}/SkillBars");
        }

        if (e.NamesWithoutLocale.Any(name => name.IsEquivalentTo($"{ModEntry.Manifest.UniqueID}/UltimateMeter")))
        {
            Textures.MeterTx =
                ModEntry.ModHelper.GameContent.Load<Texture2D>($"{ModEntry.Manifest.UniqueID}/UltimateMeter");
        }

        if (e.NamesWithoutLocale.Any(name => name.IsEquivalentTo($"{ModEntry.Manifest.UniqueID}/PrestigeProgression")))
        {
            Textures.ProgressionTx =
                ModEntry.ModHelper.GameContent.Load<Texture2D>($"{ModEntry.Manifest.UniqueID}/PrestigeProgression");
        }
    }
}