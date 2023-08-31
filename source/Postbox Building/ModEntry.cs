/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/i-saac-b/PostBoxMod
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;

namespace PostBoxMod
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {

        /*********
        ** Fields
        **********/

        private Texture2D PostboxTexture;
        private ModConfig Config;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            // Read in config file and create if needed
            try
            {
                this.Config = this.Helper.ReadConfig<ModConfig>();
            }
            catch (Exception)
            {
                this.Config = new ModConfig();
                this.Monitor.Log(this.Helper.Translation.Get("entry.badConfig"), LogLevel.Warn);
            }

            Postbox.Initialize(this.Monitor, this.Config, Helper);
            PostageMenu.Initialize(this.Monitor, Helper);

            Helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            Helper.Events.Content.AssetRequested += this.OnAssetRequested;
            Helper.Events.Display.MenuChanged += this.OnMenuChanged;
            Helper.Events.GameLoop.DayStarted += this.OnDayStarted;
            Helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            Helper.Events.Multiplayer.ModMessageReceived += this.OnModMessageReceived;

            this.PostboxTexture = Helper.ModContent.Load<Texture2D>("assets/Postbox.png");
        }

        /*********
        ** Private methods
        *********/
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Blueprints"))
            {
                e.Edit(this.EditBluePrints);
            }
            else if (e.Name.IsEquivalentTo("Buildings/Postbox"))
            {
                e.LoadFrom(() => this.PostboxTexture, AssetLoadPriority.Exclusive);
            }
        }

        private void OnModMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            if(e.FromModID == "i-saac-b.PostBoxMod"){
                PostageMessage message = e.ReadAs<PostageMessage>();
                Postbox.outgoing.Add(new Tuple<StardewValley.Object, string, Farmer>(new StardewValley.Object(Vector2.Zero, message.itemId, 0), message.receiver, Game1.getFarmer(message.senderId)));
            }
        }


        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            object spaceCore = Helper.ModRegistry.GetApi("spacechase0.SpaceCore");
            Helper.Reflection.GetMethod(spaceCore, "RegisterSerializerType").Invoke(typeof(Postbox));

            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            // register mod
            configMenu.Register(
                mod: this.ModManifest,
                reset: () => this.Config = new ModConfig(),
                save: () => this.Helper.WriteConfig(this.Config)
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Posted Gift Relationship Modifier",
                tooltip: () => "Input a percentage modifier for relationship points gained from mailed gifts. E.g., 100 = normal, 50 = half, 200 = double",
                getValue: () => (int)(this.Config.PostedGiftRelationshipModifier * 100),
                setValue: value => this.Config.PostedGiftRelationshipModifier = value / 100f
            );
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Postbox Cost",
                tooltip: () => "G cost for the Postbox",
                getValue: () => this.Config.PostboxCost,
                setValue: value => this.Config.PostboxCost = value
            );
            configMenu.AddTextOption(
                 mod: this.ModManifest,
                 name: () => "Postbox Material Cost",
                 getValue: () => this.Config.PostboxMaterialCost,
                 setValue: value => this.Config.PostboxMaterialCost = value,
                 allowedValues: new string[] { "Normal", "Free", "Expensive", "Endgame", "Custom" }
             );
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => "Custom Material Cost",
                tooltip: () => "Only used when Material Cost is set to Custom. Write a string of ItemIDs and quantities.",
                getValue: () => this.Config.CustomPostboxMaterialCost,
                setValue: value => this.Config.CustomPostboxMaterialCost = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Verbose Gifting",
                tooltip: () => "Receive chat messages listing sent gifts overnight.",
                getValue: () => this.Config.VerboseGifting,
                setValue: value => this.Config.VerboseGifting = value
            );
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            this.Helper.GameContent.InvalidateCacheAndLocalized("Data/Blueprints");
        }

        // add data to the blueprints xnb
        private void EditBluePrints(IAssetData asset)
        {
            int cost = Config.PostboxCost;
            string material = "335 3 330 5 390 50";
            switch (Config.PostboxMaterialCost) {
                case "Normal": break;
                case "Free": material = ""; break;
                case "Expensive": material = "335 10 336 5 337 1"; break;
                case "Endgame": material = "337 10 910 5 787 10 74 1"; break;
                case "Custom": material = Config.CustomPostboxMaterialCost; break;
                default: break;
            }
            asset.AsDictionary<string, string>().Data.Add("Postbox", $"{material}/3/2/-1/-1/-2/-1/null/{Helper.Translation.Get("blueprint-title")}/{Helper.Translation.Get("blueprint-description")}/Buildings/none/96/96/-1/null/Farm/{cost}/false");
        }

        //debugging
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            Monitor.Log("Checking for unconverted Postbox...", LogLevel.Debug);
            Farm farm = Game1.getFarm();
            for (int i = 0; i < farm.buildings.Count; ++i)  
            {
                Building building = farm.buildings[i];
                if (building.buildingType.Value == "Postbox" && !(building is Postbox))
                {
                    Monitor.Log("Detected " + building.ToString() + " with " + building.buildingType.Value, LogLevel.Debug);
                    farm.buildings[i] = new Postbox();
                    farm.buildings[i].buildingType.Value = building.buildingType.Value;
                    farm.buildings[i].daysOfConstructionLeft.Value = building.daysOfConstructionLeft.Value;
                    farm.buildings[i].indoors.Value = building.indoors.Value;
                    farm.buildings[i].tileX.Value = building.tileX.Value;
                    farm.buildings[i].tileY.Value = building.tileY.Value;
                    farm.buildings[i].tilesWide.Value = building.tilesWide.Value;
                    farm.buildings[i].tilesHigh.Value = building.tilesHigh.Value;
                    farm.buildings[i].load();
                    Monitor.Log("Converted.", LogLevel.Debug);
                }
            }
        }

        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (e.NewMenu is StardewValley.Menus.CarpenterMenu Menu)
            {
                if (!Helper.Reflection.GetField<bool>(Menu, "magicalConstruction").GetValue())
                {
                    var Blueprints = Helper.Reflection.GetField<List<BluePrint>>(Menu, "blueprints").GetValue();
                    Blueprints.Add(new BluePrint("Postbox"));
                }
            }
        }
    }
}