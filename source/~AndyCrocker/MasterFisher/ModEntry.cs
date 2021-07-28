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
using StardewModdingAPI;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MasterFisher
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : ModBase
    {
        /*********
        ** Fields
        *********/
        /// <summary>The fish categories that have been parsed from content packs.</summary>
        /// <remarks>This is used to temporarily store all categories before populating the repository, so edits and deletions can work correctly.</remarks>
        private readonly List<FishCategory> CategoriesBeingLoaded = new List<FishCategory>();

        /// <summary>The location areas that have been parsed from content packs.</summary>
        /// <remarks>This is used to temporarily store all location areas before populating the repository, so edits and deletions can work correctly.</remarks>
        private readonly List<LocationArea> LocationAreasBeingLoaded = new List<LocationArea>();

        /// <summary>The bait that have been parsed from content packs.</summary>
        /// <remarks>This is used to temporarily store all bait before populating the repository, so edits and deletions can work correctly.</remarks>
        private readonly List<Bait> BaitBeingLoaded = new List<Bait>();


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
            CategoriesBeingLoaded.Clear();
            LocationAreasBeingLoaded.Clear();
        }

        /// <inheritdoc/>
        protected override void LoadContentPack(IContentPack contentPack)
        {
            // categories
            if (File.Exists(Path.Combine(contentPack.DirectoryPath, "categories.json")))
                CategoriesBeingLoaded.AddRange(contentPack.LoadAsset<List<FishCategory>>("categories.json"));

            // location areas
            if (File.Exists(Path.Combine(contentPack.DirectoryPath, "locations.json")))
                LocationAreasBeingLoaded.AddRange(contentPack.LoadAsset<List<LocationArea>>("locations.json"));

            // bait
            if (File.Exists(Path.Combine(contentPack.DirectoryPath, "bait.json")))
                BaitBeingLoaded.AddRange(contentPack.LoadAsset<List<Bait>>("bait.json"));
        }

        /// <inheritdoc/>
        protected override void FinaliseContentPackLoading()
        {
            // categories
            foreach (var category in CategoriesBeingLoaded.Where(category => category.Action == Action.Add))
                Categories.Add(category);
            foreach (var category in CategoriesBeingLoaded.Where(category => category.Action == Action.Edit))
                Categories.Edit(category);
            foreach (var category in CategoriesBeingLoaded.Where(category => category.Action == Action.Delete))
                Categories.Delete(category.Name);

            // location areas
            foreach (var locationArea in LocationAreasBeingLoaded.Where(locationArea => locationArea.Action == Action.Add))
                LocationAreas.Add(locationArea);
            foreach (var locationArea in LocationAreasBeingLoaded.Where(locationArea => locationArea.Action == Action.Edit))
                LocationAreas.Edit(locationArea);
            foreach (var locationArea in LocationAreasBeingLoaded.Where(locationArea => locationArea.Action == Action.Delete))
                LocationAreas.Delete(locationArea.UniqueName);

            // bait
            foreach (var bait in BaitBeingLoaded.Where(bait => bait.Action == Action.Add))
                Bait.Add(bait);
            foreach (var bait in BaitBeingLoaded.Where(bait => bait.Action == Action.Edit))
                Bait.Edit(bait);
            foreach (var bait in BaitBeingLoaded.Where(bait => bait.Action == Action.Delete))
                Bait.Delete(bait.ObjectId);
        }
    }
}
