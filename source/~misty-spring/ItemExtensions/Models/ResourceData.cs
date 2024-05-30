/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using System.Text;
using ItemExtensions.Models.Contained;
using ItemExtensions.Models.Enums;
using ItemExtensions.Models.Items;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global

namespace ItemExtensions.Models;

/// <summary>
/// Resource info.
/// </summary>
/// See <see cref="StardewValley.GameData.Objects.ObjectData"/>
public class ResourceData
{
#if DEBUG
    private const LogLevel Level = LogLevel.Debug;
#else
    private const LogLevel Level =  LogLevel.Trace;
#endif
    private static void Log(string msg, LogLevel lv = Level) => ModEntry.Mon.Log(msg, lv);
    private const StringComparison Comparison = StringComparison.OrdinalIgnoreCase;
    
    // Required
    public string Name { get; set; }
    public CustomResourceType Type { get; set; } = CustomResourceType.Stone;
    public string DisplayName { get; set; } = "[LocalizedText Strings\\Objects:Stone_Name]";
    public string Description { get; set; } = "[LocalizedText Strings\\Objects:Stone_Description]";
    public string Texture { get; set; } = "Maps/springobjects";
    public int SpriteIndex { get; set; } = 0;
    
    // Region
    public int Width { get; set; } = 1;
    public int Height { get; set; } = 1;

    // Obtaining
    /// <summary>
    /// The stone's health. Every hit reduces UpgradeLevel + 1.
    /// For weapons, it does 10% average DMG + 1.
    /// See <see cref="StardewValley.Locations.MineShaft"/> for stone health.
    /// </summary>
    public int Health { get; set; } = 10;
    public string ItemDropped { get; set; }
    public int MinDrops { get; set; } = 1;
    public int MaxDrops { get; set; }
    public List<ExtraSpawn> ExtraItems { get; set; } = new();
    
    // Type of resource
    /// <summary>
    /// Debris when destroying item. Can be an ItemId, or one of: coins, wood, stone, bigStone, bigWood, hay, weeds
    /// </summary>
    
    public string Debris { get; set; } = "stone";

    public string[] FailSounds { get; set; } = { "clubhit", "clank" };
    public string BreakingSound { get; set; } = "stoneCrack";
    public string Sound { get; set; } = "hammer";
    public int AddHay { get; set; }
    public bool SecretNotes { get; set; } = true;
    public bool Shake { get; set; } = true;
    public StatCounter CountTowards { get; set; } = StatCounter.None;

    /// <summary>
    /// Tool's class. In the case of weapons, it can also be its number.
    /// </summary>
    public string Tool { get; set; } = "Pickaxe";

    public NotifyForTool? SayWrongTool { get; set; } = NotifyForTool.None;
    public bool ImmuneToBombs { get; set; }
    /// <summary>
    /// Minimum upgrade tool should have. If a weapon, the minimum number is checked. 
    /// ("number": 10% of average damage)
    /// </summary>
    public int MinToolLevel { get; set; }

    public int Exp { get; set; } = 5;
    public string Skill { get; set; } = "mining";
    internal int ActualSkill { get; set; } = -1;

    // Extra
    public List<string> ContextTags { get; set; } = new() { "placeable" };
    public Dictionary<string, string> CustomFields { get; set; } = new();
    public LightData Light { get; set; } = null;

    public OnBehavior OnDestroy { get; set; } = null;
    //general
    public string SpawnOnFloors { get; set; } = null;
    public double SpawnFrequency { get; set; } = 0.1;
    public double AdditionalChancePerLevel { get; set; }
    //conditional
    public List<MineSpawn> MineSpawns { get; set; } = new();
    //conditional
    internal List<MineSpawn> RealSpawnData { get; set; } = new();


