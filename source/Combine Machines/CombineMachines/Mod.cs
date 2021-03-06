/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Videogamers0/SDV-CombineMachines
**
*************************************************/

using CombineMachines.Helpers;
using CombineMachines.Patches;
using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SObject = StardewValley.Object;

namespace CombineMachines
{
    public class ModEntry : Mod
    {
        public static Version CurrentVersion = new Version(1, 0, 7); // Last updated 1/28/2021 (Don't forget to update manifest.json)

        private static UserConfig _UserConfig;
        public static UserConfig UserConfig
        {
            get { return _UserConfig; }
            private set
            {
                if (UserConfig != value)
                {
                    _UserConfig = value;

                    //  Parse the actual Enum values (SButton enum) from CombineKeyNames
                    if (UserConfig != null)
                    {
                        UserConfig.CombineKeys = new List<SButton>();
                        HashSet<string> UnrecognizedKeyNames = new HashSet<string>();
                        foreach (string KeyName in UserConfig.CombineKeyNames)
                        {
                            if (Enum.TryParse(KeyName, out SButton Value))
                                UserConfig.CombineKeys.Add(Value);
                            else
                                UnrecognizedKeyNames.Add(KeyName);
                        }

                        if (UnrecognizedKeyNames.Any())
                        {
                            List<string> ValidKeyNames = Enum.GetValues(typeof(SButton)).Cast<SButton>().Select(x => x.ToString()).OrderBy(x => x).ToList();
                            Logger.Log(string.Format("Warning - the following {0} keys were not recognized and have been ignored from your {1} settings: {2}\nValid key names are: {3}",
                                UnrecognizedKeyNames.Count, UserConfig.DefaultFilename, string.Join(", ", UnrecognizedKeyNames), string.Join(", ", ValidKeyNames)), LogLevel.Warn);
                        }
                    }
                }
            }
        }

        public const string ModDataQuantityKey = "SlayerDharok.CombineMachines.CombinedQuantity";
        public const string ModDataOutputModifiedKey = "SlayerDharok.CombineMachines.HasModifiedOutputStack";
        public static ModEntry ModInstance { get; private set; }
        public static IMonitor Logger { get { return ModInstance?.Monitor; } }

        public static bool IsAutomateModInstalled { get; private set; }

#if DEBUG
        internal static LogLevel InfoLogLevel = LogLevel.Debug;
#else
        internal static LogLevel InfoLogLevel = LogLevel.Trace;
#endif

        internal static void LogTrace(int CombinedQuantity, SObject Machine, Vector2 Position, string PropertyName, double PreviousValue, double NewValueBeforeRounding, double NewValue, double Modifier)
        {
            ModInstance.Monitor.Log(string.Format("{0}: ({1}) - Modified {2} at ({3},{4}) - Changed {5} from {6} to {7} ({8}% / Desired Value = {9})",
                nameof(CombineMachines), CombinedQuantity, Machine.DisplayName, Position.X, Position.Y, PropertyName, PreviousValue, NewValue, (Modifier * 100.0).ToString("0.##"), NewValueBeforeRounding), InfoLogLevel);
        }

        public override void Entry(IModHelper helper)
        {
            ModInstance = this;

            IsAutomateModInstalled = helper.ModRegistry.IsLoaded("Pathoschild.Automate");

            helper.Events.Input.ButtonPressed += Input_ButtonPressed;
            helper.Events.Display.RenderedWorld += Display_RenderedWorld;
            helper.Events.Input.CursorMoved += Input_CursorMoved;

            string CommandName = "combine_machines_reload_config";
            string CommandHelp = "Reloads settings from the config.json file without requiring you to restart the game.";
            helper.ConsoleCommands.Add(CommandName, CommandHelp, (string Name, string[] Args) => {
                LoadUserConfig();
                Logger.Log(string.Format("Configuration settings successfully reloaded. (Some settings may still require a full restart, such as {0}.", nameof(UserConfig.AllowCombiningScarecrows)), LogLevel.Info);
            });

            LoadUserConfig();

            DelayHelpers.Entry(helper);
            ModDataPersistenceHelper.Entry(helper, ModDataQuantityKey, ModDataOutputModifiedKey);
            PatchesHandler.Entry(helper);
        }

