/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/alcmoe/FreezeTimeMultiplayer
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;

namespace FreezeTime
{
    public partial class FreezeTime
    {
        private ModConfig _config = null!;
        public class ModConfig
        {
            public string PauseLogic { get; set; } = "All";

            public bool Any()
            {
                return PauseLogic == "Any";
            }
        }

        private void GameLaunchedEvent(object? sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
        {
            _config = Helper.ReadConfig<ModConfig>();
            _checker = new FreezeTimeChecker(_config);
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;
            configMenu.Register(
                mod: ModManifest,
                reset: ResetConfig,
                save: WriteConfig
            );
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => "PauseLogic",
                getValue: () => _config.PauseLogic,
                setValue: value => _config.PauseLogic = value,
                allowedValues: ["All", "Any"]
            );
        }

        private void WriteConfig()
        {
            if (StardewValley.Game1.IsMultiplayer) {
                if (!StardewValley.Game1.player.IsMainPlayer) {
                    StardewValley.Game1.chatBox.addMessage("u can not change pauseLogic as a client player!", Color.Red);
                    return;
                }
                BroadcastConfig();
                StardewValley.Game1.chatBox.addMessage("Broadcasting config to  all clients", Color.Blue);
            }
            Helper.WriteConfig(_config);
        }

        private void ResetConfig()
        {
            if (StardewValley.Game1.IsMultiplayer) {
                if (!StardewValley.Game1.player.IsMainPlayer) {
                    return;
                }
            }
            _config = new ModConfig();
        }
    }

    public interface IGenericModConfigMenuApi
    {
        void Register(IManifest mod, Action reset, Action save, bool titleScreenOnly = false);
        void AddTextOption(IManifest mod, Func<string> getValue, Action<string> setValue, Func<string> name, Func<string>? tooltip = null, string[]? allowedValues = null, Func<string, string>? formatAllowedValue = null, string? fieldId = null);
    }
}