/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/zack20136/ChestFeatureSet
**
*************************************************/

using ChestFeatureSet.CraftFromChests;
using ChestFeatureSet.Framework;
using ChestFeatureSet.MoveChests;
using ChestFeatureSet.StashToChests;
using StardewModdingAPI;

namespace ChestFeatureSet
{
    public class ModEntry : Mod
    {
        public ModConfig Config { get; private set; }

        internal StashToChestsModule? StashToChests { get; private set; }
        internal LockItemsModule? LockItems { get; private set; }
        internal CraftFromChestsModule? CraftFromChests { get; private set; }
        internal MoveChestsModule? MoveChests { get; private set; }

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.Config = helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.GameLaunched += (sender, e) => this.OnGameLaunched();
            helper.Events.GameLoop.SaveLoaded += (sender, e) => this.LoadModules();
            helper.Events.GameLoop.ReturnedToTitle += (sender, e) => this.UnloadModules();
        }

        private void OnGameLaunched()
        {
            ConfigMenu.StartConfigMenu(this.Helper, this.ModManifest, this.Config);
        }

        private void LoadModules()
        {
            LocationExtension.SetCustomArea(this.Config.CustomArea);

            if (this.Config.StashToChests)
            {
                this.StashToChests = new StashToChestsModule(this);
                this.StashToChests.Activate();
            }
            if (this.Config.LockItems)
            {
                this.LockItems = new LockItemsModule(this);
                this.LockItems.Activate();
            }
            if (this.Config.CraftFromChests)
            {
                this.CraftFromChests = new CraftFromChestsModule(this);
                this.CraftFromChests.Activate();
            }
            if (this.Config.MoveChests)
            {
                this.MoveChests = new MoveChestsModule(this);
                this.MoveChests.Activate();
            }
        }

        private void UnloadModules()
        {
            if (this.StashToChests != null)
            {
                this.StashToChests.Deactivate();
                this.StashToChests = null;
            }
            if (this.LockItems != null)
            {
                this.LockItems.Deactivate();
                this.LockItems = null;
            }
            if (this.CraftFromChests != null)
            {
                this.CraftFromChests.Deactivate();
                this.CraftFromChests = null;
            }
            if (this.MoveChests != null)
            {
                this.MoveChests.Deactivate();
                this.MoveChests = null;
            }
        }
    }
}
