using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;

namespace Omegasis.SaveAnywhere.Framework
{
    /// <summary>A marker subclass to detect when a custom save is in progress.</summary>
    internal class NewSaveGameMenu : SaveGameMenu
    {
        public event EventHandler SaveComplete;

        private int completePause = -1;
        private int margin = 500;
        private readonly StringBuilder _stringBuilder = new StringBuilder();
        private float _ellipsisDelay = 0.5f;
        private IEnumerator<int> loader;
        public bool quit;
        public bool hasDrawn;
        private readonly SparklingText saveText;
        private int _ellipsisCount;

        public NewSaveGameMenu()
        {
            this.saveText = new SparklingText(Game1.dialogueFont, Game1.content.LoadString("Strings\\StringsFromCSFiles:SaveGameMenu.cs.11378"), Color.LimeGreen, new Color((int)(Color.Black.R * (1.0 / 1000.0)), (int)(Color.Black.G * (1.0 / 1000.0)), (int)(Color.Black.B * (1.0 / 1000.0)), 255), false, 0.1, 1500, 32, 500);
        }

        public new void complete()
        {
            Game1.playSound("money");
            this.completePause = 1500;
            this.loader = (IEnumerator<int>)null;
            Game1.game1.IsSaving = false;

            SaveComplete.Invoke(this, EventArgs.Empty);
        }

        public override void update(GameTime time)
        {
            if (this.quit)
                return;
            //base.update(time);
            if (Game1.client != null && Game1.client.timedOut)
            {
                this.quit = true;
                if (!Game1.activeClickableMenu.Equals((object)this))
                    return;
                Game1.player.checkForLevelTenStatus();
                Game1.exitActiveMenu();
            }
            else
            {
                TimeSpan elapsedGameTime;
                if (this.loader != null)
                {
                    this.loader.MoveNext();
                    if (this.loader.Current >= 100)
                    {
                        int num = this.margin;
                        elapsedGameTime = time.ElapsedGameTime;
                        int milliseconds = elapsedGameTime.Milliseconds;
                        this.margin = num - milliseconds;
                        if (this.margin <= 0)
                            this.complete();
                    }
                    double num1 = (double)this._ellipsisDelay;
                    elapsedGameTime = time.ElapsedGameTime;
                    double totalSeconds = elapsedGameTime.TotalSeconds;
                    this._ellipsisDelay = (float)(num1 - totalSeconds);
                    if ((double)this._ellipsisDelay <= 0.0)
                    {
                        this._ellipsisDelay = this._ellipsisDelay + 0.75f;
                        this._ellipsisCount = this._ellipsisCount + 1;
                        if (this._ellipsisCount > 3)
                            this._ellipsisCount = 1;
                    }
                }
                else if (this.hasDrawn && this.completePause == -1)
                {
                    Game1.game1.IsSaving = true;
                    if (Game1.IsMasterGame)
                    {
                        if (Game1.saveOnNewDay)
                            this.loader = SaveGame.Save();
                        else
                        {
                            this.margin = -1;
                            this.complete();
                        }
                    }
                    else
                    {
                        NewSaveGameMenu.saveClientOptions();
                        this.complete();
                    }
                }
                if (this.completePause < 0)
                    return;
                int num2 = this.completePause;
                elapsedGameTime = time.ElapsedGameTime;
                int milliseconds1 = elapsedGameTime.Milliseconds;
                this.completePause = num2 - milliseconds1;
                this.saveText.update(time);
                if (this.completePause >= 0)
                    return;
                this.quit = true;
                this.completePause = -9999;
                if (Game1.activeClickableMenu.Equals((object)this))
                {
                    Game1.player.checkForLevelTenStatus();
                    Game1.exitActiveMenu();
                }
                Game1.currentLocation.resetForPlayerEntry();
            }
        }

        private static void saveClientOptions()
        {
            StartupPreferences startupPreferences = new StartupPreferences();
            int num1 = 0;
            int num2 = 1;
            startupPreferences.loadPreferences(num1 != 0, num2 != 0);
            Options options = Game1.options;
            startupPreferences.clientOptions = options;
            int num3 = 0;
            startupPreferences.savePreferences(num3 != 0);
        }

        public override void draw(SpriteBatch b)
        {
            base.draw(b);
            Vector2 vector2 = Utility.makeSafe(new Vector2(64f, (float)(Game1.viewport.Height - 64)), new Vector2(64f, 64f));
            if (this.completePause >= 0)
            {
                if (Game1.saveOnNewDay)
                    this.saveText.draw(b, vector2);
            }
            else if (this.margin < 0 || Game1.IsClient)
            {
                this._stringBuilder.Clear();
                for (int index = 0; index < this._ellipsisCount; ++index)
                    this._stringBuilder.Append(".");
                b.DrawString(Game1.dialogueFont, this._stringBuilder, vector2, Color.White);
            }
            else
            {
                this._stringBuilder.Clear();
                this._stringBuilder.Append(Game1.content.LoadString("Strings\\StringsFromCSFiles:SaveGameMenu.cs.11381"));
                for (int index = 0; index < this._ellipsisCount; ++index)
                    this._stringBuilder.Append(".");
                b.DrawString(Game1.dialogueFont, this._stringBuilder, vector2, Color.White);
            }
            this.hasDrawn = true;
        }

        public new void Dispose()
        {
            Game1.game1.IsSaving = false;
        }
    }
}
