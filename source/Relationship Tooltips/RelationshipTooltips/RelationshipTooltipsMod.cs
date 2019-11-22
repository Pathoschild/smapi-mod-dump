using RelationshipTooltips.Relationships;
using RelationshipTooltips.UI;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using RelationshipTooltips.API;
using Bookcase.Events;
using Event = StardewValley.Event;

namespace RelationshipTooltips
{
    public class RelationshipTooltipsMod : Mod
    {
        public RelationshipAPI RelationshipAPI { get; private set; }
        internal ModConfig Config { get; private set; }
        /// <summary>
        /// Ordered list of relationships to check against every quater second tick. Higher priorities are first.
        /// </summary>
        public List<IRelationship> HoverRelationships { get; private set; }
        public List<IRelationship> ScreenRelationships { get; private set; }
        public override void Entry(IModHelper helper)
        {
            RelationshipAPI = new RelationshipAPI(Monitor);
            Config = helper.ReadConfig<ModConfig>() ?? new ModConfig();
            displayEnabled = Config.displayTooltipByDefault;
            tooltip = new Tooltip(0, 0, Color.White, anchor: FrameAnchor.BottomLeft);
            HoverRelationships = new List<IRelationship>();
            ScreenRelationships = new List<IRelationship>();
            screenCharacters = new List<Character>();
            screenTooltipCache = new Dictionary<Character, Tooltip>();
            RelationshipAPI.RegisterRelationships += RegisterDefaultRelationships;
            BookcaseEvents.FirstGameTick.Add((e) => InitRelationships(), Priority.Lowest);
            BookcaseEvents.GameQuaterSecondTick.Add(QuaterSecondUpdate);
            InputEvents.ButtonPressed += (obj, e) => { if (e.Button == Config.toggleDisplayKey) { displayEnabled = !displayEnabled; } };
            GraphicsEvents.OnPostRenderEvent += DrawTooltip;
            helper.WriteConfig(Config);
            Monitor.Log("Entry Complete", LogLevel.Trace);
        }

        private void RegisterDefaultRelationships(object sender, EventArgsRegisterRelationships e)
        {
            #region OnHover
            e.RelationshipsOnHover.AddRange(new List<IRelationship>()
            {
                new PlayerRelationship(),
                new EasterEgg(),
                new PetRelationship(),
                new FarmAnimalRelationship(Config),
                new NPCGiftingRelationship(Config, Monitor),
                new NPCRelationship(Config, Monitor),
                new HorseRelationship(),
                new NonFriendNPCRelationship()
            });
            if (Config.displayBirthday)
                e.RelationshipsOnHover.Add(new VillagerBirthdayRelationship(Config));
            #endregion
            #region OnScreen
            e.RelationshipsOnScreen.AddRange(new List<IRelationship>()
            {
                new NameScreenRelationship(Config)
            });
            #endregion
        }

