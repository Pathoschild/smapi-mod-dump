/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using System.Text;
using weizinai.StardewValleyMod.Common.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Monsters;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;
using SObject = StardewValley.Object;

namespace weizinai.StardewValleyMod.LazyMod.Framework.Hud;

internal class MiningHud
{
    private readonly RootElement hud;
    private bool hasGetMineralInfo;
    private readonly Dictionary<string, int> mineralInfo = new();
    private bool hasGetMonsterInfo;
    private readonly Dictionary<string, int> monsterInfo = new();

    public MiningHud(IModHelper helper, ModConfig config)
    {
        this.hud = new RootElement();

        var ladderHud = new CombineImage(Game1.temporaryContent.Load<Texture2D>("Maps/Mines/mine_desert"),
            new Rectangle(208, 160, 16, 16), this.GetPosition(0))
        {
            CheckHidden = () => !(config.ShowLadderInfo && this.GetBuildingLayerInfo(173))
        };
        var shaftHud = new CombineImage(Game1.temporaryContent.Load<Texture2D>("Maps/Mines/mine_desert"),
            new Rectangle(224, 160, 16, 16), this.GetPosition(1))
        {
            CheckHidden = () => !(config.ShowShaftInfo && this.GetBuildingLayerInfo(174))
        };
        var monsterHud = new CombineImage(Game1.temporaryContent.Load<Texture2D>("Characters/Monsters/Green Slime"),
            new Rectangle(2, 268, 12, 10), this.GetPosition(2))
        {
            CheckHidden = () => !(config.ShowMonsterInfo && this.GetMonsters().Any()),
            OnHover = (_, spriteBatch) =>
            {
                if (!this.hasGetMonsterInfo)
                {
                    this.GetMonsterInfo();
                    this.hasGetMonsterInfo = true;
                }

                var monsterInfoString = this.GetStringFromDictionary(this.monsterInfo);
                IClickableMenu.drawHoverText(spriteBatch, monsterInfoString, Game1.smallFont);
            },
            OffHover = _ => this.hasGetMonsterInfo = false,
            OnLeftClick = () => { helper.Reflection.GetMethod(new AdventureGuild(), nameof(AdventureGuild.showMonsterKillList)).Invoke(); }
        };
        var mineralHud = new CombineImage(Game1.temporaryContent.Load<Texture2D>("TileSheets/tools"),
            new Rectangle(193, 128, 15, 15), this.GetPosition(3))
        {
            CheckHidden = () => !(config.ShowMineralInfo && this.GetMinerals().Any()),
            OnHover = (_, spriteBatch) =>
            {
                if (!this.hasGetMineralInfo)
                {
                    this.GetMineralInfo();
                    this.hasGetMineralInfo = true;
                }

                var mineralInfoString = this.GetStringFromDictionary(this.mineralInfo);
                IClickableMenu.drawHoverText(spriteBatch, mineralInfoString, Game1.smallFont);
            },
            OffHover = _ => this.hasGetMineralInfo = false
        };

        this.hud.AddChild(ladderHud, shaftHud, monsterHud, mineralHud);
    }

    public void Update()
    {
        if (Game1.player.currentLocation is not MineShaft or VolcanoDungeon) return;

        var i = 0;
        foreach (var element in this.hud.Children.Where(element => !element.IsHidden())) element.LocalPosition = this.GetPosition(i++);

        this.hud.Update();
        this.hud.ReceiveLeftClick();
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (Game1.player.currentLocation is not MineShaft or VolcanoDungeon) return;

        this.hud.Draw(spriteBatch);
        this.hud.PerformHoverAction(spriteBatch);
    }

    private Vector2 GetPosition(int index)
    {
        return new Vector2(0, 88 + 72 * index);
    }

    private bool GetBuildingLayerInfo(int targetIndex)
    {
        var location = Game1.currentLocation;
        var buildingLayer = location.Map.GetLayer("Buildings");
        for (var i = 0; i < buildingLayer.LayerWidth; i++)
        {
            for (var j = 0; j < buildingLayer.LayerHeight; j++)
            {
                var index = location.getTileIndexAt(i, j, "Buildings");
                if (index == targetIndex) return true;
            }
        }

        return false;
    }

    private List<Monster> GetMonsters()
    {
        var location = Game1.currentLocation;
        var monsters = location.characters.OfType<Monster>().ToList();
        return monsters;
    }

    private void GetMonsterInfo()
    {
        var monsters = this.GetMonsters();
        this.monsterInfo.Clear();
        foreach (var monster in monsters)
        {
            if (!this.monsterInfo.TryAdd(monster.displayName, 1)) this.monsterInfo[monster.displayName]++;
        }
    }

    private void GetMineralInfo()
    {
        var minerals = this.GetMinerals();
        this.mineralInfo.Clear();
        foreach (var mineral in minerals)
        {
            if (!this.mineralInfo.TryAdd(mineral.DisplayName, 1)) this.mineralInfo[mineral.DisplayName]++;
        }
    }

    private List<SObject> GetMinerals()
    {
        var location = Game1.currentLocation;
        var minerals = location.Objects.Values.ToList();
        return minerals;
    }

    private string GetStringFromDictionary(Dictionary<string, int> dictionary)
    {
        var stringBuilder = new StringBuilder();
        foreach (var (key, value) in dictionary)
            stringBuilder.AppendLine($"{key}: {value}");
        stringBuilder.Remove(stringBuilder.Length - 1, 1);

        return stringBuilder.ToString();
    }
}