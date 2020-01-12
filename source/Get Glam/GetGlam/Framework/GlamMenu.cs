using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Menus;
using System;
using System.Collections.Generic;

namespace GetGlam.Framework
{
    /// <summary>Class that draws the custom menu and allows the player to change appearance.<summary>
    public class GlamMenu : IClickableMenu
    {
        //Instance of ModEntry
        private ModEntry Entry;

        //The mods config
        private ModConfig Config;

        //Instance of ContentPackHelper
        private ContentPackHelper PackHelper;

        //Instance of DresserHandler
        private DresserHandler Dresser;

        //Instanc of PlayerLoader
        private CharacterLoader PlayerLoader;

        //List of new left buttons added to the menu
        private List<ClickableTextureComponent> NewLeftButtonsList = new List<ClickableTextureComponent>();

        //List of new right buttons added to the menu
        private List<ClickableTextureComponent> NewRightButtonsList = new List<ClickableTextureComponent>();

        //List of new labels added to the menu
        private List<ClickableComponent> NewLabels = new List<ClickableComponent>();

        //Button for the Hat Hair Fix
        private ClickableTextureComponent HatCoversHairButton;

        //Label for the Hait Hair Fix
        private ClickableComponent HatFixLabel;

        //Button to save a layout to the favorites
        private ClickableTextureComponent AddToFavoritesButton;

        //Tab Button for the favorite menu
        private ClickableTextureComponent FavoriteMenuTab;

        //Tab Button the Glam Menu
        private ClickableTextureComponent GlamMenuTab;

        //Okay Button
        private ClickableTextureComponent OkButton;

        //Cancel button
        private ClickableTextureComponent CancelButton;

        //Eye Color Picker
        private ColorPicker EyeColorPicker;

        //Hair Color Picker
        private ColorPicker HairColorPicker;

        //Both gender buttons
        private List<ClickableTextureComponent> GenderButtons = new List<ClickableTextureComponent>();

        //Whether the button is selected
        private bool IsHatFixSelected = false;

        //The index of the nose
        private int NoseIndex = 0;

        //The index of the face
        private int FaceIndex = 0;

        //The index of the base
        private int BaseIndex = 0;

        //The index of the base, there is always going to be 1 dresser
        private int DresserIndex = 1;

        //The indes of the shoe
        private int ShoeIndex = 0;

        //Whether the player is bald
        private bool IsBald = false;

        //Snapshot of the Farmer before making changes
        private int[] FarmerSnapshot = new int[9] {0, 0, 0, 0, 0, 0, 0, 0, 1};

        //Snapshot of haircolor
        private Color HairColorSnapshot;

        //SnapShot of eyecolor
        private Color EyeColorSnapshot;

        //Padding for each selection Y
        private int PaddingY = 8;

        //Wether to draw the base buttons
        private bool ShouldDrawBaseButtons = false;

        //Whether to draw the dresser button
        private bool ShouldDrawDresserButtons = false;

        //Whether to draw the face and nose buttons
        private bool ShouldDrawNosesAndFaceButtons = false;

        /// <summary>Glam Menu's Conrstructor</summary>
        /// <param name="entry">Instance of <see cref="ModEntry"/></param>
        /// <param name="packHelper">Instance of <see cref="ContentPackHelper"/></param>
        /// <param name="dresser">Instance of <see cref="DresserHandler"/></param>
        /// <param name="playerLoader">Instance of <seealso cref="CharacterLoader"/></param>
        public GlamMenu(ModEntry entry, ModConfig config, ContentPackHelper packHelper, DresserHandler dresser, CharacterLoader playerLoader)
            : base((int)Utility.getTopLeftPositionForCenteringOnScreen(712, 712, 0, 0).X, (int)Utility.getTopLeftPositionForCenteringOnScreen(712, 712, 0, 0).Y - IClickableMenu.borderWidth, 712, 712, false)
        {
            //Set the vars to the Instances
            Entry = entry;
            Config = config;
            PackHelper = packHelper;
            Dresser = dresser;
            PlayerLoader = playerLoader;

            //Check if they're wearing a hat
            if (Game1.player.hat.Value != null)
            {
                //Get the draw type value and change the HairFix button as selected
                if (Game1.player.hat.Value.hairDrawType.Get() == 0)
                    IsHatFixSelected = true;
            }

            //Set the positions of each item on the menu
            SetUpPositions();
        }

        /// <summary>Update the indexes on the menu when the player loads a layout</summary>
        /// <param name="baseindex">The base index</param>
        /// <param name="faceIndex">The face index</param>
        /// <param name="noseIndex">The nose index</param>
        /// <param name="shoeIndex">The shoe index</param>
        /// <param name="dresserIndex">The dresser index</param>
        /// <param name="isBald">Whether the player is bald</param>
        /// <remarks>This is only used when the player loads a layout from a json file</remarks>
        public void UpdateIndexes(int baseindex, int faceIndex, int noseIndex, int shoeIndex, bool isBald, int dresserIndex = 1)
        {
            BaseIndex = baseindex;
            FaceIndex = faceIndex;
            NoseIndex = noseIndex;
            ShoeIndex = shoeIndex;
            IsBald = isBald;

            if (dresserIndex != 1)
                DresserIndex = dresserIndex;
        }

        /// <summary>Takes a snapshot of the farmer when opening favorites/glam menu</summary>
        public void TakeSnapshot()
        {
            FarmerSnapshot[0] = Game1.player.isMale ? 0 : 1;
            FarmerSnapshot[1] = BaseIndex;
            FarmerSnapshot[2] = Game1.player.skin.Get();
            FarmerSnapshot[3] = Game1.player.hair.Get();
            FarmerSnapshot[4] = FaceIndex;
            FarmerSnapshot[5] = NoseIndex;
            FarmerSnapshot[6] = ShoeIndex;
            FarmerSnapshot[7] = Game1.player.accessory.Get();
            FarmerSnapshot[8] = IsBald ? 0 : 1;

            HairColorSnapshot = Game1.player.hairstyleColor.Get();
            EyeColorSnapshot = Game1.player.newEyeColor.Get();
        }

