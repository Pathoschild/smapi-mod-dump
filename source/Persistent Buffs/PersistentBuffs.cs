/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aceynk/PersistentBuffs
**
*************************************************/

using System.Text.Json;
using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buffs;
using StardewValley.GameData.Buffs;

namespace PersistentBuffs;

public class ModEntry : Mod
{
    public static void Log(string v)
    {
        _log.Log(v, LogLevel.Debug);
    }
    
    public static IMonitor _log = null!;
    //public NetStringList DayEndBuffs = null!;
    //public Dictionary<string, int> DayEndBuffData = new();

    public string BDname = "aceynk.PersistentBuffs.BuffData";

    /*
    public NetString CondenseBuffs(NetStringList value)
    {
        if (value == new NetStringList())
        {
            return new NetString();
        }
        
        string output = string.Join("/", value);

        return new NetString(output);
    }

    public NetStringList DecodeBuffs(NetString value)
    {
        if (value == new NetString())
        {
            return new NetStringList();
        }
        
        NetStringList output = new NetStringList(((string)value).Split("/"));

        return output;
    }

    public NetString CondenseBuffData(Dictionary<string, int> value)
    {
        if (value == new Dictionary<string, int>())
        {
            return new NetString();
        }
        
        List<string> output = value.Keys.Select(v => v + ":" + value[v]).ToList();

        return new NetString(string.Join("/", output));
    }

    
    public Dictionary<string, int> oDecodeBuffData(NetString value)
    {
        if (value == new NetString())
        {
            return new Dictionary<string, Dictionary<string, string>>();
        }
        
        List<string> itemList = value.ToString().Split("/").ToList();
        Dictionary<string, Dictionary<string, string>> output = new();

        foreach (string item in itemList)
        {
            if (item == "") continue;
            string key = item.Split(":")[0];
            int val = int.Parse(item.Split(":")[1]);

            output[key] = val;
        }

        return output;
    }
    */
    
    private NetString EncodeBuffData(Dictionary<string, Buff> value)
    {
        Dictionary<string, Dictionary<string, object>> simplifiedBuffs = new();

        foreach (KeyValuePair<string, Buff> pair in value)
        {
            Buff tObj = pair.Value;
            Dictionary<string, object> newDict
                = new();
            newDict["DisplayName"] = tObj.displayName;
            newDict["Description"] = tObj.description;
            newDict["GlowColor"] = tObj.glow.PackedValue;
            newDict["Duration"] = tObj.totalMillisecondsDuration;
            newDict["FarmingLevel"] = tObj.effects.FarmingLevel.Value;
            newDict["FishingLevel"] = tObj.effects.FishingLevel.Value;
            newDict["MiningLevel"] = tObj.effects.MiningLevel.Value;
            newDict["LuckLevel"] = tObj.effects.LuckLevel.Value;
            newDict["ForagingLevel"] = tObj.effects.ForagingLevel.Value;
            newDict["MaxStamina"] = tObj.effects.MaxStamina.Value;
            newDict["MagneticRadius"] = tObj.effects.MagneticRadius.Value;
            newDict["Speed"] = tObj.effects.Speed.Value;
            newDict["Defense"] = tObj.effects.Defense.Value;
            newDict["Attack"] = tObj.effects.Attack.Value;
            
            newDict["ActionsOnApply"] = tObj.actionsOnApply;
            newDict["Source"] = tObj.source;
            newDict["DisplaySource"] = tObj.displaySource;
            newDict["Id"] = tObj.id;

            simplifiedBuffs[tObj.id] = newDict;
        }
        
        string output = JsonSerializer.Serialize(simplifiedBuffs);
        
        return new NetString(output);
    }

