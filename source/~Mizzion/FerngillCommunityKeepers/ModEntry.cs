/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

using System;
using FerngillCommunityKeepers.Framework;
using FerngillCommunityKeepers.Framework.Configs;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace FerngillCommunityKeepers
{
    public class ModEntry : Mod
    {
        /*
         *
         *  ToDo:
         * Make config files: main config, tree config, crop config, farm animal config.
         * Add in translations
         * Code the main part of the mod.
         * Add in the completion of tasks.
         *
         */
        private FckConfig _config;
        private FckHelper _fck;

        public override void Entry(IModHelper helper)
        {
            _config = helper.ReadConfig<FckConfig>();
            _fck = new FckHelper(this.Monitor, this.Helper);

            //Lets set up the events

            helper.Events.GameLoop.DayStarted += Events_GameLoop_DayStarted;
        }

        private void Events_GameLoop_DayStarted(object sender, DayStartedEventArgs e)
        {

        }
    }
}