        /// <summary>Restores a snapshot that was taken</summary>
        public void RestoreSnapshot()
        {
            Game1.player.changeGender(FarmerSnapshot[0] == 0 ? true : false);
            Game1.player.changeSkinColor(FarmerSnapshot[2]);
            Game1.player.hair.Set(FarmerSnapshot[3]);
            Game1.player.changeAccessory(FarmerSnapshot[7]);

            Game1.player.newEyeColor.Set(EyeColorSnapshot);
            Game1.player.FarmerRenderer.recolorEyes(EyeColorSnapshot);
            Game1.player.changeHairColor(HairColorSnapshot);

            PackHelper.ChangePlayerBase(FarmerSnapshot[0] == 0 ? true : false, FarmerSnapshot[1], FarmerSnapshot[4], FarmerSnapshot[5], FarmerSnapshot[6], FarmerSnapshot[8] == 0 ? true : false);
        }

        /// <summary>Sets the position for all the UI elements in the menu</summary>
        private void SetUpPositions()
        {
            //Clear the lists incase of window size change
            NewLeftButtonsList.Clear();
            NewRightButtonsList.Clear();
            NewLabels.Clear();
            GenderButtons.Clear();

            //Add all the new left buttons to the list
            NewLeftButtonsList.Add(new ClickableTextureComponent("LeftBase", new Rectangle(this.xPositionOnScreen + 44, this.yPositionOnScreen + 128, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44, -1, -1), 1f, false));
            NewLeftButtonsList.Add(new ClickableTextureComponent("LeftChangeSkin", new Rectangle(this.xPositionOnScreen + 44, this.yPositionOnScreen + 192 + PaddingY, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44, -1, -1), 1f, false));
            NewLeftButtonsList.Add(new ClickableTextureComponent("LeftChangeHair", new Rectangle(this.xPositionOnScreen + 44, this.yPositionOnScreen + 256 + PaddingY, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44, -1, -1), 1f, false));
            NewLeftButtonsList.Add(new ClickableTextureComponent("LeftChangeAcc", new Rectangle(this.xPositionOnScreen + 44, this.yPositionOnScreen + 320 + PaddingY, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44, -1, -1), 1f, false));
            NewLeftButtonsList.Add(new ClickableTextureComponent("LeftFace", new Rectangle(this.xPositionOnScreen + 44, this.yPositionOnScreen + 384 + PaddingY, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44, -1, -1), 1f, false));
            NewLeftButtonsList.Add(new ClickableTextureComponent("LeftNose", new Rectangle(this.xPositionOnScreen + 44, this.yPositionOnScreen + 448 + PaddingY, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44, -1, -1), 1f, false));
            NewLeftButtonsList.Add(new ClickableTextureComponent("LeftShoe", new Rectangle(this.xPositionOnScreen + 44, this.yPositionOnScreen + 512 + PaddingY, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44, -1, -1), 1f, false));
            NewLeftButtonsList.Add(new ClickableTextureComponent("LeftDresser", new Rectangle(this.xPositionOnScreen + this.width / 2 - 114, this.yPositionOnScreen + 200, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44, -1, -1), 1f, false));
            NewLeftButtonsList.Add(new ClickableTextureComponent("LeftDirection", new Rectangle(this.xPositionOnScreen + this.width / 2 - 114, this.yPositionOnScreen + 288, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44, -1, -1), 1f, false));

            //Add all the new right buttons to the list
            NewRightButtonsList.Add(new ClickableTextureComponent("RightBase", new Rectangle(this.xPositionOnScreen + 170, this.yPositionOnScreen + 128, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33, -1, -1), 1f, false));
            NewRightButtonsList.Add(new ClickableTextureComponent("RightChangeSkin", new Rectangle(this.xPositionOnScreen + 170, this.yPositionOnScreen + 192 + PaddingY, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33, -1, -1), 1f, false));
            NewRightButtonsList.Add(new ClickableTextureComponent("RightChangeHair", new Rectangle(this.xPositionOnScreen + 170, this.yPositionOnScreen + 256 + PaddingY, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33, -1, -1), 1f, false));
            NewRightButtonsList.Add(new ClickableTextureComponent("RightChangeAcc", new Rectangle(this.xPositionOnScreen + 170, this.yPositionOnScreen + 320 + PaddingY, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33, -1, -1), 1f, false));
            NewRightButtonsList.Add(new ClickableTextureComponent("RightFace", new Rectangle(this.xPositionOnScreen + 170, this.yPositionOnScreen + 384 + PaddingY, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33, -1, -1), 1f, false));
            NewRightButtonsList.Add(new ClickableTextureComponent("RightNose", new Rectangle(this.xPositionOnScreen + 170, this.yPositionOnScreen + 448 + PaddingY, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33, -1, -1), 1f, false));
            NewRightButtonsList.Add(new ClickableTextureComponent("RightShoe", new Rectangle(this.xPositionOnScreen + 170, this.yPositionOnScreen + 512 + PaddingY, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33, -1, -1), 1f, false));
            NewRightButtonsList.Add(new ClickableTextureComponent("RightDresser", new Rectangle(this.xPositionOnScreen + this.width / 2 + 48, this.yPositionOnScreen + 200, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33, -1, -1), 1f, false));
            NewRightButtonsList.Add(new ClickableTextureComponent("RightDirection", new Rectangle(this.xPositionOnScreen + this.width / 2 + 48, this.yPositionOnScreen + 288, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33, -1, -1), 1f, false));

            //Create buttons.
            FavoriteMenuTab = new ClickableTextureComponent("FavoriteTab", new Rectangle(this.xPositionOnScreen - IClickableMenu.borderWidth - 8, this.yPositionOnScreen + 160, 64, 64), null, "FavoriteTab", Game1.mouseCursors, new Rectangle(656, 80, 16, 16), 4f, false);
            GlamMenuTab = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen - IClickableMenu.borderWidth + 1, this.yPositionOnScreen + 96, 64, 64), Game1.mouseCursors, new Rectangle(672, 80, 15, 16), 4f, false);
            HatCoversHairButton = new ClickableTextureComponent("HatFix", new Rectangle(this.xPositionOnScreen + this.width / 2 - 114, this.yPositionOnScreen + 128, 36, 36), null, "Hat Hair Fix", Game1.mouseCursors, new Rectangle(227, 425, 9, 9), 4f, false);
            AddToFavoritesButton = new ClickableTextureComponent("Favorite", new Rectangle(this.xPositionOnScreen + this.width - 96, this.yPositionOnScreen + 224, 48, 48), null, "Favorite", Game1.mouseCursors, new Rectangle(346, 392, 8, 8), 6f, false);