        public override object GetApi()
        {
            return RelationshipAPI;
        }
        /// <summary>
        /// Subscribes the stored Relationships to the relevant events
        /// </summary>
        private void InitRelationships()
        {
            //Fire registration event
            EventArgsRegisterRelationships result = RelationshipAPI.FireRegistrationEvent();
            //copy arrays
            HoverRelationships.AddRange(result.RelationshipsOnHover);
            ScreenRelationships.AddRange(result.RelationshipsOnScreen);
            //Sort by Priority
            HoverRelationships.Sort((x, y) => y.Priority - x.Priority);
            ScreenRelationships.Sort((x, y) => y.Priority - x.Priority);
            //Log
            Monitor.Log($"API found {HoverRelationships.Count()} Hover, and {ScreenRelationships.Count()} Screen registered types.", LogLevel.Info);
            string str = "";
            str += $"{Environment.NewLine}Hover Types ({HoverRelationships.Count()}):";
            str += String.Format("{0}{1,10} :: {2}", Environment.NewLine, "<Priority>", "<Fully Qualified Type>");
            foreach (IRelationship r in HoverRelationships)
            {
                str += String.Format("{0}{1,10} :: {2}", Environment.NewLine, r.Priority, r.GetType().ToString());
            }
            str +=$"{Environment.NewLine}Screen Types ({ScreenRelationships.Count()}):";
            str += String.Format("{0}{1,10} :: {2}", Environment.NewLine, "<Priority>", "<Fully Qualified Type>");
            foreach (IRelationship r in ScreenRelationships)
            {
                str += String.Format("{0}{1,10} :: {2}", Environment.NewLine, r.Priority, r.GetType().ToString());
            }
            Monitor.Log(str);
            //subscribe to events
            foreach (IRelationship r in HoverRelationships.Union(ScreenRelationships))
            {
                if(r is Relationships.IUpdateable)
                {
                    var o = r as Relationships.IUpdateable;
                    if(o.OnTick != null)
                        GameEvents.UpdateTick += (obj, args) => { o.OnTick(selectedCharacter, heldItem); };
                    if (o.OnQuaterSecondTick != null)
                        GameEvents.QuarterSecondTick += (obj, args) => { o.OnQuaterSecondTick(selectedCharacter, heldItem); };
                }
                if(r is IInputListener)
                {
                    var o = r as IInputListener;
                    if(o.ButtonPressed != null)
                        InputEvents.ButtonPressed += (obj, args) => { o.ButtonPressed.Invoke(selectedCharacter, heldItem, args); };
                    if(o.ButtonReleased != null)
                        InputEvents.ButtonReleased += (obj, args) => { o.ButtonReleased(selectedCharacter, heldItem, args); };
                }
                if(r is IPerSaveSerializable)
                {
                    var o = r as IPerSaveSerializable;
                    if(o.SaveData != null)
                        SaveEvents.AfterSave += (obj, args) => { o.SaveData(Helper); };
                    if(o.LoadData != null)
                        SaveEvents.AfterLoad += (obj, args) => { o.LoadData(Helper); };
                }
            }
            Monitor.Log("Relationship Event Subscription Complete.");
        }
        private IEnumerable<Character> GetLocationCharacters(GameLocation location, Event currentEvent = null)
        {

            if (location is AnimalHouse)
            {
                return (location as AnimalHouse).animals.Values
                    .Cast<Character>()
                    .Union(location.getCharacters())
                    .Union(location.farmers.Cast<Character>());
            }
            if (currentEvent != null && Config.displayTooltipDuringEvent)
            {
                return currentEvent.actors
                    .Cast<Character>()
                    .Union(currentEvent.farmerActors.Cast<Character>());
            }
            return location.getCharacters()
                    .Cast<Character>()
                    .Union(location.farmers.Cast<Character>());
        }
        /// <summary>
        /// Attempts to get a Character under the mouse, allows for more specific filtering via specification of T other than Character.
        /// </summary>
        /// <typeparam name="T">A more specific type to filter for, default to Character if you don't want a specific derived Type.</typeparam>
        /// <param name="output">The found Character or null</param>
        /// <returns>If there is a Character found under the mouse.</returns>
        private bool TryGetAtMouse<T>(out T output, IEnumerable<Character> locationCharacters) where T : Character
        {
            output = null;
            if (Game1.currentLocation == null)
                return false;
            foreach (Character c in locationCharacters)
            {
                if (c == null || c == Game1.player)
                    continue;
                if (c.getTileLocation() == Game1.currentCursorTile || c.getTileLocation() - Vector2.UnitY == Game1.currentCursorTile)
                {
                    output = c as T;
                    if (output != null)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private const int CharacterOnScreenEdgeTolerance = Game1.tileSize / 4;
        private bool TryGetOnScreen<T>(out List<T> output, IEnumerable<Character> locationCharacters) where T : Character
        {
            output = new List<T>();
            if (Game1.currentLocation == null)
                return false;
            foreach (Character c in locationCharacters)
            {
                if (c == null || c == Game1.player)
                    continue;
                if(Utility.isOnScreen(c.getTileLocationPoint(), CharacterOnScreenEdgeTolerance, Game1.currentLocation))
                {
                    if(c is T)
                        output.Add(c as T);
                }
            }
            return output.Count > 0;
        }
        #region ModLoop
        /// <summary>
        /// Whether to display the tooltip or not - togglable.
        /// </summary>
        private bool displayEnabled;
        internal Dictionary<Character, Tooltip> screenTooltipCache;
        /// <summary>
        /// All characters on the screen in the last 250ms tick.
        /// </summary>
        internal List<Character> screenCharacters;
        /// <summary>
        /// The character under the mouse at the last 250ms tick.
        /// </summary>
        internal Character selectedCharacter;
        /// <summary>
        /// The item in the player's hands at the last 250ms tick.
        /// </summary>
        internal Item heldItem;
        /// <summary>
        /// The tooltip used for display
        /// </summary>
        internal Tooltip tooltip;
        /// <summary>
        /// Used for logging
        /// </summary>
        private bool isFirstTickForMouseHover;
        /// <summary>
        /// Mod updates every 250ms for performance.
        /// </summary>
        /// <param name="e"></param>
        private void QuaterSecondUpdate(Bookcase.Events.Event e)
        {
            CheckForCharacters();
        }
        private void BuildTooltipText(Tooltip tooltip, Character selectedCharacter, IEnumerable<IRelationship> relationships)
        {
            tooltip.header.text = "";
            tooltip.body.text = "";
            foreach (IRelationship relationship in relationships)
            {
                if (relationship.ConditionsMet(selectedCharacter, heldItem))
                {
                    if (relationship.BreakAfter)
                    {
                        try
                        {
                            string header = relationship.GetHeaderText(tooltip.header.text, selectedCharacter, heldItem);
                            string body = relationship.GetDisplayText(tooltip.body.text, selectedCharacter, heldItem);
                            tooltip.header.text += header;
                            if (tooltip.body.text != "")
                                tooltip.body.text += "\n";
                            tooltip.body.text += body;
                            break;//Finds the FIRST match, ignores later matches -- may want to change this later
                        }
                        catch (ArgumentException e)
                        {
                            Monitor.Log(e.Message, LogLevel.Error);
                        }
                    }
                    else
                    {
                        string header = relationship.GetHeaderText(tooltip.header.text, selectedCharacter, heldItem);
                        string body = relationship.GetDisplayText(tooltip.body.text, selectedCharacter, heldItem);
                        tooltip.header.text = header == "" ? tooltip.header.text : tooltip.header.text + header;
                        if (tooltip.body.text != "")
                            tooltip.body.text = body == "" ? tooltip.body.text : tooltip.body.text + "\n" + body;
                        else
                            tooltip.body.text = body;
                    }
                }
            }
        }
        private void CheckForCharacters()
        {
            if (Game1.gameMode == Game1.playingGameMode && Game1.player != null && Game1.player.currentLocation != null)
            {
                heldItem = Game1.player.CurrentItem;
                IEnumerable<Character> locationCharacters = GetLocationCharacters(Game1.currentLocation, Game1.CurrentEvent);
                if (TryGetOnScreen<Character>(out screenCharacters, locationCharacters))
                {
                    if (TryGetAtMouse<Character>(out selectedCharacter, screenCharacters))
                    {
                        if (isFirstTickForMouseHover)
                        {
                            Monitor.Log($"Character '{selectedCharacter.Name}' under mouse. Type: '{selectedCharacter.GetType()}'");
                            Monitor.Log($"Held item is '{heldItem}'.");
                            isFirstTickForMouseHover = false;
                        }
                        BuildTooltipText(tooltip, selectedCharacter, HoverRelationships);
                    }
                    else
                        isFirstTickForMouseHover = true;
                    //cont here
                    if (screenCharacters.Count == 0)
                    {
                        return;
                    }
                    foreach (Character c in screenCharacters)
                    {
                        if (c == null || c == Game1.player || c == selectedCharacter)
                            continue;
                        if (!screenTooltipCache.ContainsKey(c))
                            screenTooltipCache.Add(c, new Tooltip(0, 0, Color.White, FrameAnchor.BottomMid));
                        BuildTooltipText(screenTooltipCache[c], c, ScreenRelationships);
                    }
                }
                else
                {
                    selectedCharacter = null; 
                    isFirstTickForMouseHover = true;
                }

            }
            else
            {
                heldItem = null;
                selectedCharacter = null;
                isFirstTickForMouseHover = true;
                screenCharacters.Clear();
            }
        }
        private void DrawTooltip(object sender, EventArgs e)
        {
            if (displayEnabled)
            {
                if(screenCharacters.Count > 0 && Game1.activeClickableMenu == null)
                {
                    foreach(Character c in screenCharacters.OrderBy(x=>x.Position.Y))
                    {
                        if (c == null || c == Game1.player || c == selectedCharacter)
                            continue;
                        Tooltip t = screenTooltipCache[c];
                        if (t.header.text == "" && t.body.text == "")
                            continue;
                        const int offset = -64;
                        t.localX = c.GetBoundingBox().Center.X - Game1.viewport.X;
                        t.localY = c.GetBoundingBox().Center.Y - Game1.viewport.Y + offset;
                        t.Draw(Game1.spriteBatch, null);
                    }
                }
                
                if (selectedCharacter != null && (tooltip.header.text != "" || tooltip.body.text != ""))
                {
                    tooltip.localX = Game1.getMouseX();
                    tooltip.localY = Game1.getMouseY();
                    tooltip.Draw(Game1.spriteBatch, null);
                }
            }
        }
        #endregion
    }
}
