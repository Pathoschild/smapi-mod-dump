/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/barteke22/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using GenericModConfigMenu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace StardewMods
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        ITranslationHelper translate;
        private ModConfig config;
        private readonly PerScreen<Overlay> overlay = new PerScreen<Overlay>();



        public override void Entry(IModHelper helper)
        {
            config = helper.ReadConfig<ModConfig>();
            translate = helper.Translation;

            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
            helper.Events.Display.RenderedHud += this.RenderedHud;
            helper.Events.Display.MenuChanged += this.OnMenuChanged;
            helper.Events.Display.RenderedActiveMenu += this.OnRenderMenu;
            helper.Events.GameLoop.GameLaunched += this.GenericModConfigMenuIntegration;
            helper.Events.Multiplayer.ModMessageReceived += this.OnModMessageReceived;
        }


        private void GenericModConfigMenuIntegration(object sender, GameLaunchedEventArgs e)     //Generic Mod Config Menu API
        {
            if (Context.IsSplitScreen) return;
            translate = Helper.Translation;
            var GenericMC = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (GenericMC != null)
            {
                GenericMC.RegisterModConfig(ModManifest, () => config = new ModConfig(), () => Helper.WriteConfig(config));
                GenericMC.SetDefaultIngameOptinValue(ModManifest, true);
                GenericMC.RegisterLabel(ModManifest, translate.Get("GenericMC.barLabel"), ""); //All of these strings are stored in the traslation files.
                GenericMC.RegisterParagraph(ModManifest, translate.Get("GenericMC.barDescription"));
                GenericMC.RegisterParagraph(ModManifest, translate.Get("GenericMC.barDescription2"));
                if (Constants.TargetPlatform != GamePlatform.Android)
                {
                    GenericMC.RegisterParagraph(ModManifest, translate.Get("GenericMC.barDescriptionPC"));
                    GenericMC.RegisterParagraph(ModManifest, translate.Get("GenericMC.barDescriptionPC2"));
                }
                else GenericMC.RegisterParagraph(ModManifest, translate.Get("GenericMC.barDescriptionOther"));

                try
                {
                    GenericMCPerScreen(GenericMC, 0);
                    for (int i = 2; i < 5; i++)
                    {
                        GenericMC.RegisterPageLabel(ModManifest, translate.Get("GenericMC.SplitScreen" + i), translate.Get("GenericMC.SplitScreenDesc"), translate.Get("GenericMC.SplitScreen" + i));
                    }
                    GenericMCPerScreen(GenericMC, 1);
                    GenericMCPerScreen(GenericMC, 2);
                    GenericMCPerScreen(GenericMC, 3);
                }
                catch (Exception)
                {
                    this.Monitor.Log("Error parsing config data. Please either fix your config.json, or delete it to generate a new one.", LogLevel.Error);
                }
            }
        }
        private void GenericMCPerScreen(IGenericModConfigMenuApi GenericMC, int screen)
        {
            if (screen > 0)//make new page
            {
                GenericMC.StartNewPage(ModManifest, translate.Get("GenericMC.SplitScreen" + (screen + 1)));
            }
            GenericMC.RegisterChoiceOption(ModManifest, translate.Get("GenericMC.barIconMode"), translate.Get("GenericMC.barIconModeDesc"),
                    () => (config.BarIconMode[screen] == 0) ? translate.Get("GenericMC.barIconModeHor") : (config.BarIconMode[screen] == 1) ? translate.Get("GenericMC.barIconModeVert") : (config.BarIconMode[screen] == 2) ? translate.Get("GenericMC.barIconModeVertText") : translate.Get("GenericMC.Disabled"),
                    (string val) => config.BarIconMode[screen] = Int32.Parse((val.Equals(translate.Get("GenericMC.barIconModeHor"), StringComparison.Ordinal)) ? "0" : (val.Equals(translate.Get("GenericMC.barIconModeVert"), StringComparison.Ordinal)) ? "1" : (!val.Equals(translate.Get("GenericMC.Disabled"), StringComparison.Ordinal)) ? "2" : "3"),
                    new string[] { translate.Get("GenericMC.barIconModeHor"), translate.Get("GenericMC.barIconModeVert"), translate.Get("GenericMC.barIconModeVertText"), translate.Get("GenericMC.Disabled") });//small 'hack' so options appear as name strings, while config.json stores them as integers

            GenericMC.RegisterSimpleOption(ModManifest, translate.Get("GenericMC.barPosX"), translate.Get("GenericMC.barPosXDesc"),
                 () => config.BarTopLeftLocationX[screen], (int val) => config.BarTopLeftLocationX[screen] = Math.Max(0, val));
            GenericMC.RegisterSimpleOption(ModManifest, translate.Get("GenericMC.barPosY"), translate.Get("GenericMC.barPosYDesc"),
                () => config.BarTopLeftLocationY[screen], (int val) => config.BarTopLeftLocationY[screen] = Math.Max(0, val));
            GenericMC.RegisterSimpleOption(ModManifest, translate.Get("GenericMC.barScale"), translate.Get("GenericMC.barScaleDesc"),
                () => (float)config.BarScale[screen], (float val) => config.BarScale[screen] = Math.Min(10, Math.Max(0.1f, val)));
            GenericMC.RegisterSimpleOption(ModManifest, translate.Get("GenericMC.barMaxIcons"), translate.Get("GenericMC.barMaxIconsDesc"),
               () => config.BarMaxIcons[screen], (int val) => config.BarMaxIcons[screen] = (int)Math.Min(500, Math.Max(4, val)));
            GenericMC.RegisterSimpleOption(ModManifest, translate.Get("GenericMC.barMaxIconsPerRow"), translate.Get("GenericMC.barMaxIconsPerRowDesc"),
                () => config.BarMaxIconsPerRow[screen], (int val) => config.BarMaxIconsPerRow[screen] = (int)Math.Min(500, Math.Max(4, val)));

            GenericMC.RegisterChoiceOption(ModManifest, translate.Get("GenericMC.barBackgroundMode"), translate.Get("GenericMC.barBackgroundModeDesc"),
                () => (config.BarBackgroundMode[screen] == 0) ? translate.Get("GenericMC.barBackgroundModeCircles") : (config.BarBackgroundMode[screen] == 1) ? translate.Get("GenericMC.barBackgroundModeRect") : translate.Get("GenericMC.Disabled"),
                (string val) => config.BarBackgroundMode[screen] = Int32.Parse((val.Equals(translate.Get("GenericMC.barBackgroundModeCircles"), StringComparison.Ordinal)) ? "0" : (val.Equals(translate.Get("GenericMC.barBackgroundModeRect"), StringComparison.Ordinal)) ? "1" : "2"),
                new string[] { translate.Get("GenericMC.barBackgroundModeCircles"), translate.Get("GenericMC.barBackgroundModeRect"), translate.Get("GenericMC.Disabled") });

            GenericMC.RegisterSimpleOption(ModManifest, translate.Get("GenericMC.barShowBaitTackle"), translate.Get("GenericMC.barShowBaitTackleDesc"),
                () => config.BarShowBaitAndTackleInfo[screen], (bool val) => config.BarShowBaitAndTackleInfo[screen] = val);
            GenericMC.RegisterSimpleOption(ModManifest, translate.Get("GenericMC.barShowPercentages"), translate.Get("GenericMC.barShowPercentagesDesc"),
                () => config.BarShowPercentages[screen], (bool val) => config.BarShowPercentages[screen] = val);

            GenericMC.RegisterChoiceOption(ModManifest, translate.Get("GenericMC.barSortMode"), translate.Get("GenericMC.barSortModeDesc"),
                () => (config.BarSortMode[screen] == 0) ? translate.Get("GenericMC.barSortModeName") : (config.BarSortMode[screen] == 1) ? translate.Get("GenericMC.barSortModeChance") : translate.Get("GenericMC.Disabled"),
                (string val) => config.BarSortMode[screen] = Int32.Parse((val.Equals(translate.Get("GenericMC.barSortModeName"), StringComparison.Ordinal)) ? "0" : (val.Equals(translate.Get("GenericMC.barSortModeChance"), StringComparison.Ordinal)) ? "1" : "2"),
                new string[] { translate.Get("GenericMC.barSortModeName"), translate.Get("GenericMC.barSortModeChance"), translate.Get("GenericMC.Disabled") });

            GenericMC.RegisterClampedOption(ModManifest, translate.Get("GenericMC.barScanRadius"), translate.Get("GenericMC.barScanRadiusDesc"),
                () => config.BarScanRadius[screen], (int val) => config.BarScanRadius[screen] = val, 1, 60);
            GenericMC.RegisterSimpleOption(ModManifest, translate.Get("GenericMC.barCrabPotEnabled"), translate.Get("GenericMC.barCrabPotEnabledDesc"),
                () => config.BarCrabPotEnabled[screen], (bool val) => config.BarCrabPotEnabled[screen] = val);
            GenericMC.RegisterSimpleOption(ModManifest, translate.Get("GenericMC.barUncaughtDarker"), translate.Get("GenericMC.barUncaughtDarkerDesc"),
                () => config.UncaughtFishAreDark[screen], (bool val) => config.UncaughtFishAreDark[screen] = val);
            GenericMC.RegisterSimpleOption(ModManifest, translate.Get("GenericMC.barOnlyFish"), translate.Get("GenericMC.barOnlyFishDesc"),
                () => config.OnlyFish[screen], (bool val) => config.OnlyFish[screen] = val);

            if (screen == 0)//only page 0
            {
                GenericMC.RegisterClampedOption(ModManifest, translate.Get("GenericMC.barExtraCheckFrequency"), translate.Get("GenericMC.barExtraCheckFrequencyDesc"),
                    () => config.BarExtraCheckFrequency, (int val) => config.BarExtraCheckFrequency = val, 20, 220);

                GenericMC.RegisterLabel(ModManifest, translate.Get("GenericMC.MinigameLabel"), "");
                GenericMC.RegisterParagraph(ModManifest, translate.Get("GenericMC.MinigameDescription"));
                GenericMC.RegisterParagraph(ModManifest, translate.Get("GenericMC.MinigameDescription2"));
            }
            GenericMC.RegisterChoiceOption(ModManifest, translate.Get("GenericMC.MinigameMode"), translate.Get("GenericMC.MinigameModeDesc"),
                () => (config.MinigamePreviewMode[screen] == 0) ? translate.Get("GenericMC.MinigameModeFull") : (config.MinigamePreviewMode[screen] == 1) ? translate.Get("GenericMC.MinigameModeSimple") : (config.MinigamePreviewMode[screen] == 2) ? translate.Get("GenericMC.MinigameModeBarOnly") : translate.Get("GenericMC.Disabled"),
                (string val) => config.MinigamePreviewMode[screen] = Int32.Parse((val.Equals(translate.Get("GenericMC.MinigameModeFull"), StringComparison.Ordinal)) ? "0" : (val.Equals(translate.Get("GenericMC.MinigameModeSimple"), StringComparison.Ordinal)) ? "1" : (val.Equals(translate.Get("GenericMC.MinigameModeBarOnly"), StringComparison.Ordinal)) ? "2" : "3"),
                new string[] { translate.Get("GenericMC.MinigameModeFull"), translate.Get("GenericMC.MinigameModeSimple"), translate.Get("GenericMC.MinigameModeBarOnly"), translate.Get("GenericMC.Disabled") });
        }





        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady || !(e.Button == SButton.F5)) return; // ignore if player hasn't loaded a save yet
            config = Helper.ReadConfig<ModConfig>();
            translate = Helper.Translation;
            UpdateConfig();
        }


        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            UpdateConfig();
        }

        private void RenderedHud(object sender, RenderedHudEventArgs e)
        {
            if (overlay.Value == null) overlay.Value = new Overlay(this);
            if (Context.IsWorldReady) overlay.Value.RenderedHud(sender, e);
        }

        private void OnMenuChanged(object sender, MenuChangedEventArgs e)   //Minigame data
        {
            if (overlay.Value == null) overlay.Value = new Overlay(this);
            if (Context.IsWorldReady) overlay.Value.OnMenuChanged(sender, e);
        }
        private void OnRenderMenu(object sender, RenderedActiveMenuEventArgs e)
        {
            if (Context.IsWorldReady) overlay.Value.OnRenderMenu(sender, e);
        }

        private void OnModMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            if (Context.IsWorldReady) overlay.Value.OnModMessageReceived(sender, e);
        }


        private void UpdateConfig()
        {
            for (int i = 0; i < 4; i++)
            {
                Overlay.backgroundMode[i] = config.BarBackgroundMode[i];                                                    //config: 0=Circles (dynamic), 1=Rectangle (single), 2=Off
                Overlay.barCrabEnabled[i] = config.BarCrabPotEnabled[i];                                                    //config: If bait/tackle/bait preview is enabled when holding a fishing rod
                Overlay.barPosition[i] = new Vector2(config.BarTopLeftLocationX[i] + 2, config.BarTopLeftLocationY[i] + 2); //config: Position of bar
                Overlay.barScale[i] = config.BarScale[i];                                                                   //config: Custom scale for the location bar.
                Overlay.iconMode[i] = config.BarIconMode[i];                                                                //config: 0=Horizontal Icons, 1=Vertical Icons, 2=Vertical Icons + Text, 3=Off
                Overlay.maxIcons[i] = config.BarMaxIcons[i];                                                                //config: ^Max amount of tackle + trash + fish icons
                Overlay.maxIconsPerRow[i] = config.BarMaxIconsPerRow[i];                                                    //config: ^How many per row/column.
                Overlay.onlyFish[i] = config.OnlyFish[i];                                                                   //config: Whether to hide things like furniture.
                Overlay.miniMode[i] = config.MinigamePreviewMode[i];                                                        //config: Fish preview in minigame: 0=Full, 1=Simple, 2=BarOnly, 3=Off
                Overlay.scanRadius[i] = config.BarScanRadius[i];                                                            //config: 0: Only checks if can fish, 1-50: also checks if there's water within X tiles around player.
                Overlay.showPercentages[i] = config.BarShowPercentages[i];                                                  //config: Whether it should show catch percentages.
                Overlay.showTackles[i] = config.BarShowBaitAndTackleInfo[i];                                                //config: Whether it should show Bait and Tackle info.
                Overlay.sortMode[i] = config.BarSortMode[i];                                                                //config: 0= By Name (text mode only), 1= By Percentage, 2=Off
                Overlay.uncaughtDark[i] = config.UncaughtFishAreDark[i];                                                    //config: Whether uncaught fish are displayed as ??? and use dark icons
            }
            Overlay.extraCheckFrequency = config.BarExtraCheckFrequency;                                                    //config: 20-220: Bad performance dynamic check to see if there's modded/hardcoded fish

            Overlay.locationData = Game1.content.Load<Dictionary<string, string>>("Data\\Locations");       //gets location data (which fish are here)
            Overlay.fishData = Game1.content.Load<Dictionary<int, string>>("Data\\Fish");                   //gets fish data
            Overlay.background[0] = WhiteCircle(17, 30);
            Overlay.background[1] = WhitePixel();

            //locationData = Helper.Content.Load<Dictionary<string, string>>("Data\\Locations", ContentSource.GameContent);
            //fishData = Helper.Content.Load<Dictionary<int, string>>("Data\\Fish", ContentSource.GameContent);

            overlay.ResetAllScreens();
        }


        private Texture2D WhitePixel() //returns a single pixel texture that can be recoloured and resized to make up a background
        {
            Texture2D whitePixel = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
            whitePixel.SetData(new[] { Color.White });
            return whitePixel;
        }
        private Texture2D WhiteCircle(int width, int thickness) //returns a circle texture that can be recoloured and resized to make up a background. Width works better with Odd Numbers.
        {
            Texture2D whitePixel = new Texture2D(Game1.graphics.GraphicsDevice, width, width);

            Color[] data = new Color[width * width];

            float radiusSquared = (width / 2) * (width / 2);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < width; y++)
                {
                    float dx = x - (width / 2);
                    float dy = y - (width / 2);
                    float distanceSquared = dx * dx + dy * dy;

                    if (distanceSquared <= radiusSquared + thickness)
                    {
                        data[(x + y * width)] = Color.White;
                    }
                }
            }

            whitePixel.SetData(data);
            return whitePixel;
        }
    }
}
