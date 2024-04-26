/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TheMightyAmondee/Pravoloxinone
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using HarmonyLib;
using StardewValley.Buffs;
using StardewValley.Extensions;
using StardewModdingAPI.Utilities;

namespace Pravoloxinone
{
    public class ModEntry
        : Mod
    {
        private static readonly string[] debuffs = { "12", "14", "17", "25", "26", "27" };
        private static IMonitor monitor;
        private static IModHelper helper;
        private static ModConfig staticconfig;
        public static readonly PerScreen<bool> deathbypravoloxinone = new PerScreen<bool>();
        internal ModConfig config;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
           
            this.Helper.Events.Content.AssetRequested += this.AssetRequested;
            this.Helper.Events.GameLoop.GameLaunched += this.GameLaunched;
            this.Helper.Events.GameLoop.SaveLoaded += this.SaveLoaded;
            i18n.gethelpers(helper.Translation);

            try
            {
                this.config = this.Helper.ReadConfig<ModConfig>();
            }
            catch
            {
                this.config = new ModConfig();
                this.Monitor.Log("Failed to parse config file, default values will be used.", LogLevel.Warn);
            }

            staticconfig = this.config;
            monitor = this.Monitor;
            ModEntry.helper = this.Helper;
            
            var value = config.DebuffChance + config.BuffChance + config.DeathChance + config.DamageChance;
            if (value > 0.999)
            {
                value = 1;
            }

            if (value != 1)
            {
                this.Monitor.Log($"Effect chances in config file don't add to one, default values will be used.", LogLevel.Warn);
                config.DebuffChance = 0.2f;
                config.BuffChance = 0.65f;
                config.DamageChance = 0.1f;
                config.DeathChance = 0.05f;
            }

           

            var harmony = new Harmony(this.ModManifest.UniqueID);

            harmony.Patch(
              original: AccessTools.Method(typeof(Farmer), nameof(Farmer.doneEating)),
              postfix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.doneEating_Postfix))
          );
            harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.checkForEvents)),
                prefix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.checkForEvents_Prefix))
          );

            harmony.Patch(
               original: AccessTools.Method(typeof(Event), nameof(Event.exitEvent)),
               postfix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.exitEvent_Postfix))
          );
        }

        /// <summary>
        /// Raised after the game is launched, right before the first update tick. This happens once per game session (unrelated to loading saves). All mods are loaded and initialised at this point, so this is a good time to set up mod integrations.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            this.BuildConfigMenu();
        }

        /// <summary>
        /// Creates the config menu for GMCM if installed
        /// </summary>
        private void BuildConfigMenu()
        {
            void ValidateConfig()
            {
                if (config.DebuffChance + config.BuffChance + config.DeathChance + config.DamageChance != 1.0f)
                {
                    this.Monitor.Log($"Effect chances in config file don't add to one, default values will be used.", LogLevel.Warn);
                    config.DebuffChance = 0.2f;
                    config.BuffChance = 0.65f;
                    config.DamageChance = 0.10f;
                    config.DeathChance = 0.05f;
                }
                this.Helper.WriteConfig(this.config);
            }
            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null) return;

            // register mod
            configMenu.Register(
               mod: this.ModManifest,
               reset: () => this.config = new ModConfig(),
               save: () => ValidateConfig()
           );
            configMenu.AddParagraph(
                ModManifest,
                text: () => i18n.string_GMCM_ConfigReminder()
                );
            // add some config options
            configMenu.AddNumberOption(
                ModManifest,
                name: () => i18n.string_GMCM_BuffChance(),
                tooltip: () => i18n.string_GMCM_BuffChanceTooltip(),
                min: 0f,
                max: 1.0f,
                interval: 0.025f,
                getValue: () => config.BuffChance,
                setValue: value => config.BuffChance = value
            );
            configMenu.AddNumberOption(
                ModManifest,
                name: () => i18n.string_GMCM_DebuffChance(),
                tooltip: () => i18n.string_GMCM_DebuffChanceTooltip(),
                min: 0f,
                max: 1.0f,
                interval: 0.025f,
                getValue: () => config.DebuffChance,
                setValue: value => config.DebuffChance = value
            );
            configMenu.AddNumberOption(
                ModManifest,
                name: () => i18n.string_GMCM_DamageChance(),
                tooltip: () => i18n.string_GMCM_DamageChanceTooltip(),
                min: 0f,
                max: 1.0f,
                interval: 0.025f,
                getValue: () => config.DamageChance,
                setValue: value => config.DamageChance = value
            );
            configMenu.AddNumberOption(
               ModManifest,
               name: () => i18n.string_GMCM_DeathChance(),
               tooltip: () => i18n.string_GMCM_DeathChanceTooltip(),
               min: 0f,
               max: 1.0f,
               interval: 0.025f,
               getValue: () => config.DeathChance,
               setValue: value => config.DeathChance = value
           );

        }

        /// <summary>
        /// Raised after loading a save (including the first day after creating a new save), or connecting to a multiplayer world. This happens right before DayStarted; at this point the save file is read and Context.IsWorldReady is true. This event isn't raised after saving; if you want to do something at the start of each day, see DayStarted instead.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            if (Game1.player.eventsSeen.Contains("57") == true && Game1.player.mailbox.Contains("PravoloxinoneRecipe") == false)
            {
                if (Game1.player.mailReceived.Contains("PravoloxinoneRecipe") == false)
                {
                    Game1.player.mailReceived.Add("PravoloxinoneRecipe");
                }

                if (Game1.player.craftingRecipes.ContainsKey(i18n.string_Pravoloxinone()) == false)
                {
                    Game1.player.craftingRecipes.Add(i18n.string_Pravoloxinone(), 0);
                }                               
            }
        }

        /// <summary>
        /// Raised when an asset is being requested from the content pipeline. The asset isn't necessarily being loaded yet (e.g. the game may be checking if it exists). You can register the changes you want to apply using the event arguments below; they'll be applied when the asset is actually loaded. See the content API for more info. If the asset is requested multiple times in the same tick (e.g.once to check if it exists and once to load it), SMAPI might only raise the event once and reuse the cached result.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Data\\Objects"))
            {
                e.Edit(asset =>
                {
                    // Why does this non-DRY method not give me null refernce exceptions? But creating a new instance does?
                    string texturename = this.Helper.ModContent.GetInternalAssetName("assets/Pravoloxinone.png").Name;
                    var objects = asset.AsDictionary<string, StardewValley.GameData.Objects.ObjectData>();

                    objects.Data["TheMightyAmondee.Pravoloxinone_Pravoloxinone"] = new StardewValley.GameData.Objects.ObjectData();
                    objects.Data["TheMightyAmondee.Pravoloxinone_Pravoloxinone"].Name = "TheMightyAmondee.Pravoloxinone/Pravoloxinone";
                    objects.Data["TheMightyAmondee.Pravoloxinone_Pravoloxinone"].DisplayName = i18n.string_Pravoloxinone();
                    objects.Data["TheMightyAmondee.Pravoloxinone_Pravoloxinone"].Description = i18n.string_Pravoloxinone_Description();
                    objects.Data["TheMightyAmondee.Pravoloxinone_Pravoloxinone"].Type = "Crafting";
                    objects.Data["TheMightyAmondee.Pravoloxinone_Pravoloxinone"].Category = 0;
                    objects.Data["TheMightyAmondee.Pravoloxinone_Pravoloxinone"].Price = 1000;
                    objects.Data["TheMightyAmondee.Pravoloxinone_Pravoloxinone"].Edibility = 20;
                    objects.Data["TheMightyAmondee.Pravoloxinone_Pravoloxinone"].IsDrink = true;
                    objects.Data["TheMightyAmondee.Pravoloxinone_Pravoloxinone"].Buffs = null;
                    objects.Data["TheMightyAmondee.Pravoloxinone_Pravoloxinone"].GeodeDropsDefaultItems = false;
                    objects.Data["TheMightyAmondee.Pravoloxinone_Pravoloxinone"].GeodeDrops = null;
                    objects.Data["TheMightyAmondee.Pravoloxinone_Pravoloxinone"].ArtifactSpotChances = null;
                    objects.Data["TheMightyAmondee.Pravoloxinone_Pravoloxinone"].ExcludeFromFishingCollection = false;
                    objects.Data["TheMightyAmondee.Pravoloxinone_Pravoloxinone"].ExcludeFromShippingCollection = false;
                    objects.Data["TheMightyAmondee.Pravoloxinone_Pravoloxinone"].ExcludeFromRandomSale = true;
                    objects.Data["TheMightyAmondee.Pravoloxinone_Pravoloxinone"].Texture = texturename;
                    objects.Data["TheMightyAmondee.Pravoloxinone_Pravoloxinone"].SpriteIndex = 0;
                    objects.Data["TheMightyAmondee.Pravoloxinone_Pravoloxinone"].ContextTags = new List<string> { "color_green", "medicine_item" };

                });
                  
            }

            else if (e.NameWithoutLocale.IsEquivalentTo("Data\\CraftingRecipes"))
            {
                e.Edit(asset =>
                {
                    var recipes = asset.AsDictionary<string,string>();
                    recipes.Data[i18n.string_Pravoloxinone()] = "349 1 351 1 306 1/Field/TheMightyAmondee.Pravoloxinone_Pravoloxinone/false/null";
                });

            }

            else if (e.NameWithoutLocale.IsEquivalentTo("Data\\mail"))
            {
                e.Edit(asset =>
                {
                    var recipes = asset.AsDictionary<string, string>();
                    recipes.Data["PravoloxinoneRecipe"] = $"{i18n.string_HarveyMail()}%item craftingRecipe {i18n.string_Pravoloxinone()} %%[#]Pravoloxinone recipe";
                });

            }

            else if (e.NameWithoutLocale.IsEquivalentTo("Data\\Shops"))
            {
                e.Edit(asset =>
                {
                    var shops = asset.AsDictionary<string, StardewValley.GameData.Shops.ShopData>();
                    var pravoloxinoneitem = new StardewValley.GameData.Shops.ShopItemData()
                    {
                        Price = 2000,
                        Id = "TheMightyAmondee.Pravoloxinone_Pravoloxinone",
                        ItemId = "TheMightyAmondee.Pravoloxinone_Pravoloxinone",
                        Condition = "PLAYER_HAS_MAIL Current PravoloxinoneRecipe Received"
                    };

                    shops.Data["Hospital"].Items.Add(pravoloxinoneitem);
                });

            }
        } 
        
        /// <summary>
        /// Code to run before the game checks for events to play
        /// </summary>
        /// <param name="__instance">The location instance the player is in</param>
        /// <returns>Whether the original code should run</returns>
        public static bool checkForEvents_Prefix(GameLocation __instance)
        {
            try
            {
                if (Game1.killScreen == true && Game1.eventUp == false && deathbypravoloxinone.Value == true)
                {
                    var events = helper.ModContent.Load<Dictionary<string, string>>("assets\\Events.json");
                    __instance.currentEvent = new Event(string.Format(events["PlayerKilled_Pravoloxinone"], i18n.string_HarveySpeak1(), i18n.string_HarveySpeak2(), i18n.string_HarveySpeak3()));
                    deathbypravoloxinone.Value = false;

                    if (__instance.currentEvent != null)
                    {
                        Game1.eventUp = true;
                    }
                    Game1.changeMusicTrack("none", track_interruptable: true);
                    Game1.killScreen = false;
                    Game1.player.health = 10;
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                monitor.Log($"Failed in {nameof(checkForEvents_Prefix)}:\n{ex}", LogLevel.Error);
                return true;
            }
           
        }

        /// <summary>
        /// Code to run after an event has been skipped or concluded
        /// </summary>
        /// <param name="__instance">The event that just finished</param>
        public static void exitEvent_Postfix(Event __instance)
        {
            try
            {
                if(__instance.id == "57" && Game1.player.mailForTomorrow.Contains("PravoloxinoneRecipe") == false && Game1.player.mailReceived.Contains("PravoloxinoneRecipe") == false)
                {
                    Game1.player.mailForTomorrow.Add("PravoloxinoneRecipe");
                }
            }
            catch (Exception ex)
            {
                monitor.Log($"Failed in {nameof(exitEvent_Postfix)}:\n{ex}", LogLevel.Error);
            }
        }

        /// <summary>
        /// Code to run after the player has consumed an item.
        /// </summary>
        /// <param name="__instance"></param>
        public static void doneEating_Postfix(Farmer __instance)
        {
            try
            {
                // Return early if nothing was consumed
                if (__instance.itemToEat == null)
                {
                    return;
                }

                StardewValley.Object consumed = __instance.itemToEat as StardewValley.Object;

                //Is item just consumed pravoloxinone?
                if (__instance.IsLocalPlayer && consumed != null && consumed.ItemId == "TheMightyAmondee.Pravoloxinone_Pravoloxinone")
                {
                    // Yes, try and apply effects
                    monitor.Log($"Pravoloxinone was consumed... I hope you're lucky");
                    TryApplyPravoloxinoneEffects();
                }
            }
            catch (Exception ex)
            {
                monitor.Log($"Failed in {nameof(doneEating_Postfix)}:\n{ex}", LogLevel.Error);
            }
        }

        public static void TryApplyPravoloxinoneEffects()
        {
            Random random = new Random();

            var chanceofeffect = random.NextDouble();

            if (chanceofeffect < staticconfig.BuffChance)
            {
                // Yes, apply a randomw buff to a randomly selected skill, of a random strength for 10 minutes
                var whichbuff = random.Next(0, 10);
                var buffstrength = random.Next(1, 5);
                var buffeffect = new BuffEffects();
                switch (whichbuff)
                {
                    case 0:
                        buffeffect.FarmingLevel.Value = buffstrength;
                        break;
                    case 1:
                        buffeffect.FishingLevel.Value = buffstrength;
                        break;
                    case 2:
                        buffeffect.MiningLevel.Value = buffstrength;
                        break;
                    case 3:
                        buffeffect.LuckLevel.Value = buffstrength;
                        break;
                    case 4:
                        buffeffect.ForagingLevel.Value = buffstrength;
                        break;
                    case 5:
                        buffeffect.MaxStamina.Value = buffstrength;
                        break;
                    case 6:
                        buffeffect.MagneticRadius.Value = buffstrength;
                        break;
                    case 7:
                        buffeffect.Speed.Value = buffstrength;
                        break;
                    case 8:
                        buffeffect.Defense.Value = buffstrength;
                        break;
                    case 9:
                        buffeffect.Attack.Value = buffstrength;
                        break;
                }
                var appliedbuff = new Buff("TheMightyAmondee.Buff.Pravoloxinone", i18n.string_Pravoloxinone(), i18n.string_Pravoloxinone(), 600000, effects: buffeffect, displayName: i18n.string_Pravoloxinone_Buff());
                Game1.player.applyBuff(appliedbuff);
            }

            else if (chanceofeffect < staticconfig.DebuffChance + staticconfig.BuffChance)
            {
                // Yes, Apply a random debuff from the list, also randomly selected for 60 seconds
                var applieddebuff = new Buff(random.ChooseFrom(debuffs))
                {
                    totalMillisecondsDuration = 60000,
                    millisecondsDuration = 60000,
                    source = i18n.string_Pravoloxinone(),
                    displaySource = i18n.string_Pravoloxinone()
                };
                Game1.player.applyBuff(applieddebuff);
            }

            else if (chanceofeffect < staticconfig.DebuffChance + staticconfig.BuffChance + staticconfig.DamageChance)
            {
                // Yes, subtract a random amount of health between 5 and 30 (22 added to compensate for health effect of drink)
                var healthtolose = Game1.player.health == Game1.player.maxHealth ? random.Next(5, 30) : random.Next(27, 52);
                // Lose determined health, or set health to 1 if player would die
                Game1.player.health = Math.Max(1, Game1.player.health - healthtolose);
            }

            else
            {
                // Yes, kill player
                Game1.player.health = 0;
                deathbypravoloxinone.Value = true;
            }
        }
    }
}