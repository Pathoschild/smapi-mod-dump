/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using ExtendedReach.Framework;
using ExtendedReach.Patches;
using Spacechase.Shared.Patching;
using SpaceShared;
using SpaceShared.APIs;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace ExtendedReach
{
    /// <summary>The mod entry point.</summary>
    internal class Mod : StardewModdingAPI.Mod
    {
        /*********
        ** Fields
        *********/
        private Configuration Config;

        /// <summary>Handles the logic for rendering wiggly arms.</summary>
        private WigglyArmsRenderer WigglyArmsRenderer;


        /*********
        ** Public methods
        *********/
        /// <inheritdoc />
        public override void Entry(IModHelper helper)
        {
            Log.Monitor = this.Monitor;
            this.Config = helper.ReadConfig<Configuration>();
            this.WigglyArmsRenderer = new(helper.Input, helper.Reflection);

            helper.Events.Display.RenderedWorld += this.OnRenderWorld;
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;

            HarmonyPatcher.Apply(this,
                new TileRadiusPatcher()
            );
        }


        /*********
        ** Private methods
        *********/
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            var gmcm = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (gmcm == null)
                return;

            gmcm.RegisterModConfig(this.ModManifest, () => this.Config = new Configuration(), () => this.Helper.WriteConfig(this.Config));
            gmcm.RegisterSimpleOption(this.ModManifest, "Wiggly Arms", "Show wiggly arms reaching out to your cursor.", () => this.Config.WigglyArms, value => this.Config.WigglyArms = value);
        }

        private void OnRenderWorld(object sender, RenderedWorldEventArgs e)
        {
            if (Context.IsPlayerFree && this.Config.WigglyArms)
                this.WigglyArmsRenderer.Render(e.SpriteBatch);
        }
    }
}
