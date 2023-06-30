/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TMThong/Stardew-Mods
**
*************************************************/

using FindObjectMod.Framework;
using FindObjectMod.Framework.Menus;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using System;
using FindObjectMod.Framework.Menus.Components;
using StardewValley.Menus;

namespace FindObjectMod
{
    public class ModEntry : Mod
    {
        public List<ModTool> ModTools { get; } = new List<ModTool>();
        public ModConfig config;
        public static ModEntry Instance { get; internal set; }
        public override void Entry(IModHelper helper)
        {
            bool isAndroid = Utilities.IsAndroid;
            if (isAndroid)
            {
                base.Monitor.Log("Mobile version may have errors!", LogLevel.Warn);
            }
            ModEntry.Instance = this;
            this.config = helper.ReadConfig<ModConfig>();
            ModConfig c = this.config;
            helper.Events.Input.ButtonPressed += delegate (object o, ButtonPressedEventArgs e)
            {
                bool flag = !Context.IsWorldReady || Game1.activeClickableMenu != null;
                if (!flag)
                {
                    bool flag2 = e.Button == c.KeyOpenMenu;
                    if (flag2)
                    {
                        this.InitOptionsElement();
                        Game1.activeClickableMenu = new ModMenu(helper, this.Monitor);
                    }
                    bool flag3 = (e.Button == c.KeySelectObject && !Utilities.IsAndroid && c.SearchMode) || (Utilities.IsAndroid && c.InitiatesObjectSelectionModeForMobile && c.SearchMode);
                    if (flag3)
                    {
                        int x = (int)e.Cursor.ScreenPixels.X;
                        int y = (int)e.Cursor.ScreenPixels.Y;
                        GameLocation location = Game1.currentLocation;
                        StardewValley.Object[] objects_ = Utilities.GetObjects(location);
                        bool flag4 = Utilities.isClick(x, y, objects_);
                        if (flag4)
                        {
                            StardewValley.Object fi = objects_.ToList<StardewValley.Object>().Find((StardewValley.Object p) => Utilities.isClick(x, y, p));
                            this.ObjectCliked(location, fi);
                        }
                        else
                        {
                            bool flag5 = Utilities.isClick(x, y, Utilities.GetNpcs(null).ToArray());
                            if (flag5)
                            {
                                NPC npc_ = Utilities.GetNpcs(null).Find((NPC p) => Utilities.isClick(x, y, p));
                                this.NPCClicked(location, npc_);
                            }
                        }
                    }
                }
            };
            helper.Events.GameLoop.SaveLoaded += delegate (object o, SaveLoadedEventArgs e)
            {
                Utilities.SaveKey = Constants.SaveFolderName;
                bool flag = !c.ObjectToFind.ContainsKey(Utilities.SaveKey);
                if (flag)
                {
                    c.ObjectToFind.Add(Utilities.SaveKey, new Dictionary<string, Color>());
                    this.Helper.WriteConfig<ModConfig>(c);
                }
                bool flag2 = !c.FindCharacter.ContainsKey(Utilities.SaveKey);
                if (flag2)
                {
                    c.FindCharacter.Add(Utilities.SaveKey, new Dictionary<string, Color>());
                    this.Helper.WriteConfig<ModConfig>(c);
                }
                List<ModTool> modTools = this.ModTools;
                if (modTools != null)
                {
                    modTools.Clear();
                }
                List<ModTool> modTools2 = this.ModTools;
                if (modTools2 != null)
                {
                    modTools2.Add(new NPCFind(helper, this.Monitor, c));
                }
                List<ModTool> modTools3 = this.ModTools;
                if (modTools3 != null)
                {
                    modTools3.Add(new FindObject(this.Monitor, helper, c));
                }
                List<ModTool> modTools4 = this.ModTools;
                if (modTools4 != null)
                {
                    modTools4.ForEach(delegate (ModTool p)
                    {
                        p.Initialization();
                    });
                }
            };
            helper.Events.GameLoop.GameLaunched += delegate (object o, GameLaunchedEventArgs e)
            {
                List<ModTool> modTools = this.ModTools;
                if (modTools != null)
                {
                    modTools.ForEach(delegate (ModTool p)
                    {
                        p.Destroy();
                    });
                }
                List<ModTool> modTools2 = this.ModTools;
                if (modTools2 != null)
                {
                    modTools2.Clear();
                }
            };
        }

