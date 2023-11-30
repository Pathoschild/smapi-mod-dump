/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using System.Threading.Tasks;
using BirbCore.Attributes;
using StardewModdingAPI;
using StardewValley;

namespace LookToTheSky;

[SEvent]
internal class Events
{
    [SEvent.DayEnding]
    private void GameLoop_DayEnding(object sender, StardewModdingAPI.Events.DayEndingEventArgs e)
    {
        ModEntry.Instance.SkyObjects.Clear();
        ModEntry.Instance.Projectiles.Clear();
    }

    // TODO: make a factory or something...
    [SEvent.OneSecondUpdateTicked]
    private void GameLoop_OneSecondUpdateTicked(object sender, StardewModdingAPI.Events.OneSecondUpdateTickedEventArgs e)
    {
        if (!Context.CanPlayerMove && Game1.activeClickableMenu is not SkyMenu)
        {
            return;
        }
        if (Game1.random.Next(100) < ModEntry.Config.SpawnChancePerSecond)
        {
            int rand = Game1.random.Next(100);
            if (rand <= 10)
            {
                if (Game1.timeOfDay >= 1800)
                {
                    if (Game1.currentSeason == "winter" && Game1.dayOfMonth == 25)
                    {
                        ModEntry.Instance.SkyObjects.Add(new Santa(rand % 5 * 10, rand % 2 == 0));
                    }
                    else
                    {
                        ModEntry.Instance.SkyObjects.Add(new UFO(rand % 5 * 10, rand % 2 == 0));
                    }
                }
                else
                {
                    ModEntry.Instance.SkyObjects.Add(new Plane(rand % 5 * 10, rand % 2 == 0));
                }
            }
            else if (rand <= 20)
            {
                if (Game1.timeOfDay >= 1800)
                {
                    ModEntry.Instance.SkyObjects.Add(new Witch(rand % 5 * 10, rand % 2 == 0));
                }
                else
                {
                    ModEntry.Instance.SkyObjects.Add(new Bird(20, rand % 2 == 0));
                    Task.Delay(2000).ContinueWith((t) =>
                    {
                        ModEntry.Instance.SkyObjects.Add(new Bird(25, rand % 2 == 0));
                        ModEntry.Instance.SkyObjects.Add(new Bird(15, rand % 2 == 0));
                    });
                    Task.Delay(4000).ContinueWith((t) =>
                    {
                        ModEntry.Instance.SkyObjects.Add(new Bird(30, rand % 2 == 0));
                        ModEntry.Instance.SkyObjects.Add(new Bird(10, rand % 2 == 0));
                    });
                }
            }
            else if (rand <= 30 && Game1.player.eventsSeen.Contains("10"))
            {
                ModEntry.Instance.SkyObjects.Add(new Robot(rand % 5 * 10, rand % 2 == 0));
            }
            else
            {
                ModEntry.Instance.SkyObjects.Add(new Bird(rand, rand % 2 == 0));
            }
        }
    }

    [SEvent.UpdateTicked]
    private void GameLoop_UpdateTicked(object sender, StardewModdingAPI.Events.UpdateTickedEventArgs e)
    {
        for (int i = ModEntry.Instance.SkyObjects.Count - 1; i >= 0; i--)
        {
            if (ModEntry.Instance.SkyObjects[i].X < -100 || ModEntry.Instance.SkyObjects[i].X > Game1.viewport.Width + 100 || ModEntry.Instance.SkyObjects[i].Y < -100 || ModEntry.Instance.SkyObjects[i].Y > Game1.viewport.Height + 100)
            {
                ModEntry.Instance.SkyObjects[i].OnExit();
                ModEntry.Instance.SkyObjects.RemoveAt(i);
            }
            else
            {
                ModEntry.Instance.SkyObjects[i].tick();
            }
        }
        for (int i = ModEntry.Instance.Projectiles.Count - 1; i >= 0; i--)
        {
            if (ModEntry.Instance.Projectiles[i].Y < 0)
            {
                ModEntry.Instance.Projectiles.RemoveAt(i);
            }
            else
            {
                if (ModEntry.Instance.Projectiles[i].UpdatePosition(Game1.currentGameTime))
                {
                    ModEntry.Instance.Projectiles.RemoveAt(i);
                }
            }
        }
    }

    // Open and close the sky menu
    [SEvent.ButtonPressed]
    private void Input_ButtonPressed(object sender, StardewModdingAPI.Events.ButtonPressedEventArgs e)
    {
        if (Context.IsWorldReady && Context.CanPlayerMove && Game1.currentLocation.IsOutdoors && e.Button.Equals(ModEntry.Config.Button))
        {
            if (Game1.activeClickableMenu is SkyMenu)
            {
                Game1.activeClickableMenu = null;
            }
            else
            {
                Game1.activeClickableMenu = new SkyMenu();
            }
        }
    }
}
