using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using StardustCore.Menus;
using StardustCore.UIUtilities;
using StardustCore.UIUtilities.MenuComponents;
using Vocalization.Framework;
using Vocalization.Framework.Menus;

namespace Vocalization
{
    /*
     * Things to sanitize/load in
     * 
     * NPC Dialogue(sanitized, not loaded);
     *  -Characters/Dialogue/CharacterName
     * Rainy Dialogue(sanitized, not loaded);
     *  -Characters/Dialogue/rainy.yaml
     * Marriage dialogue(sanitized?,not loaded);
     *  -Characters/Dialogue/MarriageDialogue<NPC NAME>
     *  -Characters/Dialogue/MarriageDialogue.yaml
     * Engagement dialogue(sanitized, not loaded);
     *  -Data/EngagementDialogue.yaml
     * Misc
     *  -Strings/StringsFromCSFiles.yaml
     * 
     * TV shows
     *  -cooking (sanitized, not loaded)
     *      -Data/TV/CookingChannel.yaml
     *  -interview(sanitized, not loaded)
     *      -Data/TV/InterviewShow.yaml
     *  -tip(sanitized, not loaded)
     *      -Data/TV/TipChannel.yaml
     *  -oracle(sanitized, not loaded)
     *      -Strings/StringsFromCSFiles.yaml
     *  -weather(sanitized, not loaded)
     *      -Strings/StringsFromCSFiles.yaml
     * 
     * 
     * Shops(sanitized, not loaded);
     *  -Strings/StringsFromCSFiles.yaml
     *  
     * Extra dialogue(sanitized, not loaded);
     *  -Data/ExtraDialogue.yaml
     * 
     * Letters(sanitized, not loaded);
     *  -Data/mail.yaml
     * 
     * Events(sanitized, not loaded); 
     *  -Strings/StringsFromCSFiles.yaml
     *  -Strings/Events.yaml
     * 
     * Characters:
     *  -Strings/Characters.yaml (sanitized, not loaded);
     *  
     * Strings/Events.yaml (sanitized, not loaded);
     *  -Strings/StringsFromCSFiles.yaml
     *  
     * Strings/Locations.yaml(sanitized, not loaded);
     *  -Strings/Loctions.yaml
     *  -Strings/StringsFromMaps.yaml
     * 
     * Strings/Notes.yaml(sanitized, not loaded);
     *  -Strings/Notes.yaml
     *  -Data/SecretNotes.yaml
     * 
     * Strings/Objects.yaml (not needed);
     * 
     * Utility
     *  -Strings/StringsFromCS.yaml
     *  
     *  Quests (done)
     *  -Strings/Quests
     */

    /// <summary>
    /// TODO:
    /// 
    /// Validate that all paths are loading from proper places.
    /// 
    /// Make a directory where all of the wav files will be stored. (Done?)
    /// Load in said wav files.(Done?)
    /// 
    /// Find way to add in supported dialogue via some sort of file system. (Done?)
    ///     -Make each character folder have a .json that has....
    ///         -Character Name(Done?)
    ///         -Dictionary of supported dialogue lines and values for .wav files. (Done?)
    ///         -*Note* The value for the dialogue dictionaries is the name of the file excluding the .wav extension.
    /// 
    /// Find way to play said wave files. (Done?)
    /// 
    /// Sanitize input to remove variables such as pet names, farm names, farmer name. (done)
    /// 
    /// Loop through common variables and add them to the dialogue list inside of ReplacementString.cs (done)
    ///     -ERRR that might not be fun to do.......
    ///     Dialogue.cs
    ///     adj is 679-698 (done)
    ///     noun is 699-721 (done)
    ///     verb is 722-734 ???? Not needed???
    ///     place is 735-759 (done)
    ///     colors is 795-810. What does it change though??????
    /// 
    /// Add in dialogue for npcs into their respective VoiceCue.json files. (done? Can be improved on)
    /// 
    /// 
    ///Add in sanitization for Dialogue Commands(see the wiki) (done)
    /// 
    /// 
    ///Add support for different kinds of menus. TV, shops, etc. (Done)
    ///     -All of these strings are stored in StringsFromCS and TV/file.yaml
    /// 
    ///Add support for MarriageDialogue strings. (Done)
    ///Add support for EngagementDialogue strings.(Done)
    ///Add support for ExtraDialogue.yaml file (Done)
    /// 
    /// 
    ///Add support for mail dialogue(Done)
    ///         -split using ^ to get the sender's name as the last element in the split list. Then sanitize the % information out by splitting across % and getting the first element.
    /// 
    /// 
    ///Add support for Extra dialogue via StringsFromCSFiles(Done)
    ///     -tv
    ///     -events
    ///     -NPC.cs
    ///     -Utility.cs
    /// 
    /// Make moddable to support other languages, portuguese, russian, etc (Needs testing)
    ///     -make mod config have a list of supported languages and a variable that is the currently selected language.
    ///     
    /// Remove text typing sound from game? (done) Just turn off the option for Game1.options.dialogueTyping.
    ///     
    /// Add support for adding dialogue lines when loading CharacterVoiceCue.json if the line doesn't already exist! (done)
    /// 
    /// 
    /// 
    /// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! All in Strings folder
    /// -Quests (done)
    /// -NPC Gift tastes (done)
    /// speech bubbles (done)
    /// -temp
    /// -ui (not needed???)
    /// /// 
    /// 
    /// </summary>
    public class Vocalization : Mod
    {
        public static IModHelper ModHelper;
        public static IMonitor ModMonitor;
        public static IManifest Manifest;

        /// <summary>
        /// A string that keeps track of the previous dialogue said to ensure that dialogue isn't constantly repeated while the text box is open.
        /// </summary>
        public static string previousDialogue;

        List<string> characterDialoguePaths = new List<string>();

        /// <summary>
        /// Simple Sound Manager class that handles playing and stoping dialogue.
        /// </summary>
        public static SimpleSoundManager.Framework.SoundManager soundManager;




        /// <summary>
        /// The path to the folder where all of the NPC folders for dialogue .wav files are kept.
        /// </summary>
        public static string VoicePath = "";

        public static ReplacementStrings replacementStrings;

        public static ModConfig config;


        public List<string> onScreenSpeechBubbleDialogues;

        /// <summary>
        /// A dictionary that keeps track of all of the npcs whom have voice acting for their dialogue.
        /// </summary>
        public static Dictionary<string, CharacterVoiceCue> DialogueCues;

        public override void Entry(IModHelper helper)
        {
            StardewModdingAPI.Events.SaveEvents.AfterLoad += SaveEvents_AfterLoad;
            StardewModdingAPI.Events.GameEvents.UpdateTick += GameEvents_UpdateTick;
            StardewModdingAPI.Events.MenuEvents.MenuClosed += MenuEvents_MenuClosed;
            StardewModdingAPI.Events.MenuEvents.MenuChanged += MenuEvents_MenuChanged;
            ModMonitor = Monitor;
            ModHelper = Helper;
            Manifest = ModManifest;
            DialogueCues = new Dictionary<string, CharacterVoiceCue>();
            replacementStrings = new ReplacementStrings();

            onScreenSpeechBubbleDialogues = new List<string>();

            previousDialogue = "";

            soundManager = new SimpleSoundManager.Framework.SoundManager();

            config = ModHelper.ReadConfig<ModConfig>();

            AudioCues.initialize();

            config.verifyValidMode(); //Make sure the current mode is valid.
            soundManager.volume = (float)config.voiceVolume; //Set the volume for voices.

        }

        private void MenuEvents_MenuChanged(object sender, StardewModdingAPI.Events.EventArgsClickableMenuChanged e)
        {
            if (Game1.activeClickableMenu.GetType() == typeof(ModularGameMenu))
            {
                npcPortraitHack();
            }
        }

        /// <summary>
        /// Runs whenever any onscreen menu is closed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuEvents_MenuClosed(object sender, StardewModdingAPI.Events.EventArgsClickableMenuClosed e)
        {
            //Clean out my previous dialogue when I close any sort of menu.
            try
            {
                soundManager.stopAllSounds();
                previousDialogue = "";
            }
            catch (Exception err)
            {
                previousDialogue = "";
            }
        }

        /// <summary>
        /// Runs after the game is loaded to initialize all of the mod's files.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveEvents_AfterLoad(object sender, EventArgs e)
        {


            initialzeModualGameMenuHack();
            initialzeDirectories();
            loadAllVoiceFiles();

            AudioCues.saveAudioCues();
        }

        /// <summary>
        /// Initializes the menu tab for Vocalization for the modular menu.
        /// </summary>
        public void initialzeModualGameMenuHack()
        {
            List<Texture2D> textures = new List<Texture2D>();
            foreach (GameLocation loc in Game1.locations)
            {
                foreach (NPC npc in loc.characters)
                {
                    if (npc.isVillager() == false) continue;
                    Texture2D text = npc.Sprite.Texture;
                    if (text == null) continue;
                    textures.Add(text);
                }
            }
            int randNum = Game1.random.Next(0, textures.Count);
            Texture2D myText = textures.ElementAt(randNum);
            ClickableTextureComponent c = new ClickableTextureComponent(new Rectangle(0, 16, 16, 24), myText, new Rectangle(0, 0, 16, 24), 2f, false);

            ClickableTextureComponent speech = new ClickableTextureComponent(new Rectangle(0, 0, 32, 32), ModHelper.Content.Load<Texture2D>(Path.Combine("Content", "Graphics", "SpeechBubble.png")), new Rectangle(0, 0, 32, 32), 2f, false);
            List<KeyValuePair<ClickableTextureComponent, ExtraTextureDrawOrder>> components = new List<KeyValuePair<ClickableTextureComponent, ExtraTextureDrawOrder>>();

            components.Add(new KeyValuePair<ClickableTextureComponent, ExtraTextureDrawOrder>(c, ExtraTextureDrawOrder.after));
            components.Add(new KeyValuePair<ClickableTextureComponent, ExtraTextureDrawOrder>(speech, ExtraTextureDrawOrder.after));

            Button menuTab = new Button("", new Rectangle(0, 0, 32, 32), new Texture2DExtended(ModHelper, ModManifest, Path.Combine("Content", "Graphics", "MenuTab.png")), "", new Rectangle(0, 0, 32, 32), 2f, new StardustCore.Animations.Animation(new Rectangle(0, 0, 32, 32)), Color.White, Color.White, new StardustCore.UIUtilities.MenuComponents.Delegates.Functionality.ButtonFunctionality(new StardustCore.UIUtilities.MenuComponents.Delegates.DelegatePairing(null, null), new StardustCore.UIUtilities.MenuComponents.Delegates.DelegatePairing(null, null), new StardustCore.UIUtilities.MenuComponents.Delegates.DelegatePairing(null, null)), false, components);

            //Change this to take the vocalization menu instead
            List<KeyValuePair<Button, IClickableMenuExtended>> modTabs = new List<KeyValuePair<Button, IClickableMenuExtended>>();
            modTabs.Add(new KeyValuePair<Button, IClickableMenuExtended>(menuTab, new VocalizationMenu(100, 64, 600, 300, true)));
            StardustCore.Menus.ModularGameMenu.AddTabsForMod(ModManifest, modTabs);

            ModMonitor.Log("VOCALIZATION MENU HACK COMPLETE!", LogLevel.Alert);
        }

        /// <summary>
        /// Randomize the npc below the speech bubble every time the modular game menu is drawn.
        /// </summary>
        public void npcPortraitHack()
        {
            List<KeyValuePair<Button, IClickableMenuExtended>> menuHacks = new List<KeyValuePair<Button, IClickableMenuExtended>>();

            List<Texture2D> textures = new List<Texture2D>();
            foreach (GameLocation loc in Game1.locations)
            {
                foreach (NPC npc in loc.characters)
                {
                    if (npc.isVillager() == false) continue;
                    Texture2D text = npc.Sprite.Texture;
                    textures.Add(text);
                }
            }
            int randNum = Game1.random.Next(0, textures.Count);
            Texture2D myText = textures.ElementAt(randNum);
            ClickableTextureComponent c = new ClickableTextureComponent(new Rectangle(0, 16, 16, 24), myText, new Rectangle(0, 0, 16, 24), 2f, false);
            List<KeyValuePair<ClickableTextureComponent, ExtraTextureDrawOrder>> components = new List<KeyValuePair<ClickableTextureComponent, ExtraTextureDrawOrder>>();


            ClickableTextureComponent speech = new ClickableTextureComponent(new Rectangle(0, 0, 32, 32), ModHelper.Content.Load<Texture2D>(Path.Combine("Content", "Graphics", "SpeechBubble.png")), new Rectangle(0, 0, 32, 32), 2f, false);

            components.Add(new KeyValuePair<ClickableTextureComponent, ExtraTextureDrawOrder>(c, ExtraTextureDrawOrder.after));
            components.Add(new KeyValuePair<ClickableTextureComponent, ExtraTextureDrawOrder>(speech, ExtraTextureDrawOrder.after));

            Button menuTab = new Button("", new Rectangle(0, 0, 32, 32), new Texture2DExtended(ModHelper, ModManifest, Path.Combine("Content", "Graphics", "MenuTab.png")), "", new Rectangle(0, 0, 32, 32), 2f, new StardustCore.Animations.Animation(new Rectangle(0, 0, 32, 32)), Color.White, Color.White, new StardustCore.UIUtilities.MenuComponents.Delegates.Functionality.ButtonFunctionality(null, null, null), false, components);

            //Change this to take the vocalization menu instead
            List<KeyValuePair<Button, IClickableMenuExtended>> modTabs = new List<KeyValuePair<Button, IClickableMenuExtended>>();
            modTabs.Add(new KeyValuePair<Button, IClickableMenuExtended>(menuTab, new VocalizationMenu(100, 64, 600, 300, true)));

            StardustCore.Menus.ModularGameMenu.StaticMenuTabsAndPages[ModManifest.UniqueID] = modTabs;
        }