        public void ObjectCliked(GameLocation location, StardewValley.Object gameObject)
        {
            ITranslationHelper i18n = base.Helper.Translation;
            string title = base.ModManifest.Name;
            bool add = !this.config.ObjectToFind[Utilities.SaveKey].ContainsKey(gameObject.name);
            Response[] responses = new Response[]
            {
            add ? new Response("addobject", i18n.Get("AddObject", new        {            gameObject.displayName            })) : new Response("deleteobject", i18n.Get("DeleteObject", new        {            gameObject.displayName        })),       new Response("exit", i18n.Get("Exit"))           };
            Action a111 = null;
            Action<Farmer, string> Choose = delegate (Farmer f, string k)
            {
                if (!(k == "addobject"))
                {
                    if (!(k == "deleteobject"))
                    {
                        if (!(k == "exit"))
                        {
                        }
                    }
                    else
                    {
                        this.config.ObjectToFind[Utilities.SaveKey].Remove(gameObject.name);
                        this.Helper.WriteConfig<ModConfig>(this.config);
                    }
                }
                else
                {
                    Game1.exitActiveMenu();
                    Color @object = this.config.Object;
                    Action<Color> setColor = delegate (Color null_)
                    {
                    };
                    Action callBack;
                    if ((callBack = a111) == null)
                    {
                        callBack = (a111 = delegate ()
                        {
                            add1((Game1.activeClickableMenu as ColorPickerMenu).MColor);
                            Game1.exitActiveMenu();
                            Game1.player.canMove = true;
                        });
                    }
                    Game1.activeClickableMenu = new ColorPickerMenu(@object, setColor, callBack);
                }
            };
            location.createQuestionDialogue(title, responses, new GameLocation.afterQuestionBehavior(Choose.Invoke), null);
            void add1(Color cl)
            {

                config.ObjectToFind[Utilities.SaveKey].Add(gameObject.name, cl);
                Helper.WriteConfig<ModConfig>(config);
            }
        }

        public void NPCClicked(GameLocation location, NPC npc)
        {
            ITranslationHelper i18n = base.Helper.Translation;
            string title = base.ModManifest.Name;
            bool add = !this.config.FindCharacter[Utilities.SaveKey].ContainsKey(npc.name);
            Response[] responses = new Response[]
            {
        add ? new Response("addnpc", i18n.Get("AddNPC", new
        {
            npc.displayName
        })) : new Response("deletenpc", i18n.Get("DeleteNPC", new
        {
            npc.displayName
        })),
        new Response("exit", i18n.Get("Exit"))
            };
            Action a111 = null;
            Action<Farmer, string> Choose = delegate (Farmer f, string k)
            {
                if (!(k == "addnpc"))
                {
                    if (!(k == "deletenpc"))
                    {
                        if (!(k == "exit"))
                        {
                        }
                    }
                    else
                    {
                        this.config.FindCharacter[Utilities.SaveKey].Remove(npc.name);
                        this.Helper.WriteConfig<ModConfig>(this.config);
                    }
                }
                else
                {
                    Game1.exitActiveMenu();
                    Color npc2 = this.config.NPC;
                    Action<Color> setColor = delegate (Color null_)
                    {
                    };
                    Action callBack;
                    if ((callBack = a111) == null)
                    {
                        callBack = (a111 = delegate ()
                        {
                            add1((Game1.activeClickableMenu as ColorPickerMenu).MColor);
                            Game1.exitActiveMenu();
                            Game1.player.canMove = true;
                        });
                    }
                    Game1.activeClickableMenu = new ColorPickerMenu(npc2, setColor, callBack);
                }
            };
            location.createQuestionDialogue(title, responses, new GameLocation.afterQuestionBehavior(Choose.Invoke), null);
            void add1(Color cl)
            {

                config.FindCharacter[Utilities.SaveKey].Add(npc.name, cl);
                Helper.WriteConfig<ModConfig>(config);
            }
        }

