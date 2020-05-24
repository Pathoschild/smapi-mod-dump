using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NpcAdventure.AI;
using NpcAdventure.Loader;
using NpcAdventure.Model;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NpcAdventure.HUD
{
    class CompanionDisplay : Internal.IDrawable, Internal.IUpdateable
    {
        public Dictionary<string, CompanionSkill> Skills { get; }
        public Dictionary<SButton, ClickableTextureComponent> Keys { get; }
        public Config Config { get; }

        private NPC companion;
        private ClickableTextureComponent avatar;
        private string hoverText;
        private AI_StateMachine.State state;
        private readonly IContentLoader contentLoader;
        float skillSize;

        public CompanionDisplay(Config config, IContentLoader contentLoader)
        {
            this.Skills = new Dictionary<string, CompanionSkill>();
            this.Keys = new Dictionary<SButton, ClickableTextureComponent>();
            this.Config = config;
            this.contentLoader = contentLoader;
        }

        public void AddSkill(string type, string description)
        {
            this.Skills.Add(type, new CompanionSkill(type, description));
        }

        public void SetCompanionState(AI.AI_StateMachine.State state)
        {
            if (this.avatar != null && this.companion != null)
            {
                string whoText = this.contentLoader.LoadString("Strings/Strings:recruitedCompanionHint", this.companion.displayName);
                string stateText = this.contentLoader.LoadString($"Strings/Strings:companionState_{state.ToString().ToLower()}");

                this.avatar.hoverText = whoText + Environment.NewLine + stateText;
            }

            this.state = state;
        }

        public void AssignCompanion(NPC companion)
        {
            string hoverText = this.contentLoader.LoadString("Strings/Strings:recruitedCompanionHint", companion.displayName);
            this.companion = companion;
            this.avatar = new ClickableTextureComponent("", Rectangle.Empty, null, hoverText, companion.Sprite.Texture, companion.getMugShotSourceRect(), 4f, false);
        }

        public void AddKey(SButton key, string description)
        {
            var component = new ClickableTextureComponent("", Rectangle.Empty, null, description, Game1.mouseCursors, new Rectangle(473, 36, 24, 24), 2.5f, false);
            this.Keys.Add(key, component);
        }

        public void Reset()
        {
            this.Skills.Clear();
            this.Keys.Clear();
            this.avatar = null;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!this.Config.ShowHUD || Game1.eventUp)
                return;
            if (Constants.TargetPlatform == GamePlatform.Android)
            {
                if (Game1.activeClickableMenu is GameMenu || Game1.activeClickableMenu is ShopMenu || Game1.activeClickableMenu is QuestLog)
                    return;
            }
            if (this.Skills.Count > 0)
            {
                this.DrawSkills(spriteBatch);
            }

            if (this.avatar != null)
            {
                this.DrawAvatar(spriteBatch);
            }

            if (this.Keys.Count > 0)
            {
                this.DrawKeysHelp(spriteBatch);
            }

            if (!string.IsNullOrEmpty(this.hoverText))
            {
                IClickableMenu.drawHoverText(spriteBatch, this.hoverText, Game1.smallFont);
            }
        }

        public void DrawSkills(SpriteBatch spriteBatch)
        {
            float vX = 630;
            float vY = 55;
            

            if (Constants.TargetPlatform != GamePlatform.Android)
            {             
                vX = Game1.viewport.Width - 80 - IClickableMenu.borderWidth;
                vY = 390;
            }

            Vector2 position = new Vector2(vX, vY);
            var skills = this.Skills.Values.ToList();

            for (int i = 0; i < skills.Count; i++)
                {
                    var skill = skills[i];
                    float xOffset = 50;
                    float iconOffset = 16;
                    float iconGrid = 68;
                    float xIP = position.X - xOffset + iconOffset + (10 - skill.Rectangle.Width) + (i * iconGrid);
                    float xFP = position.X - xOffset + (10 - skill.Rectangle.Height) + (i * iconGrid);

                    if (Constants.TargetPlatform != GamePlatform.Android)
                    {
                        xIP = position.X - xOffset + iconOffset + (10 - skill.Rectangle.Width) - (i * iconGrid);
                        xFP = position.X - xOffset + (10 - skill.Rectangle.Height) - (i * iconGrid);
                    }

                    Vector2 iconPosition = new Vector2(xIP, position.Y);
                    Vector2 framePosition = new Vector2(xFP, position.Y - iconOffset - 3);

                    if (Game1.isOutdoorMapSmallerThanViewport())
                     {
                        iconPosition.X = Math.Min(position.X, -Game1.viewport.X + Game1.currentLocation.map.Layers[0].LayerWidth * 64 - 70 - IClickableMenu.borderWidth) - xOffset + iconOffset - (i * iconGrid);
                        framePosition.X = Math.Min(position.X, -Game1.viewport.X + Game1.currentLocation.map.Layers[0].LayerWidth * 64 - 70 - IClickableMenu.borderWidth) - xOffset - (i * iconGrid);
                    }

                    skill.UpdatePosition(framePosition, iconPosition);
                    skill.Draw(spriteBatch);
                    this.skillSize = position.X + (i * iconGrid) + 5;                   
                }       
            
        }

        public void DrawAvatar(SpriteBatch spriteBatch)
        {
            Rectangle icon;
            float vX = 490;
            float vY = 0;

            if (Constants.TargetPlatform != GamePlatform.Android)
            {               
                vX = Game1.viewport.Width - 70 - IClickableMenu.borderWidth;
                vY = 334;
            }

            Vector2 position = new Vector2(vX, vY);
            if (Game1.isOutdoorMapSmallerThanViewport())
                position.X = Math.Min(position.X, -Game1.viewport.X + Game1.currentLocation.map.Layers[0].LayerWidth * 64 - 70 - IClickableMenu.borderWidth);
            Utility.makeSafe(ref position, 64, 64);
            this.avatar.bounds = new Rectangle((int)position.X + 16, (int)position.Y, 64, 96);
            this.avatar.draw(spriteBatch, Color.White, 1);

            switch (this.state)
            {
                case AI_StateMachine.State.IDLE:
                    icon = new Rectangle(434, 475, 9, 9);
                    break;
                case AI_StateMachine.State.FIGHT:
                    icon = new Rectangle(120, 428, 9, 9);
                    break;
                case AI_StateMachine.State.FORAGE:
                    icon = new Rectangle(60, 428, 10, 10);
                    break;
                default:
                    return;
            }

            spriteBatch.Draw(Game1.mouseCursors, new Vector2(position.X + 54, position.Y + 78), icon, Color.White * 1f, 0f, Vector2.Zero, 3f, SpriteEffects.None, 1f);
        }

        public void DrawKeysHelp(SpriteBatch spriteBatch)
        {
            float vX = this.skillSize;
            float vY = 37;

            if (Constants.TargetPlatform != GamePlatform.Android)
            {                
                vX = 0;
                vY = Game1.viewport.Height * 0.333f - (this.Keys.Count * 34) / 2;
            }

            Vector2 position = new Vector2(vX, vY);
            if (Game1.isOutdoorMapSmallerThanViewport())
                position.X = Math.Max(position.X, -Game1.viewport.X);
            Utility.makeSafe(ref position, 64, 64);
            
            for(int i = 0; i < this.Keys.Count; i++)
            {
                var keyIconPair = this.Keys.ElementAt(i);
                keyIconPair.Value.bounds = new Rectangle((int)position.X + 16, (int)position.Y + (i * 70), 48, 48);
                keyIconPair.Value.draw(spriteBatch, Color.White, 1);
                spriteBatch.DrawString(Game1.smallFont, keyIconPair.Key.ToString(), new Vector2(keyIconPair.Value.bounds.X + keyIconPair.Value.bounds.Width / 2 - 4, (float)keyIconPair.Value.bounds.Y + keyIconPair.Value.bounds.Height / 2 - 8), Color.White);
            }
        }

        public void PerformHoverAction(int x, int y)
        {
            this.hoverText = "";

            if (this.avatar != null)
            {
                this.avatar.tryHover(x, y, .15f);
                if (this.avatar.containsPoint(x, y))
                    this.hoverText = this.avatar.hoverText;
            }

            foreach (var skill in this.Skills.Values)
            {
                skill.PerformHoverAction(x, y);
                if (skill.ShowTooltip)
                {
                    this.hoverText = skill.HoverText + (skill.Glowing ? (Environment.NewLine + this.contentLoader.LoadString("Strings/Strings:hudSkillUsed")) : "");
                }

            }

            foreach (var key in this.Keys.Values)
            {
                if (key.containsPoint(x, y))
                {
                    this.hoverText = key.hoverText;
                    break;
                }
            }
        }

        public void Update(UpdateTickedEventArgs e)
        {
            foreach (var skill in this.Skills.Values)
                skill.Update(e);

            this.PerformHoverAction(Game1.getMouseX(), Game1.getMouseY());
        }

        internal void GlowSkill(string type, Color color, int duration)
        {
            if (this.Skills.TryGetValue(type, out CompanionSkill skill))
            {
                skill.Glow(color, duration);
            }
        }
    }
}
