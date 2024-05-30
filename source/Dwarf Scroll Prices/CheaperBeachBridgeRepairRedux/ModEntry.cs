/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/noriteway/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.Objects;
using StardewValley.Locations;
using StardewValley.Menus;
using xTile.Dimensions;

namespace BetterBeachBridgeRepair
{
    internal sealed class ModEntry : Mod
    {
        private ModConfig? Config;
        private bool Listening = false;

        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ModConfig>();
            if(Config == null)
            {
                Monitor.Log("Couldn't read config.", LogLevel.Error);
                return;
            }

            if(Config.Disable) return;
            //else Monitor.Log("Better Beach Bridge Repair Enabled.", LogLevel.Debug);

            //helper.Events.Content.AssetRequested += OnAssetRequested;
            //helper.Events.Input.ButtonPressed += OnButtonPressed;
            if(!Config.HideGMCM) helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
        }

        private void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
        {
            if(e.NameWithoutLocale.IsEquivalentTo("Strings/Locations"))
            {
                e.Edit(asset =>
                {
                    var data = asset.AsDictionary<string, string>().Data;

                    data["Beach_FixBridge_Question"] = $"Use {Config.Price} pieces of wood to fix the bridge?";
                    data["Beach_FixBridge_Hint"] = $"Hmmm... With {Config.Price} pieces of wood this could be fixed.";
                });
            }
        }

        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            //stop checking for things if the beach bridge is already repaired
            /*if(Context.IsWorldReady)
            {
                Beach? beach = Game1.getLocationFromName("Beach") as Beach;
                if(beach.bridgeFixed.Value)
                {
                    Monitor.Log("Beach bridge is already repaired, stopping all processes.", LogLevel.Debug);
                    Helper.Events.Content.AssetRequested -= OnAssetRequested;
                    Helper.Events.Input.ButtonPressed -= OnButtonPressed;
                    return;
                }
            }*/

            if(!e.Button.IsActionButton()) return;
            //else Monitor.Log("1. Was action button", LogLevel.Debug);

            if(!Context.IsWorldReady) return;
            //else Monitor.Log("2. World was ready", LogLevel.Debug);

            Beach? beach = Game1.currentLocation as Beach;
            if(beach == null) return;
            if(beach.bridgeFixed)
            {
                StopListening();
                return;
            }
            //if(Game1.currentLocation.Name != "Beach") return;
            //else Monitor.Log("3. At the beach", LogLevel.Debug);

            //else Monitor.Log("4. Bridge not already fixed", LogLevel.Debug);

            //swallow the input if already in the qustion dialogue box
            if(Game1.activeClickableMenu is DialogueBox) return;

            //if(!Game1.currentLocation.Name.Contains("Beach")) return;
            Monitor.Log($"Cursor at {e.Cursor.GrabTile.X},{e.Cursor.GrabTile.Y}", LogLevel.Debug);
            //Vector2 vector = Utility.clampToTile(Game1.player.GetToolLocation(e.Cursor.GrabTile, false)) / 64;
            //Vector2 target_position = Utility.PointToVector2(Game1.getMousePosition()) + new Vector2(Game1.viewport.X, Game1.viewport.Y);
            //Vector2 vector = Game1.GlobalToLocal(Game1.viewport, Utility.clampToTile(Game1.player.GetToolLocation(target_position))) / 64;
            //Vector2 vector = Utility.clampToTile(Game1.player.GetToolLocation(target_position)) / 64;
            //Vector2 vector = Utility.clampToTile(new Vector2(Game1.getOldMouseX() + Game1.viewport.X, Game1.getOldMouseY() + Game1.viewport.Y)) / 64f;
            //Monitor.Log($"Interacting at {vector.X},{vector.Y}", LogLevel.Debug);