        public void InitOptionsElement()
        {
            List<OptionsElement> oe = Utilities.OptionsElements;
            oe.Clear();
            ITranslationHelper translation = Helper.Translation;
            oe.Add(new OptionsElement(translation.Get("title")));
            oe.Add((OptionsElement)(object)new ModOptionsCheckbox(translation.Get("FindQuestObject"), -1, delegate (bool b)
            {
                config.FindQuestObject = b;
                saveConfig();
                initOptions();
            }, config.FindQuestObject));
            oe.Add((OptionsElement)(object)new ModOptionsCheckbox(translation.Get("DrawArea"), -1, delegate (bool b)
            {
                config.DrawArea = b;
                saveConfig();
            }, config.DrawArea));
            oe.Add((OptionsElement)(object)new ModOptionsCheckbox(translation.Get("SearchMode"), -1, delegate (bool b)
            {
                config.SearchMode = b;
                saveConfig();
            }, config.SearchMode));
            oe.Add((OptionsElement)(object)new ModOptionsCheckbox(translation.Get("FindAllNPC"), -1, delegate (bool b)
            {
                config.FindAllNPC = b;
                saveConfig();
                initOptions();
            }, config.FindAllNPC));
            oe.Add((OptionsElement)(object)new ModOptionsCheckbox(translation.Get("FindAllObject"), -1, delegate (bool b)
            {
                config.FindAllObject = b;
                saveConfig();
                initOptions();
            }, config.FindAllObject));
            if (Utilities.IsAndroid)
            {
                oe.Add((OptionsElement)(object)new ModOptionsCheckbox(translation.Get("InitiatesObjectSelectionModeForMobile"), -1, delegate (bool b)
                {
                    config.InitiatesObjectSelectionModeForMobile = b;
                    saveConfig();
                }, config.InitiatesObjectSelectionModeForMobile));
            }
            oe.Add((OptionsElement)new OptionsButton(translation.Get("ResetModConfig"), (Action)delegate
            {
                config.reset();
                saveConfig();
                initOptions();
            }));
            if (config.FindAllNPC)
            {
                oe.Add((OptionsElement)(object)new ModOptionsColorPicker(translation.Get("Color.Picker", (object)new
                {
                    name = translation.Get("NPC")
                }), config.NPC, delegate (Color c)
                {
                    config.NPC = c;
                    saveConfig();
                }));
                oe.Add((OptionsElement)(object)new ModOptionsColorPicker(translation.Get("Color.Picker", (object)new
                {
                    name = translation.Get("Monsters")
                }), config.Monsters, delegate (Color c)
                {
                    config.Monsters = c;
                    saveConfig();
                }));
            }
            if (config.FindAllObject)
            {
                oe.Add((OptionsElement)(object)new ModOptionsColorPicker(translation.Get("Color.Picker", (object)new
                {
                    name = translation.Get("Object")
                }), config.Object, delegate (Color c)
                {
                    config.Object = c;
                    saveConfig();
                }));
            }
            if (config.FindQuestObject)
            {
                oe.Add((OptionsElement)(object)new ModOptionsColorPicker(translation.Get("Color.Picker", (object)new
                {
                    name = translation.Get("QuestObject")
                }), config.QuestObject, delegate (Color c)
                {
                    config.QuestObject = c;
                    saveConfig();
                }));
            }
            if (config.ObjectToFind[Utilities.SaveKey].Count > 0)
            {
                oe.Add(new OptionsElement(translation.Get("ObjectToFind")));
                oe.Add((OptionsElement)new OptionsButton(translation.Get("Clear", (object)new
                {
                    name = translation.Get("Object")
                }), (Action)delegate
                {
                    config.ObjectToFind[Utilities.SaveKey].Clear();
                    saveConfig();
                    initOptions();
                }));
                foreach (string i in config.ObjectToFind[Utilities.SaveKey].Keys)
                {
                    oe.Add((OptionsElement)(object)new ModOptionsColorPicker(translation.Get("Color.Picker", (object)new
                    {
                        name = i
                    }), config.ObjectToFind[Utilities.SaveKey][i], delegate (Color c)
                    {
                        config.ObjectToFind[Utilities.SaveKey][i] = c;
                        saveConfig();
                    }));
                }
            }
            if (config.FindCharacter[Utilities.SaveKey].Count <= 0)
            {
                return;
            }
            oe.Add(new OptionsElement(translation.Get("FindCharacter")));
            oe.Add((OptionsElement)new OptionsButton(translation.Get("Clear", (object)new
            {
                name = translation.Get("NPC")
            }), (Action)delegate
            {
                config.FindCharacter[Utilities.SaveKey].Clear();
                saveConfig();
                initOptions();
            }));
            foreach (string j in config.FindCharacter[Utilities.SaveKey].Keys)
            {
                oe.Add((OptionsElement)(object)new ModOptionsColorPicker(translation.Get("Color.Picker", (object)new
                {
                    name = j
                }), config.FindCharacter[Utilities.SaveKey][j], delegate (Color c)
                {
                    config.FindCharacter[Utilities.SaveKey][j] = c;
                    saveConfig();
                }));
            }
            void initOptions()
            {
                InitOptionsElement();
                (Game1.activeClickableMenu as ModMenu)?.updateElements(oe);
            }
            void saveConfig()
            {
                Helper.WriteConfig<ModConfig>(config);
            }
        }
    }
}
