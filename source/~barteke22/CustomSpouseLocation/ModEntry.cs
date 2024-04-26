/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/barteke22/StardewMods
**
*************************************************/

using CustomSpouseLocation;
using GenericModConfigMenu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace StardewMods
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod, IKeyboardSubscriber
    {
        ITranslationHelper translate;
        private ModConfig config;

        private int dayStartedDelay = -1;
        private List<NPC> spouses;
        private Dictionary<string, Vector2> patioPoints;
        private FarmHouse houseCache;
        private int mode;//0= 1.5.4, 1= 1.5.4 + multiple spouses, 2= 1.5.5, 3= 1.5.5 + multiple spouses
        private Vector2 defaultTile;//only used to room size in auto
        private Vector2 sebastianFrogTile;

        //editor stuff
        private static Regex spaceRemover = new Regex(@"\s+");
        private static Regex animChecker = new Regex(@"^\d+(:(f|F))?:\d+(\.\d+)?$");

        private static Dictionary<string, NPC> allNPCs;
        private static DictionaryEditor state;

        private bool resized = true;
        private bool SelectedImpl;
        private int buttonHeld = 0;
        private float fontScale = 1.2f;
        private float lineSpacing;

        private KeybindList click = KeybindList.Parse("MouseLeft");
        private KeybindList remove = KeybindList.Parse("LeftControl + X");
        private KeybindList left = KeybindList.Parse("Left");
        private KeybindList right = KeybindList.Parse("Right");
        private KeybindList del = KeybindList.Parse("Delete");



        public override void Entry(IModHelper helper)
        {
            if (Constants.ApiVersion.IsOlderThan("3.13.0-beta")) mode = 0;
            else mode = 2;
            if (helper.ModRegistry.IsLoaded("aedenthorn.MultipleSpouses")) mode++;

            config = helper.ReadConfig<ModConfig>();
            translate = helper.Translation;

            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
            helper.Events.Display.RenderedWorld += this.OnRenderedWorld;
            helper.Events.Display.WindowResized += this.OnResized;
            helper.Events.Display.RenderedActiveMenu += GenericModConfigMenuIntegration;
        }

        private void GenericModConfigMenuIntegration(object sender, RenderedActiveMenuEventArgs e)//Generic Mod Config Menu API
        {
            Helper.Events.Display.RenderedActiveMenu -= GenericModConfigMenuIntegration;

            lineSpacing = Game1.smallFont.LineSpacing * 1.5f;
            translate = Helper.Translation;

            allNPCs = new Dictionary<string, NPC>();
            foreach (var item in Game1.content.Load<Dictionary<string, string>>("Data\\NPCDispositions").OrderBy(val => val.Value.Split('/')[5].Equals("datable", StringComparison.Ordinal) ? 0 : 1).ThenBy(val => val.Key))
            {
                NPC test = new NPC();
                test.datable.Value = item.Value.Split('/')[5].Equals("datable", StringComparison.Ordinal);
                test.Sprite = new AnimatedSprite("Characters\\" + (item.Key.Equals("Leo", StringComparison.Ordinal) ? "ParrotBoy" : item.Key), 0, 16, (item.Key.Equals("Krobus", StringComparison.Ordinal) ? 24 : 32));
                test.displayName = item.Key;
                allNPCs.Add(item.Key, test);
            }


            var GenericMC = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (GenericMC != null)
            {
                GenericMC.Register(ModManifest, () => config = new ModConfig(), () => Helper.WriteConfig(config));
                GenericMC.AddParagraph(ModManifest, () => translate.Get("GenericMC.Description"));//All of these strings are stored in the traslation files.
                GenericMC.AddParagraph(ModManifest, () => translate.Get("GenericMC.Description2"));

                try
                {
                    GenericMC.AddTextOption(ModManifest, () => config.SpritePreviewName, (string val) => config.SpritePreviewName = val,
                        () => translate.Get("GenericMC.SpritePreview"), () => translate.Get("GenericMC.SpritePreviewDesc"));

                    //auto
                    GenericMC.AddSectionTitle(ModManifest, () => "--- " + translate.Get("GenericMC.SpouseRoomAuto") + " ---");
                    GenericMC.AddTextOption(ModManifest, () => config.SpouseRoom_Auto_Blacklist, (string val) => config.SpouseRoom_Auto_Blacklist = val,
                        () => translate.Get("GenericMC.AutoBlacklist"), () => translate.Get("GenericMC.AutoBlacklistDesc"));
                    GenericMC.AddNumberOption(ModManifest, () => config.SpouseRoom_Auto_Chance, (float val) => config.SpouseRoom_Auto_Chance = (int)val,
                        () => translate.Get("GenericMC.AutoChance"), () => translate.Get("GenericMC.AutoChanceDesc"), 0f, 100f);
                    GenericMC.AddBoolOption(ModManifest, () => config.SpouseRoom_Auto_PerformanceMode, (bool val) => config.SpouseRoom_Auto_PerformanceMode = val,
                        () => translate.Get("GenericMC.AutoPerfomance"), () => translate.Get("GenericMC.AutoPerformanceDesc"));
                    GenericMC.AddTextOption(ModManifest, () => config.SpouseRoom_Auto_FurnitureChairs_UpOnly_Blacklist, (string val) => config.SpouseRoom_Auto_FurnitureChairs_UpOnly_Blacklist = val,
                        () => translate.Get("GenericMC.FurnitureChairs"), () => translate.Get("GenericMC.FurnitureChairsDesc"));
                    GenericMC.AddTextOption(ModManifest, () => config.SpouseRoom_Auto_MapChairs_DownOnly_Blacklist, (string val) => config.SpouseRoom_Auto_MapChairs_DownOnly_Blacklist = val,
                        () => translate.Get("GenericMC.MapChairs"), () => translate.Get("GenericMC.MapChairsDesc"));

                    GenericMC.AddPageLink(ModManifest, "SpouseRoomTile", () => translate.Get("GenericMC.SpouseRoomTile"), () => translate.Get("GenericMC.SpouseRoomTile"));

                    //manual
                    GenericMC.AddSectionTitle(ModManifest, () => "--- " + translate.Get("GenericMC.Manual") + " ---");

                    GenericMC.AddPageLink(ModManifest, "SpouseRoomManual", () => translate.Get("GenericMC.SpouseRoomManual"), () => translate.Get("GenericMC.SpouseRoomManual"));
                    GenericMC.AddPageLink(ModManifest, "Kitchen", () => translate.Get("GenericMC.Kitchen"), () => translate.Get("GenericMC.Kitchen"));
                    GenericMC.AddPageLink(ModManifest, "Patio", () => translate.Get("GenericMC.Patio"), () => translate.Get("GenericMC.Patio"));
                    GenericMC.AddPageLink(ModManifest, "Porch", () => translate.Get("GenericMC.Porch"), () => translate.Get("GenericMC.Porch"));

                    //pages
                    //spouse room auto tile config
                    GenericMC.AddPage(ModManifest, "SpouseRoomTile", () => translate.Get("GenericMC.SpouseRoomTile"));
                    GenericMCDictionaryEditor(GenericMC, ModManifest, "SpouseRoomTile", 0);

                    //spouse room manual config
                    GenericMC.AddPage(ModManifest, "SpouseRoomManual", () => translate.Get("GenericMC.SpouseRoomManual"));
                    GenericMCDictionaryEditor(GenericMC, ModManifest, "SpouseRoomManual", 1);

                    //kitchen config
                    GenericMC.AddPage(ModManifest, "Kitchen", () => translate.Get("GenericMC.Kitchen"));
                    GenericMCDictionaryEditor(GenericMC, ModManifest, "Kitchen", 2);

                    //patio config
                    GenericMC.AddPage(ModManifest, "Patio", () => translate.Get("GenericMC.Patio"));
                    GenericMCDictionaryEditor(GenericMC, ModManifest, "Patio", 3);

                    //porch config
                    GenericMC.AddPage(ModManifest, "Porch", () => translate.Get("GenericMC.Porch"));
                    GenericMCDictionaryEditor(GenericMC, ModManifest, "Porch", 4);


                    //dummy value validation trigger - must be the last thing, so all values are saved before validation
                    GenericMC.AddComplexOption(ModManifest, () => "", () => "", (SpriteBatch b, Vector2 pos) => { }, () => UpdateConfig(true));
                }
                catch (Exception)
                {
                    this.Monitor.Log("Error parsing config data. Please either fix your config.json, or delete it to generate a new one.", LogLevel.Error);
                }
            }
        }

        public bool Selected
        {
            get => this.SelectedImpl;
            set
            {
                if (this.SelectedImpl == value)
                    return;

                this.SelectedImpl = value;
                if (this.SelectedImpl)
                    Game1.keyboardDispatcher.Subscriber = this;
                else
                {
                    if (Game1.keyboardDispatcher.Subscriber == this)
                        Game1.keyboardDispatcher.Subscriber = null;
                }
            }
        }
        protected virtual void ReceiveInput(string str)
        {
            if (state?.dataEditing != null)
            {
                if (state.dataIndex < state.dataStrings[state.dataEditing].Length - 1) state.dataStrings[state.dataEditing] = state.dataStrings[state.dataEditing].Insert(state.dataIndex, str);
                else state.dataStrings[state.dataEditing] += str;
                state.dataIndex++;
            }
        }

        public void RecieveCommandInput(char command)
        {
            if (command == '\b' && (state?.dataIndex > 0 && state?.dataStrings[state.dataEditing].Length > 1))
            {
                Game1.playSound("tinyWhip");
                state.dataStrings[state.dataEditing] = state.dataStrings[state.dataEditing].Remove(state.dataIndex - 1, 1);
                state.dataIndex--;
            }
            else if (command == '\b' && state?.dataIndex == 1 && state?.dataStrings[state.dataEditing].Length < 2)
            {
                Game1.playSound("tinyWhip");
                state.dataStrings[state.dataEditing] = "";
                state.dataIndex = 0;
            }
        }

        public void RecieveSpecialInput(Keys key)
        {
            //
        }

        public void RecieveTextInput(char inputChar)
        {
            ReceiveInput(inputChar.ToString());
        }

        public void RecieveTextInput(string text)
        {
            ReceiveInput(text);
        }


        public void GenericMCDictionaryEditor(IGenericModConfigMenuApi GenericMC, IManifest mod, string name, int which)//GMCM Widget
        {
            DictionaryEditor state = null;
            void Draw(SpriteBatch b, Vector2 pos)
            {
                if (state == null)
                {
                    switch (which)
                    {
                        case 0:
                            Dictionary<string, List<KeyValuePair<string, Vector2>>> temp = new Dictionary<string, List<KeyValuePair<string, Vector2>>>();
                            foreach (var item in config.SpouseRoom_Auto_Facing_TileOffset)
                            {
                                temp[item.Key] = new List<KeyValuePair<string, Vector2>>() { new KeyValuePair<string, Vector2>("", item.Value) };
                            }
                            state = new DictionaryEditor(temp, which);
                            break;
                        case 1:
                            state = new DictionaryEditor(config.SpouseRoom_Manual_TileOffsets, which);
                            break;
                        case 2:
                            state = new DictionaryEditor(config.Kitchen_Manual_TileOffsets, which);
                            break;
                        case 3:
                            state = new DictionaryEditor(config.Patio_Manual_TileOffsets, which);
                            break;
                        case 4:
                            state = new DictionaryEditor(config.Porch_Manual_TileOffsets, which);
                            break;
                    }
                    resized = true;
                }

                ModEntry.state = state;

                if (state.dataEditing != null)
                {
                    if (left.JustPressed() || right.JustPressed() || del.JustPressed()) buttonHeld = -41;
                    buttonHeld++;
                    if (buttonHeld == 0 || buttonHeld == -40)
                    {
                        if (left.IsDown() && state.dataIndex > 0) state.dataIndex--;
                        else if (right.IsDown() && state.dataIndex < state.dataStrings[state.dataEditing].Length) state.dataIndex++;
                        else if (del.IsDown() && state.dataIndex < state.dataStrings[state.dataEditing].Length)
                        {
                            Game1.playSound("tinyWhip");
                            state.dataStrings[state.dataEditing] = state.dataStrings[state.dataEditing].Remove(state.dataIndex, 1);
                        }
                    }
                    else if (buttonHeld > 1) buttonHeld = -1;
                }


                if (state.boundsLeftRight.Contains(Game1.getMouseX(), Game1.getMouseY()))
                {
                    if (state.scrollState != Game1.input.GetMouseState().ScrollWheelValue)
                    {
                        int scroll = Game1.input.GetMouseState().ScrollWheelValue;

                        if (scroll > state.scrollState && state.scrollBar.Y + state.boundsTopBottom.Height < pos.Y + state.boundsTopBottom.Height) state.scrollBar.Y += lineSpacing;
                        else if (scroll < state.scrollState && state.contentBottom + state.scrollBar.Y - state.boundsTopBottom.Height > pos.Y) state.scrollBar.Y -= lineSpacing;

                        state.dataEditing = null;
                        state.scrollState = scroll;
                    }
                }
                else state.scrollState = Game1.input.GetMouseState().ScrollWheelValue;


                if (click.JustPressed())
                {
                    foreach (var button in state.hoverNames)
                    {
                        if (button.Value.Contains(Game1.getMouseX(), Game1.getMouseY()))
                        {
                            if (button.Key.StartsWith("Arrow", StringComparison.Ordinal))
                            {
                                if (button.Key.Equals("ArrowUp", StringComparison.Ordinal)) state.scrollBar.Y += lineSpacing;
                                else state.scrollBar.Y -= lineSpacing;
                                state.dataEditing = null;
                            }
                            else if (which != 0 && state.enabledNPCs.ContainsKey(button.Key))
                            {
                                int numb = int.Parse(state.dataStrings.Keys.Where(val => val.StartsWith(button.Key)).OrderBy(val => int.Parse(val.Replace(button.Key + "_", ""))).Last().Replace(button.Key + "_", "")) + 1;
                                state.enabledNPCs[button.Key].Add(button.Key + "_" + numb);
                                state.dataStrings.Add(button.Key + "_" + numb, "Down / 0, 0");
                                state.dataEditing = null;
                                break;
                            }
                            else if (which == 0 && (state.enabledNPCs.ContainsKey(button.Key))) ;//skips
                            else if (allNPCs.ContainsKey(button.Key))
                            {
                                state.enabledNPCs[button.Key] = new List<string>() { button.Key + "_0" };
                                if (which == 0) state.dataStrings.Add(button.Key + "_0", "0, 0");
                                else state.dataStrings.Add(button.Key + "_0", "Down / 0, 0");
                                state.dataEditing = null;
                                break;
                            }
                            else if (state.dataStrings.ContainsKey(button.Key))
                            {
                                state.dataEditing = button.Key;
                                float lineWidth = Game1.smallFont.MeasureString(state.dataStrings[button.Key]).X;

                                float ind = ((Game1.getMouseX() - button.Value.X - 12f) / (lineWidth * ((lineWidth > state.boundsLeftRight.Width - 190) ? 1f : 1.2f)));
                                ind = Utility.Clamp(ind * state.dataStrings[button.Key].Length, 0f, state.dataStrings[button.Key].Length);
                                state.dataIndex = (int)ind;
                                break;
                            }
                            else state.dataEditing = null;
                        }
                    }
                    Selected = state.dataEditing != null;
                }
                else if (remove.JustPressed())
                {
                    state.dataEditing = null;
                    foreach (var button in state.hoverNames)
                    {
                        if (button.Value.Contains(Game1.getMouseX(), Game1.getMouseY()))
                        {
                            if (state.enabledNPCs.ContainsKey(button.Key))
                            {
                                if (!button.Key.Equals("Default", StringComparison.Ordinal) && !button.Key.Equals("sebastianFrog", StringComparison.Ordinal))
                                //&& state.dataStrings.Keys.Where(val => val.StartsWith(button.Key, StringComparison.Ordinal)).Count() < 2)//old - delete only if 1 entry
                                {
                                    state.enabledNPCs.Remove(button.Key);//delete name if only 1 entry + delete entries
                                    foreach (var entry in state.hoverNames)
                                    {
                                        if (entry.Key.StartsWith(button.Key, StringComparison.Ordinal)) state.dataStrings.Remove(entry.Key);
                                    }
                                    break;
                                }
                            }
                            else if (state.dataStrings.ContainsKey(button.Key) && state.dataStrings.Keys.Where(val => val.StartsWith(button.Key.Split('_')[0], StringComparison.Ordinal)).Count() > 1)
                            {
                                state.dataStrings.Remove(button.Key);//otherwise delete selected entry
                                break;
                            }
                        }
                    }
                }

                //draw
                if (resized)
                {
                    state.scrollBar = pos;
                    int width = Math.Min(Game1.uiViewport.Width / 4, 400);
                    state.boundsTopBottom = new Rectangle(100, (int)pos.Y, width * 2, -300 + (int)(Math.Min(Game1.uiViewport.Height, 1300f) * 0.8f));
                    state.boundsLeftRight = new Rectangle((int)(-550 + pos.X), state.boundsTopBottom.Y, 1100, state.boundsTopBottom.Height);
                    resized = false;
                }
                Vector2 leftMargin = new Vector2(-100f - (state.boundsTopBottom.Width / 2f), 10f);

                state.hoverNames = new Dictionary<string, Rectangle>();

                //bg
                b.Draw(Game1.staminaRect, new Rectangle(state.boundsLeftRight.X - 66, state.boundsTopBottom.Y, state.boundsLeftRight.Width + 132, state.boundsTopBottom.Height + (int)lineSpacing), null, new Color(253, 186, 105), 0f, Vector2.Zero, SpriteEffects.None, 0.9f);
                //arrows
                if (state.scrollBar.Y + state.boundsTopBottom.Height < pos.Y + state.boundsTopBottom.Height)
                {
                    Rectangle arrow = new Rectangle((int)(pos.X + state.boundsTopBottom.Width / 2f + 100f), (int)pos.Y, 32, 32);
                    state.hoverNames["ArrowUp"] = arrow;
                    b.Draw(Game1.mouseCursors, arrow, new Rectangle(421, 459, 12, 12), Color.White);
                }
                if (state.contentBottom + state.scrollBar.Y - state.boundsTopBottom.Height > pos.Y)
                {
                    Rectangle arrow = new Rectangle((int)(pos.X + state.boundsTopBottom.Width / 2f + 100f), (int)pos.Y + state.boundsTopBottom.Height, 32, 32);
                    state.hoverNames["ArrowDown"] = arrow;
                    b.Draw(Game1.mouseCursors, arrow, new Rectangle(421, 472, 12, 12), Color.White);
                }
                //warning
                if (!Context.IsWorldReady) b.DrawString(Game1.smallFont, translate.Get("GenericMC.WarningMenu"), Vector2.One, Color.Red, 0f, Vector2.Zero, 1.1f, SpriteEffects.None, 1f);
                else if (!Context.IsMainPlayer) b.DrawString(Game1.smallFont, translate.Get("GenericMC.WarningCoop"), Vector2.One, Color.Red, 0f, Vector2.Zero, 1.1f, SpriteEffects.None, 1f);

                //npcs
                foreach (var entry in state.enabledNPCs.OrderBy(val => val.Key))//npcs in config
                {
                    if (entry.Key.Equals("sebastianFrog", StringComparison.Ordinal) && mode != 0 && mode != 2) continue;

                    Rectangle nameR = new Rectangle((int)(state.scrollBar + leftMargin).X, (int)(state.scrollBar + leftMargin).Y, state.boundsTopBottom.Width, (int)lineSpacing);

                    NPC current = null;
                    if (!allNPCs.TryGetValue(entry.Key, out current))
                    {
                        if (entry.Key.Equals("sebastianFrog", StringComparison.Ordinal) && allNPCs.TryGetValue("Sebastian", out current)) ;
                        else if (Game1.player.getSpouse()?.isVillager() != null) current = Game1.player.getSpouse();
                        else allNPCs.TryGetValue("Emily", out current);
                    }

                    if (!state.boundsTopBottom.Contains(state.boundsTopBottom.Width, (int)(state.scrollBar.Y + leftMargin.Y))) leftMargin.Y += lineSpacing; //out of bounds?
                    else
                    {
                        state.hoverNames[entry.Key] = nameR;
                        if (current != null) b.Draw(current.Sprite.Texture, state.scrollBar + leftMargin, new Rectangle(1, 2, 14, 16), Color.White, 0f, new Vector2(16f, 1f), 2f, SpriteEffects.None, 1f);
                        b.DrawString(Game1.smallFont, entry.Key + (current == null || current.displayName.Equals(entry.Key, StringComparison.Ordinal) ? "" : " (" + current.displayName + ")"), state.scrollBar + leftMargin, (nameR.Contains(Game1.getMouseX(), Game1.getMouseY())) ? Color.DarkSlateGray : Color.ForestGreen, 0f, Vector2.Zero, fontScale, SpriteEffects.None, 1f);
                        leftMargin.Y += lineSpacing;
                    }
                    foreach (var text in state.dataStrings)//npc's entries
                    {
                        if (text.Key.StartsWith(entry.Key, StringComparison.Ordinal))
                        {
                            if (!state.boundsTopBottom.Contains(state.boundsTopBottom.Width, (int)(state.scrollBar.Y + leftMargin.Y))) //out of bounds?
                            {
                                leftMargin.Y += lineSpacing;
                                continue;
                            }
                            nameR = new Rectangle((int)(state.scrollBar + leftMargin).X, (int)(state.scrollBar + leftMargin).Y, state.boundsLeftRight.Width, (int)lineSpacing);

                            float fontScale2 = 1.2f;
                            if (Game1.smallFont.MeasureString(text.Value).X > state.boundsLeftRight.Width - 190)
                            {
                                fontScale2 = 1f;
                                nameR.X = 20;
                                nameR.Width = Game1.uiViewport.Width - 40;
                                b.Draw(Game1.staminaRect, new Rectangle(nameR.X - 10, nameR.Y - 5, nameR.Width + 20, nameR.Height + 10), null, new Color(253, 186, 105), 0f, Vector2.Zero, SpriteEffects.None, 0.9f);
                            }


                            state.hoverNames[text.Key] = nameR;
                            Color color = Color.OrangeRed;

                            if (which == 0 && text.Value.Split(',').Length == 2 && float.TryParse(text.Value.Split(',')[0], out _) && float.TryParse(text.Value.Split(',')[1], out _)) color = Color.ForestGreen;
                            else if (which != 0)
                            {
                                int spriteIndex = TryGetSprite(text.Value);
                                if (spriteIndex != -9999)
                                {
                                    if (which != 0 && current != null)
                                    {
                                        b.Draw(current.Sprite.Texture, new Vector2(20f + nameR.X, nameR.Y), Game1.getSquareSourceRectForNonStandardTileSheet(current.Sprite.Texture, current.Sprite.SpriteWidth, current.Sprite.SpriteHeight, Math.Abs(spriteIndex)),
                                            Color.White, 0f, new Vector2(18f, 6f), 1.4f, (spriteIndex < 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 1f);
                                    }
                                    if (TryGetVector2(text.Value) != new Vector2(-9999f)) color = Color.ForestGreen;
                                }
                            }
                            if (nameR.Contains(Game1.getMouseX(), Game1.getMouseY()))
                            {
                                if (color == Color.OrangeRed) color = Color.Crimson;
                                else color = Color.DarkSlateGray;
                            }

                            b.DrawString(Game1.smallFont, text.Value, new Vector2(20f + nameR.X, nameR.Y), color, 0f, Vector2.Zero, fontScale2, SpriteEffects.None, 1f);

                            if (Selected && text.Key.Equals(state.dataEditing, StringComparison.Ordinal)) b.Draw(Game1.staminaRect, new Rectangle(nameR.X + 19
                                + (int)((Game1.smallFont.MeasureString((state.dataIndex == text.Value.Length) ? text.Value : text.Value.Remove(state.dataIndex)).X) * fontScale2), nameR.Y, 3, 32),
                                Color.Black * ((DateTime.UtcNow.Millisecond % 1000 >= 500) ? 0.3f : 1f));

                            leftMargin.Y += lineSpacing;
                        }
                    }
                }
                foreach (var npc in allNPCs)//.OrderBy(val => val.Value?.datable ? 0 : 1).ThenBy(val => val.Key))//technically Dictionary isn't ordered, but it works for now//other datable npcs
                {
                    if (!state.enabledNPCs.ContainsKey(npc.Key))
                    {
                        if (!state.boundsTopBottom.Contains(state.boundsTopBottom.Width, (int)(state.scrollBar.Y + leftMargin.Y))) //out of bounds?
                        {
                            leftMargin.Y += lineSpacing;
                            continue;
                        }
                        Rectangle nameR = new Rectangle((int)(state.scrollBar + leftMargin).X, (int)(state.scrollBar + leftMargin).Y, state.boundsTopBottom.Width, (int)lineSpacing);
                        state.hoverNames[npc.Key] = nameR;

                        Color c = (nameR.Contains(Game1.getMouseX(), Game1.getMouseY())) ? Color.Black : Color.Gray;

                        if (npc.Value?.datable)
                        {
                            if (c == Color.Gray) c = Color.DeepPink;
                            else c = Color.HotPink;
                        }

                        if (npc.Value != null) b.Draw(npc.Value.Sprite.Texture, state.scrollBar + leftMargin, new Rectangle(1, 2, 14, 16), Color.Gray, 0f, new Vector2(16f, 1f), 2f, SpriteEffects.None, 1f);
                        b.DrawString(Game1.smallFont, npc.Key + (npc.Value == null || npc.Value.displayName.Equals(npc.Key, StringComparison.Ordinal) ? "" : " (" + npc.Value.displayName + ")"), state.scrollBar + leftMargin, c, 0f, Vector2.Zero, fontScale, SpriteEffects.None, 1f);
                        leftMargin.Y += lineSpacing;
                    }
                }
                state.contentBottom = (int)leftMargin.Y;

                //ui
                b.Draw(Game1.staminaRect, new Rectangle((int)pos.X, (int)pos.Y, (int)(state.boundsTopBottom.Width * 1.4f), 1), null, Color.Black, 0f, new Vector2(0.5f), SpriteEffects.None, 1f);
                b.Draw(Game1.staminaRect, new Rectangle((int)pos.X, (int)(pos.Y + state.boundsTopBottom.Height + lineSpacing), (int)(state.boundsTopBottom.Width * 1.4f), 1), null, Color.Black, 0f, new Vector2(0.5f), SpriteEffects.None, 1f);
            }

            void Save()
            {
                if (state == null) return;
                Dictionary<string, List<KeyValuePair<string, Vector2>>> temp = new Dictionary<string, List<KeyValuePair<string, Vector2>>>();
                if (which != 0)
                {
                    foreach (var npc in state.enabledNPCs)
                    {
                        temp[npc.Key] = new List<KeyValuePair<string, Vector2>>();

                        foreach (var entry in state.dataStrings.Where(val => val.Key.StartsWith(npc.Key)))
                        {
                            int spriteIndex = TryGetSprite(entry.Value);
                            Vector2 offset = TryGetVector2(entry.Value);

                            if (spriteIndex != -9999 && offset != new Vector2(-9999f))
                            {
                                temp[npc.Key].Add(new KeyValuePair<string, Vector2>(entry.Value.Split('/')[0], offset));
                            }
                            else temp[npc.Key].Add(new KeyValuePair<string, Vector2>("Down", Vector2.Zero));
                        }
                    }
                }
                switch (which)
                {
                    case 0:
                        Dictionary<string, Vector2> temp2 = new Dictionary<string, Vector2>();
                        foreach (var item in state.enabledNPCs)
                        {
                            if (state.dataStrings[item.Key + "_0"].Split(',').Length == 2 && float.TryParse(state.dataStrings[item.Key + "_0"].Split(',')[0], out float x) && float.TryParse(state.dataStrings[item.Key + "_0"].Split(',')[1], out float y))
                            {
                                temp2[item.Key] = new Vector2(x, y);
                            }
                        }
                        config.SpouseRoom_Auto_Facing_TileOffset = temp2;
                        break;
                    case 1:
                        config.SpouseRoom_Manual_TileOffsets = temp;
                        break;
                    case 2:
                        config.Kitchen_Manual_TileOffsets = temp;
                        break;
                    case 3:
                        config.Patio_Manual_TileOffsets = temp;
                        break;
                    case 4:
                        config.Porch_Manual_TileOffsets = temp;
                        break;
                }
            }

            if (which == 0) GenericMC.AddSectionTitle(mod, () => translate.Get("GenericMC.Hover") + ": " + translate.Get("GenericMC.SpouseRoomTile"), 
                () => translate.Get("GenericMC.SpouseRoomTileDesc") + (mode == 0 || mode == 2 ? "\n" + translate.Get("GenericMC.SpouseRoomTileDesc2") : ""));
            else if (which == 1) GenericMC.AddSectionTitle(mod, () => translate.Get("GenericMC.Hover") + ": " + translate.Get("GenericMC.SpouseRoomManual"), 
                () => translate.Get("GenericMC.SpouseRoomManualDesc" + (mode == 0 || mode == 2 ? "\n" + translate.Get("GenericMC.SpouseRoomManualDesc2") : "")) + "\n\n" + translate.Get("GenericMC.SharedManualDesc"));
            else GenericMC.AddSectionTitle(mod, () => translate.Get("GenericMC.Hover") + ": " + translate.Get("GenericMC." + name), () => translate.Get("GenericMC." + name + "Desc") + "\n\n" + translate.Get("GenericMC.SharedManualDesc"));
            GenericMC.AddSectionTitle(mod, () => translate.Get("GenericMC.Hover") + ":  " + translate.Get("GenericMC.Instructions"), () => (which == 0 ? translate.Get("GenericMC.InstructionsTile") : translate.Get("GenericMC.InstructionsDesc")));
            GenericMC.AddComplexOption(mod, () => "", () => "", Draw, Save, () => 300);
        }

        private int TryGetSprite(string input)
        {
            string[] data = spaceRemover.Replace(input, "").Split('/');

            if (data.Length > 0)
            {
                if (int.TryParse(data[0], out int sprite)) return sprite;
                else if (data[0].Contains(':'))
                {
                    List<FarmerSprite.AnimationFrame> anims = TryGetAnimations(data[0]);
                    if (anims.Count > 0) //return anims[DateTime.UtcNow.Second % anims.Count].frame;
                    {
                        int currentMs = (int)Game1.currentGameTime.TotalGameTime.TotalMilliseconds % (anims.Sum(val => val.milliseconds) + (anims.Count * 25));
                        int indexMs = 0;
                        foreach (var frame in anims)
                        {
                            if (currentMs <= indexMs + frame.milliseconds + (anims.Count * 25))
                            {
                                if (frame.flip) return frame.frame * -1;
                                return frame.frame;
                            }
                            indexMs += frame.milliseconds;
                        }
                    }
                }
                else
                {
                    switch (data[0].ToLower())
                    {
                        case "up":
                            return 8;
                        case "left":
                            return 12;
                        case "right":
                            return 4;
                        case "down":
                            return 0;
                    }
                }
            }
            return -9999;
        }
        private List<FarmerSprite.AnimationFrame> TryGetAnimations(string animData)
        {
            List<FarmerSprite.AnimationFrame> anims = new List<FarmerSprite.AnimationFrame>();
            animData = spaceRemover.Replace(animData, "");
            string[] data = animData.Split(',');
            foreach (var frame in data)
            {
                if (animChecker.IsMatch(frame))
                {
                    bool flip = frame.ToLower().Contains(":f");
                    string[] frameData = frame.Split(':');

                    if (int.TryParse(frameData[0], out int f) && (float.TryParse(frameData[1], out float s) || float.TryParse(frameData[2], out s)))
                    {
                        anims.Add(new FarmerSprite.AnimationFrame(f, (int)Math.Max((s * 1000f), 1f)) { flip = flip });
                    }
                }
            }
            return anims;
        }
        private Vector2 TryGetVector2(string input)
        {
            string[] data = spaceRemover.Replace(input, "").Split('/');
            if (data.Length > 1)
            {
                data = data[1].Split(',');
                if (data.Length == 2 && float.TryParse(data[0], out float x) && float.TryParse(data[1], out float y))
                {
                    return new Vector2(x, y);
                }
            }
            return new Vector2(-9999f);
        }




        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady || !(e.Button == SButton.F5)) return; // ignore if player hasn't loaded a save yet
            UpdateConfig(false);
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            houseCache = Game1.getLocationFromName(Game1.player.homeLocation.Value) as FarmHouse;

            if (mode < 2) defaultTile = Utility.PointToVector2(houseCache.GetSpouseRoomSpot());//1.5.4
            else defaultTile = Utility.PointToVector2(Helper.Reflection.GetMethod(houseCache, "GetSpouseRoomCorner").Invoke<Point>()) + new Vector2(3f, 4f);//1.5.5
            sebastianFrogTile = defaultTile + new Vector2(-1f, 1f);

            foreach (var item in Game1.content.Load<Dictionary<string, string>>("Data\\NPCDispositions").OrderBy(val => val.Value.Split('/')[5].Equals("datable", StringComparison.Ordinal) ? 0 : 1).ThenBy(val => val.Key))
            {
                NPC npc = Game1.getCharacterFromName(item.Key);
                if (npc != null) allNPCs[item.Key] = npc;
            }
        }
        private void OnDayStarted(object sender, DayStartedEventArgs e)//MAIN METHOD
        {
            if (!Context.IsMainPlayer) return;//host settings decide - since others don't join at day start

            patioPoints = new Dictionary<string, Vector2>();//custom patio compatibility
            var CustomPatioAPI = Helper.ModRegistry.GetApi<ICustomSpousePatioApi>("aedenthorn.CustomSpousePatio");
            if (CustomPatioAPI != null)
            {
                foreach (var pair in CustomPatioAPI.GetCurrentSpouseAreas())
                {
                    if ((bool)allNPCs[pair.Key]?.isMarried() || (bool)allNPCs[pair.Key]?.isRoommate()) patioPoints[pair.Key] = Utility.PointToVector2(Helper.Reflection.GetField<Point>(pair.Value, "location").GetValue()) + Utility.PointToVector2(CustomPatioAPI.GetDefaultSpouseOffsets()[pair.Key]);
                }
            }

            UpdateConfig(false);

            spouses = new List<NPC>();

            foreach (var farmer in Game1.getAllFarmers())
            {
                foreach (string name in farmer.friendshipData.Pairs.Where(f => f.Value.IsMarried() || f.Value.IsRoommate()).Select(f => f.Key))
                {
                    if (allNPCs.ContainsKey(name)) spouses.Add(allNPCs[name]);
                }
            }
            //foreach (GameLocation loc in Game1.locations)
            //{
            //    if (ReferenceEquals(loc.GetType(), typeof(FarmHouse)) || loc is Farm)
            //    {
            //        foreach (NPC npc in (loc as FarmHouse).getCharacters())
            //        {
            //            if (npc.isMarried()) spouses.Add(npc);
            //        }
            //    }
            //}
            //    spouse.setTileLocation(new Vector2(38, 14));

            dayStartedDelay = 150;
        }


        private void PlaceSpousesInside()
        {
            foreach (NPC spouse in spouses)
            {
                if (spouse?.isVillager() != null)
                {
                    Vector2 tile = spouse.getTileLocation();
                    KeyValuePair<string, Vector2> newTile = new KeyValuePair<string, Vector2>();
                    GameLocation loc = spouse.currentLocation;
                    bool changed = false;


                    if (loc is FarmHouse || loc is Cabin)
                    {
                        if (tile == Utility.PointToVector2((loc as FarmHouse).getKitchenStandingSpot())) //kitchen
                        {
                            List<KeyValuePair<string, Vector2>> tiles = config.Kitchen_Manual_TileOffsets[(config.Kitchen_Manual_TileOffsets.ContainsKey(spouse.Name) ? spouse.Name : "Default")].FindAll(val => val.Value != new Vector2(-999f));

                            if (changed = tiles.Count > 0) newTile = tiles[Game1.random.Next(0, tiles.Count)];
                        }
                        else
                        {
                            float tileDistX = tile.X - (loc as FarmHouse).GetSpouseRoomSpot().X;//multiple spouses offset fix

                            if (mode != 1 && mode != 3 && spouse.Name.Equals("Sebastian", StringComparison.Ordinal) && Game1.netWorldState.Value.hasWorldStateID("sebastianFrog") && config.SpouseRoom_Manual_TileOffsets.ContainsKey("sebastianFrog") //SpouseRoom (Sebastian after frog)
                                //&& tile.Y == (loc as FarmHouse).GetSpouseRoomSpot().Y - 1 && tileDistX + 1 % 7 == 0)
                                && tile == sebastianFrogTile)
                            {
                                List<KeyValuePair<string, Vector2>> tiles = config.SpouseRoom_Manual_TileOffsets["sebastianFrog"].FindAll(val => val.Value != new Vector2(-999f));
                                
                                if (changed = tiles.Count > 0) newTile = tiles[Game1.random.Next(0, tiles.Count)];

                                if (RandomTile(spouse, changed, loc, tile, ref newTile)) changed = true;
                            }
                            if (!changed && tile.Y == (loc as FarmHouse).GetSpouseRoomSpot().Y && tileDistX % 7 == 0) //SpouseRoom - everything else
                            {
                                List<KeyValuePair<string, Vector2>> tiles = config.SpouseRoom_Manual_TileOffsets[(config.SpouseRoom_Manual_TileOffsets.ContainsKey(spouse.Name) ? spouse.Name : "Default")].FindAll(val => val.Value != new Vector2(-999f));

                                if (changed = tiles.Count > 0) newTile = tiles[Game1.random.Next(0, tiles.Count)];

                                if (RandomTile(spouse, changed, loc, tile, ref newTile)) changed = true;
                            }
                        }
                    }
                    if (changed) PlaceSpouse(spouse, tile, newTile);
                }
            }
            Monitor.Log("Made inside spouse(s) behave...");
        }
        private void PlaceSpousesOutside()
        {
            foreach (NPC spouse in spouses)
            {
                if (spouse?.isVillager() != null)
                {
                    Vector2 tile = spouse.getTileLocation();
                    KeyValuePair<string, Vector2> newTile = new KeyValuePair<string, Vector2>();
                    GameLocation loc = spouse.currentLocation;
                    bool changed = false;


                    if (loc is Farm)
                    {
                        FarmHouse home = spouse.getHome() as FarmHouse;
                        if (home != null && tile == Utility.PointToVector2(home.getPorchStandingSpot())) //porch
                        {
                            List<KeyValuePair<string, Vector2>> tiles = config.Porch_Manual_TileOffsets[(config.Porch_Manual_TileOffsets.ContainsKey(spouse.Name) ? spouse.Name : "Default")].FindAll(val => val.Value != new Vector2(-999f));

                            if (changed = tiles.Count > 0) newTile = tiles[Game1.random.Next(0, tiles.Count)];
                        }
                        else //patio - spouse area
                        {
                            Vector2 patio = (patioPoints.ContainsKey(spouse.Name) ? patioPoints[spouse.Name] : spouse.GetSpousePatioPosition());
                            Rectangle area = new Rectangle((int)patio.X - 2, (int)patio.Y - 2, (int)patio.X + 2, (int)patio.Y + 2);
                            if (area.Contains((int)tile.X, (int)tile.Y))
                            {
                                List<KeyValuePair<string, Vector2>> tiles = config.Patio_Manual_TileOffsets[(config.Patio_Manual_TileOffsets.ContainsKey(spouse.Name) ? spouse.Name : "Default")].FindAll(val => val.Value != new Vector2(-999f));

                                if (changed = tiles.Count > 0) newTile = tiles[Game1.random.Next(0, tiles.Count)];
                            }
                        }
                    }
                    if (changed) PlaceSpouse(spouse, tile, newTile);
                }
            }
            Monitor.Log("Made outside spouse(s) behave...");
        }
        private void PlaceSpouse(NPC spouse, Vector2 tile, KeyValuePair<string, Vector2> newTile)
        {
            spouse.Position = (tile + newTile.Value) * 64;

            if (int.TryParse(newTile.Key, out int spriteIndex)) spouse.Sprite.CurrentFrame = spriteIndex;
            else if (newTile.Key.Contains(':')) spouse.Sprite.setCurrentAnimation(TryGetAnimations(newTile.Key));
            else
            {
                switch (spaceRemover.Replace(newTile.Key, "").ToLower())
                {
                    case "tile":
                        if (mode != 1 && mode != 3 && spouse.Name.Equals("Sebastian", StringComparison.Ordinal) && Game1.netWorldState.Value.hasWorldStateID("sebastianFrog") && config.SpouseRoom_Auto_Facing_TileOffset.ContainsKey("sebastianFrog")) spouse.faceGeneralDirection((tile + config.SpouseRoom_Auto_Facing_TileOffset["sebastianFrog"]) * 64f);
                        else if (config.SpouseRoom_Auto_Facing_TileOffset.ContainsKey(spouse.Name)) spouse.faceGeneralDirection((tile + config.SpouseRoom_Auto_Facing_TileOffset[spouse.Name]) * 64f);
                        else spouse.faceGeneralDirection((tile + config.SpouseRoom_Auto_Facing_TileOffset["Default"]) * 64f);
                        break;
                    case "up":
                        spouse.faceDirection(0);
                        break;
                    case "left":
                        spouse.faceDirection(3);
                        break;
                    case "right":
                        spouse.faceDirection(1);
                        break;
                    default://down
                        spouse.faceDirection(2);
                        break;
                }
            }
        }
        private bool RandomTile(NPC spouse, bool changed, GameLocation loc, Vector2 tile, ref KeyValuePair<string, Vector2> newTile)
        {
            if (!(config.SpouseRoom_Auto_Blacklist.ToLower().Contains("all") || config.SpouseRoom_Auto_Blacklist.Contains(spouse.Name))
                && ((!changed && config.SpouseRoom_Auto_Chance > 0) || config.SpouseRoom_Auto_Chance > Game1.random.Next(0, 99)))
            {
                List<KeyValuePair<string, Vector2>> freeTiles = new List<KeyValuePair<string, Vector2>>();
                List<KeyValuePair<string, Vector2>> emergencyTiles = new List<KeyValuePair<string, Vector2>>();
                List<KeyValuePair<string, Vector2>> seatTiles = new List<KeyValuePair<string, Vector2>>();

                bool furniture = !(config.SpouseRoom_Auto_FurnitureChairs_UpOnly_Blacklist.ToLower().Contains("all") || config.SpouseRoom_Auto_FurnitureChairs_UpOnly_Blacklist.Contains(spouse.Name));
                bool mapSeat = !(config.SpouseRoom_Auto_MapChairs_DownOnly_Blacklist.ToLower().Contains("all") || config.SpouseRoom_Auto_MapChairs_DownOnly_Blacklist.Contains(spouse.Name));

                
                Rectangle room = new Rectangle((int)this.defaultTile.X - 3, (int)this.defaultTile.Y - 1, 7, 6);

                while (!room.Contains((int)tile.X, (int)tile.Y) && room.X < 500)
                {
                    room.X += 7;
                }
                Vector2 defaultTile = new Vector2(room.X + 3, room.Y + 1);

                for (int x = -3; x < 3; x++)
                {
                    for (int y = -1; y < 5; y++)
                    {
                        Point potential = new Point(x + (int)defaultTile.X, y + (int)defaultTile.Y);
                        bool occupied = false;
                        for (int i = 0; i < loc.characters.Count; i++)
                        {
                            if (loc.characters[i] != null && loc.characters[i].GetBoundingBox().Intersects(new Rectangle(potential.X * 64 + 1, potential.Y * 64 + 1, 62, 62))
                                && !loc.characters[i].Name.Equals(spouse.Name, StringComparison.Ordinal))
                            {
                                occupied = true;
                                break;
                            }
                        }
                        if (!occupied)
                        {
                            //tiles
                            if (!config.SpouseRoom_Auto_PerformanceMode && new PathFindController(spouse, loc, potential, 2).pathToEndPoint != null)
                            {
                                freeTiles.Add(new KeyValuePair<string, Vector2>("Tile", new Vector2(x, y)));
                            }
                            else if (!loc.isTileOccupiedForPlacement(new Vector2(x, y) + defaultTile) && loc.getObjectAtTile(potential.X, potential.Y) == null)
                            {
                                emergencyTiles.Add(new KeyValuePair<string, Vector2>("Tile", new Vector2(x, y)));
                            }
                            //furniture
                            else if (furniture && (loc.getObjectAtTile(potential.X, potential.Y) as ISittable)?.GetSittingDirection() == 0)
                            {
                                foreach (var seat in (loc.getObjectAtTile(potential.X, potential.Y) as ISittable).GetSeatPositions())
                                {
                                    if (Math.Abs(seat.X - potential.X) < 1f) seatTiles.Add(new KeyValuePair<string, Vector2>("Up", seat - defaultTile + new Vector2(-0.01f, -0.11f)));
                                }
                            }
                            //mapChairs
                            else if (mapSeat)
                            {
                                foreach (MapSeat seat in loc.mapSeats)
                                {
                                    if (seat.OccupiesTile(potential.X, potential.Y) && !seat.IsBlocked(loc) && seat.direction.Value == 2)
                                    {
                                        foreach (var spot in seat.GetSeatPositions())
                                        {
                                            if (Math.Abs(spot.X - potential.X) < 1f) seatTiles.Add(new KeyValuePair<string, Vector2>("Down", spot - defaultTile + new Vector2(0.05f, 0.15f)));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (freeTiles.Count + seatTiles.Count + emergencyTiles.Count > 0)
                {
                    if (seatTiles.Count > 0 && Game1.random.Next(0, 3) == 0) newTile = seatTiles[Game1.random.Next(0, seatTiles.Count)];
                    else if (freeTiles.Count > 0) newTile = freeTiles[Game1.random.Next(0, freeTiles.Count)];
                    else if (emergencyTiles.Count > 0) newTile = emergencyTiles[Game1.random.Next(0, emergencyTiles.Count)];
                    else return false;
                    return true;
                }
            }
            return false;
        }


        private void OnRenderedWorld(object sender, RenderedWorldEventArgs e)//preview mode
        {
            if (dayStartedDelay > -1)
            {
                if (dayStartedDelay == 149) PlaceSpousesInside();
                else if (dayStartedDelay < 1 && Context.IsMainPlayer)
                {
                    PlaceSpousesOutside();
                    dayStartedDelay = -1;
                }
                dayStartedDelay--;
            }
            if (spouses?.Count > 0)
            {
                foreach (var spouse in spouses.ToArray())//animation reset
                {
                    if (spouse.isMoving())
                    {
                        spouse.Sprite.StopAnimation();
                        spouses.Remove(spouse);
                    }
                    else if (Game1.timeOfDay == 820 && !spouse.currentLocation.isPointPassable(new xTile.Dimensions.Location(spouse.getTileX() * 64, spouse.getTileY() * 64), Game1.viewport) && !(spouse.currentLocation.getObjectAtTile(spouse.getTileX(), spouse.getTileY()) is Furniture))
                    {
                        spouse.Sprite.StopAnimation();
                        spouse.Position = Utility.getRandomAdjacentOpenTile(spouse.getTileLocation(), spouse.currentLocation) * 64;
                        spouse.faceDirection(0);
                        spouse.Schedule = spouse.getSchedule(Game1.dayOfMonth);
                        spouses.Remove(spouse);
                    }
                }
            }
            if (dayStartedDelay > -2) return;
            Farmer who = Game1.player;
            if (!config.SpritePreviewName.Equals("", StringComparison.Ordinal) && (who.currentLocation is Cabin || (Context.IsMainPlayer && (who.currentLocation is FarmHouse || who.currentLocation is Farm))))
            {
                foreach (var name in config.SpouseRoom_Auto_Facing_TileOffset.Keys
                              .Union(config.SpouseRoom_Manual_TileOffsets.Keys)
                              .Union(config.Kitchen_Manual_TileOffsets.Keys)
                              .Union(config.Patio_Manual_TileOffsets.Keys)
                              .Union(config.Porch_Manual_TileOffsets.Keys))
                {
                    if (name.Equals("sebastianFrog", StringComparison.Ordinal) && mode != 0 && mode != 2) continue;

                    if (config.SpritePreviewName.ToLower().Contains("all") || config.SpritePreviewName.Contains(name))
                    {
                        if (who.currentLocation is FarmHouse || who.currentLocation is Cabin)
                        {
                            if (config.Kitchen_Manual_TileOffsets.TryGetValue(name, out List<KeyValuePair<string, Vector2>> list))//kitchen
                            {
                                Vector2 kitchenDefault = Utility.PointToVector2((who.currentLocation as FarmHouse).getKitchenStandingSpot());

                                if (!allNPCs.TryGetValue(name, out NPC npc))
                                {
                                    if (Game1.player.getSpouse()?.isVillager() != null) npc = Game1.player.getSpouse();
                                    else allNPCs.TryGetValue("Emily", out npc);
                                }
                                if (npc != null)
                                {
                                    foreach (var entry in list)
                                    {
                                        int spriteIndex = TryGetSprite(entry.Key);
                                        if (spriteIndex != -9999)
                                        {
                                            if (npc != null) e.SpriteBatch.Draw(npc.Sprite.Texture, Game1.GlobalToLocal((kitchenDefault + entry.Value) * 64f),
                                                Game1.getSquareSourceRectForNonStandardTileSheet(npc.Sprite.Texture, npc.Sprite.SpriteWidth, npc.Sprite.SpriteHeight, Math.Abs(spriteIndex)), Color.Gray * 0.8f, 0f,
                                                new Vector2(0f, 20f), 4f, (spriteIndex < 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 1f);
                                        }
                                    }
                                }
                            }

                            Vector2 spouseDefault = Utility.PointToVector2((who.currentLocation as FarmHouse).GetSpouseRoomSpot());

                            for (int i = 0; i < allNPCs.Count; i++)
                            {
                                if (config.SpouseRoom_Manual_TileOffsets.TryGetValue(name, out list))//spouse room
                                {
                                    if (!allNPCs.TryGetValue(name, out NPC npc))
                                    {
                                        if (name.Equals("sebastianFrog", StringComparison.Ordinal) && allNPCs.TryGetValue("Sebastian", out npc)) ;
                                        else if (Game1.player.getSpouse()?.isVillager() != null) npc = Game1.player.getSpouse();
                                        else allNPCs.TryGetValue("Emily", out npc);
                                    }
                                    if (npc != null)
                                    {
                                        foreach (var entry in list)
                                        {
                                            int spriteIndex = TryGetSprite(entry.Key);
                                            if (spriteIndex != -9999)
                                            {
                                                if (npc != null) e.SpriteBatch.Draw(npc.Sprite.Texture, Game1.GlobalToLocal(((name.Equals("sebastianFrog", StringComparison.Ordinal) ? sebastianFrogTile : spouseDefault) + entry.Value) * 64f),
                                                    Game1.getSquareSourceRectForNonStandardTileSheet(npc.Sprite.Texture, npc.Sprite.SpriteWidth, npc.Sprite.SpriteHeight, Math.Abs(spriteIndex)), ((name.Equals("sebastianFrog", StringComparison.Ordinal)) ? Color.LimeGreen : Color.Gray) * 0.8f, 0f,
                                                    new Vector2(0f, 20f), 4f, (spriteIndex < 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 1f);
                                            }
                                        }
                                    }
                                }
                                if (config.SpouseRoom_Auto_Facing_TileOffset.TryGetValue(name, out Vector2 tile))//face tiles (spouse room)
                                {
                                    if (!allNPCs.TryGetValue(name, out NPC current))
                                    {
                                        if (name.Equals("sebastianFrog", StringComparison.Ordinal) && allNPCs.TryGetValue("Sebastian", out current)) ;
                                        else if (Game1.player.getSpouse()?.isVillager() != null) current = Game1.player.getSpouse();
                                        else allNPCs.TryGetValue("Emily", out current);
                                    }
                                    if (current != null) e.SpriteBatch.Draw(current.Sprite.Texture, Game1.GlobalToLocal(((name.Equals("sebastianFrog", StringComparison.Ordinal) ? sebastianFrogTile : spouseDefault) + tile) * 64f),
                                        new Rectangle(0, 2, 16, 16), (name.Equals("sebastianFrog", StringComparison.Ordinal) ? Color.LimeGreen : Color.Gray) * 0.8f, 0f, new Vector2(8f), 2f, SpriteEffects.None, 1f);
                                }
                                spouseDefault.X += 7;

                                int tileData = 0;
                                foreach (var layer in who.currentLocation.map.Layers)
                                {
                                    if (spouseDefault.X > layer.LayerWidth || spouseDefault.Y > layer.LayerHeight) continue;

                                    if (layer.Tiles[(int)spouseDefault.X, (int)spouseDefault.Y] != null) tileData++;
                                }
                                if (tileData == 0) break;
                            }
                        }
                        else if (who.currentLocation is Farm)
                        {
                            if (patioPoints.Count < 1 && config.Patio_Manual_TileOffsets.TryGetValue(name, out List<KeyValuePair<string, Vector2>> list))//patio
                            {
                                if (!allNPCs.TryGetValue(name, out NPC npc))
                                {
                                    if (Game1.player.getSpouse()?.isVillager() != null) npc = Game1.player.getSpouse();
                                    else allNPCs.TryGetValue("Emily", out npc);
                                }
                                if (npc != null)
                                {
                                    Vector2 spouseDefault = npc.GetSpousePatioPosition();
                                    if (mode < 2)
                                    {
                                        switch (npc.Name)
                                        {
                                            case "Emily":
                                                spouseDefault.X += -1f;
                                                break;
                                            case "Shane":
                                                spouseDefault.X += -2f;
                                                break;
                                            case "Sam":
                                                spouseDefault.Y += -1f;
                                                break;
                                            case "Elliott":
                                                spouseDefault.Y += -1f;
                                                break;
                                            case "Harvey":
                                                spouseDefault.Y += -1f;
                                                break;
                                            case "Alex":
                                                spouseDefault.Y += -1f;
                                                break;
                                            case "Maru":
                                                spouseDefault.X += -1f;
                                                spouseDefault.Y += -1f;
                                                break;
                                            case "Penny":
                                                spouseDefault.Y += -1f;
                                                break;
                                            case "Haley":
                                                spouseDefault.Y += -1f;
                                                spouseDefault.X += -1f;
                                                break;
                                            case "Abigail":
                                                spouseDefault.Y += -1f;
                                                break;
                                            case "Leah":
                                                spouseDefault.Y += -1f;
                                                break;
                                        }
                                    }
                                    foreach (var entry in list)
                                    {
                                        int spriteIndex = TryGetSprite(entry.Key);
                                        if (spriteIndex != -9999)
                                        {
                                            if (npc != null) e.SpriteBatch.Draw(npc.Sprite.Texture, Game1.GlobalToLocal((spouseDefault + entry.Value) * 64f),
                                                Game1.getSquareSourceRectForNonStandardTileSheet(npc.Sprite.Texture, npc.Sprite.SpriteWidth, npc.Sprite.SpriteHeight, Math.Abs(spriteIndex)), Color.Gray * 0.8f, 0f,
                                                new Vector2(0f, 20f), 4f, (spriteIndex < 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 1f);
                                        }
                                    }
                                }
                            }
                            if (config.Porch_Manual_TileOffsets.TryGetValue(name, out list))//porch
                            {
                                Vector2 spouseDefault = Utility.PointToVector2(houseCache.getPorchStandingSpot());

                                if (!allNPCs.TryGetValue(name, out NPC npc))
                                {
                                    if (Game1.player.getSpouse()?.isVillager() != null) npc = Game1.player.getSpouse();
                                    else allNPCs.TryGetValue("Emily", out npc);
                                }
                                if (npc != null)
                                {
                                    foreach (var entry in list)
                                    {
                                        int spriteIndex = TryGetSprite(entry.Key);
                                        if (spriteIndex != -9999)
                                        {
                                            if (npc != null) e.SpriteBatch.Draw(npc.Sprite.Texture, Game1.GlobalToLocal((spouseDefault + entry.Value) * 64f),
                                                Game1.getSquareSourceRectForNonStandardTileSheet(npc.Sprite.Texture, npc.Sprite.SpriteWidth, npc.Sprite.SpriteHeight, Math.Abs(spriteIndex)), Color.Gray * 0.8f, 0f,
                                                new Vector2(0f, 20f), 4f, (spriteIndex < 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 1f);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (patioPoints.Count > 1 && who.currentLocation is Farm)//custom patio
                {
                    foreach (var patio in patioPoints.Where(val => !val.Key.Equals("All", StringComparison.Ordinal)))
                    {
                        if (allNPCs.TryGetValue(patio.Key, out NPC npc))
                        {
                            foreach (var entry in (config.Patio_Manual_TileOffsets.ContainsKey(patio.Key) ? config.Patio_Manual_TileOffsets[patio.Key] : config.Patio_Manual_TileOffsets["Default"]))
                            {
                                int spriteIndex = TryGetSprite(entry.Key);
                                if (spriteIndex != -9999)
                                {
                                    if (npc != null) e.SpriteBatch.Draw(npc.Sprite.Texture, Game1.GlobalToLocal((patio.Value + entry.Value) * 64f),
                                        Game1.getSquareSourceRectForNonStandardTileSheet(npc.Sprite.Texture, npc.Sprite.SpriteWidth, npc.Sprite.SpriteHeight, Math.Abs(spriteIndex)), Color.Gray * 0.8f, 0f,
                                        new Vector2(0f, 20f), 4f, (spriteIndex < 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 1f);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void OnResized(object sender, WindowResizedEventArgs e)
        {
            resized = true;
        }


        private void UpdateConfig(bool GMCM)
        {
            if (!GMCM) config = Helper.ReadConfig<ModConfig>();
        }
    }
}