            //if(e.Cursor.GrabTile == new Vector2(Config.BridgeTileX, Config.BridgeTileY) || e.Cursor.GrabTile == new Vector2(Config.BridgeTileX, Config.BridgeTileY - 1) || e.Cursor.GrabTile == new Vector2(Config.BridgeTileX, Config.BridgeTileY + 1))
            //if(IsOnBridge(e.Cursor.GrabTile))
            //if(IsOnBridge(Game1.player.GetGrabTile()))
            //if(IsOnBridge(ToolLocTile))
            //if(vector == new Vector2(Config.BridgeTileX, Config.BridgeTileY))
            //if(IsOnBridge(vector) && Utility.tileWithinRadiusOfPlayer((int)vector.X, (int)vector.Y, 1, Game1.player))
            if(IsActionable(e.Cursor.GrabTile, new Vector2(Config.BridgeTileX, Config.BridgeTileY)))
            {
                // this doesn't accomplish anything because the menu is waiting for click to call answerDialogue(also crashes?)
                //Game1.currentLocation.answerDialogueAction(null, null);
                //Game1.currentLocation.answerDialogue(null);

                /*
                 * because this creates a new question dialogue, overwriting the old one with the default
                 * requirement, it calls ReduceId, which takes up to the default amount of items and
                 * removes the stack if count >= 0, hence why it will 'accept' 50, becuase the check is 
                 * only before the initial question is asked
                */
                //CheckAction();
                //Helper.Events.GameLoop.UpdateTicked += AwaitAnswer;

                Monitor.Log("Trying to repair bridge.", LogLevel.Debug);
                if(Game1.player.Items.ContainsId("(O)388", Config.Price))
                {
                    Game1.currentLocation.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:Beach_FixBridge_Question"), Game1.currentLocation.createYesNoResponses(), "BetterBeachBridge");
                    Helper.Events.Input.ButtonPressed += AwaitAnswer;
                }
                else Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Beach_FixBridge_Hint"));
            }
        }

        public bool IsOnBridge(Vector2 tile)
        {
            //see Game.pressActionButton for where is calls tryToCheckAt with -1 and +1 Y from selected tile
            return tile == new Vector2(Config.BridgeTileX, Config.BridgeTileY) || tile == new Vector2(Config.BridgeTileX, Config.BridgeTileY - 1) || tile == new Vector2(Config.BridgeTileX, Config.BridgeTileY + 1);
        }

        /// <summary>
        /// Meant to mimic the vanilla checks for whether the cursor is targeting an actionable tile.
        /// </summary>
        /// <param name="cursor">The tile position of the cursor.</param>
        /// <param name="target">The tile position of the actionable tile.</param>
        /// <returns></returns>
        public bool IsActionable(Vector2 cursor, Vector2 target)
        {
            if(!Utility.tileWithinRadiusOfPlayer((int)cursor.X, (int)cursor.Y, 1, Game1.player)) cursor = Game1.player.GetGrabTile();
            if(cursor == target) return true;

            cursor.Y += 1f;
            if(cursor == target) return true;

            cursor.Y -= 2f;
            if(cursor == target) return true;

            cursor = Game1.player.Tile;
            if(cursor == target) return true;

            return false;
        }

        /*private void CheckAction(Farmer who)
        {
            if(who.Items.ContainsId("(O)388", Config.Price))
            {
                Game1.currentLocation.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:Beach_FixBridge_Question"), Game1.currentLocation.createYesNoResponses(), "BetterBeachBridge");
            }
            else
            {
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Beach_FixBridge_Hint"));
            }

            //easy way: reimburse player?
            //correct way: change active menu or intercept response to only take required amount
        }*/
        private void CheckResponse(string questionAndAnswer)
        {
            Beach? beach = Game1.currentLocation as Beach;
            /*if(beach is null)
            {
                Monitor.Log("Couldn't get beach from current location.", LogLevel.Error);
                return false;
            }*/

            //Why does this make it take anywhere from 50 to 300 wood, rather than one or the other?
            //do I need to somehow prevent the default interaction from going through?

            if(questionAndAnswer == "BetterBeachBridge_Yes")
            {
                Game1.globalFadeToBlack(beach.fadedForBridgeFix);
                Game1.player.Items.ReduceId("(O)388", Config.Price);
                //return true;
            }

            //return Game1.currentLocation.answerDialogueAction(questionAndAnswer, questionParams);
            //return false;
        }

        //private int count = 5;

        /*private void AwaitAnswer(object? sender, UpdateTickedEventArgs e)
        {
            //count--;

            //if(count <= 0)
            DialogueBox? question = Game1.activeClickableMenu as DialogueBox;
            if(question != null) Monitor.Log(question.selectedResponse.ToString(), LogLevel.Debug);
            if(question != null && question.selectedResponse != -1)
            {
                Monitor.Log(question.selectedResponse.ToString(), LogLevel.Debug);
                string qna = Game1.currentLocation.lastQuestionKey + 
                //AnswerDialogueAction()
                Helper.Events.GameLoop.UpdateTicked -= AwaitAnswer;
                //count = 5;
            }
        }*/

        private void AwaitAnswer(object? sender, ButtonPressedEventArgs e)
        {
            string qna = "";

            if(e.Button.IsUseToolButton() || e.Button.IsActionButton())
            {
                DialogueBox? question = Game1.activeClickableMenu as DialogueBox;
                //selectedResponse is -1 whenever not on one of the options
                if(question != null && question.selectedResponse != -1)
                {
                    qna = Game1.currentLocation.lastQuestionKey + "_" + question.responses[question.selectedResponse].responseKey;
                }
                else 
                {
                    Monitor.Log("Not on question OR not selecting yes or no", LogLevel.Warn);
                    return;
                }
            }
            else if(e.Button == SButton.Y)
            {
                DialogueBox? question = Game1.activeClickableMenu as DialogueBox;
                if(question != null)
                {
                    qna = Game1.currentLocation.lastQuestionKey + "_" + question.responses[0].responseKey;
                }
                else
                {
                    Monitor.Log("Not on question", LogLevel.Warn);
                    return;
                }
            }
            else if(e.Button == SButton.N)
            {
                DialogueBox? question = Game1.activeClickableMenu as DialogueBox;
                if(question != null)
                {
                    qna = Game1.currentLocation.lastQuestionKey + "_" + question.responses[1].responseKey;
                }
                else
                {
                    Monitor.Log("Not on question", LogLevel.Warn);
                    return;
                }
            }
            else if(e.Button.ToString() == Game1.options.menuButton.ToString())
            {
                Helper.Events.Input.ButtonPressed -= AwaitAnswer;
            }
            else return;

            Monitor.Log(qna, LogLevel.Debug);
            CheckResponse(qna);
            Helper.Events.Input.ButtonPressed -= AwaitAnswer;
        }

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            
        }

        private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            Beach? beach = Game1.getLocationFromName("Beach") as Beach;
            //if(beach.bridgeFixed.Value && Listening)
            if(Listening) StopListening();
            //else if(!Listening)
            if(!beach.bridgeFixed.Value) StartListening();
        }

        public void StartListening()
        {
            Monitor.Log("Starting beach bridge repair processes.", LogLevel.Debug);
            Helper.Events.Content.AssetRequested += OnAssetRequested;
            Helper.Events.Input.ButtonPressed += OnButtonPressed;
            Listening = true;
        }

        public void StopListening()
        {
            Monitor.Log("Stopping current processes.", LogLevel.Debug);
            Helper.Events.Content.AssetRequested -= OnAssetRequested;
            Helper.Events.Input.ButtonPressed -= OnButtonPressed;
            Listening = false;
        }
    }
}
