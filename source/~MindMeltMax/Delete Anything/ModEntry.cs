/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Delete_Anything
{
    class ModEntry : Mod
    {
        public static DAConfig Config;
        public static bool isDeleteKeyPressed;
        public static List<Response> responses;

        public StardewValley.Object objectAtTile;
        public Vector2 vec;

        public ITranslationHelper i18n => Helper.Translation;

        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonPressed += Input_ButtonPressed;
            helper.Events.Input.ButtonReleased += Input_ButtonReleased;

            helper.Events.GameLoop.SaveLoaded += GameLoop_SaveLoaded;

            Config = helper.ReadConfig<DAConfig>();
            helper.WriteConfig<DAConfig>(Config);
        }

        private void GameLoop_SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            responses = new List<Response>();
            responses.Add(new Response("True", i18n.Get("Confirm")));
            responses.Add(new Response("False", i18n.Get("Cancel")));
        }

        private void Input_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!this.IsDeleteKey(e.Button))
                return;
            isDeleteKeyPressed = true;
            Monitor.Log($"{isDeleteKeyPressed}", LogLevel.Debug);

            vec = e.Cursor.Tile;
            GameLocation location = Game1.currentLocation;

            if (isDeleteKeyPressed)
            {
                objectAtTile = location.getObjectAtTile((int)vec.X, (int)vec.Y);
                if (objectAtTile != null)
                    removeConfirmation();
                else
                    Monitor.Log("No object at tile", LogLevel.Debug);
            }
        }

        private void Input_ButtonReleased(object sender, ButtonReleasedEventArgs e)
        {
            if (!this.IsDeleteKey(e.Button))
                return;
            isDeleteKeyPressed = false;
            Monitor.Log($"{isDeleteKeyPressed}", LogLevel.Debug);
        }

        private bool IsDeleteKey(SButton button)
        {
            string buttonAsString = button.ToString().ToLower();
            return ((IEnumerable<string>)Config.DeleteKey.ToLower().Split(new char[1]
            {
                ','
            }, StringSplitOptions.RemoveEmptyEntries)).Any<string>((Func<string, bool>)(Item => buttonAsString.Equals(Item.Trim())));
        }

        private bool removeConfirmation()
        {
            string text = i18n.Get("ConfirmText");
            Game1.currentLocation.createQuestionDialogue(text, responses.ToArray(), removeObject);
            return true;
        }

        private void removeObject(Farmer who, string key)
        {
            if (key == "Cancel")
                return;

            string txt = responses.Find(k => k.responseKey == key).responseText;
            GameLocation location = Game1.currentLocation;
            string property = location.doesTileHaveProperty((int)objectAtTile.tileLocation.X, (int)objectAtTile.tileLocation.Y, "Action", "Buildings");
            if (property != null)
            {
                Monitor.Log($"Key : {key}, Tile : {vec.X}-{vec.Y}, Object : {objectAtTile.tileLocation}, Property : {property}", LogLevel.Debug);
                location.removeObject(objectAtTile.tileLocation, false);
                location.removeTileProperty((int)objectAtTile.tileLocation.X, (int)objectAtTile.tileLocation.Y, "Buildings", "Action");
                return;
            }
            else
            {
                Monitor.Log($"Key : {key}, Tile : {vec.X}-{vec.Y}, Object : {objectAtTile.tileLocation}", LogLevel.Debug);
                location.removeObject(objectAtTile.tileLocation, false);
                return;
            }
        }
        //comment3
    }
}
