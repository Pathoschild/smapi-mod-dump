using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardustCore.UIUtilities;
using StardustCore.UIUtilities.MenuComponents;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vocalization.Framework.Menus
{

    /// <summary>
    /// TODO:
    /// Make Ok button that saves settings and closes menu instead  of readyToCloseFunction()
    /// Make Cyclic buttons(aka a button that holds a ton of buttons)
    /// Make cyclic translation button. (english, spanish, etc)
    /// Make cyclic audio modes button. (full, simple, etc)
    /// </summary>
    public class VocalizationMenu: IClickableMenuExtended
    {
        public SliderButton sliderButton;

        public CycleButton languages;

        public VocalizationMenu(int xPos, int yPos, int width, int height, bool showCloseButton = false) : base(xPos, yPos, width, height,showCloseButton)
        {
            this.xPositionOnScreen = xPos;
            this.yPositionOnScreen = yPos;
            this.width = width;
            this.height = height;
            this.showRightCloseButton = showCloseButton;
            setUpButtons();

        }

        public void setUpButtons()
        {
            Texture2DExtended buttonTexture = new Texture2DExtended(Vocalization.ModHelper, Vocalization.Manifest, Path.Combine("Content", "Graphics", "SliderButton.png"));
            Button bar = new Button(new Rectangle(this.xPositionOnScreen + 100, this.yPositionOnScreen + 220, 200, 40), new Texture2DExtended(Vocalization.ModHelper, Vocalization.Manifest, Path.Combine("Content", "Graphics", "SliderBar.png")),new Rectangle(0,0,100,10),2f);
            //Texture2DExtended barTexture = new Texture2DExtended(Vocalization.ModHelper, Vocalization.Manifest, Path.Combine("Content", "Graphics", "SliderBar.png"));
            Rectangle sourceRect = new Rectangle(0, 0, 4, 16);
            this.sliderButton = new SliderButton("Slider", "Volume", new Rectangle(this.xPositionOnScreen+100, this.yPositionOnScreen+220, 4, 16), buttonTexture, bar, sourceRect, 2f, new SliderInformation(SliderStyle.Horizontal, (int)(Vocalization.config.voiceVolume*100), 1), new StardustCore.Animations.Animation(sourceRect), Color.White, Color.Black, new StardustCore.UIUtilities.MenuComponents.Delegates.Functionality.ButtonFunctionality(null, null, null), false, null, true);

            Button English = new Button("EnglishButton", "English", new Rectangle(0, 0, 174, 39),new Texture2DExtended(Vocalization.ModHelper,Vocalization.Manifest,Path.Combine("LooseSprites","LanguageButtons.xnb"),StardewModdingAPI.ContentSource.GameContent),new Rectangle(0,0,174,39), 1f);
            Button Spanish = new Button("SpanishButton", "Spanish", new Rectangle(0, 0, 174, 39), new Texture2DExtended(Vocalization.ModHelper, Vocalization.Manifest, Path.Combine("LooseSprites", "LanguageButtons.xnb"),StardewModdingAPI.ContentSource.GameContent), new Rectangle(0, 39*2, 174, 39), 1f);
            Button Portuguese = new Button("PortugueseButton", "Brazillian Portuguese", new Rectangle(0, 0, 174, 39), new Texture2DExtended(Vocalization.ModHelper, Vocalization.Manifest, Path.Combine("LooseSprites", "LanguageButtons.xnb"), StardewModdingAPI.ContentSource.GameContent), new Rectangle(0, 39*4, 174, 39), 1f);
            Button Russian = new Button("RussianButton", "Russian", new Rectangle(0, 0, 174, 39), new Texture2DExtended(Vocalization.ModHelper, Vocalization.Manifest, Path.Combine("LooseSprites", "LanguageButtons.xnb"), StardewModdingAPI.ContentSource.GameContent), new Rectangle(0, 39*6, 174, 39), 1f);
            Button Chinese = new Button("ChineseButton", "Chinese", new Rectangle(0, 0, 174, 39), new Texture2DExtended(Vocalization.ModHelper, Vocalization.Manifest, Path.Combine("LooseSprites", "LanguageButtons.xnb"), StardewModdingAPI.ContentSource.GameContent), new Rectangle(0, 39*8, 174, 39), 1f);
            Button Japanese = new Button("JapaneseButton", "Japanese", new Rectangle(0, 0, 174, 39), new Texture2DExtended(Vocalization.ModHelper, Vocalization.Manifest, Path.Combine("LooseSprites", "LanguageButtons.xnb"), StardewModdingAPI.ContentSource.GameContent), new Rectangle(0, 39*10, 174, 39), 1f);
            Button German = new Button("GermanButton", "German", new Rectangle(0, 0, 174, 39), new Texture2DExtended(Vocalization.ModHelper, Vocalization.Manifest, Path.Combine("LooseSprites", "LanguageButtons.xnb"), StardewModdingAPI.ContentSource.GameContent), new Rectangle(0, 39*12, 174, 39), 1f);
            List<Button> buttons = new List<Button>();
            buttons.Add(English);
            buttons.Add(Spanish);
            buttons.Add(Portuguese);
            buttons.Add(Russian);
            buttons.Add(Chinese);
            buttons.Add(Japanese);
            buttons.Add(German);

            languages = new CycleButton(new Rectangle(this.xPositionOnScreen + 100, this.yPositionOnScreen + 75, 174, 39),buttons,new Rectangle(0,0,174,39),1f);

            for(int i=0; i < languages.buttons.Count; i++)
            {
                if (Vocalization.config.translationInfo.currentTranslation == languages.buttons.ElementAt(i).label)
                {

                    languages.buttonIndex = i;
                }
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
            this.drawOnlyDialogueBoxBackground(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, new Color(255, 255, 255, 255),0.4f);
            sliderButton.draw(b,Color.White,Vector2.Zero,0.5f);
            languages.draw(b, Color.White, Vector2.Zero, 0.5f);
        }

        /// <summary>
        /// Save the menu information upon menu being closed.
        /// </summary>
        /// <returns></returns>
        public override bool readyToClose()
        {
            decimal xPos = sliderButton.sliderInformation.xPos;
            Vocalization.config.voiceVolume = (decimal)(xPos/ 100.0M);
            Vocalization.ModHelper.WriteConfig<ModConfig>(Vocalization.config);
            Vocalization.soundManager.volume =(float) Vocalization.config.voiceVolume;

            if (Vocalization.config.translationInfo.currentTranslation != getTranslationInfo())
            {
                Vocalization.config.translationInfo.currentTranslation = getTranslationInfo();
                Vocalization.soundManager.sounds.Clear();
                Vocalization.DialogueCues.Clear();
                Vocalization.loadAllVoiceFiles();
            }

            if (Vocalization.config.currentMode != getAudioMode())
            {
                Vocalization.config.currentMode = getAudioMode();
            }


            return true;
        }

        public string getTranslationInfo()
        {
            //Return the name of the button which will have the translation stuff here!
            return languages.getCurrentButtonLabel();
        }

        public string getAudioMode()
        {
            //Return the name of the mode that the current mode button is selected on.
            return "Full";
        }


    }
}
