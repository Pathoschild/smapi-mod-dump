/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using BirbShared;
using BirbShared.Config;
using BirbShared.Command;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using BirbShared.Asset;
using BirbShared.APIs;

namespace RanchingToolUpgrades
{
    internal class ModEntry : Mod
    {
        public static ModEntry Instance;
        public static Config Config;
        public static Assets Assets;

        public static ISpaceCore SpaceCore;

        public override void Entry(IModHelper helper)
        {
            Instance = this;
            Log.Init(this.Monitor);

            Config = helper.ReadConfig<Config>();

            Assets = new Assets();
            new AssetClassParser(this, Assets).ParseAssets();

            this.Helper.Events.GameLoop.GameLaunched += this.GameLoop_GameLaunched;
        }

        private void GameLoop_GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            new ConfigClassParser(this, Config).ParseConfigs();
            HarmonyPatches.Patch(this.ModManifest.UniqueID);
            new CommandClassParser(this.Helper.ConsoleCommands, new Command()).ParseCommands();

            SpaceCore = this.Helper.ModRegistry
                .GetApi<ISpaceCore>
                ("spacechase0.SpaceCore");
            if (SpaceCore is null)
            {
                Log.Error("Can't access the SpaceCore API. Is the mod installed correctly?");
            }

            SpaceCore.RegisterSerializerType(typeof(UpgradeablePail));
            SpaceCore.RegisterSerializerType(typeof(UpgradeableShears));
        }
    }
}
