/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/rikai/Grandfathers-Heirlooms
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Globalization;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Tools;
using StardewValley.Objects;
using pepoHelper;

namespace GrandfathersHeirlooms
{
    class ModConfig {
        public int triggerDay { get; set; }
        public bool traceLogging { get; set; }
        public string weaponStats { get; set; }
        public bool directToChest { get; set; }

        public ModConfig()
        {
            triggerDay = 2;
            traceLogging = true;
            weaponStats = "3/5/.5/0/5/0/1/-1/-1/0/.20/3";
            directToChest = false;
        }
    }

    /// <summary>Mod entry point</summary>
    public class GrandfathersHeirlooms : Mod, IAssetEditor
    {
        /***** Constants *****/
        const int WEAP_ID = 20;
        // const int HOLDUP_MSG_DLY = 2000;
        private readonly CultureInfo CULTURE = CultureInfo.InvariantCulture;

        /***** Properteze *****/
        private ModConfig Config;
        private SDate triggerDate;

        private string letterGrandpaMessage;
        private string narrationMessage1;
        private string narrationMessage2;
        private string inventoryFullMessage;

        private Farmer farmer;
        private MeleeWeapon weapon;

        /***** Publique Methodes *****/
        public bool CanEdit<T>(IAssetInfo asset)  // implements IAssetEditor.CanEdit<T>
        {
            if (asset == null) return false;
            if (asset.AssetNameEquals("Data/weapons")) return true;
            return false;
        }

        public void Edit<T>(IAssetData asset)  // implements IAssetEditor.Edit<T>
        {
            if (asset == null) return;
            if (!asset.AssetNameEquals("Data/weapons")) return;
            IDictionary<int, string> data = asset.AsDictionary<int, string>().Data;
            string wName = Helper.Translation.Get("weapon.name");
            string wDesc = Helper.Translation.Get("weapon.desc");
            string wStat = Config.weaponStats;
            string wData = $"{wName}/{wDesc}/{wStat}";
            data[WEAP_ID] = wData;
            Log($"weapon {WEAP_ID} set to {wName} - {wDesc} - Stats: {wStat}!", LogLevel.Info);
        }

        public override void Entry(IModHelper helper)
        {
            if (helper == null) throw new ArgumentNullException(nameof(helper));
            Config = helper.ReadConfig<ModConfig>();
            if (!Config.traceLogging)
                Monitor.Log("WARNING: Trace logging disabled via config.json", LogLevel.Warn);
            else
                pepoCommon.monitor = Monitor;

            PrepTrigger();
            PrepTranslations();

            Helper.Events.GameLoop.DayStarted += OnDayStarted;
            Log($"registered for DayStarted event");
        }


        /***** Private Methodes *****/

        private void PrepTrigger()
        {
            int triggerDateDay = Config.triggerDay;
            if (triggerDateDay < 2) triggerDateDay = 2;
            else if (triggerDateDay > 28) triggerDateDay = 28;
            SDate tD = new SDate(triggerDateDay, "spring", 1);
            Log($"triggerDate set to {tD.Day} {tD.Season} {tD.Year}", LogLevel.Info);
            triggerDate = tD;
        }

        private void PrepTranslations()
        {
            var tran = Helper.Translation;

            string tran_get(string item, string desc = null) {
                string s = tran.Get(item);
                string d = desc ?? item;
                Log($"loaded {d} from i18n, {s.Length} chars");
                return s;
            }

            letterGrandpaMessage = tran_get("letter", "Grandpa's Letter");
            narrationMessage1 = tran_get("narration1", "Narration p1");
            narrationMessage2 = tran_get("narration2", "Narration p2");
            inventoryFullMessage = tran_get("full_inventory", "Inventory Full Message");
        }

        private void Log(string message, LogLevel level=LogLevel.Debug)
        {
            if (!Config.traceLogging && level == LogLevel.Trace) return;
            Monitor.Log(message, level);
        }

        private delegate void ExitFunction();

        private void OnDayStarted(object sender, DayStartedEventArgs e) {
            var curDate = SDate.Now();
            if (curDate < triggerDate) {
                Log($"new day {curDate.ToString()}, our day is coming.", LogLevel.Debug);
                return;
            }
            else if (curDate > triggerDate)
            {
                Log($"new day {curDate.ToString()}, our day has passed.", LogLevel.Debug);
                return;
            }
            Log($"new day {curDate.ToString()}, is our day", LogLevel.Debug);

            farmer = Game1.player;
            weapon = new MeleeWeapon(WEAP_ID);

            IClickableMenu narrationDayStart1 =
                new pepoHelper.DialogOnBlack(narrationMessage1) {
                    exitFunction = () => { Game1.globalFadeToBlack(); }
                };
            IClickableMenu narrationDayStart2 =
                new pepoHelper.DialogOnBlack(narrationMessage2);
            IClickableMenu letterGrandpa =
                new LetterViewerMenu(letterGrandpaMessage.Replace("@", farmer.Name)) {
                    exitFunction = spawnChestWeapon
                };
            Log("built the menus", LogLevel.Trace);

            // Make a chain of menus
            pepoHelper.MenuChainer menuChain = new pepoHelper.MenuChainer();
            menuChain.Add(narrationDayStart1, narrationDayStart2, letterGrandpa);
            Log("chained the menus", LogLevel.Trace);
            menuChain.Start(Helper.Events.Display);
            Log("displayed DayStart narration1, continuing doing things in the background");

            // Shift farmer to the left to leave the bed, location dependent on marriage status.
            
            if (farmer.isMarried())
            {
                farmer.moveRelTiles(h: -1, faceDir: 2);
                Log("Farmer is married. Moved farmer out of spouse bed", LogLevel.Trace);
            }
            else
            {
                farmer.moveRelTiles(h: -2, faceDir: 3);
                Log("Farmer is not married. Moved farmer out of single bed.", LogLevel.Trace);
            }
        }

        private void spawnChestWeapon() {
            var cloc = Game1.currentLocation;

            // Drop a chest
            Chest chest = new Chest(true);  // must be "true" or chest won't appear
            Log("created Chest(true)", LogLevel.Trace);
            chest.playerChoiceColor.Set(Microsoft.Xna.Framework.Color.Gold);
            chest.Tint = Microsoft.Xna.Framework.Color.Gold;
            if (farmer.isMarried())
            {
                cloc.setObject(farmer.relTiles(v: 1), chest); 
                Log("Farmer is in love. Chest placed below.", LogLevel.Trace);
            }
            else
            {
                cloc.setObject(farmer.relTiles(h: -1), chest);
                Log("Farmer is alone. Chest placed to the left.", LogLevel.Trace);
            }
            Log("dropped chest in front of farmer", LogLevel.Trace);

            Log($"directToChest == {Config.directToChest.ToString(CULTURE)}", LogLevel.Trace);

            if (Config.directToChest) {
                chest.addItem(weapon);
                Log("inserted weapon into chest", LogLevel.Trace);
            }

            farmer.holdUpItemThenMessage(weapon);
            if(farmer.isInventoryFull()) {
                // TODO: Figure out how to wait out the message above before creating a HUDMessage
                // Game1.addHUDMessage(new HUDMessage(inventoryFullMessage, 2));
                chest.addItem(weapon);
                Log("inserted weapon into chest", LogLevel.Trace);
            }
            else {
                farmer.addItemToInventory(weapon);
                Log("inserted weapon into inventory", LogLevel.Trace);
            }
            return;

            // TODO: Instead of a simple chest, make a chest with interaction
            // TODO: Interaction with chest = Farmer lift weapon above head + chimes

        }
    }
}
