/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Alphablackwolf/SkillPrestige
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Force.DeepCloner;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SkillPrestige.Framework;
using SkillPrestige.Framework.InputHandling;
using SkillPrestige.Framework.Menus.Dialogs;
using SkillPrestige.Framework.Menus.Elements.Buttons;
using SkillPrestige.Logging;
using SkillPrestige.Professions;
using SpaceCore;
using SpaceCore.Interface;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
// ReSharper disable PossibleLossOfFraction

namespace SkillPrestige.Menus
{
    /// <summary>Decorates the Level Up Menu with a prestiged! indicator on prestiged professions.</summary>
    public class LevelUpMenuDecorator : IClickableMenu, IInputHandler
    {
        private readonly Skill CurrentSkill;
        private readonly int CurrentLevel;
        private bool IsRightSideOfTree;
        private bool UiInitiated;
        private bool DrawToggleSwitch;
        private bool DrawLeftPrestigedIndicator;
        private bool DrawRightPrestigedIndicator;
        private TextureButton LevelTenToggleButton;
        private readonly IClickableMenu InternalMenu;

        private bool IsSpaceCoreMenu => this.InternalMenu is SkillLevelUpMenu;
        private Skills.Skill SpaceCoreSkill => this.IsSpaceCoreMenu ? Skills.GetSkill(this.CurrentSkill.Type.SpaceCoreSkillId) : null;
        private Rectangle MessageDialogBounds
        {
            get
            {
                int screenXCenter = Game1.uiViewport.Width / 2;
                int screenYCenter = Game1.uiViewport.Height / 2;
                const int dialogWidth = Game1.tileSize * 10;
                const int dialogHeight = Game1.tileSize * 8;
                int xLocation = screenXCenter - dialogWidth / 2;
                int yLocation = screenYCenter - dialogHeight / 2;
                return new Rectangle(xLocation, yLocation, dialogWidth, dialogHeight);
            }
        }

        private Rectangle ExtraTallMessageDialogBounds
        {
            get
            {
                var extraTallRectangle = this.MessageDialogBounds;
                extraTallRectangle.Height += Game1.tileSize * 4;
                return extraTallRectangle;
            }
        }

        public LevelUpMenuDecorator(Skill skill, int level, IClickableMenu internalMenu)
        {
            // init
            this.InternalMenu = internalMenu;
            this.CurrentSkill = skill;
            this.CurrentLevel = level;
            // exit decorator when the menu closes
            var prevExitFunction = this.InternalMenu.exitFunction;
            this.InternalMenu.exitFunction = () =>
            {
                prevExitFunction?.Invoke();
                this.exitThisMenu();
            };
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            this.InternalMenu.receiveRightClick(x, y, playSound);
        }

        public override void update(GameTime time)
        {
            this.InternalMenu.update(time);
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            this.InternalMenu.gameWindowSizeChanged(oldBounds, newBounds);
        }

        public override void draw(SpriteBatch spriteBatch)
        {
            this.InternalMenu.draw(spriteBatch);
            if (!this.UiInitiated)
            {
                try
                {
                    this.InitiateUi();
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Error initiating UI for level up menu decorator, reverting to internal menu. {Environment.NewLine} error: {ex.Message} {Environment.NewLine} stack trace {ex.StackTrace}");
                    Game1.activeClickableMenu = this.InternalMenu;
                }
            }
            this.DecorateUi(spriteBatch);
            this.drawMouse(spriteBatch);
        }