    private Dictionary<string, Buff> DecodeBuffData(NetString value)
    {
        
        
        Dictionary<string, Dictionary<string, object>>? input =
            JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, object>>>(value);
        Dictionary<string, Buff> output = new();
        
        if (input == null) return new Dictionary<string, Buff>();

        foreach (KeyValuePair<string, Dictionary<string, object>> pair in input)
        {
            Dictionary<string, object> buffDict = pair.Value;

            BuffAttributesData bAttr = new();
            
            // EFFECTS ADDS TO BASE EFFECT VALUES
            Buff tentativeBuff = new Buff(pair.Key);
            
            float tryDeserializeFloat(string name)
            {
                if (!buffDict.ContainsKey(name))
                {
                    return 0;
                }

                if (buffDict[name] == null)
                {
                    return 0;
                }

                float output = ((JsonElement)buffDict[name]).Deserialize<float>();
                
                return output;
            }
            
            float ProcAttr(string name)
            {
                if (buffDict.ContainsKey(name))
                {
                    
                    return tryDeserializeFloat(name);
                }

                return (float)0;
            }
            
            bAttr.FarmingLevel = ProcAttr("FarmingLevel") - tentativeBuff.effects.FarmingLevel.Value;
            bAttr.FishingLevel = ProcAttr("FishingLevel") - tentativeBuff.effects.FishingLevel.Value;
            bAttr.MiningLevel = ProcAttr("MiningLevel") - tentativeBuff.effects.MiningLevel.Value;
            bAttr.LuckLevel = ProcAttr("LuckLevel") - tentativeBuff.effects.LuckLevel.Value;
            bAttr.ForagingLevel = ProcAttr("ForagingLevel") - tentativeBuff.effects.ForagingLevel.Value;
            bAttr.MaxStamina = ProcAttr("MaxStamina") - tentativeBuff.effects.MaxStamina.Value;
            bAttr.MagneticRadius = ProcAttr("MagneticRadius") - tentativeBuff.effects.MagneticRadius.Value;
            bAttr.Speed = ProcAttr("Speed") - tentativeBuff.effects.Speed.Value;
            bAttr.Defense = ProcAttr("Defense") - tentativeBuff.effects.Defense.Value;
            bAttr.Attack = ProcAttr("Attack") - tentativeBuff.effects.Attack.Value;
            
            string tryDeserializeString(string name)
            {
                if (!buffDict.ContainsKey(name))
                {
                    return "";
                }

                if (buffDict[name] == null)
                {
                    return "";
                }

                return ((JsonElement)buffDict[name]).Deserialize<string>();
            }

            int tryDeserializeInt(string name)
            {
                if (!buffDict.ContainsKey(name))
                {
                    return 0;
                }

                if (buffDict[name] == null)
                {
                    return 0;
                }

                return ((JsonElement)buffDict[name]).Deserialize<int>();
            }
            
            Buff newBuff = new Buff(
                tryDeserializeString("Id"),
                tryDeserializeString("Source"),
                tryDeserializeString("DisplaySource"),
                duration: tryDeserializeInt("Duration"),
                effects: new BuffEffects(bAttr),
                displayName: tryDeserializeString("DisplayName"),
                description: tryDeserializeString("Description")
            );

            newBuff.actionsOnApply = (string[])buffDict["ActionsOnApply"];
            newBuff.glow = new Color((uint)tryDeserializeInt("GlowColor"));

            output[pair.Key] = newBuff;
        }

        return output;
    }

    public override void Entry(IModHelper helper)
    {
        //Config = Helper.ReadConfig<ModConfig>();
        _log = Monitor;

        Helper.Events.Content.AssetRequested += OnAssetRequested;
        Helper.Events.GameLoop.DayEnding += OnDayEnding;
        Helper.Events.GameLoop.DayStarted += OnDayStarted;
        Helper.Events.GameLoop.GameLaunched += OnGameLaunched;
    }

    private static List<string> UsedIds(Dictionary<string, bool> idBoolDict)
    {
        return idBoolDict.Keys.Where(v => idBoolDict[v]).ToList();
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        if (!Game1.player.modData.Keys.Contains(BDname))
        {
            Game1.player.modData[BDname] = new NetString();
        }
    }

    private void OnDayEnding(object? sender, DayEndingEventArgs e) 
    {
        Dictionary<string, Buff> DayEndBuffData =
            DecodeBuffData(new NetString(Game1.player.modData[BDname]));
        
        NetStringList curBuffIds = Game1.player.buffs.AppliedBuffIds;
        Dictionary<string, bool> protectedIdsDict = Game1.content.Load<Dictionary<string, bool>>("aceynk.PersistentBuffs/PersistentBuffIds");

        List<string> protectedIds = UsedIds(protectedIdsDict);
        
        List<string> curBuffIdsList = curBuffIds.Where(v => protectedIds.Contains(v)).ToList();

        int foodCount = 0;
        
        foreach (string buffId in curBuffIdsList)
        {
            if (buffId == "food")
            {
                DayEndBuffData["food" + foodCount] = Game1.player.buffs.AppliedBuffs[buffId];
                foodCount++;
                
                continue;
            }
            
            DayEndBuffData[buffId] = Game1.player.buffs.AppliedBuffs[buffId];
        }
        
        Game1.player.modData[BDname] = EncodeBuffData(DayEndBuffData);
    }

    private void OnDayStarted(object? sender, DayStartedEventArgs e)
    {
        Dictionary<string, Buff> DayEndBuffData = new();
        
        try
        {
            DayEndBuffData =
                DecodeBuffData(new NetString(Game1.player.modData[BDname]));
        }
        catch (Exception Ex)
        {
            Log("Failed to fetch modData on day start.");
            Log(Ex.ToString());
        }
        
        foreach (Buff thisBuff in DayEndBuffData.Values)
        {
            
            
            Game1.player.applyBuff(thisBuff);
        }

        DayEndBuffData.Clear();
        
        Game1.player.modData[BDname] = EncodeBuffData(DayEndBuffData);
    }

    private static void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
    {
        if (e.Name.IsEquivalentTo("aceynk.PersistentBuffs/PersistentBuffIds"))
        {
            e.LoadFrom(() => new Dictionary<string, bool>(), AssetLoadPriority.High);
        }
    }
}