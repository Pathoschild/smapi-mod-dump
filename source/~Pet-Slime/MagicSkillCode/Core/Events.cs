/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pet-Slime/StardewValley
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BirbCore.Attributes;
using MagicSkillCode.Objects;
using SpaceCore.Events;
using SpaceShared.APIs;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace MagicSkillCode.Core
{
    [SEvent]
    public class Events
    {
        [SEvent.GameLaunchedLate]
        private static void GameLaunched(object sender, GameLaunchedEventArgs e)
        {

            ModEntry.Editor = new(ModEntry.Config, ModEntry.Instance.Helper.ModContent, ModEntry.HasStardewValleyExpanded);
            // hook Mana Bar
            {
                var manaBar = ModEntry.Instance.Helper.ModRegistry.GetApi<IManaBarApi>("moonslime.ManaBarApi");
                if (manaBar == null)
                {
                    Log.Error("No mana bar API???");
                    return;
                }
                ModEntry.Mana = manaBar;
            }

            var helper = ModEntry.Instance.Helper;
            Log.Trace("Magic: Trying to Register skill.");
            Framework.Magic.Init(helper.Events, helper.Input, helper.ModRegistry, helper.Multiplayer.GetNewID);
        }


        [SEvent.AssetRequested]
        private static void AssetRequested(object sender, AssetRequestedEventArgs e)
        {
            ModEntry.Editor.TryEdit(e);
        }

        [SEvent.SaveLoaded]
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            if (!Context.IsMainPlayer)
                return;

            try
            {
                ModEntry.LegacyDataMigrator.OnSaveLoaded();
            }
            catch (Exception ex)
            {
                Log.Warn($"Exception migrating legacy save data: {ex}");
            }
        }

        [SEvent.DayStarted]
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            // fix player's magic info if needed
            Framework.Magic.FixMagicIfNeeded(Game1.player);
        }

        [SEvent.Saving]
        private void OnSaving(object sender, SavingEventArgs e)
        {
            if (!Context.IsMainPlayer)
                return;

            ModEntry.LegacyDataMigrator.OnSaved();

            ModEntry.Instance.Helper.Events.GameLoop.Saving -= this.OnSaving;
        }

    }
}
