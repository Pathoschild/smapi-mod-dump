/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using CombatLevelDamageScaler.Framework;
using CombatLevelDamageScaler.Patches;
using Spacechase.Shared.Harmony;
using SpaceShared;
using SpaceShared.APIs;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace CombatLevelDamageScaler
{
    internal class Mod : StardewModdingAPI.Mod
    {
        public static Configuration Config;

        public override void Entry(IModHelper helper)
        {
            Log.Monitor = this.Monitor;
            Mod.Config = helper.ReadConfig<Configuration>();

            HarmonyPatcher.Apply(this,
                new GameLocationPatcher()
            );

            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            var capi = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (capi != null)
            {
                capi.RegisterModConfig(this.ModManifest, () => Mod.Config = new Configuration(), () => this.Helper.WriteConfig(Mod.Config));
                capi.RegisterSimpleOption(this.ModManifest, "Damage Scale", "The amount of damage to scale up per combat level, in percentage.", () => (int)(Mod.Config.DamageScalePerLevel * 100), (int val) => Mod.Config.DamageScalePerLevel = val / 100f);
            }
        }
    }
}
