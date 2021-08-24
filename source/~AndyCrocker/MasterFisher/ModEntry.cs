/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AndyCrocker/StardewMods
**
*************************************************/

using MasterFisher.Models;
using SatoCore;
using SatoCore.Extensions;
using StardewModdingAPI;
using System.Collections.Generic;

namespace MasterFisher
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : ModBase
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The loaded fish categories.</summary>
        public Repository<FishCategory, string> Categories { get; private set; }

        /// <summary>The loaded location areas.</summary>
        public Repository<LocationArea, string> LocationAreas { get; private set; }

        /// <summary>The loaded bait.</summary>
        public Repository<Bait, string> Bait { get; private set; }

        /// <summary>The singleton instance of <see cref="ModEntry"/>.</summary>
        public static ModEntry Instance { get; private set; }


        /*********
        ** Protected Methods
        *********/
        /// <inheritdoc/>
        protected override void Entry()
        {
            Instance = this;

            Categories = new Repository<FishCategory, string>(this.Monitor);
            LocationAreas = new Repository<LocationArea, string>(this.Monitor);
            Bait = new Repository<Bait, string>(this.Monitor);

            this.Helper.Events.GameLoop.SaveLoaded += (sender, e) => LoadContentPacks();

            this.Helper.ConsoleCommands.Add("mf_summary", "Logs the current state of all fish information.\n\nUsage: mf_summary", (command, args) => CommandManager.LogSummary());
        }

        /// <inheritdoc/>
        protected override void InitialiseContentPackLoading()
        {
            Categories.Clear();
            LocationAreas.Clear();
            Bait.Clear();
        }

        /// <inheritdoc/>
        protected override void LoadContentPack(IContentPack contentPack)
        {
            // categories
            if (contentPack.TryLoadAsset<List<FishCategory>>("categories.json", out var fishCategories))
                Categories.StageItems(fishCategories);

            // location areas
            if (contentPack.TryLoadAsset<List<LocationArea>>("locations.json", out var locations))
                LocationAreas.StageItems(locations);

            // bait
            if (contentPack.TryLoadAsset<List<Bait>>("bait.json", out var bait))
                Bait.StageItems(bait);
        }

        /// <inheritdoc/>
        protected override void FinaliseContentPackLoading()
        {
            Categories.ProcessStagedItems();
            LocationAreas.ProcessStagedItems();
            Bait.ProcessStagedItems();
        }
    }
}
