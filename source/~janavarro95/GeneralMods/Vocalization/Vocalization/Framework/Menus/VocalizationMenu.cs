using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardustCore.UIUtilities;
using StardustCore.UIUtilities.MenuComponents;

namespace Vocalization.Framework.Menus
{
    // TODO:
    // Make Ok button that saves settings and closes menu instead  of readyToCloseFunction()
    // Make Cyclic buttons(aka a button that holds a ton of buttons)
    // Make cyclic translation button. (english, spanish, etc)
    // Make cyclic audio modes button. (full, simple, etc)
    public class VocalizationMenu : IClickableMenuExtended
    {
        public SliderButton sliderButton;

        public CycleButton languages;

        public VocalizationMenu(int xPos, int yPos, int width, int height, bool showCloseButton = false)
            : base(xPos, yPos, width, height, showCloseButton)
        {
            this.xPositionOnScreen = xPos;
            this.yPositionOnScreen = yPos;
            this.width = width;
            this.height = height;
            this.showRightCloseButton = showCloseButton;
            this.setUpButtons();

        }

        public void setUpButtons()
        {
            Texture2DExtended buttonTexture = new Texture2DExtended(Vocalization.ModHelper, Path.Combine("Content", "Graphics", "SliderButton.png"));
            Button bar = new Button(new Rectangle(this.xPositionOnScreen + 100, this.yPositionOnScreen + 220, 200, 40), new Texture2DExtended(Vocalization.ModHelper, Path.Combine("Content", "Graphics", "SliderBar.png")), new Rectangle(0, 0, 100, 10), 2f);
            //Texture2DExtended barTexture = new Texture2DExtended(Vocalization.ModHelper, Vocalization.Manifest, Path.Combine("Content", "Graphics", "SliderBar.png"));
            Rectangle sourceRect = new Rectangle(0, 0, 4, 16);
            this.sliderButton = new SliderButton("Slider", "Volume", new Rectangle(this.xPositionOnScreen + 100, this.yPositionOnScreen + 220, 4, 16), buttonTexture, bar, sourceRect, 2f, new SliderInformation(SliderStyle.Horizontal, (int)(Vocalization.config.voiceVolume * 100), 1), new StardustCore.Animations.Animation(sourceRect), Color.White, Color.Black, new StardustCore.UIUtilities.MenuComponents.Delegates.Functionality.ButtonFunctionality(null, null, null), false, null, true);

            Button english = new Button(LanguageName.English.ToString(), "English", new Rectangle(0, 0, 174, 39), new Texture2DExtended(Vocalization.ModHelper, Path.Combine("LooseSprites", "LanguageButtons.xnb"), StardewModdingAPI.ContentSource.GameContent), new Rectangle(0, 0, 174, 39), 1f);
            Button spanish = new Button(LanguageName.Spanish.ToString(), "Spanish", new Rectangle(0, 0, 174, 39), new Texture2DExtended(Vocalization.ModHelper, Path.Combine("LooseSprites", "LanguageButtons.xnb"), StardewModdingAPI.ContentSource.GameContent), new Rectangle(0, 39 * 2, 174, 39), 1f);
            Button portuguese = new Button(LanguageName.Portuguese.ToString(), "Brazillian Portuguese", new Rectangle(0, 0, 174, 39), new Texture2DExtended(Vocalization.ModHelper, Path.Combine("LooseSprites", "LanguageButtons.xnb"), StardewModdingAPI.ContentSource.GameContent), new Rectangle(0, 39 * 4, 174, 39), 1f);
            Button russian = new Button(LanguageName.Russian.ToString(), "Russian", new Rectangle(0, 0, 174, 39), new Texture2DExtended(Vocalization.ModHelper, Path.Combine("LooseSprites", "LanguageButtons.xnb"), StardewModdingAPI.ContentSource.GameContent), new Rectangle(0, 39 * 6, 174, 39), 1f);
            Button chinese = new Button(LanguageName.Chinese.ToString(), "Chinese", new Rectangle(0, 0, 174, 39), new Texture2DExtended(Vocalization.ModHelper, Path.Combine("LooseSprites", "LanguageButtons.xnb"), StardewModdingAPI.ContentSource.GameContent), new Rectangle(0, 39 * 8, 174, 39), 1f);
            Button japanese = new Button(LanguageName.Japanese.ToString(), "Japanese", new Rectangle(0, 0, 174, 39), new Texture2DExtended(Vocalization.ModHelper, Path.Combine("LooseSprites", "LanguageButtons.xnb"), StardewModdingAPI.ContentSource.GameContent), new Rectangle(0, 39 * 10, 174, 39), 1f);
            Button german = new Button(LanguageName.German.ToString(), "German", new Rectangle(0, 0, 174, 39), new Texture2DExtended(Vocalization.ModHelper, Path.Combine("LooseSprites", "LanguageButtons.xnb"), StardewModdingAPI.ContentSource.GameContent), new Rectangle(0, 39 * 12, 174, 39), 1f);
            List<Button> buttons = new List<Button>
            {
                english,
                spanish,
                portuguese,
                russian,
                chinese,
                japanese,
                german
            };

            this.languages = new CycleButton(new Rectangle(this.xPositionOnScreen + 100, this.yPositionOnScreen + 75, 174, 39), buttons, new Rectangle(0, 0, 174, 39), 1f);

            for (int i = 0; i < this.languages.buttons.Count; i++)
            {
                if (Vocalization.config.translationInfo.CurrentTranslation == (LanguageName)Enum.Parse(typeof(LanguageName), this.languages.buttons.ElementAt(i).name))
                    this.languages.buttonIndex = i;
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            this.sliderButton.onLeftClick(x, y);
            this.languages.onLeftClick(x, y);
            base.receiveLeftClick(x, y, playSound);
        }

        public override void leftClickHeld(int x, int y)
        {
            this.sliderButton.onLeftClickHeld(x, y);
            base.leftClickHeld(x, y);
        }

        public override IClickableMenuExtended clone()
        {
            return new VocalizationMenu(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, this.showRightCloseButton);
        }

        public override void draw(SpriteBatch b)
        {
            this.drawOnlyDialogueBoxBackground(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, new Color(255, 255, 255, 255), 0.4f);
            this.sliderButton.draw(b, Color.White, Vector2.Zero, 0.5f);
            this.languages.draw(b, Color.White, Vector2.Zero, 0.5f);
        }

        /// <summary>Save the menu information upon menu being closed.</summary>
        public override bool readyToClose()
        {
            decimal xPos = this.sliderButton.sliderInformation.xPos;
            Vocalization.config.voiceVolume = (decimal)(xPos / 100.0M);
            Vocalization.ModHelper.WriteConfig<ModConfig>(Vocalization.config);
            Vocalization.soundManager.volume = (float)Vocalization.config.voiceVolume;

            if (Vocalization.config.translationInfo.CurrentTranslation != this.getTranslationInfo())
            {
                Vocalization.config.translationInfo.CurrentTranslation = this.getTranslationInfo();
                Vocalization.soundManager.sounds.Clear();
                Vocalization.DialogueCues.Clear();
                Vocalization.loadAllVoiceFiles();
            }

            Vocalization.config.currentMode = this.getAudioMode();
            
            return true;
        }

        public LanguageName getTranslationInfo()
        {
            //Return the name of the button which will have the translation stuff here!
            string currentName = this.languages.getCurrentButtonName();
            return (LanguageName)Enum.Parse(typeof(LanguageName), currentName);
        }

        public string getAudioMode()
        {
            //Return the name of the mode that the current mode button is selected on.
            return "Full";
        }
    }
}
