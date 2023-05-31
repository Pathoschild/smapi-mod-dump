/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MaciejMarkuszewski/StardewValleyMod
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Menus;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using StardewValley.Events;
using System.Linq;

namespace MultiplayerTime
{
    public class ModConfig
    {
        public SButton ActivationKey { get; set; } = SButton.F3;
        public bool Active { get; set; } = true;
        public bool UiInfoSuite { get; set; } = false;
        public bool InvisibleUI { get; set; } = false;
    }

    public class PlayerList
    {
        public long PlayerID;
        public int message = -1;

        public PlayerList(long ID)
        {
            PlayerID = ID;
        }
    }

    public interface IGenericModConfigMenuAPI
    {
        void Register(IManifest mod, Action reset, Action save, bool titleScreenOnly = false);
        void AddBoolOption(IManifest mod, Func<bool> getValue, Action<bool> setValue, Func<string> name, Func<string> tooltip = null, string fieldId = null);
        void AddKeybind(IManifest mod, Func<SButton> getValue, Action<SButton> setValue, Func<string> name, Func<string> tooltip = null, string fieldId = null);
    }

    class MultiplayerTimeMod : Mod
    {
        /*********
       ** Public methods
       *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        int timeinterval = -1;
        List<PlayerList> Gracze = new();
        PlayerList Me = new(0);
        Vector2 PasekPosition = new(44, 240);
        Color textColor=Game1.textColor;
        Texture2D Pasek;
        Texture2D PasekWithUIS;
        Texture2D PasekZoom;
        Texture2D Black;
        Texture2D Blue;
        Texture2D Green;
        Texture2D Red;
        private ModConfig Config;
        public override void Entry(IModHelper helper)
        {
            Pasek = helper.ModContent.Load<Texture2D>("assets/Pasek.png");
            PasekWithUIS = helper.ModContent.Load<Texture2D>("assets/PasekWithUIS.png");
            PasekZoom = helper.ModContent.Load<Texture2D>("assets/PasekZoom.png");
            Black = helper.ModContent.Load<Texture2D>("assets/Black.png");
            Blue = helper.ModContent.Load<Texture2D>("assets/Blue.png");
            Green = helper.ModContent.Load<Texture2D>("assets/Green.png");
            Red = helper.ModContent.Load<Texture2D>("assets/Red.png");
            this.Config = (ModConfig)helper.ReadConfig<ModConfig>();
            if (this.Config.Active)
            {
                Helper.Events.GameLoop.UpdateTicked += this.GameEvents_UpdateTick;
                Helper.Events.Display.RenderingHud += this.PreRenderHud;
                Helper.Events.Display.RenderedHud += this.PostRenderHud;
                Helper.Events.Display.Rendered += this.RenderClock;
                Helper.Events.GameLoop.DayStarted += this.Statement;
            }
            Helper.Events.GameLoop.GameLaunched += OnLaunched;
            Helper.Events.Input.ButtonPressed += new EventHandler<ButtonPressedEventArgs>(this.ButtonPressed);
            Helper.Events.Multiplayer.PeerConnected += this.PlayerConnected;
            Helper.Events.Multiplayer.PeerDisconnected += this.PlayerDisconnected;
            Helper.Events.Multiplayer.ModMessageReceived += this.OnModMessageRecieved;
        }

        private void OnLaunched(object sender, GameLaunchedEventArgs e)
        {
            Config = Helper.ReadConfig<ModConfig>(); // This can also be done in your entry method if you want, but the rest needs to come in the GameLaunched event.
            var api = Helper.ModRegistry.GetApi<IGenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");
            api.Register(
                mod: this.ModManifest,
                reset: () => this.Config = new ModConfig(),
                save: () => this.Helper.WriteConfig(Config)
            );

            api.AddKeybind(
                mod: this.ModManifest,
                name: () => "Activation Key",
                tooltip: () => "Clicking this key during game will change state of \"Active\" checkbox below.",
                getValue: () => this.Config.ActivationKey,
                setValue: value => this.Config.ActivationKey = value
            );
            
            api.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Active",
                tooltip: () => "Mod works only if every player have this checked",
                getValue: () => this.Config.Active,
                setValue: value => this.Config.Active = value
            );

            api.AddBoolOption(
                mod: this.ModManifest,
                name: () => "UI Info Suite",
                tooltip: () => "Check this if you use UI Info Suite mod",
                getValue: () => this.Config.UiInfoSuite,
                setValue: value => this.Config.UiInfoSuite = value
            );

            api.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Invisible UI",
                tooltip: () => "UI of this mod will not appear",
                getValue: () => this.Config.InvisibleUI,
                setValue: value => this.Config.InvisibleUI = value
            );
        }

        private void ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (Context.IsWorldReady)
            {
                if (e.Button== this.Config.ActivationKey)
                {
                    if (this.Config.Active)
                    {
                        this.Config.Active = false;
                        Helper.Events.GameLoop.UpdateTicked -= this.GameEvents_UpdateTick;
                        Helper.Events.Display.RenderingHud -= this.PreRenderHud;
                        Helper.Events.Display.RenderedHud -= this.PostRenderHud;
                        Helper.Events.Display.Rendered -= this.RenderClock;
                        this.Helper.WriteConfig(Config);
                        if (Context.IsWorldReady)
                        {
                            Game1.chatBox.addMessage("Multiplayer time Mod turned off", Color.White);
                            this.Helper.Multiplayer.SendMessage(-1, "pause?", modIDs: new[] { this.ModManifest.UniqueID });
                            Me.message = -1;
                        }
                    }
                    else
                    {
                        this.Config.Active = true;
                        Helper.Events.GameLoop.UpdateTicked += this.GameEvents_UpdateTick;
                        Helper.Events.Display.RenderingHud += this.PreRenderHud;
                        Helper.Events.Display.RenderedHud += this.PostRenderHud;
                        Helper.Events.Display.Rendered += this.RenderClock;
                        this.Helper.WriteConfig(Config);
                        if (Context.IsWorldReady)
                        {
                            Game1.chatBox.addMessage("Multiplayer time Mod turned on", Color.White);
                            timeinterval = -1;
                        }
                    }
                }
                if(e.Button == SButton.MouseLeft && this.Config.Active && Context.IsMultiplayer)
                {
                    if (new Rectangle((int)((Game1.dayTimeMoneyBox.position.X + PasekPosition.X + 24) * Game1.options.uiScale), (int) ((Game1.dayTimeMoneyBox.position.Y + PasekPosition.Y + (Game1.options.zoomButtons ? 52 : 24))*Game1.options.uiScale),(int) (108 * Game1.options.uiScale), (int) (24 * Game1.options.uiScale)).Contains(Game1.getMouseX(), Game1.getMouseY()))
                    {
                        foreach (PlayerList gracz in Gracze)
                        {
                            foreach (Farmer farmer in Game1.getOnlineFarmers())
                            {
                                if (gracz.message == -1 && gracz.PlayerID == farmer.UniqueMultiplayerID && Context.IsMainPlayer)
                                {
                                    Game1.chatBox.addMessage(farmer.Name + " does not have Multiplayer Time mod", Color.Red);
                                }
                                else if(gracz.message != 1 && gracz.PlayerID == farmer.UniqueMultiplayerID)
                                {
                                    Game1.chatBox.addMessage("Time doesn't freeze because of " + farmer.Name, Color.White);
                                }
                            }
                        }
                        this.Helper.Input.Suppress(SButton.MouseLeft);
                    }
                }
            }
        }

        private void Statement(object sender, EventArgs e)
        {
            if (this.Config.Active)
            {
                Game1.chatBox.addMessage("Multiplayer time Mod turned on", Color.White);
            }
            else
            {
                Game1.chatBox.addMessage("Multiplayer time Mod turned off", Color.White);
            }
            Gracze.Clear();
            foreach (Farmer farmer in Game1.getOnlineFarmers())
            {
                if (Game1.player.UniqueMultiplayerID == farmer.UniqueMultiplayerID)
                {
                    Me.PlayerID = farmer.UniqueMultiplayerID;
                    Gracze.Add(Me);
                }
                else
                {
                    Gracze.Add(new PlayerList(farmer.UniqueMultiplayerID));
                }
            }
        }

        private void PlayerConnected(object sender, PeerConnectedEventArgs e)
        {
            if (Context.IsMainPlayer)
            {
                if (e.Peer.GetMod("lolmaj.MultiplayerTime") == null)
                {
                    Game1.chatBox.addMessage(Game1.getOnlineFarmers().FirstOrDefault(p => p.UniqueMultiplayerID == e.Peer.PlayerID)?.Name ?? e.Peer.PlayerID.ToString() + " does not have Multiplayer Time mod",Color.Red);
                }
            }
        }

        private void PlayerDisconnected(object sender, PeerDisconnectedEventArgs e)
        {
            for (int i = Gracze.Count-1; i >= 0; i--)
            {
                if (Gracze[i].PlayerID == e.Peer.PlayerID)
                {
                    Gracze.RemoveAt(i);
                }
            }
        }

        public void OnModMessageRecieved(object sender, ModMessageReceivedEventArgs e)
        {
            if(e.FromModID == this.ModManifest.UniqueID && e.Type == "pause?")
            {
                foreach(PlayerList gracz in Gracze)
                {
                    if(gracz.PlayerID == e.FromPlayerID)
                    {
                        gracz.message = e.ReadAs<int>();
                    }
                }
            }
        }

        private void PreRenderHud(object sender, EventArgs e)
        {
            if (!ShouldTimePass())
            {
                Game1.textColor *= 0f;
                Game1.dayTimeMoneyBox.timeShakeTimer = 0;
            }
            if (Context.IsMultiplayer && !Game1.isFestival() && !this.Config.InvisibleUI)
            {
                DrawPasek(Game1.spriteBatch);
            }
        }

        private void PostRenderHud(object sender, EventArgs e)
        {
            if (!ShouldTimePass())
            {
                DrawFade(Game1.spriteBatch);
            }
            Game1.textColor = textColor;
        }

        private void RenderClock(object sender, EventArgs e)
        {
            if (Context.IsWorldReady && !this.Config.InvisibleUI)
            {
                if (!Game1.isFestival() && !(Game1.farmEvent!=null && (Game1.farmEvent is FairyEvent || Game1.farmEvent is WitchEvent || Game1.farmEvent is SoundInTheNightEvent)) && (Game1.eventUp || Game1.currentMinigame != null || Game1.activeClickableMenu is AnimalQueryMenu || Game1.activeClickableMenu is PurchaseAnimalsMenu || Game1.activeClickableMenu is CarpenterMenu || Game1.freezeControls))
                {
                    Game1.textColor *= 0f;
                    Game1.dayTimeMoneyBox.timeShakeTimer = 0;
                    Game1.spriteBatch.End();
                    Game1.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
                    Game1.dayTimeMoneyBox.draw(Game1.spriteBatch);
                    Game1.textColor = textColor;
                    DrawFade(Game1.spriteBatch);
                    if (Context.IsMultiplayer)
                    {
                        DrawPasek(Game1.spriteBatch);
                    }
                }
            }
        }

        private void GameEvents_UpdateTick(object sender, EventArgs e)
        {
            if (Context.IsWorldReady)
            {
                if (Gracze.Count < Game1.getOnlineFarmers().Count)
                {
                    foreach(Farmer gracz in Game1.getOnlineFarmers().Where(x => !Gracze.Any(z => x.UniqueMultiplayerID == z.PlayerID)).ToList())
                    {
                        Gracze.Add(new PlayerList(gracz.UniqueMultiplayerID));
                    }
                }
                if (Me.message != 1 && (!Context.IsPlayerFree || (Game1.currentMinigame != null && Game1.currentMinigame.minigameId() == "PrairieKing") || Game1.player.isEating || Game1.freezeControls || Game1.activeClickableMenu is BobberBar))
                {
                    this.Helper.Multiplayer.SendMessage(1, "pause?", modIDs: new[] { this.ModManifest.UniqueID});
                    Me.message = 1;
                }
                if (Me.message != 0 && Context.IsPlayerFree && !Game1.player.isEating && Game1.currentMinigame==null && !Game1.freezeControls)
                {
                    this.Helper.Multiplayer.SendMessage(0, "pause?", modIDs: new[] { this.ModManifest.UniqueID });
                    Me.message = 0;
                }
                if (Context.IsMainPlayer && !ShouldTimePass())
                {
                    foreach (GameLocation location in Game1.locations)
                    {
                        foreach(Character NPCs in location.characters)
                        {
                            if(NPCs is NPC)
                            {
                                (NPCs as NPC).movementPause = 1;
                            }
                        }
                    }
                    foreach(Farmer gracz in Game1.getOnlineFarmers())
                    {
                        if (gracz.currentLocation.currentEvent != null)
                        {
                            if(gracz.currentLocation is Farm)
                            {
                                foreach (FarmAnimal animal in (gracz.currentLocation as Farm).getAllFarmAnimals())
                                {
                                    animal.pauseTimer = 100;
                                }
                            }
                            if (gracz.currentLocation is AnimalHouse)
                            {
                                foreach (FarmAnimal animal in (gracz.currentLocation as AnimalHouse).animals.Values)
                                {
                                    animal.pauseTimer = 100;
                                }
                            }
                            foreach(Character Monsters in gracz.currentLocation.characters)
                            {
                                if (Monsters !=null && Monsters is NPC && Monsters is Monster)
                                {
                                    if(Monsters is not Bug)
                                    {
                                        (Monsters as Monster).Halt();
                                    }
                                    if(Monsters is Bat || Monsters is Ghost || Monsters is DustSpirit)
                                    {
                                        (Monsters as Monster).xVelocity = 0f;
                                        (Monsters as Monster).yVelocity = 0f;
                                        if(Monsters is DustSpirit)
                                        {
                                            (Monsters as Monster).yJumpVelocity = 0f;
                                        }
                                    }
                                    if(Monsters is GreenSlime || Monsters is SquidKid)
                                    {
                                        (Monsters as Monster).moveTowardPlayer(0);
                                    }
                                    if(Monsters is Fly || Monsters is Serpent)
                                    {
                                        (Monsters as Monster).setInvincibleCountdown(100);
                                        (Monsters as Monster).stopGlowing();
                                    }
                                    (Monsters as Monster).Speed = 0;
                                }
                            }
                        }
                    }
                }
                if(Context.IsMainPlayer && ShouldTimePass())
                {
                    foreach (Farmer gracz in Game1.getOnlineFarmers())
                    {
                        foreach (Character Monsters in gracz.currentLocation.characters)
                        {
                            if (Monsters is NPC && Monsters is Monster && (Monsters as Monster).Speed==0 )
                            {
                                (Monsters as Monster).Speed=Convert.ToInt32(MonsterInfo((Monsters as Monster).Name)[10]);
                                if(Monsters is GreenSlime || Monsters is SquidKid)
                                {
                                    (Monsters as Monster).moveTowardPlayer(Convert.ToInt32(MonsterInfo((Monsters as Monster).Name)[9]));
                                }
                            }
                        }
                    }
                }
                if (Game1.currentLocation != null)
                {
                    for (int k = Game1.currentLocation.TemporarySprites.Count - 1; k >= 0; k--)
                    {
                        if (Game1.currentLocation.TemporarySprites[k].bombRadius > 0 && ShouldTimePass())
                        {
                            Game1.currentLocation.TemporarySprites[k].paused = false;
                        }
                        if (Game1.currentLocation.TemporarySprites[k].bombRadius > 0 && !ShouldTimePass())
                        {
                            Game1.currentLocation.TemporarySprites[k].paused = true;
                        }
                    }
                }
                if(Game1.player.swimming && !Game1.eventUp && !Game1.paused && !ShouldTimePass())
                {
                    Game1.player.swimTimer = 100;
                }
                if (Context.IsMainPlayer)
                {
                    if (ShouldTimePass())
                    {
                        timeinterval = -1;
                        return;
                    }
                    if (timeinterval == -1)
                    {
                        if (Game1.gameTimeInterval > 6800)
                        {
                            timeinterval = 6800;
                        }
                        else
                        {
                            timeinterval = Game1.gameTimeInterval;
                        }
                    }
                    else
                    {
                        Game1.gameTimeInterval = timeinterval;
                    }
                }
                return;
            }
        }

        private static string[] MonsterInfo(string name) {
            return Game1.content.Load<Dictionary<string, string>>("Data\\Monsters")[name].Split(new char[]
            {
                '/'
            });
        }

        private bool ShouldTimePass()
        {
            foreach (PlayerList gracz in Gracze)
            {
                if (gracz.message != 1)
                {
                    return true;
                }
            }
            return false;
        }

        private void DrawFade(SpriteBatch b)
        {
            string text = Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth) + ". " + Game1.dayOfMonth;
            Vector2 dayPosition = new((float)Math.Floor(183.5f - Game1.dialogueFont.MeasureString(text).X / 2), (float)18);
            b.DrawString(Game1.dialogueFont, text, Game1.dayTimeMoneyBox.position + dayPosition, textColor);
            string timeofDay = (Game1.timeOfDay < 1200 || Game1.timeOfDay >= 2400) ? " am" : " pm";
            string zeroPad = (Game1.timeOfDay % 100 == 0) ? "0" : "";
            string hours = (Game1.timeOfDay / 100 % 12 == 0) ? "12" : string.Concat(Game1.timeOfDay / 100 % 12);
            string time = string.Concat(new object[]
            {
                hours,
                ":",
                Game1.timeOfDay % 100,
                zeroPad,
                timeofDay
            });
            Vector2 timePosition = new((float)Math.Floor(183.5 - Game1.dialogueFont.MeasureString(time).X/2), (float)108);
            bool nofade = ShouldTimePass() || Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 2000.0 > 1000.0;
            b.DrawString(Game1.dialogueFont, time, Game1.dayTimeMoneyBox.position + timePosition, (Game1.timeOfDay >= 2400) ? Color.Red : (textColor * (nofade ? 1f : 0.5f)));
        }

        private void DrawPasek(SpriteBatch b)
        {
            int width = (int)(108-(Game1.getOnlineFarmers().Count-1)*4)/ Game1.getOnlineFarmers().Count;
            int i = 0;
            if (this.Config.UiInfoSuite)
            {
                b.Draw(PasekWithUIS, Game1.dayTimeMoneyBox.position + PasekPosition, null, Color.White, 0.0f, Vector2.Zero, 4, SpriteEffects.None, 0.99f);
            }
            b.Draw(Game1.options.zoomButtons ? PasekZoom : Pasek, Game1.dayTimeMoneyBox.position + PasekPosition + new Vector2 (0, this.Config.UiInfoSuite ? 42 : 0), null, Color.White, 0.0f, Vector2.Zero, 4, SpriteEffects.None, 0.99f);
            foreach (PlayerList gracz in Gracze)
            {
                if (gracz.message != 1)
                {
                    b.Draw(Blue, Game1.dayTimeMoneyBox.position + PasekPosition + new Vector2(24,Game1.options.zoomButtons ? 52 : 24) + new Vector2(0, this.Config.UiInfoSuite ? 42 : 0) + new Vector2(i * (width + 4), 0), new Rectangle(0, 0, width, 24), Color.White, 0.0f, Vector2.Zero, 1, SpriteEffects.None, 0.99f);
                    if (Context.IsMainPlayer && gracz.message == -1)
                    {
                        b.Draw(Red, Game1.dayTimeMoneyBox.position + PasekPosition + new Vector2(24, Game1.options.zoomButtons ? 52 : 24) + new Vector2(0, this.Config.UiInfoSuite ? 42 : 0) + new Vector2(i * (width + 4), 0), new Rectangle(0, 0, width, 24), Color.White, 0.0f, Vector2.Zero, 1, SpriteEffects.None, 0.99f);
                    }
                }
                else
                {
                    b.Draw(Green, Game1.dayTimeMoneyBox.position + PasekPosition + new Vector2(24, Game1.options.zoomButtons ? 52 : 24) + new Vector2(0, this.Config.UiInfoSuite ? 42 : 0) + new Vector2(i * (width + 4), 0), new Rectangle(0, 0, width, 24), Color.White, 0.0f, Vector2.Zero, 1, SpriteEffects.None, 0.99f);
                }
                if (i != Game1.getOnlineFarmers().Count - 1)
                {
                    b.Draw(Black, Game1.dayTimeMoneyBox.position + PasekPosition + new Vector2(24, Game1.options.zoomButtons ? 52 : 24) + new Vector2(0, this.Config.UiInfoSuite ? 42 : 0) + new Vector2(i * (width + 4) + width, 0), new Rectangle(i * (width + 4) + width, 0, 4, 24), Color.White, 0.0f, Vector2.Zero, 1, SpriteEffects.None, 0.99f);
                }
                i++;
            }
        }
    }
}
