/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sophiesalacia/StardewMods
**
*************************************************/

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;

namespace CarWarp;

class CarWarp
{
    private static Dictionary<string, WarpLocationModel> WarpLocations;

    private readonly Building Car;
    private readonly Vector2 CarDriversSeat;

    private const float LeftSideOffset = 1f;
    private const float RightSideOffset = 2f;

    public CarWarp(Building car)
    {
        Car = car;

        float driversSeatOffset = GetDriversSeatOffset();
        // get drivers seat of car into Activate function
        CarDriversSeat = new Vector2(
                x: (car.tileX.Value + driversSeatOffset) * 64f,
                y: (car.tileY.Value + 4.75f) * 64f
            );
    }

    public CarWarp()
    {
        Car = null;
        CarDriversSeat = Game1.player.Position;
    }

    /// <summary>
    /// Creates the car warp question dialogue.
    /// </summary>
    public void Activate()
    {
        // invalidate and reload warp locations asset so that it is up-to-date
        Globals.GameContent.InvalidateCache(Globals.WarpLocationsContentPath);
        WarpLocations = Globals.GameContent.Load<Dictionary<string, WarpLocationModel>>(Globals.WarpLocationsContentPath);

        // build list of destination responses
        List<Response> responses = new();
        foreach ((string key, WarpLocationModel warpLoc) in WarpLocations)
        {
            // validate destination before adding response
            if (IsValidLocation(warpLoc.Location))
            {
                // add response keyed to the warp location model key
                responses.Add(new Response(key, warpLoc.DisplayName));
            }
        }

        // add cancel option
        responses.Add(new Response("Cancel", Game1.content.LoadString("Strings\\Locations:MineCart_Destination_Cancel")));

        // create question dialogue
        Game1.currentLocation.createQuestionDialogue("", responses.ToArray(), AnswerCarWarp);
    }

    /// <summary>
    /// Handles the response to the car warp question dialogue.
    /// </summary>
    private void AnswerCarWarp(Farmer who, string answer)
    {
        // back out if selected answer is Cancel
        if (answer == "Cancel")
            return;

        // use key to get warp location details from WarpLocations
        WarpLocationModel selectedWarp = WarpLocations[answer];

        StartCarWarp(selectedWarp);

    }

    private void StartCarWarp(WarpLocationModel warp)
    {
        // put player in car
        Game1.player.Position = CarDriversSeat;
        Game1.player.faceDirection(2);

        // freeze player in place
        Game1.player.freezePause = 3000;

        // play door open sound
        Game1.currentLocation.playSound("doorClose");

        // slightly randomize engine pitch
        int enginePitch = Game1.random.Next(1000, 2000);

        // play engine starting up sound after 1 second
        DelayedAction.playSoundAfterDelay("busDriveOff", 1000, Game1.currentLocation, enginePitch);

        // 5% chance to "honk" horn
        if (Game1.random.Next(99) < 5)
        {
            DelayedAction.playSoundAfterDelay("Duck", 3000, Game1.currentLocation, enginePitch * 8);
            DelayedAction.playSoundAfterDelay("Duck", 3250, Game1.currentLocation, enginePitch * 8);
        }

        // initiate warp after roughly 3 seconds
        // the lower the engine pitch, the longer the sound is drawn out, so we delay the warp a little to compensate
        // otherwise the sound lingers unpleasantly on the new map
        DelayedAction.functionAfterDelay(
                delegate
                    {
                        LocationRequest locationRequest = Game1.getLocationRequest(warp.Location);
                        Game1.warpFarmer(locationRequest, warp.X, warp.Y, 2);
                    },
                3300 - (enginePitch / 10)
            );
    }

    /// <summary>
    /// Checks to make sure provided location name is a valid location in the game.
    /// </summary>
    private static bool IsValidLocation(string loc)
    {
        return Game1.getLocationFromName(loc) is not null;
    }

    private float GetDriversSeatOffset()
    {
        // do not adjust position if there is no car
        if (Car is null)
            return 0f;

        return Globals.Config.Configuration switch
        {
            "Right" => RightSideOffset,
            "Left" => LeftSideOffset,
            "None" => GetClosestSide(),
            "Empty" => GetClosestSide(),
            _ => RightSideOffset,
        };
    }

    private float GetClosestSide()
    {
        float playerX = Game1.player.Position.X;
        float carX = Car.tileX.Value * 64f;
        return playerX < carX ? 1f : 2f;
    }
}
