/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using StardewValley;
using StardewValley.Triggers;

namespace ItemExtensions.Models.Internal;

public interface IWorldChangeData
{
    string Health { get; set; }
    string Stamina { get; set; }
    
    Dictionary<string, int> AddItems { get; set; }
    Dictionary<string, int> RemoveItems { get; set; }
    
    string PlayMusic { get; set; }
    string PlaySound { get; set; }
    
    string AddQuest { get; set; }
    string AddSpecialOrder { get; set; }
    
    string RemoveQuest { get; set; }
    string RemoveSpecialOrder { get; set; }
    
    List<string> AddFlags { get; set; }
    List<string> RemoveFlags { get; set; }
    
    List<ObjectBuffData> AddBuffs { get; set; } = new();
    //List<ObjectBuffData> RemoveBuffs { get; set; } = new();
    
    string Conditions { get; set; }
    string TriggerAction { get; set; }

    public static void Solve(IWorldChangeData data)
    {
        /*
        if (string.IsNullOrWhiteSpace(data.Conditions) == false &&
            GameStateQuery.CheckConditions(data.Conditions) == false)
            return;*/
        
        #region player values
        if (!string.IsNullOrWhiteSpace(data.Health))
        {
            Game1.player.health = ChangeValues(data.Health, Game1.player.health, Game1.player.maxHealth);
        }
        
        if (!string.IsNullOrWhiteSpace(data.Stamina))
        {
            Game1.player.Stamina = ChangeValues(data.Stamina, Game1.player.Stamina, Game1.player.MaxStamina);
        }
        #endregion
        
        #region flags
        if (data.AddFlags != null && data.AddFlags.Any())
        {
            foreach (var pair in data.AddFlags)
            {
                Game1.player.mailReceived.Add(pair);
            }
        }
        
        if (data.RemoveFlags != null && data.RemoveFlags.Any())
        {
            foreach (var pair in data.RemoveFlags)
            {
                Game1.player.RemoveMail(pair);
            }
        }
        #endregion
        
        #region items
        if (data.AddItems != null && data.AddItems.Any())
        {
            foreach (var pair in data.AddItems)
            {
                var item = ItemRegistry.Create(pair.Key, pair.Value);
                Game1.player.addItemByMenuIfNecessary(item);
            }
        }
        
        if (data.RemoveItems != null && data.RemoveItems.Any())
        {
            foreach (var pair in data.RemoveItems)
            {
                Game1.player.removeFirstOfThisItemFromInventory(pair.Key, pair.Value);
            }
        }

        if (data.AddBuffs.Any())
        {
            foreach (var buff in data.AddBuffs)
            {
                Game1.player.applyBuff(buff);
            }
        }

        /*
        if (data.RemoveBuffs.Any())
        {
            foreach (var buff in data.RemoveBuffs)
            {
                //
            }
        }*/
        #endregion
        
        #region quests
        if(!string.IsNullOrWhiteSpace(data.AddQuest))
            Game1.player.addQuest(data.AddQuest);
        
        if(!string.IsNullOrWhiteSpace(data.AddSpecialOrder))
            Game1.player.team.AddSpecialOrder(data.AddSpecialOrder);
        
        if(!string.IsNullOrWhiteSpace(data.RemoveQuest))
            Game1.player.removeQuest(data.RemoveQuest);

        if (!string.IsNullOrWhiteSpace(data.RemoveSpecialOrder))
        {
            var specialOrders = Game1.player.team.specialOrders;
            for (var index = specialOrders.Count - 1; index >= 0; --index)
            {
                if (specialOrders[index].questKey.Value == data.RemoveSpecialOrder)
                    specialOrders.RemoveAt(index);
            }
        }
        #endregion

        #region play
        if (!string.IsNullOrWhiteSpace(data.PlaySound))
            Game1.playSound(data.PlaySound);
        
        if (!string.IsNullOrWhiteSpace(data.PlayMusic))
            Game1.changeMusicTrack(data.PlayMusic);
        #endregion

        if (string.IsNullOrWhiteSpace(data.TriggerAction)) 
            return;

        //get all actions
        var actions = data.TriggerAction.replace(", ", ",").split(',');
        foreach(var trigger in actions)
        {
            TriggerActionManager.TryRunAction(trigger, out var error, out var exception);
            if (!string.IsNullOrWhiteSpace(error))
            {
                ModEntry.Mon.Log($"Error: {error}. {exception}", LogLevel.Warn);
            }
        }
    }

    private static int ChangeValues(string howMuch, float value, int defaultValue) =>
        ChangeValues(howMuch, (int)value, defaultValue);
    
    public static int ChangeValues(string howMuch, int value, int defaultValue)
    {
        if(string.IsNullOrWhiteSpace(howMuch))
            return -1;

        int result;
        
        if (int.TryParse(howMuch, out var justNumbers))
        {
            return justNumbers + value;
        }

        var split = howMuch.Split(' ');
        var type = split[0];
        var amt = int.Parse(split[1]);
        
        var addsOrReduces = type switch
        {
            "add" => true,
            "more" => true,
            "reduce" => true,
            "less" => true,
            "+" => true,
            "-" => true,
            _ => false
        };

        if(addsOrReduces)
        {
            //Log("Adding/Substracting from player health.");

            //add/reduce hp
            if (type is "less" or "-" or "reduce")
            {
                var trueAmt = value - amt;
                result = trueAmt <= 0 ? 1 : trueAmt;
            }
            else
            {
                var trueAmt = value + amt;
                result = trueAmt >= value ? value : trueAmt;
            }
        }
        else if (type == "reset")
        {
            //Log("Resetting player health.");
            result = defaultValue;
        }
        else
        {
            //Log("Setting player health.");
            //set
            result = amt;
        }

        return result;
    }
}
