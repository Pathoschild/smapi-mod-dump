/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ophaneom/Let-Me-Rest
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using LetMeRest.Framework.Common;
using LetMeRest.Framework.Lists;
using LetMeRest.Framework.Multiplayer;

namespace LetMeRest
{
    public class ModEntry : Mod
    {
        public static ModEntry instance;
        public static Data data;
        public static ModConfig config;

        public override void Entry(IModHelper helper)
        {
            instance = this;
            config = helper.ReadConfig<ModConfig>() ?? new ModConfig();

            DataBase.GetDataBase();

            helper.Events.GameLoop.UpdateTicked += this.onUpdate;
            helper.Events.GameLoop.SaveLoaded += this.onSaveLoaded;
            helper.Events.Multiplayer.PeerConnected += this.OnPeerConnected;
            helper.Events.Multiplayer.ModMessageReceived += this.OnModMessageReceived;

            helper.ConsoleCommands.Add("rest_reset_data", "Resets the datafile", Commands.cm_ResetData);
            helper.ConsoleCommands.Add("rest_enable_buffs", "Enable/disable buff status\nUsage: rest_enable_buffs <true/false>", Commands.cm_OnChangeBuffs);
            helper.ConsoleCommands.Add("rest_enable_riding", "Enable/disable the riding verification\nUsage: rest_enable_riding <true/false>", Commands.cm_OnChangeRiding);
            helper.ConsoleCommands.Add("rest_enable_sitting", "Enable/disable the sitting verification\nUsage: rest_enable_sitting <true/false>", Commands.cm_OnChangeSitting);
            helper.ConsoleCommands.Add("rest_enable_secrets", "Enable/disable the secrets verification\nUsage: rest_enable_secrets <true/false>", Commands.cm_OnChangeSecrets);
            helper.ConsoleCommands.Add("rest_enable_standing", "Enable/disable the standing verification\nUsage: rest_enable_standing <true/false>", Commands.cm_OnChangeStanding);
            helper.ConsoleCommands.Add("rest_change_multiplier", "Changes the stamina multiplier\nUsage: rest_change_multiplier <multiplier_value>", Commands.cm_OnChangeMultiplier);
        }

        private void onUpdate(object sender, UpdateTickedEventArgs e)
        {
            Check.IsUsingTool();
            if (Check.IsPaused() || Check.AreInEvent()) return;
            Check.IsRecoveringStamina();
        }

        private void onSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            NetController.OnSaveLoaded();
        }

        private void OnPeerConnected(object sender, PeerConnectedEventArgs e)
        {
            NetController.SyncSpecificPlayer(e.Peer.PlayerID);
        }

        public void OnModMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            NetController.OnMessageReceived(e);
        }
    }
}
