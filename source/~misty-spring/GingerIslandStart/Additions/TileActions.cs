/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using Object = StardewValley.Object;

namespace GingerIslandStart.Additions;

public static class TileActions
{
#if DEBUG
    private const LogLevel Level = LogLevel.Debug;
#else
    private const LogLevel Level =  LogLevel.Trace;
#endif
    
    private static void Log(string msg, LogLevel lv = Level) => ModEntry.Mon.Log(msg, lv);
    private static string Translate(string msg) => ModEntry.Help.Translation.Get(msg);
    
    public static bool Batteries(GameLocation arg1, string[] arg2, Farmer arg3, Point arg4)
    {
        if (Game1.MasterPlayer.hasOrWillReceiveMail("willyBoatTicketMachine")) 
            return false;
        
        if (arg3.Items.ContainsId("(O)787", 5))
            arg1.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:BoatTunnel_DonateBatteries"), arg1.createYesNoResponses(), DonateBatteries);
        else
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:BoatTunnel_DonateBatteriesHint"));

        return true;

    }

    private static void DonateBatteries(Farmer who, string whichanswer)
    {
        if (whichanswer == "No")
            return;
        
        Game1.Multiplayer.globalChatInfoMessage("RepairBoatMachine", Game1.player.Name);
        Game1.player.Items.ReduceId("(O)787", 5);
        DelayedAction.playSoundAfterDelay("openBox", 600);
        Game1.addMailForTomorrow("willyBoatTicketMachine", true, true);
        CheckForBoatComplete();
    }

    public static bool Anchor(GameLocation arg1, string[] arg2, Farmer arg3, Point arg4)
    {
        if (Game1.MasterPlayer.hasOrWillReceiveMail("willyBoatAnchor")) 
            return false;
        
        if (arg3.Items.ContainsId("(O)337", 5))
            arg1.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:BoatTunnel_DonateIridium"), arg1.createYesNoResponses(), DonateIridium);
        else
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:BoatTunnel_DonateIridiumHint"));
            
        return true;
    }

    private static void DonateIridium(Farmer who, string whichanswer)
    {
        if (whichanswer == "No")
            return;
        
        Game1.Multiplayer.globalChatInfoMessage("RepairBoatAnchor", Game1.player.Name);
        Game1.player.Items.ReduceId("(O)337", 5);
        DelayedAction.playSoundAfterDelay("clank", 600);
        DelayedAction.playSoundAfterDelay("clank", 1200);
        DelayedAction.playSoundAfterDelay("clank", 1800);
        Game1.addMailForTomorrow("willyBoatAnchor", true, true);
        CheckForBoatComplete();
    }

    public static bool Hull(GameLocation arg1, string[] arg2, Farmer arg3, Point arg4)
    {
        if (Game1.MasterPlayer.hasOrWillReceiveMail("willyBoatHull"))
            return false;
        
        if (arg3.Items.ContainsId("(O)709", 200))
            arg1.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:BoatTunnel_DonateHardwood"), arg1.createYesNoResponses(), DonateHardwood);
        else
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:BoatTunnel_DonateHardwoodHint"));
        return true;
    }

    private static void DonateHardwood(Farmer who, string whichanswer)
    {
        if (whichanswer == "No")
            return;
        
        Game1.Multiplayer.globalChatInfoMessage("RepairBoatHull", Game1.player.Name);
        Game1.player.Items.ReduceId("(O)709", 200);
        DelayedAction.playSoundAfterDelay("Ship", 600);
        Game1.addMailForTomorrow("willyBoatHull", true, true);
        CheckForBoatComplete();
    }

    private static void CheckForBoatComplete()
    {
        if (!Game1.player.hasOrWillReceiveMail("willyBoatTicketMachine") || !Game1.player.hasOrWillReceiveMail("willyBoatHull") || !Game1.player.hasOrWillReceiveMail("willyBoatAnchor"))
            return;
        Game1.player.freezePause = 1500;
        DelayedAction.functionAfterDelay((Action) (() =>
        {
            Game1.Multiplayer.globalChatInfoMessage("RepairBoat");
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:BoatTunnel_boatcomplete"));
        }), 1500);
    }