    public bool IsValid(bool skipTextureCheck)
    {
        //fix possible nulls because I messed up and they were null in previous templates
        RealSpawnData ??= new List<MineSpawn>();
        CustomFields ??= new Dictionary<string, string>();
        ContextTags ??= new List<string>();
        ExtraItems ??= new List<ExtraSpawn>();
        
        //now do actual check
        if (!skipTextureCheck && Game1.content.DoesAssetExist<Texture2D>(Texture) == false)
        {
            Log($"Couldn't find texture {Texture} for resource {Name}. Skipping.", LogLevel.Info);
            return false;
        }

        if (MaxDrops < MinDrops)
            MaxDrops = MinDrops + 1;
        
        if (Width <= 0)
        {
            Log("Resource width must be over 0. Skipping.", LogLevel.Warn);
            return false;
        }
        
        if (Height <= 0)
        {
            Log("Resource height must be over 0. Skipping.", LogLevel.Warn);
            return false;
        }

        ActualSkill = GetSkill(Skill);
        
        if (Light != null)
        {
            if (Light.Size == 0)
            {
                Log("Item light can't be size 0. Skipping.", LogLevel.Warn);
                return false;
            }
                
            if(Light.Transparency == 0)
            {
                Log("Item transparency can't be 0. Skipping.", LogLevel.Warn);
                return false;
            }
        }

        if (Health <= 0)
        {
            Log("Resource health must be over 0. Skipping.", LogLevel.Warn);
            return false;
        }
        
        if (SpriteIndex < 0)
        {
            Log("Resource index can't be negative. Skipping.", LogLevel.Warn);
            return false;
        }
        
        if(string.IsNullOrWhiteSpace(Texture))
        {
            Log("Must specify a texture for resource. Skipping.", LogLevel.Warn);
            return false;
        }
        
        if(string.IsNullOrWhiteSpace(Tool))
        {
            Log("Must specify a tool for resource. Skipping.", LogLevel.Warn);
            return false;
        }

        if (string.IsNullOrWhiteSpace(ItemDropped))
        {
            Log("Resource's dropped item is empty. The resource will still be added, but keep this in mind when debugging your content pack.");
        }

        try
        {
            if(!string.IsNullOrWhiteSpace(SpawnOnFloors))
                RealSpawnData.Add(new MineSpawn(GetFloors(SpawnOnFloors), SpawnFrequency, AdditionalChancePerLevel, true));
        }
        catch (Exception e)
        {
            //silent error because it might not happen unless there's no data, i think.
            Log($"Error: {e}");
        }
        
        foreach (var floorData in MineSpawns)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(floorData.Floors))
                    continue;
                
                floorData.Parse(GetFloors(floorData.Floors)); 
                RealSpawnData.Add(floorData);
            }
            catch (Exception e)
            {
                //silent error because it might not happen unless there's no data, i think.
                Log($"Error: {e}");
            }
        }
        
        return true;
    }

    internal static int GetSkill(string skill)
    {
        if (string.IsNullOrWhiteSpace(skill))
        {
            return -1;
        }
        
        int actualSkill;
        
        if (int.TryParse(skill, out var intSkill))
            actualSkill = intSkill;
        if (skill.StartsWith("farm", Comparison))
            actualSkill = 0;
        else if (skill.StartsWith("fish", Comparison))
            actualSkill = 1;
        else if (skill.Equals("foraging", Comparison))
            actualSkill = 2;
        else if (skill.Equals("mining", Comparison))
            actualSkill = 3;
        else if (skill.Equals("combat", Comparison))
            actualSkill = 4;
        else if (skill.Equals("luck", Comparison))
            actualSkill = 5;
        else
            actualSkill = -1;
        
        return actualSkill;
    }

    /// <summary>
    /// Called when the user patches a vanilla resource. Removes every custom value except extra drops
    /// </summary>
    /// <param name="asInt"></param>
    public void Trim(int asInt)
    {
        /* doesn't need editing because we skip the entry
        Name = null;
        DisplayName = null;
        Description = null;
        Texture = null;
        SpriteIndex = -1;*/
        if (Additions.GeneralResource.VanillaClumps.Contains(asInt) == false)
        {
            Width = 1;
            Height = 1;
        }
        Health = -1;
        /*
        ItemDropped = null;
        MinDrops = 1;
        MaxDrops = null; */
        Debris = null;
        BreakingSound = null;
        Sound = null;
        AddHay = -1;
        SecretNotes = false;
        Shake = false;
        CountTowards = StatCounter.None;
        SayWrongTool = NotifyForTool.None;
        MinToolLevel = -1;
        Exp = 0;
        Skill = null;
        ActualSkill = -1;
        ContextTags = new();
        CustomFields = new();
        Light = null;
        Tool = "vanilla";
    }
    
    internal static IEnumerable<string> GetFloors(string floorData)
    {
        var all = new List<string>();
        //removes spaces and then separates by comma
        var allFloors = new StringBuilder(floorData);
        allFloors.Replace(" ", ""); 
        allFloors.Replace(',', ' ');
        var floors = ArgUtility.SplitBySpace(allFloors.ToString());
        foreach (var value in floors)
        {
            if(string.IsNullOrWhiteSpace(value))
                continue;
            
            if (int.TryParse(value, out var isInt) && isInt < 1)
            {
                continue;
            }
            
            if (value.Contains('-'))
            {
                var multipleFloors = new StringBuilder(value);
                multipleFloors.Replace('-', '/');
                multipleFloors.Replace("//999", "/-999");
                
                all.Add(multipleFloors.ToString());
                continue;
            }
            
            all.Add(value);
        }

        return all;
    }
}