        internal static void LoadUserConfig()
        {
            //  Load global user settings into memory
            UserConfig GlobalUserConfig = UserConfig.Load(ModInstance.Helper.Data);
#if DEBUG
            //GlobalUserConfig = null; // Force full refresh of config file for testing purposes
#endif
            if (GlobalUserConfig == null || GlobalUserConfig.CreatedByVersion < CurrentVersion)
            {
                if (GlobalUserConfig == null)
                    GlobalUserConfig = new UserConfig();
                GlobalUserConfig.CreatedByVersion = CurrentVersion;
                ModInstance.Helper.Data.WriteJsonFile(UserConfig.DefaultFilename, GlobalUserConfig);
            }
            UserConfig = GlobalUserConfig;
        }

        /// <summary>Does not account for <see cref="Game1"/>.<see cref="Options.uiScale"/></summary>
        internal static Vector2 MouseScreenPosition { get; private set; }
        internal static Vector2 HoveredTile { get; private set; }

        private void Input_CursorMoved(object sender, CursorMovedEventArgs e)
        {
            MouseScreenPosition = e.NewPosition.LegacyScreenPixels();
            HoveredTile = new Vector2(
                (int)((Game1.viewport.X + MouseScreenPosition.X / (Game1.options.zoomLevel / Game1.options.uiScale)) / Game1.tileSize),
                (int)((Game1.viewport.Y + MouseScreenPosition.Y / (Game1.options.zoomLevel / Game1.options.uiScale)) / Game1.tileSize)
            );
        }