        /// <summary>Raised after the player moves the in-game cursor.</summary>
        /// <param name="e">The event data.</param>
        public void OnCursorMoved(CursorMovedEventArgs e)
        {
            this.LevelTenToggleButton?.OnCursorMoved(e);
        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="e">The event data.</param>
        /// <param name="isClick">Whether the button press is a click.</param>
        public void OnButtonPressed(ButtonPressedEventArgs e, bool isClick)
        {
            this.LevelTenToggleButton?.OnButtonPressed(e, isClick);
        }

        public override void snapToDefaultClickableComponent()
        {
            if (this.LevelTenToggleButton is not null)
            {
                this.currentlySnappedComponent = this.getComponentWithID(this.LevelTenToggleButton.ClickableTextureComponent.myID);
                this.snapCursorToCurrentSnappedComponent();
            }
            else this.InternalMenu.snapToDefaultClickableComponent();
        }

        private void InitiateUi()
        {
            if (this.UiInitiated)
                return;
            this.UiInitiated = true;
            Logger.LogVerbose("Level Up Menu - initializing UI...");
            var prestigeData = PrestigeSet.Instance.Prestiges.SingleOrDefault(x => x.SkillType.Name == this.CurrentSkill.Type.Name);
            if (prestigeData is null)
            {
                Logger.LogCriticalWarning($"Unable to obtain prestige data for skill {this.CurrentSkill.Type.Name}, reverting to basic level up menu");
                Game1.activeClickableMenu = this.InternalMenu;
                return;
            }
            var prestigedProfessionsForThisSkillAndLevel = this.CurrentSkill.Professions
                .Where(x => prestigeData.PrestigeProfessionsSelected.Contains(x.Id) && x.LevelAvailableAt == this.CurrentLevel)
                .ToList();
            var professionsToChooseFrom = this.CurrentSkill.Professions.Where(x => x.LevelAvailableAt == this.CurrentLevel).ToList();

            if (this.CurrentLevel == 5)
            {
                if (!prestigedProfessionsForThisSkillAndLevel.Any())
                {
                    Logger.LogVerbose("Level Up Menu - No prestiged professions found for this skill/level combination.");
                    return;
                }
                // ReSharper disable once ConvertIfStatementToSwitchStatement - reads better this way
                if (prestigedProfessionsForThisSkillAndLevel.Count == 1)
                {
                    Logger.LogInformation("Level Up Menu - One level 5 prestiged profession found, automatically selecting the other.");
                    var professionToAdd = professionsToChooseFrom.First(x => !prestigedProfessionsForThisSkillAndLevel.Contains(x));
                    Game1.player.professions.Add(professionToAdd.Id);
                    professionToAdd.SpecialHandling?.ApplyEffect();
                    this.exitThisMenu(false);
                    RemoveLevelFromLevelList(this.CurrentSkill.Type.Ordinal, this.CurrentLevel);
                    Game1.activeClickableMenu = new LevelUpMessageDialogWithProfession(this.MessageDialogBounds, $"You levelled your {this.CurrentSkill.Type.Name} skill to level {this.CurrentLevel} and gained a profession!", this.CurrentSkill, professionToAdd);
                    return;
                }
                if (prestigedProfessionsForThisSkillAndLevel.Count >= 2)
                {
                    Logger.LogInformation("Level Up Menu - Both available level 5 professions are already prestiged.");
                    this.exitThisMenu(false);
                    RemoveLevelFromLevelList(this.CurrentSkill.Type.Ordinal, this.CurrentLevel);
                    Game1.activeClickableMenu = new LevelUpMessageDialog(this.MessageDialogBounds, $"You levelled your {this.CurrentSkill.Type.Name} skill to level {this.CurrentLevel}!", this.CurrentSkill);
                    return;
                }
            }
            if (this.CurrentLevel != 10)
                return;

            int levelFiveProfessionsCount = Game1.player.professions
                .Intersect(
                    this.CurrentSkill.Professions.Where(x => x is TierOneProfession).Select(x => x.Id)
                )
                .Count();
            if (levelFiveProfessionsCount == 1)
            {
                if (!prestigedProfessionsForThisSkillAndLevel.Any())
                {
                    Logger.LogVerbose("Level Up Menu - No prestiged professions found for this skill/level combination.");
                    return;
                }
                // ReSharper disable once ConvertIfStatementToSwitchStatement - reads better without
                if (prestigedProfessionsForThisSkillAndLevel.Count == 1)
                {
                    Logger.LogInformation("Level Up Menu - One level 10 prestiged profession found for only one available level 5 skill (cheater!), automatically selecting the other.");
                    var tierOneProfession = ((TierTwoProfession)prestigedProfessionsForThisSkillAndLevel.First()).TierOneProfession;
                    var professionToAdd = professionsToChooseFrom
                        .First(x =>
                            (x as TierTwoProfession)?.TierOneProfession == tierOneProfession
                            && !prestigedProfessionsForThisSkillAndLevel.Contains(x)
                        );
                    Game1.player.professions.Add(professionToAdd.Id);
                    professionToAdd.SpecialHandling?.ApplyEffect();
                    this.exitThisMenu(false);
                    RemoveLevelFromLevelList(this.CurrentSkill.Type.Ordinal, this.CurrentLevel);
                    Game1.activeClickableMenu = new LevelUpMessageDialogWithProfession(this.ExtraTallMessageDialogBounds, $"You levelled your {this.CurrentSkill.Type.Name} skill to level {this.CurrentLevel} and gained a profession! {Environment.NewLine} You may now prestige this skill again!", this.CurrentSkill, professionToAdd);
                    return;
                }
                if (prestigedProfessionsForThisSkillAndLevel.Count < 2)
                    return;
                Logger.LogInformation("Level Up Menu - Only one level 5 profession found with both level 10 professions already prestiged (cheater!).");
                this.exitThisMenu(false);
                RemoveLevelFromLevelList(this.CurrentSkill.Type.Ordinal, this.CurrentLevel);
                Game1.activeClickableMenu = new LevelUpMessageDialog(this.MessageDialogBounds, $"You levelled your {this.CurrentSkill.Type.Name} skill to level {this.CurrentLevel}!  {Environment.NewLine} You may now prestige this skill again!", this.CurrentSkill);
            }
            else
            {
                // ReSharper disable once ConvertIfStatementToSwitchStatement
                if (prestigedProfessionsForThisSkillAndLevel.Count <= 2)
                {
                    Logger.LogInformation("Level Up Menu - Two or less prestiged level 10 professions found for this skill, with more than one level 5 profession found.");
                    if (prestigedProfessionsForThisSkillAndLevel.Intersect(professionsToChooseFrom.Take(2)).Count() == 2)
                    {
                        Logger.LogInformation("Level Up Menu - All of one level 10 profession branch found, switching to remaining menu.");
                        this.ToggleLevelTenMenu();
                        return;
                    }
                    if (prestigedProfessionsForThisSkillAndLevel.Intersect(professionsToChooseFrom.Skip(2).Take(2)).Count() == 2)
                    {
                        Logger.LogInformation("Level Up Menu - All of one level 10 profession branch found, leaving at default menu.");
                        return;
                    }
                    Logger.LogInformation("Level Up Menu - Both level up menus found as viable, enabling user side toggle.");
                    this.SetupLevelTenToggleButton();
                    this.DrawToggleSwitch = true;
                    this.DrawLeftPrestigedIndicator = prestigedProfessionsForThisSkillAndLevel
                        .Contains(professionsToChooseFrom.Skip(this.IsRightSideOfTree == false ? 0 : 2).First());
                    this.DrawRightPrestigedIndicator = prestigedProfessionsForThisSkillAndLevel
                        .Contains(professionsToChooseFrom.Skip(this.IsRightSideOfTree == false ? 1 : 3).First());
                    return;
                }
                if (prestigedProfessionsForThisSkillAndLevel.Count == 3)
                {
                    Logger.LogInformation("Level Up Menu - All but one level 10 profession found, selecting remaining profession.");
                    var professionToAdd = professionsToChooseFrom.First(x => !prestigedProfessionsForThisSkillAndLevel.Contains(x));
                    Game1.player.professions.Add(professionToAdd.Id);
                    professionToAdd.SpecialHandling?.ApplyEffect();
                    this.exitThisMenu(false);
                    RemoveLevelFromLevelList(this.CurrentSkill.Type.Ordinal, this.CurrentLevel);
                    Game1.activeClickableMenu = new LevelUpMessageDialogWithProfession(this.ExtraTallMessageDialogBounds, $"You levelled your {this.CurrentSkill.Type.Name} skill to level {this.CurrentLevel} and gained a profession!  {Environment.NewLine} You may now prestige this skill again!", this.CurrentSkill, professionToAdd);
                    return;
                }
                if (prestigedProfessionsForThisSkillAndLevel.Count < 4)
                    return;
                Logger.LogInformation("Level Up Menu - All professions already prestiged for this skill.");
                this.exitThisMenu(false);
                    RemoveLevelFromLevelList(this.CurrentSkill.Type.Ordinal, this.CurrentLevel);
                Game1.activeClickableMenu = new LevelUpMessageDialog(this.ExtraTallMessageDialogBounds, $"You levelled your {this.CurrentSkill.Type.Name} skill to level {this.CurrentLevel}!  {Environment.NewLine} Congratulations! You have prestiged all of your professions and reached level 10 again! You may continue to earn prestige points if you wish, as more prestige options are coming soon!", this.CurrentSkill);
            }
        }

        private void DecorateUi(SpriteBatch spriteBatch)
        {
            if (this.DrawToggleSwitch)
                this.LevelTenToggleButton.Draw(spriteBatch);
            this.DrawPrestigedIndicators(spriteBatch, this.DrawLeftPrestigedIndicator, this.DrawRightPrestigedIndicator);

        }

        private void DrawPrestigedIndicators(SpriteBatch spriteBatch, bool left, bool right)
        {
            const string text = "Prestiged!";
            const int textPadding = Game1.tileSize;
            int yPositionOfText = this.InternalMenu.yPositionOnScreen + this.InternalMenu.height + textPadding;
            if (left)
                spriteBatch.DrawString(Game1.dialogueFont, text, new Vector2(this.InternalMenu.xPositionOnScreen + this.InternalMenu.width / 4 - Game1.dialogueFont.MeasureString(text).X / 2, yPositionOfText), Color.LimeGreen);
            if (right)
                spriteBatch.DrawString(Game1.dialogueFont, text, new Vector2(this.InternalMenu.xPositionOnScreen + this.InternalMenu.width * 3 / 4 - Game1.dialogueFont.MeasureString(text).X / 2, yPositionOfText), Color.LimeGreen);
        }

        private void SetupLevelTenToggleButton()
        {
            if (this.LevelTenToggleButton != null)
                return;
            Logger.LogInformation("Level Up Menu - initiating level 10 toggle button...");
            var position = new Vector2(this.InternalMenu.xPositionOnScreen + this.InternalMenu.width + Game1.tileSize, this.InternalMenu.yPositionOnScreen);
            var bounds = new Rectangle(position.X.Floor(), position.Y.Floor(), Game1.tileSize, Game1.tileSize);
            this.LevelTenToggleButton = new TextureButton(bounds, Game1.mouseCursors, new Rectangle(0, 192, 64, 64), this.ToggleLevelTenMenu, "More professions...");

            this.SetupLevelTenForGamepad();

            Logger.LogInformation("Level Up Menu - Level 10 toggle button initiated.");
        }

        private void SetupLevelTenForGamepad()
        {
            this.LevelTenToggleButton.ClickableTextureComponent.myID = 5;
            this.LevelTenToggleButton.ClickableTextureComponent.downNeighborID = 6;
            ClickableComponent leftProfession;
            ClickableComponent rightProfession;
            if (this.IsSpaceCoreMenu)
            {
                var menu = this.InternalMenu as SkillLevelUpMenu;
                leftProfession = menu.leftProfession.DeepClone();
                rightProfession = menu.rightProfession.DeepClone();

                //use new bounds for the clickable textures for gamepad movement, this is copied from space core menu's profession icon draw location, default values are outside of menu.
                leftProfession.bounds =
                    new Rectangle(
                        this.InternalMenu.xPositionOnScreen + spaceToClearSideBorder + this.InternalMenu.width / 2 - 112,
                        this.InternalMenu.yPositionOnScreen + spaceToClearTopBorder + 160-16,
                        16,
                        16);
                rightProfession.bounds =
                    new Rectangle(
                        this.InternalMenu.xPositionOnScreen + spaceToClearSideBorder + this.InternalMenu.width - 128,
                        this.InternalMenu.yPositionOnScreen + spaceToClearTopBorder + 160 - 16,
                        16,
                        16);
            }
            else
            {
                var menu = this.InternalMenu as LevelUpMenu;
                leftProfession = menu.leftProfession;
                rightProfession = menu.rightProfession;
            }

            leftProfession.myID = 6;
            leftProfession.rightNeighborID = 7;
            leftProfession.upNeighborID = 5;
            rightProfession.myID = 7;
            rightProfession.leftNeighborID = 6;
            rightProfession.upNeighborID = 5;
            this.allClickableComponents = new List<ClickableComponent>
            {
                this.LevelTenToggleButton.ClickableTextureComponent,
                leftProfession,
                rightProfession
            };
        }

        private void ToggleLevelTenMenu()
        {
            Logger.LogInformation("Toggling level 10 menu...");
            this.IsRightSideOfTree = !this.IsRightSideOfTree;
            var professionsToChoose = this.CurrentSkill.Professions.Where(x => x is TierTwoProfession).Skip(this.IsRightSideOfTree ? 2 : 0).ToList();
            this.InternalMenu.SetInstanceField("professionsToChoose", professionsToChoose.Select(x => x.Id).ToList());

            this.InternalMenu.SetInstanceField("leftProfessionDescription", GetProfessionDescription(professionsToChoose[0]));
            this.InternalMenu.SetInstanceField("rightProfessionDescription", GetProfessionDescription(professionsToChoose[1]));
            if (this.IsSpaceCoreMenu)
            {
                var professionPair = this.SpaceCoreSkill.ProfessionsForLevels.SingleOrDefault(x => x.First.GetVanillaId() == professionsToChoose[0].Id);
                ((SkillLevelUpMenu)this.InternalMenu).SetInstanceField("profPair", professionPair);
            }
            var prestigeData = PrestigeSet.Instance.Prestiges.Single(x => x.SkillType == this.CurrentSkill.Type);
            var prestigedProfessionsForThisSkillAndLevel = this.CurrentSkill.Professions.Where(x => prestigeData.PrestigeProfessionsSelected.Contains(x.Id) && x.LevelAvailableAt == this.CurrentLevel).ToList();
            var professionsToChooseFrom = this.CurrentSkill.Professions.Where(x => x.LevelAvailableAt == this.CurrentLevel).ToList();
            this.DrawLeftPrestigedIndicator = prestigedProfessionsForThisSkillAndLevel.Contains(professionsToChooseFrom.Skip(this.IsRightSideOfTree == false ? 0 : 2).First());
            this.DrawRightPrestigedIndicator = prestigedProfessionsForThisSkillAndLevel.Contains(professionsToChooseFrom.Skip(this.IsRightSideOfTree == false ? 1 : 3).First());
        }

        private static List<string> GetProfessionDescription(Profession profession)
        {
            var returnList = new List<string>
            {
                profession.DisplayName
            };
            returnList.AddRange(profession.EffectText);
            return returnList;
        }

        private static void RemoveLevelFromLevelList(int skill, int level)
        {
            for (int index = 0; index < Game1.player.newLevels.Count; ++index)
            {
                var newLevel = Game1.player.newLevels[index];
                // ReSharper disable once InvertIf - reads cleaner this way
                if (newLevel.X == skill && newLevel.Y == level)
                {
                    Game1.player.newLevels.RemoveAt(index);
                    --index;
                }
            }
        }
    }
}
