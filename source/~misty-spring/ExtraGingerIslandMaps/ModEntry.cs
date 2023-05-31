/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using ExtraGingerIslandMaps.Patches;

namespace ExtraGingerIslandMaps
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            Mon = Monitor;
            Help = Helper;

            helper.Events.GameLoop.GameLaunched += OnGameLaunch;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.DayStarted += OnDayStart;

            helper.Events.Content.AssetRequested += Asset.Request;

            var harmony = new Harmony(ModManifest.UniqueID);

            GameLocationPatches.Apply(harmony);
            NpcPatches.Apply(harmony);
            MonsterPatches.Apply(harmony);
            BatPatches.Apply(harmony);
            ProjectilePatches.Apply(harmony);
        }

        private void OnGameLaunch(object sender, GameLaunchedEventArgs e)
        {
            //giantCrops = Helper.ModRegistry.GetApi<IGrowableGiantCropsAPI>("modname");
            HasSgi = Helper.ModRegistry.Get("mistyspring.spousesisland") != null;

            CloudyBg = Helper.GameContent.Load<Texture2D>("LooseSprites/Cloudy_Ocean_BG");
            Cursors = Helper.GameContent.Load<Texture2D>("LooseSprites/Cursors");
            CloudyBgNight = Helper.GameContent.Load<Texture2D>("LooseSprites/Cloudy_Ocean_BG_Night");
            Ostrich = Helper.GameContent.Load<Texture2D>("Animals/Ostrich");
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            HasJackie = Game1.player.mailReceived.Contains("Island_Resort");

            Monitor.Log("Fire Ring ID: " + FireRing);

            //get text used for GiEX.RopeToWest
            var texts = Helper.GameContent.Load<Dictionary<string, string>>("Strings/Locations");
            texts.TryGetValue("Mines_ShaftJumpIn",out var jump);
            Monitor.Log("Mines_ShaftJumpIn string: " + jump);
            
            //replace dot for question mark. if spanish, also add at start
            jump = jump?.Replace('.', '?');
            if(LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.es)
            {
                jump = "¿" + jump;
            }

            RopeQuestion = jump;

            //we null it so any ContentPatcher changes can be applied in the future
            CloudyBg = null;
            CloudyBgNight = null;
            Ostrich = null;
            Cursors = null;

            if(HasJackie)
            {
                //get jackie text 
                UpsetJackie = Game1.content.LoadString("Characters/Dialogue/JackieGiex:StoleMoney");
                UpdateJackieIfNeccesary();
            }

            //get $ dialogue
            var coines = Game1.content.LoadString("Strings/StringsFromCSFiles:Debris.cs.625");
            var found = Game1.content.LoadString("Strings/UI:Collections_Description_MineralsFound");

            FoundG = $"({coines}) {found}$"; //format: "(Coins) Number found: x"

            /*//reset terrain for 1.4upd
            if (Game1.player.mailReceived.Contains("GiEX_1.4update")) return;
            
            var rivermouth = Utility.fuzzyLocationSearch("Custom_GiRiver");
            try
            {
                Terrain.Reset(rivermouth);
            }
            catch (Exception ex)
            {
                Monitor.Log("Error: " + ex, LogLevel.Error);
                return;
            }
            Game1.player.mailReceived.Add("GiEX_1.4update");*/
        }

        private static void UpdateJackieIfNeccesary()
        {
            // ReSharper disable once InconsistentNaming
            var hasUpdatedTo1_6 = Game1.player.mailReceived.Contains("GiEX_1.6Update");
            if (hasUpdatedTo1_6)
                return;

            var hasPreviousJackie = Game1.player.friendshipData.ContainsKey("Jackie");
            if (!hasPreviousJackie)
            {
                Game1.player.mailReceived.Add("GiEX_1.6Update");
                return;
            }

            var jackie = Game1.player.friendshipData["Jackie"];
            Game1.player.friendshipData["JackieGiex"] = jackie;
            Game1.player.friendshipData.Remove("Jackie");
            
            Game1.player.mailReceived.Add("GiEX_1.6Update");
        }

        private static void OnDayStart(object sender, DayStartedEventArgs e)
        {
            //if player hasnt seen jackie's entry event: make invisible for the day
            if(Game1.player.eventsSeen.Contains("121951") == false && HasJackie)
            {
                var jackie = Game1.getCharacterFromName("Jackie",false);
                jackie.IsInvisible = true;
                jackie.daysUntilNotInvisible = 1;
                //Game1.removeCharacterFromItsLocation("Jackie", false);
            }

            //reset daily variable
            HasObtainedCashToday = false;
        }

        //internal static IGrowableGiantCropsAPI giantCrops;

        internal static IMonitor Mon { get; private set; }
        internal static IModHelper Help { get; private set; }
        internal static Texture2D Cursors { get; private set; }
        internal static Texture2D CloudyBg { get; private set; }
        internal static Texture2D CloudyBgNight { get; private set; }
        internal static Texture2D Ostrich { get; private set; }
        internal const string FireRing = "mistyspring.extraGImaps_fireRing";
        internal static string RopeQuestion { get; private set; }
        internal static string FoundG { get; private set; }
        internal static string UpsetJackie { get; private set; }
        private static bool HasJackie { get; set; }
        internal static bool HasSgi { get; private set; }
        internal static bool HasObtainedCashToday { get; set; }
    }
}
