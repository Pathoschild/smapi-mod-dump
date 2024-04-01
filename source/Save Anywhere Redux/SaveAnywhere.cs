/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/RealSweetPanda/SaveAnywhereRedux
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using SaveAnywhere.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Locations;
using StardewValley.Monsters;
using StardewValley.Network;

namespace SaveAnywhere
{
    // remove Resharper warning about class SaveAnywhere not being instantiated (it's done via SMAPI)
    // ReSharper disable once ClassNeverInstantiated.Global
    public class SaveAnywhere : Mod
    {
        public static SaveAnywhere Instance;

        private ModConfig _config;
        private Dictionary<GameLocation, List<Monster>> _monsters;
        public SaveManager SaveManager;

        public override void Entry(IModHelper helper)
        {
            _config = helper.ReadConfig<ModConfig>();
            SaveManager = new SaveManager(Helper, null);
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            helper.Events.GameLoop.DayEnding += OnDayEnded;
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            Helper.Events.GameLoop.GameLaunched += BuildConfigMenu;
            Helper.Events.GameLoop.Saved += SaveManager.RunAfterSave;
            
            
            Instance = this;
        }

        private void BuildConfigMenu(object sender, GameLaunchedEventArgs e)
        {
            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null) return;

            // register mod
            configMenu.Register(
                ModManifest,
                () => _config = new ModConfig(),
                () => Helper.WriteConfig(_config)
            );

            configMenu.AddSectionTitle(ModManifest, () => "Key Bindings");
            configMenu.AddKeybind(
                ModManifest,
                name: () => "Save Key",
                getValue: () => _config.SaveKey,
                setValue: value => _config.SaveKey = value
            );
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            // ShouldResetSchedules = true;
            if (SaveManager.saveDataExists())
                SaveManager.LoadData();
        }

        private void OnDayEnded(object sender, DayEndingEventArgs e)
        {
            SaveManager.ClearData();
        }

        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady || !Game1.player.IsMainPlayer)
                return;
            SaveManager.Update();
        }


        public void cleanMonsters()
        {
            _monsters = new Dictionary<GameLocation, List<Monster>>();
            foreach (var location in Game1.locations)
            {
                if (location is Forest)
                {
                    if (!(Game1.year <= 2 || location.getCharacterFromName("TrashBear") == null ||
                         !NetWorldState.checkAnywhereForWorldStateID("trashBearDone")))
                        location.characters.Remove(location.getCharacterFromName("TrashBear"));
                }
                _monsters.Add(location, new List<Monster>());
                foreach (var character in location.characters)
                    if (character is Monster monster)
                    {
                        Monitor.Log(character.Name);
                        _monsters[location].Add(monster);
                    }

                foreach (var monster in _monsters[location])
                    location.characters.Remove(monster);
            }
            
            
        }

        public void RestoreMonsters()
        {
            foreach (var monster1 in _monsters)
            foreach (var monster2 in monster1.Value)
                monster1.Key.addCharacter(monster2);
            var forest = Utility.fuzzyLocationSearch("Forest");
            if (Game1.year <= 2 || Game1.isRaining || Utility.isFestivalDay(Game1.dayOfMonth, Game1.season) || forest.getCharacterFromName("TrashBear") != null || NetWorldState.checkAnywhereForWorldStateID("trashBearDone"))
                return;
            forest.characters.Add( new TrashBear());
            _monsters.Clear();
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsPlayerFree || e.Button != _config.SaveKey || Game1.eventUp || Game1.isFestival())
                return;
            if (Game1.client == null)
            {
                if (Game1.player.currentLocation.characters.Any(x => x is Junimo))
                    Game1.addHUDMessage(new HUDMessage("The spirits don't want you to save here.", 3));
                else
                    SaveManager.BeginSaveData();
            }
            else
            {
                Game1.addHUDMessage(new HUDMessage("Only server hosts can save anywhere.", 3));
            }
        }


        public override object GetApi()
        {
            return new SaveAnywhereApi();
        }
    }
}