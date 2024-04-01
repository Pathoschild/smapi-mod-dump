/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/noriteway/StardewMods
**
*************************************************/

using GenericModConfigMenu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewModdingAPI.Framework;
using StardewModdingAPI.Enums;
using StardewValley;
using StardewValley.Audio;
using StardewValley.BellsAndWhistles;
using StardewValley.Buffs;
using StardewValley.Buildings;
using StardewValley.Characters;
using StardewValley.Companions;
using StardewValley.ConsoleAsync;
using StardewValley.Constants;
using StardewValley.Delegates;
using StardewValley.Enchantments;
using StardewValley.Events;
using StardewValley.Extensions;
using StardewValley.GameData.Objects;
using StardewValley.GameData.BigCraftables;
using StardewValley.Logging;
using StardewValley.Tools;
using System.Linq;
using Object = StardewValley.Object;
using StardewValley.Pathfinding;
//using System.Numerics;

namespace FarmDrones
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        /*********
        ** Properties
        *********/
        /// <summary>The mod configuration from the player.</summary>
        private ModConfig? Config;
        //private bool needRefresh = false;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ModConfig>();

            helper.Events.Content.AssetRequested += OnAssetRequested;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.Input.ButtonPressed += OnButtonPressed;
        }

        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
        {
            /*if(e.NameWithoutLocale.IsEquivalentTo("Data/Objects"))
            {
                e.Edit(asset =>
                {
                    var data = asset.AsDictionary<string, ObjectData>().Data;
                    if(Config != null)
                    {
                        data["96"].Price = Config.price1;
                        data["97"].Price = Config.price2;
                        data["98"].Price = Config.price3;
                        data["99"].Price = Config.price4;
                    }
                });
            }*/
            if(e.NameWithoutLocale.IsEquivalentTo("TileSheets/DroneStation"))
            {
                e.LoadFromModFile<Texture2D>("assets/DroneStation.png", AssetLoadPriority.Medium);
                /*e.Edit(asset =>
                {
                    var editor = asset.AsImage();
                    IRawTextureData sourceImage = this.Helper.ModContent.Load<IRawTextureData>("custom-texture.png");
                    editor.PatchImage(sourceImage, targetArea: new Rectangle(300, 100, 200, 200));
                }
                );*/
            }
            if(e.NameWithoutLocale.IsEquivalentTo("Data/BigCraftables"))
            {
                //e.LoadFromModFile<StardewValley.Object>("assets/DroneStation.json", AssetLoadPriority.Medium);
                e.LoadFromModFile<Dictionary<String, BigCraftableData>>("assets/DroneStation.json", AssetLoadPriority.Medium);
            }
        }

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            // Get MCM API(if installed)
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            
            // Exit if no config
            if(configMenu is null) return;
            if(Config == null) return;

            // Register mod for MCM
            /*configMenu.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => OnSave()
            );

            // Add UI for MCM
            configMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => Config.price1,
                setValue: value => Config.price1 = (int)value,
                name: () => "Dwarf Scroll I",
                tooltip: () => "Sets the sell price of dwarf scroll I.",
                min: 1,
                fieldId: "ds1"
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => Config.price2,
                setValue: value => Config.price2 = (int)value,
                name: () => "Dwarf Scroll II",
                tooltip: () => "Sets the sell price of dwarf scroll II.",
                min: 1,
                fieldId: "ds2"
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => Config.price3,
                setValue: value => Config.price3 = (int)value,
                name: () => "Dwarf Scroll III",
                tooltip: () => "Sets the sell price of dwarf scroll III.",
                min: 1,
                fieldId: "ds3"
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => Config.price4,
                setValue: value => Config.price4 = (int)value,
                name: () => "Dwarf Scroll IV",
                tooltip: () => "Sets the sell price of dwarf scroll IV.",
                min: 1,
                fieldId: "ds4"
            );*/
        }

        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if(e.Button.IsUseToolButton())
            {
                if(Game1.player.CurrentTool is WateringCan)
                {
                    // need to make sure not just clicking in inventory while cursor on watering can

                    Vector2 cursorPos = e.Cursor.GrabTile;
                    int cursorX = (int)cursorPos.X;
                    int cursorY = (int)cursorPos.Y;
                    Object target = Game1.currentLocation.getObjectAtTile(cursorX, cursorY);
                    if(target != null && target.ItemId == "DroneStation")
                    {
                        //Monitor.Log("Watering " + target.ItemId + " at " + cursorPos.ToString(), LogLevel.Debug);

                        // Station refill visual effect(vanilla)
                        Utility.addSprinklesToLocation(Game1.currentLocation, cursorX, cursorY, 1, 1, 200, 20, Color.Aquamarine);

                        // Station refill sound effect(vanilla)
                        // slime rancher - fairy_heal, squid_move, dropItemInWater, cavedrip, squid_bubble, 
                        //                              dwop, jingle1, glug, squid_move, dropItemInWater, pullItemFromWater
                        //                              cavedrip, healSound, squid_bubble, qi_shop_purchase, statue_of_blessings
                        // robotSoundEffects
                        Game1.currentLocation.playSound("squid_move", cursorPos);

                        // Station open animation
                        TemporaryAnimatedSprite petalsOpen = new("TileSheets\\DroneStation", new Rectangle(0, 0, 16, 32), cursorPos, false, 0, Color.Black);
                        petalsOpen.holdLastFrame = true;
                        petalsOpen.totalNumberOfLoops = 0;
                        petalsOpen.bigCraftable = true;
                        Game1.currentLocation.temporarySprites.Add(petalsOpen);
                        foreach(var temp in Game1.currentLocation.TemporarySprites)
                        {
                            Monitor.Log(temp.textureName, LogLevel.Debug);
                        }
                    }
                    //get tile position through obj.TileLocation?
                    //getBoundingBox()
                    //GetBoundingBoxAt(x,y)
                    //GameLocation.tilePositionComparer
                    /*Utility.ForEachItemIn(Game1.currentLocation, item =>
                    {
                        //Monitor.Log(item.Name, LogLevel.Debug);
                        if(item.QualifiedItemId == "(BC)DroneStation")
                        {
                            // Get station location
                            //item.canBePlacedHere(GameLocation l, Vector2 tile, CollisionMask collisionMask = CollisionMask.All, bool showError = false)
                            foreach(var tag in item.GetContextTags()) Monitor.Log(tag, LogLevel.Debug);
                            
                        }
                        return true;
                    });
                    Utility.ForEachBuilding(building =>
                    {
                        //Monitor.Log(building.ToString(), LogLevel.Debug);

                        return true;
                    });
                    //ForEachItemIn
                    */
                }
            }
        }

        // Called when the MCM for this mod saves
        void OnSave()
        {
            /*if(Config != null)
            {
                Helper.WriteConfig(Config);

                // Apply new prices to object data
                Game1.objectData["96"].Price = Config.price1;
                Game1.objectData["97"].Price = Config.price2;
                Game1.objectData["98"].Price = Config.price3;
                Game1.objectData["99"].Price = Config.price4;
            }

            // Apply new prices to existing scrolls
            RefreshScrolls();

            // Need to also refresh once a save is loaded if changes are made in the main menu
            needRefresh = true;*/
        }
    }
}