        private void Display_RenderedWorld(object sender, RenderedWorldEventArgs e)
        {
            if (Game1.activeClickableMenu == null)
            {
                GameLocation CurrentLocation = Game1.player.currentLocation;

                bool IsHoveringPlacedObject = CurrentLocation.Objects.TryGetValue(HoveredTile, out SObject HoveredObject);
                if (IsHoveringPlacedObject)
                {
                    if (UserConfig.DrawToolTip)
                    {
                        //  Draw a tooltip that shows how many of the machine were combined, and its total combined processing power
                        //  Such as: "Quantity: 5\nPower: 465%"
                        if (HoveredObject.TryGetCombinedQuantity(out int CombinedQuantity))
                        {
                            Cask Cask = HoveredObject as Cask;
                            bool IsCask = Cask != null;
                            CrabPot CrabPot = HoveredObject as CrabPot;
                            bool IsCrabPot = CrabPot != null;
                            bool IsScarecrow = HoveredObject.IsScarecrow();

                            bool HasHeldObject = HoveredObject.heldObject?.Value != null;

                            float UIScaleFactor = Game1.options.zoomLevel / Game1.options.uiScale;

                            SpriteFont DefaultFont = Game1.dialogueFont;
                            int Padding = 25;
                            int MarginBetweenColumns = 10;
                            int MarginBetweenRows = 5;
                            float LabelTextScale = 0.75f;
                            float ValueTextScale = 1.0f;

                            bool ShowDurationInfo = UserConfig.ToolTipShowDuration && UserConfig.ShouldModifyProcessingSpeed(HoveredObject);
                            bool ShowQuantityInfo = UserConfig.ToolTipShowQuantity && UserConfig.ShouldModifyInputsAndOutputs(HoveredObject);

                            //  Compute row headers
                            List<string> RowHeaders = new List<string>() { Helper.Translation.Get("ToolTipQuantityLabel"), Helper.Translation.Get("ToolTipPowerLabel") };
                            if (ShowDurationInfo)
                            {
                                if (IsCask)
                                {
                                    //RowHeaders.Add(Helper.Translation.Get("ToolTipCaskAgingRateLabel"));
                                    if (HasHeldObject)
                                        RowHeaders.Add(Helper.Translation.Get("ToolTipCaskDaysUntilIridiumLabel"));
                                }
                                else if (IsCrabPot)
                                {
                                    RowHeaders.Add(Helper.Translation.Get("ToolTipCrabPotProcessingIntervalLabel"));
                                }
                                else
                                {
                                    if (HasHeldObject)
                                        RowHeaders.Add(Helper.Translation.Get("ToolTipMinutesRemainingLabel"));
                                }
                            }
                            if (ShowQuantityInfo)
                            {
                                if (HasHeldObject && HoveredObject.HasModifiedOutput())
                                    RowHeaders.Add(Helper.Translation.Get("ToolTipProducedQuantityLabel"));
                            }
                            if (IsScarecrow)
                            {
                                RowHeaders.Add(Helper.Translation.Get("ToolTipScarecrowRadiusLabel"));
                            }
                            List<Vector2> RowHeaderSizes = RowHeaders.Select(x => DefaultFont.MeasureString(x) * LabelTextScale).ToList();

                            double ProcessingPower = UserConfig.ComputeProcessingPower(CombinedQuantity) * 100.0;
                            string FormattedProcessingPower = string.Format("{0}%", ProcessingPower.ToString("0.#"));

                            //  Compute row values
                            List<string> RowValues = new List<string>() { CombinedQuantity.ToString(), FormattedProcessingPower };
                            if (ShowDurationInfo)
                            {
                                if (IsCask)
                                {
                                    //RowValues.Add(Cask.agingRate.Value.ToString("0.##"));
                                    if (HasHeldObject)
                                        RowValues.Add(Math.Ceiling(Cask.daysToMature.Value / Cask.agingRate.Value).ToString("0.##"));
                                }
                                else if (IsCrabPot)
                                {
                                    CrabPot.TryGetProcessingInterval(out double Power, out double IntervalHours, out int IntervalMinutes);
                                    RowValues.Add(IntervalMinutes.ToString());
                                }
                                else
                                {
                                    if (HasHeldObject)
                                        RowValues.Add(HoveredObject.MinutesUntilReady.ToString("0.##"));
                                }
                            }
                            if (ShowQuantityInfo)
                            {
                                if (HasHeldObject && HoveredObject.HasModifiedOutput())
                                    RowValues.Add(HoveredObject.heldObject.Value.Stack.ToString());
                            }
                            if (IsScarecrow)
                            {
                                //  Subtract 1 because the game internally counts the scarecrow's occupied tile as part of its radius, but users usually would be confused by that
                                //  So a typical user expects radius=8 for a regular scarecrow, even though the game does its calculations with radius=9
                                int OriginalRadius = HoveredObject.GetScarecrowBaseRadius() - 1;
                                int AlteredRadius = HoveredObject.GetScarecrowRadius() - 1;
                                //RowValues.Add(string.Format("{0}-->{1}", OriginalRadius, AlteredRadius));
                                RowValues.Add(AlteredRadius.ToString());
                            }
                            List<Vector2> RowValueSizes = RowValues.Select(x => DrawHelpers.MeasureStringWithSpecialNumbers(x, ValueTextScale, 0.0f)).ToList();

                            //  Measure the tooltip
                            List<int> RowHeights = new List<int>();
                            for (int i = 0; i < RowHeaders.Count; i++)
                            {
                                RowHeights.Add((int)Math.Max(RowHeaderSizes[i].Y, RowValueSizes[i].Y));
                            }

                            List<int> ColumnWidths = new List<int> {
                                (int)RowHeaderSizes.Max(x => x.X),
                                (int)RowValueSizes.Max(x => x.X)
                            };

                            int ToolTipTopWidth = Padding + ColumnWidths.Sum() + (ColumnWidths.Count - 1) * MarginBetweenColumns + Padding;
                            int ToolTipHeight = Padding + RowHeights.Sum() + (RowHeights.Count - 1) * MarginBetweenRows + Padding;
                            Point ToolTipTopleft = DrawHelpers.GetTopleftPosition(new Point(ToolTipTopWidth, ToolTipHeight), 
                                new Point((int)((MouseScreenPosition.X + UserConfig.ToolTipOffset.X) / UIScaleFactor), (int)((MouseScreenPosition.Y + UserConfig.ToolTipOffset.Y) / UIScaleFactor)), 100);

                            //  Draw tooltip background
                            DrawHelpers.DrawBox(e.SpriteBatch, new Rectangle(ToolTipTopleft.X, ToolTipTopleft.Y, ToolTipTopWidth, ToolTipHeight));

                            //  Draw each row's header and value
                            int CurrentY = ToolTipTopleft.Y + Padding;
                            for (int i = 0; i < RowHeights.Count; i++)
                            {
                                int CurrentRowHeight = RowHeights[i];

                                //  Draw the row header
                                Vector2 RowHeaderPosition = new Vector2(
                                    ToolTipTopleft.X + Padding + ColumnWidths[0] - RowHeaderSizes[i].X,
                                    CurrentY + (RowHeights[i] - RowHeaderSizes[i].Y) / 2.0f
                                );
                                e.SpriteBatch.DrawString(DefaultFont, RowHeaders[i], RowHeaderPosition, Color.Black, 0.0f, Vector2.Zero, LabelTextScale, SpriteEffects.None, 1.0f);

                                //  Draw the row value
                                Vector2 RowValuePosition = new Vector2(
                                    ToolTipTopleft.X + Padding + ColumnWidths[0] + MarginBetweenColumns,
                                    CurrentY + (RowHeights[i] - RowValueSizes[i].Y) / 2.0f
                                );
                                DrawHelpers.DrawStringWithSpecialNumbers(e.SpriteBatch, RowValuePosition, RowValues[i], ValueTextScale, Color.White);

                                CurrentY += CurrentRowHeight + MarginBetweenRows;
                            }
                        }
                    }
                }
            }
        }

