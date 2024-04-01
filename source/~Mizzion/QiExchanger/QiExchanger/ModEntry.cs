/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace QiExchanger
{
    /// <summary>The mod entry class loaded by SMAPI.</summary>
    public class ModEntry : Mod
    {
        private ModConfig _config;
        private const bool IsDebugging = false;

        private ITranslationHelper _i18N;


        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this._config = helper.ReadConfig<ModConfig>();
            this._i18N = helper.Translation;
            
            
            //Set Up Events
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.Display.MenuChanged += this.OnMenuChanged;
        }
        //Event Voids
        /// <summary>
        /// Event that gets triggered when we press a button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">The event arg</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady || !Context.IsPlayerFree)
                return;

            if (e.IsDown(_config.ActivationKey))
            {
                DoMenu("main");
            }
            else if (e.IsDown(SButton.F5))
            {
                _config = Helper.ReadConfig<ModConfig>();
                Log("Config was reloaded", LogLevel.Trace, true);
            }
        }

        /// <summary>
        /// Event that gets triggered when a menu is opened
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Event Args</param>
        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (Game1.activeClickableMenu is null)
            {
                return;
            }

            if (e.NewMenu is DialogueBox db && 
                Game1.player.currentLocation.Name.Contains("Club") && 
                db.dialogues.ToArray().Aggregate("", (current, d) => current + d).Contains(_i18N.Get("original.text")))
            {
                db.closeDialogue();
                DoMenu("main");
            }
        }
        
        //Custom Voids

        /// <summary>
        /// Process the Dialogue Menus
        /// </summary>
        /// <param name="menuType">The menu type. So we can switch into it.</param>
        private void DoMenu(string menuType)
        {
            var player = Game1.player;
            var hasQiCoins = player.clubCoins > 0;
            var hasMoney = player.Money > 0;
            switch (menuType)
            {
              case "main":
              {
                  Log("Made it to the DoMenu main switch");
                  var mainResponses = new[]
                  {
                      new Response("Buy", _i18N.Get("main.text.option.one")),
                      new Response("Sell", _i18N.Get("main.text.option.two"))
                  };
                  Game1.currentLocation.createQuestionDialogue(
                      _i18N.Get("main.text", new { player_name = Game1.player.Name }), mainResponses, DoAnswers);
                  break;
              }
              case "Buy":
              {
                  var buyResponses = new List<Response>();
                  
                  if (player.Money >= (100 * 10) && hasMoney)
                      buyResponses.Add(new Response("100money", _i18N.Get("option.one")));
                  if (player.Money >= (1000 * 10) && hasMoney)
                      buyResponses.Add(new Response("1000money", _i18N.Get("option.two")));
                  if (player.Money >= (10000 * 10) && hasMoney)
                      buyResponses.Add(new Response("10000money", _i18N.Get("option.three")));
                  if (player.Money >= (100000 * 10) && hasMoney)
                      buyResponses.Add(new Response("100000money", _i18N.Get("option.four")));
                  if (player.Money >= (1000000 * 10) && hasMoney)
                      buyResponses.Add(new Response("1000000money", _i18N.Get("option.five")));
                  if (hasMoney)
                  {
                      buyResponses.Add(new Response("exit", "None Today"));
                      Game1.currentLocation.createQuestionDialogue(_i18N.Get("main.money.exchange.text", new { player_name = player.Name, amt_money = player.Money }), buyResponses.ToArray(), DoAnswers);
                  }
                  else
                  {
                      Game1.drawObjectDialogue(_i18N.Get("no.money"));
                  }
                  break;
              }
              case "Sell":
              {
                  Log("Made it to the DoMenu Sell switch");
                  
                  var sellResponses = new List<Response>();
                  
                  if (player.clubCoins >= 100 && hasQiCoins)
                      sellResponses.Add(new Response("100", _i18N.Get("option.one")));
                  if (player.clubCoins >= 1000 && hasQiCoins)
                      sellResponses.Add(new Response("1000", _i18N.Get("option.two")));
                  if (player.clubCoins >= 10000 && hasQiCoins)
                      sellResponses.Add(new Response("10000", _i18N.Get("option.three")));
                  if (player.clubCoins >= 100000 && hasQiCoins)
                      sellResponses.Add(new Response("100000", _i18N.Get("option.four")));
                  if (player.clubCoins >= 1000000 && hasQiCoins)
                      sellResponses.Add(new Response("1000000", _i18N.Get("option.five")));
                  if (hasQiCoins)
                  {
                      sellResponses.Add(new Response("exit", "None Today"));
                      Game1.currentLocation.createQuestionDialogue(_i18N.Get("main.exchange.text", new { player_name = player.Name, qi_amount = player.clubCoins, exchange_rate = _config.ExchangeRate }), sellResponses.ToArray(), DoAnswers);
                  }
                  else
                  {
                      Game1.drawObjectDialogue(_i18N.Get("no.qi.coins"));
                  }
                  break;
              }
            }
        }

        /// <summary>
        /// Process the Dialogue answers
        /// </summary>
        /// <param name="who">The farmer</param>
        /// <param name="answer">The answer chosen</param>
        private void DoAnswers(Farmer who, string answer)
        {
            switch (answer)
            {
                case "Buy":
                    Log("Made it to the DoAnswer buy menu");
                    Game1.currentLocation.lastQuestionKey = "Buy";
                    Game1.afterDialogues = DoBuyMenu;
                    break;
                case "Sell":
                    Log("Made it to the DoAnswer sell switch");
                    Game1.currentLocation.lastQuestionKey = "Sell";
                    Game1.afterDialogues = DoSellMenu;
                    break;
                case "100":
                    Log("Made it to the DoAnswer 100 switch");
                    Game1.currentLocation.lastQuestionKey = "100";
                    DoExchange(100);
                    break;
                case "1000":
                    Log("Made it to the DoAnswer 1000 switch");
                    Game1.currentLocation.lastQuestionKey = "1000";
                    DoExchange(1000);
                    break;
                case "10000":
                    Log("Made it to the DoAnswer 10000 switch");
                    Game1.currentLocation.lastQuestionKey = "10000";
                    DoExchange(10000);
                    break;
                case "100000":
                    Log("Made it to the DoAnswer 100000 switch");
                    Game1.currentLocation.lastQuestionKey = "100000";
                    DoExchange(100000);
                    break;
                case "1000000":
                    Log("Made it to the DoAnswer 1000000 switch");
                    Game1.currentLocation.lastQuestionKey = "1000000";
                    DoExchange(1000000);
                    break;
                case "100money":
                    Log("Made it to the DoAnswer 100money switch");
                    Game1.currentLocation.lastQuestionKey = "100money";
                    DoBuyExchange(100);
                    break;
                case "1000money":
                    Log("Made it to the DoAnswer 1000money switch");
                    Game1.currentLocation.lastQuestionKey = "1000money";
                    DoBuyExchange(1000);
                    break;
                case "10000money":
                    Log("Made it to the DoAnswer 10000money switch");
                    Game1.currentLocation.lastQuestionKey = "10000money";
                    DoBuyExchange(10000);
                    break;
                case "100000money":
                    Log("Made it to the DoAnswer 100000money switch");
                    Game1.currentLocation.lastQuestionKey = "100000money";
                    DoBuyExchange(100000);
                    break;
                case "1000000money":
                    Log("Made it to the DoAnswer 1000000money switch");
                    Game1.currentLocation.lastQuestionKey = "1000000money";
                    DoBuyExchange(1000000);
                    break;
                default:
                    Log("Hit the default");
                    break;
            }
        }

        /// <summary>
        /// Void to summon the buy menu. Otherwise it bugs out.
        /// </summary>
        private void DoBuyMenu()
        {
            DoMenu("Buy");
        }
        
        /// <summary>
        /// Void to summon the sell menu. Otherwise it bugs out.
        /// </summary>
        private void DoSellMenu()
        {
            DoMenu("Sell");
        }
        
        /// <summary>
        /// Process the amount of club coins to buy
        /// </summary>
        /// <param name="val">The amount the player wants to buy</param>
        private void DoBuyExchange(int val)
        {
            Log("Made it to the BuyExchange method");
            if (val > 0)
            {
                var cost = val * 10;
                if (Game1.player.Money >= cost)
                {
                    Game1.player.Money -= cost;
                    Game1.player.clubCoins += val;
                    Game1.drawObjectDialogue(_i18N.Get("do.buy.exchange", new { amount = val, g_coins = cost }));
                }
                else
                {
                    Game1.drawObjectDialogue(_i18N.Get("not.enough.money"));
                }
            }
            else
            {
                Log("A negative value was pass to the buyexchange", LogLevel.Trace, true);
            }
        }
        
        /// <summary>
        /// Process the Amount we should exchange
        /// </summary>
        /// <param name="val">The amount of clubCoins we're trading in.</param>
        private void DoExchange(int val)
        {
            Log("Made it to the exchange method");
            if (val > 0)
            {
                if (Game1.player.clubCoins >= val)
                {
                    var outer = _config.ExchangeRate > 0 ? (val * _config.ExchangeRate) : 0;
                    Game1.player.clubCoins -= val;
                    Game1.player.Money += Math.Max(0, outer);
                    Game1.drawObjectDialogue(_i18N.Get("do.exchange", new{ qi_coins = val, g_coins = outer}));
                }
                else
                {
                    Game1.drawObjectDialogue(_i18N.Get("not.enough.qicoins"));
                }
            }
            else
            {
                Log("A negative value was pass to the exchange", LogLevel.Trace, true);
            }
        }
        
        /// <summary>
        /// Checks to see if we are in debug mode(Development) and sends logs.
        /// </summary>
        /// <param name="msg">The message to show in the console</param>
        /// <param name="level">The log level. Default is trace</param>
        /// <param name="bypassDebugging">Whether or not we should do the log anyways.</param>
        private void Log(string msg, LogLevel level = LogLevel.Trace, bool bypassDebugging = false)
        {
            if(IsDebugging || bypassDebugging)
                Monitor.Log($"{msg}\r\n", level);
        }

       
    }
}
