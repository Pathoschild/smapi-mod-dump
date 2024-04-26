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
using StardewWebApi.Game.Animals;
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

public static class PlayerActions
{
    public static void RefillEnergy()
    {
        Game1.player.Stamina = Game1.player.MaxStamina;
    }

    public static void PassOut()
    {
        Game1.player.Stamina = -16;
    }

    public static void FullyHeal()
    {
        Game1.player.health = Game1.player.maxHealth;
    }

    public static void KnockOut()
    {
        Game1.player.health = 0;
    }

    public static void GiveMoney(int amount)
    {
        Game1.player.addUnearnedMoney(amount);
    }

    private static WarpLocation GetNewWarpLocation()
    {
        var locations = new List<WarpLocation>() {
            WarpLocation.Farm,
            WarpLocation.Beach,
            WarpLocation.Mountain
        };

        if (Game1.player.mailReceived.Contains("ccVault"))
        {
            locations.Add(WarpLocation.Desert);
        }

        if (Game1.player.mailReceived.Contains("willyBoatFixed"))
        {
            locations.Add(WarpLocation.Island);
        }

        return locations[Random.Shared.Next(0, locations.Count)];
    }

    public static ActionResult WarpPlayer(WarpLocation location = WarpLocation.Random, bool playWarpAnimation = true)
    {
        var result = new ActionResult(true);

        if (location == WarpLocation.Random)
        {
            location = GetNewWarpLocation();
        }

        try
        {
            if ((location == WarpLocation.Desert && !Game1.player.mailReceived.Contains("ccVault") && !Game1.player.mailReceived.Contains("jojaVault"))
                || (location == WarpLocation.Island && !Game1.player.mailReceived.Contains("willyBoatFixed"))
            )
            {
                throw new Exception("Can't warp to that location yet!");
            }

            var who = Game1.player;

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
                        var frontDoorCoords = Utility.getHomeOfFarmer(Game1.player).getFrontDoorSpot();
                        Game1.warpFarmer("Farm", frontDoorCoords.X, frontDoorCoords.Y, false);
                        break;
                }

                if (playWarpAnimation)
                {
                    Game1.fadeToBlackAlpha = 0.99f;
                    Game1.screenGlow = false;
                    who.temporarilyInvincible = false;
                    who.temporaryInvincibilityTimer = 0;
                    Game1.displayFarmer = true;
                    who.CanMove = true;
                }
            }

            // Replicate the animation that the totems/wand plays
            if (playWarpAnimation)
            {
                for (int i = 0; i < 12; i++)
                {
                    Game1.Multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite(354, Game1.random.Next(25, 75), 6, 1, new Vector2(Game1.random.Next((int)who.position.X - 256, (int)who.position.X + 192), Game1.random.Next((int)who.position.Y - 256, (int)who.position.Y + 192)), flicker: false, Game1.random.NextBool()));
                }
                who.playNearbySoundAll("wand");
                Game1.displayFarmer = false;
                who.temporarilyInvincible = true;
                who.temporaryInvincibilityTimer = -2000;
                who.Halt();
                who.faceDirection(2);
                who.CanMove = false;
                who.freezePause = 2000;
                Game1.flashAlpha = 1f;
                DelayedAction.fadeAfterDelay(warpDelegate, 1000);
                Rectangle boundingBox = who.GetBoundingBox();
                new Rectangle(boundingBox.X, boundingBox.Y, 64, 64).Inflate(192, 192);
                int num = 0;
                Point tilePoint = who.TilePoint;
                for (int num2 = tilePoint.X + 8; num2 >= tilePoint.X - 8; num2--)
                {
                    Game1.Multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite(6, new Vector2(num2, tilePoint.Y) * 64f, Color.White, 8, flipped: false, 50f)
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
                Game1.player.resetState();
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

    public static ActionResult PetFarmAnimal(string animalName)
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
                Game1.player.FarmerSprite.PauseForSingleAnimation = false;
                animal.pet(Game1.player);
            }
        }
        else
        {
            result = new(false, new ErrorResponse($"Could not find a farm animal named {animalName}"));
        }

        return result;
    }

    public static void GiveItem(Item item)
    {
        Game1.player.addItemByMenuIfNecessary(item);
    }

    public static void GiveItems(IEnumerable<Item> items)
    {
        Game1.player.addItemsByMenuIfNecessary(new List<Item>(items));
    }
}