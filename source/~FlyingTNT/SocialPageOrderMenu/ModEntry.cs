/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/FlyingTNT/StardewValleyMods
**
*************************************************/

using Common.Utilities;
using Common.Integrations;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using static StardewValley.Menus.SocialPage;

namespace SocialPageOrderRedux
{
    public class ModEntry : Mod
    {
        public static ModConfig Config;
        public static IMonitor SMonitor;
        public static IModHelper SHelper;

        /// <summary> Location of the organize button within LooseSprites/Cursors </summary>
        public static readonly Rectangle buttonTextureSource = new Rectangle(162, 440, 16, 16);

        /// <summary> The X offset of the sort button from the left of the game menu. </summary>
        private const int xOffset = -16;

        /// <summary> The Y offset ot the dropdown from the bottom of the game menu. </summary>
        private const int dropdownYOffset = -28;

        /// <summary> The unique id of the sort button Clickable Component. </summary>
        private const int buttonId = 231445356;

        /// <summary> The dropdown object. </summary>
        public static readonly PerScreen<MyOptionsDropDown> dropDown = new();

        /// <summary> The button object. </summary>
        public static readonly PerScreen<ClickableTextureComponent> button = new();

        /// <summary> The position to return to when returning to the social page after clicking to a ProfileMenu (the npc at the top of the screen when you click on a character to open their page).  </summary>
        public static readonly PerScreen<int> lastSlotPosition = new PerScreen<int>(() => 0);

        /// <summary> The string that was in the filter field the last time it was checked. </summary>
        private static readonly PerScreen<string> lastFilterString = new PerScreen<string>(()=>"");

        /// <summary> The filter field (search bar) object. </summary>
        private static readonly PerScreen<TextBox> filterField = new();

        /// <summary> All of the entries in the social page, before we removed any when searching. Used to restore the page when we clear the search bar. </summary>
        private static readonly PerScreen<List<SocialEntry>> allEntries = new PerScreen<List<SocialEntry>>(() => new List<SocialEntry>());

        /// <summary> Whether the mod was enabled when the game was loaded. If it wasn't, we don't do anything because the patches weren't applied. </summary>
        private static bool WasModEnabled = false;

        /// <summary>
        /// The sort curently selected by Game1.player. It is stored in their mod data so that it is preserved between sessions, and for splitscreen support (it used to be stored in the config file,
        /// but this would link the two screens' sorts together).
        /// </summary>
        public static int CurrentSort
        {
            get
            {
                return PerPlayerConfig.LoadConfigOption(Game1.player, "FlyingTNT.SocialPageOrderRedux.CurrentSort", defaultValue: 0);
            }
            set
            {
                PerPlayerConfig.SaveConfigOption(Game1.player, "FlyingTNT.SocialPageOrderRedux.CurrentSort", value);
            }
        }

        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ModConfig>();
            WasModEnabled = Config.EnableMod;
            helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;
            if (!Config.EnableMod)
                return;
            SMonitor = Monitor;
            SHelper = Helper;

            helper.Events.Input.ButtonsChanged += Input_ButtonsChanged;
            helper.Events.Display.MenuChanged += Display_MenuChanged;
            helper.Events.GameLoop.ReturnedToTitle += GameLoop_ReturnedToTitle;

            var harmony = new Harmony(ModManifest.UniqueID);
            harmony.PatchAll();

            harmony.Patch(AccessTools.Constructor(typeof(SocialPage), new Type[] { typeof(int), typeof(int), typeof(int), typeof(int) }),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(SocialPage_Constructor_Postfix))
            );

            harmony.Patch(AccessTools.Method(typeof(IClickableMenu), nameof(IClickableMenu.receiveKeyPress)),
                prefix: new HarmonyMethod(typeof(ModEntry), nameof(SocialPage_recieveKeyPress_Prefix))
            );
            
            harmony.Patch(AccessTools.Method(typeof(GameMenu), nameof(GameMenu.receiveKeyPress)),
                prefix: new HarmonyMethod(typeof(ModEntry), nameof(GameMenu_recieveKeyPress_Prefix))
            );
            
