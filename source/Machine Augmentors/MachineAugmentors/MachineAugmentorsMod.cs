using Harmony;
using MachineAugmentors.Harmony;
using MachineAugmentors.Helpers;
using MachineAugmentors.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Object = StardewValley.Object;

namespace MachineAugmentors
{
    public class MachineAugmentorsMod : Mod
    {
        public static Version CurrentVersion = new Version(1, 0, 6); // Last updated 4/22/2020 (Don't forget to update manifest.json)
        public const string ModUniqueId = "SlayerDharok.MachineAugmentors";

        private const string UserConfigFilename = "config.json";
        public static UserConfig UserConfig { get; private set; }
        private const string MachineConfigFilename = "custom_machines.json";
        public static MachineConfig MachineConfig { get; private set; }

        internal static MachineAugmentorsMod ModInstance { get; private set; }
        internal static string Translate(string Key, Dictionary<string, string> Parameters = null)
        {
            if (Parameters != null)
                return ModInstance.Helper.Translation.Get(Key, Parameters);
            else
                return ModInstance.Helper.Translation.Get(Key);
        }

        //TODO Multiplayer support -
        //1. When a farmhand connects to host, host client has to send them a json string of the augmentor data.
        //(new SerializablePlacedAugmentors(PlacedAugmentorsManager.Instance) ) ----- convert to a Json string
        //when the farmhand receives it, deserialize back to a SerializablePlacedAugmentors, then call PlacedAugmentorsManager.Instance.LoadSettings(...)
        //2. When a client (host or farmhand) places an augmentor, need to send info about what they placed, quantity, gamelocation, tile location to all other clients
        //so the other clients can update their PlacedAugmentorsManager.Instance data.

        internal static void LogTrace(AugmentorType Type, int Quantity, Object Machine, bool RequiresInput, Vector2 Position, string PropertyName, double PreviousValue, double NewValueBeforeRounding, double NewValue, double Modifier)
        {
#if DEBUG
            LogLevel LogLevel = LogLevel.Debug;
#else
            LogLevel LogLevel = LogLevel.Trace;
#endif
            ModInstance.Monitor.Log(string.Format("MachineAugmentors: {0} ({1}) - Modified {2}{3} at ({4},{5}) - Changed {6} from {7} to {8} ({9}% / Desired Value = {10})",
                Type.ToString(), Quantity, Machine.DisplayName, RequiresInput ? "" : " (Inputless)", Position.X, Position.Y, PropertyName, PreviousValue, NewValue, (Modifier * 100.0).ToString("0.##"), NewValueBeforeRounding), LogLevel);
        }

        public override void Entry(IModHelper helper)
        {
            ModInstance = this;

            LoadUserConfig();

            //  Load custom machine settings
            MachineConfig GlobalMachineConfig = helper.Data.ReadJsonFile<MachineConfig>(Path.Combine("assets", MachineConfigFilename));
#if DEBUG
            //GlobalMachineConfig = null; // Force full refresh of config file for testing purposes
#endif
            if (GlobalMachineConfig == null)
            {
                GlobalMachineConfig = new MachineConfig();
                helper.Data.WriteJsonFile(MachineConfigFilename, GlobalMachineConfig);
            }
            MachineConfig = GlobalMachineConfig;
            MachineInfo.LoadAugmentableMachineData();

            MachineInfo.IsPrismaticToolsModInstalled = helper.ModRegistry.IsLoaded("stokastic.PrismaticTools");

            Helper.Events.Display.RenderedWorld += Display_RenderedWorld;
            Helper.Events.GameLoop.Saving += GameLoop_Saving;
            Helper.Events.GameLoop.SaveLoaded += GameLoop_SaveLoaded;
            Helper.Events.GameLoop.UpdateTicked += GameLoop_UpdateTicked;
            Helper.Events.Input.CursorMoved += Input_CursorMoved;
            Helper.Events.Display.MenuChanged += Display_MenuChanged;
            helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;

#region Game Patches
            HarmonyInstance Harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);

            //  Patch Object.performObjectDropInAction, so that we can detect when items are put into a machine, and then modify the output based on attached augmentors
            Harmony.Patch(
               original: AccessTools.Method(typeof(Object), nameof(Object.performObjectDropInAction)),
               prefix: new HarmonyMethod(typeof(GamePatches), nameof(GamePatches.PerformObjectDropInAction_Prefix)),
               postfix: new HarmonyMethod(typeof(GamePatches), nameof(GamePatches.PerformObjectDropInAction_Postfix))
            );

