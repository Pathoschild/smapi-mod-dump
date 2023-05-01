/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/CJBok/SDV-Mods
**
*************************************************/

using System.Collections.Generic;
using CJBCheatsMenu.Framework.Components;
using StardewValley.Menus;

namespace CJBCheatsMenu.Framework.Cheats.Advanced
{
    /// <summary>A cheat which unlocks locked game content.</summary>
    internal class UnlockContentCheat : BaseCheat
    {
        /*********
        ** Public methods
        *********/
        /// <summary>Get the config UI fields to show in the cheats menu.</summary>
        /// <param name="context">The cheat context.</param>
        public override IEnumerable<OptionsElement> GetFields(CheatContext context)
        {
            yield return new CheatsOptionsCheckbox(
                label: I18n.Flags_UnlockedContent_DyesAndTailoring(),
                value: this.HasEvent(992559),
                setValue: value => this.SetEvent(992559, value)
            );
            yield return new CheatsOptionsCheckbox(
                label: I18n.Flags_UnlockedContent_JunimoText(),
                value: this.HasFlag("canReadJunimoText"),
                setValue: value => this.SetFlag(value, "canReadJunimoText")
            );
        }
    }
}
