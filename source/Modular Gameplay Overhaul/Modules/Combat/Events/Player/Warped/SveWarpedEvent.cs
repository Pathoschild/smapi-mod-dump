/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Combat.Events.Player.Warped;

#region using directives

using DaLion.Overhaul.Modules.Combat.Enums;
using DaLion.Shared.Constants;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class SveWarpedEvent : WarpedEvent
{
    /// <summary>Initializes a new instance of the <see cref="SveWarpedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal SveWarpedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnWarpedImpl(object? sender, WarpedEventArgs e)
    {
        if (e.NewLocation.Name != "Custom_TreasureCave")
        {
            return;
        }

        //var obtained = e.Player.Read(DataKeys.GalaxyArsenalObtained).ParseList<int>();
        //if (obtained.Count >= 4)
        //{
        e.NewLocation.setTileProperty(10, 7, "Buildings", "Success", $"W {WeaponIds.LavaKatana} 1");
        //}
        //else
        //{
        //    var chosen = new[]
        //    {
        //        (int)ObjectId.GalaxySword,
        //        (int)ObjectId.GalaxyHammer,
        //        (int)ObjectId.GalaxyDagger,
        //        (int)ObjectId.GalaxySlingshot,
        //    }.Except(obtained).Choose();

        //    e.NewLocation.setTileProperty(10, 7, "Buildings", "Success", $"W {chosen} 1");
        //}

        if (Virtue.Valor.Proven())
        {
            return;
        }

        e.Player.Write(Virtue.Valor.Name, int.MaxValue.ToString());
        Game1.chatBox.addMessage(I18n.Virtues_Recognize_Yoba(), Color.Green);
        CombatModule.State.HeroQuest?.UpdateTrialProgress(Virtue.Valor);
    }
}