    public static void Sleep(GameLocation arg1, string[] arg2, Farmer arg3, Vector2 arg4)
    {
        var yn = arg1.createYesNoResponses();
        arg1.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:FarmHouse_Bed_GoToSleep"), yn, SleepResponse);
        return;

        void SleepResponse(Farmer who, string whichanswer)
        {
            if (whichanswer == "No")
                return;

            DebugCommands.TryHandle(new[] { "sleep" });
        }
    }

    /*
    /// <summary>
    /// Pirate shop, akin to adv. guild's.
    /// </summary>
    /// <param name="arg1"></param>
    /// <param name="arg2"></param>
    /// <param name="arg3"></param>
    /// <param name="arg4"></param>
    /// <see cref="StardewValley.GameLocation.adventureShop"/>
    /// <see cref="StardewValley.Item"/>
    public static void PirateShop(GameLocation arg1, string[] arg2, Farmer arg3, Vector2 arg4)
    {
        if (Game1.player.itemsLostLastDeath.Count > 0)
        {
            var responses = new List<Response>()
            {
                new Response("Shop", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_Shop")),
                new Response("Recovery", Game1.content.LoadString("Strings\\Locations:AdventureGuild_ItemRecovery")),
                new Response("Leave", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_Leave"))
            }.ToArray();

            arg1.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:AdventureGuild_Greeting"),
                responses, HandleShopResponse);
        }
        else
        {
            OpenPirateShop(arg1, arg3, arg4);
            //Utility.TryOpenShopMenu($"{ModEntry.Id}_Trader", null, true);
        }

        return;

        void HandleShopResponse(Farmer who, string whichanswer)
        {
            switch(whichanswer) {
                case "Recovery":
                    Game1.player.forceCanMove();
                    OpenPirateShop(arg1, arg3, arg4, true);
                    //Utility.TryOpenShopMenu("AdventureGuildRecovery", null, true);
                    break;
                case "Shop":
                    Game1.player.forceCanMove();
                    OpenPirateShop(arg1, arg3, arg4);
                    //Utility.TryOpenShopMenu($"{ModEntry.Id}_Trader", null, true);
                    break;
            }
        }
    }

    private static void OpenPirateShop(GameLocation where, Farmer who, Vector2 tile, bool isRecovery = false)
    {
        var t = new xTile.Dimensions.Location((int)tile.X, (int)tile.Y);
        var id = ModEntry.Id;
        var actionString = isRecovery ? $"OpenShop {id}_Recovery" : $"OpenShop {id}_Trader";
        
        where.performAction(actionString, who, t);
    }
    */
    public static bool Builder(GameLocation arg1, string[] arg2, Farmer arg3, Point arg4)
    {
        var yn = arg1.createYesNoResponses();
        
        arg1.createQuestionDialogue(ModEntry.BuildQuestion, yn, AfterBuildQuestion);
        return true;
    }

    private static void AfterBuildQuestion(Farmer who, string whichanswer)
    {
        if (whichanswer == "No")
            return;

        if (!Game1.player.Items.ContainsId("(O)388", 120) || !Game1.player.Items.ContainsId("(O)390", 75))
        {
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings/StringsFromCSFiles:BlueprintsMenu.cs.10002"));
            return;
        }
        
        var buildEvent = new Event("continue/-1000 -1000/farmer 95 46 0/pause 2000/playSound dirtyHit/pause 200/playSound dirtyHit/pause 200/playSound dirtyHit/pause 500/playSound clank/pause 900/playSound clank/pause 200/playSound throw/pause 200/playSound clank/pause 1200/playSound doorClose/removeItem (O)388 120/removeItem (O)390 75/end", Game1.player) 
        { 
            onEventFinished = BuildBarn
        };
        who.currentLocation.currentEvent = buildEvent;
        
        var newStamina = Game1.player.Stamina - 80;
        if (newStamina <= 0)
            newStamina = 1;
        
        Game1.player.Stamina = newStamina;

        //if any farmer has the coop quest, complete it
        var farmersWithQuest = Game1.getAllFarmers().Where(f => f.hasQuest("7")).ToList();
        foreach (var farmer in farmersWithQuest)
        {
            farmer.completeQuest("7");
        }
    }

    private static void BuildBarn() => IslandChanges.CheckBarn(true);

