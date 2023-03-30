/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using GrowableGiantCrops;
using HarmonyLib;
using JsonAssets;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;

namespace ExtraGingerIslandMaps
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            Mon = this.Monitor;
            Help = this.Helper;

            helper.Events.GameLoop.GameLaunched += OnGameLaunch;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.DayStarted += OnDayStart;

            helper.Events.Content.AssetRequested += Asset.Request;

            var harmony = new Harmony(this.ModManifest.UniqueID);

            harmony.PatchAll(typeof(ModEntry).Assembly);
            harmony.PatchAll(typeof(NPCPatches).Assembly);

            //monster related patches

            this.Monitor.Log($"Applying Harmony patch \"{nameof(BatPatches)}\": prefixing SDV method \"Bat.onDealContactDamage\".");
            harmony.PatchAll(typeof(BatPatches).Assembly);

            this.Monitor.Log($"Applying Harmony patch \"{nameof(MonsterPatches)}\": postfixing SDV method \"Monster.behaviorAtGameTick\".");
            harmony.PatchAll(typeof(MonsterPatches).Assembly);

            this.Monitor.Log($"Applying Harmony patch \"{nameof(ProjectilePatches)}\": prefixing SDV method \"BasicProjectile.behaviorOnCollisionWithPlayer\".");
            harmony.PatchAll(typeof(ProjectilePatches).Assembly);

        }

        private void OnGameLaunch(object sender, GameLaunchedEventArgs e)
        {
            jsonAssets = Helper.ModRegistry.GetApi<IApi>("spacechase0.JsonAssets");
            //giantCrops = Helper.ModRegistry.GetApi<IGrowableGiantCropsAPI>("modname");
            HasSGI = Helper.ModRegistry.Get("mistyspring.spousesisland") != null;

            CloudyBG = this.Helper.GameContent.Load<Texture2D>("LooseSprites/Cloudy_Ocean_BG");
            CloudyBG_night = this.Helper.GameContent.Load<Texture2D>("LooseSprites/Cloudy_Ocean_BG_Night");
            Ostrich = this.Helper.GameContent.Load<Texture2D>("Animals/Ostrich");
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            
            if(jsonAssets is not null)
            {
                FireRing = jsonAssets.GetObjectId("Fire Ring");
            }

            this.Monitor.Log("Fire Ring ID: " + FireRing);

            //get text used for GiEX.RopeToWest
            var texts = Helper.GameContent.Load<Dictionary<string, string>>("Strings/Locations");
            texts.TryGetValue("Mines_ShaftJumpIn",out string jump);
            this.Monitor.Log("Mines_ShaftJumpIn string: " + jump, LogLevel.Trace);
            
            //replace dot for question mark. if spanish, also add at start
            jump = jump.Replace('.', '?');
            if(LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.es)
            {
                jump = "¿" + jump;
            }

            RopeQuestion = jump;

            //we null it so any ContentPatcher changes can be applied in the future
            CloudyBG = null;
            CloudyBG_night = null;
            Ostrich = null;

            //get jackie text 
            UpsetJackie = Game1.content.LoadString("Characters/Dialogue/Jackie:StoleMoney");

            //get $ dialogue
            var coines = Game1.content.LoadString("Strings/StringsFromCSFiles:Debris.cs.625");
            var found = Game1.content.LoadString("Strings/UI:Collections_Description_MineralsFound");

            FoundG = $"({coines}) {found}$"; //format: "(Coins) Number found: x"

            //reset terrain for 1.4upd
            if (!Game1.player.mailReceived.Contains("GiEX_1.4update"))
            {
                var rivermouth = Utility.fuzzyLocationSearch("Custom_GiRiver");

                try
                {
                    Terrain.Reset(rivermouth);
                }
                catch (Exception ex)
                {
                    this.Monitor.Log("Error: " + ex, LogLevel.Error);
                    return;
                }
                Game1.player.mailReceived.Contains("GiEX_1.4update");
            }
        }

        private void OnDayStart(object sender, DayStartedEventArgs e)
        {
            //if player hasnt seen jackie's entry event: make invisible for the day
            if(Game1.player.eventsSeen.Contains(121951) == false)
            {
                var jackie = Game1.getCharacterFromName("Jackie",false);
                jackie.IsInvisible = true;
                jackie.daysUntilNotInvisible = 1;
                //Game1.removeCharacterFromItsLocation("Jackie", false);
            }

            //reset daily variable
            ModEntry.HasObtainedCashToday = false;
        }

        internal IApi jsonAssets;
        //internal static IGrowableGiantCropsAPI giantCrops;

        internal static IMonitor Mon { get; private set; }
        internal static IModHelper Help { get; private set; }
        
        internal static Texture2D CloudyBG { get; set; }
        internal static Texture2D CloudyBG_night { get; set; }
        internal static Texture2D Ostrich { get; set; }
        
        internal static int FireRing { get; private set; }
        internal static string RopeQuestion { get; private set; }
        internal static string FoundG { get; private set; }
        internal static string UpsetJackie { get; private set; }
        internal static bool HasSGI { get; private set; } = false;
        internal static bool HasObtainedCashToday { get; set; } = false;
    }
}
