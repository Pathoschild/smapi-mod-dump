/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace XSLite;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Common.Helpers;
using Common.Helpers.ItemMatcher;
using Common.Integrations.DynamicGameAssets;
using Common.Integrations.GenericModConfigMenu;
using Common.Integrations.XSLite;
using Common.Integrations.XSPlus;
using Microsoft.Xna.Framework.Graphics;
using Migrations.JsonAsset;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;

/// <inheritdoc />
public class XSLiteApi : IXSLiteApi
{
    private static readonly HashSet<string> VanillaNames = new()
    {
        "Chest",
        "Stone Chest",
        "Junimo Chest",
        "Mini-Shipping Bin",
        "Mini-Fridge",
        "Auto-Grabber",
    };
    private readonly DynamicGameAssetsIntegration _dynamicAssets;

    private readonly IModHelper _helper;
    private readonly ItemMatcher _itemMatcher = new(string.Empty);
    private readonly GenericModConfigMenuIntegration _modConfigMenu;
    private readonly XSPlusIntegration _xsPlus;

    /// <summary>
    ///     Initializes a new instance of the <see cref="XSLiteApi" /> class.
    /// </summary>
    /// <param name="helper">Provides simplified APIs for writing mods.</param>
    internal XSLiteApi(IModHelper helper)
    {
        this._helper = helper;
        this._dynamicAssets = new(helper.ModRegistry);
        this._modConfigMenu = new(helper.ModRegistry);
        this._xsPlus = new(helper.ModRegistry);
    }

    /// <inheritdoc />
    public bool LoadContentPack(IManifest manifest, string path)
    {
        var contentPack = this._helper.ContentPacks.CreateTemporary(
            path,
            manifest.UniqueID,
            manifest.Name,
            manifest.Description,
            manifest.Author,
            manifest.Version);

        return this.LoadContentPack(contentPack);
    }

