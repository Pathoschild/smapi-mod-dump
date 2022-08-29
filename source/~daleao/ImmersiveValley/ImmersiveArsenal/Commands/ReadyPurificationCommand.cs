/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Arsenal.Commands;

#region using directives

using Common;
using Common.Commands;
using Common.Extensions.Stardew;
using StardewValley.Tools;
using System.Linq;

#endregion using directives

[UsedImplicitly]
internal sealed class ReadyPurificationCommand : ConsoleCommand
{
    /// <summary>Construct an instance.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal ReadyPurificationCommand(CommandHandler handler)
        : base(handler) { }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "ready_dark_sword", "ready_sword", "ready_purify" };

    /// <inheritdoc />
    public override string Documentation => "Ready a currently held Dark Sword for purification.";

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        var darkSword = Game1.player.Items.FirstOrDefault(item => item is MeleeWeapon
        {
            InitialParentTileIndex: Constants.DARK_SWORD_INDEX_I
        });
        if (darkSword is null)
        {
            Log.W("You are not carrying the Dark Sword.");
            return;
        }

        darkSword.Write("EnemiesSlain", ModEntry.Config.RequiredKillCountToPurifyDarkSword.ToString());
    }
}