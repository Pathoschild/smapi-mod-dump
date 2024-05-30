/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/rtrox/LineSprinklersRedux
**
*************************************************/

global using SObject = StardewValley.Object;
using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.GameData.Buildings;
using StardewValley.GameData.Objects;
using StardewValley.GameData.BigCraftables;
using Microsoft.Xna.Framework.Graphics;
using LineSprinklersRedux.Framework.Data;
using StardewValley.GameData.Machines;

using LineSprinklersRedux.Framework;
using HarmonyLib;
using xTile.Tiles;
using LineSprinklersRedux.Framework.Patches;
using System.Diagnostics;


/* TODO NEXT 
 * 
 * 1. Cleanup Rotation Code to leverage the Direction Enum, & ModData to determine spriteIndex.
 * 2. Patch IsSprinkler
 * 3. Patch GetSprinklerTiles
 * 4. Test and see if it just works?
 * 5. Add Config for Directions, Keybinds
 * 
 * Others:
 * - Abstract out Asset Loading
 */


namespace LineSprinklersRedux
{

    public class ModEntry : Mod
    {
        internal static IMonitor? Mon;
        internal static IManifest? Manifest;
        internal static new IModHelper? Helper;

        internal LineSprinklersReduxMod? mod;
     
        public override void Entry(IModHelper helper)
        {
           this.mod = new LineSprinklersReduxMod(helper, Monitor, ModManifest);
            Mon = Monitor;
            Manifest = ModManifest;
            Helper = helper;

        }

        public override object? GetApi()
        {
            return this.mod!.GetApi();
        }
    }
    
    /// <summary>The mod entry point.</summary>
    internal sealed class LineSprinklersReduxMod
    {
        internal IModHelper Helper;
        internal IMonitor Monitor;
        internal IManifest ModManifest;
        internal ILineSprinklersReduxAPI api;

        internal static List<ObjectInformation>? SprinklersObjectsInfo;

        private ModConfig Config = null!;

        public LineSprinklersReduxMod(IModHelper helper, IMonitor Monitor, IManifest ModManifest)
        {
            this.Helper = helper;
            this.Monitor = Monitor;
            this.ModManifest = ModManifest;

            this.api = new LineSprinklerReduxAPI();
            this.Config = helper.ReadConfig<ModConfig>();

            SprinklersObjectsInfo = Helper.Data.ReadJsonFile<List<ObjectInformation>>("assets/data.json");
            if (SprinklersObjectsInfo == null)
            {
                this.Monitor.Log("Could not load Sprinkler Information from data.json, is this mod correctly installed?");
                SprinklersObjectsInfo = new List<ObjectInformation>();
                return;
            }

            helper.Events.Input.ButtonsChanged += this.OnButtonsChanged;
            helper.Events.Content.AssetRequested += this.OnAssetRequested;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        }

        public object GetApi()
        {
            return api;
        }

        /// <inheritdoc cref="IGameLoopEvents.GameLaunched"/>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            new GenericModConfigMenuForLineSprinklers(
                modRegistry: this.Helper.ModRegistry,
                modManifest: this.ModManifest,
                i18n: this.Helper.Translation,
                getConfig: () => this.Config,
                reset: () =>
                {
                    this.Config = new ModConfig();
                    this.Helper.WriteConfig(this.Config);
                },
                saveAndApply: () => this.Helper.WriteConfig(this.Config)
            ).Register();

            var harmony = new Harmony(ModManifest.UniqueID);