    /// <summary>
    /// See <see cref="StardewValley.Menus.GeodeMenu"/>
    /// </summary>
    /// <param name="arg1"></param>
    /// <param name="arg2"></param>
    /// <param name="arg3"></param>
    /// <param name="arg4"></param>
    /// <returns></returns>
    public static bool WeaponShop(GameLocation arg1, string[] arg2, Farmer arg3, Point arg4)
    {
        //if any lost on death
        if (arg3.itemsLostLastDeath.Count > 0)
        {
            var answerChoices= new[]
            {
                new Response("Shop", Game1.content.LoadString("Strings\\Locations:Blacksmith_Clint_Shop")),
                new Response("Recovery", Game1.content.LoadString("Strings\\Locations:AdventureGuild_ItemRecovery")),
                new Response("Leave", Game1.content.LoadString("Strings\\Locations:Blacksmith_Clint_Leave"))
            };
            arg1.createQuestionDialogue("", answerChoices, AfterPirateAnswer);
        }
        //or if backpack isn't fully upgraded
        else if (Game1.player.MaxItems < 36)
        {
            var answerChoices= new[]
            {
                new Response("Shop", Game1.content.LoadString("Strings\\Locations:Blacksmith_Clint_Shop")),
                new Response("IncreaseBackpack", Translate("BackpackUpgrade")),
                new Response("Leave", Game1.content.LoadString("Strings\\Locations:Blacksmith_Clint_Leave"))
            };
            arg1.createQuestionDialogue("", answerChoices, AfterPirateAnswer);
        }
        // or just open shop
        else
        {
            OpenShop(arg1, new[]{"OpenShop", "mistyspring.GingerIslandStart_Pirate"},arg3,arg4);
        }
        return true;

        void AfterPirateAnswer(Farmer who, string whichanswer)
        {
            if (whichanswer == "IncreaseBackpack")
            {
                var marlon = Game1.getCharacterFromName("Marlon");
                marlon.displayName = Game1.content.LoadString("Strings/UI:LevelUp_ProfessionName_Pirate");
                var raw = Translate("TrainingQuestion");
                var money = who.MaxItems == 12 ? 3000 : 12000;
                var question = string.Format(raw, money);
                
                var dialogue = new Dialogue(marlon, null, Translate("PirateTraining"))
                {
                    overridePortrait = Game1.content.Load<Texture2D>("Mods/mistyspring.GingerIslandStart/Shop_Pirate"),
                    onFinish = () => arg1.createQuestionDialogue(question, arg1.createYesNoResponses(), AfterTrainingQuestion)
                };
                Game1.DrawDialogue(dialogue);
            }
            if (whichanswer == "Recovery")
            {
                OpenShop(arg1, new[]{"OpenShop", "mistyspring.GingerIslandStart_PirateRecovery"}, who, arg4);
            }
            if (whichanswer == "Shop")
            {
                OpenShop(arg1, new[]{"OpenShop", "mistyspring.GingerIslandStart_Pirate"}, who, arg4);
            }
        }
    }

    /// <summary>
    /// Depending on answer and money, plays upgrade event => reduces stamina & upgrades backpack.
    /// </summary>
    /// <param name="who"></param>
    /// <param name="whichanswer"></param>
    private static void AfterTrainingQuestion(Farmer who, string whichanswer)
    {
        if (whichanswer == "No")
            return;

        if (who.MaxItems == 36)
        {
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings/UI:Item_CantBeUsedHere"));
            return;
        }
        
        if (who.MaxItems == 12)
        {
            if (who.Money < 3000)
            {
                var notEnoughMoney = Game1.content.LoadString("Strings/StringsFromCSFiles:PurchaseAnimalsMenu.cs.11325");
                Game1.showRedMessage(notEnoughMoney);
                return;
            }

            who.Money -= 3000;
        }
        else
        {
            //if not enough money, return.
            if (who.Money < 12000)
            {
                var notEnoughMoney = Game1.content.LoadString("Strings/StringsFromCSFiles:PurchaseAnimalsMenu.cs.11325");
                Game1.showRedMessage(notEnoughMoney);
                return;
            }

            //otherwise reduce
            who.Money -= 12000;
        }
        
        Game1.PlayEvent("GingerIslandStart_UpgradePackage");
        Game1.delayedActions.Add(new DelayedAction(1000, () =>
        {
            Game1.CurrentEvent.onEventFinished += IncreaseStamina;
        }));
    }

