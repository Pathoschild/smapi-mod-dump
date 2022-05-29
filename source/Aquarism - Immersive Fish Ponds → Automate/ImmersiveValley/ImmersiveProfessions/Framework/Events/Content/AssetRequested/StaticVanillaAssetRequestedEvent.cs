/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Events.Content;

#region using directives

using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;

using Utility;

#endregion using directives

[UsedImplicitly]
internal class StaticVanillaAssetRequestedEvent : AssetRequestedEvent
{
    /// <summary>Construct an instance.</summary>
    internal StaticVanillaAssetRequestedEvent()
    {
        this.Enable();
    }

    /// <inheritdoc />
    protected override void OnAssetRequestedImpl(object sender, AssetRequestedEventArgs e)
    {
        if (e.NameWithoutLocale.IsEquivalentTo("TileSheets/BuffsIcons"))
        {
            e.Edit(asset =>
            {
                // patch modded profession buff icons
                var editor = asset.AsImage();
                editor.ExtendImage(192, 80);
                var srcArea = new Rectangle(0, 80, 96, 32);
                var targetArea = new Rectangle(0, 48, 96, 32);

                editor.PatchImage(Textures.Spritesheet, srcArea, targetArea);
            });
        }
        else if (e.NameWithoutLocale.IsEquivalentTo("LooseSprites/Cursors"))
        {
            e.Edit(asset =>
            {
                // patch modded profession icons
                var editor = asset.AsImage();
                var srcArea = new Rectangle(0, 0, 96, 80);
                var targetArea = new Rectangle(0, 624, 96, 80);

                editor.PatchImage(Textures.Spritesheet, srcArea, targetArea);
            });
        }
    }
}