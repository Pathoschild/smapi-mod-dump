/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/zunderscore/StardewWebApi
**
*************************************************/

using StardewValley;
using StardewValley.Objects;
using StardewWebApi.Game.Items;

namespace StardewWebApi.Game.Players;

public record PlayerClothing(
    BasicItem? Hat,
    BasicItem? Shirt,
    BasicItem? Pants,
    BasicItem? Boots,
    BasicItem? RightRing,
    BasicItem? LeftRing
);

public class Player
{
    private readonly Farmer _player;

    private Player(Farmer player)
    {
        _player = player;
    }

    public static Player FromMain()
    {
        return new(Game1.player);
    }

    public static Player FromFarmer(Farmer farmer)
    {
        return new(farmer);
    }

    public string Name => _player.Name;
    public string DisplayName => _player.displayName;
    public string FarmName => _player.farmName.Value;
    public int Money => _player.Money;
    public float Stamina => _player.Stamina;
    public int MaxStamina => _player.MaxStamina;
    public int Health => _player.health;
    public int MaxHealth => _player.maxHealth;
    public string Location => _player.currentLocation.Name;
    public double DailyLuck => _player.DailyLuck;
    public string DailyLuckFriendlyValue => DailyLuck.ToString("P");
    public string DailyLuckDescription => new TV().getFortuneForecast(_player);

    public PlayerClothing Clothing => new(
        BasicItem.FromItem(_player.hat.Value),
        BasicItem.FromItem(_player.shirtItem.Value),
        BasicItem.FromItem(_player.pantsItem.Value),
        BasicItem.FromItem(_player.boots.Value),
        BasicItem.FromItem(_player.rightRing.Value),
        BasicItem.FromItem(_player.leftRing.Value)
    );

    public IEnumerable<BasicSkill> Skills
    {
        get
        {
            for (int x = 0; x < 5; x++)
            {
                var level = _player.GetSkillLevel(x);
                var professionIds = new List<int>();

                if (level >= 5) professionIds.Add(_player.getProfessionForSkill(x, 5));
                if (level >= 10) professionIds.Add(_player.getProfessionForSkill(x, 10));

                yield return new(x, level, professionIds);
            }
        }
    }

    public IEnumerable<Relationship> Relationships => _player.friendshipData.Keys
        .Select(f => Relationship.FromFriendshipData(f, _player.friendshipData[f]));
}