    private static void IncreaseStamina()
    {
        var newStamina = Game1.player.Stamina - (int)(30 * ModEntry.GeneralDifficultyMultiplier);
        if (newStamina < 1)
            newStamina = 1;
                
        Game1.player.Stamina = newStamina;
        Game1.player.increaseBackpackSize(12);
                
        if (Game1.player.MaxItems == 24)
            Game1.Multiplayer.globalChatInfoMessage("BackpackLarge", Game1.player.Name);
        else
            Game1.Multiplayer.globalChatInfoMessage("BackpackDeluxe", Game1.player.Name);
    }

    public static bool ToolShop(GameLocation arg1, string[] arg2, Farmer arg3, Point arg4)
    {
        {
            Response[] answerChoices;
        
            //if any item is a geode
            if (arg3.Items.Any(item => item is Object o && Game1.objectData.ContainsKey(o.ItemId) && Game1.objectData[o.ItemId].GeodeDrops != null))
            {
                answerChoices= new[]
                {
                    new Response("Shop", Game1.content.LoadString("Strings\\Locations:Blacksmith_Clint_Shop")),
                    new Response("Process", Game1.content.LoadString("Strings\\Locations:Blacksmith_Clint_Geodes")),
                    new Response("Leave", Game1.content.LoadString("Strings\\Locations:Blacksmith_Clint_Leave"))
                };
            arg1.createQuestionDialogue("", answerChoices, AfterPirateAnswer);
            }
            else
            {
                OpenShop(arg1, new[]{"OpenShop", "mistyspring.GingerIslandStart_Dwarf"},arg3,arg4);
            }
            return true;

            void AfterPirateAnswer(Farmer who, string whichanswer)
            {
                if (whichanswer == "Shop")
                {
                    OpenShop(arg1, new[]{"OpenShop", "mistyspring.GingerIslandStart_Dwarf"}, who, arg4);
                }
                if (whichanswer == "Process")
                {
                    Game1.activeClickableMenu = new GeodeMenu();
                }
            }
        }
    }

    private static void OpenShop(GameLocation gameLocation, string[] action, Farmer who, Point tileLocation)
    {
        if (!ArgUtility.TryGet(action, 1, out var str10, out var error) ||
            !ArgUtility.TryGetOptional(action, 2, out var str11, out error) ||
            !ArgUtility.TryGetOptionalInt(action, 3, out var num2, out error, -1) ||
            !ArgUtility.TryGetOptionalInt(action, 4, out var num3, out error, -1) ||
            !ArgUtility.TryGetOptionalInt(action, 5, out var x, out error, -1) ||
            !ArgUtility.TryGetOptionalInt(action, 6, out var y, out error, -1) ||
            !ArgUtility.TryGetOptionalInt(action, 7, out var width, out error, -1) ||
            !ArgUtility.TryGetOptionalInt(action, 8, out var height, out error, -1))
        {
            Log($"Error: {error}", LogLevel.Error);
            return;
        }
        var nullable = new Rectangle?();
        if (x != -1 || y != -1 || width != -1 || height != -1)
        {
            if (x == -1 || y == -1 || width == -1 || height == -1)
                Log("when specifying any of the shop area 'x y width height' arguments (indexes 5-8), all four must be specified");
            nullable = new Rectangle(x, y, width, height);
        }
        switch (str11)
        {
            case "down":
                if (who.TilePoint.Y < tileLocation.Y)
                    return;
                break;
            case "up":
                if (who.TilePoint.Y > tileLocation.Y)
                    return;
                break;
            case "left":
                if (who.TilePoint.X > tileLocation.X)
                    return;
                break;
            case "right":
                if (who.TilePoint.X < tileLocation.X)
                    return;
                break;
        }
        if (num2 >= 0 && Game1.timeOfDay < num2 || num3 >= 0 && Game1.timeOfDay >= num3)
            return;
        var shopId = str10;
        var ownerArea = nullable;
        var flag1 = !nullable.HasValue;
        var maxOwnerY = new int?();
        var num4 = flag1 ? 1 : 0;
        Utility.TryOpenShopMenu(shopId, gameLocation, ownerArea, maxOwnerY, num4 != 0);
    }
}