/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using ItemExtensions.Models.Internal;
using StardewValley.GameData.Objects;

namespace ItemExtensions.Models.Contained;

public class OnBehavior : IWorldChangeData
{
    public string Conditions { get; set; } = "TRUE";
    public string TriggerAction { get; set; } = null;
    
    //messaging
    public string Message { get; set; } = null;
    public string Confirm { get; set; } = null;
    public string Reject { get; set; } = null;
    public NoteData ShowNote { get; set; } = null;
    
    //item
    public int TextureIndex { get; set; }
    public int ReduceBy { get; set; } = 0;
    
    //setting
    public string ChangeMoney { get; set; } = null;
    public string Health { get; set; } = null;
    public string Stamina { get; set; } = null;
    
    //audio
    public string PlayMusic { get; set; } = null;
    public string PlaySound { get; set; } = null;
    
    public string AddQuest { get; set; } = null;
    public string AddSpecialOrder { get; set; } = null;
    
    public string RemoveQuest { get; set; } = null;
    public string RemoveSpecialOrder { get; set; } = null;
    
    public Dictionary<string, int> AddItems { get; set; } = null;
    public Dictionary<string, int> RemoveItems { get; set; } = null;
    public List<string> AddFlags { get; set; } = null;
    public List<string> RemoveFlags { get; set; }

    public List<MonsterSpawnData> SpawnMonsters { get; set; } = null;
    public List<ObjectBuffData> AddBuffs { get; set; }
}