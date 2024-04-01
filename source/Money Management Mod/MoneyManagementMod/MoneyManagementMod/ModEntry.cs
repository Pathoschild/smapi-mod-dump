/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Tbonetomtom/StardewMods
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using StardewValley.BellsAndWhistles;
using Netcode;
using StardewValley.Monsters;
using GenericModConfigMenu;
using System.Reflection;
using System.Linq;
using StardewValley.Locations;

namespace MoneyManagementMod
{
    public class ModEntry : Mod
    {
        private ModConfig Config = new();
        private PublicMoney? _publicMoney;
        private Dictionary<int, Texture2D>? _backgrounds;
        private Texture2D? _Glow; //i was planning on using this to make the lights glow but i got lazy
        Vector2 position = new(0, 100); //i was planning on adding this to the config at some point
        
        private readonly Dictionary<long, PlayerData> _playerData = new();
        public override void Entry(IModHelper helper)
        {
            this.Config = helper.ReadConfig<ModConfig>();
            _publicMoney = new PublicMoney(7, this, true);
            helper.Events.GameLoop.DayEnding += this.OnDayEnding;
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.Display.RenderingHud += this.OnRenderingHud;
            // helper.Events.Display.RenderedHud += this.OnRenderedHud;
            _backgrounds = new Dictionary<int, Texture2D>();

            for (int i = 1; i <= 8; i++)
            {
                int transferAmount = (int)Math.Pow(10, i - 1);
                _backgrounds[transferAmount] = helper.Content.Load<Texture2D>($"assets/background{i}.png", ContentSource.ModFolder);
            }

            _Glow = helper.Content.Load<Texture2D>("assets/GlowEffect.png", ContentSource.ModFolder);
            /* This code is not included in the final version of the code.
            // this is for debugging
            helper.ConsoleCommands.Add("player_setmoney", "Sets the player's money.\n\nUsage: player_setmoney <value>\n- value: the integer amount.", this.Commands);
            helper.ConsoleCommands.Add("public_setmoney", "Sets the public money.\n\nUsage: public_setmoney <value>\n- value: the integer amount.", this.Commands);
            */
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            helper.Events.GameLoop.Saving += this.OnBeforeSave;
            helper.Events.Display.MenuChanged += this.OnMenuChanged;
            helper.Events.GameLoop.UpdateTicked += Update;
            helper.Events.Multiplayer.ModMessageReceived += OnModMessageReceived;
            helper.Events.Multiplayer.PeerConnected += OnPeerConnected;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        }
        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuIntergrationForMoneyManagementMod>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            // register mod
            configMenu.Register(
                mod: this.ModManifest,
                reset: () => this.Config = new ModConfig(),
                save: () => Helper.WriteConfig(this.Config)
            );
            // add some config options