        public static object GetInstanceField(Type type, object instance, string fieldName)
        {
            BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
            FieldInfo field = type.GetField(fieldName, bindFlags);
            /*
            FieldInfo[] meh = type.GetFields(bindFlags);
            foreach(var v in meh)
            {
                if (v.Name == null)
                {
                    continue;
                }
                Monitor.Log(v.Name);
            }
            */
            return field.GetValue(instance);
        }
        /// <summary>
        /// Runs every game tick to check if the player is talking to an npc.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameEvents_UpdateTick(object sender, EventArgs e)
        {
            soundManager.update();
            if (Game1.player != null)
            {
                if (Game1.player.currentLocation != null)
                {
                    foreach (NPC v in Game1.currentLocation.characters)
                    {
                        string text = (string)GetInstanceField(typeof(NPC), v, "textAboveHead");
                        int timer = (int)GetInstanceField(typeof(NPC), v, "textAboveHeadTimer");
                        if (text == null) continue;
                        string currentDialogue = text;

                        if (onScreenSpeechBubbleDialogues.Contains(currentDialogue) && timer > 0) continue; //If I have already added this dialogue and the timer has not run out, do nothing.

                        if (!onScreenSpeechBubbleDialogues.Contains(currentDialogue) && timer > 0) //If I have not added this dialogue and the timer has not run out, add it.
                        {
                            List<string> tries = new List<string>();
                            tries.Add("SpeechBubbles");
                            tries.Add(v.Name);
                            foreach (var speech in tries)
                            {
                                CharacterVoiceCue voice;
                                DialogueCues.TryGetValue(speech, out voice);
                                currentDialogue = sanitizeDialogueInGame(currentDialogue); //If contains the stuff in the else statement, change things up.
                                if (voice.dialogueCues.ContainsKey(currentDialogue))
                                {
                                    //Not variable messages. Aka messages that don't contain words the user can change such as farm name, farmer name etc. 
                                    voice.speak(currentDialogue);
                                    onScreenSpeechBubbleDialogues.Add(currentDialogue);
                                    return;
                                }
                                else
                                {
                                    ModMonitor.Log("New unregistered dialogue detected for NPC: " + speech + " saying: " + currentDialogue, LogLevel.Alert);
                                    ModMonitor.Log("Make sure to add this to their respective VoiceCue.json file if you wish for this dialogue to have voice acting associated with it!", LogLevel.Alert);

                                }
                            }
                        }
                        else
                        {
                            if (timer <= 0 && onScreenSpeechBubbleDialogues.Contains(currentDialogue)) //If the timer has run out and I still contain the dialogue, remove it.
                            {
                                onScreenSpeechBubbleDialogues.Remove(currentDialogue);
                            }
                            if (timer <= 0 && !onScreenSpeechBubbleDialogues.Contains(currentDialogue)) //If the timer has run out and I no longer contain the dialogue, continue on.
                            {
                                continue;
                            }
                        }
                    }
                }
            }

            if (Game1.currentSpeaker != null)
            {
                string speakerName = Game1.currentSpeaker.Name;
                if (Game1.activeClickableMenu.GetType() == typeof(StardewValley.Menus.DialogueBox))
                {
                    StardewValley.Menus.DialogueBox dialogueBox = (DialogueBox)Game1.activeClickableMenu;
                    string currentDialogue = dialogueBox.getCurrentString();
                    if (previousDialogue != currentDialogue)
                    {
                        //ModMonitor.Log(speakerName);
                        previousDialogue = currentDialogue; //Update my previously read dialogue so that I only read the new string once when it appears.
                        //ModMonitor.Log(currentDialogue); //Print out my dialogue.

                        //Do logic here to figure out what audio clips to play.
                        //Sanitize input here!
                        //Load all game dialogue files and then sanitize input.

                        List<string> tries = new List<string>();
                        tries.Add(speakerName);
                        tries.Add("ExtraDialogue");
                        tries.Add("Events");
                        tries.Add("CharactersStrings");
                        tries.Add("LocationDialogue");
                        tries.Add("Utility");
                        tries.Add("Quests");
                        tries.Add("NPCGiftTastes");
                        tries.Add("Temp");
                        foreach (var v in tries)
                        {
                            CharacterVoiceCue voice;
                            DialogueCues.TryGetValue(v, out voice);
                            currentDialogue = sanitizeDialogueInGame(currentDialogue); //If contains the stuff in the else statement, change things up.

                            if (voice.dialogueCues.ContainsKey(currentDialogue))
                            {
                                //Not variable messages. Aka messages that don't contain words the user can change such as farm name, farmer name etc. 
                                voice.speak(currentDialogue);
                                return;
                            }
                            else
                            {
                                ModMonitor.Log("New unregistered dialogue detected for NPC: " + v + " saying: " + currentDialogue, LogLevel.Alert);
                                ModMonitor.Log("Make sure to add this to their respective VoiceCue.json file if you wish for this dialogue to have voice acting associated with it!", LogLevel.Alert);

                            }
                        }
                    }
                }
            }
            else
            {
                if (Game1.activeClickableMenu == null) return;
                //Support for TV
                if (Game1.activeClickableMenu.GetType() == typeof(StardewValley.Menus.DialogueBox))
                {
                    StardewValley.Menus.DialogueBox dialogueBox = (DialogueBox)Game1.activeClickableMenu;
                    string currentDialogue = dialogueBox.getCurrentString();
                    if (previousDialogue != currentDialogue)
                    {
                        previousDialogue = currentDialogue; //Update my previously read dialogue so that I only read the new string once when it appears.
                        ModMonitor.Log(currentDialogue); //Print out my dialogue.

                        List<string> tries = new List<string>();
                        tries.Add("TV");
                        tries.Add("Events");
                        tries.Add("Characters");
                        tries.Add("LocationDialogue");
                        tries.Add("Notes");
                        tries.Add("Utility");
                        tries.Add("Quests");
                        tries.Add("NPCGiftTastes");
                        foreach (var v in tries)
                        {
                            //Add in support for TV Shows
                            CharacterVoiceCue voice;
                            bool f = DialogueCues.TryGetValue(v, out voice);
                            currentDialogue = sanitizeDialogueInGame(currentDialogue); //If contains the stuff in the else statement, change things up.
                            if (voice.dialogueCues.ContainsKey(currentDialogue))
                            {
                                //Not variable messages. Aka messages that don't contain words the user can change such as farm name, farmer name etc. 
                                voice.speak(currentDialogue);

                                //ModMonitor.Log("SPEAK THE TELLE");
                                return;
                            }
                            else
                            {
                                ModMonitor.Log("New unregistered dialogue detected saying: " + currentDialogue, LogLevel.Alert);
                                ModMonitor.Log("Make sure to add this to their respective VoiceCue.json file if you wish for this dialogue to have voice acting associated with it!", LogLevel.Alert);

                            }
                        }
                    }
                }

                //Support for Letters
                if (Game1.activeClickableMenu.GetType() == typeof(StardewValley.Menus.LetterViewerMenu))
                {
                    //Use reflection to get original text back.
                    var menu = (StardewValley.Menus.LetterViewerMenu)Game1.activeClickableMenu;
                    //mail dialogue text will probably need to be sanitized as well....
                    List<string> mailText = (List<string>)ModHelper.Reflection.GetField<List<string>>(menu, "mailMessage", true);
                    string currentDialogue = "";
                    foreach (var v in mailText)
                    {
                        currentDialogue += mailText;
                    }

                    previousDialogue = currentDialogue; //Update my previously read dialogue so that I only read the new string once when it appears.


                    //Add in support for TV Shows
                    CharacterVoiceCue voice;
                    DialogueCues.TryGetValue("Mail", out voice);
                    currentDialogue = sanitizeDialogueInGame(currentDialogue); //If contains the stuff in the else statement, change things up.
                    if (voice.dialogueCues.ContainsKey(currentDialogue))
                    {
                        //Not variable messages. Aka messages that don't contain words the user can change such as farm name, farmer name etc. 
                        voice.speak(currentDialogue);
                    }
                    else
                    {
                        ModMonitor.Log("New unregistered Mail dialogue detected saying: " + currentDialogue, LogLevel.Alert);
                        ModMonitor.Log("Make sure to add this to their respective VoiceCue.json file if you wish for this dialogue to have voice acting associated with it!", LogLevel.Alert);
                    }

                }

                //Support for shops
                if (Game1.activeClickableMenu.GetType() == typeof(StardewValley.Menus.ShopMenu))
                {

                    var menu = (StardewValley.Menus.ShopMenu)Game1.activeClickableMenu;
                    string shopDialogue = menu.potraitPersonDialogue; //Check this string to the dict of voice cues

                    shopDialogue = shopDialogue.Replace(Environment.NewLine, "");

                    NPC npc = menu.portraitPerson;

                    if (previousDialogue == shopDialogue) return;
                    previousDialogue = shopDialogue; //Update my previously read dialogue so that I only read the new string once when it appears.


                    //Add in support for Shops
                    CharacterVoiceCue voice;
                    //character shops
                    bool f = DialogueCues.TryGetValue("Shops", out voice);
                    if (f == false)
                    {
                        ModMonitor.Log("Can't find the dialogue for the shop: " + npc.Name);
                    }
                    shopDialogue = sanitizeDialogueInGame(shopDialogue); //If contains the stuff in the else statement, change things up.


                    //I have no clue why the parsing adds in an extra character sometimes but I guess I have to do this in some cases....
                    if (!voice.dialogueCues.ContainsKey(shopDialogue))
                    {
                        shopDialogue = shopDialogue.Substring(0, shopDialogue.Length - 1);
                    }


                    if (voice.dialogueCues.ContainsKey(shopDialogue))
                    {
                        //Not variable messages. Aka messages that don't contain words the user can change such as farm name, farmer name etc. 
                        voice.speak(shopDialogue);
                    }
                    else
                    {


                        ModMonitor.Log("New unregistered dialogue detected saying: " + shopDialogue, LogLevel.Alert);
                        ModMonitor.Log("Make sure to add this to their respective VoiceCue.json file if you wish for this dialogue to have voice acting associated with it!", LogLevel.Alert);
                    }

                }

            }
        }

        /// <summary>
        /// Runs after loading and creates necessary mod directories.
        /// </summary>
        private void initialzeDirectories()
        {
            string basePath = ModHelper.DirectoryPath;
            string contentPath = Path.Combine(basePath, "Content");
            string graphicsPath = Path.Combine(contentPath, "Graphics");
            string audioPath = Path.Combine(contentPath, "Audio");
            string voicePath = Path.Combine(audioPath, "VoiceFiles");

            if (!Directory.Exists(graphicsPath)) Directory.CreateDirectory(graphicsPath);

            VoicePath = voicePath; //Set a static reference to my voice files directory.



            //Get a list of all characters in the game and make voice directories for them in each supported translation of the mod.
            foreach (var loc in Game1.locations)
            {
                foreach (var NPC in loc.characters)
                {
                    foreach (var translation in config.translationInfo.translations)
                    {
                        string characterPath = Path.Combine(translation, NPC.Name);
                        characterDialoguePaths.Add(characterPath);
                    }
                }
            }

            //Create all of the necessary folders for different translations.
            foreach (var dir in config.translationInfo.translations)
            {
                if (!Directory.Exists(Path.Combine(voicePath, dir))) Directory.CreateDirectory(Path.Combine(voicePath, dir));
            }

            //Add in folder for TV Shows
            foreach (var translation in config.translationInfo.translations)
            {
                string TVPath = Path.Combine(translation, "TV");
                characterDialoguePaths.Add(TVPath);
            }

            //Add in folder for shop support
            foreach (var translation in config.translationInfo.translations)
            {
                string shop = Path.Combine(translation, "Shops"); //Used to hold NPC Shops
                characterDialoguePaths.Add(shop);
            }

            //Add in folder for Mail support.
            foreach (var translation in config.translationInfo.translations)
            {
                string mail = Path.Combine(translation, "Mail");
                characterDialoguePaths.Add(mail);
            }

            //Add in folder for ExtraDiaogue.yaml
            foreach (var translation in config.translationInfo.translations)
            {
                string extra = Path.Combine(translation, "ExtraDialogue");
                characterDialoguePaths.Add(extra);
            }

            foreach (var translation in config.translationInfo.translations)
            {
                string extra = Path.Combine(translation, "Events");
                characterDialoguePaths.Add(extra);
            }

            foreach (var translation in config.translationInfo.translations)
            {
                string extra = Path.Combine(translation, "Characters");
                characterDialoguePaths.Add(extra);
            }

            foreach (var translation in config.translationInfo.translations)
            {
                string extra = Path.Combine(translation, "LocationDialogue");
                characterDialoguePaths.Add(extra);
            }

            foreach (var translation in config.translationInfo.translations)
            {
                string extra = Path.Combine(translation, "Notes");
                characterDialoguePaths.Add(extra);
            }

            foreach (var translation in config.translationInfo.translations)
            {
                string extra = Path.Combine(translation, "Utility");
                characterDialoguePaths.Add(extra);
            }

            foreach (var translation in config.translationInfo.translations)
            {
                string extra = Path.Combine(translation, "NPCGiftTastes");
                characterDialoguePaths.Add(extra);
            }

            foreach (var translation in config.translationInfo.translations)
            {
                string extra = Path.Combine(translation, "SpeechBubbles");
                characterDialoguePaths.Add(extra);
            }

            foreach (var translation in config.translationInfo.translations)
            {
                string kent = Path.Combine(translation, "Kent");
                characterDialoguePaths.Add(kent);


                string gil = Path.Combine(translation, "Gil");
                characterDialoguePaths.Add(gil);


                string governor = Path.Combine(translation, "Governor");
                characterDialoguePaths.Add(governor);


                string grandpa = Path.Combine(translation, "Grandpa");
                characterDialoguePaths.Add(grandpa);


                string morris = Path.Combine(translation, "Morris");
                characterDialoguePaths.Add(morris);
            }



            foreach (var translation in config.translationInfo.translations)
            {
                string extra = Path.Combine(translation, "Quests");
                characterDialoguePaths.Add(extra);
            }


            foreach (var translation in config.translationInfo.translations)
            {
                string extra = Path.Combine(translation, "Temp");
                characterDialoguePaths.Add(extra);
            }

            if (!Directory.Exists(contentPath)) Directory.CreateDirectory(contentPath);
            if (!Directory.Exists(audioPath)) Directory.CreateDirectory(audioPath);
            if (!Directory.Exists(voicePath)) Directory.CreateDirectory(voicePath);



            //Create a list of new directories if the corresponding character directory doesn't exist.
            //Note: A modder could also manually add in their own character directory for voice lines instead of having to add it via code.
            foreach (var dir in characterDialoguePaths)
            {
                if (!Directory.Exists(Path.Combine(voicePath, dir))) Directory.CreateDirectory(Path.Combine(voicePath, dir));
            }
        }


        /// <summary>
        /// Loads in all of the .wav files associated with voice acting clips.
        /// </summary>
        public static void loadAllVoiceFiles()
        {
            //get a list of all translations supported by this mod.
            List<string> translations = Directory.GetDirectories(VoicePath).ToList();
            foreach (var translation in translations)
            {
                string[] characterVoiceLines = Directory.GetDirectories(translation);
                //get a list of all characters supported in this translation and load their voice cue file.
                foreach (var dir in characterVoiceLines)
                {
                    ModMonitor.Log(dir);

                    string[] clips = Directory.GetFiles(dir, "*.wav");

                    //For every .wav file in every character voice clip directory load in the voice clip.
                    foreach (var file in clips)
                    {
                        string fileName = Path.GetFileNameWithoutExtension(file);
                        soundManager.loadWavFile(ModHelper, fileName, file);
                        ModMonitor.Log("Loaded sound file: " + fileName + " from: " + file);
                    }

                    //Get the character dialogue cues (aka when the character should "speak") from the .json file.
                    string voiceCueFile = Path.Combine(dir, "VoiceCues.json");
                    string characterName = Path.GetFileName(dir);

                    //If a file was not found, create one and add it to the list of character voice cues.
                    //I have to scrape all files if they don't exist so that way all options are available for release.
                    if (!File.Exists(voiceCueFile))
                    {

                        CharacterVoiceCue cue = new CharacterVoiceCue(characterName);
                        cue.initializeEnglishScrape();
                        cue.initializeForTranslation(translation);
                        scrapeDictionaries(voiceCueFile, cue, translation);
                        try
                        {
                            if (Path.GetFileName(translation) == config.translationInfo.currentTranslation)
                            {
                                DialogueCues.Add(characterName, cue);
                            }
                        }
                        catch (Exception err)
                        {

                        }
                    }
                    else
                    {
                        try
                        {
                            //Only load in the cues for the current translation.
                            if (Path.GetFileName(translation) == config.translationInfo.currentTranslation)
                            {
                                CharacterVoiceCue cue = ModHelper.ReadJsonFile<CharacterVoiceCue>(voiceCueFile);
                                //scrapeDictionaries(voiceCueFile,cue);
                                DialogueCues.Add(characterName, cue);
                            }
                        }
                        catch (Exception err)
                        {
                            ModMonitor.Log(err.ToString());
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Used to obtain all strings for almost all possible dialogue in the game.
        /// </summary>
        /// <param name="cue"></param>
        public static void scrapeDictionaries(string path, CharacterVoiceCue cue, string translation)
        {

            var dialoguePath = Path.Combine("Characters", "Dialogue");
            var stringsPath = Path.Combine("Strings"); //Used for all sorts of extra strings and stuff for like StringsFromCS
            var dataPath = Path.Combine("Data"); //Used for engagement dialogue strings, and ExtraDialogue, Notes, Secret Notes, Mail
            var festivalPath = Path.Combine(dataPath, "Festivals");
            var eventPath = Path.Combine(dataPath, "Events");

            ModMonitor.Log("Scraping dialogue for character: " + cue.name, LogLevel.Info);
            //If the "character"'s name is TV which means I'm watching tv, scrape the data from the TV shows.
            if (cue.name == "TV")
            {

                foreach (var fileName in cue.dataFileNames)
                {
                    ModMonitor.Log("    Scraping dialogue file: " + fileName, LogLevel.Info);
                    //basically this will never run but can be used below to also add in dialogue.
                    if (!String.IsNullOrEmpty(fileName))
                    {
                        string dialoguePath2 = Path.Combine(dataPath, "TV", fileName);
                        var DialogueDict = ModHelper.Content.Load<Dictionary<string, string>>(dialoguePath2, ContentSource.GameContent);

                        //Scraping the CookingChannel dialogue
                        if (fileName.Contains("CookingChannel"))
                        {
                            //Scrape the whole dictionary looking for the character's name.
                            foreach (KeyValuePair<string, string> pair in DialogueDict)
                            {
                                //Get the key in the dictionary
                                string key = pair.Key;
                                string rawDialogue = pair.Value;
                                List<string> splitDialogues = new List<string>();
                                splitDialogues = rawDialogue.Split('/').ToList();

                                string cookingDialogue = splitDialogues.ElementAt(1);
                                //If the key contains the character's name.
                                List<string> cleanDialogues = new List<string>();
                                cleanDialogues = sanitizeDialogueFromDictionaries(cookingDialogue, cue);
                                foreach (var str in cleanDialogues)
                                {
                                    if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, fileName, key)))
                                    {
                                        AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, fileName, key), out VoiceAudioOptions value);

                                        cue.addDialogue(str, new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));
                                    }
                                    else
                                    {
                                        cue.addDialogue(str, new VoiceAudioOptions()); //Make a new dialogue line based off of the text, but have the .wav value as empty.
                                        AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, fileName, key), new VoiceAudioOptions());
                                    }
                                }

                            }
                            continue;
                        }

                        //Interview Show
                        if (fileName.Contains("InterviewShow"))
                        {
                            //Scrape the whole dictionary looking for the character's name.
                            foreach (KeyValuePair<string, string> pair in DialogueDict)
                            {
                                //Get the key in the dictionary
                                string key = pair.Key;
                                string rawDialogue = pair.Value;
                                if (key != "intro") continue;
                                //If the key contains the character's name.
                                List<string> cleanDialogues = new List<string>();
                                cleanDialogues = sanitizeDialogueFromDictionaries(rawDialogue, cue);
                                foreach (var str in cleanDialogues)
                                {

                                    if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, fileName, key)))
                                    {
                                        AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, fileName, key), out VoiceAudioOptions value);

