/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/evfredericksen/StardewSpeak
**
*************************************************/

using System;
using System.Collections.Generic;
using StardewValley;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using Microsoft.Xna.Framework;

namespace StardewSpeak
{

    public class EventHandler
    {
        public Dictionary<string, string> MapKeysToButtons;
        public List<string> buttons = new() { "moveUpButton", "moveLeftButton", "moveDownButton", "moveRightButton" };
        private readonly IModHelper modHelper;
        private readonly SpeechEngine speechEngine;
        private dynamic PreviousEvent = null;

        public EventHandler(IModHelper modHelper, SpeechEngine speechEngine) {
            PopulateMapKeysToButtons();
            this.modHelper = modHelper;
            this.speechEngine = speechEngine;
            this.RegisterEvents();
        }
        private void RegisterEvents() 
        {
            modHelper.Events.Input.ButtonPressed += this.OnButtonPressed;
        }

        public void CheckNewInGameEvent() 
        {
            dynamic evt = Game1.CurrentEvent;
            bool bothNull = evt == null && PreviousEvent == null;
            if (bothNull) return;
            if (PreviousEvent == null || evt == null || PreviousEvent.id != evt.id || PreviousEvent.skippable != evt.skippable || PreviousEvent.skipped != evt.skipped)
            {
                dynamic serializedEvent = Serialization.SerializeGameEvent(evt);
                this.speechEngine.SendEvent("GAME_EVENT", serializedEvent);
                PreviousEvent = serializedEvent;
            }
            
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            string key = e.Button.ToString();
            MapKeysToButtons.TryGetValue(key, out string button);
            speechEngine.SendEvent("KEY_PRESSED", new { key, button, isWorldReady = Context.IsWorldReady });
            string pressed = e.Button.ToString();
            if (ModEntry.Config.Debug)
            {
                if (pressed == "R")
                {
                    var menu = Game1.activeClickableMenu;
                    var serializedMenu = Utils.SerializeMenu(Game1.activeClickableMenu);
                    Utils.WriteJson("menu.json", serializedMenu);
                }
                else if (pressed == "L")
                {
                    var player = Game1.player;
                    var location = player.currentLocation;
                    var mouseX = Game1.getMouseX();
                    var mouseY = Game1.getMouseY();
                    var point = Game1.currentCursorTile;
                    var tileX = (int)point.X;
                    var tileY = (int)point.Y;
                    var vec = new Vector2(tileX, tileY);
                    var viewport = Game1.viewport;
                    ModEntry.Log($"Current tiles: x: {tileX}, y: {tileY}", LogLevel.Trace);
                    ModEntry.Log($"Current mouse position: x: {mouseX}, y: {mouseY}", LogLevel.Trace);
                    var isPassable = Pathfinder.Pathfinder.isTileWalkable(location, tileX, tileY);

                    var isOccupied = location.isTileOccupiedIgnoreFloors(vec);
                    var rec = new xTile.Dimensions.Location(tileX, tileY);
                    var t = player.CurrentTool;
                    Utils.WriteJson("debris.json", location.debris.ToList());
                    Utils.WriteJson("objects.json", location.Objects.Values.ToList());
                    Utils.WriteJson("resourceClumps.json", location.resourceClumps.ToList());
                    Utils.WriteJson("currentTool.json", player.CurrentTool);
                    Utils.WriteJson("serializedResourceClumps.json", GameState.ResourceClumps());
                }
            }
        }
        public void PopulateMapKeysToButtons() 
        {
            var map = new Dictionary<string, string>();
            foreach (var buttonName in buttons) 
            {
                AddButtonMap(map, buttonName);
            }
            MapKeysToButtons = map;
        }

        private void AddButtonMap(Dictionary<string, string> map, string buttonName) 
        {
            InputButton[] buttons = Utils.GetPrivateField(Game1.options, buttonName);
            foreach (var btn in buttons) 
            {
                map[btn.ToString()] = buttonName;
            }
        }

    }
}
