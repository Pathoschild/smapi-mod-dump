/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Alphablackwolf/SkillPrestige
**
*************************************************/

using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SkillPrestige.Framework.InputHandling;
using SkillPrestige.Logging;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace SkillPrestige.Framework.Menus.Elements;

 public class PrestigableSkillButtonSet
        {
            public Skill Skill;
            private int PrestigePoints;
            public int Position;
            private int IconWidth;
            private int IconHeight;
            private int TotalWidth;
            private bool CanPrestige;
            public bool IsHovered => this.Hitbox.Contains(Game1.getMousePosition(true));

            public ClickableComponent ClickableComponent => new (this.Hitbox, $"Open {this.Skill.Type.Name} Prestige Menu");
            private Point ExperienceBarSize;
            private Vector2 PrestigePointDisplayLocation;
            private Vector2 _drawLocation;
            public Vector2 DrawLocation
            {
                get => this._drawLocation;
                set
                {
                    this._drawLocation = value;
                    this.UpdateDisplayLocations();
                }
            }


            private Rectangle Hitbox => new (this.DrawLocation.ToPoint(), new Point(this.TotalWidth, this.IconHeight));


            public bool ShouldBeDrawn;
            private bool Done;
            private Vector2 SkillNameDrawLocation;
            private ExperienceBar ExperienceBar;
            private int PaddingAfterIcon = 8;
            private int ExperienceBarLength = 400;
            private int PaddingBetweenSkillNameAndExperienceBar = 2;

            private IClickableMenu ParentMenu;
            public PrestigableSkillButtonSet(Skill skill, int position, int width, IClickableMenu parentMenu)
            {
                this.Skill = skill;
                this.Position = position;
                this.TotalWidth = width;
                this.ParentMenu = parentMenu;

                var prestige = PrestigeSet.Instance.Prestiges
                    .Single(x => x.SkillType == skill.Type);
                this.PrestigePoints = prestige.PrestigePoints;
                this.Done = prestige.PrestigeProfessionsSelected.Count >= 6;
                this.LoadElements();
            }

            private void LoadElements()
            {
                this.IconWidth = this.Skill.SourceRectangleForSkillIcon.Width * Game1.pixelZoom;
                this.IconHeight = this.Skill.SourceRectangleForSkillIcon.Height * Game1.pixelZoom;
                var skillNameSize = Game1.smallFont.MeasureString(this.Skill.Type.Name);
                this.ExperienceBarSize = new Point(this.ExperienceBarLength, this.IconHeight - (int)skillNameSize.Y - this.PaddingBetweenSkillNameAndExperienceBar);
                int expNeeded = PerSaveOptions.Instance.PainlessPrestigeMode
                    ? PerSaveOptions.Instance.ExperienceNeededPerPainlessPrestige + 15000
                    : 15000;
                this.ExperienceBar = new ExperienceBar(this.ExperienceBarSize, Color.LightGreen)
                {
                    Progress = ((float)this.Skill.GetSkillExperience() / expNeeded).Clamp(0f, 1f),
                    HoverText = $"Experience: {this.Skill.GetSkillExperience()} / {expNeeded}"
                };
                this.CanPrestige = (float)this.Skill.GetSkillExperience() / expNeeded > 1;
                this.UpdateDisplayLocations();
            }

            private void UpdateDisplayLocations()
            {
                this.SkillNameDrawLocation = new Vector2(this.DrawLocation.X + this.IconWidth + this.PaddingAfterIcon, this.DrawLocation.Y);
                var skillNameSize = Game1.smallFont.MeasureString(this.Skill.Type.Name);
                this.ExperienceBar.Location = new Point((int)this.SkillNameDrawLocation.X, (int)this.SkillNameDrawLocation.Y + (int)skillNameSize.Y + this.PaddingBetweenSkillNameAndExperienceBar);
                this.PrestigePointDisplayLocation =
                    new Vector2(this.ExperienceBar.Location.X + this.ExperienceBarLength + this.PaddingAfterIcon * 2,
                        this.DrawLocation.Y + skillNameSize.Y / 2);
            }

            public void OnCursorMoved()
            {
                this.ExperienceBar.OnCursorMoved();
            }

            public void OnButtonPressed(ButtonPressedEventArgs e, bool isClick)
            {
                if (isClick && this.ShouldBeDrawn && this.Hitbox.Contains(Game1.getMousePosition(true)))
                    this.OpenPrestigeMenu();
            }

            private void OpenPrestigeMenu()
            {
                Logger.LogVerbose("Selection Menu - Setting up Prestige Menu...");
                const int menuWidth = Game1.tileSize * 18;
                const int menuHeight = Game1.tileSize * 11;

                int menuXCenter = (menuWidth + IClickableMenu.borderWidth * 2) / 2;
                int menuYCenter = (menuHeight + IClickableMenu.borderWidth * 2) / 2;
                int screenXCenter = Game1.uiViewport.Width / 2;
                int screenYCenter = Game1.uiViewport.Height / 2;
                var bounds = new Rectangle(screenXCenter - menuXCenter, screenYCenter - menuYCenter, menuWidth + IClickableMenu.borderWidth * 2, menuHeight + IClickableMenu.borderWidth * 2);
                Game1.playSound("bigSelect");
                Logger.LogVerbose("Getting currently loaded prestige data...");
                var prestige = PrestigeSet.Instance.Prestiges.Single(x => x.SkillType == this.Skill.Type);
                Logger.LogVerbose($"Opening prestige menu for skill {this.Skill.Type.Name}");
                Game1.activeClickableMenu = new PrestigeMenu(bounds, this.Skill, prestige);
                Game1.nextClickableMenu.Add(this.ParentMenu);
                Mouse.ForceUseGamepadSelectionMouse = false;
                Logger.LogVerbose("Selection Menu - Loaded Prestige Menu.");
            }

            private bool CheckmarkTextureMissingLogged;
            public void Draw(SpriteBatch spriteBatch)
            {
                if (!this.ShouldBeDrawn) return;
                Utility.drawWithShadow(spriteBatch, this.Skill.SkillIconTexture, this.DrawLocation, this.Skill.SourceRectangleForSkillIcon, Color.White, 0.0f, Vector2.Zero, Game1.pixelZoom, false, 0.88f);
                spriteBatch.DrawString(Game1.smallFont, this.Skill.Type.Name, this.SkillNameDrawLocation, Game1.textColor);
                this.ExperienceBar.Draw(spriteBatch);
                //NumberSprite.draw(this.PrestigePoints, spriteBatch, this.PrestigePointDisplayLocation, Color.White, 1f, .85f, 1f, 0);
                string prestigePointText = this.PrestigePoints.ToString();
                if (this.CanPrestige) prestigePointText += "+";
                spriteBatch.DrawString(Game1.dialogueFont, prestigePointText, this.PrestigePointDisplayLocation, Color.Black);
                if (!this.Done) return;
                if (ModEntry.CheckmarkTexture == null)
                {
                    if (this.CheckmarkTextureMissingLogged) return;
                    Logger.LogWarning("Selection Menu - Checkmark texture not loaded, skipping checkmark draw action.");
                    this.CheckmarkTextureMissingLogged = true;
                    return;
                }
                var checkmarkSourceRectangle = new Rectangle(0, 0, 32, 32);
                var checkmarkDrawLocation = new Vector2(this._drawLocation.X + this.TotalWidth - ModEntry.CheckmarkTexture.Width, this._drawLocation.Y + (this.IconHeight / 2f).Floor() - (ModEntry.CheckmarkTexture.Height / 2f).Floor());
                spriteBatch.Draw(ModEntry.CheckmarkTexture, checkmarkDrawLocation, checkmarkSourceRectangle, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
            }

            public void DrawHover(SpriteBatch spriteBatch)
            {
                this.ExperienceBar.DrawHover(spriteBatch);
            }
        }
