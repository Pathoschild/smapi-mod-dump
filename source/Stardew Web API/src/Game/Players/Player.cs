/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/zunderscore/StardewWebApi
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.Objects;
using StardewWebApi.Game.Animals;
using StardewWebApi.Game.Items;
using StardewWebApi.Server;

namespace StardewWebApi.Game.Players;

public enum WarpLocation
{
    Random = 0,
    Farm,
    Beach,
    Mountain,
    Desert,
    Island,
    FrontDoor
}

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

    public static Player Main => new(Game1.player);

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

    public IEnumerable<Quest> Quests => _player.questLog
        .Select(q => new Quest(q));

    public IEnumerable<PlayerAchievement> Achievements => Game1.achievements
        .Select(a => new PlayerAchievement(a.Key, _player));

    public IEnumerable<Relationship> Relationships => _player.friendshipData.Keys
        .Where(f => f != "Henchman")
        .Select(f => Relationship.FromFriendshipData(f, _player.friendshipData[f]));

    public IEnumerable<BasicItem> GetInventory() => _player.Items
        .Select(i => BasicItem.FromItem(i)!);

    public void RefillEnergy()
    {
        _player.Stamina = _player.MaxStamina;
    }

    public void PassOut()
    {
        _player.Stamina = -16;
    }

    public void FullyHeal()
    {
        _player.health = _player.maxHealth;
    }

    public void KnockOut()
    {
        _player.health = 0;
    }

    public void GiveMoney(int amount)
    {
        _player.addUnearnedMoney(amount);
    }

    private WarpLocation GetNewWarpLocation()
    {
        var locations = new List<WarpLocation>() {
            WarpLocation.Farm,
            WarpLocation.Beach,
            WarpLocation.Mountain
        };

        if (_player.mailReceived.Contains("ccVault"))
        {
            locations.Add(WarpLocation.Desert);
        }

        if (_player.mailReceived.Contains("willyBoatFixed"))
        {
            locations.Add(WarpLocation.Island);
        }

        return locations[Random.Shared.Next(0, locations.Count)];
    }

    public ActionResult WarpPlayer(WarpLocation location = WarpLocation.Random, bool playWarpAnimation = true)
    {
        var result = new ActionResult(true);

        if (location == WarpLocation.Random)
        {
            location = GetNewWarpLocation();
        }

        try
        {
            if ((location == WarpLocation.Desert && !_player.mailReceived.Contains("ccVault") && !_player.mailReceived.Contains("jojaVault"))
                || (location == WarpLocation.Island && !_player.mailReceived.Contains("willyBoatFixed"))
            )
            {
                throw new Exception("Can't warp to that location yet!");
            }

            void warpDelegate()
            {
                switch (location)
                {
                    case WarpLocation.Farm:
                        Game1.warpFarmer("Farm", 48, 7, false);
                        break;

                    case WarpLocation.Beach:
                        Game1.warpFarmer("Beach", 20, 4, false);
                        break;

                    case WarpLocation.Mountain:
                        Game1.warpFarmer("Mountain", 31, 20, false);
                        break;

                    case WarpLocation.Desert:
                        Game1.warpFarmer("Desert", 35, 43, false);
                        break;

                    case WarpLocation.Island:
                        Game1.warpFarmer("IslandSouth", 11, 11, false);
                        break;

                    case WarpLocation.FrontDoor:
                    default:
                        var frontDoorCoords = Utility.getHomeOfFarmer(_player).getFrontDoorSpot();
                        Game1.warpFarmer("Farm", frontDoorCoords.X, frontDoorCoords.Y, false);
                        break;
                }

                if (playWarpAnimation)
                {
                    Game1.fadeToBlackAlpha = 0.99f;
                    Game1.screenGlow = false;
                    _player.temporarilyInvincible = false;
                    _player.temporaryInvincibilityTimer = 0;
                    Game1.displayFarmer = true;
                    _player.CanMove = true;
                }
            }

            // Replicate the animation that the totems/wand plays
            if (playWarpAnimation)
            {
                for (int i = 0; i < 12; i++)
                {
                    Game1.Multiplayer.broadcastSprites(_player.currentLocation, new TemporaryAnimatedSprite(354, Game1.random.Next(25, 75), 6, 1, new Vector2(Game1.random.Next((int)_player.position.X - 256, (int)_player.position.X + 192), Game1.random.Next((int)_player.position.Y - 256, (int)_player.position.Y + 192)), flicker: false, Game1.random.NextBool()));
                }
                _player.playNearbySoundAll("wand");
                Game1.displayFarmer = false;
                _player.temporarilyInvincible = true;
                _player.temporaryInvincibilityTimer = -2000;
                _player.Halt();
                _player.faceDirection(2);
                _player.CanMove = false;
                _player.freezePause = 2000;
                Game1.flashAlpha = 1f;
                DelayedAction.fadeAfterDelay(warpDelegate, 1000);
                Rectangle boundingBox = _player.GetBoundingBox();
                new Rectangle(boundingBox.X, boundingBox.Y, 64, 64).Inflate(192, 192);
                int num = 0;
                Point tilePoint = _player.TilePoint;
                for (int num2 = tilePoint.X + 8; num2 >= tilePoint.X - 8; num2--)
                {
                    Game1.Multiplayer.broadcastSprites(_player.currentLocation, new TemporaryAnimatedSprite(6, new Vector2(num2, tilePoint.Y) * 64f, Color.White, 8, flipped: false, 50f)
                    {
                        layerDepth = 1f,
                        delayBeforeAnimationStart = num * 25,
                        motion = new Vector2(-0.25f, 0f)
                    });
                    num++;
                }
            }
            else
            {
                _player.resetState();
                warpDelegate();
            }
        }
        catch (Exception ex)
        {
            SMAPIWrapper.LogError($"Error warping player: {ex.Message}");
            result = new(false, new ErrorResponse(ex.Message));
        }

        return result;
    }

    public ActionResult PetFarmAnimal(string animalName)
    {
        var result = new ActionResult(true);
        var animal = AnimalUtilities.GetFarmAnimalByName(animalName);

        if (animal is not null)
        {
            if (animal.wasAutoPet.Value)
            {
                result = new(false, new ErrorResponse($"{animal.Name} was already auto-pet today"));
            }
            else if (animal.wasPet.Value)
            {
                result = new(false, new ErrorResponse($"{animal.Name} was already pet today"));
            }
            else
            {
                _player.FarmerSprite.PauseForSingleAnimation = false;
                animal.pet(_player);
            }
        }
        else
        {
            result = new(false, new ErrorResponse($"Could not find a farm animal named {animalName}"));
        }

        return result;
    }

    public void GiveItem(Item item)
    {
        _player.addItemByMenuIfNecessary(item);
    }

    public void GiveItems(IEnumerable<Item> items)
    {
        _player.addItemsByMenuIfNecessary(new List<Item>(items));
    }
}