            BaseGamePatches.Apply(harmony);
        }

        /// <inheritdoc cref="IInputEvents.ButtonsChanged"/>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonsChanged(object? sender, ButtonsChangedEventArgs e)
        {
            if (this.Config.RotateSprinklerKeybindList.JustPressed())
            {
                Vector2 tile = this.Helper.Input.GetCursorPosition().GrabTile;
                var obj = Game1.player.currentLocation.getObjectAtTile((int)tile.X, (int)tile.Y);
                if (obj == null)
                {
                    obj = Game1.player.ActiveObject;
                };

                if (Sprinkler.IsLineSprinkler(obj))
                {
                    Sprinkler.Rotate(obj);
                }

            }
        }

        /// <inheritdoc cref="IContentEvents.AssetRequested"/>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Data/BigCraftables"))
            {
                e.Edit((Action<IAssetData>)(asset =>
                {
                    var data = asset.AsDictionary<string, BigCraftableData>().Data;
                    foreach (var item in SprinklersObjectsInfo ??= new List<ObjectInformation>())
                    {
                        var id = $"{this.ModManifest.UniqueID}_{item.Id}";
                        if (item.Object == null)
                        {
                            this.Monitor.Log($"Could not register object {id}, Object is null.");
                            continue;
                        }
                        item.Object.DisplayName = Helper.Translation.Get($"{item.Id}.DisplayName");
                        item.Object.Description = Helper.Translation.Get($"{item.Id}.Description");
                        item.Object.Texture = string.Format(item.Object.Texture, this.ModManifest.UniqueID);

                        // Store a copy of the baseSprite so it's preserved when we rotate a sprinkler;
                        CustomFields.SetBaseSprite(item.Object);

                        // Label all assets so they can be referenced easily in patches.
                        if (item.Object.ContextTags == null) item.Object.ContextTags = new();
                        item.Object.ContextTags.Add(ModConstants.MainContextTag);

                        if (item.Object.CustomFields.ContainsKey("Range"))
                        {
                            if (!int.TryParse(item.Object.CustomFields["Range"], out var range))
                            {
                                this.Monitor.Log($"Could Not Parse Range Custom Field on {item.Id}: {item.Object.CustomFields["Range"]}", LogLevel.Warn);
                            }
                            else
                            {
                                this.Monitor.Log($"Range on {id}: {range}", LogLevel.Debug);
                                CustomFields.SetRange(item.Object, range);
                            }

                        }
                        data[id] = item.Object;
                    }
                }));
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Data/Objects"))
            {
                e.Edit(asset =>
                {
                    // Dummy Object to let the base game handle loading these textures.
                    var data = asset.AsDictionary<string, ObjectData>().Data;
                    var id = ModConstants.OverlayDummyItemID;
                    var obj = new ObjectData
                    {
                        Name = id,
                        DisplayName = id,
                        Description = id,
                        Texture = $"/Mods/{this.ModManifest.UniqueID}/Overlays",
                        SpriteIndex = 0
                    };
                    data[id] = obj;
                });
            }
            if (e.NameWithoutLocale.IsEquivalentTo("Data/CraftingRecipes"))
            {
                e.Edit(asset =>
                {
                    var data = asset.AsDictionary<string, string>().Data;
                    foreach (var item in SprinklersObjectsInfo ??= new List<ObjectInformation>())
                    {
                        var id = $"{this.ModManifest.UniqueID}_{item.Id}";
                        if (item.Recipe == null) continue;

                        data[id] = string.Format(item.Recipe, id);
                    }
                });
            }

            if (e.NameWithoutLocale.IsEquivalentTo($"/Mods/{this.ModManifest.UniqueID}/Objects"))
            {
                e.LoadFromModFile<Texture2D>("assets/LineSprinklers.png", AssetLoadPriority.Exclusive);
            }
            if (e.NameWithoutLocale.IsEquivalentTo($"/Mods/{this.ModManifest.UniqueID}/Overlays"))
            {
                e.LoadFromModFile<Texture2D>("assets/Overlays.png", AssetLoadPriority.Exclusive);
            }
            if (e.NameWithoutLocale.IsEquivalentTo($"/Mods/{this.ModManifest.UniqueID}/Animations"))
            {
                e.LoadFromModFile<Texture2D>("assets/Animations.png", AssetLoadPriority.Exclusive);
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Data/Machines"))
            {
                e.Edit(asset =>
                {
                    var editor = asset.AsDictionary<string, MachineData>();
                    var recyclingMachine = editor.Data["(BC)20"];
                    foreach (var item in SprinklersObjectsInfo ??= new List<ObjectInformation>())
                    {
                        if (item.RecyclerOutput == null) continue;
                        var id = $"{this.ModManifest.UniqueID}_{item.Id}";
                        MachineOutputRule recyclerRule = new()
                        {
                            Id = id,
                            UseFirstValidOutput = true,
                            MinutesUntilReady = 60,
                            Triggers = new(),
                            OutputItem = new(),
                        };
                        recyclerRule.Triggers.Add(new MachineOutputTriggerRule
                        {
                            Trigger = MachineOutputTrigger.ItemPlacedInMachine,
                            RequiredItemId = id,
                            RequiredCount = 1,
                        });
                        recyclerRule.OutputItem.Add(new MachineItemOutput
                        {
                            ItemId = item.RecyclerOutput,
                        });
                        recyclingMachine.OutputRules.Add(recyclerRule);
                    }
                });
            }
        }
    }
}