            //  Patch Object.checkForAction, so that we can detect when processed items are collected from machines that don't require input,
            //  and then we can modify the new processed items based on attached augmentors
            Harmony.Patch(
               original: AccessTools.Method(typeof(Object), nameof(Object.checkForAction)),
               prefix: new HarmonyMethod(typeof(GamePatches), nameof(GamePatches.CheckForAction_Prefix)),
               postfix: new HarmonyMethod(typeof(GamePatches), nameof(GamePatches.CheckForAction_Postfix))
            );

            //  Patch Object.draw, so that we can draw icons of the attached augmentors after the object is drawn to a tile of the game world
            Harmony.Patch(
                original: AccessTools.Method(typeof(Object), nameof(Object.draw), new Type[] { typeof(SpriteBatch), typeof(int), typeof(int), typeof(float) }),
                postfix: new HarmonyMethod(typeof(GamePatches), nameof(GamePatches.Draw_Postfix))
            );

            //  Patch GameLocation.monsterDrop, so that we can give a small chance of making monsters also drop augmentors
            Harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.monsterDrop)),
                postfix: new HarmonyMethod(typeof(GamePatches), nameof(GamePatches.MonsterDrop_Postfix))
            );
#endregion Game Patches

            RegisterConsoleCommands();
        }

        internal static void LoadUserConfig()
        {
            //  Load global user settings into memory
            UserConfig GlobalUserConfig = ModInstance.Helper.Data.ReadJsonFile<UserConfig>(UserConfigFilename);
#if DEBUG
            //GlobalUserConfig = null; // Force full refresh of config file for testing purposes
#endif
            if (GlobalUserConfig != null)
            {
                bool RewriteConfig = false;

                //  Version 1.0.5 added additional settings to augmentor formulas
                if (GlobalUserConfig.CreatedByVersion < new Version(1, 0, 5))
                {
                    RewriteConfig = true;
                }

                if (RewriteConfig)
                {
                    GlobalUserConfig.CreatedByVersion = CurrentVersion;
                    ModInstance.Helper.Data.WriteJsonFile(UserConfigFilename, GlobalUserConfig);
                }
            }
            else
            {
                GlobalUserConfig = new UserConfig() { CreatedByVersion = CurrentVersion };
                ModInstance.Helper.Data.WriteJsonFile(UserConfigFilename, GlobalUserConfig);
            }
            GlobalUserConfig.AfterLoaded();
            UserConfig = GlobalUserConfig;
        }

        private void GameLoop_UpdateTicked(object sender, StardewModdingAPI.Events.UpdateTickedEventArgs e)
        {
            if (Context.IsWorldReady)
                PlacedAugmentorsManager.Instance?.Update();
        }

        private void GameLoop_DayStarted(object sender, StardewModdingAPI.Events.DayStartedEventArgs e)
        {
            TodaysStock = null;
        }

        private static Dictionary<ISalable, int[]> TodaysStock = null;

        private void Display_MenuChanged(object sender, StardewModdingAPI.Events.MenuChangedEventArgs e)
        {
            //  Add Augmentors items to the shop's stock
            if (e.NewMenu is ShopMenu NewShop && IsTravellingMerchantShop(NewShop))
            {
                if (TodaysStock == null)
                {
                    TodaysStock = new Dictionary<ISalable, int[]>();

                    //  Pick the Augmentor types that will be sold today
                    int NumTypes = Augmentor.WeightedRound(UserConfig.ShopSettings.NumAugmentorTypesInShop);
                    List<AugmentorConfig> ChosenTypes = new List<AugmentorConfig>();
                    List<AugmentorConfig> RemainingTypes = new List<AugmentorConfig>(UserConfig.AugmentorConfigs);
                    while (ChosenTypes.Count < NumTypes && RemainingTypes.Any())
                    {
                        int TotalWeight = RemainingTypes.Sum(x => x.ShopAppearanceWeight);
                        int ChosenWeight = Augmentor.Randomizer.Next(TotalWeight);

                        //  EX: If the remaining types had weights = { 3, 6, 2 }, picks random number from 0 to 10 inclusive.
                        //  Then the first is selected if ChosenWeight is 0-2 (3/11 chance), second is selected if 3-8 (6/11 chance), third is selected if 9-10 (2/11 chance)
                        int CurrentSum = 0;
                        for (int i = 0; i < RemainingTypes.Count; i++)
                        {
                            AugmentorConfig CurrentConfig = RemainingTypes[i];
                            CurrentSum += CurrentConfig.ShopAppearanceWeight;
                            if (ChosenWeight < CurrentSum)
                            {
                                ChosenTypes.Add(CurrentConfig);
                                RemainingTypes.RemoveAt(i);
                                break;
                            }
                        }
                    }

                    //  Add each type to today's stock
                    foreach (AugmentorConfig Config in ChosenTypes)
                    {
                        //  Compute price
                        double BasePrice = Config.BasePrice * UserConfig.GlobalPriceMultiplier;
                        double Price = BasePrice;
                        if (UserConfig.ShopSettings.PriceDeviationRolls > 0)
                        {
                            double MinMultiplier = 1.0 - UserConfig.ShopSettings.PriceDeviation;
                            double MaxMultiplier = 1.0 + UserConfig.ShopSettings.PriceDeviation;
                            double Multiplier = Enumerable.Range(0, UserConfig.ShopSettings.PriceDeviationRolls).Select(x => Augmentor.GetRandomNumber(MinMultiplier, MaxMultiplier)).Average();
                            Price = Math.Round(BasePrice * Multiplier, MidpointRounding.AwayFromZero);
                        }

                        //  Compute quantity
                        double BaseQuantityInStock = UserConfig.ShopSettings.BaseQuantityInStock;
                        double YearMultiplier = 1.0 + (UserConfig.ShopSettings.YearShopStockMultiplierBonus * (Game1.Date.Year - 1));
                        double DesiredValue = BaseQuantityInStock * Config.ShopStockMultiplier * YearMultiplier;
                        int QuantityInStock = Math.Max(1, Augmentor.WeightedRound(DesiredValue));

                        Augmentor SellableInstance = Augmentor.CreateInstance(Config.AugmentorType, 1);
                        TodaysStock.Add(SellableInstance, new int[] { (int)Price, QuantityInStock });
                    }
                }

                //  Add today's stock to the shop
                if (TodaysStock.Any())
                {
                    Dictionary<ISalable, int[]> Stock = NewShop.itemPriceAndStock;
                    foreach (KeyValuePair<ISalable, int[]> Item in TodaysStock)
                    {
                        if (Item.Value[1] > 0 && !Stock.ContainsKey(Item.Key))
                            Stock.Add(Item.Key, Item.Value);
                    }

                    NewShop.setItemPriceAndStock(Stock);
                }
            }
        }

        private static bool IsTravellingMerchantShop(ShopMenu Menu)
        {
            return Menu != null && Menu.portraitPerson == null && Menu.storeContext != null && Menu.storeContext.Equals("Forest", StringComparison.CurrentCultureIgnoreCase)
                    && Menu.onPurchase?.GetMethodInfo().Name == "onTravelingMerchantShopPurchase";
        }

        private void RegisterConsoleCommands()
        {
            List<string> ValidTypes = Enum.GetValues(typeof(AugmentorType)).Cast<AugmentorType>().Select(x => x.ToString()).ToList();

            //Possible TODO: Add translation support for this command
            string CommandName = "player_addaugmentor";
            string CommandHelp = string.Format("Adds augmentors of the given AugmentorType into your inventory.\n"
                + "Arguments: <AugmentorType> <Quantity>\n"
                + "Example: {0} EfficiencyAugmentor 6\n\n"
                + "Valid values for <AugmentorType>: {1}",
                CommandName, string.Join(", ", ValidTypes), string.Join(", ", ValidTypes));
            Helper.ConsoleCommands.Add(CommandName, CommandHelp, (string Name, string[] Args) =>
            {
                if (Game1.player.isInventoryFull())
                {
                    Monitor.Log("Unable to execute command: Inventory is full!", LogLevel.Alert);
                }
                else if (Args.Length < 2)
                {
                    Monitor.Log("Unable to execute command: Required arguments missing!", LogLevel.Alert);
                }
                else
                {
                    string TypeName = Args[0];
                    if (!Enum.TryParse(TypeName, out AugmentorType AugmentorType))
                    {
                        Monitor.Log(string.Format("Unable to execute command: <AugmentorType> \"{0}\" is not valid. Expected valid values: {1}", TypeName, string.Join(", ", ValidTypes)), LogLevel.Alert);
                    }
                    else
                    {
                        if (!int.TryParse(Args[1], out int Quantity))
                        {
                            Monitor.Log(string.Format("Unable to execute command: could not parse an integer from \"{0}\".", Args[1]), LogLevel.Alert);
                        }
                        else
                        {
                            Augmentor SpawnedItem = Augmentor.CreateInstance(AugmentorType, Quantity);
                            Game1.player.addItemToInventory(SpawnedItem);
                        }
                    }
                }
            });

            //Possible TODO: Add translation support for this command
            CommandName = "machine_augmentors_reload_config";
            CommandHelp = "Reloads configuration settings from this mod's config.json file. Normally this file's settings are only loaded once when the game is started."
                + " Use this command if you've made changes to the config during this game session.";
            Helper.ConsoleCommands.Add(CommandName, CommandHelp, (string Name, string[] Args) =>
            {
                try
                {
                    LoadUserConfig();
                    Monitor.Log("config.json settings were successfully reloaded.", LogLevel.Alert);
                }
                catch (Exception ex)
                {
                    Monitor.Log(string.Format("Machine Augmentors: Unhandled error while executing command: {0}", ex.Message), LogLevel.Error);
                }
            });
        }

        private void GameLoop_Saving(object sender, StardewModdingAPI.Events.SavingEventArgs e)
        {
            if (!Context.IsMultiplayer || Context.IsMainPlayer)
            {
                Helper.Data.WriteSaveData(PlacedAugmentorsManager.SavedDataKey, new SerializablePlacedAugmentors(PlacedAugmentorsManager.Instance));
            }
        }

        internal static Point MouseScreenPosition { get; private set; }
        internal static Vector2 HoveredTile { get; private set; }

        private void Input_CursorMoved(object sender, StardewModdingAPI.Events.CursorMovedEventArgs e)
        {
            MouseScreenPosition = e.NewPosition.ScreenPixels.AsAndroidCompatibleCursorPoint();
            HoveredTile = e.NewPosition.Tile;
        }

        private void GameLoop_OneSecondUpdateTicked(object sender, StardewModdingAPI.Events.OneSecondUpdateTickedEventArgs e)
        {
            if (Context.IsWorldReady)
                PlacedAugmentorsManager.Instance?.Update();
        }

        private void GameLoop_SaveLoaded(object sender, StardewModdingAPI.Events.SaveLoadedEventArgs e)
        {
            if (!Context.IsWorldReady)
                throw new InvalidOperationException("Context not ready in GameLoop_SavedLoaded. Cannot read required data from the Game's GameLocations.");

            TodaysStock = null;
            PlacedAugmentorsManager.Instance = new PlacedAugmentorsManager();
        }

        private void Display_RenderedWorld(object sender, StardewModdingAPI.Events.RenderedWorldEventArgs e)
        {
            if (Game1.activeClickableMenu == null && PlacedAugmentorsManager.Instance != null)
            {
                GameLocation CurrentLocation = Game1.player.currentLocation;

                bool IsHoveringPlacedObject = CurrentLocation.Objects.TryGetValue(HoveredTile, out Object HoveredObject);
                if (IsHoveringPlacedObject)
                {
                    Dictionary<AugmentorType, int> AttachedAugmentors = PlacedAugmentorsManager.Instance.GetAugmentorQuantities(CurrentLocation.NameOrUniqueName, HoveredTile);
                    bool HasAttachedAugmentors = AttachedAugmentors.Any(x => x.Value > 0);
                    if (MachineInfo.TryGetMachineInfo(HoveredObject, out MachineInfo MI))
                    {
                        SpriteFont DefaultFont = Game1.dialogueFont;

                        //  Draw a tooltip showing what effects the held item will have when attached to this machine
                        if (Game1.player.CurrentItem is Augmentor HeldAugmentor && HeldAugmentor.IsAugmentable(HoveredObject))
                        {
                            AttachedAugmentors.TryGetValue(HeldAugmentor.AugmentorType, out int CurrentAttachedQuantity);

                            double CurrentEffect = CurrentAttachedQuantity <= 0 ?
                                Augmentor.GetDefaultEffect(HeldAugmentor.AugmentorType) : Augmentor.ComputeEffect(HeldAugmentor.AugmentorType, CurrentAttachedQuantity, MI.RequiresInput);
                            double NewEffectSingle = Augmentor.ComputeEffect(HeldAugmentor.AugmentorType, CurrentAttachedQuantity + 1, MI.RequiresInput);
                            double NewEffectAll = Augmentor.ComputeEffect(HeldAugmentor.AugmentorType, CurrentAttachedQuantity + HeldAugmentor.Stack, MI.RequiresInput);

                            //  Example of desired tooltip:
                            //        -------------------------------------------
                            //        |          [Icon] Time to process:        |
                            //        |-----------------------------------------|
                            //        |  Current Effect: 95.5%                  |
                            //        |      New Effect: 92.2% (1) / 81.1% (4)  |
                            //        -------------------------------------------

                            int Padding = 28;
                            int MarginAfterIcon = 10;
                            float LabelTextScale = 0.75f;
                            float ValueTextScale = 1.0f;

                            //  Compute sizes of each row so we know how big the tooltip is, and can horizontally center the header and other rows

                            //  Compute size of header
                            int HeaderHorizontalPadding = 4;
                            Augmentor.TryGetIconDetails(HeldAugmentor.AugmentorType, out Texture2D IconTexture, out Rectangle IconSourcePosition, out float IconScale, out SpriteEffects IconEffects);
                            Vector2 IconSize = new Vector2(IconSourcePosition.Width * IconScale, IconSourcePosition.Height * IconScale);
                            string HeaderText = string.Format("{0}:", HeldAugmentor.GetEffectDescription());
                            Vector2 HeaderTextSize = DefaultFont.MeasureString(HeaderText) * LabelTextScale;
                            float HeaderRowWidth = IconSize.X + MarginAfterIcon + HeaderTextSize.X + HeaderHorizontalPadding * 2;
                            float HeaderRowHeight = Math.Max(IconSize.Y, HeaderTextSize.Y);

                            //  Compute size of horizontal separator
                            int HorizontalSeparatorHeight = 6;
                            int HorizontalSeparatorMargin = 8;

                            //  Compute size of the labels before the effect values
                            int MarginAfterLabel = 8;
                            string CurrentEffectLabel = string.Format("{0}:", Translate("CurrentEffectLabel"));
                            Vector2 CurrentEffectLabelSize = DefaultFont.MeasureString(CurrentEffectLabel) * LabelTextScale;
                            string NewEffectLabel = string.Format("{0}:", Translate("NewEffectLabel"));
                            Vector2 NewEffectLabelSize = DefaultFont.MeasureString(NewEffectLabel) * LabelTextScale;
                            float EffectLabelWidth = Math.Max(CurrentEffectLabelSize.X, NewEffectLabelSize.X);
                            Vector2 EffectLabelSize = new Vector2(EffectLabelWidth, CurrentEffectLabelSize.Y + NewEffectLabelSize.Y);

                            //  Compute size of the effect values
                            string CurrentEffectValue = string.Format("{0}% ({1})", (CurrentEffect * 100.0).ToString("0.##"), CurrentAttachedQuantity);
                            Vector2 CurrentEffectValueSize = DrawHelpers.MeasureStringWithSpecialNumbers(CurrentEffectValue, ValueTextScale, 0.0f);
                            string NewEffectValue;
                            if (HeldAugmentor.Stack > 1)
                            {
                                NewEffectValue = string.Format("{0}% ({1}) / {2}% ({3})",
                                    (NewEffectSingle * 100.0).ToString("0.##"), (CurrentAttachedQuantity + 1), (NewEffectAll * 100.0).ToString("0.##"), (CurrentAttachedQuantity + HeldAugmentor.Stack));
                            }
                            else
                            {
                                NewEffectValue = string.Format("{0}% ({1})", (NewEffectSingle * 100.0).ToString("0.##"), (CurrentAttachedQuantity + 1));
                            }
                            Vector2 NewEffectValueSize = DrawHelpers.MeasureStringWithSpecialNumbers(NewEffectValue, ValueTextScale, 0.0f);

                            Vector2 EffectContentSize = new Vector2(EffectLabelWidth + MarginAfterLabel + Math.Max(CurrentEffectValueSize.X, NewEffectValueSize.X),
                                Math.Max(CurrentEffectLabelSize.Y, CurrentEffectValueSize.Y) + Math.Max(NewEffectLabelSize.Y, NewEffectValueSize.Y));

                            //  Compute total size of tooltip, draw the background
                            Vector2 ToolTipSize = new Vector2(Padding * 2 + Math.Max(HeaderRowWidth, EffectContentSize.X), Padding + HeaderRowHeight + HorizontalSeparatorMargin + HorizontalSeparatorHeight + HorizontalSeparatorMargin + EffectContentSize.Y + Padding);
                            Point ToolTipTopleft = DrawHelpers.GetTopleftPosition(new Point((int)ToolTipSize.X, (int)ToolTipSize.Y), MouseScreenPosition, 100);
                            DrawHelpers.DrawBox(e.SpriteBatch, new Rectangle(ToolTipTopleft.X, ToolTipTopleft.Y, (int)ToolTipSize.X, (int)ToolTipSize.Y));
                            float CurrentY = ToolTipTopleft.Y + Padding;

                            //  Draw the header
                            float HeaderStartX = ToolTipTopleft.X + (ToolTipSize.X - HeaderRowWidth) / 2.0f;
                            Vector2 IconPosition = new Vector2(HeaderStartX, CurrentY + (HeaderRowHeight - IconSize.Y) / 2.0f);
                            e.SpriteBatch.Draw(IconTexture, IconPosition, IconSourcePosition, Color.White, 0f, Vector2.Zero, IconScale, IconEffects, 1f);
                            Vector2 HeaderTextPosition = new Vector2(HeaderStartX + IconSize.X + MarginAfterIcon, CurrentY + (HeaderRowHeight - HeaderTextSize.Y) / 2.0f);
                            e.SpriteBatch.DrawString(DefaultFont, HeaderText, HeaderTextPosition, Color.Black, 0.0f, Vector2.Zero, LabelTextScale, SpriteEffects.None, 1.0f);
                            CurrentY += HeaderRowHeight + HorizontalSeparatorMargin;

                            //  Draw the horizontal separator
                            DrawHelpers.DrawHorizontalSeparator(e.SpriteBatch, ToolTipTopleft.X + Padding, (int)CurrentY, (int)(ToolTipSize.X - 2 * Padding), HorizontalSeparatorHeight);
                            CurrentY += HorizontalSeparatorHeight + HorizontalSeparatorMargin;

                            //  Draw the current effect
                            Vector2 CurrentEffectLabelPosition = new Vector2(ToolTipTopleft.X + Padding + (EffectLabelWidth - CurrentEffectLabelSize.X), CurrentY);
                            Vector2 CurrentEffectValuePosition = new Vector2(ToolTipTopleft.X + Padding + EffectLabelWidth + MarginAfterLabel, CurrentY);
                            e.SpriteBatch.DrawString(DefaultFont, CurrentEffectLabel, CurrentEffectLabelPosition, Color.Black, 0.0f, Vector2.Zero, LabelTextScale, SpriteEffects.None, 1.0f);
                            DrawHelpers.DrawStringWithSpecialNumbers(e.SpriteBatch, CurrentEffectValuePosition, CurrentEffectValue, ValueTextScale, Color.White);
                            CurrentY += Math.Max(CurrentEffectLabelSize.Y, CurrentEffectValueSize.Y);

                            //  Draw the new effect
                            Vector2 NewEffectLabelPosition = new Vector2(ToolTipTopleft.X + Padding + (EffectLabelWidth - NewEffectLabelSize.X), CurrentY);
                            Vector2 NewEffectValuePosition = new Vector2(ToolTipTopleft.X + Padding + EffectLabelWidth + MarginAfterLabel, CurrentY);
                            e.SpriteBatch.DrawString(DefaultFont, NewEffectLabel, NewEffectLabelPosition, Color.Black, 0.0f, Vector2.Zero, LabelTextScale, SpriteEffects.None, 1.0f);
                            DrawHelpers.DrawStringWithSpecialNumbers(e.SpriteBatch, NewEffectValuePosition, NewEffectValue, ValueTextScale, Color.White);
                        }
                        //  Draw a tooltip showing what effects are currently applied to this machine
                        else if (HasAttachedAugmentors)
                        {
                            int Padding = 28;
                            int MarginAfterIcon = 10;

                            //  Compute the size of each icon
                            Dictionary<AugmentorType, Vector2> IconSizes = new Dictionary<AugmentorType, Vector2>();
                            foreach (KeyValuePair<AugmentorType, int> KVP in AttachedAugmentors.Where(x => x.Value > 0))
                            {
                                Augmentor.TryGetIconDetails(KVP.Key, out Texture2D IconTexture, out Rectangle IconSourcePosition, out float IconScale, out SpriteEffects IconEffects);
                                Vector2 IconSize = new Vector2(IconSourcePosition.Width * IconScale, IconSourcePosition.Height * IconScale);
                                IconSizes.Add(KVP.Key, IconSize);
                            }
                            float IconColumnWidth = IconSizes.Values.Max(x => x.Y);

                            //  Compute the size of each row (each row shows the effect of a type of augmentor that has been applied to this machine)
                            Dictionary<AugmentorType, Vector2> RowSizes = new Dictionary<AugmentorType, Vector2>();
                            foreach (KeyValuePair<AugmentorType, int> KVP in AttachedAugmentors.Where(x => x.Value > 0))
                            {
                                double CurrentEffect = Augmentor.ComputeEffect(KVP.Key, KVP.Value, MI.RequiresInput);
                                string Text = string.Format("{0}% ({1})", (CurrentEffect * 100.0).ToString("0.#"), KVP.Value);
                                Vector2 TextSize = DrawHelpers.MeasureStringWithSpecialNumbers(Text, 1.0f, 4.0f);

                                float RowWidth = IconColumnWidth + MarginAfterIcon + TextSize.X;
                                float RowHeight = Math.Max(IconSizes[KVP.Key].Y, TextSize.Y);
                                RowSizes.Add(KVP.Key, new Vector2(RowWidth, RowHeight));
                            }

                            //  Compute total size of tooltip, draw the background
                            Vector2 ToolTipSize = new Vector2(Padding * 2 + RowSizes.Values.Max(x => x.X), Padding * 2 + RowSizes.Values.Sum(x => x.Y));
                            Point ToolTipTopleft = DrawHelpers.GetTopleftPosition(new Point((int)ToolTipSize.X, (int)ToolTipSize.Y), MouseScreenPosition, 100);
                            DrawHelpers.DrawBox(e.SpriteBatch, new Rectangle(ToolTipTopleft.X, ToolTipTopleft.Y, (int)ToolTipSize.X, (int)ToolTipSize.Y));
                            float CurrentY = ToolTipTopleft.Y + Padding;

                            //  Draw each row
                            float RowStartX = ToolTipTopleft.X + Padding;
                            foreach (KeyValuePair<AugmentorType, int> KVP in AttachedAugmentors.Where(x => x.Value > 0))
                            {
                                float CurrentX = RowStartX;
                                float RowHeight = RowSizes[KVP.Key].Y;

                                //  Draw the icon
                                Augmentor.TryGetIconDetails(KVP.Key, out Texture2D IconTexture, out Rectangle IconSourcePosition, out float IconScale, out SpriteEffects IconEffects);
                                Vector2 IconSize = IconSizes[KVP.Key];
                                Vector2 IconPosition = new Vector2(CurrentX + (IconColumnWidth - IconSize.X) / 2.0f, CurrentY + (RowHeight - IconSize.Y) / 2.0f);
                                e.SpriteBatch.Draw(IconTexture, IconPosition, IconSourcePosition, Color.White, 0f, Vector2.Zero, IconScale, IconEffects, 1f);
                                CurrentX += IconColumnWidth + MarginAfterIcon;

                                //  Draw the value
                                double CurrentEffect = Augmentor.ComputeEffect(KVP.Key, KVP.Value, MI.RequiresInput);
                                string Text = string.Format("{0}% ({1})", (CurrentEffect * 100.0).ToString("0.#"), KVP.Value);
                                Vector2 TextSize = DrawHelpers.MeasureStringWithSpecialNumbers(Text, 1.0f, 0.0f);
                                Vector2 TextPosition = new Vector2(CurrentX, CurrentY + (RowHeight - TextSize.Y) / 2.0f);
                                DrawHelpers.DrawStringWithSpecialNumbers(e.SpriteBatch, TextPosition, Text, 1.0f, Color.White);

                                CurrentY += RowHeight;
                            }

                            //Maybe also show MinutesUntilReady if it's not ReadyForHarvest?
                        }
                    }
                }
            }
        }
    }
}