        private void Input_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
#if NEVER //DEBUG
            //  Testing the Scarecrow transpiler harmony patch
            if (e.Button == SButton.R && Game1.activeClickableMenu == null && Game1.player.currentLocation is Farm farm)
            {
                farm.addCrows();
            }
#endif

            if (IsCombineKeyHeld(Helper.Input))
            {
                if (Game1.activeClickableMenu is GameMenu GM && GM.currentTab == GameMenu.inventoryTab && TryGetClickedInventoryItem(GM, e, out Item ClickedItem, out int ClickedItemIndex))
                {
                    //  Detect when player clicks on a machine in their inventory while another machine of the same type is selected
                    if (e.Button == SButton.MouseLeft && Game1.player.CursorSlotItem is SObject SourceObj && ClickedItem is SObject TargetObj && CanCombine(SourceObj, TargetObj))
                    {
                        if (!SourceObj.TryGetCombinedQuantity(out int SourceQuantity))
                            SourceQuantity = SourceObj.Stack;
                        if (!TargetObj.TryGetCombinedQuantity(out int TargetQuantity))
                            TargetQuantity = TargetObj.Stack;

                        TargetObj.SetCombinedQuantity(SourceQuantity + TargetQuantity);
                        Game1.player.CursorSlotItem = null;

                        //  Clicking an item will make the game set it to the new CursorSlotItem, but since we just combined them, we want the CursorSlotItem to be empty
                        DelayHelpers.InvokeLater(1, () =>
                        {
                            if (Game1.player.CursorSlotItem != null && Game1.player.Items[ClickedItemIndex] == null)
                            {
                                Game1.player.Items[ClickedItemIndex] = Game1.player.CursorSlotItem;
                                Game1.player.CursorSlotItem = null;
                            }
                        });
                    }
                    //  Right-clicking a combined machine splits it to its un-combined state
                    else if (e.Button == SButton.MouseRight && Game1.player.CursorSlotItem == null && ClickedItem is SObject ClickedObject 
                        && ClickedObject.IsCombinedMachine() && ClickedObject.TryGetCombinedQuantity(out int CombinedQuantity) && CombinedQuantity > 1)
                    {
                        ClickedObject.modData.Remove(ModDataQuantityKey);
                        ClickedObject.Stack += CombinedQuantity - 1;
                        Logger.Log(string.Format("De-combined {0} (Stack={1})", ClickedObject.DisplayName, CombinedQuantity), InfoLogLevel);
                    }
                }
            }
        }

        internal static bool IsCombineKeyHeld(IInputHelper InputHelper)
        {
            return UserConfig.CombineKeys.Any(x => InputHelper.IsDown(x));
        }

        private static bool TryGetClickedInventoryItem(GameMenu GM, ButtonPressedEventArgs e, out Item Result, out int Index)
        {
            Result = null;
            Index = -1;

            if (GM == null || GM.currentTab != GameMenu.inventoryTab)
                return false;

            InventoryPage InvPage = GM.pages.First(x => x is InventoryPage) as InventoryPage;
            InventoryMenu InvMenu = InvPage.inventory;

            Vector2 CursorPos = e.Cursor.LegacyScreenPixels();
            int ClickedItemIndex = InvMenu.getInventoryPositionOfClick((int)CursorPos.X, (int)CursorPos.Y);
            bool IsValidInventorySlot = ClickedItemIndex >= 0 && ClickedItemIndex < InvMenu.actualInventory.Count;
            if (!IsValidInventorySlot)
                return false;

            Result = InvMenu.actualInventory[ClickedItemIndex];
            Index = ClickedItemIndex;
            return Result != null;
        }

        internal static bool CanCombine(SObject Machine1, SObject Machine2)
        {
            return Machine1 != null && Machine2 != null &&
                Machine1.IsCombinableObject() && Machine2.IsCombinableObject() &&
                Machine1.Stack >= 1 && Machine2.Stack >= 1 &&
                Machine1.ParentSheetIndex == Machine2.ParentSheetIndex &&
                (Machine1.IsCombinedMachine() || Machine2.IsCombinedMachine() || Machine1.canStackWith(Machine2));
        }
    }
}