            harmony.Patch(AccessTools.Method(typeof(SocialPage), nameof(SocialPage.performHoverAction)),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(SocialPage_performHoverAction_Postfix))
            );
            
            harmony.Patch(AccessTools.Method(typeof(SocialPage), nameof(SocialPage.updateSlots)),
                prefix: new HarmonyMethod(typeof(ModEntry), nameof(SocialPage_updateSlots_Prefix))
            );
            
            harmony.Patch(AccessTools.Method(typeof(SocialPage), nameof(SocialPage.FindSocialCharacters)),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(SocialPage_FindSocialCharacters_Postfix))
            );
            
            harmony.Patch(AccessTools.Method(typeof(IClickableMenu), nameof(IClickableMenu.populateClickableComponentList)),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(IClickableMenu_populateClickableComponentList_Postfix))
            );

            harmony.Patch(AccessTools.Method(typeof(IClickableMenu), nameof(IClickableMenu.readyToClose)),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(IClickableMenu_readyToClose_Postfix))
            );

            harmony.Patch(AccessTools.Method(typeof(GameMenu), nameof(GameMenu.changeTab)),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(GameMenu_changeTab_Postfix))
            );
        }

        private void GameLoop_GameLaunched(object sender, GameLaunchedEventArgs e)
        {

            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            // register mod
            configMenu.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => Helper.WriteConfig(Config)
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Mod Enabled?",
                getValue: () => Config.EnableMod,
                setValue: value => Config.EnableMod = value
            );

            configMenu.AddKeybindList(
                mod: ModManifest,
                name: () => "Prev Sort Key",
                getValue: () => Config.prevButton,
                setValue: value => Config.prevButton = value
            );

            configMenu.AddKeybindList(
                mod: ModManifest,
                name: () => "Next Sort Key",
                getValue: () => Config.nextButton,
                setValue: value => Config.nextButton = value
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Use Filter",
                getValue: () => Config.UseFilter,
                setValue: (value) => { Config.UseFilter = value;
                                       InitElements();}
            );
            
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Use Button",
                getValue: () => Config.UseButton,
                setValue: (value) => { Config.UseButton = value;
                                       InitElements();}
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Use Dropdown",
                getValue: () => Config.UseDropdown,
                setValue: (value) => { Config.UseDropdown = value;
                                       InitElements();}
            );

            configMenu.AddNumberOption(
               mod: ModManifest,
               name: () => "Button X Offset",
               getValue: () => Config.ButtonOffsetX,
               setValue: value => Config.ButtonOffsetX = value
           );

            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Button Y Offset",
                getValue: () => Config.ButtonOffsetY,
                setValue: value => Config.ButtonOffsetY = value
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Dropdown X Offset",
                getValue: () => Config.DropdownOffsetX,
                setValue: value => Config.DropdownOffsetX = value
            );
            
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Dropdown Y Offset",
                getValue: () => Config.DropdownOffsetY,
                setValue: value => Config.DropdownOffsetY = value
            );
            
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Filter X Offset",
                getValue: () => Config.FilterOffsetX,
                setValue: value => Config.FilterOffsetX = value
            );
            
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Filter Y Offset",
                getValue: () => Config.FilterOffsetY,
                setValue: value => Config.FilterOffsetY = value
            );
        }

        private void Input_ButtonsChanged(object sender, ButtonsChangedEventArgs e)
        {
            if (!WasModEnabled || Game1.activeClickableMenu is not GameMenu || (Game1.activeClickableMenu as GameMenu).GetCurrentPage() is not SocialPage)
                return;
            if (Config.prevButton.JustPressed())
            {
                DecrementSort();
            }
            else if (Config.nextButton.JustPressed())
            {
                IncrementSort();
            }
        }

        private void Display_MenuChanged(object sender, MenuChangedEventArgs e)
        {
            lastFilterString.Value += "dirty";// Force an update

            if (Game1.activeClickableMenu is not GameMenu menu)
                return;

            if(Config.UseFilter && e.OldMenu is not ProfileMenu)
            {
                filterField.Value.Text = "";
            }

            if (menu.GetCurrentPage() is not SocialPage socialPage)
                return;

            // Resort the social page (it is reset when the menu is changed); if the previous menu was a ProfileMenu (the menu that pops up when you click on a npc in the social page), scroll the menu to that npc
            ResortSocialList(slotToSelect: e.OldMenu is ProfileMenu pm ? pm.Current : null);

            // Snap the cursor to the selected item if necessary
            if (Game1.options.snappyMenus && Game1.options.gamepadControls)
            {
                if(socialPage.currentlySnappedComponent is not null)
                {
                    // For some reason, the snapping doesn't work correctly in local multiplayer. I have no idea why. With moveCursorInDirection(-1), the cursor is at least consistently *in* the correct box.
                    if (Context.IsSplitScreen)
                        socialPage.moveCursorInDirection(-1);
                    else
                        socialPage.snapCursorToCurrentSnappedComponent();
                }
                else
                {
                    SMonitor.Log("Currently snapped component is null.");
                }
            }
        }

        public static void GameLoop_ReturnedToTitle(object sender, ReturnedToTitleEventArgs args)
        {
            // Null the dropdown. This forces a reload next time the dropdown is loaded. This is necessary in case the player changes their language - the dropdown
            // sets the text for its options when it is created, so if the player changes the language the options won't be updated unless we reload the dropdown.
            dropDown.Value = null;
        }

        public static void SocialPage_Constructor_Postfix(SocialPage __instance)
        {
            if (!Config.EnableMod)
                return;

            InitElements();
        }

        public static void SocialPage_FindSocialCharacters_Postfix(List<SocialEntry> __result)
        {
            // __result is the list of all of the entries

            allEntries.Value.Clear();
            allEntries.Value.AddRange(__result);
        }

        public static void IClickableMenu_populateClickableComponentList_Postfix(IClickableMenu __instance)
        {
            if (!Config.EnableMod || !Config.UseButton || __instance is not SocialPage)
                return;

            __instance.allClickableComponents.Add(button.Value);
        }

        public static void IClickableMenu_readyToClose_Postfix(IClickableMenu __instance, ref bool __result)
        {
            if (!Config.EnableMod || __instance is not SocialPage || filterField.Value is null)
                return;

            // If the filter is selected, make the result false. This is because some mods that add their own menus will overwrite the current one when their menu's key is pressed,
            // but if the player has the filter selected, they didn't mean to open the other menu; they were just typing in the search bar.
            __result &= !filterField.Value.Selected;
        }

        [HarmonyPatch(typeof(SocialPage), nameof(SocialPage.draw), new Type[] { typeof(SpriteBatch) })]
        public class SocialPage_drawTextureBox_Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                SMonitor.Log("Transpiling SocialPage.draw");
                var codes = new List<CodeInstruction>(instructions);
                int index = codes.FindLastIndex(ci => ci.opcode == OpCodes.Call && (MethodInfo)ci.operand == AccessTools.Method(typeof(IClickableMenu), nameof(IClickableMenu.drawTextureBox), new Type[] { typeof(SpriteBatch), typeof(Texture2D), typeof(Rectangle), typeof(int), typeof(int), typeof(int), typeof(int), typeof(Color), typeof(float), typeof(bool), typeof(float) }));
                if(index > -1)
                {
                    SMonitor.Log("Inserting dropdown draw method");
                    codes.Insert(index + 1, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(SocialPage_drawTextureBox_Patch), nameof(DrawDropDown))));
                    codes.Insert(index + 1, new CodeInstruction(OpCodes.Ldarg_1));
                    codes.Insert(index + 1, new CodeInstruction(OpCodes.Ldarg_0));

                }
                return codes.AsEnumerable();
            }
            public static void DrawDropDown(SocialPage page, SpriteBatch b)
            {
                if (!Config.EnableMod)
                    return;

                try
                {
                    if (Config.UseFilter)
                    {
                        UpdateFilterPosition(page);
                        filterField.Value.Draw(b);
                    }

                    if (Config.UseDropdown)
                    {
                        dropDown.Value.draw(b, GetDropdownX(page), GetDropdownY(page));
                        if (SHelper.Input.IsDown(SButton.MouseLeft) && AccessTools.FieldRefAccess<OptionsDropDown, bool>(dropDown.Value, "clicked") && dropDown.Value.dropDownBounds.Contains(Game1.getMouseX() - GetDropdownX(page), Game1.getMouseY() - GetDropdownY(page)))
                        {
                            dropDown.Value.selectedOption = (int)Math.Max(Math.Min((float)(Game1.getMouseY() - GetDropdownY(page) - dropDown.Value.dropDownBounds.Y) / (float)dropDown.Value.bounds.Height, (float)(dropDown.Value.dropDownOptions.Count - 1)), 0f);
                        }
                    }

                    if (Config.UseButton)
                    {
                        button.Value.bounds = GetButtonRectangle(page);
                        button.Value.draw(b);
                        if (button.Value.bounds.Contains(Game1.getMousePosition()))
                        {
                            (Game1.activeClickableMenu as GameMenu).hoverText = SHelper.Translation.Get($"sort-by") + SHelper.Translation.Get($"sort-{CurrentSort}");
                        }
                    }
                }
                catch(Exception ex)
                {
                    SMonitor.Log($"Failed in {nameof(DrawDropDown)}: {ex}", LogLevel.Error);
                }
            }
        }
        [HarmonyPatch(typeof(SocialPage), nameof(SocialPage.receiveLeftClick))]
        public class SocialPage_receiveLeftClick_Patch
        {
            public static bool Prefix(SocialPage __instance, int x, int y)
            {
                if (!Config.EnableMod)
                    return true;

                lastSlotPosition.Value = SHelper.Reflection.GetField<int>(__instance, "slotPosition").GetValue();

                if (Config.UseDropdown && dropDown.Value.bounds.Contains(x - GetDropdownX(__instance), y - GetDropdownY(__instance)))
                {
                    dropDown.Value.receiveLeftClick(x - GetDropdownX(__instance), y - GetDropdownY(__instance));
                    return false;
                }

                if(Config.UseButton)
                {
                    if (button.Value.bounds.Contains(x, y))
                    {
                        IncrementSort();
                        return false;
                    }
                }

                if (Config.UseFilter)
                    filterField.Value.Update();
                return true;
            }
        }

        [HarmonyPatch(typeof(SocialPage), nameof(SocialPage.releaseLeftClick))]
        public class SocialPage_releaseLeftClick_Patch
        {
            public static bool Prefix(SocialPage __instance, int x, int y)
            {
                if (!Config.EnableMod || !Config.UseDropdown)
                    return true;

                if (AccessTools.FieldRefAccess<OptionsDropDown, bool>(dropDown.Value, "clicked"))
                {
                    if(dropDown.Value.dropDownBounds.Contains(Game1.getMouseX() - GetDropdownX(__instance), Game1.getMouseY() - GetDropdownY(__instance)))
                    {
                        dropDown.Value.leftClickReleased(Game1.getMouseX() - GetDropdownX(__instance), Game1.getMouseY() - GetDropdownY(__instance));
                    }
                    return false;
                }
                return true;
            }
        }

        public static bool SocialPage_recieveKeyPress_Prefix(IClickableMenu __instance, Keys key)
        {
            if (!Config.EnableMod || __instance is not SocialPage socialPage || !Config.UseFilter || filterField.Value is null || !filterField.Value.Selected || Game1.options.gamepadControls)
                return true;

            if(key == Keys.Escape)
            {
                filterField.Value.Selected = false;
                return true;
            }

            socialPage.updateSlots();
            return false;
        }

        public static bool GameMenu_recieveKeyPress_Prefix(GameMenu __instance, Keys key)
        {
            if (!Config.EnableMod || __instance.GetCurrentPage() is not SocialPage socialPage || !Config.UseFilter || filterField.Value is null || !filterField.Value.Selected || Game1.options.gamepadControls)
                return true;

            if (key == Keys.Escape)
            {
                filterField.Value.Selected = false;
                return true;
            }

            socialPage.updateSlots();
            return false;
        }

        public static void SocialPage_performHoverAction_Postfix(int x, int y)
        {
            if (!Config.EnableMod || !Config.UseFilter)
                return;
            filterField.Value.Hover(x, y);
        }

        public static void SocialPage_updateSlots_Prefix(SocialPage __instance)
        {
            if (!Config.EnableMod || !Config.UseFilter || filterField.Value is null || lastFilterString.Value == filterField.Value.Text)
                return;

            lastFilterString.Value = filterField.Value.Text;

            __instance.SocialEntries.Clear();

            __instance.SocialEntries.AddRange(filterField.Value.Text == "" ? allEntries.Value : allEntries.Value.Where((entry)=>entry.DisplayName.ToLower().StartsWith(filterField.Value.Text)));

            SHelper.Reflection.GetField<int>(__instance, "numFarmers").SetValue(__instance.SocialEntries.Count((SocialEntry p) => p.IsPlayer));

            __instance.CreateComponents();

            ResortSocialList();
        }

        public static void GameMenu_changeTab_Postfix(GameMenu __instance)
        {
            if (!Config.EnableMod)
                return;

            if (__instance.currentTab == GameMenu.socialTab)
            {
                if (Config.UseButton)
                    __instance.tabs[GameMenu.inventoryTab].leftNeighborID = buttonId;
                ResortSocialList();
                if(Game1.options.SnappyMenus)
                    __instance.snapToDefaultClickableComponent();
            }

            if (Config.UseFilter && filterField.Value is not null && !Game1.options.gamepadControls)
                filterField.Value.Selected = __instance.currentTab == GameMenu.socialTab;
        }

        public static void ResortSocialList(SocialEntry slotToSelect = null)
        {
            if (Game1.activeClickableMenu is GameMenu)
            {
                SocialPage page = (Game1.activeClickableMenu as GameMenu).pages[GameMenu.socialTab] as SocialPage;

                List<NameSpriteSlot> nameSprites = new List<NameSpriteSlot>();
                List<ClickableTextureComponent> sprites = new List<ClickableTextureComponent>(SHelper.Reflection.GetField<List<ClickableTextureComponent>>(page, "sprites").GetValue());
                for (int i = 0; i < page.SocialEntries.Count; i++)
                {
                    nameSprites.Add(new NameSpriteSlot(page.SocialEntries[i], sprites[i], page.characterSlots[i]));
                }
                switch (CurrentSort)
                {
                    case 0: // friend asc
                        SMonitor.Log("sorting by friend asc");
                        nameSprites.Sort(delegate (NameSpriteSlot x, NameSpriteSlot y)
                        {
                            if (x.entry.IsPlayer && y.entry.IsPlayer)
                                return 0;

                            if (x.entry.IsPlayer)
                                return -1;

                            if (y.entry.IsPlayer)
                                return 1;

                            bool xIsNullFriendship = x.entry.Friendship is null || x.entry.IsChild;
                            bool yIsNullFriendship = y.entry.Friendship is null || y.entry.IsChild;
                            if (xIsNullFriendship && yIsNullFriendship)
                                return 0;

                            if (xIsNullFriendship)
                                return 1;

                            if (yIsNullFriendship)
                                return -1;

                            int c = x.entry.Friendship.Points.CompareTo(y.entry.Friendship.Points);
                            if (c == 0)
                                c = x.entry.DisplayName.CompareTo(y.entry.DisplayName);
                            return c;

                        });
                        break;
                    case 1: // friend desc
                        SMonitor.Log("sorting by friend desc");
                        nameSprites.Sort(delegate (NameSpriteSlot x, NameSpriteSlot y)
                        {
                            if (x.entry.IsPlayer && y.entry.IsPlayer)
                                return 0;

                            if (x.entry.IsPlayer)
                                return -1;

                            if (y.entry.IsPlayer)
                                return 1;

                            bool xIsNullFriendship = x.entry.Friendship is null || x.entry.IsChild;
                            bool yIsNullFriendship = y.entry.Friendship is null || y.entry.IsChild;
                            if (xIsNullFriendship && yIsNullFriendship)
                                return 0;

                            if (xIsNullFriendship)
                                return 1;

                            if (yIsNullFriendship)
                                return -1;

                            int c = -x.entry.Friendship.Points.CompareTo(y.entry.Friendship.Points);
                            if (c == 0)
                                c = x.entry.DisplayName.CompareTo(y.entry.DisplayName);
                            return c;

                        });
                        break;
                    case 2: // alpha asc
                        SMonitor.Log("sorting by alpha asc");
                        nameSprites.Sort(delegate (NameSpriteSlot x, NameSpriteSlot y)
                        {
                            if (x.entry.IsPlayer && y.entry.IsPlayer)
                                return 0;

                            if (x.entry.IsPlayer)
                                return -1;

                            if (y.entry.IsPlayer)
                                return 1;

                            if (!x.entry.IsMet && !y.entry.IsMet)
                                return 0;
                            if (!x.entry.IsMet)
                                return 1;
                            if (!y.entry.IsMet)
                                return -1;

                            return x.entry.DisplayName.CompareTo(y.entry.DisplayName);
                        });
                        break;
                    case 3: // alpha desc
                        SMonitor.Log("sorting by alpha desc");
                        nameSprites.Sort(delegate (NameSpriteSlot x, NameSpriteSlot y)
                        {
                            if (x.entry.IsPlayer && y.entry.IsPlayer)
                                return 0;

                            if (x.entry.IsPlayer)
                                return -1;

                            if (y.entry.IsPlayer)
                                return 1;

                            if (!x.entry.IsMet && !y.entry.IsMet)
                                return 0;
                            if (!x.entry.IsMet)
                                return 1;
                            if (!y.entry.IsMet)
                                return -1;
                            return -x.entry.DisplayName.CompareTo(y.entry.DisplayName);
                        });
                        break;
                }
                int indexToSelect = -1;

                var cslots = page.characterSlots;
                for (int i = 0; i < nameSprites.Count; i++)
                {
                    nameSprites[i].slot.myID = i;
                    nameSprites[i].slot.downNeighborID = i + 1;
                    nameSprites[i].slot.upNeighborID = i - 1;
                    if (nameSprites[i].slot.upNeighborID < 0)
                    {
                        nameSprites[i].slot.upNeighborID = 12342;
                    }
                    if(Config.UseButton)
                    {
                        nameSprites[i].slot.leftNeighborID = buttonId;
                    }
                    sprites[i] = nameSprites[i].sprite;
                    nameSprites[i].slot.bounds = cslots[i].bounds;
                    cslots[i] = nameSprites[i].slot;
                    page.SocialEntries[i] = nameSprites[i].entry;
                    if(slotToSelect is not null && slotToSelect.InternalName == nameSprites[i].entry.InternalName && slotToSelect.IsPlayer == nameSprites[i].entry.IsPlayer && slotToSelect.IsChild == nameSprites[i].entry.IsChild)
                    {
                        indexToSelect = i;
                    }
                }
                SHelper.Reflection.GetField<List<ClickableTextureComponent>>((Game1.activeClickableMenu as GameMenu).pages[GameMenu.socialTab], "sprites").SetValue(new List<ClickableTextureComponent>(sprites));

                if(indexToSelect == -1)
                {
                    for (int l = 0; l < page.SocialEntries.Count; l++)
                    {
                        if (!page.SocialEntries[l].IsPlayer)
                        {
                            indexToSelect = l;
                            break;
                        }
                    }
                }

                indexToSelect = indexToSelect == -1 ? 0 : indexToSelect;

                if(slotToSelect is null)
                {
                    SHelper.Reflection.GetField<int>((Game1.activeClickableMenu as GameMenu).pages[GameMenu.socialTab], "slotPosition").SetValue(indexToSelect);
                    SHelper.Reflection.GetMethod((Game1.activeClickableMenu as GameMenu).pages[GameMenu.socialTab], "setScrollBarToCurrentIndex").Invoke();
                }
                else
                {
                    page.updateSlots();
                    page.currentlySnappedComponent = page.characterSlots[indexToSelect];
                    SHelper.Reflection.GetField<int>((Game1.activeClickableMenu as GameMenu).pages[GameMenu.socialTab], "slotPosition").SetValue(lastSlotPosition.Value);
                    SHelper.Reflection.GetMethod((Game1.activeClickableMenu as GameMenu).pages[GameMenu.socialTab], "setScrollBarToCurrentIndex").Invoke();
                    page.updateSlots();
                }
            }
        }

        public static void UpdateFilterPosition(SocialPage page)
        {
            filterField.Value.X = page.xPositionOnScreen + page.width / 2 - filterField.Value.Width / 2 + Config.FilterOffsetX;
            filterField.Value.Y = page.yPositionOnScreen + page.height + (Config.UseDropdown ? dropDown.Value.bounds.Height + dropdownYOffset + 20 : 0) + Config.FilterOffsetY;
        }

        public static int GetDropdownX(SocialPage page)
        {
            return page.xPositionOnScreen + page.width / 2 - dropDown.Value.bounds.Width / 2 + Config.DropdownOffsetX;
        }
        public static int GetDropdownY(SocialPage page)
        {
            return page.yPositionOnScreen + page.height + Config.DropdownOffsetY + dropdownYOffset;
        }

        public static int GetButtonX(SocialPage page)
        {
            return page.xPositionOnScreen + xOffset + Config.ButtonOffsetX;
        }

        public static int GetButtonY(SocialPage page)
        {
            return page.yPositionOnScreen + Config.ButtonOffsetY;
        }

        public static Rectangle GetButtonRectangle(SocialPage page)
        {
            return new Rectangle(GetButtonX(page), GetButtonY(page), buttonTextureSource.Width * 4, buttonTextureSource.Height * 4);
        }

        public static void SetUseFilter(bool value)
        {
            if(value)
            {
                if(filterField.Value is null)
                {
                    filterField.Value = new TextBox(Game1.content.Load<Texture2D>("LooseSprites\\textBox"), null, Game1.smallFont, Game1.textColor)
                    {
                        Text = ""
                    };
                }
            }

            Config.UseFilter = value;
        }

        public static void InitElements()
        {
            if (!WasModEnabled)
                return;

            if (filterField.Value is null)
            {
                filterField.Value = new TextBox(Game1.content.Load<Texture2D>("LooseSprites\\textBox"), null, Game1.smallFont, Game1.textColor)
                {
                    Text = ""
                };

                filterField.Value.OnEnterPressed += sender => sender.Selected = false;
                filterField.Value.OnTabPressed += sender => sender.Selected = false;
            }

            if(dropDown.Value is null)
            {
                dropDown.Value = new MyOptionsDropDown("", -1);

                for (int i = 0; i < 4; i++)
                {
                    dropDown.Value.dropDownDisplayOptions.Add(SHelper.Translation.Get($"sort-{i}"));
                    dropDown.Value.dropDownOptions.Add(SHelper.Translation.Get($"sort-{i}"));
                }
                dropDown.Value.RecalculateBounds();
                dropDown.Value.selectedOption = CurrentSort;
            }

            if(button.Value is null)
            {
                button.Value = new ClickableTextureComponent(Rectangle.Empty, Game1.mouseCursors, buttonTextureSource, 4, false)
                {
                    rightNeighborID = GameMenu.region_inventoryTab,
                    myID = buttonId
                };
            }
        }

        public static void IncrementSort()
        {
            CurrentSort++;
            CurrentSort %= 4;
            ResortSocialList();
            if(Config.UseDropdown)
            {
                dropDown.Value.selectedOption = CurrentSort;
            }
        }

        public static void DecrementSort()
        {
            CurrentSort--;
            if (CurrentSort < 0)
                CurrentSort = 3;
            ResortSocialList();
            if (Config.UseDropdown)
            {
                dropDown.Value.selectedOption = CurrentSort;
            }
        }
    }



    internal class NameSpriteSlot
    {
        public SocialPage.SocialEntry entry;
        public ClickableTextureComponent sprite;
        public ClickableTextureComponent slot;

        public NameSpriteSlot(SocialPage.SocialEntry obj, ClickableTextureComponent clickableTextureComponent, ClickableTextureComponent slotComponent)
        {
            entry = obj;
            sprite = clickableTextureComponent;
            slot = slotComponent;
        }
    }
}