                                        cue.addDialogue(str, new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));
                                    }
                                    else
                                    {
                                        cue.addDialogue(str, new VoiceAudioOptions()); //Make a new dialogue line based off of the text, but have the .wav value as empty.
                                        AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, fileName, key), new VoiceAudioOptions());
                                    }
                                }

                            }
                            continue;
                        }

                        //Tip channel
                        if (fileName.Contains("TipChannel"))
                        {
                            //Scrape the whole dictionary looking for the character's name.
                            foreach (KeyValuePair<string, string> pair in DialogueDict)
                            {
                                //Get the key in the dictionary
                                string key = pair.Key;
                                string rawDialogue = pair.Value;

                                List<string> cleanDialogues = new List<string>();
                                cleanDialogues = sanitizeDialogueFromDictionaries(rawDialogue, cue);
                                foreach (var str in cleanDialogues)
                                {
                                    if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, fileName, key)))
                                    {
                                        AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, fileName, key), out VoiceAudioOptions value);

                                        cue.addDialogue(str, new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));
                                    }
                                    else
                                    {
                                        cue.addDialogue(str, new VoiceAudioOptions()); //Make a new dialogue line based off of the text, but have the .wav value as empty.
                                        AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, fileName, key), new VoiceAudioOptions());
                                    }
                                }

                            }
                            continue;
                        }
                    }
                }

                foreach (var fileName in cue.stringsFileNames)
                {
                    ModMonitor.Log("    Scraping dialogue file: " + fileName, LogLevel.Info);
                    //basically this will never run but can be used below to also add in dialogue.
                    if (!String.IsNullOrEmpty(fileName))
                    {
                        string dialoguePath2 = Path.Combine(stringsPath, fileName);
                        var DialogueDict = ModHelper.Content.Load<Dictionary<string, string>>(dialoguePath2, ContentSource.GameContent);
                        if (fileName.Contains("StringsFromCSFiles"))
                        {
                            //Scrape the whole dictionary looking for the character's name.
                            foreach (KeyValuePair<string, string> pair in DialogueDict)
                            {
                                //Get the key in the dictionary
                                string key = pair.Key;
                                string rawDialogue = pair.Value;
                                if (!key.Contains("TV")) continue;
                                //If the key contains the character's name.
                                List<string> cleanDialogues = new List<string>();


                                if (key == "TV.cs.13151")
                                {
                                    foreach (string recipe in Vocabulary.getAllCookingRecipes(translation))
                                    {
                                        rawDialogue = config.translationInfo.LoadString(Path.Combine("Strings", "StringsFromCSFiles:TV.cs.13151"), translation, (object)recipe);
                                        List<string> cleanDialogues2 = new List<string>();
                                        cleanDialogues2 = sanitizeDialogueFromDictionaries(rawDialogue, cue);
                                        foreach (var str in cleanDialogues2)
                                        {
                                            if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, fileName, key)))
                                            {
                                                AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, fileName, key), out VoiceAudioOptions value);

                                                cue.addDialogue(str, new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));
                                            }
                                            else
                                            {
                                                cue.addDialogue(str, new VoiceAudioOptions()); //Make a new dialogue line based off of the text, but have the .wav value as empty.
                                                AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, fileName, key), new VoiceAudioOptions());
                                            }
                                        }
                                    }
                                    continue;
                                }


                                if (key == "TV.cs.13153")
                                {
                                    foreach (string recipe in Vocabulary.getAllCookingRecipes(translation))
                                    {
                                        rawDialogue = config.translationInfo.LoadString(Path.Combine("Strings", "StringsFromCSFiles:TV.cs.13153"), translation, (object)recipe);
                                        List<string> cleanDialogues2 = new List<string>();
                                        cleanDialogues2 = sanitizeDialogueFromDictionaries(rawDialogue, cue);
                                        foreach (var str in cleanDialogues2)
                                        {
                                            if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, fileName, key)))
                                            {
                                                AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, fileName, key), out VoiceAudioOptions value);

                                                cue.addDialogue(str, new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));
                                            }
                                            else
                                            {
                                                cue.addDialogue(str, new VoiceAudioOptions()); //Make a new dialogue line based off of the text, but have the .wav value as empty.
                                                AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, fileName, key), new VoiceAudioOptions());
                                            }
                                        }
                                    }
                                    continue;
                                }

                                if (key == "TV.cs.13175")
                                {
                                    Dictionary<string, string> dictionary;
                                    foreach (string season in Vocabulary.getSeasons())
                                    {
                                        for (int i = 1; i <= 28; i++)
                                        {
                                            try
                                            {
                                                dictionary = Game1.content.Load<Dictionary<string, string>>(Path.Combine("Data", "Festivals", config.translationInfo.getXNBForTranslation(season + (object)(i.ToString()), translation)));
                                                ModMonitor.Log("Scraping TV Festival File: " + season + i.ToString());
                                                dictionary.TryGetValue("name", out string name);
                                                dictionary.TryGetValue("conditions", out string condition);
                                                string location = condition.Split('/').ElementAt(0);
                                                string times = condition.Split('/').ElementAt(1);
                                                string startTime = times.Split(' ').ElementAt(0);
                                                string finishTime = times.Split(' ').ElementAt(1);
                                                config.translationInfo.changeLocalizedContentManagerFromTranslation(translation);
                                                string dialogueString = config.translationInfo.LoadString(Path.Combine("Strings", "StringsFromCSFiles:TV.cs.13175"), translation, (object)name, (object)location, (object)Game1.getTimeOfDayString(Convert.ToInt32(startTime)), (object)Game1.getTimeOfDayString(Convert.ToInt32(finishTime)));
                                                config.translationInfo.resetLocalizationCode();

                                                cleanDialogues = sanitizeDialogueFromDictionaries(dialogueString, cue);

                                                foreach (var str in cleanDialogues)
                                                {
                                                    string ahh = sanitizeDialogueFromMailDictionary(str);
                                                    if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, fileName, key)))
                                                    {
                                                        AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, fileName, key), out VoiceAudioOptions value);

                                                        cue.addDialogue(ahh, new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));
                                                    }
                                                    else
                                                    {
                                                        cue.addDialogue(ahh, new VoiceAudioOptions()); //Make a new dialogue line based off of the text, but have the .wav value as empty.
                                                        AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, fileName, key), new VoiceAudioOptions());
                                                    }
                                                }


                                            }
                                            catch (Exception err)
                                            {
                                                //ModMonitor.Log(err.ToString());
                                            }
                                        }
                                    }
                                    continue;
                                }

                                cleanDialogues = sanitizeDialogueFromDictionaries(rawDialogue, cue);

                                foreach (var str in cleanDialogues)
                                {
                                    string ahh = sanitizeDialogueFromMailDictionary(str);
                                    if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, fileName, key)))
                                    {
                                        AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, fileName, key), out VoiceAudioOptions value);

                                        cue.addDialogue(ahh, new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));
                                    }
                                    else
                                    {
                                        cue.addDialogue(ahh, new VoiceAudioOptions()); //Make a new dialogue line based off of the text, but have the .wav value as empty.
                                        AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, fileName, key), new VoiceAudioOptions());
                                    }
                                }

                            }
                            continue;
                        }
                    }
                }


            }

            //If the "character"'s name is Shops which means I'm talking to a shopkeeper.
            else if (cue.name == "Shops")
            {
                foreach (var fileName in cue.stringsFileNames)
                {
                    ModMonitor.Log("    Scraping dialogue file: " + fileName, LogLevel.Info);
                    //basically this will never run but can be used below to also add in dialogue.
                    if (!String.IsNullOrEmpty(fileName))
                    {
                        string dialoguePath2 = Path.Combine(stringsPath, fileName);
                        var DialogueDict = ModHelper.Content.Load<Dictionary<string, string>>(dialoguePath2, ContentSource.GameContent);

                        //Scraping the CookingChannel dialogue

                        if (fileName.Contains("StringsFromCSFiles"))
                        {
                            //Scrape the whole dictionary looking for the character's name.
                            foreach (KeyValuePair<string, string> pair in DialogueDict)
                            {
                                //Get the key in the dictionary
                                string key = pair.Key;
                                string rawDialogue = pair.Value;
                                if (!key.Contains("ShopMenu")) continue;
                                //If the key contains the character's name.


                                if (key == "ShopMenu.cs.11464")
                                {
                                    foreach (var obj in Vocabulary.getCarpenterStock(translation))
                                    {
                                        foreach (string word1 in Vocabulary.getRandomPositiveAdjectivesForEventOrPerson(translation, null))
                                        {

                                            rawDialogue = config.translationInfo.LoadString(Path.Combine("Strings", "StringsFromCSFiles:ShopMenu.cs.11464"), translation, (object)obj, (object)word1, (object)Vocabulary.getProperArticleForWord(obj, translation));
                                            List<string> cleanDialogues2 = new List<string>();
                                            cleanDialogues2 = sanitizeDialogueFromDictionaries(rawDialogue, cue);
                                            foreach (var str in cleanDialogues2)
                                            {
                                                if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, fileName, key)))
                                                {
                                                    AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, fileName, key), out VoiceAudioOptions value);

                                                    cue.addDialogue(str, new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));
                                                }
                                                else
                                                {
                                                    cue.addDialogue(str, new VoiceAudioOptions()); //Make a new dialogue line based off of the text, but have the .wav value as empty.
                                                    AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, fileName, key), new VoiceAudioOptions());
                                                }
                                            }
                                        }
                                    }
                                    continue;
                                }
                                if (key == "ShopMenu.cs.11502")
                                {
                                    foreach (var obj in Vocabulary.getMerchantStock(translation))
                                    {
                                        rawDialogue = config.translationInfo.LoadString(Path.Combine("Strings", "StringsFromCSFiles:ShopMenu.cs.11502"), translation, (object)obj);
                                        List<string> cleanDialogues2 = new List<string>();
                                        cleanDialogues2 = sanitizeDialogueFromDictionaries(rawDialogue, cue);
                                        foreach (var str in cleanDialogues2)
                                        {
                                            if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, fileName, key)))
                                            {
                                                AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, fileName, key), out VoiceAudioOptions value);

                                                cue.addDialogue(str, new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));
                                            }
                                            else
                                            {
                                                cue.addDialogue(str, new VoiceAudioOptions()); //Make a new dialogue line based off of the text, but have the .wav value as empty.
                                                AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, fileName, key), new VoiceAudioOptions());
                                            }
                                        }

                                    }
                                    continue;
                                }

                                if (key == "ShopMenu.cs.11512")
                                {
                                    foreach (var obj in Vocabulary.getMerchantStock(translation))
                                    {
                                        rawDialogue = config.translationInfo.LoadString(Path.Combine("Strings", "StringsFromCSFiles:ShopMenu.cs.11512"), translation, (object)obj);
                                        List<string> cleanDialogues2 = new List<string>();
                                        cleanDialogues2 = sanitizeDialogueFromDictionaries(rawDialogue, cue);
                                        foreach (var str in cleanDialogues2)
                                        {
                                            if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, fileName, key)))
                                            {
                                                AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, fileName, key), out VoiceAudioOptions value);

                                                cue.addDialogue(str, new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));
                                            }
                                            else
                                            {
                                                cue.addDialogue(str, new VoiceAudioOptions()); //Make a new dialogue line based off of the text, but have the .wav value as empty.
                                                AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, fileName, key), new VoiceAudioOptions());
                                            }
                                        }

                                    }
                                    continue;
                                }

                                List<string> cleanDialogues = new List<string>();
                                cleanDialogues = sanitizeDialogueFromDictionaries(rawDialogue, cue);
                                foreach (var str in cleanDialogues)
                                {
                                    if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, fileName, key)))
                                    {
                                        AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, fileName, key), out VoiceAudioOptions value);

                                        cue.addDialogue(str, new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));
                                    }
                                    else
                                    {
                                        cue.addDialogue(str, new VoiceAudioOptions()); //Make a new dialogue line based off of the text, but have the .wav value as empty.
                                        AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, fileName, key), new VoiceAudioOptions());
                                    }
                                }

                            }

                            continue;
                        }
                        //For moddablity add a generic scrape here!
                    }
                }

            }

            //Scrape Content/Data/ExtraDialogue.yaml
            else if (cue.name == "ExtraDialogue")
            {
                foreach (var fileName in cue.dataFileNames)
                {
                    ModMonitor.Log("    Scraping dialogue file: " + fileName, LogLevel.Info);
                    //basically this will never run but can be used below to also add in dialogue.
                    if (!String.IsNullOrEmpty(fileName))
                    {
                        string dialoguePath2 = Path.Combine(dataPath, fileName);
                        var DialogueDict = ModHelper.Content.Load<Dictionary<string, string>>(dialoguePath2, ContentSource.GameContent);
                        foreach (KeyValuePair<string, string> pair in DialogueDict)
                        {
                            //Get the key in the dictionary
                            string key = pair.Key;
                            string rawDialogue = pair.Value;

                            List<string> cleanDialogues = new List<string>();
                            cleanDialogues = sanitizeDialogueFromDictionaries(rawDialogue, cue);
                            foreach (var str in cleanDialogues)
                            {

                                if (key == "NewChild_Adoption")
                                {
                                    if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, fileName, key)))
                                    {
                                        AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, fileName, key), out VoiceAudioOptions value);

                                        cue.addDialogue(config.translationInfo.LoadString(Path.Combine("Data", "ExtraDialogue:" + key), translation, replacementStrings.kid1Name), new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));
                                        cue.addDialogue(config.translationInfo.LoadString(Path.Combine("Data", "ExtraDialogue:" + key), translation, replacementStrings.kid2Name), new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));
                                    }
                                    else
                                    {
                                        cue.addDialogue(config.translationInfo.LoadString(Path.Combine("Data", "ExtraDialogue:" + key), translation, replacementStrings.kid1Name), new VoiceAudioOptions());
                                        cue.addDialogue(config.translationInfo.LoadString(Path.Combine("Data", "ExtraDialogue:" + key), translation, replacementStrings.kid2Name), new VoiceAudioOptions());
                                        AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, fileName, key), new VoiceAudioOptions());
                                    }
                                    continue;
                                }
                                if (key == "NewChild_FirstChild")
                                {
                                    if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, fileName, key)))
                                    {
                                        AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, fileName, key), out VoiceAudioOptions value);
                                        cue.addDialogue(config.translationInfo.LoadString(Path.Combine("Data", "ExtraDialogue:" + key), translation, replacementStrings.kid1Name), new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));

                                    }
                                    else
                                    {
                                        cue.addDialogue(config.translationInfo.LoadString(Path.Combine("Data", "ExtraDialogue:" + key), translation, replacementStrings.kid1Name), new VoiceAudioOptions());
                                        AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, fileName, key), new VoiceAudioOptions());
                                    }
                                    continue;
                                }

                                if (key == "Farm_RobinWorking_ReadyTomorrow" || key == "Robin_NewConstruction_Festival" || key == "Robin_NewConstruction" || key == "Robin_Instant")
                                {
                                    string buildingsPath = Path.Combine(dataPath, config.translationInfo.getBuildingXNBForTranslation(translation));
                                    var BuildingDict = ModHelper.Content.Load<Dictionary<string, string>>(buildingsPath, ContentSource.GameContent);

                                    foreach (KeyValuePair<string, string> pair2 in BuildingDict)
                                    {
                                        List<string> cleanedDialogues = sanitizeDialogueFromDictionaries(config.translationInfo.LoadString(Path.Combine("Data", "ExtraDialogue:" + key), translation, pair2.Key), cue);
                                        foreach (var clean_str in cleanedDialogues)
                                        {
                                            if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, fileName, key)))
                                            {
                                                AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, fileName, key), out VoiceAudioOptions value);
                                                cue.addDialogue(clean_str, new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));

                                            }
                                            else
                                            {
                                                cue.addDialogue(clean_str, new VoiceAudioOptions());
                                                AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, fileName, key), new VoiceAudioOptions());
                                            }
                                        }
                                    }
                                    continue;
                                }

                                if (key == "Farm_RobinWorking1" || key == "Farm_RobinWorking2")
                                {
                                    string buildingsPath = Path.Combine(dataPath, config.translationInfo.getBuildingXNBForTranslation(translation));
                                    var BuildingDict = ModHelper.Content.Load<Dictionary<string, string>>(buildingsPath, ContentSource.GameContent);

                                    foreach (KeyValuePair<string, string> pair2 in BuildingDict)
                                    {
                                        for (int i = 1; i <= 3; i++)
                                        {

                                            if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, fileName, key)))
                                            {
                                                AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, fileName, key), out VoiceAudioOptions value);
                                                cue.addDialogue(config.translationInfo.LoadString(Path.Combine("Data", "ExtraDialogue:" + key), translation, pair2.Key, i.ToString()), new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));

                                            }
                                            else
                                            {
                                                cue.addDialogue(config.translationInfo.LoadString(Path.Combine("Data", "ExtraDialogue:" + key), translation, pair2.Key, i.ToString()), new VoiceAudioOptions());
                                                AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, fileName, key), new VoiceAudioOptions());
                                            }


                                        }
                                    }
                                    continue;
                                }

                                //Generate all possible tool combinations for clint.
                                if (key == "Clint_StillWorking")
                                {
                                    List<string> tools = new List<string>();
                                    tools.Add("Hoe");
                                    tools.Add("Pickaxe");
                                    tools.Add("Axe");
                                    tools.Add("Watering Can");

                                    List<string> levels = new List<string>();
                                    levels.Add("Copper ");
                                    levels.Add("Steel ");
                                    levels.Add("Gold ");
                                    levels.Add("Iridium ");

                                    foreach (var tool in tools)
                                    {
                                        foreach (var lvl in levels)
                                        {
                                            if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, fileName, key)))
                                            {
                                                AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, fileName, key), out VoiceAudioOptions value);
                                                cue.addDialogue(config.translationInfo.LoadString(Path.Combine("Data", "ExtraDialogue:" + key), translation, lvl + tool), new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));

                                            }
                                            else
                                            {
                                                cue.addDialogue(config.translationInfo.LoadString(Path.Combine("Data", "ExtraDialogue:" + key), translation, lvl + tool), new VoiceAudioOptions());
                                                AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, fileName, key), new VoiceAudioOptions());
                                            }

                                        }
                                    }
                                    continue;
                                }

                                if (key == "Morris_WeekendGreeting_MembershipAvailable" || key == "Morris_FirstGreeting_MembershipAvailable")
                                {
                                    List<string> cleanedDialogues = sanitizeDialogueFromDictionaries(rawDialogue, cue);
                                    foreach (var dia in cleanedDialogues)
                                    {
                                        if (dia.Contains("{0}"))
                                        {
                                            string actual = dia.Replace("{0}", "5000");

                                            if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, fileName, key)))
                                            {
                                                AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, fileName, key), out VoiceAudioOptions value);
                                                cue.addDialogue(actual, new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));

                                            }
                                            else
                                            {
                                                cue.addDialogue(actual, new VoiceAudioOptions());
                                                AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, fileName, key), new VoiceAudioOptions());
                                            }

                                        }
                                        else
                                        {
                                            if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, fileName, key)))
                                            {
                                                AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, fileName, key), out VoiceAudioOptions value);
                                                cue.addDialogue(dia, new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));

                                            }
                                            else
                                            {
                                                cue.addDialogue(dia, new VoiceAudioOptions());
                                                AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, fileName, key), new VoiceAudioOptions());
                                            }
                                        }
                                        continue;
                                    }
                                    continue;
                                }

                                if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, fileName, key)))
                                {
                                    AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, fileName, key), out VoiceAudioOptions value);
                                    cue.addDialogue(str, new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));

                                }
                                else
                                {
                                    cue.addDialogue(str, new VoiceAudioOptions());
                                    AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, fileName, key), new VoiceAudioOptions());
                                }


                            }
                        }

                    }
                }

            }

            //Used to scrape Strings/Locations.yaml and Strings/StringsFromMaps.yaml
            else if (cue.name == "LocationDialogue")
            {
                foreach (var fileName in cue.stringsFileNames)
                {
                    ModMonitor.Log("    Scraping dialogue file: " + fileName, LogLevel.Info);
                    //basically this will never run but can be used below to also add in dialogue.
                    if (!String.IsNullOrEmpty(fileName))
                    {
                        string dialoguePath2 = Path.Combine(stringsPath, fileName);
                        var DialogueDict = ModHelper.Content.Load<Dictionary<string, string>>(dialoguePath2, ContentSource.GameContent);

                        //Scraping the CookingChannel dialogue
                        //Scrape the whole dictionary looking for the character's name.
                        foreach (KeyValuePair<string, string> pair in DialogueDict)
                        {
                            //Get the key in the dictionary
                            string key = pair.Key;
                            string rawDialogue = pair.Value;
                            //If the key contains the character's name.

                            List<string> cleanDialogues = new List<string>();
                            cleanDialogues = sanitizeDialogueFromDictionaries(rawDialogue, cue);
                            foreach (var str in cleanDialogues)
                            {
                                if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, fileName, key)))
                                {
                                    AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, fileName, key), out VoiceAudioOptions value);
                                    cue.addDialogue(str, new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));

                                }
                                else
                                {
                                    cue.addDialogue(str, new VoiceAudioOptions());
                                    AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, fileName, key), new VoiceAudioOptions());
                                }
                            }

                        }
                    }
                }

            }

            //Scrape for event dialogue.
            else if (cue.name == "Events")
            {
                foreach (var fileName in cue.stringsFileNames)
                {
                    ModMonitor.Log("    Scraping dialogue file: " + fileName, LogLevel.Info);
                    if (!String.IsNullOrEmpty(fileName))
                    {
                        string dialoguePath2 = Path.Combine(stringsPath, fileName);
                        var DialogueDict = ModHelper.Content.Load<Dictionary<string, string>>(dialoguePath2, ContentSource.GameContent);

                        //Scrape Strings/Events.yaml for dialogue strings
                        if (fileName.Contains("StringsFromCSFiles"))
                        {
                            //Scrape Strings/StringsFromCSFiles.yaml
                            foreach (KeyValuePair<string, string> pair in DialogueDict)
                            {
                                //Get the key in the dictionary
                                string key = pair.Key;
                                string rawDialogue = pair.Value;
                                //If the key contains the character's name.
                                if (!key.Contains("Event")) continue;
                                List<string> cleanDialogues = new List<string>();
                                cleanDialogues = sanitizeDialogueFromDictionaries(rawDialogue, cue);
                                foreach (var str in cleanDialogues)
                                {
                                    if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, fileName, key)))
                                    {
                                        AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, fileName, key), out VoiceAudioOptions value);
                                        cue.addDialogue(str, new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));

                                    }
                                    else
                                    {
                                        cue.addDialogue(str, new VoiceAudioOptions());
                                        AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, fileName, key), new VoiceAudioOptions());
                                    }
                                }
                            }
                        }
                        //Scrape Strings/Events.yaml
                        if (fileName.Contains("Events"))
                        {
                            foreach (KeyValuePair<string, string> pair in DialogueDict)
                            {
                                //Get the key in the dictionary
                                string key = pair.Key;
                                string rawDialogue = pair.Value;
                                //If the key contains the character's name.
                                List<string> cleanDialogues = new List<string>();
                                cleanDialogues = sanitizeDialogueFromDictionaries(rawDialogue, cue);
                                foreach (var str in cleanDialogues)
                                {
                                    if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, fileName, key)))
                                    {
                                        AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, fileName, key), out VoiceAudioOptions value);
                                        cue.addDialogue(str, new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));

                                    }
                                    else
                                    {
                                        cue.addDialogue(str, new VoiceAudioOptions());
                                        AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, fileName, key), new VoiceAudioOptions());
                                    }
                                }
                            }
                        }
                    }
                }
            }

            //Scrape for mail dialogue.
            else if (cue.name == "Mail")
            {
                foreach (var fileName in cue.dataFileNames)
                {
                    ModMonitor.Log("    Scraping dialogue file: " + fileName, LogLevel.Info);
                    //basically this will never run but can be used below to also add in dialogue.
                    if (!String.IsNullOrEmpty(fileName))
                    {
                        string dialoguePath2 = Path.Combine(dataPath, fileName);
                        var DialogueDict = ModHelper.Content.Load<Dictionary<string, string>>(dialoguePath2, ContentSource.GameContent);

                        //Scrape the whole dictionary looking for the character's name.
                        foreach (KeyValuePair<string, string> pair in DialogueDict)
                        {
                            //Get the key in the dictionary
                            string key = pair.Key;
                            string rawDialogue = pair.Value;
                            //If the key contains the character's name.

                            string str = "";
                            str = sanitizeDialogueFromMailDictionary(rawDialogue);
                            if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, fileName, key)))
                            {
                                AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, fileName, key), out VoiceAudioOptions value);
                                cue.addDialogue(str, new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));

                            }
                            else
                            {
                                cue.addDialogue(str, new VoiceAudioOptions());
                                AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, fileName, key), new VoiceAudioOptions());
                            }
                        }
                    }
                }

            }

            //Used to scrape Content/strings/Characters.yaml.
            else if (cue.name == "Characters")
            {
                foreach (var fileName in cue.stringsFileNames)
                {
                    ModMonitor.Log("    Scraping dialogue file: " + fileName, LogLevel.Info);
                    if (!String.IsNullOrEmpty(fileName))
                    {
                        string dialoguePath2 = Path.Combine(stringsPath, fileName);
                        var DialogueDict = ModHelper.Content.Load<Dictionary<string, string>>(dialoguePath2, ContentSource.GameContent);

                        if (fileName.Contains("Characters"))
                        {
                            //Scrape the whole dictionary looking for the character's name.
                            foreach (KeyValuePair<string, string> pair in DialogueDict)
                            {
                                //Get the key in the dictionary
                                string key = pair.Key;
                                string rawDialogue = pair.Value;
                                //If the key contains the character's name.

                                List<string> cleanDialogues = new List<string>();
                                cleanDialogues = sanitizeDialogueFromDictionaries(rawDialogue, cue);
                                foreach (var str in cleanDialogues)
                                {
                                    if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, fileName, key)))
                                    {
                                        AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, fileName, key), out VoiceAudioOptions value);
                                        cue.addDialogue(str, new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));

                                    }
                                    else
                                    {
                                        cue.addDialogue(str, new VoiceAudioOptions());
                                        AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, fileName, key), new VoiceAudioOptions());
                                    }
                                }

                            }
                            continue;
                        }

                        if (fileName.Contains("StringsFromCSFiles"))
                        {
                            //do nothing.....for now.....
                            //FORTUNE TELLER DIALOGUE SCRAPE GOES HERE!!!!
                        }
                    }
                }

            }

            else if (cue.name == "Notes")
            {
                //Used mainly to scrape Content/Strings/Notes.yaml
                foreach (var fileName in cue.stringsFileNames)
                {
                    ModMonitor.Log("    Scraping dialogue file: " + fileName, LogLevel.Info);
                    //basically this will never run but can be used below to also add in dialogue.
                    if (!String.IsNullOrEmpty(fileName))
                    {
                        string dialoguePath2 = Path.Combine(stringsPath, fileName);
                        var DialogueDict = ModHelper.Content.Load<Dictionary<string, string>>(dialoguePath2, ContentSource.GameContent);

                        //Scrape the whole dictionary looking for the character's name.
                        foreach (KeyValuePair<string, string> pair in DialogueDict)
                        {
                            //Get the key in the dictionary
                            string key = pair.Key;
                            string rawDialogue = pair.Value;
                            //If the key contains the character's name.

                            List<string> cleanDialogues = new List<string>();
                            cleanDialogues = sanitizeDialogueFromDictionaries(rawDialogue, cue);
                            foreach (var str in cleanDialogues)
                            {
                                if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, fileName, key)))
                                {
                                    AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, fileName, key), out VoiceAudioOptions value);
                                    cue.addDialogue(str, new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));

                                }
                                else
                                {
                                    cue.addDialogue(str, new VoiceAudioOptions());
                                    AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, fileName, key), new VoiceAudioOptions());
                                }
                            }

                        }
                        continue;
                    }
                }


                //Used mainly to scrape Content/Data/SecretNotes.yaml
                foreach (var fileName in cue.dataFileNames)
                {
                    ModMonitor.Log("    Scraping dialogue file: " + fileName, LogLevel.Info);
                    if (!String.IsNullOrEmpty(fileName))
                    {
                        string dialoguePath2 = Path.Combine(dataPath, fileName);
                        var DialogueDict = ModHelper.Content.Load<Dictionary<int, string>>(dialoguePath2, ContentSource.GameContent);

                        //Scrape the whole dictionary looking for the character's name.
                        foreach (KeyValuePair<int, string> pair in DialogueDict)
                        {
                            //Get the key in the dictionary
                            int key = pair.Key;
                            string rawDialogue = pair.Value;
                            //If the key contains the character's name.

                            List<string> cleanDialogues = new List<string>();
                            cleanDialogues = sanitizeDialogueFromDictionaries(rawDialogue, cue);
                            foreach (var str in cleanDialogues)
                            {
                                if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, fileName, key.ToString())))
                                {
                                    AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, fileName, key.ToString()), out VoiceAudioOptions value);
                                    cue.addDialogue(str, new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));

                                }
                                else
                                {
                                    cue.addDialogue(str, new VoiceAudioOptions());
                                    AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, fileName, key.ToString()), new VoiceAudioOptions());
                                }
                            }

                        }
                        continue;

                    }
                }

            }

            //used to scrape Content/Strings/Utility.yaml
            else if (cue.name == "Utility")
            {
                foreach (var fileName in cue.stringsFileNames)
                {
                    ModMonitor.Log("    Scraping dialogue file: " + fileName, LogLevel.Info);
                    //basically this will never run but can be used below to also add in dialogue.
                    if (!String.IsNullOrEmpty(fileName))
                    {
                        string dialoguePath2 = Path.Combine(stringsPath, fileName);
                        var DialogueDict = ModHelper.Content.Load<Dictionary<string, string>>(dialoguePath2, ContentSource.GameContent);

                        //Scrape the whole dictionary looking for the character's name.
                        foreach (KeyValuePair<string, string> pair in DialogueDict)
                        {
                            //Get the key in the dictionary
                            string key = pair.Key;
                            string rawDialogue = pair.Value;
                            //If the key contains the character's name.

                            List<string> cleanDialogue = new List<string>();
                            cleanDialogue = sanitizeDialogueFromDictionaries(rawDialogue, cue);

                            foreach (var str in cleanDialogue)
                            {
                                if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, fileName, key)))
                                {
                                    AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, fileName, key), out VoiceAudioOptions value);
                                    cue.addDialogue(str, new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));

                                }
                                else
                                {
                                    cue.addDialogue(str, new VoiceAudioOptions());
                                    AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, fileName, key), new VoiceAudioOptions());
                                }
                            }
                        }
                    }
                }

            }

            else if (cue.name == "Quests")
            {
                foreach (var fileName in cue.dataFileNames)
                {
                    ModMonitor.Log("    Scraping dialogue file: " + fileName, LogLevel.Info);
                    string dialoguePath2 = Path.Combine(dataPath, fileName);
                    string root = Game1.content.RootDirectory;///////USE THIS TO CHECK FOR EXISTENCE!!!!!
                    if (!File.Exists(Path.Combine(root, dialoguePath2)))
                    {
                        ModMonitor.Log("Dialogue file not found for:" + fileName + ". This might not necessarily be a mistake just a safety check.");
                        continue; //If the file is not found for some reason...
                    }
                    var DialogueDict = ModHelper.Content.Load<Dictionary<int, string>>(dialoguePath2, ContentSource.GameContent);
                    //Scrape the whole dictionary looking for the character's name.
                    foreach (KeyValuePair<int, string> pair in DialogueDict)
                    {
                        //Get the key in the dictionary
                        string key = pair.Key.ToString();
                        string rawDialogue = pair.Value;
                        //If the key contains the character's name.

                        int count = rawDialogue.Split('/').Length - 1;
                        string[] strippedRawQuestDialogue = new string[count];
                        List<string> strippedFreshQuestDialogue = new List<string>();
                        strippedRawQuestDialogue = rawDialogue.Split('/');
                        string prompt = strippedRawQuestDialogue.ElementAt(2);
                        string response = strippedRawQuestDialogue.ElementAt(strippedRawQuestDialogue.Length - 1);

                        strippedFreshQuestDialogue.Add(prompt);
                        if (response != "true" && response != "false")
                        {
                            strippedFreshQuestDialogue.Add(response);
                        }

                        List<string> cleanDialogues = new List<string>();
                        foreach (var dia in strippedFreshQuestDialogue)
                        {
                            cleanDialogues = sanitizeDialogueFromDictionaries(dia, cue);
                            foreach (var str in cleanDialogues)
                            {
                                if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, fileName, key)))
                                {
                                    AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, fileName, key), out VoiceAudioOptions value);
                                    cue.addDialogue(str, new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));

                                }
                                else
                                {
                                    cue.addDialogue(str, new VoiceAudioOptions());
                                    AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, fileName, key), new VoiceAudioOptions());
                                }
                            }
                        }
                    }
                    continue;


                }
            }

            //ADD THIS TO THE ACTUAL NPC????
            else if (cue.name == "NPCGiftTastes")
            {
                foreach (var fileName in cue.dataFileNames)
                {
                    ModMonitor.Log("    Scraping dialogue file: " + fileName, LogLevel.Info);
                    string dialoguePath2 = Path.Combine(dataPath, fileName);
                    string root = Game1.content.RootDirectory;///////USE THIS TO CHECK FOR EXISTENCE!!!!!
                    if (!File.Exists(Path.Combine(root, dialoguePath2)))
                    {
                        ModMonitor.Log("Dialogue file not found for:" + fileName + ". This might not necessarily be a mistake just a safety check.");
                        continue; //If the file is not found for some reason...
                    }
                    var DialogueDict = ModHelper.Content.Load<Dictionary<string, string>>(dialoguePath2, ContentSource.GameContent);
                    //Scrape the whole dictionary looking for the character's name.

                    List<string> ignoreKeys = new List<string>();
                    ignoreKeys.Add("Universal_Love");
                    ignoreKeys.Add("Universal_Like");
                    ignoreKeys.Add("Universal_Neutral");
                    ignoreKeys.Add("Universal_Dislike");
                    ignoreKeys.Add("Universal_Hate");

                    foreach (KeyValuePair<string, string> pair in DialogueDict)
                    {
                        //Get the key in the dictionary
                        string key = pair.Key;
                        string rawDialogue = pair.Value;

                        //Check to see if I need to ignore this key in my dictionary I am scaping.
                        bool ignore = false;
                        foreach (var value in ignoreKeys)
                        {
                            if (key == value)
                            {
                                ignore = true;
                                break;
                            }
                        }

                        if (ignore) continue;

                        string[] strippedRawQuestDialogue = new string[20];
                        List<string> strippedFreshQuestDialogue = new List<string>();
                        strippedRawQuestDialogue = rawDialogue.Split(new string[] { "/" }, StringSplitOptions.None);


                        string prompt1 = strippedRawQuestDialogue.ElementAt(0);
                        string prompt2 = strippedRawQuestDialogue.ElementAt(2);
                        string prompt3 = strippedRawQuestDialogue.ElementAt(4);
                        string prompt4 = strippedRawQuestDialogue.ElementAt(6);
                        string prompt5 = strippedRawQuestDialogue.ElementAt(8);

                        strippedFreshQuestDialogue.Add(prompt1);
                        strippedFreshQuestDialogue.Add(prompt2);
                        strippedFreshQuestDialogue.Add(prompt3);
                        strippedFreshQuestDialogue.Add(prompt4);
                        strippedFreshQuestDialogue.Add(prompt5);

                        List<string> cleanDialogues = new List<string>();
                        foreach (var dia in strippedFreshQuestDialogue)
                        {
                            cleanDialogues = sanitizeDialogueFromDictionaries(dia, cue);
                            foreach (var str in cleanDialogues)
                            {
                                if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, fileName, key)))
                                {
                                    AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, fileName, key), out VoiceAudioOptions value);
                                    cue.addDialogue(str, new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));

                                }
                                else
                                {
                                    cue.addDialogue(str, new VoiceAudioOptions());
                                    AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, fileName, key), new VoiceAudioOptions());
                                }
                            }
                        }
                    }
                    continue;
                }
            }

            else if (cue.name == "SpeechBubbles")
            {
                foreach (var fileName in cue.stringsFileNames)
                {
                    ModMonitor.Log("    Scraping dialogue file: " + fileName, LogLevel.Info);
                    string dialoguePath2 = Path.Combine(stringsPath, fileName);
                    string root = Game1.content.RootDirectory;///////USE THIS TO CHECK FOR EXISTENCE!!!!!
                    if (!File.Exists(Path.Combine(root, dialoguePath2)))
                    {
                        ModMonitor.Log("Dialogue file not found for:" + fileName + ". This might not necessarily be a mistake just a safety check.");
                        continue; //If the file is not found for some reason...
                    }
                    var DialogueDict = ModHelper.Content.Load<Dictionary<string, string>>(dialoguePath2, ContentSource.GameContent);
                    //Scrape the whole dictionary looking for the character's name.

                    foreach (KeyValuePair<string, string> pair in DialogueDict)
                    {
                        //Get the key in the dictionary
                        string key = pair.Key;
                        string rawDialogue = pair.Value;
                        string str = sanitizeDialogueFromSpeechBubblesDictionary(rawDialogue);
                        if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, fileName, key)))
                        {
                            AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, fileName, key), out VoiceAudioOptions value);
                            cue.addDialogue(str, new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));

                        }
                        else
                        {
                            cue.addDialogue(str, new VoiceAudioOptions());
                            AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, fileName, key), new VoiceAudioOptions());
                        }
                    }
                    continue;
                }

                string basePath = ModHelper.DirectoryPath;
                string contentPath = Path.Combine(basePath, "Content");
                string audioPath = Path.Combine(contentPath, "Audio");
                string voicePath = Path.Combine(audioPath, "VoiceFiles");


                string[] dirs = Directory.GetDirectories(translation);
                //Some additional scraping to put together better options for speech bubbles.
                foreach (var v in dirs)
                {
                    string name = Path.GetFileName(v);

                    string fileName = "StringsFromCSFiles";
                    string key = "NPC.cs.4068";
                    string str = config.translationInfo.LoadString(Path.Combine("Strings", "StringsFromCSFiles:NPC.cs.4068"), translation, (object)name);
                    if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, fileName, key)))
                    {
                        AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, fileName, key), out VoiceAudioOptions value);
                        cue.addDialogue(str, new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));

                    }
                    else
                    {
                        cue.addDialogue(str, new VoiceAudioOptions());
                        AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, fileName, key), new VoiceAudioOptions());
                    }

                    key = "NPC.cs.4065";
                    str = config.translationInfo.LoadString(Path.Combine("Strings", "StringsFromCSFiles:NPC.cs.4065"), translation) + ", " + config.translationInfo.LoadString(Path.Combine("Strings", "StringsFromCSFiles:NPC.cs.4066"), translation, (object)name);
                    if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, fileName, key)))

                    {
                        AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, fileName, key), out VoiceAudioOptions value);
                        cue.addDialogue(str, new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));

                    }
                    else
                    {
                        cue.addDialogue(str, new VoiceAudioOptions());
                        AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, fileName, key), new VoiceAudioOptions());
                    }

                    key = "NPC.cs.4071";
                    str = config.translationInfo.LoadString(Path.Combine("Strings", "StringsFromCSFiles:NPC.cs.4071"), translation, (object)name);

                    if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, fileName, key)))

                    {
                        AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, fileName, key), out VoiceAudioOptions value);
                        cue.addDialogue(str, new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));

                    }
                    else
                    {
                        cue.addDialogue(str, new VoiceAudioOptions());
                        AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, fileName, key), new VoiceAudioOptions());
                    }
                }

                string fileName1 = "StringsFromCSFiles";
                string str1 = config.translationInfo.LoadString(Path.Combine("Strings", "StringsFromCSFiles:NPC.cs.4060"), translation);
                string key1 = "NPC.cs.4060";

                if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, fileName1, key1)))

                {
                    AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, fileName1, key1), out VoiceAudioOptions value);
                    cue.addDialogue(str1, new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));

                }
                else
                {
                    cue.addDialogue(str1, new VoiceAudioOptions());
                    AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, fileName1, key1), new VoiceAudioOptions());
                }

                str1 = config.translationInfo.LoadString(Path.Combine("Strings", "StringsFromCSFiles:NPC.cs.4072"), translation);
                key1 = "NPC.cs.4072";

                if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, fileName1, key1)))

                {
                    AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, fileName1, key1), out VoiceAudioOptions value);
                    cue.addDialogue(str1, new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));

                }
                else
                {
                    cue.addDialogue(str1, new VoiceAudioOptions());
                    AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, fileName1, key1), new VoiceAudioOptions());
                }

                key1 = "NPC.cs.4063";
                str1 = config.translationInfo.LoadString(Path.Combine("Strings", "StringsFromCSFiles:NPC.cs.4063"), translation);
                if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, fileName1, key1)))

                {
                    AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, fileName1, key1), out VoiceAudioOptions value);
                    cue.addDialogue(str1, new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));

                }
                else
                {
                    cue.addDialogue(str1, new VoiceAudioOptions());
                    AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, fileName1, key1), new VoiceAudioOptions());
                }

                key1 = "NPC.cs.4064";
                str1 = config.translationInfo.LoadString(Path.Combine("Strings", "StringsFromCSFiles:NPC.cs.4064"), translation);

                if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, fileName1, key1)))

                {
                    AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, fileName1, key1), out VoiceAudioOptions value);
                    cue.addDialogue(str1, new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));

                }
                else
                {
                    cue.addDialogue(str1, new VoiceAudioOptions());
                    AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, fileName1, key1), new VoiceAudioOptions());
                }

                //cue.addDialogue("Hey, it's farmer, " + replacementStrings.farmerName,new VoiceAudioOptions());

                key1 = "NPC.cs.4062";
                str1 = config.translationInfo.LoadString(Path.Combine("Strings", "StringsFromCSFiles:NPC.cs.4062"), translation);


                if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, fileName1, key1)))
                {
                    AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, fileName1, key1), out VoiceAudioOptions value);
                    cue.addDialogue(str1, new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));

                }
                else
                {
                    cue.addDialogue(str1, new VoiceAudioOptions());
                    AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, fileName1, key1), new VoiceAudioOptions());
                }

                key1 = "NPC.cs.4061";
                str1 = config.translationInfo.LoadString(Path.Combine("Strings", "StringsFromCSFiles:NPC.cs.4061"), translation);

                if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, fileName1, key1)))
                {
                    AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, fileName1, key1), out VoiceAudioOptions value);
                    cue.addDialogue(str1, new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));

                }
                else
                {
                    cue.addDialogue(str1, new VoiceAudioOptions());
                    AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, fileName1, key1), new VoiceAudioOptions());
                }

                key1 = "NPC.cs.4060";
                str1 = config.translationInfo.LoadString(Path.Combine("Strings", "StringsFromCSFiles:NPC.cs.4060"), translation);

                if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, fileName1, key1)))
                {
                    AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, fileName1, key1), out VoiceAudioOptions value);
                    cue.addDialogue(str1, new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));

                }
                else
                {
                    cue.addDialogue(str1, new VoiceAudioOptions());
                    AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, fileName1, key1), new VoiceAudioOptions());
                }

                key1 = "NPC.cs.4059";
                str1 = config.translationInfo.LoadString(Path.Combine("Strings", "StringsFromCSFiles:NPC.cs.4059"), translation);

                if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, fileName1, key1)))
                {
                    AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, fileName1, key1), out VoiceAudioOptions value);
                    cue.addDialogue(str1, new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));

                }
                else
                {
                    cue.addDialogue(str1, new VoiceAudioOptions());
                    AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, fileName1, key1), new VoiceAudioOptions());
                }

                key1 = "NPC.cs.4058";
                str1 = config.translationInfo.LoadString(Path.Combine("Strings", "StringsFromCSFiles:NPC.cs.4058"), translation);

                if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, fileName1, key1)))
                {
                    AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, fileName1, key1), out VoiceAudioOptions value);
                    cue.addDialogue(str1, new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));

                }
                else
                {
                    cue.addDialogue(str1, new VoiceAudioOptions());
                    AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, fileName1, key1), new VoiceAudioOptions());
                }

            }

            else if (cue.name == "Temp")
            {
                Vocalization.ModMonitor.Log("Scraping dialogue file: Temp.xnb", StardewModdingAPI.LogLevel.Debug);
                //dataFileNames.Add(Path.Combine("Events", "Temp.xnb"));

                Dictionary<string, string> meh = Game1.content.Load<Dictionary<string, string>>(Path.Combine("Data", "Events", config.translationInfo.getXNBForTranslation("Temp", translation)));


                foreach (KeyValuePair<string, string> pair in meh)
                {
                    if (pair.Key == "decorate")
                    {
                        string dia = pair.Value;
                        Vocalization.ModMonitor.Log(dia);
                        string[] values = dia.Split('\"');

                        foreach (var v in values)
                        {
                            Vocalization.ModMonitor.Log(v);
                            Vocalization.ModMonitor.Log("HELLO?");
                        }

                        List<string> goodValues = new List<string>();
                        goodValues.Add(values.ElementAt(1));
                        goodValues.Add(values.ElementAt(3));
                        goodValues.Add(values.ElementAt(5));

                        foreach (var sentence in goodValues)
                        {
                            List<string> clean = Vocalization.sanitizeDialogueFromDictionaries(sentence, cue);
                            foreach (var cleanSentence in clean)
                            {
                                try
                                {
                                    if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, config.translationInfo.getXNBForTranslation("Temp", translation), pair.Key)))
                                    {
                                        AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, config.translationInfo.getXNBForTranslation("Temp", translation), pair.Key), out VoiceAudioOptions value);
                                        cue.addDialogue(cleanSentence, new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));

                                    }
                                    else
                                    {
                                        cue.addDialogue(cleanSentence, new VoiceAudioOptions());
                                        AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, config.translationInfo.getXNBForTranslation("Temp", translation), pair.Key), new VoiceAudioOptions());
                                    }
                                }
                                catch (Exception err)
                                {

                                }
                            }
                        }

                    }

                    if (pair.Key == "leave")
                    {
                        string dia = pair.Value;
                        string[] values = dia.Split('\"');
                        List<string> goodValues = new List<string>();
                        goodValues.Add(values.ElementAt(1));
                        goodValues.Add(values.ElementAt(3));
                        goodValues.Add(values.ElementAt(5));

                        foreach (var sentence in goodValues)
                        {
                            List<string> clean = Vocalization.sanitizeDialogueFromDictionaries(sentence, cue);
                            foreach (var cleanSentence in clean)
                            {
                                try
                                {
                                    if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, config.translationInfo.getXNBForTranslation("Temp", translation), pair.Key)))
                                    {
                                        AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, config.translationInfo.getXNBForTranslation("Temp", translation), pair.Key), out VoiceAudioOptions value);
                                        cue.addDialogue(cleanSentence, new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));

                                    }
                                    else
                                    {
                                        cue.addDialogue(cleanSentence, new VoiceAudioOptions());
                                        AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, config.translationInfo.getXNBForTranslation("Temp", translation), pair.Key), new VoiceAudioOptions());
                                    }
                                }
                                catch (Exception err)
                                {

                                }

                            }
                        }

                    }

                    if (pair.Key == "tooBold")
                    {
                        string dia = pair.Value;
                        string[] values = dia.Split('\"');
                        List<string> goodValues = new List<string>();
                        goodValues.Add(values.ElementAt(1));

                        foreach (var sentence in goodValues)
                        {
                            List<string> clean = Vocalization.sanitizeDialogueFromDictionaries(sentence, cue);
                            foreach (var cleanSentence in clean)
                            {
                                try
                                {
                                    if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, config.translationInfo.getXNBForTranslation("Temp", translation), pair.Key)))
                                    {
                                        AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, config.translationInfo.getXNBForTranslation("Temp", translation), pair.Key), out VoiceAudioOptions value);
                                        cue.addDialogue(cleanSentence, new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));

                                    }
                                    else
                                    {
                                        cue.addDialogue(cleanSentence, new VoiceAudioOptions());
                                        AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, config.translationInfo.getXNBForTranslation("Temp", translation), pair.Key), new VoiceAudioOptions());
                                    }
                                }
                                catch (Exception err)
                                {

                                }
                            }
                        }
                    }

                    if (pair.Key == "poppy" || pair.Key == "heavy" || pair.Key == "techno" || pair.Key == "honkytonk")
                    {
                        string dia = pair.Value;
                        string[] values = dia.Split('\"');
                        List<string> goodValues = new List<string>();
                        goodValues.Add(values.ElementAt(1));
                        goodValues.Add(values.ElementAt(3));
                        goodValues.Add(values.ElementAt(5));
                        goodValues.Add(values.ElementAt(7));
                        goodValues.Add(values.ElementAt(9));
                        goodValues.Add(values.ElementAt(11));
                        goodValues.Add(values.ElementAt(13));
                        goodValues.Add(values.ElementAt(15));
                        goodValues.Add(values.ElementAt(17));
                        goodValues.Add(values.ElementAt(19));

                        foreach (var sentence in goodValues)
                        {
                            List<string> clean = Vocalization.sanitizeDialogueFromDictionaries(sentence, cue);
                            foreach (var cleanSentence in clean)
                            {
                                try
                                {
                                    if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, config.translationInfo.getXNBForTranslation("Temp", translation), pair.Key)))
                                    {
                                        AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, config.translationInfo.getXNBForTranslation("Temp", translation), pair.Key), out VoiceAudioOptions value);
                                        cue.addDialogue(cleanSentence, new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));

                                    }
                                    else
                                    {
                                        cue.addDialogue(cleanSentence, new VoiceAudioOptions());
                                        AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, config.translationInfo.getXNBForTranslation("Temp", translation), pair.Key), new VoiceAudioOptions());
                                    }
                                }
                                catch (Exception err)
                                {

                                }
                            }
                        }
                    }
                }
            }



            //Dialogue scrape for npc specific text.
            else
            {
                foreach (var fileName in cue.dialogueFileNames)
                {
                    ModMonitor.Log("    Scraping dialogue file: " + fileName, LogLevel.Info);
                    //basically this will never run but can be used below to also add in dialogue.
                    if (!String.IsNullOrEmpty(fileName))
                    {
                        string dialoguePath2 = Path.Combine(dialoguePath, fileName);
                        string root = Game1.content.RootDirectory;///////USE THIS TO CHECK FOR EXISTENCE!!!!!
                        if (!File.Exists(Path.Combine(root, dialoguePath2)))
                        {
                            ModMonitor.Log("Dialogue file not found for:" + fileName + ". This might not necessarily be a mistake just a safety check.");
                            continue; //If the file is not found for some reason...
                        }
                        var DialogueDict = ModHelper.Content.Load<Dictionary<string, string>>(dialoguePath2, ContentSource.GameContent);

                        //Scraping the rainy dialogue file.
                        if (fileName.Contains("rainy"))
                        {
                            //Scrape the whole dictionary looking for the character's name.
                            foreach (KeyValuePair<string, string> pair in DialogueDict)
                            {
                                //Get the key in the dictionary
                                string key = pair.Key;
                                string rawDialogue = pair.Value;
                                //If the key contains the character's name.
                                if (key.Contains(cue.name))
                                {
                                    List<string> cleanDialogues = new List<string>();
                                    cleanDialogues = sanitizeDialogueFromDictionaries(rawDialogue, cue);
                                    foreach (var str in cleanDialogues)
                                    {
                                        if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, fileName, key)))
                                        {
                                            AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, fileName, key), out VoiceAudioOptions value);
                                            cue.addDialogue(str, new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));

                                        }
                                        else
                                        {
                                            cue.addDialogue(str, new VoiceAudioOptions());
                                            AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, fileName, key), new VoiceAudioOptions());
                                        }
                                    }
                                }
                            }
                            continue;
                        }

                        //Check for just my generic file
                        if (fileName.Contains("MarriageDialogue") && !fileName.Contains("MarriageDialogue" + cue.name))
                        {
                            //Scrape the whole dictionary looking for other character's names to ignore.
                            if (!replacementStrings.spouseNames.Contains(cue.name)) continue;
                            foreach (KeyValuePair<string, string> pair in DialogueDict)
                            {
                                //Get the key in the dictionary
                                string key = pair.Key;
                                string rawDialogue = pair.Value;

                                //get my current charcter's name
                                //check the current key
                                //if my key contains a different spouse's name continue the loop
                                //else sanitize it and add it to my list
                                foreach (var spouse in replacementStrings.spouseNames)
                                {
                                    if (key.Contains(spouse) && spouse != cue.name)
                                    {
                                        //If the key contains a spouse name and it is not my character's name...
                                        continue;
                                    }
                                    //If the key contains the character's name or is generic dialogue.
                                    if (key.Contains(cue.name))
                                    {
                                        List<string> cleanDialogues = new List<string>();
                                        cleanDialogues = sanitizeDialogueFromDictionaries(rawDialogue, cue);
                                        foreach (var str in cleanDialogues)
                                        {
                                            if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, fileName, key)))
                                            {
                                                AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, fileName, key), out VoiceAudioOptions value);
                                                cue.addDialogue(str, new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));

                                            }
                                            else
                                            {
                                                cue.addDialogue(str, new VoiceAudioOptions());
                                                AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, fileName, key), new VoiceAudioOptions());
                                            }
                                        }
                                    }
                                }
                            }

                        }

                        //Check for character specific marriage dialogue
                        if (fileName.Contains("MarriageDialogue" + cue.name))
                        {
                            //Scrape the whole dictionary looking for other character's names to ignore.
                            if (!replacementStrings.spouseNames.Contains(cue.name)) continue;
                            foreach (KeyValuePair<string, string> pair in DialogueDict)
                            {
                                //Get the key in the dictionary
                                string key = pair.Key;
                                string rawDialogue = pair.Value;
                                //If the key contains the character's name or is generic dialogue.
                                if (key.Contains(cue.name))
                                {
                                    List<string> cleanDialogues = new List<string>();
                                    cleanDialogues = sanitizeDialogueFromDictionaries(rawDialogue, cue);
                                    foreach (var str in cleanDialogues)
                                    {
                                        if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, fileName, key)))
                                        {
                                            AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, fileName, key), out VoiceAudioOptions value);
                                            cue.addDialogue(str, new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));

                                        }
                                        else
                                        {
                                            cue.addDialogue(str, new VoiceAudioOptions());
                                            AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, fileName, key), new VoiceAudioOptions());
                                        }
                                    }
                                }
                            }
                        }
                        foreach (KeyValuePair<string, string> pair in DialogueDict)
                        {
                            string key = pair.Key;
                            string rawDialogue = pair.Value;
                            List<string> cleanDialogues = new List<string>();
                            cleanDialogues = sanitizeDialogueFromDictionaries(rawDialogue, cue);
                            foreach (var str in cleanDialogues)
                            {
                                if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, fileName, key)))
                                {
                                    AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, fileName, key), out VoiceAudioOptions value);
                                    cue.addDialogue(str, new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));

                                }
                                else
                                {
                                    cue.addDialogue(str, new VoiceAudioOptions());
                                    AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, fileName, key), new VoiceAudioOptions());
                                }
                            }
                        }
                    }
                }
                foreach (var fileName in cue.festivalFileNames)
                {
                    ModMonitor.Log("    Scraping festival file: " + fileName, LogLevel.Info);
                    //basically this will never run but can be used below to also add in dialogue.
                    if (!String.IsNullOrEmpty(fileName))
                    {
                        string dialoguePath2 = Path.Combine(festivalPath, fileName);
                        string root = Game1.content.RootDirectory;///////USE THIS TO CHECK FOR EXISTENCE!!!!!
                        if (!File.Exists(Path.Combine(root, dialoguePath2)))
                        {
                            ModMonitor.Log("Dialogue file not found for:" + fileName + ". This might not necessarily be a mistake just a safety check.");
                            continue; //If the file is not found for some reason...
                        }
                        var DialogueDict = ModHelper.Content.Load<Dictionary<string, string>>(dialoguePath2, ContentSource.GameContent);

                        foreach (KeyValuePair<string, string> pair in DialogueDict)
                        {
                            string key = pair.Key;
                            if (key != cue.name && key != cue.name + "_spouse") continue;
                            string rawDialogue = pair.Value;
                            List<string> cleanDialogues = new List<string>();
                            cleanDialogues = sanitizeDialogueFromDictionaries(rawDialogue, cue);
                            foreach (var str in cleanDialogues)
                            {
                                if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, fileName, key)))
                                {
                                    AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, fileName, key), out VoiceAudioOptions value);
                                    cue.addDialogue(str, new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));

                                }
                                else
                                {
                                    cue.addDialogue(str, new VoiceAudioOptions());
                                    AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, fileName, key), new VoiceAudioOptions());
                                }
                            }
                        }
                    }
                }


                foreach (var fileName in cue.eventFileNames)
                {
                    ModMonitor.Log("    Scraping event file: " + fileName, LogLevel.Info);
                    //basically this will never run but can be used below to also add in dialogue.
                    if (!String.IsNullOrEmpty(fileName))
                    {
                        string dialoguePath2 = Path.Combine(eventPath, fileName);
                        string root = Game1.content.RootDirectory;///////USE THIS TO CHECK FOR EXISTENCE!!!!!
                        if (!File.Exists(Path.Combine(root, dialoguePath2)))
                        {
                            ModMonitor.Log("Dialogue file not found for:" + fileName + ". This might not necessarily be a mistake just a safety check.");
                            continue; //If the file is not found for some reason...
                        }
                        var DialogueDict = ModHelper.Content.Load<Dictionary<string, string>>(dialoguePath2, ContentSource.GameContent);

                        foreach (KeyValuePair<string, string> pair in DialogueDict)
                        {
                            string key = pair.Key;
                            string rawDialogue = pair.Value;

                            List<string> speakingLines = getEventSpeakerLines(rawDialogue, cue.name);
                            //Sanitize Event info here!

                            foreach (var line in speakingLines)
                            {
                                List<string> cleanDialogues = new List<string>();
                                cleanDialogues = sanitizeDialogueFromDictionaries(line, cue);
                                foreach (var str in cleanDialogues)
                                {
                                    if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, fileName, key)))
                                    {
                                        AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, fileName, key), out VoiceAudioOptions value);
                                        cue.addDialogue(str, new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));

                                    }
                                    else
                                    {
                                        cue.addDialogue(str, new VoiceAudioOptions());
                                        AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, fileName, key), new VoiceAudioOptions());
                                    }
                                }
                            }
                        }
                    }
                }




                foreach (var fileName in cue.dataFileNames)
                {
                    ModMonitor.Log("    Scraping dialogue file: " + fileName, LogLevel.Info);
                    string dialoguePath2 = Path.Combine(dataPath, fileName);
                    string root = Game1.content.RootDirectory;///////USE THIS TO CHECK FOR EXISTENCE!!!!!
                    if (!File.Exists(Path.Combine(root, dialoguePath2)))
                    {
                        ModMonitor.Log("Dialogue file not found for:" + fileName + ". This might not necessarily be a mistake just a safety check.");
                        continue; //If the file is not found for some reason...
                    }

                    var DialogueDict = ModHelper.Content.Load<Dictionary<string, string>>(dialoguePath2, ContentSource.GameContent);

                    //Load in engagement dialogue for this npc.
                    if (fileName.Contains("EngagementDialogue"))
                    {
                        //Scrape the whole dictionary looking for the character's name.
                        foreach (KeyValuePair<string, string> pair in DialogueDict)
                        {
                            //Get the key in the dictionary
                            string key = pair.Key;
                            string rawDialogue = pair.Value;
                            //If the key contains the character's name.
                            if (key.Contains(cue.name))
                            {
                                List<string> cleanDialogues = new List<string>();
                                cleanDialogues = sanitizeDialogueFromDictionaries(rawDialogue, cue);
                                foreach (var str in cleanDialogues)
                                {
                                    if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, fileName, key)))
                                    {
                                        AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, fileName, key), out VoiceAudioOptions value);
                                        cue.addDialogue(str, new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));

                                    }
                                    else
                                    {
                                        cue.addDialogue(str, new VoiceAudioOptions());
                                        AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, fileName, key), new VoiceAudioOptions());
                                    }
                                }
                            }
                        }
                        continue;
                    }
                }
                foreach (var fileName in cue.stringsFileNames)
                {
                    ModMonitor.Log("    Scraping dialogue file: " + fileName, LogLevel.Info);
                    string dialoguePath2 = Path.Combine(stringsPath, fileName);
                    string root = Game1.content.RootDirectory;///////USE THIS TO CHECK FOR EXISTENCE!!!!!
                    if (!File.Exists(Path.Combine(root, dialoguePath2)))
                    {
                        ModMonitor.Log("Dialogue file not found for:" + fileName + ". This might not necessarily be a mistake just a safety check.");
                        continue; //If the file is not found for some reason...
                    }
                    var DialogueDict = ModHelper.Content.Load<Dictionary<string, string>>(dialoguePath2, ContentSource.GameContent);

                    //Load in super generic dialogue for this npc. This may or may not be a good idea....
                    if (fileName.Contains("StringsFromCSFiles"))
                    {
                        //Have a list of generic dialogue that I won't scrape since I do a more specific scrape after the main scrape.
                        List<string> ignoreKeys = new List<string>();
                        ignoreKeys.Add("NPC.cs.3955");
                        ignoreKeys.Add("NPC.cs.3969");
                        ignoreKeys.Add("NPC.cs.3981");
                        ignoreKeys.Add("NPC.cs.3985");
                        ignoreKeys.Add("NPC.cs.3987");
                        ignoreKeys.Add("NPC.cs.4066");
                        ignoreKeys.Add("NPC.cs.4068");
                        ignoreKeys.Add("NPC.cs.4071");
                        ignoreKeys.Add("NPC.cs.4440");
                        ignoreKeys.Add("NPC.cs.4441");
                        ignoreKeys.Add("NPC.cs.4444");
                        ignoreKeys.Add("NPC.cs.4445");
                        ignoreKeys.Add("NPC.cs.4447");
                        ignoreKeys.Add("NPC.cs.4448");
                        ignoreKeys.Add("NPC.cs.4463");
                        ignoreKeys.Add("NPC.cs.4465");
                        ignoreKeys.Add("NPC.cs.4466");
                        ignoreKeys.Add("NPC.cs.4486");
                        //Scrape the whole dictionary looking for the character's name.
                        foreach (KeyValuePair<string, string> pair in DialogueDict)
                        {
                            //Get the key in the dictionary
                            string key = pair.Key;
                            if (ignoreKeys.Contains(key)) continue;
                            string rawDialogue = pair.Value;

                            //This helps eliminate the fortune teller dialogue from more specific npcs.
                            if (rawDialogue.Contains("{0}") && !ignoreKeys.Contains(key))
                            {
                                continue;
                            }

                            //If the key contains the character's name.
                            if (key.Contains("NPC"))
                            {
                                List<string> cleanDialogues = new List<string>();
                                cleanDialogues = sanitizeDialogueFromDictionaries(rawDialogue, cue);

                                foreach(var clean in cleanDialogues)
                                {
                                    string[] bday = clean.Split('/');

                                    foreach (var str in bday)
                                    {
                                        if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, fileName, key)))
                                        {
                                            AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, fileName, key), out VoiceAudioOptions value);
                                            cue.addDialogue(str, new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));

                                        }
                                        else
                                        {
                                            cue.addDialogue(str, new VoiceAudioOptions());
                                            AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, fileName, key), new VoiceAudioOptions());
                                        }
                                    }
                                }
                            }
                        }
                        //Scrape dialogue more specifically and replace some generic {0}'s and {1}'s


                        string fileName1 = "StringsFromCSFiles";

                        string key1 = "NPC.cs.3955";
                        string str1 = config.translationInfo.LoadString(Path.Combine("Strings", "StringsFromCSFiles:NPC.cs.3955"), translation, (object)cue.name);

                        if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, fileName1, key1)))
                        {
                            AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, fileName, key1), out VoiceAudioOptions value);
                            cue.addDialogue(str1, new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));

                        }
                        else
                        {
                            cue.addDialogue(str1, new VoiceAudioOptions());
                            AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, fileName1, key1), new VoiceAudioOptions());
                        }

                        key1 = "NPC.cs.3955";
                        str1 = config.translationInfo.LoadString(Path.Combine("Strings", "StringsFromCSFiles:NPC.cs.3969"), translation, (object)cue.name);

                        if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, fileName1, key1)))
                        {
                            AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, fileName1, key1), out VoiceAudioOptions value);
                            cue.addDialogue(str1, new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));

                        }
                        else
                        {
                            cue.addDialogue(str1, new VoiceAudioOptions());
                            AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, fileName1, key1), new VoiceAudioOptions());
                        }

                        key1 = "NPC.cs.3981";
                        str1 = config.translationInfo.LoadString(Path.Combine("Strings", "StringsFromCSFiles:NPC.cs.3981"), translation, (object)cue.name);
                        if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, fileName1, key1)))
                        {
                            AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, fileName1, key1), out VoiceAudioOptions value);
                            cue.addDialogue(str1, new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));

                        }
                        else
                        {
                            cue.addDialogue(str1, new VoiceAudioOptions());
                            AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, fileName1, key1), new VoiceAudioOptions());
                        }

                        key1 = "NPC.cs.3987";
                        str1 = config.translationInfo.LoadString(Path.Combine("Strings", "StringsFromCSFiles:NPC.cs.3987"), translation, (object)cue.name, "2");


                        if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, fileName1, key1)))
                        {
                            AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, fileName1, key1), out VoiceAudioOptions value);
                            cue.addDialogue(str1, new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));

                        }
                        else
                        {
                            cue.addDialogue(str1, new VoiceAudioOptions());
                            AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, fileName1, key1), new VoiceAudioOptions());
                        }

                        key1 = "NPC.cs.4066";
                        str1 = config.translationInfo.LoadString(Path.Combine("Strings", "StringsFromCSFiles:NPC.cs.4066"), translation, (object)replacementStrings.farmerName);

                        if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, fileName1, key1)))
                        {
                            AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, fileName1, key1), out VoiceAudioOptions value);
                            cue.addDialogue(str1, new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));

                        }
                        else
                        {
                            cue.addDialogue(str1, new VoiceAudioOptions());
                            AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, fileName1, key1), new VoiceAudioOptions());
                        }


                        key1 = "NPC.cs.4068";
                        str1 = config.translationInfo.LoadString(Path.Combine("Strings", "StringsFromCSFiles:NPC.cs.4068"), translation, (object)replacementStrings.farmerName);


                        if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, fileName1, key1)))
                        {
                            AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, fileName1, key1), out VoiceAudioOptions value);
                            cue.addDialogue(str1, new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));

                        }
                        else
                        {
                            cue.addDialogue(str1, new VoiceAudioOptions());
                            AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, fileName1, key1), new VoiceAudioOptions());
                        }


                        key1 = "NPC.cs.4071";
                        str1 = config.translationInfo.LoadString(Path.Combine("Strings", "StringsFromCSFiles:NPC.cs.4071"), translation, (object)replacementStrings.farmerName);



                        if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, fileName1, key1)))
                        {
                            AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, fileName1, key1), out VoiceAudioOptions value);
                            cue.addDialogue(str1, new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));

                        }
                        else
                        {
                            cue.addDialogue(str1, new VoiceAudioOptions());
                            AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, fileName1, key1), new VoiceAudioOptions());
                        }

                        key1 = "NPC.cs.4440";
                        str1 = sanitizeDialogueFromDictionaries(config.translationInfo.LoadString(Path.Combine("Strings", "StringsFromCSFiles:NPC.cs.4440"), translation, (object)replacementStrings.farmerName), cue).ElementAt(0);

                        if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, fileName1, key1)))
                        {
                            AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, fileName1, key1), out VoiceAudioOptions value);
                            cue.addDialogue(str1, new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));

                        }
                        else
                        {
                            cue.addDialogue(str1, new VoiceAudioOptions());
                            AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, fileName1, key1), new VoiceAudioOptions());
                        }

                        key1 = "NPC.cs.4441";
                        str1 = sanitizeDialogueFromDictionaries(config.translationInfo.LoadString(Path.Combine("Strings", "StringsFromCSFiles:NPC.cs.4441"), translation, (object)replacementStrings.farmerName), cue).ElementAt(0);

                        if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, fileName1, key1)))
                        {
                            AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, fileName1, key1), out VoiceAudioOptions value);
                            cue.addDialogue(str1, new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));

                        }
                        else
                        {
                            cue.addDialogue(str1, new VoiceAudioOptions());
                            AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, fileName1, key1), new VoiceAudioOptions());
                        }

                        key1 = "NPC.cs.4444";
                        str1 = sanitizeDialogueFromDictionaries(config.translationInfo.LoadString(Path.Combine("Strings", "StringsFromCSFiles:NPC.cs.4444"), translation, (object)replacementStrings.farmerName), cue).ElementAt(0);


                        //cue.addDialogue(sanitizeDialogueFromDictionaries(config.translationInfo.LoadString(Path.Combine("Strings","StringsFromCSFiles:NPC.cs.4444"), translation, (object)replacementStrings.farmerName), cue).ElementAt(0), new VoiceAudioOptions());

                        if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, fileName1, key1)))
                        {
                            AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, fileName1, key1), out VoiceAudioOptions value);
                            cue.addDialogue(str1, new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));

                        }
                        else
                        {
                            cue.addDialogue(str1, new VoiceAudioOptions());
                            AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, fileName1, key1), new VoiceAudioOptions());
                        }
                        key1 = "NPC.cs.4445";
                        str1 = sanitizeDialogueFromDictionaries(config.translationInfo.LoadString(Path.Combine("Strings", "StringsFromCSFiles:NPC.cs.4445"), translation, (object)replacementStrings.farmerName), cue).ElementAt(0);


                        // cue.addDialogue(sanitizeDialogueFromDictionaries(config.translationInfo.LoadString(Path.Combine("Strings","StringsFromCSFiles:NPC.cs.4445"), translation, (object)replacementStrings.farmerName), cue).ElementAt(0), new VoiceAudioOptions());

                        if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, fileName1, key1)))
                        {
                            AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, fileName1, key1), out VoiceAudioOptions value);
                            cue.addDialogue(str1, new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));

                        }
                        else
                        {
                            cue.addDialogue(str1, new VoiceAudioOptions());
                            AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, fileName1, key1), new VoiceAudioOptions());
                        }

                        key1 = "NPC.cs.4447";
                        str1 = sanitizeDialogueFromDictionaries(config.translationInfo.LoadString(Path.Combine("Strings", "StringsFromCSFiles:NPC.cs.4447"), translation, (object)replacementStrings.farmerName), cue).ElementAt(0);


                        //cue.addDialogue(sanitizeDialogueFromDictionaries(config.translationInfo.LoadString(Path.Combine("Strings","StringsFromCSFiles:NPC.cs.4447"), translation, (object)replacementStrings.farmerName), cue).ElementAt(0), new VoiceAudioOptions());

                        if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, fileName1, key1)))
                        {
                            AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, fileName1, key1), out VoiceAudioOptions value);
                            cue.addDialogue(str1, new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));

                        }
                        else
                        {
                            cue.addDialogue(str1, new VoiceAudioOptions());
                            AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, fileName1, key1), new VoiceAudioOptions());
                        }

                        key1 = "NPC.cs.4448";
                        str1 = sanitizeDialogueFromDictionaries(config.translationInfo.LoadString(Path.Combine("Strings", "StringsFromCSFiles:NPC.cs.4448"), translation, (object)replacementStrings.farmerName), cue).ElementAt(0);


                        //cue.addDialogue(sanitizeDialogueFromDictionaries(config.translationInfo.LoadString(Path.Combine("Strings","StringsFromCSFiles:NPC.cs.4448"), translation, (object)replacementStrings.farmerName), cue).ElementAt(0), new VoiceAudioOptions());

                        if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, fileName1, key1)))
                        {
                            AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, fileName1, key1), out VoiceAudioOptions value);
                            cue.addDialogue(str1, new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));

                        }
                        else
                        {
                            cue.addDialogue(str1, new VoiceAudioOptions());
                            AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, fileName1, key1), new VoiceAudioOptions());
                        }

                        key1 = "NPC.cs.4463";
                        str1 = sanitizeDialogueFromDictionaries(config.translationInfo.LoadString(Path.Combine("Strings", "StringsFromCSFiles:NPC.cs.4463"), translation, (object)replacementStrings.petName), cue).ElementAt(0);


                        //cue.addDialogue(sanitizeDialogueFromDictionaries(config.translationInfo.LoadString(Path.Combine("Strings","StringsFromCSFiles:NPC.cs.4463"), translation, (object)replacementStrings.petName), cue).ElementAt(0), new VoiceAudioOptions());

                        if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, fileName1, key1)))
                        {
                            AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, fileName1, key1), out VoiceAudioOptions value);
                            cue.addDialogue(str1, new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));

                        }
                        else
                        {
                            cue.addDialogue(str1, new VoiceAudioOptions());
                            AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, fileName1, key1), new VoiceAudioOptions());
                        }

                        key1 = "NPC.cs.4465";
                        str1 = sanitizeDialogueFromDictionaries(config.translationInfo.LoadString(Path.Combine("Strings", "StringsFromCSFiles:NPC.cs.4465"), translation, (object)replacementStrings.farmerName), cue).ElementAt(0);

                        //cue.addDialogue(sanitizeDialogueFromDictionaries(config.translationInfo.LoadString(Path.Combine("Strings","StringsFromCSFiles:NPC.cs.4465"), translation, (object)replacementStrings.farmerName), cue).ElementAt(0), new VoiceAudioOptions());

                        if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, fileName1, key1)))
                        {
                            AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, fileName1, key1), out VoiceAudioOptions value);
                            cue.addDialogue(str1, new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));

                        }
                        else
                        {
                            cue.addDialogue(str1, new VoiceAudioOptions());
                            AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, fileName1, key1), new VoiceAudioOptions());
                        }

                        key1 = "NPC.cs.4466";
                        str1 = sanitizeDialogueFromDictionaries(config.translationInfo.LoadString(Path.Combine("Strings", "StringsFromCSFiles:NPC.cs.4466"), translation, (object)replacementStrings.farmerName), cue).ElementAt(0);

                        //cue.addDialogue(sanitizeDialogueFromDictionaries(config.translationInfo.LoadString(Path.Combine("Strings","StringsFromCSFiles:NPC.cs.4466"), translation, (object)replacementStrings.farmerName), cue).ElementAt(0), new VoiceAudioOptions());

                        if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, fileName1, key1)))
                        {
                            AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, fileName1, key1), out VoiceAudioOptions value);
                            cue.addDialogue(str1, new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));

                        }
                        else
                        {
                            cue.addDialogue(str1, new VoiceAudioOptions());
                            AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, fileName1, key1), new VoiceAudioOptions());
                        }

                        key1 = "NPC.cs.4486";
                        str1 = sanitizeDialogueFromDictionaries(config.translationInfo.LoadString(Path.Combine("Strings", "StringsFromCSFiles:NPC.cs.4486"), translation, (object)replacementStrings.farmerName), cue).ElementAt(0);

                        //cue.addDialogue(sanitizeDialogueFromDictionaries(config.translationInfo.LoadString(Path.Combine("Strings", "StringsFromCSFiles:NPC.cs.4486"), translation, (object)replacementStrings.farmerName), cue).ElementAt(0), new VoiceAudioOptions());

                        if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, fileName1, key1)))
                        {
                            AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, fileName1, key1), out VoiceAudioOptions value);
                            cue.addDialogue(str1, new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));

                        }
                        else
                        {
                            cue.addDialogue(str1, new VoiceAudioOptions());
                            AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, fileName1, key1), new VoiceAudioOptions());
                        }
                        for (int i = 4507; i <= 4523; i++)
                        {
                            if (i == 20 || i == 21)
                            {
                                continue;
                            }

                            key1 = "NPC.cs.4465";
                            str1 = sanitizeDialogueFromDictionaries(config.translationInfo.LoadString(Path.Combine("Strings", "StringsFromCSFiles:NPC.cs.4465"), translation, (object)config.translationInfo.LoadString(Path.Combine("Strings", "StringsFromCSFiles:NPC.cs.") + i.ToString(), translation)), cue).ElementAt(0);

                            if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, fileName1, key1)))
                            {
                                AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, fileName1, key1), out VoiceAudioOptions value);
                                cue.addDialogue(str1, new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));

                            }
                            else
                            {
                                cue.addDialogue(str1, new VoiceAudioOptions());
                                AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, fileName1, key1), new VoiceAudioOptions());
                            }
                            key1 = "NPC.cs.4466";
                            str1 = sanitizeDialogueFromDictionaries(config.translationInfo.LoadString(Path.Combine("Strings", "StringsFromCSFiles:NPC.cs.4466"), translation, (object)config.translationInfo.LoadString(Path.Combine("Strings", "StringsFromCSFiles:NPC.cs.") + i.ToString(), translation)), cue).ElementAt(0);

                            if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, fileName1, key1)))
                            {
                                AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, fileName1, key1), out VoiceAudioOptions value);
                                cue.addDialogue(str1, new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));

                            }
                            else
                            {
                                cue.addDialogue(str1, new VoiceAudioOptions());
                                AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, fileName1, key1), new VoiceAudioOptions());
                            }
                        }


                        //DO PARSE LOGIC HERE   
                        //cue.addDialogue(config.translationInfo.LoadString("Strings\\StringsFromCSFiles:NPC.cs.3985", (object)cue.name), new VoiceAudioOptions());

                        string basePath = ModHelper.DirectoryPath;
                        string contentPath = Path.Combine(basePath, "Content");
                        string audioPath = Path.Combine(contentPath, "Audio");
                        string voicePath = Path.Combine(audioPath, "VoiceFiles");

                        string[] dirs = Directory.GetDirectories(translation);
                        //Some additional scraping to put together better options for speech bubbles.
                        foreach (var v in dirs)
                        {

                            string name = Path.GetFileName(v);
                            string key = "NPC.cs.3985";

                            string str2 = sanitizeDialogueFromDictionaries(config.translationInfo.LoadString(Path.Combine("Strings", "StringsFromCSFiles:NPC.cs.3985"), translation, (object)name), cue).ElementAt(0);
                            if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, fileName, key)))
                            {
                                AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, fileName, key), out VoiceAudioOptions value);
                                cue.addDialogue(str2, new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));

                            }
                            else
                            {
                                cue.addDialogue(str2, new VoiceAudioOptions());
                                AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, fileName, key), new VoiceAudioOptions());
                            }
                        }

                        


                        continue;
                    }

                    if (fileName.Contains(cue.name))
                    {
                        dialoguePath2 = Path.Combine(stringsPath, "schedules", fileName);
                        root = Game1.content.RootDirectory;///////USE THIS TO CHECK FOR EXISTENCE!!!!!
                        if (!File.Exists(Path.Combine(root, dialoguePath2)))
                        {
                            ModMonitor.Log("Dialogue file not found for:" + fileName + ". This might not necessarily be a mistake just a safety check.");
                            continue; //If the file is not found for some reason...
                        }
                        DialogueDict = ModHelper.Content.Load<Dictionary<string, string>>(dialoguePath2, ContentSource.GameContent);
                        //Scrape the whole dictionary looking for the character's name.
                        foreach (KeyValuePair<string, string> pair in DialogueDict)
                        {
                            //Get the key in the dictionary
                            string key = pair.Key;
                            string rawDialogue = pair.Value;
                            //If the key contains the character's name.
                            List<string> cleanDialogues = new List<string>();
                            cleanDialogues = sanitizeDialogueFromDictionaries(rawDialogue, cue);
                            foreach (var str in cleanDialogues)
                            {
                                if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, fileName, key)))
                                {
                                    AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, fileName, key), out VoiceAudioOptions value);
                                    cue.addDialogue(str, new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));

                                }
                                else
                                {
                                    cue.addDialogue(str, new VoiceAudioOptions());
                                    AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, fileName, key), new VoiceAudioOptions());
                                }
                            }

                        }
                        continue;
                    }
                }


                //LOad item dictionary, pass in item and npc, sanitize the output string using the sanitizationDictionary function, and add in the cue!
                Dictionary<int, string> objDict = Game1.content.Load<Dictionary<int, string>>(Path.Combine("Data", config.translationInfo.getXNBForTranslation("ObjectInformation", translation)));
                //ModMonitor.Log("LOAD THE OBJECT INFO: ", LogLevel.Alert);
                foreach (KeyValuePair<int, string> pair in objDict)
                {
                    for (int i = 0; i <= 3; i++)
                    {
                        StardewValley.Object obj = new StardewValley.Object(pair.Key, 1, false, -1, i);

                        string[] strArray = config.translationInfo.LoadString(Path.Combine("Strings", "Lexicon:GenericPlayerTerm"), translation).Split('^');
                        string str2 = strArray[0];
                        if (strArray.Length > 1 && !(bool)((NetFieldBase<bool, NetBool>)Game1.player.isMale))
                            str2 = strArray[1];
                        string str3 = Game1.player.Name;

                        List<string> rawScrape = getPurchasedItemDialogueForNPC(obj, cue.name, str3, translation);

                        foreach (string raw in rawScrape)
                        {
                            List<string> cleanDialogues = sanitizeDialogueFromDictionaries(raw, cue);
                            foreach (var str in cleanDialogues)
                            {
                                string fileName = config.translationInfo.getXNBForTranslation("ObjectInformation", translation);
                                if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, fileName, pair.Key.ToString())))
                                {
                                    AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, fileName, pair.Key.ToString()), out VoiceAudioOptions value);
                                    cue.addDialogue(str, new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));

                                }
                                else
                                {
                                    cue.addDialogue(str, new VoiceAudioOptions());
                                    AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, fileName, pair.Key.ToString()), new VoiceAudioOptions());
                                }
                            }
                        }

                        str3 = str2;
                        List<string> rawScrape2 = getPurchasedItemDialogueForNPC(obj, cue.name, str3, translation);
                        foreach (string raw in rawScrape2)
                        {
                            List<string> cleanDialogues2 = sanitizeDialogueFromDictionaries(raw, cue);
                            foreach (var str in cleanDialogues2)
                            {
                                if (AudioCues.getWavFileReferences(translation).ContainsKey(AudioCues.generateKey(translation, cue.name, "StringsFromCSFiles", pair.Key.ToString())))
                                {
                                    AudioCues.getWavFileReferences(translation).TryGetValue(AudioCues.generateKey(translation, cue.name, "StringsFromCSFiles", pair.Key.ToString()), out VoiceAudioOptions value);
                                    cue.addDialogue(str, new VoiceAudioOptions(value.simple, value.full, value.heartEvents, value.simpleAndHeartEvents));

                                }
                                else
                                {
                                    cue.addDialogue(str, new VoiceAudioOptions());
                                    AudioCues.addWavReference(AudioCues.generateKey(translation, cue.name, "StringsFromCSFiles", pair.Key.ToString()), new VoiceAudioOptions());
                                }
                            }
                        }
                    }
                }
            }

            ModHelper.WriteJsonFile<CharacterVoiceCue>(path, cue);
            //DialogueCues.Add(cue.name, cue);
        }

        public static List<string> getEventSpeakerLines(string rawDialogue, string speakerName)
        {
            string[] dialogueSplit = rawDialogue.Split('/');
            List<string> speakingData = new List<string>();
            foreach (var dia in dialogueSplit)
            {
                //ModMonitor.Log(dia);
                if (!dia.Contains("speak") && !dia.Contains("textAboveHead")) continue;
                string[] actualDialogue = dia.Split(new string[] { "\"" }, StringSplitOptions.None);
                //Check to make sure this is the speaker's line.
                if (!actualDialogue[0].Contains(speakerName)) continue;
                //ModMonitor.Log(actualDialogue[1],LogLevel.Alert);
                //Get the actual dialogue line from this npc.
                speakingData.Add(actualDialogue[1]);
            }


            return speakingData;
        }


        /// <summary>
        /// Function taken from game code to satisfy all dialogue options.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="npcName"></param>
        /// <returns></returns>
        public static List<string> getPurchasedItemDialogueForNPC(StardewValley.Object i, string npcName, string str3, string translation)
        {
            NPC n = Game1.getCharacterFromName(npcName);
            if (n == null) return new List<string>();
            List<string> dialogueReturn = new List<string>();

            if (n.Age != 0)
                str3 = Game1.player.Name;
            string str4 = LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.en ? Lexicon.getProperArticleForWord(i.name) : "";
            if ((i.Category == -4 || i.Category == -75 || i.Category == -79) && Game1.random.NextDouble() < 0.5)
                str4 = config.translationInfo.LoadString(Path.Combine("Strings", "StringsFromCSFiles:SeedShop.cs.9701"), translation);

            for (int v = 0; v <= 5; v++)
            {
                int num = v;

                switch (num)
                {
                    case 0:
                        if (i.quality.Value == 1)
                        {
                            foreach (string str in Vocabulary.getRandomDeliciousAdjectives(translation, n))
                            {
                                string str19 = config.translationInfo.LoadString(Path.Combine("Data", "ExtraDialogue:PurchasedItem_1_QualityHigh"), translation, (object)str3, (object)str4, (object)i.DisplayName, (object)str);
                                dialogueReturn.Add(str19);
                            }
                            //break;
                        }
                        if (i.quality.Value == 0)
                        {
                            foreach (string str in Vocabulary.getRandomNegativeFoodAdjectives(translation, n))
                            {
                                string str18 = config.translationInfo.LoadString(Path.Combine("Data", "ExtraDialogue:PurchasedItem_1_QualityLow"), translation, (object)str3, (object)str4, (object)i.DisplayName, (object)str);
                                dialogueReturn.Add(str18);
                            }
                        }
                        break;
                    case 1:
                        string str2 = (i.quality.Value) != 0 ? (!n.Name.Equals("Jodi") ? config.translationInfo.LoadString(Path.Combine("Data", "ExtraDialogue:PurchasedItem_2_QualityHigh"), translation, (object)str3, (object)str4, (object)i.DisplayName) : config.translationInfo.LoadString(Path.Combine("Data", "ExtraDialogue:PurchasedItem_2_QualityHigh_Jodi"), translation, (object)str3, (object)str4, (object)i.DisplayName)) : config.translationInfo.LoadString(Path.Combine("Data", "ExtraDialogue:PurchasedItem_2_QualityLow"), translation, (object)str3, (object)str4, (object)i.DisplayName);
                        dialogueReturn.Add(str2);
                        break;
                    case 2:
                        if (n.Manners == 2)
                        {
                            if (i.quality.Value != 2)
                            {
                                foreach (var word1 in Vocabulary.getRandomNegativeFoodAdjectives(translation, n))
                                {
                                    foreach (string word2 in Vocabulary.getRandomNegativeItemSlanderNouns(translation))
                                    {
                                        string str17 = config.translationInfo.LoadString(Path.Combine("Data", "ExtraDialogue:PurchasedItem_3_QualityLow_Rude"), translation, (object)str3, (object)str4, (object)i.DisplayName, (object)(i.salePrice() / 2), (object)word1, (object)word2);
                                        dialogueReturn.Add(str17);
                                    }
                                }
                                break;
                            }
                            foreach (string word1 in Vocabulary.getRandomSlightlyPositiveAdjectivesForEdibleNoun(translation))
                            {
                                string str10 = config.translationInfo.LoadString(Path.Combine("Data", "ExtraDialogue:PurchasedItem_3_QualityHigh_Rude"), translation, (object)str3, (object)str4, (object)i.DisplayName, (object)(i.salePrice() / 2), (object)word1);
                                dialogueReturn.Add(str10);
                            }
                            break;
                        }
                        string str11 = config.translationInfo.LoadString(Path.Combine("Data", "ExtraDialogue:PurchasedItem_3_NonRude"), translation, (object)str3, (object)str4, (object)i.DisplayName, (object)(i.salePrice() / 2));
                        dialogueReturn.Add(str11);
                        break;
                    case 3:
                        string str12 = config.translationInfo.LoadString(Path.Combine("Data", "ExtraDialogue:PurchasedItem_4"), translation, (object)str3, (object)str4, (object)i.DisplayName);
                        dialogueReturn.Add(str12);
                        break;
                    case 4:
                        if (i.Category == -75 || i.Category == -79)
                        {
                            string str13 = config.translationInfo.LoadString(Path.Combine("Data", "ExtraDialogue:PurchasedItem_5_VegetableOrFruit"), translation, (object)str3, (object)str4, (object)i.DisplayName);
                            dialogueReturn.Add(str13);
                            break;
                        }
                        if (i.Category == -7)
                        {
                            foreach (string forEventOrPerson in Vocabulary.getRandomPositiveAdjectivesForEventOrPerson(translation))
                            {
                                string str14 = config.translationInfo.LoadString(Path.Combine("Data", "ExtraDialogue:PurchasedItem_5_Cooking"), translation, (object)str3, (object)str4, (object)i.DisplayName, (object)Lexicon.getProperArticleForWord(forEventOrPerson), (object)forEventOrPerson);
                                dialogueReturn.Add(str14);
                            }
                            break;
                        }
                        string str15 = config.translationInfo.LoadString(Path.Combine("Data", "ExtraDialogue:PurchasedItem_5_Foraged"), translation, (object)str3, (object)str4, (object)i.DisplayName);
                        dialogueReturn.Add(str15);
                        break;
                }
            }
            if (n.Age == 1)
            {
                string str16 = config.translationInfo.LoadString(Path.Combine("Data", "ExtraDialogue:PurchasedItem_Teen"), translation, (object)str3, (object)str4, (object)i.DisplayName);
                dialogueReturn.Add(str16);
            }

            string name = n.Name;
            string str1 = "";
            if (name == "Alex")
            {
                str1 = config.translationInfo.LoadString(Path.Combine("Data", "ExtraDialogue:PurchasedItem_Alex"), translation, (object)str3, (object)str4, (object)i.DisplayName);
            }
            if (name == "Caroline")
            {
                str1 = (int)((NetFieldBase<int, NetInt>)i.quality) != 0 ? config.translationInfo.LoadString(Path.Combine("Data", "ExtraDialogue:PurchasedItem_Caroline_QualityHigh"), translation, (object)str3, (object)str4, (object)i.DisplayName) : config.translationInfo.LoadString(Path.Combine("Data", "ExtraDialogue:PurchasedItem_Caroline_QualityLow"), translation, (object)str3, (object)str4, (object)i.DisplayName);
            }
            if (name == "Pierre")
            {
                str1 = (int)((NetFieldBase<int, NetInt>)i.quality) != 0 ? config.translationInfo.LoadString(Path.Combine("Data", "ExtraDialogue:PurchasedItem_Pierre_QualityHigh"), translation, (object)str3, (object)str4, (object)i.DisplayName) : config.translationInfo.LoadString(Path.Combine("Data", "ExtraDialogue:PurchasedItem_Pierre_QualityLow"), translation, (object)str3, (object)str4, (object)i.DisplayName);
            }


            if (name == "Abigail")
            {
                if (i.quality.Value == 0)
                {
                    foreach (string word1 in Vocabulary.getRandomNegativeItemSlanderNouns(translation))
                    {
                        string str12 = config.translationInfo.LoadString(Path.Combine("Data", "ExtraDialogue:PurchasedItem_Abigail_QualityLow"), translation, (object)str3, (object)str4, (object)i.DisplayName, (object)word1);
                        dialogueReturn.Add(str12);
                    }
                }
                else
                {
                    str1 = config.translationInfo.LoadString(Path.Combine("Data", "ExtraDialogue:PurchasedItem_Abigail_QualityHigh"), translation, (object)str3, (object)str4, (object)i.DisplayName);
                }

            }


            if (name == "Haley")
                str1 = config.translationInfo.LoadString(Path.Combine("Data", "ExtraDialogue:PurchasedItem_Haley"), translation, (object)str3, (object)str4, (object)i.DisplayName);
            if (name == "Elliott")
                str1 = config.translationInfo.LoadString(Path.Combine("Data", "ExtraDialogue:PurchasedItem_Elliott"), translation, (object)str3, (object)str4, (object)i.DisplayName);
            if (name == "Leah")
                str1 = config.translationInfo.LoadString(Path.Combine("Data", "ExtraDialogue:PurchasedItem_Leah"), translation, (object)str3, (object)str4, (object)i.DisplayName);
            if (str1 != "")
            {
                dialogueReturn.Add(str1);
            }
            return dialogueReturn;
        }


        /// <summary>
        /// Removes a lot of variables that would be hard to voice act from dkialogue strings such as player's name, pet names, farm names, etc.
        /// </summary>
        /// <param name="dialogue"></param>
        /// <returns></returns>
        public static string sanitizeDialogueInGame(string dialogue)
        {
            if (dialogue.Contains(Game1.player.Name))
            {
                dialogue = dialogue.Replace(Game1.player.name, replacementStrings.farmerName); //Remove player's name from dialogue.
            }

            if (Game1.player.hasPet())
            {
                if (dialogue.Contains(Game1.player.getPetName()))
                {
                    dialogue = dialogue.Replace(Game1.player.getPetName(), replacementStrings.petName);
                }
            }

            if (dialogue.Contains(Game1.player.farmName.Value))
            {
                dialogue = dialogue.Replace(Game1.player.farmName.Value, replacementStrings.farmName);
            }

            if (dialogue.Contains(Game1.player.favoriteThing.Value))
            {
                dialogue = dialogue.Replace(Game1.player.favoriteThing.Value, replacementStrings.favoriteThing);
            }

            if (dialogue.Contains(Game1.samBandName))
            {
                dialogue = dialogue.Replace(Game1.samBandName, replacementStrings.bandName);
            }

            if (dialogue.Contains(Game1.elliottBookName))
            {
                dialogue = dialogue.Replace(Game1.elliottBookName, replacementStrings.bookName);
            }

            //Sanitize children names from the dialogue.
            if (Game1.player.getChildren().Count > 0)
            {
                int count = 1;
                foreach (var child in Game1.player.getChildren())
                {
                    if (dialogue.Contains(child.Name))
                    {
                        if (count == 1)
                        {
                            dialogue = dialogue.Replace(child.Name, replacementStrings.kid1Name);
                        }
                        if (count == 2)
                        {
                            dialogue = dialogue.Replace(child.Name, replacementStrings.kid2Name);
                        }
                    }
                    count++;
                }
            }

            return dialogue;
        }

        /// <summary>
        /// Load in all dialogue.xnb files and attempt to sanitize all of the dialogue from it to help making adding dialogue easier.
        /// </summary>
        /// <param name="dialogue"></param>
        /// <returns></returns>
        public static List<string> sanitizeDialogueFromDictionaries(string dialogue, CharacterVoiceCue cue)
        {
            List<string> possibleDialogues = new List<string>();

            //remove $ symbols and their corresponding letters.

            if (dialogue.Contains("$neutral"))
            {
                dialogue = dialogue.Replace("$neutral", "");
                dialogue = dialogue.Replace("  ", " "); //Remove awkward spacing.
            }

            if (dialogue.Contains("$h"))
            {
                dialogue = dialogue.Replace("$h", "");
                dialogue = dialogue.Replace("  ", " "); //Remove awkward spacing.
            }

            if (dialogue.Contains("$b"))
            {
                dialogue = dialogue.Replace("$b", "");
                dialogue = dialogue.Replace("  ", " "); //Remove awkward spacing.
            }

            if (dialogue.Contains("$s"))
            {
                dialogue = dialogue.Replace("$s", "");
                dialogue = dialogue.Replace("  ", " "); //Remove awkward spacing.
            }

            if (dialogue.Contains("$u"))
            {
                dialogue = dialogue.Replace("$u", "");
                dialogue = dialogue.Replace("  ", " "); //Remove awkward spacing.
            }

            if (dialogue.Contains("$l"))
            {
                dialogue = dialogue.Replace("$l", "");
                dialogue = dialogue.Replace("  ", " "); //Remove awkward spacing.
            }

            if (dialogue.Contains("$a"))
            {
                dialogue = dialogue.Replace("$a", "");
                dialogue = dialogue.Replace("  ", " "); //Remove awkward spacing.
            }

            if (dialogue.Contains("$q"))
            {
                dialogue = dialogue.Replace("$q", "");
                dialogue = dialogue.Replace("  ", " "); //Remove awkward spacing.
            }

            if (dialogue.Contains("$e"))
            {
                dialogue = dialogue.Replace("$e", "");
                dialogue = dialogue.Replace("  ", " "); //Remove awkward spacing.
            }


            //This is probably the worst possible way to do this but I don't have too much a choice.
            for (int i = 0; i <= 100; i++)
            {
                string combine = "";
                if (i == 1) continue;
                combine = "$" + i.ToString();
                if (dialogue.Contains(combine))
                {
                    dialogue = dialogue.Replace(combine, "");
                    dialogue = dialogue.Replace("  ", " "); //Remove awkward spacing.
                    //remove dialogue symbol.
                }
            }

            //split across % symbol
            //Just remove the %symbol for generic text boxes. Not for forks.
            if (dialogue.Contains("%") && dialogue.Contains("%fork") == false)
            {
                dialogue = dialogue.Replace("%", "");
            }

            if (dialogue.Contains("$fork"))
            {
                dialogue = dialogue.Replace("%fork", "");
            }


            string[] split = dialogue.Split('#');
            List<string> dialogueSplits1 = new List<string>(); //Returns an element size of 1 if # isn't found.

            foreach (var s in split)
            {
                dialogueSplits1.Add(s);
            }



            //Split across choices
            List<string> orSplit = new List<string>();

            List<string> quoteSplit = new List<string>();

            //Split across genders
            List<string> finalSplit = new List<string>();

            //split across | symbol
            foreach (var dia in dialogueSplits1)
            {
                if (dia.Contains("|")) //If I can split my string do so and add all the split strings into my orSplit list.
                {
                    List<string> tempSplits = dia.Split('|').ToList();
                    foreach (var v in tempSplits)
                    {
                        orSplit.Add(v);
                    }
                }
                else
                {
                    orSplit.Add(dia); //If I can't split the list just add the dialogue and keep processing.
                }
            }

            foreach (var dia in orSplit)
            {
                if (dia.Contains("\"") && cue.name.StartsWith("Temp")) //If I can split my string do so and add all the split strings into my orSplit list.
                {
                    List<string> tempSplits = dia.Split('\"').ToList();
                    foreach (var v in tempSplits)
                    {
                        quoteSplit.Add(v);
                    }
                }
                else
                {
                    quoteSplit.Add(dia); //If I can't split the list just add the dialogue and keep processing.
                }
            }

            //split across ^ symbol   
            foreach (var dia in quoteSplit)
            {
                if (dia.Contains("^")) //If I can split my string do so and add all the split strings into my orSplit list.
                {
                    List<string> tempSplits = dia.Split('^').ToList();
                    foreach (var v in tempSplits)
                    {
                        finalSplit.Add(v);
                    }
                }
                else
                {
                    finalSplit.Add(dia); //If I can't split the list just add the dialogue and keep processing.
                }
            }


            //Loop through all adjectives and add them to our list of possibilities.
            for (int i = 0; i < finalSplit.Count(); i++)
            {
                string dia = finalSplit.ElementAt(i);
                if (dia.Contains("%adj"))
                {
                    foreach (var adj in replacementStrings.adjStrings)
                    {
                        dia = dia.Replace("%adj", adj);
                        finalSplit.Add(dia);
                    }
                }
            }

            //Loop through all nouns and add them to our list of possibilities.
            for (int i = 0; i < finalSplit.Count(); i++)
            {
                string dia = finalSplit.ElementAt(i);
                if (dia.Contains("%noun"))
                {
                    foreach (var noun in replacementStrings.nounStrings)
                    {
                        dia = dia.Replace("%noun", noun);
                        finalSplit.Add(dia);
                    }
                }
            }

            //Loop through all places and add them to our list of possibilities.
            for (int i = 0; i < finalSplit.Count(); i++)
            {
                string dia = finalSplit.ElementAt(i);
                if (dia.Contains("%place"))
                {
                    foreach (var place in replacementStrings.placeStrings)
                    {
                        dia = dia.Replace("%place", place);
                        finalSplit.Add(dia);
                    }
                }
            }

            //Loop through all spouses and add them to our list of possibilities.
            for (int i = 0; i < finalSplit.Count(); i++)
            {
                string dia = finalSplit.ElementAt(i);
                if (dia.Contains("%spouse"))
                {
                    foreach (var spouse in replacementStrings.spouseNames)
                    {
                        dia = dia.Replace("%spouse", spouse);
                        finalSplit.Add(dia);
                    }
                }
            }


            //iterate across ll dialogues and return a list of them.
            for (int i = 0; i < finalSplit.Count(); i++)
            {
                string dia = finalSplit.ElementAt(i);

                if (dia.Contains("@"))
                {
                    //replace with farmer name.
                    dia = dia.Replace("@", replacementStrings.farmerName);
                }

                if (dia.Contains("%band"))
                {
                    //Replace with<Sam's Band Name>
                    dia = dia.Replace("%band", replacementStrings.bandName);
                }

                if (dia.Contains("%book"))
                {
                    //Replace with<Elliott's Book Name>
                    dia = dia.Replace("%book", replacementStrings.bookName);
                }

                if (dia.Contains("%rival"))
                {
                    //Replace with<Rival Name>
                    dia = dia.Replace("%rival", replacementStrings.rivalName);
                }

                if (dia.Contains("%pet"))
                {
                    //Replace with <Pet Name>
                    dia = dia.Replace("%pet", replacementStrings.petName);
                }

                if (dia.Contains("%farm"))
                {
                    dia = dia.Replace("%pet", replacementStrings.farmName);
                }

                if (dia.Contains("%favorite"))
                {
                    //Replace with <Favorite thing>
                    dia = dia.Replace("%pet", replacementStrings.favoriteThing);
                }

                if (dia.Contains("%kid1"))
                {
                    //Replace with <Kid 1's Name>
                    dia = dia.Replace("%pet", replacementStrings.kid1Name);
                }

                if (dia.Contains("%kid2"))
                {
                    //Replace with <Kid 2's Name>
                    dia = dia.Replace("%pet", replacementStrings.kid2Name);
                }

                if (dia.Contains("%time"))
                {
                    //Replace with all times of day. 600-2600.
                    for (int t = 600; t <= 2600; t += 10)
                    {
                        string time = t.ToString();
                        string diaTime = dia.Replace("%time", time);
                        possibleDialogues.Add(diaTime);
                    }
                }
                else
                {
                    possibleDialogues.Add(dia);
                }

            }

            List<string> removalList = new List<string>();
            //Clean out all dialogue commands.
            foreach (var dia in possibleDialogues)
            {
                if (dia.Contains("$r"))
                {
                    removalList.Add(dia);
                }

                if (dia.Contains("$p"))
                {
                    removalList.Add(dia);
                }

                if (dia.Contains("$b"))
                {
                    removalList.Add(dia);
                }

                if (dia.Contains("$e"))
                {
                    removalList.Add(dia);
                }

                if (dia.Contains("$d"))
                {
                    removalList.Add(dia);
                }

                if (dia.Contains("$k"))
                {
                    removalList.Add(dia);
                }
            }

            //Delete all garbage dialogues left over.
            foreach (var v in removalList)
            {
                possibleDialogues.Remove(v);
            }


            return possibleDialogues;
        }

        public static string sanitizeDialogueFromSpeechBubblesDictionary(string text)
        {
            if (text.Contains("{0}"))
            {
                text = text.Replace("{0}", replacementStrings.farmerName);
            }
            if (text.Contains("{1}"))
            {
                text = text.Replace("{1}", replacementStrings.farmName);
            }
            return text;
        }

        /// <summary>
        /// Used to remove all garbage strings from Content/Data/mail.yaml
        /// </summary>
        /// <param name="mailText"></param>
        /// <returns></returns>
        public static string sanitizeDialogueFromMailDictionary(string mailText)
        {

            List<string> texts = mailText.Split('%').ToList();

            string splicedText = texts.ElementAt(0); //The actual message of the mail minus the items stored at the end.

            if (splicedText.Contains("@"))
            {
                splicedText = splicedText.Replace("@", replacementStrings.farmerName);
            }

            if (splicedText.Contains("^"))
            {
                splicedText = splicedText.Replace("^", "");
            }

            if (splicedText.Contains("\""))
            {
                splicedText = splicedText.Replace("\"", "");
            }

            if (splicedText.Contains("+"))
            {
                splicedText = splicedText.Replace("+", "");
            }

            if (splicedText.Contains("\n"))
            {
                splicedText = splicedText.Replace("\n", "");
            }

            return splicedText;

        }
    }
}