    /// <inheritdoc />
    public bool LoadContentPack(IContentPack contentPack)
    {
        Log.Info($"Loading {contentPack.Manifest.Name} {contentPack.Manifest.Version}");

        var storages = contentPack.ReadJsonFile<IDictionary<string, Storage>>("expanded-storage.json");

        if (storages is null)
        {
            Log.Warn($"Nothing to load from {contentPack.Manifest.Name} {contentPack.Manifest.Version}");
            return false;
        }

        // Remove any duplicate storages
        foreach (var storage in storages.Where(storage => XSLite.Storages.ContainsKey(storage.Key)))
        {
            Log.Warn($"Duplicate storage {storage.Key} in {contentPack.Manifest.UniqueID}.");
            storages.Remove(storage.Key);
        }

        if (storages.Count == 0)
        {
            Log.Warn($"Nothing to load from {contentPack.Manifest.Name} {contentPack.Manifest.Version}");
            return false;
        }

        // Setup GMCM for Content Pack
        Dictionary<string, ModConfig> config = null;
        if (storages.Any(storage => storage.Value.PlayerConfig))
        {
            if (this._modConfigMenu.IsLoaded)
            {
                void Reset()
                {
                    foreach (var storage in storages)
                    {
                        storage.Value.Config.Capacity = storage.Value.Capacity;
                        storage.Value.Config.EnabledFeatures = storage.Value.EnabledFeatures;
                    }
                }

                void Save()
                {
                    contentPack.WriteJsonFile(
                        "config.json",
                        storages.ToDictionary(
                            storage => storage.Key,
                            storage => storage.Value.Config));
                }
                    
                this._modConfigMenu.API.Register(contentPack.Manifest, Reset, Save);

                // Add a page for each storage
                foreach (var storage in storages)
                {
                    this._modConfigMenu.API.AddPageLink(
                        contentPack.Manifest,
                        storage.Key,
                        () => storage.Key);
                }
            }

            config = contentPack.ReadJsonFile<Dictionary<string, ModConfig>>("config.json");
        }

        config ??= new();

        // Load expanded storages
        foreach (var storage in storages)
        {
            storage.Value.Name = storage.Key;
            storage.Value.Manifest = contentPack.Manifest;
            storage.Value.Format = XSLiteApi.VanillaNames.Contains(storage.Value.Name) ? Storage.AssetFormat.Vanilla : Storage.AssetFormat.DynamicGameAssets;

            // Load base texture
            if (!string.IsNullOrWhiteSpace(storage.Value.Image) && !contentPack.HasFile($"{storage.Value.Image}") && contentPack.HasFile($"assets/{storage.Value.Image}"))
            {
                storage.Value.Image = Path.Combine("assets", storage.Value.Image);
            }

            if (!string.IsNullOrWhiteSpace(storage.Value.Image) && contentPack.HasFile(storage.Value.Image))
            {
                var texture = contentPack.LoadAsset<Texture2D>(storage.Value.Image);
                XSLite.Textures.Add(storage.Key, texture);
                storage.Value.ReloadTexture(texture);
            }

            if ((storage.Value.Width > 16 || storage.Value.Height > 32) && !this._xsPlus.IsLoaded)
            {
                Log.Warn($"Cannot load {storage.Key} from {contentPack.Manifest.Name} {contentPack.Manifest.Version}\nXSPlus is required for bigger storages!");
                continue;
            }

            // Add to config
            if (!config.TryGetValue(storage.Key, out var storageConfig))
            {
                storageConfig = new()
                {
                    Capacity = storage.Value.Capacity,
                    EnabledFeatures = storage.Value.EnabledFeatures,
                };

                config.Add(storage.Key, storageConfig);
            }

            storage.Value.Config = storageConfig;

            // Enable XSPlus features
            if (this._xsPlus.IsLoaded)
            {
                // Opt-in to Expanded Menu
                this._xsPlus.API.EnableWithModData("ExpandedMenu", $"{XSLite.ModPrefix}/Storage", storage.Key, true);

                // Enable bigger chests
                if (storage.Value.Width > 16 || storage.Value.Height > 32)
                {
                    this._xsPlus.API.EnableWithModData("BiggerChest", $"{XSLite.ModPrefix}/Storage", storage.Key, new Tuple<int, int, int>(storage.Value.Width, storage.Value.Height, storage.Value.Depth));
                }

                // Enable filtering items
                if (storage.Value.FilterItems.Any())
                {
                    this._xsPlus.API.EnableWithModData("FilterItems", $"{XSLite.ModPrefix}/Storage", storage.Key, storage.Value.FilterItems);
                }

                // Enable additional capacity
                if (storageConfig.Capacity != 0)
                {
                    this._xsPlus.API.EnableWithModData("Capacity", $"{XSLite.ModPrefix}/Storage", storage.Key, storageConfig.Capacity);
                }

                // Enable Open Nearby
                if (storage.Value.OpenNearby > 0)
                {
                    this._xsPlus.API.EnableWithModData("OpenNearby", $"{XSLite.ModPrefix}/Storage", storage.Key, storage.Value.OpenNearby);
                }

                // Disable color picker if storage does not support player color
                if (!storage.Value.PlayerColor)
                {
                    this._xsPlus.API.EnableWithModData("ColorPicker", $"{XSLite.ModPrefix}/Storage", storage.Key, false);
                }

                // Enable other toggleable features
                foreach (var featureName in storageConfig.EnabledFeatures)
                {
                    this._xsPlus.API.EnableWithModData(featureName, $"{XSLite.ModPrefix}/Storage", storage.Key, true);
                }

                // Disable features from
                foreach (var featureName in storage.Value.DisabledFeatures)
                {
                    this._xsPlus.API.EnableWithModData(featureName, $"{XSLite.ModPrefix}/Storage", storage.Key, false);
                }
            }

            // Add GMCM page for storage
            if (this._modConfigMenu.IsLoaded && storage.Value.PlayerConfig)
            {
                Func<bool> OptionGet(string featureName)
                {
                    return () => storageConfig.EnabledFeatures.Contains(featureName);
                }

                Action<bool> OptionSet(string featureName)
                {
                    return value =>
                    {
                        if (value)
                        {
                            storageConfig.EnabledFeatures.Add(featureName);
                        }
                        else
                        {
                            storageConfig.EnabledFeatures.Remove(featureName);
                        }

                        this._xsPlus.API?.EnableWithModData(featureName, $"{XSLite.ModPrefix}/Storage", storage.Key, value);
                    };
                }

                this._modConfigMenu.API.AddPage(contentPack.Manifest, storage.Key);
                this._modConfigMenu.API.AddSectionTitle(contentPack.Manifest, () => storage.Key);

                this._modConfigMenu.API.AddNumberOption(
                    contentPack.Manifest,
                    name: () => this._helper.Translation.Get("config.capacity.name"),
                    tooltip: () => this._helper.Translation.Get("config.capacity.tooltip"),
                    getValue: () => storageConfig.Capacity,
                    setValue: value =>
                    {
                        storageConfig.Capacity = value;
                        this._xsPlus.API?.EnableWithModData("Capacity", $"{XSLite.ModPrefix}/Storage", storage.Key, value);
                    });

                this._modConfigMenu.API.AddBoolOption(
                    contentPack.Manifest,
                    name: () => this._helper.Translation.Get("config.access-carried.name"),
                    tooltip: () => this._helper.Translation.Get("config.access-carried.tooltip"),
                    getValue: OptionGet("AccessCarried"),
                    setValue: OptionSet("AccessCarried"));

                this._modConfigMenu.API.AddBoolOption(
                    contentPack.Manifest,
                    name: () => this._helper.Translation.Get("config.can-carry.name"),
                    tooltip: () => this._helper.Translation.Get("config.can-carry.tooltip"),
                    getValue: OptionGet("CanCarry"),
                    setValue: OptionSet("CanCarry"));

                this._modConfigMenu.API.AddBoolOption(
                    contentPack.Manifest,
                    name: () => this._helper.Translation.Get("config.craft-from-chest.name"),
                    tooltip: () => this._helper.Translation.Get("config.craft-from-chest.tooltip"),
                    getValue: OptionGet("CraftFromChest"),
                    setValue: OptionSet("CraftFromChest"));

                this._modConfigMenu.API.AddBoolOption(
                    contentPack.Manifest,
                    name: () => this._helper.Translation.Get("config.stash-to-chest.name"),
                    tooltip: () => this._helper.Translation.Get("config.stash-to-chest.tooltip"),
                    getValue: OptionGet("StashToChest"),
                    setValue: OptionSet("StashToChest"));

                this._modConfigMenu.API.AddBoolOption(
                    contentPack.Manifest,
                    name: () => this._helper.Translation.Get("config.vacuum-items.name"),
                    tooltip: () => this._helper.Translation.Get("config.vacuum-items.tooltip"),
                    getValue: OptionGet("VacuumItems"),
                    setValue: OptionSet("VacuumItems"));
            }

            // Alternative Textures Compat
            if (this._helper.ModRegistry.IsLoaded("PeacefulEnd.AlternativeTextures"))
            {
                storage.Value.ModData.Add("AlternativeTextureOwner", "Stardew.Default");
                storage.Value.ModData.Add("AlternativeTextureVariation", "-1");
            }

            XSLite.Storages.Add(storage.Key, storage.Value);
        }

        if (!this._dynamicAssets.IsLoaded)
        {
            return true;
        }

        if (Directory.Exists(Path.Combine(contentPack.DirectoryPath, "BigCraftables")))
        {
            var migrator = JsonAssetsMigrator.FromContentPack(contentPack);
            if (migrator is null)
            {
                return true;
            }

            if (contentPack.HasFile("content.json") || migrator.ToDynamicGameAssets(contentPack))
            {
                foreach (var jsonAsset in migrator.JsonAssets)
                {
                    if (!storages.TryGetValue(jsonAsset.Name, out var storage) || !string.IsNullOrWhiteSpace(storage.Image))
                    {
                        continue;
                    }

                    if (XSLite.Textures.ContainsKey(jsonAsset.Name) || !contentPack.HasFile($"assets/{jsonAsset.Name}.png"))
                    {
                        continue;
                    }

                    var texture = contentPack.LoadAsset<Texture2D>($"assets/{jsonAsset.Name}.png");
                    XSLite.Textures.Add(jsonAsset.Name, texture);
                    storage.Format = Storage.AssetFormat.JsonAssets;
                    storage.Frames = texture.Width / 16;
                    storage.PlayerColor = texture.Height > 32;
                }
            }
        }

        foreach (var storage in storages)
        {
            storage.Value.DisplayName = contentPack.Translation.Get($"big-craftable.{storage.Key}.name");
            storage.Value.Description = contentPack.Translation.Get($"big-craftable.{storage.Key}.description");
        }

        var manifest = new ContentPackManifest(contentPack.Manifest)
        {
            ContentPackFor = new ManifestContentPackFor
            {
                UniqueID = "spacechase0.DynamicGameAsset",
                MinimumVersion = null,
            },
            ExtraFields = new Dictionary<string, object>
            {
                {
                    "DGA.FormatVersion", "2"
                },
                {
                    "DGA.ConditionsFormatVersion", "1.23.0"
                },
            },
        };

        this._dynamicAssets.API.AddEmbeddedPack(manifest, contentPack.DirectoryPath);
        return true;
    }

    /// <inheritdoc />
    public bool AcceptsItem(Chest chest, Item item)
    {
        if (!chest.TryGetStorage(out var storage))
        {
            return true;
        }

        this._itemMatcher.SetSearch(storage.FilterItems);
        return this._itemMatcher.Matches(item);
    }

    /// <inheritdoc />
    public IEnumerable<string> GetAllStorages()
    {
        return XSLite.Storages.Keys;
    }

    /// <inheritdoc />
    public IEnumerable<string> GetOwnedStorages(IManifest manifest)
    {
        foreach (var storage in XSLite.Storages.Values)
        {
            if (storage.Manifest.UniqueID.Equals(manifest.UniqueID))
            {
                yield return storage.Name;
            }
        }
    }
}