            HatCoversHairButton.visible = false;

            //Add the new labels to the list
            NewLabels.Add(new ClickableComponent(new Rectangle(NewLeftButtonsList[0].bounds.X + 70, NewLeftButtonsList[0].bounds.Y + 16, 1, 1), "Base", "Base"));
            NewLabels.Add(new ClickableComponent(new Rectangle(NewLeftButtonsList[1].bounds.X + 70, NewLeftButtonsList[1].bounds.Y + 16, 1, 1), "Skin", "Skin"));
            NewLabels.Add(new ClickableComponent(new Rectangle(NewLeftButtonsList[2].bounds.X + 70, NewLeftButtonsList[2].bounds.Y + 16, 1, 1), "Hair", "Hair"));
            NewLabels.Add(new ClickableComponent(new Rectangle(NewLeftButtonsList[3].bounds.X + 70, NewLeftButtonsList[3].bounds.Y + 16, 1, 1), "Acc.", "Acc."));
            NewLabels.Add(new ClickableComponent(new Rectangle(NewLeftButtonsList[4].bounds.X + 70, NewLeftButtonsList[4].bounds.Y + 16, 1, 1), "Face", "Face"));
            NewLabels.Add(new ClickableComponent(new Rectangle(NewLeftButtonsList[5].bounds.X + 70, NewLeftButtonsList[5].bounds.Y + 16, 1, 1), "Nose", "Nose"));
            NewLabels.Add(new ClickableComponent(new Rectangle(NewLeftButtonsList[6].bounds.X + 70, NewLeftButtonsList[6].bounds.Y + 16, 1, 1), "Shoe", "Shoe"));
            NewLabels.Add(new ClickableComponent(new Rectangle(NewLeftButtonsList[7].bounds.X + 70, NewLeftButtonsList[7].bounds.Y + 16, 1, 1), "Dresser", "Dresser"));
            NewLabels.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + width - 212, this.yPositionOnScreen + 272, 1, 1), "Eye Color:", "Eye Color:"));
            NewLabels.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + width - 212, this.yPositionOnScreen + 360, 1, 1), "Hair Color:", "Hair Color:"));

            HatFixLabel = new ClickableComponent(new Rectangle(HatCoversHairButton.bounds.X + 48, HatCoversHairButton.bounds.Y, 1, 1), "Hat Ignores Hair", "Hat Ignores Hair");

            //Create the color pickers and assign their values
            EyeColorPicker = new ColorPicker("Eyes", NewLabels[8].bounds.X, NewLabels[8].bounds.Y + 32);
            EyeColorPicker.setColor(Game1.player.newEyeColor.Value);
            HairColorPicker = new ColorPicker("Hair", NewLabels[9].bounds.X, NewLabels[9].bounds.Y + 32);
            HairColorPicker.setColor(Game1.player.hairstyleColor.Value);

            //Add male gender button
            GenderButtons.Add(new ClickableTextureComponent(
                "Male",
                new Rectangle(xPositionOnScreen + this.width - IClickableMenu.spaceToClearSideBorder - IClickableMenu.borderWidth - 128, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder, 64, 64),
                null,
                "Male",
                Game1.mouseCursors,
                new Rectangle(128, 192, 16, 16),
                4f,
                false)
            );

            //Add female gender button
            GenderButtons.Add(new ClickableTextureComponent(
                "Female",
                new Rectangle(xPositionOnScreen + this.width - IClickableMenu.spaceToClearSideBorder - IClickableMenu.borderWidth - 64, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder, 64, 64),
                null,
                "Female",
                Game1.mouseCursors,
                new Rectangle(144, 192, 16, 16),
                4f,
                false)
            );

            //Create the Ok button
            OkButton = new ClickableTextureComponent("Ok", new Rectangle(this.xPositionOnScreen + width - 108, this.yPositionOnScreen + height - 108, 64, 64), null, null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46, -1, -1), 1f, false);

            //Create the cancel button
            CancelButton = new ClickableTextureComponent("Cancel", new Rectangle(OkButton.bounds.X - 74, this.OkButton.bounds.Y, 64, 64), null, null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 47, -1, -1), 1f, false);

            //Set the buttons to hide by default visibility to false
            NewLeftButtonsList[0].visible = false;
            NewRightButtonsList[0].visible = false;
            NewLeftButtonsList[7].visible = false;
            NewRightButtonsList[7].visible = false;

            //Check if the menu should draw the base buttons if the base count is greater than 1
            if (Game1.player.isMale && PackHelper.MaleBaseTextureList.Count > 0)
            {
                ShouldDrawBaseButtons = true;
                NewLeftButtonsList[0].visible = true;
                NewRightButtonsList[0].visible = true;
            }
            else if (PackHelper.FemaleBaseTextureList.Count > 0)
            {
                ShouldDrawBaseButtons = true;
                NewLeftButtonsList[0].visible = true;
                NewRightButtonsList[0].visible = true;
            }

            //Check if the menu should draw the dresser buttons
            if (Dresser.GetNumberOfDressers() > 1)
            {
                ShouldDrawDresserButtons = true;
                NewLeftButtonsList[7].visible = true;
                NewRightButtonsList[7].visible = true;
            }

            if (PackHelper.GetNumberOfFacesAndNoses(Game1.player.isMale, BaseIndex, true) > 0)
            {
                ShouldDrawNosesAndFaceButtons = true;
                UpdateFaceAndNoseButtonsPositions(ShouldDrawNosesAndFaceButtons);
            }
            else
            {
                ShouldDrawNosesAndFaceButtons = false;
                UpdateFaceAndNoseButtonsPositions(ShouldDrawNosesAndFaceButtons);
            }
        }

        /// <summary>Override to change the menu when the window size changes</summary>
        /// <param name="oldBounds">The old bounds</param>
        /// <param name="newBounds">The new bounds</param>
        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            //Call the base version and update the x and y position of the menu
            base.gameWindowSizeChanged(oldBounds, newBounds);
            this.xPositionOnScreen = Game1.viewport.Width / 2 - (632 + IClickableMenu.borderWidth * 2) / 2;
            this.yPositionOnScreen = Game1.viewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2 - 64;

            //Set up the UI elements again
            SetUpPositions();
        }

        /// <summary>Override that checks if the mouse is above a certain elememt</summary>
        /// <param name="x">The x position</param>
        /// <param name="y">The y position</param>
        public override void performHoverAction(int x, int y)
        {
            //Call the base version
            base.performHoverAction(x, y);

            //Change the scale of the new left buttons
            foreach (ClickableTextureComponent leftButton in NewLeftButtonsList)
            {
                if (leftButton.containsPoint(x, y))
                    leftButton.scale = Math.Min(leftButton.scale + 0.02f, leftButton.baseScale + 0.1f);
                else
                    leftButton.scale = Math.Max(leftButton.scale - 0.02f, leftButton.baseScale);
            }

            //Change the scale of the new right buttons
            foreach (ClickableTextureComponent rightButton in NewRightButtonsList)
            {
                if (rightButton.containsPoint(x, y))
                    rightButton.scale = Math.Min(rightButton.scale + 0.02f, rightButton.baseScale + 0.1f);
                else
                    rightButton.scale = Math.Max(rightButton.scale - 0.02f, rightButton.baseScale);
            }

            //Change the scale of the gender buttons
            foreach (ClickableTextureComponent genderButton in GenderButtons)
            {
                if (genderButton.containsPoint(x, y))
                    genderButton.scale = Math.Min(genderButton.scale + 0.05f, genderButton.baseScale + 0.1f);
                else
                    genderButton.scale = Math.Max(genderButton.scale - 0.05f, genderButton.baseScale);
            }

            if (EyeColorPicker.containsPoint(x, y) || HairColorPicker.containsPoint(x, y))
                Game1.SetFreeCursorDrag();

            if (AddToFavoritesButton.containsPoint(x, y))
                AddToFavoritesButton.scale = Math.Min(AddToFavoritesButton.scale + 0.05f, AddToFavoritesButton.baseScale + 0.5f);
            else
                AddToFavoritesButton.scale = Math.Max(AddToFavoritesButton.scale - 0.05f, AddToFavoritesButton.baseScale);

            if (OkButton.containsPoint(x, y))
                OkButton.scale = Math.Min(OkButton.scale + 0.05f, OkButton.baseScale + 0.1f);
            else
                OkButton.scale = Math.Max(OkButton.scale - 0.05f, OkButton.baseScale);

            if (CancelButton.containsPoint(x, y))
                CancelButton.scale = Math.Min(CancelButton.scale + 0.05f, CancelButton.baseScale + 0.1f);
            else
                CancelButton.scale = Math.Max(CancelButton.scale - 0.05f, CancelButton.baseScale);
        }

        /// <summary>Override that handles recieving a left click</summary>
        /// <param name="x">The x position</param>
        /// <param name="y">The y position</param>
        /// <param name="playSound">Whether to play the sounds</param>
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            //Call the base version
            base.receiveLeftClick(x, y, playSound);

            //Check if the Hat Hair fix button is pressed and change accordingly
            if (HatCoversHairButton.bounds.Contains(x, y) && !IsHatFixSelected && HatCoversHairButton.visible)
            {
                IsHatFixSelected = true;
                Game1.player.hat.Value.hairDrawType.Set(0);
                HatCoversHairButton.sourceRect.X = 236;
            }
            else if (HatCoversHairButton.bounds.Contains(x, y) && IsHatFixSelected && HatCoversHairButton.visible)
            {
                IsHatFixSelected = false;
                Game1.player.hat.Value.hairDrawType.Set(1);
                HatCoversHairButton.sourceRect.X = 227;
            }

            //Eye and Color Picker clicks
            if (EyeColorPicker.containsPoint(x, y))
                Game1.player.changeEyeColor(EyeColorPicker.click(x, y));

            if (HairColorPicker.containsPoint(x, y))
                Game1.player.changeHairColor(HairColorPicker.click(x, y));

            //Check if any of the new left buttons were clicked
            foreach (ClickableTextureComponent component in NewLeftButtonsList)
            {
                if (component.bounds.Contains(x, y))
                {  
                    SelectionClick(component.name, -1);
                    if (component.scale != 0f)
                    {
                        component.scale -= 0.25f;
                        component.scale = Math.Max(0.75f, component.scale);
                    }

                    if (Game1.player.hair.Value - 49 <= 6 && !IsBald && component.name.Contains("ChangeHair"))
                    {
                        IsBald = true;
                        PackHelper.ChangePlayerBase(Game1.player.isMale, BaseIndex, FaceIndex, NoseIndex, ShoeIndex, IsBald);
                    }
                    else if (IsBald && !(Game1.player.hair.Value - 49 <= 6) && component.name.Contains("ChangeHair"))
                    {
                        IsBald = false;
                        PackHelper.ChangePlayerBase(Game1.player.isMale, BaseIndex, FaceIndex, NoseIndex, ShoeIndex, IsBald);
                    }

                    Game1.playSound("grassyStep");
                }
            }

            //Check if any of the right buttons were clicked
            foreach (ClickableTextureComponent component in NewRightButtonsList)
            {
                if (component.bounds.Contains(x, y))
                { 
                    SelectionClick(component.name, 1);
                    if (component.scale != 0f)
                    {
                        component.scale -= 0.25f;
                        component.scale = Math.Max(0.75f, component.scale);
                    }

                    if (Game1.player.hair.Value - 49 <= 6 && !IsBald && component.name.Contains("ChangeHair"))
                    {
                        IsBald = true;
                        PackHelper.ChangePlayerBase(Game1.player.isMale, BaseIndex, FaceIndex, NoseIndex, ShoeIndex, IsBald);
                    }
                    else if (IsBald && !(Game1.player.hair.Value - 49 <= 6) && component.name.Contains("ChangeHair"))
                    {
                        IsBald = false;
                        PackHelper.ChangePlayerBase(Game1.player.isMale, BaseIndex, FaceIndex, NoseIndex, ShoeIndex, IsBald);
                    }

                    Game1.playSound("grassyStep");
                }
            }

            //Check if any of the gender buttons are clicked
            foreach (ClickableTextureComponent genderButton in GenderButtons)
            {
                if (genderButton.containsPoint(x, y))
                {
                    SelectionClick(genderButton.name, 0);
                    Game1.playSound("yoba");
                }
            }

            //Check if the add to favorites button was clicked
            if (AddToFavoritesButton.containsPoint(x, y))
            {
                PlayerLoader.SaveFavoriteToList(Game1.player.isMale, BaseIndex, Game1.player.skin.Get(), Game1.player.hair.Get(), FaceIndex, NoseIndex, ShoeIndex, Game1.player.accessory.Get(), IsBald);
                if (AddToFavoritesButton.scale != 0f)
                {
                    AddToFavoritesButton.scale -= 0.25f;
                    AddToFavoritesButton.scale = Math.Max(0.75f, AddToFavoritesButton.scale);
                }
                Game1.playSound("coin");
            }

            //Take a snapshot of the farmer and open the favorites menu
            if (FavoriteMenuTab.containsPoint(x, y))
            {
                TakeSnapshot();
                Game1.activeClickableMenu = new FavoriteMenu(Entry, PlayerLoader, this);
            }

            //Check the okbutton again to save the player when the menu closes
            if (OkButton.containsPoint(x, y))
            {
                PlayerLoader.SaveCharacterLayout(Game1.player.isMale, BaseIndex, Game1.player.skin.Value, Game1.player.hair.Value, FaceIndex, NoseIndex, ShoeIndex, Game1.player.accessory.Value, DresserIndex, IsBald);
                Game1.exitActiveMenu();
                Game1.flashAlpha = 1f;
                Game1.playSound("yoba");
            }

            //Check if the cancel button was clicked
            if (CancelButton.containsPoint(x, y))
            {
                RestoreSnapshot();
                Game1.activeClickableMenu = null;
                if (CancelButton.scale != 0f)
                {
                    CancelButton.scale -= 0.25f;
                    CancelButton.scale = Math.Max(0.75f, CancelButton.scale);
                }
                Game1.playSound("bigDeSelect");
            }
        }

        /// <summary>Override to handle the left click being held</summary>
        /// <param name="x">The x position of the mouse</param>
        /// <param name="y">The y position of the mouse</param>
        public override void leftClickHeld(int x, int y)
        {
            base.leftClickHeld(x, y);

            //Tell the color pickers that the click was held and change the color
            if (EyeColorPicker.containsPoint(x, y))
            {
                EyeColorPicker.clickHeld(x, y);
                Game1.player.newEyeColor.Set(EyeColorPicker.getSelectedColor());
                Game1.player.changeEyeColor(Game1.player.newEyeColor);

            }

            if (HairColorPicker.containsPoint(x, y))
            {
                HairColorPicker.clickHeld(x, y);
                Game1.player.hairstyleColor.Set(HairColorPicker.getSelectedColor());
                Game1.player.changeHairColor(Game1.player.hairstyleColor);
            }
        }

        /// <summary>Override to handle the left click being released</summary>
        /// <param name="x">The x position of the mouse</param>
        /// <param name="y">The y position of the mouse</param>
        public override void releaseLeftClick(int x, int y)
        {
            base.releaseLeftClick(x, y);

            //Tell the color pickers that the click was released
            if (EyeColorPicker.containsPoint(x, y))
                EyeColorPicker.releaseClick();

            if (HairColorPicker.containsPoint(x, y))
                HairColorPicker.releaseClick();
        }

        /// <summary>Update the buttons for changing the face and nose.</summary>
        /// <param name="isFaceAndNoseDrawing">Wether the face and nose buttons are drawing</param>
        private void UpdateFaceAndNoseButtonsPositions(bool isFaceAndNoseDrawing)
        {
            if (isFaceAndNoseDrawing)
            {
                NewLeftButtonsList[4].visible = true;
                NewLeftButtonsList[5].visible = true;
                NewRightButtonsList[4].visible = true;
                NewRightButtonsList[5].visible = true;

                NewLeftButtonsList[6].bounds.Y = this.yPositionOnScreen + 512 + PaddingY;
                NewRightButtonsList[6].bounds.Y = this.yPositionOnScreen + 512 + PaddingY;
                NewLabels[6].bounds.Y = NewLeftButtonsList[6].bounds.Y + 16;
            }
            else
            {
                NewLeftButtonsList[4].visible = false;
                NewLeftButtonsList[5].visible = false;
                NewRightButtonsList[4].visible = false;
                NewRightButtonsList[5].visible = false;

                NewLeftButtonsList[6].bounds.Y = this.yPositionOnScreen + 384 + PaddingY;
                NewRightButtonsList[6].bounds.Y = this.yPositionOnScreen + 384 + PaddingY;
                NewLabels[6].bounds.Y = NewLeftButtonsList[6].bounds.Y + 16;
            }
        }

        /// <summary>Handles which index to move and by what direction</summary>
        /// <param name="name">The name of the button</param>
        /// <param name="direction">Which direction to move the indexes</param>
        private void SelectionClick(string name, int direction)
        {
            //Switch statement of the different button names
            switch (name)
            {
                case "LeftBase":
                    if (!NewLeftButtonsList[0].visible)
                        return;

                    if (BaseIndex + direction > -1)
                        BaseIndex += direction;
                    else
                        BaseIndex = Game1.player.isMale ? PackHelper.MaleBaseTextureList.Count : PackHelper.FemaleBaseTextureList.Count;

                    FaceIndex = 0;
                    NoseIndex = 0;
                    PackHelper.ChangePlayerBase(Game1.player.isMale, BaseIndex, FaceIndex, NoseIndex, ShoeIndex, IsBald);
                    break;
                case "RightBase":
                    if (!NewRightButtonsList[0].visible)
                        return;

                    if (BaseIndex + direction > (Game1.player.isMale ? PackHelper.MaleBaseTextureList.Count : PackHelper.FemaleBaseTextureList.Count))
                        BaseIndex = 0;
                    else
                        BaseIndex += direction;

                    FaceIndex = 0;
                    NoseIndex = 0;
                    PackHelper.ChangePlayerBase(Game1.player.isMale, BaseIndex, FaceIndex, NoseIndex, ShoeIndex, IsBald);
                    break;
                case "LeftNose":
                    if (NoseIndex + direction > -1)
                        NoseIndex += direction;
                    else
                        NoseIndex = PackHelper.GetNumberOfFacesAndNoses(Game1.player.isMale, BaseIndex, false);

                    PackHelper.ChangePlayerBase(Game1.player.isMale, BaseIndex, FaceIndex, NoseIndex, ShoeIndex, IsBald);
                    break;
                case "RightNose":
                    if (NoseIndex + direction > PackHelper.GetNumberOfFacesAndNoses(Game1.player.isMale, BaseIndex, false))
                        NoseIndex = 0;
                    else
                        NoseIndex += direction;

                    PackHelper.ChangePlayerBase(Game1.player.isMale, BaseIndex, FaceIndex, NoseIndex, ShoeIndex, IsBald);
                    break;
                case "LeftFace":
                    if (FaceIndex + direction > -1)
                        FaceIndex += direction;
                    else
                        FaceIndex = PackHelper.GetNumberOfFacesAndNoses(Game1.player.isMale, BaseIndex, true);

                    PackHelper.ChangePlayerBase(Game1.player.isMale, BaseIndex, FaceIndex, NoseIndex, ShoeIndex, IsBald);
                    break;
                case "RightFace":
                    if (FaceIndex + direction > PackHelper.GetNumberOfFacesAndNoses(Game1.player.isMale, BaseIndex, true))
                        FaceIndex = 0;
                    else
                        FaceIndex += direction;

                    PackHelper.ChangePlayerBase(Game1.player.isMale, BaseIndex, FaceIndex, NoseIndex, ShoeIndex, IsBald);
                    break;
                case "LeftShoe":
                    if (ShoeIndex + direction > -1)
                        ShoeIndex += direction;
                    else
                        ShoeIndex = Game1.player.isMale ? PackHelper.MaleShoeTextureList.Count: PackHelper.FemaleShoeTextureList.Count;

                    PackHelper.ChangePlayerBase(Game1.player.isMale, BaseIndex, FaceIndex, NoseIndex, ShoeIndex, IsBald);
                    break;
                case "RightShoe":
                    if (ShoeIndex + direction > (Game1.player.isMale ? PackHelper.MaleShoeTextureList.Count : PackHelper.FemaleShoeTextureList.Count))
                        ShoeIndex = 0;
                    else
                        ShoeIndex += direction;

                    PackHelper.ChangePlayerBase(Game1.player.isMale, BaseIndex, FaceIndex, NoseIndex, ShoeIndex, IsBald);
                    break;
                case "LeftDresser":
                    if (!NewLeftButtonsList[7].visible)
                        return;

                    if (DresserIndex + direction > 0)
                        DresserIndex += direction;
                    else
                        DresserIndex = Dresser.GetNumberOfDressers();

                    //Change the Dressers source rect and update the dresser in the farmhouse
                    Dresser.TextureSourceRect.Y = DresserIndex.Equals(1) ? 0 : DresserIndex * 32 - 32;
                    Dresser.SetDresserTileSheetPoint(DresserIndex);
                    Dresser.UpdateDresserInFarmHouse();
                    break;
                case "RightDresser":
                    if (!NewRightButtonsList[7].visible)
                        return;

                    if (DresserIndex + direction <= Dresser.GetNumberOfDressers())
                        DresserIndex += direction;
                    else
                        DresserIndex = 1;

                    //Change the Dressers source rect and update the dresser in the farmhouse
                    Dresser.TextureSourceRect.Y = DresserIndex.Equals(1) ? 0 : DresserIndex * 32- 32; 
                    Dresser.SetDresserTileSheetPoint(DresserIndex);
                    Dresser.UpdateDresserInFarmHouse();
                    break;
                case "Male":
                    Game1.player.changeGender(true);

                    //Reset the BaseIndex, ShoeIndex to prevent crashing
                    BaseIndex = 0;
                    ShoeIndex = 0;
                    PackHelper.ChangePlayerBase(Game1.player.isMale, BaseIndex, FaceIndex, NoseIndex, ShoeIndex, IsBald);
                    break;
                case "Female":
                    Game1.player.changeGender(false);
                    
                    //Reset the BaseIndex, ShoeIndex to prevent crashing
                    BaseIndex = 0;
                    ShoeIndex = 0;
                    PackHelper.ChangePlayerBase(Game1.player.isMale, BaseIndex, FaceIndex, NoseIndex, ShoeIndex, IsBald);
                    break;
                case "LeftChangeHair":
                    if (Game1.player.hair.Get().Equals(0) && PackHelper.NumberOfHairstlyesAdded == 56)
                        Game1.player.hair.Set(FarmerRenderer.hairStylesTexture.Height / 96 * 8 - 1);
                    else if (Game1.player.hair.Get().Equals(0) && PackHelper.NumberOfHairstlyesAdded != 56)
                        Game1.player.hair.Set(PackHelper.NumberOfHairstlyesAdded - 1);
                    else
                        Game1.player.hair.Set(Game1.player.hair.Value + direction);
                    break;   
                case "RightChangeHair":
                    if (PackHelper.NumberOfHairstlyesAdded == 56 && Game1.player.hair.Value + 1 > FarmerRenderer.hairStylesTexture.Height / 96 * 8 - 1)
                        Game1.player.hair.Set(0);
                    else if (PackHelper.NumberOfHairstlyesAdded != 56 && Game1.player.hair.Value.Equals(PackHelper.NumberOfHairstlyesAdded - 1))
                        Game1.player.hair.Set(0);
                    else
                        Game1.player.hair.Set(Game1.player.hair.Value + direction);
                    break;
                case "LeftChangeSkin":
                case "RightChangeSkin":
                    Game1.player.changeSkinColor(Game1.player.skin.Value + direction);
                    break;
                case "LeftChangeAcc":
                case "RightChangeAcc":
                    Game1.player.changeAccessory(Game1.player.accessory.Value + direction);
                    break;
                case "LeftDirection":
                case "RightDirection":
                    Game1.player.faceDirection((Game1.player.FacingDirection - direction + 4) % 4);
                    Game1.player.FarmerSprite.StopAnimation();
                    Game1.player.completelyStopAnimatingOrDoingAction();
                    break;
            }
        }

        /// <summary>Override to draw the different menu parts</summary>
        /// <param name="b">The games spritebatch</param>
        public override void draw(SpriteBatch b)
        {
            //Draw the dialogue box or else Minerva will haunt my dreams
            Game1.drawDialogueBox(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, false, true);

            //Draw the tabs
            FavoriteMenuTab.draw(b);
            GlamMenuTab.draw(b);

            //Draw the Dresser Texture if the Config is true
            if (Config.DrawDresserInMenu)
                b.Draw(Dresser.Texture, new Vector2(this.xPositionOnScreen + this.width / 2 - 96, this.yPositionOnScreen + this.height / 2 - 80), Dresser.TextureSourceRect, Color.White, 0f, Vector2.Zero, 12f, SpriteEffects.None, 0.86f);
            else
            {
                b.Draw(Game1.daybg, new Vector2(this.xPositionOnScreen + this.width / 2 - 64, this.yPositionOnScreen + this.height / 2 - 64), Color.White);
                if (ShouldDrawDresserButtons)
                    b.Draw(Dresser.Texture, new Vector2(NewRightButtonsList[4].bounds.X + 64, NewRightButtonsList[4].bounds.Y), Dresser.TextureSourceRect, Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0.86f);
            }

            //Draw the Farmer!!!
            Game1.player.FarmerRenderer.draw(b, Game1.player.FarmerSprite.CurrentAnimationFrame, Game1.player.FarmerSprite.CurrentFrame, Game1.player.FarmerSprite.SourceRect, new Vector2((this.xPositionOnScreen + this.width / 2 - 32), (this.yPositionOnScreen + this.height / 2 - 32)), Vector2.Zero, 0.8f, Color.White, 0f, 1f, Game1.player);

            //Draw the add to favorites button and the text
            AddToFavoritesButton.draw(b);
            Utility.drawTextWithShadow(b, "Add Fav:", Game1.smallFont, new Vector2(AddToFavoritesButton.bounds.X - 112, AddToFavoritesButton.bounds.Y + 8), Game1.textColor);

            //Draw the eye color picker and hair color picker
            EyeColorPicker.draw(b);
            HairColorPicker.draw(b);

            //Check if the player is wearing a hat
            if (Game1.player.hat.Value != null)
            {
                HatCoversHairButton.visible = true;
                HatCoversHairButton.draw(b);
                Utility.drawTextWithShadow(b, HatFixLabel.name, Game1.smallFont, new Vector2(HatFixLabel.bounds.X, HatFixLabel.bounds.Y), Game1.textColor);

                if (IsHatFixSelected)
                    HatCoversHairButton.sourceRect.X = 236;
            }

            //Draw each of the new left buttons
            foreach (ClickableTextureComponent component in NewLeftButtonsList)
            {
                if (component.name.Contains("Base") && ShouldDrawBaseButtons)
                    component.draw(b);
                else if (component.name.Contains("Dresser") && ShouldDrawDresserButtons)
                    component.draw(b);
                else if ((component.name.Contains("Nose") || component.name.Contains("Face") && ShouldDrawNosesAndFaceButtons))
                    component.draw(b);
                else if (!component.name.Contains("Dresser") && !component.name.Contains("Base") && !component.name.Contains("Face") && !component.name.Contains("Nose"))
                    component.draw(b);
            }

            //Draw each of the new right buttons
            foreach (ClickableTextureComponent component in NewRightButtonsList)
            {
                if (component.name.Contains("Base") && ShouldDrawBaseButtons)
                    component.draw(b);
                else if (component.name.Contains("Dresser") && ShouldDrawDresserButtons)
                    component.draw(b);
                else if ((component.name.Contains("Nose") || component.name.Contains("Face") && ShouldDrawNosesAndFaceButtons))
                    component.draw(b);
                else if (!component.name.Contains("Dresser") && !component.name.Contains("Base") && !component.name.Contains("Face") && !component.name.Contains("Nose"))
                    component.draw(b);
            }

            //Draw each of the new labels, I hate this tbh, this might needs to be done a different way
            foreach (ClickableComponent component in NewLabels)
            {
                if (component.name.Equals("Base") && ShouldDrawBaseButtons)
                {
                    Utility.drawTextWithShadow(b, component.name, Game1.smallFont, new Vector2(component.bounds.X, component.bounds.Y), Game1.textColor);
                    Utility.drawTextWithShadow(b, BaseIndex.ToString(), Game1.smallFont, new Vector2(component.bounds.X + 16, component.bounds.Y + 32), Game1.textColor);
                }
                else if (component.name.Equals("Face") && ShouldDrawNosesAndFaceButtons)
                {
                    Utility.drawTextWithShadow(b, component.name, Game1.smallFont, new Vector2(component.bounds.X, component.bounds.Y), Game1.textColor);
                    Utility.drawTextWithShadow(b, FaceIndex.ToString(), Game1.smallFont, new Vector2(component.bounds.X + 16, component.bounds.Y + 32), Game1.textColor);
                }
                else if (component.name.Equals("Nose") && ShouldDrawNosesAndFaceButtons)
                {
                    Utility.drawTextWithShadow(b, component.name, Game1.smallFont, new Vector2(component.bounds.X, component.bounds.Y), Game1.textColor);
                    Utility.drawTextWithShadow(b, NoseIndex.ToString(), Game1.smallFont, new Vector2(component.bounds.X + 16, component.bounds.Y + 32), Game1.textColor);
                }
                else if (component.name.Equals("Shoe"))
                {
                    Utility.drawTextWithShadow(b, component.name, Game1.smallFont, new Vector2(component.bounds.X, component.bounds.Y), Game1.textColor);
                    Utility.drawTextWithShadow(b, ShoeIndex.ToString(), Game1.smallFont, new Vector2(component.bounds.X + 16, component.bounds.Y + 32), Game1.textColor);
                }
                else if (component.name.Equals("Hair"))
                {
                    Utility.drawTextWithShadow(b, component.name, Game1.smallFont, new Vector2(component.bounds.X, component.bounds.Y), Game1.textColor);
                    Utility.drawTextWithShadow(b, Game1.player.hair.Value.ToString(), Game1.smallFont, new Vector2(component.bounds.X + 16, component.bounds.Y + 32), Game1.textColor);
                }
                else if (component.name.Equals("Skin"))
                {
                    Utility.drawTextWithShadow(b, component.name, Game1.smallFont, new Vector2(component.bounds.X, component.bounds.Y), Game1.textColor);
                    Utility.drawTextWithShadow(b, Game1.player.skin.Value.ToString(), Game1.smallFont, new Vector2(component.bounds.X + 16, component.bounds.Y + 32), Game1.textColor);
                }
                else if (component.name.Equals("Acc."))
                {
                    Utility.drawTextWithShadow(b, component.name, Game1.smallFont, new Vector2(component.bounds.X, component.bounds.Y), Game1.textColor);
                    Utility.drawTextWithShadow(b, Game1.player.accessory.Value == -1 ? "na" : Game1.player.accessory.Value.ToString(), Game1.smallFont, new Vector2(component.bounds.X + 16, component.bounds.Y + 32), Game1.textColor);
                }
                else if (component.name.Equals("Eye Color:") || component.name.Equals("Hair Color:"))
                {
                    Utility.drawTextWithShadow(b, component.name, Game1.smallFont, new Vector2(component.bounds.X, component.bounds.Y), Game1.textColor);
                }
                else if (component.name.Equals("Dresser") && ShouldDrawDresserButtons)
                {
                    Utility.drawTextWithShadow(b, component.name, Game1.smallFont, new Vector2(component.bounds.X, component.bounds.Y), Game1.textColor);
                }
            }

            //Draw the gender buttons
            foreach (ClickableTextureComponent component in GenderButtons)
            {
                component.draw(b);
                if (component.name.Equals("Male") && Game1.player.isMale || (component.name.Equals("Female") && !Game1.player.isMale))
                    b.Draw(Game1.mouseCursors, component.bounds, new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 34, -1, -1)), Color.White);
            }

            //Draw the ok button and cancel button
            OkButton.draw(b);
            CancelButton.draw(b);

            //Lastly, draw the mouse if they're not using the hardware cursor
            if (Game1.activeClickableMenu == this && !Game1.options.hardwareCursor)
                base.drawMouse(b);
        }
    }
}
