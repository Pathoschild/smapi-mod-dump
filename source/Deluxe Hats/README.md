# Deluxe Hats

Deluxe Hats is a mod for Stardew Valley that brings unique mechanics to hats. The goal behind this mod is to give the player a better sense of achievement when they spend possibly hours to obtain a particular hat.  Without this mod, hats only serve a purpose as a cosmetic item. With this mod, hats will have unique effects that alter gameplay, making them a valuable asset to any player.

## How to Contribute

Deluxe Hats is currently under development. All contributions would be much appreciated. You can contribute by:

- Sharing an idea for a [hat](https://github.com/domsim1/stardew-valley-deluxe-hats-mod/issues/1).
- Implement a hat or fix a bug.
- Report an [issue](https://github.com/domsim1/stardew-valley-deluxe-hats-mod/issues).
- Buy me a [coffee](https://www.buymeacoffee.com/domsim1) ☕.

## How to Implement a Hat or fix a bug

Create a fork of the repository, then create a branch based on `development` called `feature/<hat-name>` if its a hat effect or `bugfix/<issue-number>` if its a bug fix. If the branch you are developing on is behind `development`, please rebase, do **not** merge `development` into your branch.

The project can be opened with Visual Studio, its built using [SMAPI](https://stardewvalleywiki.com/Modding:Modder_Guide/APIs). To implement a hat, find the file with the same name as the hat you would like to add an effect to in the `Hats` directory. The file will look like so:

```C#
using System;

namespace BetterHats.Hats
{
  public static class BowlerHat
  {
    public const string Name = "Bowler Hat";
    public static void Activate()
    {
      throw new NotImplementedException();
    }

    public static void Disable()
    {
      throw new NotImplementedException();
    }
  }
}
```

The `Activate()` method is called when the player puts on the hat, the `Disable()` method is called when the hat takes off the hat. Logic to apply effects should go within the `Active()` method and clean up should be done within the `Disable()` method. If no clean up is needed to leave the `Disable()` empty.

To access SMAPI `Mod.Helper` use `HatService.Helper` and `Mod.Monitor` use `HatService.Monitor`.

Do **not** use events from SMAPI directly from the `HatSevice.Helper`. Instead, bind a lambda function the corresponding delegate variable. For example, if you wish to use `Helper.Events.GameLoop.UpdateTicked` assign a lambda function to `HatService.OnUpdateTicked` like so:

```C#
public static void Activate()
{
  HatService.OnUpdateTicked = (e) =>
  {
    // Do stuff
  }
}

```

The `HatService.OnTick` will be set `null` automaticlly when the hat is taken off. This does **not** need to be done within the `Disable()` method.

If the event you wish to use does not yet have a delegate you can add it to the `HatService` class like so:

```C#
// Declare the delegate
public delegate void OnUpdateTickedDelegate(UpdateTickedEventArgs e);
public static OnUpdateTickedDelegate OnUpdateTicked;

// Set the variable to null in CleanUp() method
public static void CleanUp()
{
  DisableHat?.Invoke();
  DisableHat = null;
  OnUpdateTicked = null; // like this
}

// Create a method that will be called by the event
// and try to envoke the delegate method
public static void UpdateTicked(object sender, UpdateTickedEventArgs e)
{
  OnUpdateTicked?.Invoke(e);
}

```

Lastly you will need to bind the event to the method in the `ModEntry` class like so:

```C#
public override void Entry(IModHelper helper)
{
  HatService.Monitor = Monitor;
  HatService.Helper = helper;
  helper.Events.GameLoop.SaveLoaded += SaveLoaded;
  helper.Events.GameLoop.DayStarted += HatService.DayStarted;
  helper.Events.GameLoop.ReturnedToTitle += ReturnedToTitle;
  helper.Events.GameLoop.UpdateTicked += HatService.UpdateTicked; // like this
}
```

## Help

If you know the basics C# and would like help implementing a hat feature, open an issue with the `help wanted` tag explaining what you are trying to accomplish and I (or maybe someone in the community) will be happy to help.