            configMenu.AddSectionTitle(
               mod: this.ModManifest,
               text: () => "Keybinds"
           );
            configMenu.AddKeybindList(
                mod: this.ModManifest,
                name: () => "Withdraw From Public Account",
                tooltip: () => "will add the amount specified from your private account to the public bal.",
                getValue: () => Config.WithdrawFromPublicAccount,
                setValue: value => Config.WithdrawFromPublicAccount = value
            );
            configMenu.AddKeybindList(
                mod: this.ModManifest,
                name: () => "Deposit To Public Account",
                tooltip: () => "will remove the amount specified from your private account to the public bal.",
                getValue: () => Config.DepositToPublicAccount,
                setValue: value => Config.DepositToPublicAccount = value
            );
            configMenu.AddKeybindList(
                mod: this.ModManifest,
                name: () => "Increase Transfer Amount",
                tooltip: () => "increases the amount of money you transfer to and from the public bal.",
                getValue: () => Config.IncreaseTransferAmount,
                setValue: value => Config.IncreaseTransferAmount = value
            );
            configMenu.AddKeybindList(
                mod: this.ModManifest,
                name: () => "Decrease Transfer Amount",
                tooltip: () => "decreases the amount of money you transfer to and from the public bal.",
                getValue: () => Config.DecreaseTransferAmount,
                setValue: value => Config.DecreaseTransferAmount = value
            );
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => "HUD"
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Display Currency HUD",
                tooltip: () => "Toggle the rendering of the HUD.",
                getValue: () => Config.RenderHUD,
                setValue: value => Config.RenderHUD = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Show HUD In Menus",
                tooltip: () => "Perfect for setting your HUD position",
                getValue: () => Config.ShowHUDInMenus,
                setValue: value => Config.ShowHUDInMenus = value
            );
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Move HUD X",
                tooltip: () => "Moves the HUD along the X axis.",
                getValue: () => Config.HUDX,
                setValue: value => Config.HUDX = value
            );
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Move HUD Y",
                tooltip: () => "Moves the HUD along the Y axis.",
                getValue: () => Config.HUDY,
                setValue: value => Config.HUDY = value
            );
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => "Misc"
            );
            /*
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Distribute Shipping Bin Equally",
                tooltip: () => "splits the proceeds from the sale of goods and crops equally among all online players in the shipping bin.",
                getValue: () => Config.DistributeShippingBinMoneyEqually,
                setValue: value => Config.DistributeShippingBinMoneyEqually = value
            );*/
            configMenu.SetTitleScreenOnlyForNextOptions(mod: this.ModManifest, true);
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Tax Percent",
                tooltip: () => "When players join, \"Tax Percent\" will automatically sync with the main player's game.",
                min: 0,
                max: 100,
                getValue: () => Config.TaxPercent,
                setValue: value => Config.TaxPercent = value
            );
           configMenu.AddParagraph(
                mod: this.ModManifest,
                text: () => "You can only change \"Tax percent\" in the main menu."
            );
            SendTaxPercentileToAllPlayers();
        }
        private void OnPeerConnected(object? sender, PeerConnectedEventArgs e)
        {
            if (Game1.IsMasterGame)
            {
                SendPublicBalToAllPlayers();
                SendTaxPercentileToAllPlayers();
            }
        }
        public void SendTaxPercentileToAllPlayers()
        {
            if (Game1.IsMasterGame)
            {
                var message = new Messages { TaxPercentile = Config.TaxPercent };
                Helper.Multiplayer.SendMessage(message, "TaxPercentileUpdate", new[] { this.ModManifest.UniqueID });
            }
        }
        public void SendPublicBalToAllPlayers()
        {
            var message = new Messages { PublicBal = _publicMoney.PublicBal };
            Helper.Multiplayer.SendMessage(message, "PublicBalUpdate", new[] { this.ModManifest.UniqueID });
            return;
        }
        private void OnModMessageReceived(object? sender, ModMessageReceivedEventArgs e)
        {
            if (e.FromModID == this.ModManifest.UniqueID && e.Type == "PublicBalUpdate")
            {
                var message = e.ReadAs<Messages>();
                _publicMoney.PublicBal = message.PublicBal;
            }
            if (e.FromModID == this.ModManifest.UniqueID && e.Type == "TaxPercentileUpdate")
            {
                var message = e.ReadAs<Messages>();
                Config.TaxPercent = message.TaxPercentile;
            }
            if (e.FromModID == this.ModManifest.UniqueID && e.Type == "TransferRequest" && Game1.IsMasterGame)
            {
                var message = e.ReadAs<Messages>();
                if (message.TransferType == "ToPublic")
                {
                    _publicMoney.TransferToPublic(message.TransferAmount, message.PlayerID);
                }
                    
                else
                    _publicMoney.TransferFromPublic(message.TransferAmount, message.PlayerID);

                SendPublicBalToAllPlayers();
            }
        }
        private void Update(object? sender, UpdateTickedEventArgs e)
        {
            if (Context.IsWorldReady)
            {
                long playerID = Game1.player.UniqueMultiplayerID;

                // Check if the playerID is present in the dictionary
                if (!_playerData.ContainsKey(playerID))
                {
                    // If not, create a new PlayerData instance and add it to the dictionary
                    _playerData[playerID] = new PlayerData();
                }

                PlayerData playerData = _playerData[playerID];

                if (playerData.CanTax)
                {
                    if (playerData.PreviousMoney == -101)
                    {
                        playerData.PreviousMoney = Game1.player.Money;
                    }
                    playerData.CurrentMoney = Game1.player.Money;
                    if (playerData.PreviousMoney < playerData.CurrentMoney)
                    {
                        int moneyDifference = playerData.CurrentMoney - playerData.PreviousMoney;
                        TaxMoney(moneyDifference);
                    }

                    playerData.PreviousMoney = Game1.player.Money;
                }
                else if (playerData.PreviousMoney != Game1.player.Money)
                {
                    playerData.PreviousMoney = Game1.player.Money;
                }
            }
        }
        private void OnMenuChanged(object? sender, MenuChangedEventArgs e)
        {
            
            if (Context.IsWorldReady)
            {
                long playerID = Game1.player.UniqueMultiplayerID;
                PlayerData playerData = _playerData[playerID];

                if (e.NewMenu is ShopMenu)
                {
                    playerData.CanTax = true;
                }
                else
                {
                    playerData.CanTax = false;
                } 
                if (Config.ShowHUDInMenus) {
                    playerData.DrawHUD = true;
                    return;
                }
                if (e.NewMenu is null)
                {
                    playerData.DrawHUD = true;
                }
                else
                {
                    playerData.DrawHUD = false;
                    
                }
            }
        }
        private void OnDayEnding(object? sender, DayEndingEventArgs e)
        {
            int shippingBinValue = CalculateShippingBinValue();

            this.Monitor.Log($"{shippingBinValue}", LogLevel.Debug);
            TaxMoney(shippingBinValue);
            SendPublicBalToAllPlayers();
        }
        /*   private void Commands(string command, string[] args)
           {
               if (command == "player_setmoney")
               {
                   this.Monitor.Log(command, LogLevel.Debug);
                   Game1.player.Money = int.Parse(args[0]);
                   this.Monitor.Log($"OK, set your money to {args[0]}.", LogLevel.Info);
               }
               else if (command == "public_setmoney")
               {
                   this.Monitor.Log(command, LogLevel.Debug);
                   _publicMoney.PublicBal = int.Parse(args[0]);
                   this.Monitor.Log($"OK, set your money to {args[0]}.", LogLevel.Info);
               }
           }*/
        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;
            long playerID = Game1.player.UniqueMultiplayerID;
            PlayerData playerData = _playerData[playerID];
            int[] transferAmounts = { 1, 10, 100, 1000, 10000, 100000, 1000000, 10000000 };
            int currentIndex = Array.IndexOf(transferAmounts, playerData.TransferAmount);
            if (Config.DepositToPublicAccount.JustPressed() || Config.WithdrawFromPublicAccount.JustPressed())
            {
                if (!Game1.IsMasterGame)
                {
                    var message = new Messages
                    {
                        TransferAmount = playerData.TransferAmount,
                        TransferType = Config.DepositToPublicAccount.JustPressed() ? "ToPublic" : "FromPublic",
                        PlayerID = playerID
                    };
                    Helper.Multiplayer.SendMessage(message, "TransferRequest", new[] { this.ModManifest.UniqueID });
                }
                else
                {
                    Game1.player.CanMove = false;
                    if (Config.DepositToPublicAccount.JustPressed())
                        _publicMoney.TransferToPublic(playerData.TransferAmount, Game1.player.UniqueMultiplayerID);
                    else
                        _publicMoney.TransferFromPublic(playerData.TransferAmount, Game1.player.UniqueMultiplayerID);
                    Game1.player.CanMove = true;
                }
            }
            else if (Config.DecreaseTransferAmount.JustPressed())
            {
                Game1.player.CanMove = false; 
                currentIndex--;
                if (currentIndex < 0)
                {
                    currentIndex = transferAmounts.Length - 1;
                }
                Game1.player.CanMove = true;
            }
            else if (Config.IncreaseTransferAmount.JustPressed())
            {
                Game1.player.CanMove = false;
                currentIndex++;
                if (currentIndex >= transferAmounts.Length)
                {
                    currentIndex = 0;
                }
                Game1.player.CanMove = true;
            }
            
            playerData.TransferAmount = transferAmounts[currentIndex];
            
            // Uncomment the following code block to handle button press events for the W, S, A, and D buttons.
            // This code is not included in the final version of the code.
            /* this is for moving the money menu
                    else if (e.Button == SButton.W)
                    {
                        scale.Y -= 16;
                        Monitor.Log($"Scale Y :  : {scale.Y}", LogLevel.Info);
                    }
                    else if (e.Button == SButton.S)
                    {
                        scale.Y += 16;
                        Monitor.Log($"Scale Y :  {scale.Y}", LogLevel.Info);
                    }
                    else if (e.Button == SButton.A)
                    {
                        scale.X -= 27;
                        Monitor.Log($"Scale X : {scale.X}", LogLevel.Info);
                    }
                    else if (e.Button == SButton.D)
                    {
                        scale.X += 27;
                        Monitor.Log($"Scale X : {scale.X}", LogLevel.Info);
                    }
                    else if (e.Button == SButton.NumPad8)
                    {
                        position.Y -= 1;
                        Monitor.Log($"pos y : {position.Y}", LogLevel.Info);
                    }
                    else if (e.Button == SButton.NumPad2)
                    {
                        position.Y += 1;
                        Monitor.Log($"pos y : {position.Y}", LogLevel.Info);
                    }
                    else if (e.Button == SButton.NumPad4)
                    {
                        position.X -= 1;
                        Monitor.Log($"pos x : {position.X}", LogLevel.Info);
                    }
                    else if (e.Button == SButton.NumPad6)
                    {
                        position.X += 1;
                        Monitor.Log($"pos x : {position.X}", LogLevel.Info);
                    }
                    */
        }
        private void OnRenderingHud(object? sender, RenderingHudEventArgs e)
        {
            if (Config.RenderHUD)
            {
                if (!Context.IsWorldReady)
                    return;
                long playerID = Game1.player.UniqueMultiplayerID;
                PlayerData playerData = _playerData[playerID];
                if (playerData.DrawHUD)
                {
                    if (_backgrounds.TryGetValue(playerData.TransferAmount, out Texture2D? background))
                    {
                        Game1.spriteBatch.Draw(background, new Rectangle(Config.HUDX + 4, Config.HUDY + 8, 260, 60), Color.White);
                    }

                    MoneyMenu customMoneyMenu = new(_publicMoney, Config.HUDX, Config.HUDY);
                    customMoneyMenu.draw(Game1.spriteBatch);
                }
            }
            
        }
        private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            if (Context.IsWorldReady)
            {
                if (Game1.IsMasterGame)
                {
                    string key = $"{this.ModManifest.UniqueID}/publicBal";
                    if (Game1.MasterPlayer.modData.TryGetValue(key, out string value))
                    {
                        _publicMoney.PublicBal = int.Parse(value);
                    }


                    else
                    {
                        _publicMoney.PublicBal = 0;
                    }
                }
                long playerID = Game1.player.UniqueMultiplayerID;
                if (!_playerData.ContainsKey(playerID))
                {
                    _playerData[playerID] = new PlayerData
                    {
                        CurrentMoney = Game1.player.Money
                    };
                }
                _playerData[playerID].DrawHUD = true;
                if (Game1.IsMasterGame)
                {
                    SendPublicBalToAllPlayers();
                }
            }
        }
        private void OnBeforeSave(object? sender, SavingEventArgs e)
        {
            if (Context.IsWorldReady)
            {
                string key = $"{this.ModManifest.UniqueID}/publicBal";
                Game1.MasterPlayer.modData[key] = _publicMoney.PublicBal.ToString();
            }
        }
        private static int CalculateItemValue(Item item)
        {
            int value = 0;

            if (item is StardewValley.Object obj)
            {
                int basePrice = obj.sellToStorePrice();
                int stackSize = obj.Stack;
                value = (int)(basePrice * stackSize); // calculate value based on stack size
            }

            return value;
        }
        private static int CalculateShippingBinValue()
        {
            int totalValue = 0;

            foreach (Item item in Game1.getFarm().getShippingBin(Game1.player))
            {
                int itemValue = CalculateItemValue(item);
                totalValue += itemValue;
            }

            return totalValue;
        }
        private int TaxMoney(int amount)
        {
            decimal taxRate = Config.TaxPercent / 100m;
            int transferAmount = (int)Math.Round(amount * taxRate);
            _publicMoney.PublicBal += transferAmount;
            Game1.player.Money -= transferAmount;
            return transferAmount;
        }
    }
}